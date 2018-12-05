using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.HttpHandlerBase;
using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace IdentityServer.Controllers
{
    public class HomeController : BaseController
    {
        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }
        [MyAuthorize]
        [ActionName("About")]
        public async Task<IActionResult> AboutAsync()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("test/[controller]/con", Name = "test")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ConsentCookie()
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            consentFeature.GrantConsent();
            return Json(true);
        }

        public IActionResult SetLanguage(string lang)
        {
            var returnUrl = HttpContext.RequestReferer() ?? "/Home";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect(returnUrl);
        }

        [HttpDelete, HttpGet, HttpHead, HttpOptions, HttpPatch, HttpPost, HttpPut]
        [ActionName("GetControllersAndActions")]
        public async Task<ActionResult> GetControllersAndActionsAsync()
        {
            //创建控制器类型列表
            List<Type> controllerTypes = new List<Type>();

            //获取程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //获取程序集下所有继承Controller的类型
            foreach (var ass in assemblies)
            {
                controllerTypes
                    .AddRange(ass.GetTypes()
                        .Where(type =>
                            typeof(Controller).IsAssignableFrom(type)
                            && !type.IsAbstract//排除抽象类
                        )
                    );
            }

            //根据Area分组控制器
            var areas = controllerTypes.GroupBy(c => c.GetCustomAttribute<AreaAttribute>()?.RouteValue); 

            //json对象
            dynamic data = new ExpandoObject();

            //根，从area开始
            data.areas = new List<AreaInfo>();

            //遍历区域
            foreach (var area in areas)
            {
                var a = new AreaInfo {Name = /*string.IsNullOrEmpty(area.Key) ? "" : */area.Key};
                data.areas.Add(a);

                //遍历控制器
                foreach (var controller in area)
                {
                    var conInfo = new AreaInfo.ControllerInfo
                    {
                        Name = controller.Name,
                        TypeFullName = controller.FullName,
                        Desc = (controller.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                            ?.Description,
                        Area = (controller.GetCustomAttribute(typeof(AreaAttribute)) as AreaAttribute)?.RouteValue,
                    };
                    a.Controllers.Add(conInfo);

                    //获取控制器下所有返回类型实现了IActionResult接口（或者是Task<IActionResult>）的方法
                    var actions = controller
                        .GetMethods()
                        .Where(method =>
                            method.DeclaringType != typeof(Controller) //排除控制器基类的内部方法，能扫描到但是没什么用
                            && method.DeclaringType != typeof(ControllerBase)
                            && (typeof(IActionResult).IsAssignableFrom(method.ReturnType)
                                || method.ReturnType.GenericTypeArguments
                                    .Any(t => typeof(IActionResult).IsAssignableFrom(t))
                            )
                        );

                    //遍历动作
                    foreach (var action in actions)
                    {
                        var act = new AreaInfo.ControllerInfo.ActionInfo
                        {
                            Name = action.Name,
                            SignName = action.ToString(),
                            ActionName =
                                (action.GetCustomAttribute(typeof(ActionNameAttribute)) as ActionNameAttribute)
                                ?.Name, //ActionName特性
                            Desc = (action.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                                ?.Description,
                            //Url生成优先使用ActionName特性，控制器要去掉Controller后缀，要检查area特性
                            Url = Url.Action(
                                (action.GetCustomAttribute(typeof(ActionNameAttribute)) as ActionNameAttribute)?.Name ??
                                action.Name,
                                //Controller有10个字母
                                controller.Name.Substring(0, controller.Name.Length - 10),
                                new
                                {
                                    area = (controller.GetCustomAttribute(typeof(AreaAttribute)) as AreaAttribute)
                                        ?.RouteValue
                                }
                            ),
                        };

                        //检查Http Method特性，用于限定访问动作的方法，列出允许使用的方法，为空表示不限定，可使用任何方法访问
                        var attris = action.GetCustomAttributes();

                        if (attris.Any(attri => attri.GetType() == typeof(HttpGetAttribute))) act.HttpMethodLimits.Add("Get");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpPostAttribute))) act.HttpMethodLimits.Add("Post");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpPutAttribute))) act.HttpMethodLimits.Add("Put");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpDeleteAttribute))) act.HttpMethodLimits.Add("Delete");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpHeadAttribute))) act.HttpMethodLimits.Add("Head");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpOptionsAttribute))) act.HttpMethodLimits.Add("Options");
                        if (attris.Any(attri => attri.GetType() == typeof(HttpPatchAttribute))) act.HttpMethodLimits.Add("Patch");

                        conInfo.Actions.Add(act);
                    }
                }
            }

            #region 遍历Razor Page使用程序集扫描

            //创建Razor Page类型列表
            //List<Type> pageTypes = new List<Type>();

            //获取程序集下所有继承PageModel的类型
            //foreach (var ass in assemblies)
            //{
            //    pageTypes
            //        .AddRange(ass.GetTypes()
            //            .Where(type =>
            //                    typeof(PageModel).IsAssignableFrom(type)
            //                    && !type.IsAbstract //排除抽象类
            //                    && !type.Namespace.Contains(
            //                        "Microsoft.AspNetCore.Identity.UI.Pages") //排除AspNet Identity内置页面
            //            )
            //        );
            //}

            //遍历Razor Page使用程序集扫描
            //命名空间需要与文件夹结构一致，否则会出错
            //使用默认Razor Page路由，非默认情况下可能会出错，未验证
            //foreach (var page in pageTypes)
            //{
            //    var spases = page.Namespace.Split('.');
            //    //根据命名空间获取区域，如果命名空间与文件夹结构不匹配会出错
            //    var area = spases.IndexOf("Areas") > -1 ? spases[spases.IndexOf("Areas") + 1] : null;
            //    var p = new AreaInfo.PageInfo
            //    {
            //        Area = area,
            //        Name = page.Name,
            //        HandlerTypeFullName = page.FullName
            //    };

            //    //查找在扫描控制器时创建的同名区域，存在就把Razor Page信息加进去，不存在就新建区域加进去
            //    var pa = (data.areas as List<AreaInfo>).FirstOrDefault(parea => parea.Name == area);
            //    if (pa != null)
            //    {
            //        pa.Pages.Add(p);
            //    }
            //    else
            //    {
            //        pa = new AreaInfo { Name = area };
            //        data.areas.Add(pa);
            //    }

            //    //获取返回类型实现了IActionResult接口（或者是Task<IActionResult>）或者以On{HttpMethod}开头的方法
            //    //和MVC不同，Razor Page返回void会调用绑定页面，所以不能排除非IActionResult返回，所以handler必须按格式取名，如需要特殊处理请自行添加筛选逻辑
            //    var handlers = page
            //        .GetMethods()
            //        .Where(method =>
            //            method.DeclaringType != typeof(PageModel)
            //            && (method.ReturnType.IsAssignableFrom(
            //                    typeof(IActionResult))
            //                || method.ReturnType.GenericTypeArguments
            //                    .Any(t => t.IsAssignableFrom(
            //                        typeof(IActionResult)))
            //                || method.Name.ToLower().StartsWith("onget")
            //                || method.Name.ToLower().StartsWith("onpost")
            //                || method.Name.ToLower().StartsWith("onput")
            //                || method.Name.ToLower().StartsWith("ondelete")
            //                || method.Name.ToLower().StartsWith("onhead")
            //                || method.Name.ToLower().StartsWith("onoptions")
            //                || method.Name.ToLower().StartsWith("onpatch")
            //            )
            //        );

            //    //遍历handler
            //    foreach (var handler in handlers)
            //    {
            //        string httpMethod = null;
            //        //去掉方法名开头的On
            //        var mName = handler.Name.ToLower().StartsWith("on") ? handler.Name.Substring(2) : handler.Name;
            //        //去掉方法名末尾的Async
            //        if (mName.ToLower().EndsWith("async")) mName = mName.Substring(0, mName.Length - 5);

            //        //根据handler名称识别HttpMethod，方法名开头On之后紧跟的就是HttpMethod，这些是常用的（MVC的HttpMethod特性有这几种），不是全部
            //        var m2 = mName.ToLower();
            //        if (m2.StartsWith("get")) httpMethod = "Get";
            //        if (m2.StartsWith("post")) httpMethod = "Post";
            //        if (m2.StartsWith("put")) httpMethod = "Put";
            //        if (m2.StartsWith("delete")) httpMethod = "Delete";
            //        if (m2.StartsWith("head")) httpMethod = "Head";
            //        if (m2.StartsWith("options")) httpMethod = "Options";
            //        if (m2.StartsWith("patch")) httpMethod = "Patch";

            //        //识别handlerName,在方法名开头的On{HttpMethod}和方法名末尾的Async之间的部分，Async不一定有，一般为异步方法后缀
            //        string hName = null;
            //        if (httpMethod != null)
            //        {
            //            if (mName.Length > httpMethod.Length)
            //            {
            //                hName = mName.Substring(httpMethod.Length);
            //            }
            //        }

            //        //识别pageName,按照AspNet Core默认文件夹结构且命名空间与文件夹结构匹配，去除命名空间中Areas以及之前的部分和Pages，剩下的就是pageName，否则会出错，需要自行修改生成逻辑
            //        var pName = string.Join("/", spases.Skip(spases.IndexOf("Areas") + 2).Where(s => s != "Pages"));
            //        if (page.Name.ToLower().EndsWith("model"))//去掉Razor Page类的后缀Model
            //        {
            //            pName += "/" + page.Name.Substring(0, page.Name.Length - 5);
            //        }
            //        else
            //        {
            //            pName += "/" + page.Name;
            //        }
            //        if (!pName.StartsWith("/")) pName = "/" + pName;//增加前导斜杠使用根路径，否则会出错，只有在相同区域的Razor Page之间才能使用相对路径

            //        //生成Url
            //        var url = Url.Page(pName, hName, new { area = area });

            //        p.PageHandlers.Add(new AreaInfo.PageInfo.PageHandlerInfo
            //        {
            //            Name = handler.Name,
            //            SignName = handler.ToString(),
            //            HttpMethod = httpMethod,
            //            Url = url
            //        });
            //    }
            //}

            #endregion 遍历Razor Page使用程序集扫描

            //遍历Razor Page，使用由PageModelBase提供的RazorPageInfo接口请求信息
            //命名空间不需要与文件夹结构一致
            //使用默认Razor Page路由，非默认情况下可能会出错，未验证
            var routeCollection = RouteData.Routers.FirstOrDefault(r => r.GetType() == typeof(RouteCollection));
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo routesField = routeCollection?.GetType().GetField("_routes",flag);
            var iRouters = routesField?.GetValue(routeCollection) as List<IRouter>;
            var attributeRoute = iRouters?.FirstOrDefault(r => r.GetType() == typeof(AttributeRoute));

            FieldInfo actionDescriptorCollectionProviderField =
                attributeRoute?.GetType().GetField("_actionDescriptorCollectionProvider", flag);
            var actionDescriptorCollectionProvider =
                actionDescriptorCollectionProviderField?.GetValue(attributeRoute) as ActionDescriptorCollectionProvider;

            var pageActionDescriptors = actionDescriptorCollectionProvider?.ActionDescriptors.Items
                .Where(p => p.GetType() == typeof(PageActionDescriptor)).ToList();

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001");
            if (pageActionDescriptors?.Count > 0)
            {
                foreach (var page in pageActionDescriptors)
                {
                    var areapro = page.GetType().GetProperty("AreaName");

                    var u = new Uri(httpClient.BaseAddress,
                        new Uri(Url.Page(page.DisplayName.Substring(6),
                                "RazorPageInfo",
                                new {area = areapro.GetValue(page) as string}
                            ).Substring(1),
                            UriKind.Relative
                        )
                    );

                    var res = await httpClient.GetAsync(u);
                    if (res.IsSuccessStatusCode &&
                        res.Content.Headers.ContentType.MediaType.Contains("application/json"))
                    {
                        var jsonPageInfo =
                            JsonConvert.DeserializeObject<AreaInfo.PageInfo>(await res.Content.ReadAsStringAsync());

                        //查找在扫描控制器时创建的同名区域，存在就把Razor Page信息加进去，不存在就新建区域加进去
                        var pa = (data.areas as List<AreaInfo>).FirstOrDefault(parea =>
                            parea.Name == jsonPageInfo.Area);
                        if (pa != null)
                        {
                            pa.Pages.Add(jsonPageInfo);
                        }
                        else
                        {
                            pa = new AreaInfo {Name = jsonPageInfo.Area};
                            data.areas.Add(pa);
                        }
                    }
                }
            }
            
            return Json(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {  RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ActionName("NotFound")]
        public IActionResult NotFoundResult()
        {
            return NotFoundView();
        }
    }
}
