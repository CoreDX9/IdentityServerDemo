using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.HttpHandlerBase;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class DetailsModel : PageModelBase
    {
        private readonly ApplicationIdentityDbContext _context;

        public DetailsModel(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

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
    }
}
