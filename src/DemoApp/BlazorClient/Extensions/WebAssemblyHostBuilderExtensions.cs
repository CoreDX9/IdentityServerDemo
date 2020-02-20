using Microsoft.AspNetCore.Blazor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder UseStartup<T>(this WebAssemblyHostBuilder builder) where T : class, new()
        {
            var startup = new T();
            var configureServices = startup.GetType().GetMethod("ConfigureServices");
            configureServices.Invoke(startup, new object[] { builder.Services });
            return builder;
        }
    }
}
