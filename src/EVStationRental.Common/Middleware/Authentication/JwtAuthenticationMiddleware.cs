//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace EVStationRental.Common.Middleware.Authentication
//{
//    public class JwtAuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly string _secretKey;

//        public JwtAuthenticationMiddleware(RequestDelegate next, string secretKey)
//        {
//            _next = next;
//            _secretKey = secretKey;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//            if (string.IsNullOrEmpty(token))
//            {
//                context.Response.StatusCode = 401;
//                await context.Response.WriteAsync("Authorization token not found");
//                return;
//            }

//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes(_secretKey);

//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(key),
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                    ClockSkew = TimeSpan.Zero
//                }, out SecurityToken validatedToken);

//                var jwtToken = (JwtSecurityToken)validatedToken;
                
//                // Add claims to the context
//                var claims = new List<Claim>();
//                claims.AddRange(jwtToken.Claims);
                
//                var identity = new ClaimsIdentity(claims, "JWT");
//                context.User = new ClaimsPrincipal(identity);

//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                context.Response.StatusCode = 401;
//                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
//            }
//        }
//    }
//}