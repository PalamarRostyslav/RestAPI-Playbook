using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    internal class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _movieValidator = movieValidator ?? throw new ArgumentNullException(nameof(movieValidator));
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);

            return await _movieRepository.CreateAsync(movie, token);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _movieRepository.DeleteByIdAsync(id, token);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            return await _movieRepository.GetAllAsync(token);
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _movieRepository.GetByIdAsync(id, token);
        }

        public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            return await _movieRepository.GetBySlugAsync(slug, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);

            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);

            if (!movieExists)
            {
                return null;
            }

            return await _movieRepository.UpdateAsync(movie) ? movie : null;
        }
    }
}
