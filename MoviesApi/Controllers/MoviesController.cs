namespace MovieApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;
    using MovieApi.Models;
    using MovieApi.Service;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly ILogger<MoviesController> _logger;
        public MoviesController(ILogger<MoviesController> logger, IMoviesService moviesService)
        {
            _logger = logger;
            _moviesService = moviesService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Movies>>> GetAllMovies()
        {
            _logger.LogInformation("Fetching all movies");
            var movies = await _moviesService.GetAllMoviesAsync();
            return Ok(movies);
        }
        [HttpGet("page/{page}/size/{pageSize}")]
        public async Task<ActionResult<List<Movies>>> MoviesByPagination(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                _logger.LogWarning($"Invalid page or page size: page={page}, pageSize={pageSize}");
                return BadRequest("Page and page size must be greater than 0.");
            }
            _logger.LogInformation($"Fetching movies for page {page} with page size {pageSize}");
            var movies = await _moviesService.GetAllMoviesAsync(page, pageSize);
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movies>> GetMovieById(ObjectId id)
        {
            _logger.LogInformation($"Fetching movie with ID: {id}");
            var movie = await _moviesService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }
        [HttpGet("title/{title}")]
        public async Task<ActionResult<List<Movies>>> GetMoviesByTitle(string title)
        {
            _logger.LogInformation($"Fetching movies with title: {title}");
            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("Title is null or empty");
                return BadRequest("Title cannot be null or empty.");
            }
            var movies = await _moviesService.GetMoviesByTitleAsync(title);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<List<Movies>>> GetMoviesByGenre(string genre)
        {
            _logger.LogInformation($"Fetching movies with genre: {genre}");
            if (string.IsNullOrWhiteSpace(genre))
            {
                _logger.LogWarning("Genre is null or empty");
                return BadRequest("Genre cannot be null or empty.");
            }
            var movies = await _moviesService.GetMoviesByGenreAsync(genre);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("year/{year}")]
        public async Task<ActionResult<List<Movies>>> GetMoviesByYear(int year)
        {
            _logger.LogInformation($"Fetching movies from year: {year}");
            if (year < 1888 || year > DateTime.UtcNow.Year)
            {
                _logger.LogWarning($"Invalid year: {year}");
                return BadRequest("Year must be between 1888 and the current year.");
            }
            var movies = await _moviesService.GetMoviesByYearAsync(year);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("director/{director}")]
        public async Task<ActionResult<List<Movies>>> GetMoviesByDirector(string director)
        {
            _logger.LogInformation($"Fetching movies directed by: {director}");
            if (string.IsNullOrWhiteSpace(director))
            {
                _logger.LogWarning("Director is null or empty");
                return BadRequest("Director cannot be null or empty.");
            }
            var movies = await _moviesService.GetMoviesByDirectorAsync(director);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("actor/{actor}")]
        public async Task<ActionResult<List<Movies>>> GetMoviesByActor(string actor)
        {
            _logger.LogInformation($"Fetching movies with actor: {actor}");
            if (string.IsNullOrWhiteSpace(actor))
            {
                _logger.LogWarning("Actor is null or empty");
                return BadRequest("Actor cannot be null or empty.");
            }
            var movies = await _moviesService.GetMoviesByActorAsync(actor);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("top-rated/{count}")]
        public async Task<ActionResult<List<Movies>>> GetTopRatedMovies(int count)
        {
            _logger.LogInformation($"Fetching top {count} rated movies");
            if (count < 1)
            {
                _logger.LogWarning($"Invalid count: {count}");
                return BadRequest("Count must be greater than 0.");
            }
            var movies = await _moviesService.GetTopRatedMoviesAsync(count);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpGet("searchByNameYear")]
        // uri=/api/movies/search?name=Inception?year=2010
        public async Task<ActionResult<Movies>> SearchMovies([FromQuery] string name, [FromQuery] int? year)
        {
            _logger.LogInformation($"Searching movies with name: {name} and year: {year}");
            if (string.IsNullOrWhiteSpace(name) && !year.HasValue)
            {
                _logger.LogWarning("Both name and year are null or empty");
                return BadRequest("At least one of name or year must be provided.");
            }
            var movies = await _moviesService.SearchMoviesAsync(name, year);
            if (movies == null)
            {
                _logger.LogWarning($"No movies found for name: {name} and year: {year}");
                return NotFound();
            }
            return Ok(movies);
        }


        [HttpGet("search")]
        //uri=/api/movies/search?query=Inception
        public async Task<ActionResult<List<Movies>>> SearchMovies([FromQuery] string query)
        {
            var movies = await _moviesService.SearchMoviesAsync(query);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }

        [HttpGet("sort/rating/{sortOrder}")]
        // uri=/api/movies/sort/rating/asc?page=1&pageSize=10
        public async Task<ActionResult<List<Movies>>> GetMoviesSortedByRating(string sortOrder, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Fetching movies sorted by rating in {sortOrder} order");
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                _logger.LogWarning("Sort order is null or empty");
                return BadRequest("Sort order cannot be null or empty.");
            }
            if (sortOrder != "asc" && sortOrder != "desc")
            {
                _logger.LogWarning($"Invalid sort order: {sortOrder}");
                return BadRequest("Sort order must be either 'asc' or 'desc'.");
            }

            var movies = await _moviesService.GetMoviesSortedByRatingAsync(sortOrder, page, pageSize);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }

        [HttpGet("sort/released/{sortOrder}")]
        // uri=/api/movies/sort/released/asc?page=1&pageSize=10
        public async Task<ActionResult<List<Movies>>> GetMoviesSortedByReleased(string sortOrder, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Fetching movies sorted by released date in {sortOrder} order");
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                _logger.LogWarning("Sort order is null or empty");
                return BadRequest("Sort order cannot be null or empty.");
            }
            if (sortOrder != "asc" && sortOrder != "desc")
            {
                _logger.LogWarning($"Invalid sort order: {sortOrder}");
                return BadRequest("Sort order must be either 'asc' or 'desc'.");
            }

            var movies = await _moviesService.GetMoviesSortedByReleased(sortOrder, page, pageSize);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }

        [HttpGet("sort/title/{sortOrder}")]
        // uri=/api/movies/sort/title/asc?page=1&pageSize=10
        public async Task<ActionResult<List<Movies>>> GetMoviesSortedByTitle(string sortOrder, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Fetching movies sorted by title in {sortOrder} order");
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                _logger.LogWarning("Sort order is null or empty");
                return BadRequest("Sort order cannot be null or empty.");
            }
            if (sortOrder != "asc" && sortOrder != "desc")
            {
                _logger.LogWarning($"Invalid sort order: {sortOrder}");
                return BadRequest("Sort order must be either 'asc' or 'desc'.");
            }

            var movies = await _moviesService.GetMoviesSortedByTitle(sortOrder, page, pageSize);
            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }
            return Ok(movies);
        }


    }
}