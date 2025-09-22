using Microsoft.AspNetCore.Builder;

namespace EVStationRental.Common.Middleware.Authentication
{
    public static class JwtAuthenticationExtensions
    {
        public static IApplicationBuilder UseJwtAuthentication(
            this IApplicationBuilder builder,
            string secretKey)
        {
            return builder.UseMiddleware<JwtAuthenticationMiddleware>(secretKey);
        }
    }
}