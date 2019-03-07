using System;
using System.Collections.Generic;
using System.Linq;

namespace Util.TypeExtensions
{
    /// <summary>
    /// 枚举方式
    /// </summary>
    public enum EnumerateType : byte
    {
        /// <summary>
        /// 深度优先，先序
        /// </summary>
        DfsDlr,

        /// <summary>
        /// 广度优先（层序）
        /// </summary>
        Bfs
    }

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
        /// 祖先节点集合
        /// </summary>
        IEnumerable<IHierarchical<T>> Ancestors { get; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        IEnumerable<IHierarchical<T>> Children { get; }

        /// <summary>
        /// 后代节点集合
        /// </summary>
        IEnumerable<IHierarchical<T>> Descendant { get; }

        /// <summary>
        /// 兄弟节点集合
        /// </summary>
        IEnumerable<IHierarchical<T>> Sibling { get; }

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
        /// 树（以当前节点为根的子树）的所有节点的度的最大值
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
    }

    /// <summary>
    /// 可分层数据扩展
    /// </summary>
    public static class HierarchicalExtensions
    {
        /// <summary>
        /// 转换为可分层数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="item">数据</param>
        /// <param name="childSelector">下层数据选择器</param>
        /// <returns>已分层的数据</returns>
        public static IHierarchical<T> AsHierarchical<T>(this T item, Func<T, IEnumerable<T>> childSelector)
        {
            return new Hierarchical<T>(item, childSelector);
        }

        /// <summary>
        /// 获取从根到节点的路径（中顺序经过的节点集合）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="node">节点</param>
        /// <returns>路径中按经过顺序组成的节点集合</returns>
        public static IEnumerable<IHierarchical<T>> GetPathFromRoot<T>(this IHierarchical<T> node) =>
            node.Ancestors.Reverse();

        //public static IEnumerable<IHierarchical<T>> GetPathFromNode<T>(this IHierarchical<T> node, IHierarchical<T> from)
        //public static IEnumerable<IHierarchical<T>> GetPathToNode<T>(this IHierarchical<T> node, IHierarchical<T> to)
        //public static bool IsDescendantOf<T>(this IHierarchical<T> node, IHierarchical<T> target)
        //public static bool IsAncestorOf<T>(this IHierarchical<T> node, IHierarchical<T> target)

        /// <summary>
        /// 分层数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        private class Hierarchical<T> : IHierarchical<T>
        {
            private readonly object _locker;
            private readonly Func<T, IEnumerable<T>> _childSelector;
            private IEnumerable<IHierarchical<T>> _children;

            /// <summary>
            /// 实例化分层数据
            /// </summary>
            /// <param name="item">数据</param>
            /// <param name="childSelector">下层数据选择器</param>
            public Hierarchical(T item, Func<T, IEnumerable<T>> childSelector)
            {
                _locker = new object();
                _children = null;
                Current = item;
                _childSelector = childSelector;
            }

            /// <summary>
            /// 实例化分层数据
            /// </summary>
            /// <param name="item">数据</param>
            /// <param name="parent">上层数据</param>
            /// <param name="childSelector">下层数据选择器</param>
            private Hierarchical(T item, IHierarchical<T> parent, Func<T, IEnumerable<T>> childSelector)
                : this(item, childSelector)
            {
                Parent = parent;
            }

            /// <summary>
            /// 初始化下层节点集合
            /// </summary>
            /// <returns>迭代结果集合接口</returns>
            private IEnumerable<IHierarchical<T>> InitializeChildren()
            {
                var children = _childSelector(Current);
                if (children == null)
                    yield break;

                foreach (T item in children)
                {
                    yield return new Hierarchical<T>(item, this, _childSelector);
                }
            }

            #region IHierarchicalDataItem<T> 成员

            public T Current { get; }

            public IHierarchical<T> Root
            {
                get
                {
                    IHierarchical<T> node = this;

                    while (node.Parent != null)
                        node = node.Parent;

                    return node;
                }
            }

            public IHierarchical<T> Parent { get; }

            public IEnumerable<IHierarchical<T>> Ancestors
            {
                get
                {
                    IHierarchical<T> node = Parent;

                    while (node != null)
                    {
                        yield return node;
                        node = node.Parent;
                    }
                }
            }

            public IEnumerable<IHierarchical<T>> Children
            {
                get
                {
                    if (_children == null)
                        lock (_locker)
                            if (_children == null)
                                _children = InitializeChildren().ToArray();

                    return _children;
                }
            }

            public IEnumerable<IHierarchical<T>> Descendant
            {
                get
                {
                    Stack<IHierarchical<T>> stack = new Stack<IHierarchical<T>>(Children);

                    while (stack.Count > 0)
                    {
                        IHierarchical<T> node = stack.Pop();
                        yield return node;

                        foreach (IHierarchical<T> child in node.Children)
                        {
                            stack.Push(child);
                        }
                    }
                }
            }

            public IEnumerable<IHierarchical<T>> Sibling => Parent?.Children?.Where(node => node != this);

            //无缓存方法，每次访问相同节点都会重新枚举数据源并生成结果对象
            //包含相同数据T的包装IHierarchical<T>每次都不一样
            //public IEnumerable<IHierarchical<T>> Children
            //{
            //    get
            //    {
            //        var children = _childSelector(Current);
            //        if (children == null)
            //            yield break;

            //        foreach (T item in children)
            //        {
            //            yield return new Hierarchical<T>(item, this, _childSelector);
            //        }
            //    }
            //}

            public int Level => Parent?.Level + 1 ?? 0;

            public int Height => (Descendant.Any() ? Descendant.Select(node => node.Level).Max() - Level : 0) + 1;

            public int Degree => Children?.Count() ?? 0;

            public int MaxDegreeOfTree => Math.Max(Degree, Descendant.Any() ? Descendant.Select(node => node.Degree).Max() : 0);

            public bool IsRoot => Parent == null;

            public bool IsLeaf => Degree == 0;

            public bool HasChild => !IsLeaf;

            #endregion
        }

