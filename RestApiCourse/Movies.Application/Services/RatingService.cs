using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
            _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid? userId, CancellationToken cancellationToken = default)
        {
            if (rating is <= 0 or > 5)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure
                    {
                        PropertyName = nameof(rating),
                        ErrorMessage = "Rating must be between 1 and 5."
                    }
                });
            }

            var movieExists = await _movieRepository.ExistsByIdAsync(movieId, cancellationToken);

            if (!movieExists)
            {
                return false;
            }

            return await _ratingRepository.RateMovieAsync(movieId, rating, userId, cancellationToken);
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default)
        {
            return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
        }

        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid? userId, CancellationToken token = default)
        {
            return await _ratingRepository.GetRatingsForUserAsync(userId, token);
        }
    }
}
