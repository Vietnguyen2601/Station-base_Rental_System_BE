using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EVStationRental.Common.Middleware.Authorization
{
    public class RoleBasedAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedRoles;

        public RoleBasedAuthorizationMiddleware(RequestDelegate next, string[] allowedRoles)
        {
            _next = next;
            _allowedRoles = allowedRoles;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("User is not authenticated");
                return;
            }

            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!_allowedRoles.Any(role => userRoles.Contains(role)))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("User does not have the required roles");
                return;
            }

            await _next(context);
        }
    }
}