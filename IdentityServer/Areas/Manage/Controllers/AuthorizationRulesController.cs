using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Security;
using Repository.EntityFrameworkCore;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthorizationRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manage/AuthorizationRules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AuthorizationRules.Include(a => a.CreationUser).Include(a => a.LastModificationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manage/AuthorizationRules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _context.AuthorizationRules
                .Include(a => a.CreationUser)
                .Include(a => a.LastModificationUser)
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
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Manage/AuthorizationRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] AuthorizationRule authorizationRule)
        {
            if (ModelState.IsValid)
            {
                authorizationRule.Id = Guid.NewGuid();
                _context.Add(authorizationRule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.LastModificationUserId);
            return View(authorizationRule);
        }

        // GET: Manage/AuthorizationRules/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _context.AuthorizationRules.FindAsync(id);
            if (authorizationRule == null)
            {
                return NotFound();
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.LastModificationUserId);
            return View(authorizationRule);
        }

        // POST: Manage/AuthorizationRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AuthorizationRuleConfigJson,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] AuthorizationRule authorizationRule)
        {
            if (id != authorizationRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authorizationRule);
                    await _context.SaveChangesAsync();
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
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", authorizationRule.LastModificationUserId);
            return View(authorizationRule);
        }

        // GET: Manage/AuthorizationRules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizationRule = await _context.AuthorizationRules
                .Include(a => a.CreationUser)
                .Include(a => a.LastModificationUser)
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
            var authorizationRule = await _context.AuthorizationRules.FindAsync(id);
            _context.AuthorizationRules.Remove(authorizationRule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorizationRuleExists(Guid id)
        {
            return _context.AuthorizationRules.Any(e => e.Id == id);
        }
    }
}
