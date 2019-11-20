using System.Linq;
using System.Threading.Tasks;
using CoreDX.Application.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.HttpHandlerBase;
using X.PagedList;
using CoreDX.Domain.Entity.App.Sample;
using CoreDX.Common.Util.TypeExtensions;

namespace IdentityServer.Pages.TreeDomainDemo
{
    public class IndexModel : PageModelBase
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPagedList<TreeDomain> TreeDomain { get;set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int ItemCount { get; set; }

        public async Task OnGetAsync(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            ItemCount = _context.TreeDomains.Count();
        }

        public async Task<IActionResult> OnGetVueDataAsync(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;

            TreeDomain = await _context.TreeDomains.ToPagedListAsync(pageIndex, pageSize);

            var model = TreeDomain.Select(t => new
            {
                t.SampleColumn,
                t.ParentId,
                t.IsDeleted,
                CreationTime = t.CreationTime.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                LastModificationTime = t.LastModificationTime.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                t.CreatorId,
                t.LastModifierId,
                EditLink = Url.Page("Edit",new{id = t.Id}),
                DetailsLink = Url.Page("Details", new{id = t.Id}),
                DeleteLink = Url.Page("Delete", new{id = t.Id})
            });
            return new JsonResult(model);
            //.Include(t => t.CreationUser)
            //.Include(t => t.LastModificationUser)
            //.Include(t => t.Parent).ToListAsync();
        }

        public IActionResult OnGetTreeGridData()
        {
            //从视图返回数据
            var data = _context.TreeDomainViews.ToList();

            #region 重建树形结构导航属性（ef不能为视图建立外键，无法自动恢复导航属性）

            data.ForEach(d=>
            {
                if (!d.IsRoot)
                    d.Parent = data.FirstOrDefault(p => p.Id == d.ParentId);
                if (d.HasChildren)
                    foreach (var t in data.Where(c=>c.ParentId == d.Id))
                    {
                        d.Children.Add(t);
                    }
            });

            #endregion

            #region 树后端排序

            //使用$('jqGrid').jqGrid('sortGrid', 'sampleColumn');可以调用插件排序前端
            //将数据转换为可分层数据然后执行深度优先遍历排序（不然插件首次加载数据时会出现显示错乱，虽然点击列名进行排序可以修复错乱问题，但是还是不太好）
            //先找出根节点（ParentId为空）
            //转换为可分层接口，下层节点选择器为data中ParentId为传入节点Id的节点集合
            //转换为可枚举接口（不对子节点进行筛选，第二个参数为null）
            //选出元素的Current属性，将数据类型还原为原始类型集合（转换为可分层接口时原始数据元素被包装到Current属性中了）
            //如果存在超过1个根节点，数据是森林而不是一颗树

            //找出所有树根
            var roots = data.Where(d => d.ParentId == null);

            //填充第一棵树
            var sortedData = roots.FirstOrDefault()
                ?.AsHierarchical(p => data.Where(c => c.ParentId == p.Id))
                ?.AsEnumerable()
                ?.Select(h => h.Current);

            //填充剩下的树
            foreach (var root in roots.Skip(1))
            {
                sortedData = sortedData.Union(
                    root.AsHierarchical(p => data.Where(c => c.ParentId == p.Id))
                        .AsEnumerable()
                        .Select(h => h.Current)
                );
            }

            #endregion

            return new JsonResult(
                new
                {
                    rows //数据集合
                        = sortedData?.Where(d => d != null).Select(d => new
                        {
                            d.SampleColumn,
                            d.CreationTime,
                            d.CreatorId,
                            d.HasChildren,
                            d.LastModificationTime,
                            d.Path,
                            //以下为JqGrid的TreeGrid组件在邻接表模型中必须的字段
                            d.Id, //记录的唯一标识，就算不是TreeGrid也必须有，可在插件中配置为其它字段，但是必须能作为记录的唯一标识用，不能重复
                            d.ParentId, //上层节点唯一标识，可配置为其它字段
                            d.Depth, //所在层级，默认以0位最上层，插件默认字段名为level
                            Expanded = true, //标识节点状态是展开还是收起，这个可以没有
                            IsLeaf = !d.HasChildren, //标识节点是否为叶子节点，插件据此进行按需加载的服务器交互配置并确定节点是否还能继续展开
                            Loaded = true //标识节点的子节点是否已经加载，如果为false，在展开节点时会向服务器请求子节点数据
                        }),
                    total = 1, //总页数
                    page = 1, //当前页码
                    records = data.Count //总记录数
                }
            );
        }
    }
}
