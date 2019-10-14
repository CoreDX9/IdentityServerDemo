using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Domain.Core.Entity;
using CoreDX.EntityFrameworkCore.Extensions.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Domain.Entity.Identity
{
    [DbDescription("性别枚举")]
    public enum Gender
    {
        [DbDescription("男")]
        Male = 1,
        [DbDescription("女")]
        Female = 2
    }

    /// <summary>
    /// 实际使用的用户类，添加自己的属性存储自定义信息
    /// </summary>
    public class ApplicationUser : ApplicationUser<int, ApplicationUser, ApplicationRole, Organization>
    {
        [DbDescription("性别")]
        public virtual Gender? Sex { get; set; }

        public virtual long InsertOrder { get; set; }
    }

    public abstract class ApplicationUser<TKey, TIdentityUser, TIdentityRole, TOrganization> : IdentityUser<TKey>
        , IOptimisticConcurrencySupported
        , IDomainEntity<TKey>
        , ICreatorRecordable<TKey, TIdentityUser>
        , ILastModifierRecordable<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
        where TIdentityRole : IEntity<TKey>
        where TOrganization : Organization<TKey, TOrganization, TIdentityUser>
    {
        /// <summary>
        /// 需要使用.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<TIdentityRole> Roles => UserRoles?.Select(ur => ur.Role);

        /// <summary>
        /// 需要使用.Include(u => u.UserOrganizations).ThenInclude(uo => uo.Organization)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<Organization<TKey, TOrganization, TIdentityUser>> Organizations => UserOrganizations?.Select(uo => uo.Organization);

        #region 导航属性

        public virtual List<ApplicationUserClaim<TKey, TIdentityUser>> Claims { get; set; } = new List<ApplicationUserClaim<TKey, TIdentityUser>>();
        public virtual List<ApplicationUserLogin<TKey, TIdentityUser>> Logins { get; set; } = new List<ApplicationUserLogin<TKey, TIdentityUser>>();
        public virtual List<ApplicationUserToken<TKey, TIdentityUser>> Tokens { get; set; } = new List<ApplicationUserToken<TKey, TIdentityUser>>();
        public virtual List<ApplicationUserRole<TKey, TIdentityUser, TIdentityRole>> UserRoles { get; set; } = new List<ApplicationUserRole<TKey, TIdentityUser, TIdentityRole>>();
        public virtual List<ApplicationUserOrganization<TKey, TIdentityUser, TOrganization>> UserOrganizations { get; set; } = new List<ApplicationUserOrganization<TKey, TIdentityUser, TOrganization>>();

        #endregion

        public override TKey Id { get; set; }
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        #region IEntity成员

        public virtual bool? Active { get; set; } = true;
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
        public virtual DateTimeOffset LastModificationTime { get; set; } = DateTimeOffset.Now;

        #endregion

        #region IDomainEntity成员

        public virtual TKey? CreatorId { get; set; }
        public virtual TIdentityUser Creator { get; set; }
        public virtual TKey? LastModifierId { get; set; }
        public virtual TIdentityUser LastModifier { get; set; }

        #endregion

        #region IPropertyChangeTrackable成员

        private static readonly object Locker = new object();
        private static readonly Dictionary<Type, string[]> PropertyNamesDictionary = new Dictionary<Type, string[]>();

        private readonly BitArray _propertyChangeMask;

        /// <summary>
        /// 全局属性变更通知事件处理器
        /// </summary>
        public static PropertyChangedEventHandler PublicPropertyChangedEventHandler { get; set; }

        /// <summary>
        /// 初始化用于跟踪属性变更所需的属性信息
        /// </summary>
        protected ApplicationUser()
        {
            //判断类型是否已经加入字典
            //将未加入的类型添加进去（一般为该类对象首次初始化时）
            var type = this.GetType();
            if (!PropertyNamesDictionary.ContainsKey(type))
            {
                lock (Locker)
                {
                    if (!PropertyNamesDictionary.ContainsKey(type))
                    {
                        PropertyNamesDictionary.Add(type, type.GetProperties()
                            .OrderBy(property => property.Name)
                            .Select(property => property.Name).ToArray());
                    }
                }
            }

            //初始化属性变更掩码
            _propertyChangeMask = new BitArray(PropertyNamesDictionary[type].Length, false);

            //注册全局属性变更事件处理器
            if (PublicPropertyChangedEventHandler != null)
            {
                PropertyChanged += PublicPropertyChangedEventHandler;
            }
        }

        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 内部属性变更事件处理器
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            //Perform property validation

            _propertyChangeMask[Array.IndexOf(PropertyNamesDictionary[this.GetType()], propertyName)] = true;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 判断指定的属性或任意属性是否被变更过（<see cref="IPropertyChangeTrackable"/>接口的实现）
        /// </summary>
        /// <param name="names">指定要判断的属性名数组，如果为空(null)或空数组则表示判断任意属性。</param>
        /// <returns>
        ///	<para>如果指定的<paramref name="names"/>参数有值，当只有参数中指定的属性发生过更改则返回真(True)，否则返回假(False)；</para>
        ///	<para>如果指定的<paramref name="names"/>参数为空(null)或空数组，当实体中任意属性发生过更改则返回真(True)，否则返回假(False)。</para>
        ///	</returns>
        public bool HasChanges(params string[] names)
        {
            if (!(names?.Length > 0))
            {
                foreach (bool mask in _propertyChangeMask)
                {
                    if (mask == true)
                    {
                        return true;
                    }
                }

                return false;
            }

            var type = this.GetType();
            foreach (var name in names)
            {
                var index = Array.IndexOf(PropertyNamesDictionary[type], name);
                if (index >= 0 && _propertyChangeMask[index] == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实体中发生过变更的属性集（<see cref="IPropertyChangeTrackable"/>接口的实现）
        /// </summary>
        /// <returns>如果实体没有属性发生过变更，则返回空白字典，否则返回被变更过的属性键值对</returns>
        public IDictionary<string, object> GetChanges()
        {
            Dictionary<string, object> changeDictionary = new Dictionary<string, object>();
            var type = this.GetType();
            for (int i = 0; i < _propertyChangeMask.Length; i++)
            {
                if (_propertyChangeMask[i] == true)
                {
                    changeDictionary.Add(PropertyNamesDictionary[type][i],
                        type.GetProperty(PropertyNamesDictionary[type][i])?.GetValue(this));
                }
            }

            return changeDictionary;
        }

        /// <summary>
        /// 重置指定的属性或任意属性变更状态（为未变更）（<see cref="IPropertyChangeTrackable"/>接口的实现）
        /// </summary>
        /// <param name="names">指定要重置的属性名数组，如果为空(null)或空数组则表示重置所有属性的变更状态（为未变更）</param>
        public void ResetPropertyChangeStatus(params string[] names)
        {
            if (names?.Length > 0)
            {
                var type = this.GetType();
                foreach (var name in names)
                {
                    var index = Array.IndexOf(PropertyNamesDictionary[type], name);
                    if (index >= 0)
                    {
                        _propertyChangeMask[index] = false;
                    }
                }
            }
            else
            {
                _propertyChangeMask.SetAll(false);
            }
        }

        #endregion
    }
}
