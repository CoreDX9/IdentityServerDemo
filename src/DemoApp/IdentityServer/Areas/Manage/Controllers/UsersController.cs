using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreDX.Domain.Core.Command;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Service.UserManage;
using IdentityServer.Areas.Manage.Models.Users;
using IdentityServer.HttpHandlerBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using CoreDX.Common.Util.QueryHelper;

namespace IdentityServer.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommandBus<ListUserCommand, IPagedList<ApplicationUser>> _commandBus;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, ICommandBus<ListUserCommand, IPagedList<ApplicationUser>> commandBus, IMapper mapper)
        {
            _userManager = userManager;
            _commandBus = commandBus;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ActionName("List")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var cmd = new ListUserCommand(1, 10, new QueryFilter());
                var users = await _commandBus.SendCommandAsync(cmd, default);
                var usersQuery = _userManager.Users.AsNoTracking();
                //if (jqGridParameter._search == "true")
                //{
                //    usersQuery = usersQuery.Where(BuildWhere<ApplicationUser>(jqGridParameter.FilterObject, null));
                //}

                //var users = usersQuery
                //    .Include(u => u.UserRoles)
                //    .ThenInclude(ur => ur.Role)
                //    .ToPagedList(jqGridParameter.Page, jqGridParameter.Rows);

                return new JsonResult(
                    new
                    {
                        rows = users.Select(u => _mapper.Map<ApplicationUserDto>(u)),
                        total = users.PageCount, //总页数
                    page = users.PageNumber, //当前页码
                    records = users.TotalItemCount //总记录数
                }
                );
            }
            catch(System.Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreateAsync(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Content("添加成功");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToPage("./ResetPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Content("");
        }
    }
}