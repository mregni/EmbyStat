using System;
using System.IO;
using System.Threading.Tasks;
using EmbyStat.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Microsoft.Net.Http.Headers;

namespace EmbyStat.Controllers.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            await InvokeWithLogging(context);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task InvokeWithLogging(HttpContext context)
    {
        context.Request.EnableBuffering();
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);
      
        context.Request.Body.Seek(0, SeekOrigin.Begin);
        var requestText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Seek(0, SeekOrigin.Begin);
        
        var originalBodyStream = context.Response.Body;
        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        var requestStartedOn = DateTime.UtcNow;
        await _next(context);
        var requestEndedOn = DateTime.UtcNow;

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        
        _logger.LogDebug($"{Environment.NewLine}Http Request Information:{Environment.NewLine}" +
                      $"\tFull Path:\t| {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{Environment.NewLine}" +
                      $"\tStatus: \t| {context.Response.StatusCode}{Environment.NewLine}" +
                      $"\tUserAgent: \t| {context.Request.Headers[HeaderNames.UserAgent]}{Environment.NewLine}" +
                      $"\tAuthenticated: \t| {context.User.Identity?.IsAuthenticated.ToString() ?? "Unknown"}{Environment.NewLine}" +
                      $"\tTime: \t\t| {(requestEndedOn - requestStartedOn).TotalMilliseconds}ms{Environment.NewLine}" +
                      $"\tQueryString: \t| {context.Request.QueryString}{Environment.NewLine}" +
                      $"\tRequest Body: \t| {requestText}{Environment.NewLine}" +
                      $"\tResponse Body: \t| {responseText}");
        await responseBody.CopyToAsync(originalBodyStream);
    }
}