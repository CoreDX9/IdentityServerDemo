using System.Collections.Generic;
using System.Linq;
using Util.Hierarchical;
using Util.TypeExtensions;

namespace Entity
{
    /// <summary>
    /// 树形实体扩展
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// 转换为可分层数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="root">根</param>
        /// <returns>已分层的数据</returns>
        public static IHierarchical<ITree<T>> AsHierarchical<T>(this ITree<T> root)
            where T : ITree<T>
        {
            return root.AsHierarchical(entity => entity.Children.OfType<ITree<T>>());
        }

        /// <summary>
        /// 转换为可枚举集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="enumerateType">枚举方式</param>
        /// <returns>已枚举的集合</returns>
        public static IEnumerable<T> AsEnumerable<T>(this ITree<T> root,
            EnumerateType enumerateType = EnumerateType.DfsDlr)
            where T : ITree<T>
        {
            foreach (var item in root.AsHierarchical().AsEnumerable(enumerateType))
            {
                yield return (T)item.Current;
            }
        }
    }
}
