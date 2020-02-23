using System;
using System.Collections.Generic;

namespace CoreDX.Common.Util.Hierarchical
{
    /// <summary>
    /// 可分层数据接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IHierarchical<out T>
    {
        /// <summary>
        /// 当前节点的数据
        /// </summary>
        T Current { get; }

        /// <summary>
        /// 根节点
        /// </summary>
        IHierarchical<T> Root { get; }

        /// <summary>
        /// 双亲（父）节点
        /// </summary>
        IHierarchical<T> Parent { get; }

        /// <summary>
        /// 祖先节点集合（按路径距离升序）
        /// </summary>
        IEnumerable<IHierarchical<T>> Ancestors { get; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        IEnumerable<IHierarchical<T>> Children { get; }

        /// <summary>
        /// 后代节点集合（深度优先先序遍历）
        /// </summary>
        IEnumerable<IHierarchical<T>> Descendants { get; }

        /// <summary>
        /// 兄弟节点集合（不包括自身节点）
        /// </summary>
        IEnumerable<IHierarchical<T>> Siblings { get; }

        /// <summary>
        /// 在兄弟节点中的排行
        /// </summary>
        int IndexOfSiblings { get; }

        /// <summary>
        /// 节点的层
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 节点（以当前节点为根的子树）的高度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 节点的度
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// 树（以当前节点为根的子树）的所有节点中度最大的节点的度
        /// </summary>
        int MaxDegreeOfTree { get; }

        /// <summary>
        /// 当前节点是否是根节点
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// 当前节点是否是叶子节点
        /// </summary>
        bool IsLeaf { get; }

        /// <summary>
        /// 当前节点是否有子节点
        /// </summary>
        bool HasChild { get; }

        /// <summary>
        /// 以当前节点为根，相同的子节点选择器新建一个IHierarchical对象
        /// 可以用来新建一个独立的子树或在修改数据源后重新生成树（已经生成的树不会再对数据源的改变做出反应）
        /// </summary>
        /// <returns>新建对象的根</returns>
        IHierarchical<T> BuildNew();

        /// <summary>
        /// 以当前节点为根返回树形排版的结构字符串
        /// </summary>
        /// <param name="formatter">数据对象格式化器（内容要为一行，否则会影响排版）</param>
        /// <param name="convertToSingleLine">处理掉换行符变成单行文本</param>
        /// <returns></returns>
        string ToString(Func<IHierarchical<T>, string> formatter, bool convertToSingleLine = false);
    }
}
