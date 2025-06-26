using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

Thread.Sleep(5000);

var services = new ServiceCollection();

services
.AddHttpClient()
.AddSingleton<AuthTokenProvider>()
.AddRefitClient<IMoviesApi>(provider => new RefitSettings
{
    AuthorizationHeaderValueGetter = async (hrm, ct) => await provider.GetRequiredService<AuthTokenProvider>().GetTokenAsync(),
})
.ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:7269"));

var provider = services.BuildServiceProvider();

var moviesApiFromProvider = provider.GetRequiredService<IMoviesApi>();

var newMovie = await moviesApiFromProvider.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Test Movie Name 2024",
    YearOfRelease = 2024,
    Genres = ["Action", "Adventure"],
});

await moviesApiFromProvider.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
    Title = "Test Movie Name 2024 - V2",
    YearOfRelease = 2024,
    Genres = ["Action", "Adventure"],
});


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


Console.ReadKey();