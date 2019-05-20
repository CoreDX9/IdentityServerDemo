using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Sample;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Repository.EntityFrameworkCore;

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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            TreeDomain = await _context.TreeDomains
                .Include(t => t.CreationUser)
                .Include(t => t.LastModificationUser)
                .Include(t => t.Parent).FirstOrDefaultAsync(m => m.Id == id);

            if (TreeDomain == null)
            {
                return NotFoundView();
            }
           ViewData["CreationUserId"] = new SelectList(_context.Users, "Id", "UserName");
           ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "UserName");
           ViewData["ParentId"] = new SelectList(_context.TreeDomains, "Id", "SampleColumn");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TreeDomain.LastModificationUserId = new Guid(HttpContext.User.GetSubjectId());
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

        private bool TreeDomainExists(Guid id)
        {
            return _context.TreeDomains.Any(e => e.Id == id);
        }
    }
}
