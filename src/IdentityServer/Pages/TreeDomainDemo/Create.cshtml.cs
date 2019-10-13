using System;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Model.Entity.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class CreateModel : PageModelBase
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["LastModificationUserId"] = new SelectList(_context.Users, "Id", "Id");
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
            
            TreeDomain.CreatorId = new Guid(HttpContext.User.GetSubjectId());
            TreeDomain.LastModificationUserId = new Guid(HttpContext.User.GetSubjectId());
            _context.TreeDomains.Add(TreeDomain);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}