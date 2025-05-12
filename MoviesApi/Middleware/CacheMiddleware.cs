namespace MovieApi.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    using System.Diagnostics; // Added for Stopwatch
    using System; // Added for DateTimeOffset and TimeSpan

    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CacheMiddleware> _logger;
        private readonly string _cacheFolderPath;

        public CacheMiddleware(RequestDelegate next, ILogger<CacheMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _cacheFolderPath = Path.Combine(env.ContentRootPath, "FileCache");
            if (!Directory.Exists(_cacheFolderPath))
            {
                Directory.CreateDirectory(_cacheFolderPath);
            }
        }

        private string GetHashedCacheKey(string cacheKey)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(cacheKey));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew(); // Start timing
            var rawCacheKey = context.Request.Path.ToString() + context.Request.QueryString.ToString();
            var hashedCacheKey = GetHashedCacheKey(rawCacheKey);
            var cacheFilePath = Path.Combine(_cacheFolderPath, hashedCacheKey + ".cache");
            const int cacheExpiryMinutes = 1;

            if (File.Exists(cacheFilePath))
            {
                var lines = await File.ReadAllLinesAsync(cacheFilePath);
                if (lines.Length > 0 && DateTimeOffset.TryParse(lines[0], out var timestamp))
                {
                    if (DateTimeOffset.UtcNow - timestamp < TimeSpan.FromMinutes(cacheExpiryMinutes))
                    {
                        _logger.LogInformation($"Cache hit for {rawCacheKey} (file: {cacheFilePath})");
                        context.Response.ContentType = "application/json";
                        var cachedResponse = string.Join(Environment.NewLine, lines, 1, lines.Length - 1);
                        await context.Response.WriteAsync(cachedResponse);
                        stopwatch.Stop(); // Stop timing
                        _logger.LogInformation($"Request to {context.Request.Path} processed in {stopwatch.ElapsedMilliseconds}ms (from cache)");
                        return;
                    }
                    else
                    {
                        _logger.LogInformation($"Cache expired for {rawCacheKey} (file: {cacheFilePath})");
                        File.Delete(cacheFilePath);
                    }
                }
                else
                {
                    _logger.LogWarning($"Invalid cache file format for {rawCacheKey} (file: {cacheFilePath}). Deleting.");
                    File.Delete(cacheFilePath);
                }
            }

            _logger.LogInformation($"Cache miss for {rawCacheKey}");

            var originalBodyStream = context.Response.Body;
            using (var responseStream = new MemoryStream())
            {
                context.Response.Body = responseStream;

                await _next(context); // Process the request

                responseStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
                responseStream.Seek(0, SeekOrigin.Begin);

                var timestampLine = DateTimeOffset.UtcNow.ToString("o"); // ISO 8601 format
                var contentToCache = $"{timestampLine}{Environment.NewLine}{responseBody}";
                await File.WriteAllTextAsync(cacheFilePath, contentToCache);
                _logger.LogInformation($"Cache set for {rawCacheKey} (file: {cacheFilePath})");

                await responseStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
            stopwatch.Stop(); // Stop timing
            _logger.LogInformation($"Request to {context.Request.Path} processed in {stopwatch.ElapsedMilliseconds}ms (from origin)");
        }
    }
}