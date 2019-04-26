using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Management;
using IdentityServer.CustomServices;
using Newtonsoft.Json;
using Repository.EntityFrameworkCore;
using Util.TypeExtensions;
using Z.EntityFramework.Plus;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRequestHandlerInfo _requestHandlerInfo;


        public MenuController(ApplicationDbContext context, IRequestHandlerInfo requestHandlerInfo)
        {
            _context = context;
            _requestHandlerInfo = requestHandlerInfo;
        }

        // GET: Manage/Menu
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Menus.Include(m => m.Items).Include(m => m.CreationUser).Include(m => m.LastModificationUser).Include(m => m.Parent);
            return View(await applicationDbContext.ToListAsync());
        }

        [ActionName("GetMenu")]
        public async Task<JsonResult> GetMenuAsync()
        {
            var menus = await _context.Menus
                .AsNoTracking()
                .Include(m => m.Items)
                .ToListAsync();

            //使用以下方法可以在不调用.Include(m => m.Parent)生成复杂sql的情况下在内存中设置导航属性
            var menuRoot = menus.Single(m => m.ParentId == null);
            menuRoot = menuRoot.AsHierarchical(m => menus.Where(sm => sm.ParentId == m.Id))
                .SetParentChildren(
                    n => n.Children
                    , parentSetter: (m, mp) =>
                    {
                        m.Parent = mp;
                        m.ParentId = mp?.Id;
                    });
            return Json(menuRoot, new JsonSerializerSettings(){ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
        }

        public JsonResult GetLinks()
        {
            var links = _requestHandlerInfo.GetRequestHandlerUrlInfos()
                .Select(h =>
                {
                    var methods = string.Join(',',
                        _requestHandlerInfo.GetAreaInfos().SelectMany(a => a.Handlers)
                            .Single(ha => ha.MethodInfo == h.Key).SupportedHttpMethods ?? new string[0]);
                    return new
                    {
                        Url = h.Value,
                        HttpMethods = methods,
                        Type = h.Key.DeclaringType.FullName
                    };
                })
                .Where(h => !(!h.HttpMethods.IsNullOrEmpty() && !h.HttpMethods.ToLower().Contains("get")))
                .Select(h => new {h.Type, h.Url})
                .OrderBy(i => i.Url)
                .GroupBy(h => h.Type)
                .Select(h => new {h.Key, links = h.Select(l => l.Url)});

            return Json(links);
        }

        // GET: Manage/Menu/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.CreationUser)
                .Include(m => m.LastModificationUser)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Manage/Menu/Create
        public IActionResult Create()
        {
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ParentId"] = new SelectList(_context.Menus, "Id", "Id");
            return View();
        }

        // POST: Manage/Menu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Index,Title,Order,ParentId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] Menu menu)
        {
            if (ModelState.IsValid)
            {
                menu.Id = Guid.NewGuid();
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.LastModificationUserId);
            ViewData["ParentId"] = new SelectList(_context.Menus, "Id", "Id", menu.ParentId);
            return View(menu);
        }

        // GET: Manage/Menu/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.LastModificationUserId);
            ViewData["ParentId"] = new SelectList(_context.Menus, "Id", "Id", menu.ParentId);
            return View(menu);
        }

        // POST: Manage/Menu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Index,Title,Order,ParentId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", menu.LastModificationUserId);
            ViewData["ParentId"] = new SelectList(_context.Menus, "Id", "Id", menu.ParentId);
            return View(menu);
        }

        // GET: Manage/Menu/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.CreationUser)
                .Include(m => m.LastModificationUser)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Manage/Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(Guid id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult CreateMenu(Menu menu)
        {
            _context.Menus.Add(menu);
            var re = _context.SaveChanges();
            if (re > 0)
            {
                return Json(menu);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult UpdateMenu(Menu menu)
        {
            _context.Menus.Update(menu);
            var re = _context.SaveChanges();
            if (re > 0)
            {
                return Json(menu);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult DeleteMenu(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var menu = _context.Menus.Include(m => m.Items).SingleOrDefault(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            _context.MenuItems.RemoveRange(menu.Items);
            _context.Menus.Remove(menu);
            var re = _context.SaveChanges();
            if (re > 0)
            {
                return Json(true);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult CreateMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            var re = _context.SaveChanges();
            if (re > 0)
            {
                return Json(menuItem);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult UpdateMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            var re =_context.SaveChanges();
            if (re > 0)
            {
                return Json(menuItem);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult DeleteMenuItem(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var item = _context.MenuItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(item);
            var re = _context.SaveChanges();
            if (re > 0)
            {
                return Json(true);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
