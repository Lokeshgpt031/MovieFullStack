namespace MovieApi.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MovieApi.Models;

    public interface IMoviesService
    {
        Task<List<Movies>> GetAllMoviesAsync();
        // pagination
        Task<List<Movies>> GetAllMoviesAsync(int page, int pageSize);
        Task<Movies> GetMovieByIdAsync(ObjectId id);
        Task<List<Movies>> GetMoviesByTitleAsync(string title);
        Task<List<Movies>> GetMoviesByGenreAsync(string genre);
        Task<List<Movies>> GetMoviesByYearAsync(object year);
        Task<List<Movies>> GetMoviesByDirectorAsync(string director);
        Task<List<Movies>> GetMoviesByActorAsync(string actor);
        Task<List<Movies>> GetTopRatedMoviesAsync(int count);
        Task<List<Movies>> SearchMoviesAsync(string query);
        Task<List<Movies>> GetMoviesSortedByRatingAsync(string sortOrder,int page, int pageSize);
        Task<List<Movies>> GetMoviesSortedByTitle(string sortOrder, int page, int pageSize);
        Task<List<Movies>> GetMoviesSortedByReleased(string sortOrder, int page, int pageSize);
        Task<Movies> SearchMoviesAsync(string name, int? year);
    }
    public class MoviesService : IMoviesService
    {
        private readonly IMongoCollection<Movies> _moviesCollection;
        private readonly ILogger<MoviesService> _logger;
        public MoviesService(ILogger<MoviesService> logger, MongoDbService mongoDbService) // Changed from IMongoDatabase to MongoDbService
        {
            _logger = logger;
            _moviesCollection = mongoDbService.GetCollection<Movies>("movies");
            _logger.LogInformation("MoviesService initialized with collection: movies");
        }

        public async Task<List<Movies>> GetAllMoviesAsync()
        {
            var movies = await _moviesCollection.Find(_ => true).ToListAsync();
            return movies.ToList(); // Limit the number of movies returned to 20
        }

        public Task<List<Movies>> GetAllMoviesAsync(int page, int pageSize)
        {
            _logger.LogInformation($"Fetching movies for page {page} with page size {pageSize}");
            if (page < 1 || pageSize < 1)
            {
                _logger.LogError("Invalid page or page size");
                throw new ArgumentException("Page and page size must be greater than 0.");
            }
            return _moviesCollection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Movies> GetMovieByIdAsync(ObjectId id)
        {
            return await _moviesCollection.Find(movie => movie.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Movies>> GetMoviesByActorAsync(string actor)
        {
            return _moviesCollection.Find(movie => movie.Cast != null && movie.Cast.Contains(actor)).ToListAsync();
        }

        public async Task<List<Movies>> GetMoviesByDirectorAsync(string director)
        {
            return await _moviesCollection.Find(movie => movie.Directors != null && movie.Directors.Contains(director)).ToListAsync();
        }

        public async Task<List<Movies>> GetMoviesByGenreAsync(string genre)
        {
            return await _moviesCollection.Find(movie => movie.Genres != null && movie.Genres.Contains(genre)).ToListAsync();
        }

        public Task<List<Movies>> GetMoviesSortedByTitle(string sortOrder, int page, int pageSize)
        {
            var filter = Builders<Movies>.Filter.Where(movie => movie.Title != null);
            var sort = sortOrder == "desc" ? Builders<Movies>.Sort.Descending(movie => movie.Title) : Builders<Movies>.Sort.Ascending(movie => movie.Title);

            return _moviesCollection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public Task<List<Movies>> GetMoviesSortedByReleased(string sortOrder, int page, int pageSize)
        {
            var filter = Builders<Movies>.Filter.Where(movie => movie.Released != null);
            var sort = sortOrder == "desc" ? Builders<Movies>.Sort.Descending(movie => movie.Released) : Builders<Movies>.Sort.Ascending(movie => movie.Released);

            return _moviesCollection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Movies>> GetMoviesByTitleAsync(string title)
        {
            return await _moviesCollection.Find(movie => movie.Title != null && movie.Title.Contains(title)).ToListAsync();
        }

        public Task<List<Movies>> GetMoviesByYearAsync(object year) // Added missing method
        {
            return _moviesCollection.Find(movie => movie.Year == year).ToListAsync();
        }



        public Task<List<Movies>> GetMoviesSortedByRatingAsync(string sortOrder, int page, int pageSize)
        {
            
            var filter = Builders<Movies>.Filter.Where(movie => movie.Imdb != null && movie.Imdb.Rating != null);
            var sort = sortOrder == "desc" ? Builders<Movies>.Sort.Descending(movie => movie.Imdb.Rating) : Builders<Movies>.Sort.Ascending(movie => movie.Imdb.Rating);

            return _moviesCollection.Find(filter)
                .Sort(sort)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Movies>> GetTopRatedMoviesAsync(int count)
        {
            _logger.LogInformation($"Fetching top {count} rated movies");
            if (count < 1)
            {
                _logger.LogWarning($"Invalid count: {count}");
                throw new ArgumentException("Count must be greater than 0.");
            }
            var movies = await _moviesCollection.Find(_ => true).ToListAsync();
            if (movies == null || movies.Count == 0)
            {
                _logger.LogWarning("No movies found");
                return new List<Movies>();
            }
            return movies
                .Where(a => a.Imdb != null && a.Imdb.Rating != null && a.Imdb.Rating.ToString() != "")
                .OrderByDescending(a => a.Imdb?.Rating)
                .Take(count)
                .ToList();
       
        }

        public Task<List<Movies>> SearchMoviesAsync(string query)
        {
            var filter = Builders<Movies>.Filter.Or(
                Builders<Movies>.Filter.Where(movie => movie.Title != null && movie.Title.Contains(query)),
                Builders<Movies>.Filter.Where(movie => movie.Fullplot != null && movie.Fullplot.Contains(query)),
                Builders<Movies>.Filter.Where(movie => movie.Genres != null && movie.Genres.Contains(query)),
                Builders<Movies>.Filter.Where(movie => movie.Directors != null && movie.Directors.Contains(query)),
                Builders<Movies>.Filter.Where(movie => movie.Cast != null && movie.Cast.Contains(query))
            );

            return _moviesCollection.Find(filter).ToListAsync();
        }

        public Task<Movies> SearchMoviesAsync(string name, int? year)
        {
            var filter = Builders<Movies>.Filter.Eq(movie => movie.Title, name);
            if (year.HasValue)
            {
                filter &= Builders<Movies>.Filter.Eq(movie => movie.Year, year.Value);
            }
            return _moviesCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}