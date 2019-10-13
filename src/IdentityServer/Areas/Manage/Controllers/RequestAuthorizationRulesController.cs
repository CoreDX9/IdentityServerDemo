using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer.CustomServices;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class RequestAuthorizationRulesController : Controller
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly IRequestHandlerInfo _requestHandlerInfo;

        public RequestAuthorizationRulesController(ApplicationIdentityDbContext context, IRequestHandlerInfo requestHandlerInfo)
        {
            _context = context;
            _requestHandlerInfo = requestHandlerInfo;
        }

        // GET: Manage/RequestAuthorizationRules
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _context.RequestAuthorizationRules.Include(r => r.CreationUser).Include(r => r.LastModificationUser);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/RequestAuthorizationRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestAuthorizationRule = await _context.RequestAuthorizationRules
                .Include(r => r.CreationUser)
                .Include(r => r.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestAuthorizationRule == null)
            {
                return NotFound();
            }

            return View(requestAuthorizationRule);
        }

        // GET: Manage/RequestAuthorizationRules/Create
        public IActionResult Create()
        {
            ViewData["Handlers"] = new SelectList(
                _requestHandlerInfo.GetRequestHandlerUrlInfos()
                    .Select(h =>
                    {
                        var methods = string.Join(',',
                            _requestHandlerInfo.GetAreaInfos().SelectMany(a => a.Handlers)
                                .Single(ha => ha.MethodInfo == h.Key).SupportedHttpMethods ?? new string[0]);
                        return new
                        {
                            Url = $"{h.Value}{(!methods.IsNullOrEmpty() ? $" ({methods})" : null)}",
                            Info =
                                $"{(h.Key.GetCustomAttribute(typeof(RequestHandlerIdentificationAttribute)) as RequestHandlerIdentificationAttribute)?.UniqueKey}|{h.Key.DeclaringType.FullName}|{h.Key.ToString()}",
                            Type = h.Key.DeclaringType
                        };
                    }).OrderBy(i=>i.Url),
                "Info",
                "Url",
                null,
                "Type");

            return View();
        }

        // POST: Manage/RequestAuthorizationRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HandlerMethodSignature,TypeFullName,IdentificationKey,AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] AuthorizationRule requestAuthorizationRule)
        {
            if (ModelState.IsValid)
            {
                requestAuthorizationRule.Id = Guid.NewGuid();
                _context.Add(requestAuthorizationRule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestAuthorizationRule);
        }

        // GET: Manage/RequestAuthorizationRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestAuthorizationRule = await _context.RequestAuthorizationRules.FindAsync(id);
            if (requestAuthorizationRule == null)
            {
                return NotFound();
            }
            return View(requestAuthorizationRule);
        }

        // POST: Manage/RequestAuthorizationRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("HandlerMethodSignature,TypeFullName,IdentificationKey,AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] AuthorizationRule requestAuthorizationRule)
        {
            if (id != requestAuthorizationRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestAuthorizationRule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestAuthorizationRuleExists(requestAuthorizationRule.Id))
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
            return View(requestAuthorizationRule);
        }

        // GET: Manage/RequestAuthorizationRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestAuthorizationRule = await _context.RequestAuthorizationRules
                .Include(r => r.CreationUser)
                .Include(r => r.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestAuthorizationRule == null)
            {
                return NotFound();
            }

            return View(requestAuthorizationRule);
        }

        // POST: Manage/RequestAuthorizationRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var requestAuthorizationRule = await _context.RequestAuthorizationRules.FindAsync(id);
            _context.RequestAuthorizationRules.Remove(requestAuthorizationRule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestAuthorizationRuleExists(Guid id)
        {
            return _context.RequestAuthorizationRules.Any(e => e.Id == id);
        }
    }
}
