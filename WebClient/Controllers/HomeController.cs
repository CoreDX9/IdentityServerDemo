using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using WebClient.Models;
using Util.TypeExtensions;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Login()
        {
            var returnUrl = HttpContext.Request.Headers[HeaderNames.Referer].ToString();
            returnUrl = returnUrl.IsNullOrEmpty()  ? "/Home/Index" : returnUrl;

            return Redirect(returnUrl);
        }

        [Authorize]
        public async Task Logout()
        {
            //退出Cookies架构登录，清除本地Cookie
            await HttpContext.SignOutAsync("Cookies");
            //退出oidc架构登录，OpenID Connect中间件已经实现退出登录协议
            await HttpContext.SignOutAsync("oidc");
        }

        [Authorize]
        public IActionResult UserInfo()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);//client.SetBearerToken(accessToken);//不知道为啥这个扩展方法出不来System.Net.Http.HttpClientExtensions.SetBearerToken(this HttpClient client, string token)

            var content = await client.GetStringAsync("https://localhost:5003/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View();
        }
    }
}
