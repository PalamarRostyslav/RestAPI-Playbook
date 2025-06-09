using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IRatingService
    {
        Task<bool> RateMovieAsync(Guid moviedId, int rating, Guid? userId, CancellationToken cancellationToken = default);

        Task<bool> DeleteRatingAsync(Guid movieId, Guid? userId, CancellationToken token = default);

        Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid? userId, CancellationToken token = default);
    }
}
