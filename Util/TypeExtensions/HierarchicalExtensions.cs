using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// 深度优先，后序
        /// </summary>
        DfsLrd,

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
        /// 以当前节点为根返回树形排版的结构字符串
        /// </summary>
        /// <param name="formatter">数据对象格式化器（内容要为一行，否则会影响排版）</param>
        /// <param name="convertToSingleLine">处理掉换行符变成单行文本</param>
        /// <returns></returns>
        string ToString(Func<T, string> formatter, bool convertToSingleLine = false);
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

            public IEnumerable<IHierarchical<T>> Descendants
            {
                get
                {
                    Stack<IHierarchical<T>> stack = new Stack<IHierarchical<T>>(Children.Reverse());

                    while (stack.Count > 0)
                    {
                        IHierarchical<T> node = stack.Pop();
                        yield return node;

                        foreach (IHierarchical<T> child in node.Children.Reverse())
                        {
                            stack.Push(child);
                        }
                    }
                }
            }

            public IEnumerable<IHierarchical<T>> Siblings => Parent?.Children?.Where(node => node != this);

            public int IndexOfSiblings
            {
                get
                {
                    if (Parent == null) return 0;

                    int index = 0;
                    foreach (var child in Parent.Children)
                    {
                        if (child == this) break;
                        index++;
                    }

                    return index;
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

            public int Level => Parent?.Level + 1 ?? 1;

            public int Height => (Descendants.Any() ? Descendants.Select(node => node.Level).Max() - Level : 0) + 1;

            public int Degree => Children?.Count() ?? 0;

            public int MaxDegreeOfTree => Math.Max(Degree, Descendants.Any() ? Descendants.Select(node => node.Degree).Max() : 0);

            public bool IsRoot => Parent == null;

            public bool IsLeaf => Degree == 0;

            public bool HasChild => !IsLeaf;

            public string ToString(Func<T, string> formatter, bool convertToSingleLine = false)
            {
                var sbr = new StringBuilder();
                sbr.AppendLine(convertToSingleLine
                    ? formatter(Current).Replace("\r", @"\r").Replace("\n", @"\n")
                    : formatter(Current));

                var sb = new StringBuilder();
                foreach (var node in Descendants)
                {
                    sb.Append(convertToSingleLine
                        ? formatter(node.Current).Replace("\r", @"\r").Replace("\n", @"\n")
                        : formatter(node.Current));
                    sb.Insert(0, node == node.Parent.Children.Last() ? "└─" : "├─");

                    for (int i = 0; i < node.Ancestors.Count() - (Ancestors?.Count() ?? 0) - 1; i++)
                    {
                        sb.Insert(0,
                            node.Ancestors.ElementAt(i) == node.Ancestors.ElementAt(i + 1).Children.Last()
                                ? "    "
                                : "│  ");
                    }

                    sbr.AppendLine(sb.ToString());
                    sb.Clear();
                }

                return sbr.ToString();
            }

            public override string ToString()
            {
                return ToString(node => node.ToString());
            }

            #endregion
        }

        /// <summary>
        /// 获取从根到节点的路径（中顺序经过的节点集合）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="node">节点</param>
        /// <returns>路径中按经过顺序组成的节点集合</returns>
        public static IEnumerable<IHierarchical<T>> GetPathFromRoot<T>(this IHierarchical<T> node) =>
            node.Ancestors.Reverse();

        /// <summary>
        /// 判断节点是否是指定节点的祖先节点
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="node">待判断的节点</param>
        /// <param name="target">目标节点</param>
        /// <returns>判断结果</returns>
        public static bool IsAncestorOf<T>(this IHierarchical<T> node, IHierarchical<T> target)
        {
            if (node.Root != target.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(target)} are not at same tree.");

            return target.Ancestors.SingleOrDefault(n => n == node) != null;
        }

        /// <summary>
        /// 判断节点是否是指定节点的后代节点
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="node">待判断的节点</param>
        /// <param name="target">目标节点</param>
        /// <returns>判断结果</returns>
        public static bool IsDescendantOf<T>(this IHierarchical<T> node, IHierarchical<T> target)
        {
            if (node.Root != target.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(target)} are not at same tree.");

            return target.IsAncestorOf(node);
        }

        /// <summary>
        /// 获取节点与指定节点的最近公共祖先节点
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="node">待查找的节点</param>
        /// <param name="target">目标节点</param>
        /// <returns>最近的公共祖先节点</returns>
        public static IHierarchical<T> GetNearestCommonAncestor<T>(this IHierarchical<T> node, IHierarchical<T> target)
        {
            if (node.Root != target.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(target)} are not at same tree.");

            if (node.IsAncestorOf(target)) return node;
            if (target.IsAncestorOf(node)) return target;

            return node.Ancestors.Intersect(target.Ancestors).OrderByDescending(no => no.Level).First();
        }

        /// <summary>
        /// 获取从指定节点到当前节点的路径
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="node">当前节点（终点）</param>
        /// <param name="from">目标节点（起点）</param>
        /// <returns>按从目标节点到当前节点顺序经过的节点集合</returns>
        public static IEnumerable<IHierarchical<T>> GetPathFromNode<T>(this IHierarchical<T> node,
            IHierarchical<T> from)
        {
            if(node.Root != from.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(from)} are not at same tree.");

            yield return from;

            if (node == from) yield break;

            if (node.IsAncestorOf(from))
            {
                foreach (var ancestor in from.Ancestors)
                {
                    yield return ancestor;
                    if (ancestor == node)
                    {
                        yield break;
                    }
                }
            }

            var ancestorsOfNode = node.Ancestors.ToArray();
            if (node.IsDescendantOf(from))
            {
                for (int i = Array.IndexOf(ancestorsOfNode, from) - 1; i >= 0; i--)
                {
                    yield return ancestorsOfNode[i];
                }

                yield return node;
                yield break;
            }

            var keyNode = ancestorsOfNode.Intersect(from.Ancestors).OrderByDescending(no => no.Level).First();
            foreach (var ancestor in from.Ancestors)
            {
                yield return ancestor;
                if (ancestor == keyNode)
                {
                    break;
                }
            }

            for (int i = Array.IndexOf(ancestorsOfNode, keyNode) - 1; i >= 0; i--)
            {
                yield return ancestorsOfNode[i];
            }

            yield return node;
        }

        /// <summary>
        /// 获取从当前节点到指定节点的路径
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="node">当前节点（起点）</param>
        /// <param name="to">目标节点（终点）</param>
        /// <returns>按从当前节点到目标节点顺序经过的节点集合</returns>
        public static IEnumerable<IHierarchical<T>> GetPathToNode<T>(this IHierarchical<T> node, IHierarchical<T> to)
        {
            if (node.Root != to.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(to)} are not at same tree.");

            return to.GetPathFromNode(node);
        }

        /// <summary>
        /// 获取子孙数据（深度优先，先序）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <returns>筛选的子孙</returns>
        public static IEnumerable<IHierarchical<T>> GetDescendantsDfsDlr<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, bool> predicate = null)
        {
            var children = predicate == null ? root.Children : root.Children.Where(predicate);
            Stack<IHierarchical<T>> stack = new Stack<IHierarchical<T>>(children.Reverse());

            while (stack.Count > 0)
            {
                IHierarchical<T> node = stack.Pop();
                yield return node;

                children = predicate == null ? node.Children : node.Children.Where(predicate);
                foreach (IHierarchical<T> child in children.Reverse())
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
        /// 获取子孙数据（深度优先，后序）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <returns>筛选的子孙</returns>
        public static IEnumerable<IHierarchical<T>> GetDescendantsDfsLrd<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, bool> predicate = null)
        {
            var children = predicate == null ? root.Children : root.Children.Where(predicate);
            Stack<IHierarchical<T>> stack = new Stack<IHierarchical<T>>(children.Reverse());

            IHierarchical<T> lastAccessedNode = null;

            while (stack.Count > 0)
            {
                var node = stack.Peek();

                if (node.Children.Any() && node.Children.Last() != lastAccessedNode)
                {
                    children = predicate == null ? node.Children : node.Children.Where(predicate);
                    foreach (IHierarchical<T> child in children.Reverse())
                    {
                        stack.Push(child);
                    }
                }
                else
                {
                    yield return node;

                    lastAccessedNode = node;
                    stack.Pop();
                }
            }
        }

        /// <summary>
        /// 获取子孙数据（广度优先）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="root">根</param>
        /// <param name="predicate">子孙筛选条件</param>
        /// <returns>筛选的子孙</returns>
        public static IEnumerable<IHierarchical<T>> GetDescendantsBfs<T>(this IHierarchical<T> root,
            Func<IHierarchical<T>, bool> predicate = null)
        {
            predicate = predicate ?? (t => true);
            Queue<IHierarchical<T>> queue = new Queue<IHierarchical<T>>(root.Children.Where(predicate));

            while (queue.Count > 0)
            {
                IHierarchical<T> node = queue.Dequeue();
                yield return node;

                foreach (IHierarchical<T> child in node.Children.Where(predicate))
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
        /// <param name="predicate">子孙筛选条件</param>
        /// <param name="enumerateType">枚举方式</param>
        /// <returns>已枚举的集合</returns>
        public static IEnumerable<IHierarchical<T>> AsEnumerable<T>(this IHierarchical<T> root,
            EnumerateType enumerateType = EnumerateType.DfsDlr,
            Func<IHierarchical<T>, bool> predicate = null)
        {
            switch (enumerateType)
            {
                case EnumerateType.DfsDlr:
                    yield return root;

                    foreach (var descendant in GetDescendantsDfsDlr(root, predicate))
                    {
                        yield return descendant;
                    }

                    break;
                case EnumerateType.DfsLrd:
                    foreach (var descendant in GetDescendantsDfsLrd(root, predicate))
                    {
                        yield return descendant;
                    }

                    yield return root;

                    break;
                case EnumerateType.Bfs:
                    yield return root;

                    foreach (var descendant in GetDescendantsBfs(root, predicate))
                    {
                        yield return descendant;
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enumerateType), enumerateType, null);
            }
        }
    }
}
