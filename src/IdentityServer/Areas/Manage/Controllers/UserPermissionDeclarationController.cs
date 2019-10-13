using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class UserPermissionDeclarationController : Controller
    {
        private readonly ApplicationIdentityDbContext _context;

        public UserPermissionDeclarationController(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        // GET: Manage/UserPermissionDeclaration
        public async Task<IActionResult> Index()
        {
            var applicationIdentityDbContext = _context.UserPermissionDeclarations.Include(u => u.CreationUser).Include(u => u.LastModificationUser).Include(u => u.PermissionDefinition).Include(u => u.User);
            return View(await applicationIdentityDbContext.ToListAsync());
        }

        // GET: Manage/UserPermissionDeclaration/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _context.UserPermissionDeclarations
                .Include(u => u.CreationUser)
                .Include(u => u.LastModificationUser)
                .Include(u => u.PermissionDefinition)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPermissionDeclaration == null)
            {
                return NotFound();
            }

            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Create
        public IActionResult Create()
        {
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
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
                userPermissionDeclaration.Id = Guid.NewGuid();
                _context.Add(userPermissionDeclaration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _context.UserPermissionDeclarations.FindAsync(id);
            if (userPermissionDeclaration == null)
            {
                return NotFound();
            }
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // POST: Manage/UserPermissionDeclaration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,PermissionValue,PermissionDefinitionId,Id,Remark,OrderNumber,RowVersion,IsEnable,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] UserPermissionDeclaration userPermissionDeclaration)
        {
            if (id != userPermissionDeclaration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPermissionDeclaration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPermissionDeclarationExists(userPermissionDeclaration.Id))
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
            ViewData["PermissionDefinitionId"] = new SelectList(_context.PermissionDefinitions, "Id", "Name", userPermissionDeclaration.PermissionDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", userPermissionDeclaration.UserId);
            return View(userPermissionDeclaration);
        }

        // GET: Manage/UserPermissionDeclaration/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPermissionDeclaration = await _context.UserPermissionDeclarations
                .Include(u => u.CreationUser)
                .Include(u => u.LastModificationUser)
                .Include(u => u.PermissionDefinition)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var userPermissionDeclaration = await _context.UserPermissionDeclarations.FindAsync(id);
            _context.UserPermissionDeclarations.Remove(userPermissionDeclaration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPermissionDeclarationExists(Guid id)
        {
            return _context.UserPermissionDeclarations.Any(e => e.Id == id);
        }
    }
}
