using Application.Models;
using Core;
using SkyLearn.Portal.Api;
using System.Net;
using System.Text.Json;
using static SkyLearn.Portal.Api.Middleware.AuthorizedUser;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case AppException e:
                    // Custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // Not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ForbiddenException e:
                    response.StatusCode=(int)HttpStatusCode.Forbidden;
                    break;
                default:
                    // Unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            _logger.LogError(error, error.Message);
            var responseObject = new
            {
                StatusCode = (HttpStatusCode)response.StatusCode,
                Message = error.Message, // Include the error message in the response
                                         // Add any other properties you want to include in the response object
            };

            // Serialize the response object to JSON
            var result = JsonSerializer.Serialize(responseObject);

            // Write the JSON response to the response stream
            await response.WriteAsync(result);
            LogErrorToDatabase(error);
        }
    }
    private void LogErrorToDatabase(Exception error)
    {
        // Create a log entity and save it to the database
        var logEntry = new ErrorLog
        {
            Pid=UniqueIndexGenerator.GenerateUniqueAlphanumericIndex(6),
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ErrorMessage = error.Message,
            StackTrace = error.StackTrace // You may want to customize this based on your needs
        };
    }
}