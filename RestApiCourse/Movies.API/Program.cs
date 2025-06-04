using Movies.Application.Database;
using Movies.Application.DI;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAplication();

var connectionString = config.GetSection("Database")["ConnectionString"];
builder.Services.AddDatabase(connectionString ?? throw new InvalidOperationException("Connection string 'Movies' not found."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
