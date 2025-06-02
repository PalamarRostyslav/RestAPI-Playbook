using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories;

namespace Movies.Application.DI
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAplication(this IServiceCollection services)
        {
            services.AddSingleton<IMovieRepository,  MovieRepository>();

            return services;
        }
    }
}