        /// <summary>
        /// 获取子孙数据（深度优先，先序）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="childSelector">下层数据选择器</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <returns>筛选的子孙</returns>
        public static IEnumerable<IHierarchical<T>> GetDescendantDfsDlr<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, IEnumerable<IHierarchical<T>>> childSelector, Func<IHierarchical<T>, bool> predicate)
        {
            predicate = predicate ?? (t => true);
            Stack<IHierarchical<T>> stack = new Stack<IHierarchical<T>>(childSelector(root).Where(predicate).Reverse());

            while (stack.Count > 0)
            {
                IHierarchical<T> node = stack.Pop();
                yield return node;

                foreach (IHierarchical<T> child in childSelector(node).Where(predicate).Reverse())
                {
                    stack.Push(child);
                }
            }

            #region 递归方式

            //foreach (T t in childSelector(root))
            //{
            //    if (predicate(t))
            //        yield return t;
            //    foreach (T child in GetDescendantDfsDlr(t, childSelector, predicate))
            //        yield return child;
            //}

            #endregion 递归方式
        }

        /// <summary>
        /// 获取子孙数据（广度优先）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="childSelector">下层数据选择器</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <returns>筛选的子孙</returns>
        public static IEnumerable<IHierarchical<T>> GetDescendantsBfs<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, IEnumerable<IHierarchical<T>>> childSelector, Func<IHierarchical<T>, bool> predicate)
        {
            predicate = predicate ?? (t => true);
            Queue<IHierarchical<T>> queue = new Queue<IHierarchical<T>>(childSelector(root).Where(predicate));

            while (queue.Count > 0)
            {
                IHierarchical<T> node = queue.Dequeue();
                yield return node;

                foreach (IHierarchical<T> child in childSelector(node).Where(predicate))
                {
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// 转换为可枚举集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="childSelector">下层数据选择器</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <param name="enumerateType">枚举方式</param>
        /// <returns>已枚举的集合</returns>
        public static IEnumerable<IHierarchical<T>> AsEnumerable<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, IEnumerable<IHierarchical<T>>> childSelector, Func<IHierarchical<T>, bool> predicate,
            EnumerateType enumerateType = EnumerateType.DfsDlr)
        {
            yield return root;

            switch (enumerateType)
            {
                case EnumerateType.DfsDlr:
                    foreach (var descendant in GetDescendantDfsDlr(root, childSelector, predicate))
                    {
                        yield return descendant;
                    }

                    break;
                case EnumerateType.Bfs:
                    foreach (var descendant in GetDescendantsBfs(root, childSelector, predicate))
                    {
                        yield return descendant;
                    }

                    break;
            }
        }
    }
}
