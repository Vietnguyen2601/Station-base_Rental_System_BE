using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EVStationRental.Common.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("No authorization header found.");
                return;
            }

            // Add your token validation logic here
            // Example: if (token is valid)
            if (authHeader.StartsWith("Bearer "))
            {
                // Validate JWT token here
                // You can inject your token validation service here
                
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid authorization header.");
            }
        }
    }
}