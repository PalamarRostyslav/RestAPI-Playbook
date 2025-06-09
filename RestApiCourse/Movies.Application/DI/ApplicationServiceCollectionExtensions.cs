using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Application.Validators;
using FluentValidation;

namespace Movies.Application.DI
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAplication(this IServiceCollection services)
        {
            services.AddSingleton<IMovieRepository, MovieRepository>();
            services.AddSingleton<IRatingRepository, RatingRepository>();
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IRatingService, RatingService>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlDbConnectionFactory(connectionString));
            services.AddSingleton<DbInitializer>();

            return services;
        }
    }
}
