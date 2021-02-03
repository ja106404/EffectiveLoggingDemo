using System;
using Microsoft.Extensions.Logging;
public static class LoggerExtensions
{
    private static readonly Action<ILogger, Exception> _getMethodCalled;
    private static readonly Action<ILogger, string, Exception> _responseReturned;

    private static readonly Action<ILogger, string, string, Exception> _exceptionOccurred;

    private static readonly Func<ILogger, string, IDisposable> _setScope;

    static LoggerExtensions()
    {
        _getMethodCalled = LoggerMessage.Define(LogLevel.Information, LoggingEvents.MethodCall, "Get Method Called");
        _responseReturned = LoggerMessage.Define<string>(LogLevel.Trace, LoggingEvents.ResponseReturned, "Response: {Response}");
        _exceptionOccurred = LoggerMessage.Define<string, string>(LogLevel.Error, LoggingEvents.Error, "{InnerMessage} -- {ErrorId}");
        _setScope = LoggerMessage.DefineScope<string>("Tenant {TenantId}");
    }

    public static void LogCall(this ILogger logger) => _getMethodCalled(logger, null);

    public static void LogResponse(this ILogger logger, string response) => _responseReturned(logger, response, null);

    public static void LogException(this ILogger logger, string message, string errorId, Exception exception)
            => _exceptionOccurred(logger, message, errorId, exception);

    public static IDisposable SetTenantScope(this ILogger logger, string tenant) => _setScope(logger, tenant);
}