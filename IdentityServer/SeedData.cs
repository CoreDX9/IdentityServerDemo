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
using Repository.EntityFrameworkCore;

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
                catch(Exception ex)//DbContext.Database.ProviderName = "Microsoft.EntityFrameworkCore.InMemory"
                {
                    if (ex.Message !=
                        "Relational-specific methods can only be used when the context is using a relational database provider."
                    )
                    {
                        throw;
                    }
                }

                try
                {
                    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                }
                catch (Exception ex)
                {
                    if (ex.Message !=
                        "Relational-specific methods can only be used when the context is using a relational database provider."
                    )
                    {
                        throw;
                    }
                }

                {
                    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message !=
                            "Relational-specific methods can only be used when the context is using a relational database provider."
                        )
                        {
                            throw;
                        }
                    }

                    EnsureSeedData(context);
                }

                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message !=
                            "Relational-specific methods can only be used when the context is using a relational database provider."
                        )
                        {
                            throw;
                        }
                    }

                    var innKai = context.Organizations.AsNoTracking()
                        .SingleOrDefault(o => o.Name == "IdentityServerDemo委员会");
                    if (innKai == null)
                    {
                        innKai = new Organization{Id = Guid.NewGuid(), Name = "IdentityServerDemo委员会" };
                        context.Organizations.Add(innKai);
                        var result = context.SaveChanges();
                        if (result != 1)
                        {
                            throw new Exception("创建组织：“IdentityServerDemo委员会” 失败！");
                        }
                        Console.WriteLine("已创建组织：“IdentityServerDemo委员会”");
                    }
                    else
                    {
                        Console.WriteLine("组织：“IdentityServerDemo委员会” 已存在");
                    }

                    var zimuzu = context.Organizations.AsNoTracking()
                        .SingleOrDefault(o => o.Name == "字幕组");
                    if (zimuzu == null)
                    {
                        zimuzu = new Organization { Id = Guid.NewGuid(), Name = "字幕组", Parent = innKai};
                        context.Organizations.Add(zimuzu);
                        var result = context.SaveChanges();
                        if (result != 1)
                        {
                            throw new Exception("创建组织：“字幕组” 失败！");
                        }
                        Console.WriteLine("已创建组织：“字幕组”");
                    }
                    else
                    {
                        Console.WriteLine("组织：“字幕组” 已存在");
                    }

                    var hanhuazu = context.Organizations.AsNoTracking()
                        .SingleOrDefault(o => o.Name == "汉化组");
                    if (hanhuazu == null)
                    {
                        hanhuazu = new Organization { Id = Guid.NewGuid(), Name = "汉化组", Parent = innKai };
                        context.Organizations.Add(hanhuazu);
                        var result = context.SaveChanges();
                        if (result != 1)
                        {
                            throw new Exception("创建组织：“汉化组” 失败！");
                        }
                        Console.WriteLine("已创建组织：“汉化组”");
                    }
                    else
                    {
                        Console.WriteLine("组织：“汉化组” 已存在");
                    }

                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                    var admin = roleMgr.FindByNameAsync("admin").Result;
                    if (admin == null)
                    {
                        admin = new ApplicationRole("admin");
                        var result = roleMgr.CreateAsync(admin).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建角色：admin");
                    }
                    else
                    {
                        Console.WriteLine("角色：admin 已经存在");
                    }

                    var user = roleMgr.FindByNameAsync("user").Result;
                    if (user == null)
                    {
                        user = new ApplicationRole("user");
                        var result = roleMgr.CreateAsync(user).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建角色：user");
                    }
                    else
                    {
                        Console.WriteLine("角色：user 已经存在");
                    }

                    var vip = roleMgr.FindByNameAsync("vip").Result;
                    if (vip == null)
                    {
                        vip = new ApplicationRole("vip");
                        vip.Parent = user;
                        var result = roleMgr.CreateAsync(vip).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建角色：vip");
                    }
                    else
                    {
                        Console.WriteLine("角色：vip 已经存在");
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

                        result = userMgr.AddToRoleAsync(alice, "vip").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：alice 已加入角色 “vip”");

                        context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                            { Id = Guid.NewGuid(), User = alice, Organization = zimuzu });
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            throw new Exception("用户 alice 加入组织 “字幕组” 失败！");
                        }
                        Console.WriteLine("用户：alice 已加入组织 “字幕组”");
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

                        result = userMgr.AddToRoleAsync(bob, "user").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：bob 已加入角色 “user”");

                        context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                            {Id = Guid.NewGuid(), User = bob, Organization = hanhuazu});
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            throw new Exception("用户 bob 加入组织 “汉化组” 失败！");
                        }
                        Console.WriteLine("用户：bob 已加入组织 “汉化组”");
                    }
                    else
                    {
                        Console.WriteLine("用户：bob 已经存在");
                    }

                    var coredx = userMgr.FindByNameAsync("coredx").Result;
                    if (coredx == null)
                    {
                        coredx = new ApplicationUser()
                        {
                            UserName = "coredx",
                            Email = "coredx@email.com"
                        };
                        var result = userMgr.CreateAsync(coredx, "Pass123$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "coredx"),
                            new Claim(JwtClaimTypes.GivenName, "coredx"),
                            new Claim(JwtClaimTypes.FamilyName, "coredx"),
                            new Claim(JwtClaimTypes.Email, "coredx@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://coredx.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': '(づ｡◕ᴗᴗ◕｡)づ', 'locality': 'Kunming', 'postal_code': 650000, 'country': 'China' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("location", "!!!∑(ﾟДﾟノ)ノ")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：coredx；初始密码为：Pass123$");

                        result = userMgr.AddToRoleAsync(coredx, "admin").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：coredx 已加入角色 “admin”");

                        context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                            { Id = Guid.NewGuid(), User = coredx, Organization = innKai });
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            throw new Exception("用户 coredx 加入组织 “IdentityServerDemo委员会” 失败！");
                        }
                        Console.WriteLine("用户：coredx 已加入组织 “IdentityServerDemo委员会”");
                    }
                    else
                    {
                        Console.WriteLine("用户：coredx 已经存在");
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
