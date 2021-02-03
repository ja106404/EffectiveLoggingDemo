using System;
using Microsoft.AspNetCore.Http;

public class ApiExceptionOptions
{
    public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }
}