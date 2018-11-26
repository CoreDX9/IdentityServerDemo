using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API",new []{JwtClaimTypes.Role})
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //AccessTokenLifetime = 60,token有效期

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "mvcIm",
                    ClientName = "Implicit MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5005/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5005/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvcHcc",
                    ClientName = "HybridAndClientCredentials MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "https://localhost:5005/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5005/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        //只要客户端作用域包含OfflineAccess，无论用户是否已经授权OfflineAccess
                        //每次登陆都会跳到同意授权页面，重新请求授权，除非设置RequireConsent为false
                        //但是设置RequireConsent会导致登陆即授权，用户无法手动选择要授权的作用域
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Phone,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "mvcHcc2",
                    ClientName = "HybridAndClientCredentials MVC Client2",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "https://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Phone,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                // JavaScript Client
                new Client
                {
                    ClientId = "jsIm",
                    ClientName = "Implicit JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "https://localhost:5007/callback.html", "https://localhost:5003/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5007/index.html" },
                    AllowedCorsOrigins =     { "https://localhost:5007" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                }
            };
        }
    }
}
