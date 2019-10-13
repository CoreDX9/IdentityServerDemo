using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Model.Entity.Identity;
using CoreDX.Domain.Model.Entity.Sample;
using IdentityServer.Extensions;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Controllers
{
    public class DomainDemoController : BaseController
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DomainDemoController(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DomainDemo
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            var identityDbContext = _context.Domains.OrderBy(d=>d.OrderNumber);//.Include(d => d.CreationUser).Include(d => d.LastModificationUser);
            var model = await identityDbContext.ToPagedListAsync(pageIndex, pageSize);

            if (this.IsAjaxRequest())
            {
                return PartialView("_IndexTBody", model);
            }
            else
            {
                return View(model);
            }
        }

        // GET: DomainDemo/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var domain = await _context.Domains
                .Include(d => d.CreationUser)
                .Include(d => d.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (domain == null)
            {
                return NotFoundView();
            }

            return View(domain);
        }

        // GET: DomainDemo/Create
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DomainDemo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SampleColumn,Id,OrderNumber,RowVersion,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] Domain domain)
        {
            if (ModelState.IsValid)
            {
                domain.Id = Guid.NewGuid();
                domain.CreatorId = new Guid(HttpContext.User.GetSubjectId());
                //导航属性的优先级高于显式外键属性，如果导航属性与外键属性冲突，以导航属性为准
                //domain.CreationUser = await _userManager.FindByNameAsync("alice");
                _context.Add(domain);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", domain.CreatorId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", domain.LastModificationUserId);
            return View(domain);
        }

        // GET: DomainDemo/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var domain = await _context.Domains.FindAsync(id);
            if (domain == null)
            {
                return NotFoundView();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", domain.CreatorId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", domain.LastModificationUserId);
            return View(domain);
        }

        // POST: DomainDemo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SampleColumn,Id,OrderNumber,RowVersion,IsDeleted,CreationTime,LastModificationTime,CreatorId,LastModificationUserId")] Domain domain)
        {
            if (id != domain.Id)
            {
                return NotFoundView();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    domain.LastModificationUserId = new Guid(HttpContext.User.GetSubjectId());
                    _context.Update(domain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DomainExists(domain.Id))
                    {
                        return NotFoundView();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", domain.CreatorId);
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id", domain.LastModificationUserId);
            return View(domain);
        }

        // GET: DomainDemo/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            var domain = await _context.Domains
                .Include(d => d.CreationUser)
                .Include(d => d.LastModificationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (domain == null)
            {
                return NotFoundView();
            }

            return View(domain);
        }

        // POST: DomainDemo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var domain = await _context.Domains.FindAsync(id);
            _context.Domains.Remove(domain);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DomainExists(Guid id)
        {
            return _context.Domains.Any(e => e.Id == id);
        }
    }
}
