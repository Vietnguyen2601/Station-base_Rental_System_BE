using Microsoft.AspNetCore.Builder;

namespace EVStationRental.Common.Middleware.Authorization
{
    public static class RoleBasedAuthorizationExtensions
    {
        public static IApplicationBuilder UseRoleBasedAuthorization(
            this IApplicationBuilder builder,
            params string[] allowedRoles)
        {
            return builder.UseMiddleware<RoleBasedAuthorizationMiddleware>(allowedRoles);
        }
    }
}