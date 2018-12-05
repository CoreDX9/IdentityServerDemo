using System;
using System.Linq;
using System.Security.Claims;
using Domain.Identity;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.EntityFrameworkCore.Identity;

namespace IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("正在初始化Identity数据库...");

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                try
                {
                    scope.ServiceProvider.GetRequiredService<LocalizationModelContext>().Database.Migrate();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                }
                catch
                {
                    //DbContext.Database.ProviderName = "Microsoft.EntityFrameworkCore.InMemory"
                    // ignored
                }

                {
                    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                    try
                    {
                        context.Database.Migrate();
                    }
                    catch
                    {
                        // ignored
                    }
                    
                    EnsureSeedData(context);
                }

                {
                    var context = scope.ServiceProvider.GetService<ApplicationIdentityDbContext>();

                    try
                    {
                        context.Database.Migrate();
                    }
                    catch
                    {
                        // ignored
                    }
                    
                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var alice = userMgr.FindByNameAsync("alice").Result;
                    if (alice == null)
                    {
                        alice = new ApplicationUser()
                        {
                            UserName = "alice",
                            Email = "AliceSmith@email.com"
                        };
                        var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：alice；初始密码为：Pass123$");
                    }
                    else
                    {
                        Console.WriteLine("用户：alice 已经存在");
                    }

                    var bob = userMgr.FindByNameAsync("bob").Result;
                    if (bob == null)
                    {
                        bob = new ApplicationUser()
                        {
                            UserName = "bob",
                            Email = "BobSmith@email.com"
                        };
                        var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：bob；初始密码为：Pass123$");
                    }
                    else
                    {
                        Console.WriteLine("用户：bob 已经存在");
                    }
                }
            }

            Console.WriteLine("Identity数据库初始化完成");
            Console.WriteLine();
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            Console.WriteLine("正在初始化IdentityServer配置数据库……");

            if (!context.Clients.Any())
            {
                Console.WriteLine("正在初始化客户端……");
                foreach (var client in Config.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("客户端已初始化");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("正在初始化Identity资源……");
                foreach (var resource in Config.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Identity资源已初始化");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("正在初始化API资源");
                foreach (var resource in Config.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("API资源已初始化");
            }

            Console.WriteLine("IdentityServer配置数据库初始化完成");
            Console.WriteLine();
        }
    }
}
