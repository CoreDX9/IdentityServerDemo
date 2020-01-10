using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Sudoku
{
    public class PathTree
    {
        public PathTree Parent { get; set; }
        public List<PathTree> Children { get; } = new List<PathTree>();

        public SudokuBlock Block { get; }
        public int X { get; }
        public int Y { get; }
        public int Number { get; }
        public bool Pass { get; private set; } = true;
        public (byte x, byte y)[] SetList { get; set; }

        public PathTree(SudokuBlock block, int x, int y, int number)
        {
            Block = block;
            X = x;
            Y = y;
            Number = number;

        }

        public PathTree(SudokuBlock block, int row, int column, int number, PathTree parent)
            : this(block, row, column, number)
        {
            Parent = parent;
            Parent.Children.Add(this);
        }

        public void SetPass(bool pass)
        {
            Pass = pass;
        }
    }
}
