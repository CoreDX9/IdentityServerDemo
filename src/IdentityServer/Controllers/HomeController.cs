using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using IdentityServer.CustomServices;
using IdentityServer.Extensions;
using IdentityServer.HttpHandlerBase;
using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

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
        [RequestHandlerIdentification("home-about")]
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
        public async Task<ActionResult> GetControllersAndActionsAsync([FromServices]IRequestHandlerInfo requestHandlerInfo)
        {
            //json对象
            dynamic data = new ExpandoObject();

            data.Areas = requestHandlerInfo.GetAreaInfos();

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
