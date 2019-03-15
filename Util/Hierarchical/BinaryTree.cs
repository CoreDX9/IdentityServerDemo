using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Hierarchical
{
    public class BinaryTree<T> : IHierarchical<T>
    {
        private BinaryTree<T> _left, _right, _parent;

        public BinaryTree<T> Left => _left;

        public BinaryTree<T> Right => _right;

        public BinaryTree(T data)
        {
            Current = data;
            _parent = null;
        }

        private BinaryTree(T data, BinaryTree<T> parent)
            :this(data)
        {
            _parent = parent;
        }

        public void SetLeft(T data)
        {
            _left = new BinaryTree<T>(data, this);
        }

        public void SetRight(T data)
        {
            _right = new BinaryTree<T>(data, this);
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

        public IHierarchical<T> Parent => _parent;
        public BinaryTree<T> ParentB => _parent;

        public IEnumerable<IHierarchical<T>> Ancestors => throw new NotImplementedException();

        public IEnumerable<IHierarchical<T>> Children => throw new NotImplementedException();

        public IEnumerable<IHierarchical<T>> Descendants => throw new NotImplementedException();

        public IEnumerable<IHierarchical<T>> Siblings => throw new NotImplementedException();

        public int IndexOfSiblings => throw new NotImplementedException();

        public int Level => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public int Degree => throw new NotImplementedException();

        public int MaxDegreeOfTree => throw new NotImplementedException();

        public bool IsRoot => Parent == null;

        public bool IsLeaf => Degree == 0;

        public bool HasChild => throw new NotImplementedException();

        public string ToString(Func<T, string> formatter, bool convertToSingleLine = false)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
