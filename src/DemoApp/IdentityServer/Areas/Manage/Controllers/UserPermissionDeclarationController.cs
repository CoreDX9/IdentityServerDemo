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
    public class UserPermissionDeclarationController : Controller
    {
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly ApplicationPermissionDbContext _permissionDbContext;

        public UserPermissionDeclarationController(ApplicationIdentityDbContext identityDbContext, ApplicationPermissionDbContext permissionDbContext)
        {
            _identityDbContext = identityDbContext;
            _permissionDbContext = permissionDbContext;
        }

        // GET: Manage/UserPermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _permissionDbContext.UserPermissionDeclarations.Include(u => u.PermissionDefinition);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/UserPermissionDeclaration/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _permissionDbContext.UserPermissionDeclarations
                .Include(u => u.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.PermissionDefinitionId == id);
            if (userPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name");
            ViewData["UserId"] = new SelectList(_identityDbContext.Users, "Id", "UserName");
            return View();
        }

        // POST: Manage/UserPermissionDeclaration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] UserPermissionDeclaration userPermissionDeclaration)
        {
            if (ModelState.IsValid)
            {
                _permissionDbContext.Add(userPermissionDeclaration);
                await _permissionDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_identityDbContext.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _permissionDbContext.UserPermissionDeclarations.FindAsync(id);
            if (userPermissionDeclaration == null)
            {
                return NotFound();
            }
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_identityDbContext.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // POST: Manage/UserPermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] UserPermissionDeclaration userPermissionDeclaration)
        {
            if (id != userPermissionDeclaration.PermissionDefinitionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _permissionDbContext.Update(userPermissionDeclaration);
                    await _permissionDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPermissionDeclarationExists(userPermissionDeclaration.PermissionDefinitionId))
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
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_identityDbContext.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _permissionDbContext.UserPermissionDeclarations
                .Include(u => u.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.PermissionDefinitionId == id);
            if (userPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(userPermissionDeclaration);
        }

        // POST: Manage/UserPermissionDeclaration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userPermissionDeclaration = await _permissionDbContext.UserPermissionDeclarations.FindAsync(id);
            _permissionDbContext.UserPermissionDeclarations.Remove(userPermissionDeclaration);
            await _permissionDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPermissionDeclarationExists(int? id)
        {
            return _permissionDbContext.UserPermissionDeclarations.Any(e => e.PermissionDefinitionId == id);
        }
    }
}
