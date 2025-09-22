using Microsoft.AspNetCore.Builder;

namespace EVStationRental.Common.Middleware.Validation
{
    public static class RequestValidationExtensions
    {
        public static IApplicationBuilder UseRequestValidation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}