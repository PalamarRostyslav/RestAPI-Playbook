using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.API.Mapping
{
    public class ValidationMappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMappingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var validationFailureResponse = new ValidationFailureResponse
                {
                    Errors = ex.Errors.Select(error => new ValidationResponse
                    {
                        PropertyName = error.PropertyName,
                        ErrorMessage = error.ErrorMessage
                    })
                };

                await context.Response.WriteAsJsonAsync(validationFailureResponse);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var response = new { Error = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
