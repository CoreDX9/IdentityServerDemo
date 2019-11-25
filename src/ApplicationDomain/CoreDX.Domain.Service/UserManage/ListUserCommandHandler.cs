using CoreDX.Application.EntityFrameworkCore;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Model.Command;
using CoreDX.Domain.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;
using CoreDX.Common.Util.TypeExtensions;

namespace CoreDX.Domain.Service.UserManage
{
    public class ListUserCommandHandler : MediatRCommandHandler<ListUserCommand, IPagedList<ApplicationUser>>
    {
        private IEFCoreRepository<ApplicationUser, int, ApplicationIdentityDbContext> repository;

        public ListUserCommandHandler(IEFCoreRepository<ApplicationUser, int, ApplicationIdentityDbContext> repository)
        {
            this.repository = repository;
        }

        public override Task<IPagedList<ApplicationUser>> Handle(ListUserCommand command, CancellationToken cancellationToken = default)
        {
            return repository.Set
                //.Where(command.QueryFilter.FilterObject.BuildWhere<ApplicationUser>())
                .OrderBy(x => x.Id)//这里到时候换成生成的
                .ToPagedListAsync(command.PageNumber, command.PageSize);
        }
    }
}
