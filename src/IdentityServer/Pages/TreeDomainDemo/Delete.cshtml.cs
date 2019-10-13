using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.HttpHandlerBase;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class DeleteModel : PageModelBase
    {
        private readonly ApplicationIdentityDbContext _context;

        public DeleteModel(ApplicationIdentityDbContext context)
        {
            _context = context;
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFoundView();
            }

            TreeDomain = await _context.TreeDomains.FindAsync(id);

            if (TreeDomain != null)
            {
                _context.TreeDomains.Remove(TreeDomain);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
