using System;
using System.Linq;
using System.Security.Claims;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.App.Management;
using CoreDX.Domain.Entity.Identity;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerAdmin.Admin.EntityFramework.Shared.DbContexts;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class SeedData
    {
        public static async System.Threading.Tasks.Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
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
                    scope.ServiceProvider.GetRequiredService<IdentityServerPersistedGrantDbContext>().Database.Migrate();
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
                    var context = scope.ServiceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();

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

                //测试预定义Domain基类能不能用，正式数据迁移工具bug无法生成代码
                //var testContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                //testContext.Database.Migrate();

                #region 先跳过初始化，等ef迁移工具修复bug，事实证明是SetComent()的锅 3.0.1

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

                    #region 初始化菜单数据

                    {
                        if (!context.Menus.Any())
                        {
                            Menu menu = new Menu
                            {
                                MenuIcon = new MenuIcon(),
                                Title = "（根菜单）"
                            };
                            Menu menu2 = new Menu
                            {
                                MenuIcon = new MenuIcon { Type = "css", Value = "el-icon-setting" },
                                Title = "系统管理"
                            };
                            Menu menu3 = new Menu
                            {
                                MenuIcon = new MenuIcon(),
                                Title = "账户系统"
                            };
                            Menu menu4 = new Menu
                            {
                                MenuIcon = new MenuIcon(),
                                Title = "权限系统"
                            };
                            menu.Children.Add(menu2);
                            menu2.Children.Add(menu3);
                            menu2.Children.Add(menu4);
                            MenuItem i = new MenuItem
                            {
                                Title = "主页",
                                Link = "/",
                                MenuItemIcon = new MenuItemIcon { Type = "css", Value = "el-icon-setting" }
                            };
                            menu.Items.Add(i);
                            MenuItem ii = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "本地化翻译管理",
                                Link = "/Manage/Localization"
                            };
                            menu2.Items.Add(ii);
                            MenuItem ii2 = new MenuItem
                            {
                                Title = "菜单管理",
                                Link = "/Manage/Menu",
                                MenuItemIcon = new MenuItemIcon()
                            };
                            menu2.Items.Add(ii2);
                            MenuItem i1 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "用户管理",
                                Link = "/Manage/Users"
                            };
                            menu3.Items.Add(i1);
                            MenuItem i2 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "角色管理",
                                Link = "/"
                            };
                            menu3.Items.Add(i2);
                            MenuItem i3 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "组织管理",
                                Link = "/Manage/Organizations"
                            };
                            menu3.Items.Add(i3);
                            MenuItem i4 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "权限定义管理",
                                Link = "/Manage/PermissionDefinition"
                            };
                            menu4.Items.Add(i4);
                            MenuItem i5 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "授权规则管理",
                                Link = "/Manage/RequestAuthorizationRules"
                            };
                            menu4.Items.Add(i5);
                            MenuItem i6 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "用户权限管理",
                                Link = "/Manage/UserPermissionDeclaration"
                            };
                            menu4.Items.Add(i6);
                            MenuItem i7 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "角色权限管理",
                                Link = "/Manage/RolePermissionDeclaration"
                            };
                            menu4.Items.Add(i7);
                            MenuItem i8 = new MenuItem
                            {
                                MenuItemIcon = new MenuItemIcon(),
                                Title = "组织权限管理",
                                Link = "/Manage/OrganizationPermissionDeclaration"
                            };
                            menu4.Items.Add(i8);

                            context.Menus.Add(menu);
                            var result = context.SaveChanges();

                            if (result > 0)
                            {
                                Console.WriteLine("已创建初始菜单数据");
                            }
                            else
                            {
                                throw new Exception("创建初始菜单数据失败！");
                            }
                        }
                        else
                        {
                            Console.WriteLine("菜单数据已经存在");
                        }
                    }

                    #endregion
                }

                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

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
                        innKai = new Organization { Name = "IdentityServerDemo委员会" };
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
                        zimuzu = new Organization { Name = "字幕组", Parent = innKai };
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
                        hanhuazu = new Organization { Name = "汉化组", Parent = innKai };
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
                    var admin = await roleMgr.FindByNameAsync("admin");
                    if (admin == null)
                    {
                        admin = new ApplicationRole("admin");
                        var result = await roleMgr.CreateAsync(admin);
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

                    var user = await roleMgr.FindByNameAsync("user");
                    if (user == null)
                    {
                        user = new ApplicationRole("user");
                        var result = await roleMgr.CreateAsync(user);
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

                    var vip = await roleMgr.FindByNameAsync("vip");
                    if (vip == null)
                    {
                        vip = new ApplicationRole("vip");
                        vip.Parent = user;
                        var result = await roleMgr.CreateAsync(vip);
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
                    var alice = await userMgr.FindByNameAsync("alice");
                    if (alice == null)
                    {
                        alice = new ApplicationUser()
                        {
                            UserName = "alice",
                            Email = "AliceSmith@email.com"
                        };
                        var result = await userMgr.CreateAsync(alice, "Pass123$");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = await userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：alice；初始密码为：Pass123$");

                        result = await userMgr.AddToRoleAsync(alice, "vip");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：alice 已加入角色 “vip”");

                        //context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                        //    {User = alice, Organization = zimuzu });
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            //throw new Exception("用户 alice 加入组织 “字幕组” 失败！");
                        }
                        Console.WriteLine("用户：alice 已加入组织 “字幕组”");
                    }
                    else
                    {
                        Console.WriteLine("用户：alice 已经存在");
                    }

                    var bob = await userMgr.FindByNameAsync("bob");
                    if (bob == null)
                    {
                        bob = new ApplicationUser()
                        {
                            UserName = "bob",
                            Email = "BobSmith@email.com"
                        };
                        var result = await userMgr.CreateAsync(bob, "Pass123$");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：bob；初始密码为：Pass123$");

                        result = await userMgr.AddToRoleAsync(bob, "user");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：bob 已加入角色 “user”");

                        //context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                        //    {User = bob, Organization = hanhuazu});
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            //throw new Exception("用户 bob 加入组织 “汉化组” 失败！");
                        }
                        Console.WriteLine("用户：bob 已加入组织 “汉化组”");
                    }
                    else
                    {
                        Console.WriteLine("用户：bob 已经存在");
                    }

                    var coredx = await userMgr.FindByNameAsync("coredx");
                    if (coredx == null)
                    {
                        coredx = new ApplicationUser()
                        {
                            UserName = "coredx",
                            Email = "coredx@email.com"
                        };
                        var result = await userMgr.CreateAsync(coredx, "Pass123$");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "coredx"),
                            new Claim(JwtClaimTypes.GivenName, "coredx"),
                            new Claim(JwtClaimTypes.FamilyName, "coredx"),
                            new Claim(JwtClaimTypes.Email, "coredx@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://coredx.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': '(づ｡◕ᴗᴗ◕｡)づ', 'locality': 'Kunming', 'postal_code': 650000, 'country': 'China' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("location", "!!!∑(ﾟДﾟノ)ノ")
                        });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("已创建用户：coredx；初始密码为：Pass123$");

                        result = await userMgr.AddToRoleAsync(coredx, "admin");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("用户：coredx 已加入角色 “admin”");

                        //context.ApplicationUserOrganizations.Add(new ApplicationUserOrganization
                        //    {User = coredx, Organization = innKai });
                        var rel = context.SaveChanges();
                        if (rel != 1)
                        {
                            //throw new Exception("用户 coredx 加入组织 “IdentityServerDemo委员会” 失败！");
                        }
                        Console.WriteLine("用户：coredx 已加入组织 “IdentityServerDemo委员会”");
                    }
                    else
                    {
                        Console.WriteLine("用户：coredx 已经存在");
                    }
                }

                #endregion
            }

            Console.WriteLine("Identity数据库初始化完成");
            Console.WriteLine();
        }

        private static void EnsureSeedData(IdentityServerConfigurationDbContext context)
        {
            Console.WriteLine("正在初始化IdentityServer配置数据库……");

            var cids = Config.GetClients().Select(x => x.ClientId).ToArray();
            if (!context.Clients.Any(x => cids.Contains(x.ClientId)))
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

            var irns = Config.GetIdentityResources().Select(x => x.Name).ToArray();
            if (!context.IdentityResources.Any(x => irns.Contains(x.Name)))
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

            var arns = Config.GetApiResources().Select(x => x.Name).ToArray();
            if (!context.ApiResources.Any(x => arns.Contains(x.Name)))
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
