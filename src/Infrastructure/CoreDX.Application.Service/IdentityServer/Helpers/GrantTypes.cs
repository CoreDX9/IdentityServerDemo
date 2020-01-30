using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Application.Service.IdentityServer.Helpers
{
    public class GrantTypes
    {
        public static ICollection<string> Implicit => new string[1]
        {
            "implicit"
        };

        public static ICollection<string> ImplicitAndClientCredentials => new string[2]
        {
            "implicit",
            "client_credentials"
        };

        public static ICollection<string> Code => new string[1]
        {
            "authorization_code"
        };

        public static ICollection<string> CodeAndClientCredentials => new string[2]
        {
            "authorization_code",
            "client_credentials"
        };

        public static ICollection<string> Hybrid => new string[1]
        {
            "hybrid"
        };

        public static ICollection<string> HybridAndClientCredentials => new string[2]
        {
            "hybrid",
            "client_credentials"
        };

        public static ICollection<string> ClientCredentials => new string[1]
        {
            "client_credentials"
        };

        public static ICollection<string> ResourceOwnerPassword => new string[1]
        {
            "password"
        };

        public static ICollection<string> ResourceOwnerPasswordAndClientCredentials => new string[2]
        {
            "password",
            "client_credentials"
        };

        public static ICollection<string> DeviceFlow => new string[1]
        {
            "urn:ietf:params:oauth:grant-type:device_code"
        };
    }
}
