using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.HttpHandlerBase;
using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

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
        public async Task<ActionResult> GetControllersAndActionsAsync([FromServices]IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, [FromServices]IPageLoader pageLoader)
        {
            //json对象
            dynamic data = new ExpandoObject();

            //根，从area开始
            data.areas = new List<AreaInfo>();

            //遍历区域
            foreach (var area in actionDescriptorCollectionProvider.ActionDescriptors.Items.GroupBy(item => item.RouteValues["area"]))
            {
                var areaInfo = new AreaInfo { Name = area.Key };
                data.areas.Add(areaInfo);

                //遍历控制器
                foreach (var controllerActions in area.OfType<ControllerActionDescriptor>().GroupBy(c=>c.ControllerTypeInfo))
                {
                    var conInfo = new AreaInfo.ControllerInfo
                    {
                        Name = controllerActions.First().ControllerName,
                        TypeFullName = controllerActions.Key.FullName,
                        Desc = (controllerActions.Key.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                            ?.Description,
                        Area = controllerActions.First().RouteValues["area"],
                    };
                    areaInfo.Controllers.Add(conInfo);

                    //遍历动作
                    foreach (var action in controllerActions)
                    {
                        var act = new AreaInfo.ControllerInfo.ActionInfo
                        {
                            Name = action.MethodInfo.Name,
                            SignName = action.MethodInfo.ToString(),
                            ActionName = action.ActionName,
                            Desc = (action.MethodInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                                ?.Description,
                            Url = Url.Action(action.ActionName, conInfo.Name, new { area = conInfo.Area }),
                            HttpMethodLimits = action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.ToList()
                        };
                        conInfo.Actions.Add(act);
                    }
                }

                //遍历Razor Page
                foreach (var pageActionDescriptor in area.OfType<PageActionDescriptor>())
                {
                    var page = pageLoader.Load(pageActionDescriptor);

                    var pageInfo = new AreaInfo.PageInfo
                    {
                        Name = page.DisplayName,
                        Area = page.AreaName,
                        HandlerTypeFullName = page.HandlerTypeInfo.FullName,
                    };

                    foreach (var handlerMethod in page.HandlerMethods)
                    {
                        pageInfo.PageHandlers.Add(new AreaInfo.PageInfo.PageHandlerInfo
                        {
                            HttpMethod = handlerMethod.HttpMethod,
                            Name = handlerMethod.Name,
                            SignName = handlerMethod.MethodInfo.ToString(),
                            Url = Url.Page(page.DisplayName, handlerMethod.Name, new { area = page.AreaName })
                        });
                    }
                    areaInfo.Pages.Add(pageInfo);
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
