using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.DI
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAplication(this IServiceCollection services)
        {
            services.AddSingleton<IMovieRepository, MovieRepository>();
            services.AddSingleton<IMovieService, MovieService>();

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
