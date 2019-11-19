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
    public class RolePermissionDeclarationController : Controller
    {
        private readonly ApplicationPermissionDbContext _permissionDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;

        public RolePermissionDeclarationController(ApplicationPermissionDbContext permissionDbContext, ApplicationIdentityDbContext identityDbContext)
        {
            _permissionDbContext = permissionDbContext;
            _identityDbContext = identityDbContext;
        }

        // GET: Manage/RolePermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _permissionDbContext.RolePermissionDeclarations.Include(r => r.PermissionDefinition);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/RolePermissionDeclaration/Details/5
        public async Task<IActionResult> Details(int? permissionDefinitionId, int? roleId)
        {
            if (permissionDefinitionId == null || roleId == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _permissionDbContext.RolePermissionDeclarations
                .Include(r => r.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.PermissionDefinitionId == permissionDefinitionId  && m.RoleId == roleId);
            if (rolePermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["PermissionDefinition"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name");
            ViewData["Role"] = new SelectList(_identityDbContext.Roles, "Id", "Name");
            return View();
        }

        // POST: Manage/RolePermissionDeclaration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] RolePermissionDeclaration rolePermissionDeclaration)
        {
            if (ModelState.IsValid)
            {
                _permissionDbContext.Add(rolePermissionDeclaration);
                await _permissionDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionDefinition"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_identityDbContext.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _permissionDbContext.RolePermissionDeclarations.FindAsync(id);
            if (rolePermissionDeclaration == null)
            {
                return NotFound();
            }
            ViewData["PermissionDefinition"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_identityDbContext.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // POST: Manage/RolePermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int permissionDefinitionId, [Bind("RoleId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] RolePermissionDeclaration rolePermissionDeclaration)
        {
            if (permissionDefinitionId != rolePermissionDeclaration.PermissionDefinitionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _permissionDbContext.Update(rolePermissionDeclaration);
                    await _permissionDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolePermissionDeclarationExists(rolePermissionDeclaration.PermissionDefinitionId))
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
            ViewData["PermissionDefinition"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", rolePermissionDeclaration.PermissionDefinitionId);
            ViewData["Role"] = new SelectList(_identityDbContext.Roles, "Id", "Name", rolePermissionDeclaration.RoleId);
            return View(rolePermissionDeclaration);
        }

        // GET: Manage/RolePermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolePermissionDeclaration = await _permissionDbContext.RolePermissionDeclarations
                .Include(r => r.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.PermissionDefinitionId == id);
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
            var rolePermissionDeclaration = await _permissionDbContext.RolePermissionDeclarations.FindAsync(id);
            _permissionDbContext.RolePermissionDeclarations.Remove(rolePermissionDeclaration);
            await _permissionDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolePermissionDeclarationExists(int? id)
        {
            return _permissionDbContext.RolePermissionDeclarations.Any(e => e.PermissionDefinitionId == id);
        }
    }
}
