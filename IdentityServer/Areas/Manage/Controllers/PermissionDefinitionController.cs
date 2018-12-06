using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Security;
using Repository.EntityFrameworkCore.Identity;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class PermissionDefinitionController : Controller
    {
        private readonly ApplicationIdentityDbContext _context;

        public PermissionDefinitionController(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        // GET: Manage/PermissionDefinition
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _context.PermissionDefinitions.Include(p => p.CreationUser).Include(p => p.LastModificationUser);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/PermissionDefinition/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _context.PermissionDefinitions
                .Include(p => p.CreationUser)
                .Include(p => p.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissionDefinition == null)
            {
                return NotFound();
            }

            return View(permissionDefinition);
        }

        // GET: Manage/PermissionDefinition/Create
        public IActionResult Create()
        {
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Manage/PermissionDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ValueType,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] PermissionDefinition permissionDefinition)
        {
            if (ModelState.IsValid)
            {
                permissionDefinition.Id = Guid.NewGuid();
                _context.Add(permissionDefinition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // GET: Manage/PermissionDefinition/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _context.PermissionDefinitions.FindAsync(id);
            if (permissionDefinition == null)
            {
                return NotFound();
            }
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // POST: Manage/PermissionDefinition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,ValueType,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] PermissionDefinition permissionDefinition)
        {
            if (id != permissionDefinition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permissionDefinition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionDefinitionExists(permissionDefinition.Id))
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
            ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreationUserId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // GET: Manage/PermissionDefinition/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _context.PermissionDefinitions
                .Include(p => p.CreationUser)
                .Include(p => p.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissionDefinition == null)
            {
                return NotFound();
            }

            return View(permissionDefinition);
        }

        // POST: Manage/PermissionDefinition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var permissionDefinition = await _context.PermissionDefinitions.FindAsync(id);
            _context.PermissionDefinitions.Remove(permissionDefinition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionDefinitionExists(Guid id)
        {
            return _context.PermissionDefinitions.Any(e => e.Id == id);
        }
    }
}
