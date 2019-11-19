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
    public class OrganizationPermissionDeclarationController : Controller
    {
        private readonly ApplicationPermissionDbContext _permissionDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;

        public OrganizationPermissionDeclarationController(ApplicationPermissionDbContext permissionDbContext, ApplicationIdentityDbContext identityDbContext)
        {
            _permissionDbContext = permissionDbContext;
            _identityDbContext = identityDbContext;
        }

        // GET: Manage/OrganizationPermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _permissionDbContext.OrganizationPermissionDeclarations.Include(o => o.PermissionDefinition);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/OrganizationPermissionDeclaration/Details/5
        public async Task<IActionResult> Details(int? organizationId, int? permissionDefinitionId)
        {
            if (organizationId == null || permissionDefinitionId == null)
            {
                return NotFound();
            }

            var organizationPermissionDeclaration = await _permissionDbContext.OrganizationPermissionDeclarations
                .Include(o => o.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.PermissionDefinitionId == permissionDefinitionId);
            if (organizationPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(organizationPermissionDeclaration);
        }

        // GET: Manage/OrganizationPermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["OrganizationId"] = new SelectList(_identityDbContext.Organizations, "Id", "Name");
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name");
            return View();
        }

        // POST: Manage/OrganizationPermissionDeclaration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrganizationId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] OrganizationPermissionDeclaration organizationPermissionDeclaration)
        {
            if (ModelState.IsValid)
            {
                organizationPermissionDeclaration.OrganizationId = organizationPermissionDeclaration.OrganizationId;
                organizationPermissionDeclaration.PermissionDefinitionId = organizationPermissionDeclaration.PermissionDefinitionId;
                _permissionDbContext.Add(organizationPermissionDeclaration);
                await _permissionDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(_identityDbContext.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
            ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
            return View(organizationPermissionDeclaration);
        }

        // GET: Manage/OrganizationPermissionDeclaration/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var organizationPermissionDeclaration = await _permissionDbContext.OrganizationPermissionDeclarations.FindAsync(id);
        //    if (organizationPermissionDeclaration == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["OrganizationId"] = new SelectList(_identityDbContext.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
        //    ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
        //    return View(organizationPermissionDeclaration);
        //}

        // POST: Manage/OrganizationPermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("OrganizationId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] OrganizationPermissionDeclaration organizationPermissionDeclaration)
        //{
        //    if (id != organizationPermissionDeclaration.id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _permissionDbContext.Update(organizationPermissionDeclaration);
        //            await _permissionDbContext.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrganizationPermissionDeclarationExists(organizationPermissionDeclaration.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["OrganizationId"] = new SelectList(_permissionDbContext.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
        //    ViewData["PermissionDefinitionId"] = new SelectList(_permissionDbContext.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
        //    return View(organizationPermissionDeclaration);
        //}

        // GET: Manage/OrganizationPermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(int? organizationId, int? permissionDefinitionId)
        {
            if (organizationId == null || permissionDefinitionId == null)
            {
                return NotFound();
            }

            var organizationPermissionDeclaration = await _permissionDbContext.OrganizationPermissionDeclarations
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.PermissionDefinitionId == permissionDefinitionId);
            if (organizationPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(organizationPermissionDeclaration);
        }

        // POST: Manage/OrganizationPermissionDeclaration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var organizationPermissionDeclaration = await _permissionDbContext.OrganizationPermissionDeclarations.FindAsync(id);
            _permissionDbContext.OrganizationPermissionDeclarations.Remove(organizationPermissionDeclaration);
            await _permissionDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool OrganizationPermissionDeclarationExists(Guid id)
        //{
        //    return _permissionDbContext.OrganizationPermissionDeclarations.Any(e => e.Id == id);
        //}
    }
}
