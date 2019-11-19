using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Entity.Identity;
using IdentityServer.HttpHandlerBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Areas.Identity.Pages.Manage.Roles
{
    public partial class IndexModel : PageModelBase
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
            return Page();
        }

        public async Task<IActionResult> OnGetRoleListAsync()
        {
            var roles = _dbContext.IdentityRoleView.ToList();

            //找出所有树根
            var roots = roles.Where(d => string.IsNullOrEmpty(d.ParentId));

            //填充第一棵树
            var sortedRoles = roots.First()
                .AsHierarchical(p => roles.Where(c => c.ParentId == p.Id))
                .AsEnumerable()
                .Select(h => h.Current);

            //填充剩下的树
            foreach (var root in roots.Skip(1))
            {
                sortedRoles = sortedRoles.Union(
                    root.AsHierarchical(p => roles.Where(c => c.ParentId == p.Id))
                        .AsEnumerable()
                        .Select(h => h.Current)
                );
            }

            return new JsonResult(
                new
                {
                    rows //数据集合
                        = sortedRoles.Any(r=>r != null) ? sortedRoles.Select(r => new
                        {
                            r.Name,
                            r.CreationTime,
                            r.CreatorId,
                            r.Active,
                            r.LastModificationTime,
                            r.ConcurrencyStamp,
                            //以下为JqGrid中必须的字段
                            r.Id, //记录的唯一标识，可在插件中配置为其它字段，但是必须能作为记录的唯一标识用，不能重复
                            r.ParentId, //上层节点唯一标识，可配置为其它字段
                            r.Depth, //所在层级，默认以0位最上层，插件默认字段名为level
                            Expanded = true, //标识节点状态是展开还是收起，这个可以没有
                            IsLeaf = !r.HasChildren, //标识节点是否为叶子节点，插件据此进行按需加载的服务器交互配置并确定节点是否还能继续展开
                            Loaded = true //标识节点的子节点是否已经加载，如果为false，在展开节点时会向服务器请求子节点数据
                        }) : new object(),
                    total = 1, //总页数
                    page = 1, //当前页码
                    records = sortedRoles.Count() //总记录数
                }
            );
        }
    }
}
