using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Model.Entity.Security;

namespace CoreDX.Domain.Model.Entity.Identity
{
    public class Organization : Organization<Guid, Organization, ApplicationUser>
    , IStorageOrderRecordable
    {
        public long InsertOrder { get; set; }
    }

    public abstract class Organization<TKey, TEntity, TIdentityUser> : DomainTreeEntityBase<TKey, TEntity, TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
        where TEntity : Organization<TKey, TEntity, TIdentityUser>
    {
        /// <summary>
        /// 需要使用.Include(o => o.UserOrganizations).ThenInclude(uo => uo.User)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<TIdentityUser> Users => UserOrganizations?.Select(uo => uo.User);

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<ApplicationUserOrganization<TKey, TIdentityUser, TEntity>> UserOrganizations { get; set; } =
            new List<ApplicationUserOrganization<TKey, TIdentityUser, TEntity>>();
    }
}
