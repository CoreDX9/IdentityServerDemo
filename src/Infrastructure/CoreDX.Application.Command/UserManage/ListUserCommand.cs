using CoreDX.Common.Util.QueryHelper;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Model.Command;
using System;
using X.PagedList;

namespace CoreDX.Application.Command.UserManage
{
    public class ListUserCommand : MediatRCommand<IPagedList<ApplicationUser>>
    {
        public PageInfo PageInfo { get; }
        public QueryFilter QueryFilter { get; }
        public ListUserCommand(PageInfo pageInfo, QueryFilter queryFilter)
        {
            PageInfo = pageInfo;
            QueryFilter = queryFilter;
        }
    }
}
