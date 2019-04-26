using System.Linq;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace IdentityServer.CustomMiddlewares
{
    public class AntiforgeryTokenGenerateMiddleware : IMiddleware
    {
        private readonly IAntiforgery antiforgery;

        public AntiforgeryTokenGenerateMiddleware(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Headers["X-REQUEST-CSRF-TOKEN"].FirstOrDefault()?.Equals("true") == true && await antiforgery.IsRequestValidAsync(context))
            {
                    var token = antiforgery.GetAndStoreTokens(context);
                    context.Response.Headers.Add("X-RESPONSE-CSRF-TOKEN-NAME", token.HeaderName);
                    context.Response.Headers.Add("X-RESPONSE-CSRF-TOKEN-VALUE", token.RequestToken);
            }

            await next(context);
        }
    }


    public static class AntiforgeryTokenGenerateMiddlewareExtensions
    {
        public static IApplicationBuilder UseAntiforgeryTokenGenerateMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiforgeryTokenGenerateMiddleware>();
        }
    }
}
