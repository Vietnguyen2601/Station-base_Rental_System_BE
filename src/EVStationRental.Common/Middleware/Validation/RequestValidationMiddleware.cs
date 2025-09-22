using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EVStationRental.Common.Middleware.Validation
{
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Validate request size
            if (context.Request.ContentLength > 10485760) // 10MB
            {
                context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                await context.Response.WriteAsync("Request too large");
                return;
            }

            // Validate Content-Type for POST/PUT requests
            if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method))
            {
                var contentType = context.Request.ContentType?.ToLower() ?? "";
                if (!contentType.Contains("application/json"))
                {
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    await context.Response.WriteAsync("Unsupported Media Type. Please use application/json");
                    return;
                }

                // Validate JSON format for POST/PUT requests
                try
                {
                    using var reader = new StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    JsonDocument.Parse(body);

                    // Reset the request body position
                    var bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);
                    context.Request.Body = new MemoryStream(bodyBytes);
                    context.Request.Body.Position = 0;
                }
                catch (JsonException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid JSON format");
                    return;
                }
            }

            await _next(context);
        }
    }
}