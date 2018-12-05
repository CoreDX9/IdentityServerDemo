using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Domain.Identity;
using IdentityServer.HttpHandlerBase;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer.Areas.Identity.Pages.Manage.Roles
{
    public class CreateModel : PageModelBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            public Guid? ParentId { get; set; }

            //只是拿来显示标签用的
            public int Parent { get; set; }
        }

        public void OnGet()
        {
            ViewData["Parent"] = new SelectList(_roleManager.Roles, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = new Guid(User.GetSubjectId());
                var role = new ApplicationRole(Input.Name)
                {
                    ParentId = Input.ParentId,
                    CreationUserId = userId,
                    LastModificationUserId = userId
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToPage("./Index");
                }
            }

            return Page();
        }
    }
}