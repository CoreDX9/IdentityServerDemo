using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Domain.Core.Entity;
using Microsoft.AspNetCore.Identity;
using PropertyChanged;

namespace CoreDX.Domain.Model.Entity.Identity
{
    //实现IDomainTreeEntity<TParentKey, TEntity, TIdentityUserKey>接口后不知道为什么无法为CreationUserId
    //和LastModificationUserId赋值，会报System.Security.VerificationException异常说
    //Method System.Nullable.Equals: type argument 'TEntity' violates the constraint of type parameter 'T'.
    //原因未知，只能放弃实现IDomainTreeEntity<TParentKey, TEntity, TIdentityUserKey>接口
    //先在基类实现IDomainEntity<TIdentityUserKey>接口，再在最终实体类实现ITree<T>接口
    public class ApplicationRole : ApplicationRole<Guid, ApplicationUser, ApplicationRole>
        , IStorageOrderRecordable
    {
        public ApplicationRole() => Id = Guid.NewGuid();

        public ApplicationRole(string roleName)
            : this() => Name = roleName;

        public virtual long InsertOrder { get; set; }
    }

    public abstract class ApplicationRole<TKey, TIdentityUser, TIdentityRole> : IdentityRole<TKey>
        , IOptimisticConcurrencySupported
        , IDomainTreeEntity<TKey, TIdentityRole>
        , ICreatorRecordable<TKey, TIdentityUser>
        , ILastModifierRecordable<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
        where TIdentityRole : ApplicationRole<TKey, TIdentityUser, TIdentityRole>
    {
        public string Description { get; set; }

        /// <summary>
        /// 需要使用.Include(r => r.UserRoles).ThenInclude(ur => ur.Role)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<TIdentityUser> Users => UserRoles?.Select(ur => ur.User);

        #region 导航属性

        public virtual List<ApplicationUserRole<TKey, TIdentityUser, TIdentityRole>> UserRoles { get; set; } = new List<ApplicationUserRole<TKey, TIdentityUser, TIdentityRole>>();

        public virtual List<ApplicationRoleClaim<TKey, TIdentityUser, TIdentityRole>> RoleClaims { get; set; } = new List<ApplicationRoleClaim<TKey, TIdentityUser, TIdentityRole>>();

        #endregion

        public override TKey Id { get; set; }
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        #region IDomainTreeEntity成员

        public virtual TKey? ParentId { get; set; }

        #endregion

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

        #region ITree成员

        public virtual TIdentityRole Parent { get; set; }

        public virtual IList<TIdentityRole> Children { get; set; }

        [DoNotNotify, NotMapped]
        public virtual int Depth => Parent?.Depth + 1 ?? 0;

        [DoNotNotify, NotMapped]
        public virtual bool IsRoot => Parent == null;

        [DoNotNotify, NotMapped]
        public virtual bool IsLeaf => Children?.Count == 0;

        [DoNotNotify, NotMapped]
        public virtual bool HasChildren => !IsLeaf;

        [DoNotNotify, NotMapped]
        public virtual string Path => Parent == null ? Id.ToString() : $@"{Parent.Path}/{Id}";

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
        protected ApplicationRole()
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

        #region 重写 Equals

        public override bool Equals(object obj)
        {
            var role = obj as ApplicationRole<TKey, TIdentityUser, TIdentityRole>;
            return role != null &&
                   EqualityComparer<TKey>.Default.Equals(Id, role.Id);
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<TKey>.Default.GetHashCode(Id);
        }

        public static bool operator ==(ApplicationRole<TKey, TIdentityUser, TIdentityRole> role1, ApplicationRole<TKey, TIdentityUser, TIdentityRole> role2)
        {
            return EqualityComparer<ApplicationRole<TKey, TIdentityUser, TIdentityRole>>.Default.Equals(role1, role2);
        }

        public static bool operator !=(ApplicationRole<TKey, TIdentityUser, TIdentityRole> role1, ApplicationRole<TKey, TIdentityUser, TIdentityRole> role2)
        {
            return !(role1 == role2);
        }

        #endregion
    }
}
