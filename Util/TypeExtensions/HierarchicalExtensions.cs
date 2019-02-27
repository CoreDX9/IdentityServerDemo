using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        T Current { get; }

        IHierarchical<T> Parent { get; }

        IEnumerable<IHierarchical<T>> Children { get; }

        int Depth { get; }

        bool IsRoot { get; }

        bool IsLeaf { get; }

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
        /// 分层数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        private class Hierarchical<T> : IHierarchical<T>
        {
            private object _locker;
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

            public IHierarchical<T> Parent { get; }

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

            public int Depth => Parent?.Depth + 1 ?? 0;

            public bool IsRoot => Parent == null;

            public bool IsLeaf => !(Children?.Any() ?? false);

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
