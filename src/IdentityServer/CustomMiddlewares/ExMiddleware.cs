using System;
using System.Linq;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace IdentityServer.CustomMiddlewares
{
    public class ExMiddleware
    {
        private readonly RequestDelegate _next;

        public ExMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }

    public static class ExMiddlewareExtensions
    {
        public static IApplicationBuilder UseExMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExMiddleware>();
        }
    }
}
