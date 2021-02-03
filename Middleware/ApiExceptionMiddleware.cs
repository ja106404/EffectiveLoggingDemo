using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiExceptionMiddleware
{
    private readonly ApiExceptionOptions _options;
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _options = options;
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {            
            using (_logger.SetTenantScope("AllState"))
            {
                await _next(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _options);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, ApiExceptionOptions options)
    {
        var error = new ApiError
        {
            Id = Guid.NewGuid().ToString(),
            Status = (short)HttpStatusCode.InternalServerError,
            Title = $"An error occurred. Please contact support team and share the error Id"
        };

        options.AddResponseDetails?.Invoke(context, exception, error);

        var innerExMessage = GetInnermostExceptionMessage(exception);
        var result = JsonSerializer.Serialize(error);
        //_logger.LogError(LoggingEvents.Error, exception, innerExMessage + " -- {ErrorId}", error.Id);
        _logger.LogException(innerExMessage, error.Id, exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(result);
    }

    private string GetInnermostExceptionMessage(Exception exception)
    {
        if (exception.InnerException != null)
            return GetInnermostExceptionMessage(exception.InnerException);

        return exception.Message;
    }
}