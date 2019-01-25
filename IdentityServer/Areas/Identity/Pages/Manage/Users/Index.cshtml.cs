using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Identity;
using IdentityServer.HttpHandlerBase;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.EntityFrameworkCore.Identity;
using static IdentityServer.Extensions.JqGridSearchExtensions;
using static System.Math;

namespace IdentityServer.Areas.Identity.Pages.Manage.Users
{
    public class IndexModel : PageModelBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationIdentityDbContext _dbContext;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationIdentityDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            //[Required]
            public string UserName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            //var userName = await _userManager.GetUserNameAsync(user);
            //var email = await _userManager.GetEmailAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //Username = userName;

            //Input = new InputModel
            //{
            //    Email = email,
            //    UserName = userName,
            //    PhoneNumber = phoneNumber
            //};

            //IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnGetUserListAsync([FromQuery]JqGridParameter jqGridParameter)
        {
            var usersQuery = _userManager.Users.AsNoTracking();
            if (jqGridParameter._search == "true")
            {
                usersQuery = usersQuery.Where(BuildWhere<ApplicationUser>(jqGridParameter.FilterObject, null));
            }

            var users = usersQuery.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).OrderBy(u => u.OrderNumber)
                .Skip((jqGridParameter.Page - 1) * jqGridParameter.Rows).Take(jqGridParameter.Rows).ToList();
            var userCount = usersQuery.Count();
            var pageCount = Ceiling((double) userCount / jqGridParameter.Rows);
            return new JsonResult(
                new
                {
                    rows //数据集合
                        = users.Select(u => new
                        {
                            u.UserName,
                            u.Sex,
                            u.Email,
                            u.PhoneNumber,
                            u.EmailConfirmed,
                            u.PhoneNumberConfirmed,
                            u.CreationTime,
                            u.CreationUserId,
                            u.IsEnable,
                            u.LastModificationTime,
                            u.LastModificationUserId,
                            u.OrderNumber,
                            u.RowVersion,
                            //以下为JqGrid中必须的字段
                            u.Id //记录的唯一标识，可在插件中配置为其它字段，但是必须能作为记录的唯一标识用，不能重复
                        }),
                    total = pageCount, //总页数
                    page = jqGridParameter.Page, //当前页码
                    records = userCount //总记录数
                }
            );
        }
    }
}
