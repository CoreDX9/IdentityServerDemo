using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.App.Management;
using CoreDX.Domain.Entity.Identity;
using System.Security.Claims;
using Localization.SqlLocalizer.DbStringLocalizer;

namespace IdentityServer.Helpers
{
    public static class DbMigrationHelpers
    {
        /// <summary>
        /// Generate migrations before running this method, you can use these steps bellow:
        /// https://github.com/skoruba/IdentityServer4.Admin#ef-core--data-access
        /// </summary>
        /// <param name="host"></param>      
        public static async Task EnsureSeedData<TIdentityServerDbContext, TIdentityDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TUser, TRole, TKey>(IHost host)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLogDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                await EnsureDatabasesMigrated<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext>(services);
                await EnsureSeedData<TIdentityServerDbContext, TUser, TRole, TKey>(services);
            }
        }

        public static async Task EnsureDatabasesMigrated<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext>(IServiceProvider services)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext
            where TConfigurationDbContext : DbContext
            where TLogDbContext : DbContext
            where TAuditLogDbContext : DbContext
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TPersistedGrantDbContext>())
                {
                    if(!context.Database.IsInMemory())
                        await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TIdentityDbContext>())
                {
                    if (!context.Database.IsInMemory())
                        await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TConfigurationDbContext>())
                {
                    if (!context.Database.IsInMemory())
                        await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TLogDbContext>())
                {
                    if (!context.Database.IsInMemory())
                        await context.Database.MigrateAsync();
                }

                using (var context = scope.ServiceProvider.GetRequiredService<TAuditLogDbContext>())
                {
                    if (!context.Database.IsInMemory())
                        await context.Database.MigrateAsync();
                }

                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<LocalizationModelContext>();
                    if (!context.Database.IsInMemory())
                        //这里不能用 using
                        await context.Database.MigrateAsync();
                }
                catch { }

            }
        }

        public static async Task EnsureSeedData<TIdentityServerDbContext, TUser, TRole, TKey>(IServiceProvider serviceProvider)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TIdentityServerDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TRole>>();
                var rootConfiguration = scope.ServiceProvider.GetRequiredService<IRootConfiguration>();

                await EnsureSeedIdentityServerData(context, rootConfiguration.IdentityServerDataConfiguration);
                await EnsureSeedIdentityData<TUser, TRole, TKey>(userManager, roleManager, rootConfiguration.IdentityDataConfiguration, scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData<TUser, TRole, TKey>(UserManager<TUser> userManager,
            RoleManager<TRole> roleManager, IdentityDataConfiguration identityDataConfiguration, IServiceProvider serviceProvider)
            where TUser : IdentityUser<TKey>, new()
            where TRole : IdentityRole<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            var rs = identityDataConfiguration.Roles.ToArray();
            var rns = (await roleManager.Roles.ToListAsync()).Select(x => x.Name).ToArray();
            var urs = rs.Where(x => !rns.Contains(x.Name)).ToArray();
            if (urs.Length > 0)
            {
                // adding roles from seed
                foreach (var r in urs)
                {
                    if (!await roleManager.RoleExistsAsync(r.Name))
                    {
                        var role = new TRole
                        {
                            Name = r.Name
                        };

                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            foreach (var claim in r.Claims)
                            {
                                await roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                            }
                        }
                    }
                }
            }

            var us = identityDataConfiguration.Users.ToArray();
            var uns = (await userManager.Users.ToListAsync()).Select(x => x.UserName).ToArray();
            var uus = us.Where(x => !uns.Contains(x.Username)).ToArray();
            if (uus.Length > 0)
            {
                // adding users from seed
                foreach (var user in uus)
                {
                    var identityUser = new TUser
                    {
                        UserName = user.Username,
                        Email = user.Email,
                        EmailConfirmed = true
                    };

                    // if there is no password we create user without password
                    // user can reset password later, because accounts have EmailConfirmed set to true
                    var result = !string.IsNullOrEmpty(user.Password)
                        ? await userManager.CreateAsync(identityUser, user.Password)
                        : await userManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        foreach (var claim in user.Claims)
                        {
                            await userManager.AddClaimAsync(identityUser, new Claim(claim.Type, claim.Value));
                        }

                        foreach (var role in user.Roles)
                        {
                            await userManager.AddToRoleAsync(identityUser, role);
                        }
                    }
                }
            }

            {
                var context = serviceProvider.GetService<ApplicationDbContext>();

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
                var context = serviceProvider.GetRequiredService<ApplicationIdentityDbContext>();

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

                var admin = await roleManager.FindByNameAsync("admin");
                if (admin == null)
                {
                    admin = new TRole();
                    admin.Name = "admin";
                    var result = await roleManager.CreateAsync(admin);
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

                var user = await roleManager.FindByNameAsync("user") as ApplicationRole;
                if (user == null)
                {
                    user = new ApplicationRole();
                    user.Name = "user";
                    var result = await roleManager.CreateAsync(user as TRole);
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

                var vip = await roleManager.FindByNameAsync("vip") as ApplicationRole;
                if (vip == null)
                {
                    vip = new ApplicationRole();
                    vip.Name = "vip";
                    vip.Parent = user;
                    var result = await roleManager.CreateAsync(vip as TRole);
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

                var alice = await userManager.FindByNameAsync("alice");
                if (alice == null)
                {
                    alice = new TUser()
                    {
                        UserName = "alice",
                        Email = "AliceSmith@email.com"
                    };
                    var result = await userManager.CreateAsync(alice, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddClaimsAsync(alice, new Claim[]{
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

                    result = await userManager.AddToRoleAsync(alice, "vip");
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

                var bob = await userManager.FindByNameAsync("bob");
                if (bob == null)
                {
                    bob = new TUser()
                    {
                        UserName = "bob",
                        Email = "BobSmith@email.com"
                    };
                    var result = await userManager.CreateAsync(bob, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddClaimsAsync(bob, new Claim[]{
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

                    result = await userManager.AddToRoleAsync(bob, "user");
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

                var coredx = await userManager.FindByNameAsync("coredx");
                if (coredx == null)
                {
                    coredx = new TUser()
                    {
                        UserName = "coredx",
                        Email = "coredx@email.com"
                    };
                    var result = await userManager.CreateAsync(coredx, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = await userManager.AddClaimsAsync(bob, new Claim[]{
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

                    result = await userManager.AddToRoleAsync(coredx, "admin");
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
        }

        /// <summary>
        /// Generate default clients, identity and api resources
        /// </summary>
        private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IdentityServerDataConfiguration identityServerDataConfiguration)
            where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        {
            var irs = identityServerDataConfiguration.IdentityResources.ToArray();
            var irns = (await context.IdentityResources.ToListAsync()).Select(x => x.Name).ToArray();
            var uirs = irs.Where(x => !irns.Contains(x.Name)).ToArray();
            if (uirs.Length > 0)
            {
                foreach (var resource in uirs)
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            var ars = identityServerDataConfiguration.ApiResources.ToArray();
            var arns = (await context.ApiResources.ToListAsync()).Select(x => x.Name).ToArray();
            var uars = ars.Where(x => !arns.Contains(x.Name)).ToArray();
            if (uars.Length > 0)
            {
                foreach (var resource in uars)
                {
                    foreach (var s in resource.ApiSecrets)
                    {
                        s.Value = s.Value.ToSha256();
                    }

                    await context.ApiResources.AddAsync(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            var cs = identityServerDataConfiguration.Clients.ToArray();
            var cids = (await context.Clients.ToListAsync()).Select(x => x.ClientId).ToArray();
            var ucs = cs.Where(x => !cids.Contains(x.ClientId)).ToArray();
            if (ucs.Length > 0)
            {
                foreach (var client in ucs)
                {
                    foreach (var secret in client.ClientSecrets)
                    {
                        secret.Value = secret.Value.ToSha256();
                    }

                    client.Claims = client.ClientClaims
                        .Select(c => new Claim(c.Type, c.Value))
                        .ToList();

                    await context.Clients.AddAsync(client.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}






