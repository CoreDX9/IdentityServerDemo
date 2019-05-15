using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Hierarchical;

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

            #region IHierarchical<T> 成员

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

            public IEnumerable<IHierarchical<T>> Siblings => Parent?.Children?.Where(node => node != this) ?? Array.Empty<IHierarchical<T>>();

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

            public IHierarchical<T> ReBuild()
            {
                return new Hierarchical<T>(Current, _childSelector);
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
            if (node.Root != from.Root)
                throw new InvalidOperationException($"{nameof(node)} and {nameof(from)} are not at same tree.");

            yield return from; //起点必然是需要的

            if (node == from) yield break; //如果终点就是起点，那么可以直接结束路径查找了

            //只有终点和起点不存在双亲孩子关系时才需要执行内部代码，否则直接返回终点即可
            if (!(node.Parent == from || node.Children.Any(n => n == from)))
            {
                var nearestCommonAncestor = node.GetNearestCommonAncestor(from);

                //如果起点不是终点的祖先，需要先返回从起点的双亲到最近公共祖先
                //（不包括最近公共祖先。最近公共祖先可能就是终点，返回的话会导致终点被返回两次。终点会在方法末尾统一返回）的路径
                if (!from.IsAncestorOf(node))
                    foreach (var ancestor in from.Ancestors)
                        if (ancestor != nearestCommonAncestor)
                            yield return ancestor;
                        else
                            break;

                //如果最近公共祖先不是终点，返回从最近公共祖先到终点（不包括终点，原因不再赘述）的路径
                if (nearestCommonAncestor != node)
                {
                    var ancestorsOfNode = node.Ancestors.ToArray();
                    for (int i = Array.IndexOf(ancestorsOfNode, nearestCommonAncestor); i >= 0; i--)
                        yield return ancestorsOfNode[i];
                }
            }

            yield return node; //最后返回终点
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

        /// <summary>
        /// 设置节点的双亲和子节点引用
        /// </summary>
        /// <typeparam name="T">节点类型</typeparam>
        /// <param name="root">根节点</param>
        /// <param name="childrenSelector">子节点选择器</param>
        /// <param name="clearChildrenBeforeSet">是否在设置子节点前清空子节点集合</param>
        /// <param name="parentSetter">双亲节点设置器，第一个参数为待设置引用的节点，第二个参数为第一个参数的双亲节点</param>
        /// <returns>原始根节点</returns>
        public static T SetParentChildren<T>(this IHierarchical<T> root,
            Func<T, ICollection<T>> childrenSelector, bool clearChildrenBeforeSet = false, Action<T, T> parentSetter = null)
        {
            if (childrenSelector == null) throw new ArgumentNullException(nameof(childrenSelector));

            foreach (var node in root.AsEnumerable())
            {
                parentSetter?.Invoke(node.Current, node.Parent != null ? node.Parent.Current : default(T));

                var children = childrenSelector.Invoke(node.Current);
                if (children == null) throw new InvalidOperationException("子节点集合为 null");

                if(clearChildrenBeforeSet) children.Clear();

                foreach (var child in node.Children)
                {
                    children.Add(child.Current);
                }
            }

            return root.Current;
        }

        /// <summary>
        /// 设置节点的双亲和子节点引用
        /// </summary>
        /// <typeparam name="T">节点类型</typeparam>
        /// <param name="roots">根节点集合</param>
        /// <param name="childrenSelector">子节点选择器</param>
        /// <param name="clearChildrenBeforeSet">是否在设置子节点前清空子节点集合</param>
        /// <param name="parentSetter">双亲节点设置器，第一个参数为待设置引用的节点，第二个参数为第一个参数的双亲节点</param>
        /// <returns>原始根节点集合</returns>
        public static IEnumerable<T> SetParentChildren<T>(this IEnumerable<IHierarchical<T>> roots,
            Func<T, ICollection<T>> childrenSelector, bool clearChildrenBeforeSet = false, Action<T, T> parentSetter = null)
        {
            foreach (var root in roots)
            {
                root.SetParentChildren(childrenSelector, false, parentSetter);
            }

            return roots.Select(r => r.Current);
        }
    }
}
