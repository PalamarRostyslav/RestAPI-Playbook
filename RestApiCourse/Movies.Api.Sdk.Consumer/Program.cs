using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Contracts.Requests;
using Refit;
using System.Text.Json;

Thread.Sleep(5000);

var services = new ServiceCollection();

services.AddRefitClient<IMoviesApi>().ConfigureHttpClient(x=>x.BaseAddress = new Uri("https://localhost:7269"));

var provider = services.BuildServiceProvider();

var moviesApiFromProvider = provider.GetRequiredService<IMoviesApi>();

var moviesResponse = await moviesApiFromProvider.GetMoviesAsync(new GetAllMoviesRequest
{
    Title = null,
    YearOfRelease = null,
    SortBy = null,
    Page = 1,
    PageSize = 3
});

foreach (var movie in moviesResponse.Items)
{
    Console.WriteLine($"Title: {movie.Title}, Year: {movie.YearOfRelease}, Slug: {movie.Slug}");
}
