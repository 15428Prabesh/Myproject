namespace User.Identity.Herlpers;

using System.Net;
using System.Text.Json;

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
        }
    }



}