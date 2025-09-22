using Microsoft.AspNetCore.Builder;

namespace EVStationRental.Common.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}