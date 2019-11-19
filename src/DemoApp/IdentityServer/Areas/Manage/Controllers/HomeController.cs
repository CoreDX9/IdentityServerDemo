using AutoMapper;
using CoreDX.Application.EntityFrameworkCore;
using IdentityServer.Areas.Manage.Models.Menus;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using CoreDX.Common.Util.TypeExtensions;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAntiforgery _antiforgery;

        public HomeController(ApplicationDbContext context, IMapper mapper, IAntiforgery antiforgery)
        {
            _context = context;
            _mapper = mapper;
            _antiforgery = antiforgery;
        }

        public IActionResult Index()
        {
            var menus = _context.Menus
                .AsNoTracking()
                .Include(m => m.Items)
                //.Include(m => m.Parent)
                .ToList();

            //使用以下方法可以在不调用.Include(m => m.Parent)生成复杂sql的情况下在内存中设置导航属性
            var menuRoot = menus.Single(m => m.ParentId == null);
            menuRoot = menuRoot.AsHierarchical(m => menus.Where(sm => sm.ParentId == m.Id))
                .SetParentChildren(n => n.Children
                    //, parentSetter: (n, np) => n.Parent = np //如果不需要双亲节点导航可以忽略这个参数，能顺便避免json序列化时可能的引用循环异常
                    );
            var mm = _mapper.Map<MenuViewModel>(menuRoot);
            ViewBag.Menu = JsonConvert.SerializeObject(mm, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            return View();
        }

        public IActionResult Page1()
        {
            return View();
        }

        public IActionResult Page2()
        {
            return View();
        }

        [/*HttpPost,*/ IgnoreAntiforgeryToken]
        public JsonResult GetAntiXsrfRequestToken()
        {
            var token = _antiforgery.GetAndStoreTokens(HttpContext);
            return Json(new{token.HeaderName, token.RequestToken});
        }
    }
}