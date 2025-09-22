//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System.Diagnostics;

//namespace EVStationRental.Common.Middleware.Custom
//{
//    public class CustomLoggingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<CustomLoggingMiddleware> _logger;

//        public CustomLoggingMiddleware(RequestDelegate next, ILogger<CustomLoggingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var stopwatch = Stopwatch.StartNew();
//            var requestTime = DateTime.UtcNow;

//            var originalBodyStream = context.Response.Body;
//            using var responseBody = new MemoryStream();
//            context.Response.Body = responseBody;

//            try
//            {
//                // Log request
//                var requestInfo = new
//                {
//                    Time = requestTime,
//                    context.Request.Method,
//                    context.Request.Path,
//                    context.Request.QueryString,
//                    context.User?.Identity?.Name
//                };

//                _logger.LogInformation("Request: {@RequestInfo}", requestInfo);

//                await _next(context);

//                stopwatch.Stop();

//                // Log response
//                context.Response.Body.Seek(0, SeekOrigin.Begin);
//                var responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
//                context.Response.Body.Seek(0, SeekOrigin.Begin);

//                var responseInfo = new
//                {
//                    Time = DateTime.UtcNow,
//                    context.Response.StatusCode,
//                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
//                    ResponseLength = responseContent.Length
//                };

//                _logger.LogInformation("Response: {@ResponseInfo}", responseInfo);

//                await responseBody.CopyToAsync(originalBodyStream);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while processing the request");
//                throw;
//            }
//            finally
//            {
//                context.Response.Body = originalBodyStream;
//            }
//        }
//    }
//}