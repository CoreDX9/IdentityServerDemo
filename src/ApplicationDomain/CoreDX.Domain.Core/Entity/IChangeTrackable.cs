using System.Collections.Generic;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 跟踪属性的变更
    /// </summary>
    public interface IPropertyChangeTrackable
    {
        /// <summary>
        /// 判断指定的属性或任意属性是否被变更过
        /// </summary>
        /// <param name="names">指定要判断的属性名数组，如果为空(null)或空数组则表示判断任意属性</param>
        /// <returns>
        ///	<para>如果指定的<paramref name="names"/>参数有值，当只有参数中指定的属性发生过更改则返回真(True)，否则返回假(False)</para>
        ///	<para>如果指定的<paramref name="names"/>参数为空(null)或空数组，当实体中任意属性发生过更改则返回真(True)，否则返回假(False)</para>
        ///	</returns>
        bool HasChanges(params string[] names);

        /// <summary>
        /// 获取实体中发生过变更的属性集
        /// </summary>
        /// <returns>如果实体没有属性发生过变更，则返回空白字典，否则返回被变更过的属性键值对</returns>
        IDictionary<string, object> GetChanges();

        /// <summary>
        /// 重置指定的属性或任意属性变更状态（为未变更）
        /// </summary>
        /// <param name="names">指定要重置的属性名数组，如果为空(null)或空数组则表示重置所有属性的变更状态（为未变更）</param>
        void ResetPropertyChangeStatus(params string[] names);
    }
}
