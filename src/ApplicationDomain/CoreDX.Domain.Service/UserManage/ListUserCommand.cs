using CoreDX.Common.Util.QueryHelper;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Model.Command;
using System;
using X.PagedList;

namespace CoreDX.Domain.Service.UserManage
{
    public class ListUserCommand : MediatRCommand<IPagedList<ApplicationUser>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public QueryFilter QueryFilter { get; }
        public ListUserCommand(int pageNumber, int pageSize, QueryFilter queryFilter)
            : base()
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            QueryFilter = queryFilter;
        }
    }
}
