using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.App.Sample;
using CoreDX.Domain.Entity.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class EditModel : PageModelBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public TreeDomain TreeDomain { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            TreeDomain = await _context.TreeDomains
                .Include(t => t.Parent).FirstOrDefaultAsync(m => m.Id == id);

            if (TreeDomain == null)
            {
                return NotFoundView();
            }
           ViewData["ParentId"] = new SelectList(_context.TreeDomains, "Id", "SampleColumn");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TreeDomain.LastModifierId = int.Parse(HttpContext.User.GetSubjectId());
            _context.Attach(TreeDomain).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreeDomainExists(TreeDomain.Id))
                {
                    return NotFoundView();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TreeDomainExists(int id)
        {
            return _context.TreeDomains.Any(e => e.Id == id);
        }
    }
}
