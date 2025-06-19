using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Auth;
using Movies.API.Health;
using Movies.API.Mapping;
using Movies.API.Swagger;
using Movies.Application.Database;
using Movies.Application.DI;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("healthchecks.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy(AuthConstants.AdminUserPolicyName, policy =>
    {
        policy.RequireClaim(AuthConstants.AdminUserClaimName, "true");
    });

    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, policy =>
    {
        policy.RequireAssertion(c =>
            c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }));
    });
});

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = ApiVersionReader.Combine(
                           new UrlSegmentApiVersionReader(),
                           new QueryStringApiVersionReader("api-version"),
                           new HeaderApiVersionReader("X-Version"),
                           new MediaTypeApiVersionReader("x-version"));
}).AddMvc(options =>
{
    options.Conventions.Add(new VersionByNamespaceConvention());

}).AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();

builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);
builder.Services.AddHostedService<HealthCheckLoggerService>();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => {
    x.OperationFilter<SwaggerDefaultValues>();
}); 
builder.Services.AddAplication();

var connectionString = config.GetSection("Database")["ConnectionString"];
builder.Services.AddDatabase(connectionString ?? throw new InvalidOperationException("Connection string 'Movies' not found."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            var groupName = description.GroupName;
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.MapHealthChecks("_health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        Log.Information("Health check endpoint called at {Time}. Status: {Status}", DateTimeOffset.Now, report.Status);

        foreach (var entry in report.Entries)
        {
            Log.Information(" - {Key}: {Status} ({Description})", entry.Key, entry.Value.Status, entry.Value.Description);
        }

        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { key = e.Key, value = e.Value.Status.ToString(), description = e.Value.Description })
        });

        await context.Response.WriteAsync(result);
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

await app.StartAsync();

foreach (var address in app.Urls)
{
    Log.Information("App is running now at {Address}", address);
    Console.WriteLine($"App is running now at {address}");
}

await app.WaitForShutdownAsync();