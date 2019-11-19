using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Model.Entity;

namespace CoreDX.Domain.Entity.Identity
{
    public class Organization : Organization<int, Organization, ApplicationUser, ApplicationUserOrganization>
    , IStorageOrderRecordable
    {
        public long InsertOrder { get; set; }
    }

    public abstract class Organization<TKey, TEntity, TIdentityUser, TUserOrganization> : DomainTreeEntityBase<TKey, TEntity, TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
        where TEntity : Organization<TKey, TEntity, TIdentityUser, TUserOrganization>
        where TUserOrganization : ApplicationUserOrganization<TKey, TIdentityUser, TEntity, TUserOrganization>
    {
        /// <summary>
        /// 需要使用.Include(o => o.UserOrganizations).ThenInclude(uo => uo.User)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<TIdentityUser> Users => UserOrganizations?.Select(uo => uo.User);

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<TUserOrganization> UserOrganizations { get; set; } = new List<TUserOrganization>();
    }
}
