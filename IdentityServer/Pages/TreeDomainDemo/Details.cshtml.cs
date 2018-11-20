using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Sample;
using IdentityServer.HttpHandlerBase;
using Repository.EntityFrameworkCore.Identity;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class DetailsModel : PageModelBase
    {
        private readonly Repository.EntityFrameworkCore.Identity.ApplicationIdentityDbContext _context;

        public DetailsModel(Repository.EntityFrameworkCore.Identity.ApplicationIdentityDbContext context)
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
