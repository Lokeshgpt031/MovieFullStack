using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Middleware;
using MovieApi.Models; // For MongoDbSettings
using MovieApi.Service;
// Ensure the "FileCache" folder is deleted on app initialization
var cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "FileCache");
if (Directory.Exists(cacheFolderPath))
{
    Directory.Delete(cacheFolderPath, true);
}


var builder = WebApplication.CreateBuilder(args);

SecretClientOptions options = new SecretClientOptions()
{
    Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
};
var client = new SecretClient(new Uri(builder.Configuration["KeyVault:Uri"] ?? throw new InvalidOperationException("KeyVault Uri is not configured")), new DefaultAzureCredential(), options);

KeyVaultSecret secret = client.GetSecret("MongoDB--ConnectionString");

string ConnectionString = secret.Value;
KeyVaultSecret databaseName = client.GetSecret("MongoDB--DatabaseName");
string DatabaseName = databaseName.Value;

// Configure services
builder.Services.Configure<MongoDbSettings>(
    options =>
    {
        options.ConnectionString = ConnectionString;
        options.DatabaseName = DatabaseName;
    });
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IMoviesService, MoviesService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(config =>
{
    config.AddPolicy("AllowAll", policy =>
    {
        // Allow all origins, methods, and headers
        // Note: In production, you should specify the allowed origins instead of using "*"
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure CORS is applied early in the pipeline
app.UseCors("AllowAll");

app.UseMiddleware<CacheMiddleware>();

// Define endpoints

app.UseHttpsRedirection();
// app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapGet("/error", () => Results.Problem("An error occurred"));


app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();
