using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using CoreDX.Domain.Entity.Permission;
using System.Linq;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class PermissionDefinitionController : BaseController
    {
        private readonly ApplicationPermissionDbContext _permissionDbContext;
        private readonly ApplicationIdentityDbContext _identityDbContext;

        public PermissionDefinitionController(ApplicationIdentityDbContext identityDbContext, ApplicationPermissionDbContext permissionDbContext)
        {
            _identityDbContext = identityDbContext;
            _permissionDbContext = permissionDbContext;
        }

        // GET: Manage/PermissionDefinition
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _permissionDbContext.PermissionDefinitions;
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/PermissionDefinition/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _permissionDbContext.PermissionDefinitions
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
            //ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Manage/PermissionDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ValueType,Remark,IsEnable,IsDeleted")] PermissionDefinition permissionDefinition)
        {
            if (ModelState.IsValid)
            {
                permissionDefinition.CreatorId = int.Parse(User.GetSubjectId());
                _permissionDbContext.Add(permissionDefinition);
                await _permissionDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreatorId);
            //ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // GET: Manage/PermissionDefinition/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _permissionDbContext.PermissionDefinitions.FindAsync(id);
            if (permissionDefinition == null)
            {
                return NotFound();
            }
            //ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreatorId);
            //ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // POST: Manage/PermissionDefinition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,ValueType,Id,Remark,RowVersion,IsEnable")] PermissionDefinition permissionDefinition)
        {
            if (id != permissionDefinition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _permissionDbContext.Update(permissionDefinition);
                    await _permissionDbContext.SaveChangesAsync();
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
            //ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.CreatorId);
            //ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", permissionDefinition.LastModificationUserId);
            return View(permissionDefinition);
        }

        // GET: Manage/PermissionDefinition/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissionDefinition = await _permissionDbContext.PermissionDefinitions
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
            var permissionDefinition = await _permissionDbContext.PermissionDefinitions.FindAsync(id);
            _permissionDbContext.PermissionDefinitions.Remove(permissionDefinition);
            await _permissionDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionDefinitionExists(int id)
        {
            return _permissionDbContext.PermissionDefinitions.Any(e => e.Id == id);
        }
    }
}
