using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace IdentityServer.Extensions
{
    public abstract class ConfigureOptionsUseServiceBase<TOptions> : IConfigureOptions<TOptions>
        where TOptions : class
    {
        protected IServiceProvider Service { get; }
        public ConfigureOptionsUseServiceBase(IServiceProvider service)
        {
            Service = service;
        }

        public abstract void Configure(TOptions options);
    }
}
