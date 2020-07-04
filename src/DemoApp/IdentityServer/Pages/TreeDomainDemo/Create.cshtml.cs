using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.App.Sample;
using CoreDX.Domain.Entity.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class CreateModel : PageModelBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            ViewData["Parent"] = new SelectList(_context.TreeDomains, "Id", "SampleColumn");
            return Page();
        }

        [BindProperty]
        public TreeDomain TreeDomain { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(int.TryParse(HttpContext.User.GetSubjectId(), out var subId))
            {
                TreeDomain.CreatorId = subId;
                TreeDomain.LastModifierId = subId;
            }

            _context.TreeDomains.Add(TreeDomain);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}