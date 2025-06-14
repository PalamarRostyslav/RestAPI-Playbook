﻿using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    internal class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<Movie> _movieValidator;
        private readonly IValidator<GetAllMoviesOptions> _getAllMoviesOptionsValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> getAllMoviesOptionsValidator)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _movieValidator = movieValidator ?? throw new ArgumentNullException(nameof(movieValidator));
            _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
            _getAllMoviesOptionsValidator = getAllMoviesOptionsValidator ?? throw new ArgumentNullException(nameof(ratingRepository));
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

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await _getAllMoviesOptionsValidator.ValidateAndThrowAsync(options, token);

            return await _movieRepository.GetAllAsync(options, token);
        }

        public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
        {
            return await _movieRepository.GetByIdAsync(id, userId, token);
        }

        public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default)
        {
            return await _movieRepository.GetBySlugAsync(slug, userId, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);

            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);

            if (!movieExists)
            {
                return null;
            }

            await _movieRepository.UpdateAsync(movie);

            if (!userId.HasValue)
            {
                var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
                movie.Rating = rating;
                return movie;
            }

            var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
           
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;

            return movie;
        }
    }
}
