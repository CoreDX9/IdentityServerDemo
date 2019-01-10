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
    public class RolePermissionDeclarationController : Controller
    {
        private readonly ApplicationIdentityDbContext _context;

        public RolePermissionDeclarationController(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        // GET: Manage/RolePermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _context.RolePermissionDeclarations.Include(r => r.CreationUser).Include(r => r.LastModificationUser).Include(r => r.PermissionDefinition).Include(r => r.Role);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/RolePermissionDeclaration/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _context.RolePermissionDeclarations
                .Include(r => r.CreationUser)
                .Include(r => r.LastModificationUser)
                .Include(r => r.PermissionDefinition)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolePermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["PermissionDefinition"] = new SelectList(_context.PermissionDefinitions, "Id", "Name");
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Manage/RolePermissionDeclaration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] RolePermissionDeclaration rolePermissionDeclaration)
        {
            if (ModelState.IsValid)
            {
                rolePermissionDeclaration.Id = Guid.NewGuid();
                _context.Add(rolePermissionDeclaration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionDefinition"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _context.RolePermissionDeclarations.FindAsync(id);
            if (rolePermissionDeclaration == null)
            {
                return NotFound();
            }
            ViewData["PermissionDefinition"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // POST: Manage/RolePermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RoleId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreationUserId,LastModificationUserId")] RolePermissionDeclaration rolePermissionDeclaration)
        {
            if (id != rolePermissionDeclaration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolePermissionDeclaration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolePermissionDeclarationExists(rolePermissionDeclaration.Id))
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
            ViewData["PermissionDefinition"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_context.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _context.RolePermissionDeclarations
                .Include(r => r.CreationUser)
                .Include(r => r.LastModificationUser)
                .Include(r => r.PermissionDefinition)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolePermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(rolePermissionDeclaration);
        }

        // POST: Manage/RolePermissionDeclaration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rolePermissionDeclaration = await _context.RolePermissionDeclarations.FindAsync(id);
            _context.RolePermissionDeclarations.Remove(rolePermissionDeclaration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolePermissionDeclarationExists(Guid id)
        {
            return _context.RolePermissionDeclarations.Any(e => e.Id == id);
        }
    }
}
