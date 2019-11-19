using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.HttpHandlerBase;
using CoreDX.Domain.Entity.App.Sample;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class DetailsModel : PageModelBase
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
