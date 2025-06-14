using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators
{
    public class MoviewValidator : AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;

        public MoviewValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

            RuleFor(movie => movie.Id)
               .NotEmpty();

            RuleFor(movie => movie.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(movie => movie.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Release year should be in the past or current year");

            RuleFor(movie => movie.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("This movie already exists in the system");

            RuleFor(movie => movie.Genres)
                .NotEmpty().WithMessage("Genres are required to be provided.");
        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(slug);

            if (existingMovie is not null)
            {
               return existingMovie.Id == movie.Id;
            }

            return existingMovie is null;
        }
    }
}
