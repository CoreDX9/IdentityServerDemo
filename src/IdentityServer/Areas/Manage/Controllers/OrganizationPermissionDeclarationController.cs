using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class OrganizationPermissionDeclarationController : Controller
    {
        private readonly ApplicationIdentityDbContext _context;

        public OrganizationPermissionDeclarationController(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        // GET: Manage/OrganizationPermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _context.OrganizationPermissionDeclarations.Include(o => o.CreationUser).Include(o => o.LastModificationUser).Include(o => o.Organization).Include(o => o.PermissionDefinition);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/OrganizationPermissionDeclaration/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationPermissionDeclaration = await _context.OrganizationPermissionDeclarations
                .Include(o => o.CreationUser)
                .Include(o => o.LastModificationUser)
                .Include(o => o.Organization)
                .Include(o => o.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (organizationPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(organizationPermissionDeclaration);
        }

        // GET: Manage/OrganizationPermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name");
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name");
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
                organizationPermissionDeclaration.Id = Guid.NewGuid();
                _context.Add(organizationPermissionDeclaration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
            return View(organizationPermissionDeclaration);
        }

        // GET: Manage/OrganizationPermissionDeclaration/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationPermissionDeclaration = await _context.OrganizationPermissionDeclarations.FindAsync(id);
            if (organizationPermissionDeclaration == null)
            {
                return NotFound();
            }
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
            return View(organizationPermissionDeclaration);
        }

        // POST: Manage/OrganizationPermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OrganizationId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] OrganizationPermissionDeclaration organizationPermissionDeclaration)
        {
            if (id != organizationPermissionDeclaration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(organizationPermissionDeclaration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationPermissionDeclarationExists(organizationPermissionDeclaration.Id))
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
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", organizationPermissionDeclaration.OrganizationId);
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", organizationPermissionDeclaration.PermissionDefinitionId);
            return View(organizationPermissionDeclaration);
        }

        // GET: Manage/OrganizationPermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizationPermissionDeclaration = await _context.OrganizationPermissionDeclarations
                .Include(o => o.CreationUser)
                .Include(o => o.LastModificationUser)
                .Include(o => o.Organization)
                .Include(o => o.PermissionDefinition)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var organizationPermissionDeclaration = await _context.OrganizationPermissionDeclarations.FindAsync(id);
            _context.OrganizationPermissionDeclarations.Remove(organizationPermissionDeclaration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationPermissionDeclarationExists(Guid id)
        {
            return _context.OrganizationPermissionDeclarations.Any(e => e.Id == id);
        }
    }
}
