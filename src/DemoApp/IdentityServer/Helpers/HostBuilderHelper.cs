using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDX.Common.Util.TypeExtensions;

namespace IdentityServer.Helpers
{
    public static class HostBuilderHelper
    {
        public static IConfigurationBuilder AddJsonConfiguration(this IConfigurationBuilder configBuilder, string name, string environmentName = null, bool optional = true, bool reloadOnChange = true)
        {
            configBuilder.AddJsonFile($"{name}.json", optional, reloadOnChange);
            if(!environmentName.IsNullOrEmpty())
                configBuilder.AddJsonFile($"{name}.{environmentName}.json", optional, reloadOnChange);
            return configBuilder;
        }
    }
}
