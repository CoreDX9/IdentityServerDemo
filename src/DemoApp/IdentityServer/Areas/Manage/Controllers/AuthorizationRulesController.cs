using System;
using System.Linq;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthorizationRulesController : Controller
    {
        private readonly ApplicationIdentityDbContext _identitycontext;
        private readonly ApplicationPermissionDbContext _permissionDbContext;

        public AuthorizationRulesController(ApplicationIdentityDbContext identitycontext, ApplicationPermissionDbContext permissionDbContext)
        {
            _identitycontext = identitycontext;
            _permissionDbContext = permissionDbContext;
        }

        // GET: Manage/AuthorizationRules
        public async Task<IActionResult> Index()
        {
            var ApplicationIdentityDbContext = _permissionDbContext.AuthorizationRules;
            return View(await ApplicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/AuthorizationRules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _permissionDbContext.AuthorizationRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorizationRule == null)
            {
                return NotFound();
            }

            return View(authorizationRule);
        }

        // GET: Manage/AuthorizationRules/Create
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_identitycontext.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_identitycontext.Users, "Id", "Id");
            return View();
        }

        // POST: Manage/AuthorizationRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] AuthorizationRule authorizationRule)
        {
            if (ModelState.IsValid)
            {
                _permissionDbContext.Add(authorizationRule);
                await _permissionDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_identitycontext.Users, "Id", "Id", authorizationRule.CreatorId);
            return View(authorizationRule);
        }

        // GET: Manage/AuthorizationRules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _permissionDbContext.AuthorizationRules.FindAsync(id);
            if (authorizationRule == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_identitycontext.Users, "Id", "Id", authorizationRule.CreatorId);
            return View(authorizationRule);
        }

        // POST: Manage/AuthorizationRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] AuthorizationRule authorizationRule)
        {
            if (id != authorizationRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _permissionDbContext.Update(authorizationRule);
                    await _permissionDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorizationRuleExists(authorizationRule.Id))
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
            ViewData["CreatorId"] = new SelectList(_identitycontext.Users, "Id", "Id", authorizationRule.CreatorId);
            return View(authorizationRule);
        }

        // GET: Manage/AuthorizationRules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _permissionDbContext.AuthorizationRules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorizationRule == null)
            {
                return NotFound();
            }

            return View(authorizationRule);
        }

        // POST: Manage/AuthorizationRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var authorizationRule = await _permissionDbContext.AuthorizationRules.FindAsync(id);
            _permissionDbContext.AuthorizationRules.Remove(authorizationRule);
            await _permissionDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorizationRuleExists(int id)
        {
            return _permissionDbContext.AuthorizationRules.Any(e => e.Id == id);
        }
    }
}
