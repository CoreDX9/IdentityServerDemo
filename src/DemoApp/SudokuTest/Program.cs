using CoreDX.Sudoku;
using System;
using System.Linq;

namespace SudokuTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //模板
            //byte[][] game = new byte[][] {
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
            //    new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0},};
            //这个简单，无需尝试，一直填唯一数单元格，填完剩下的单元格又有会变唯一数单元格
            //byte[][] game = new byte[][] {
            //    new byte[]{0, 5, 0, 7, 0, 6, 0, 1, 0},
            //    new byte[]{0, 8, 0, 0, 9, 0, 0, 6, 0},
            //    new byte[]{0, 6, 9, 0, 8, 0, 7, 3, 0},
            //    new byte[]{0, 1, 0, 0, 4, 0, 0, 0, 6},
            //    new byte[]{6, 0, 7, 1, 0, 3, 8, 0, 5},
            //    new byte[]{9, 0, 0, 0, 0, 8, 0, 2, 0},
            //    new byte[]{0, 2, 4, 0, 1, 0, 6, 5, 0},
            //    new byte[]{0, 7, 0, 0, 6, 0, 0, 4, 0},
            //    new byte[]{0, 9, 0, 4, 0, 2, 0, 8, 0},};
            //可以填一部分唯一数单元格，剩下一部分需要尝试，调试用
            //byte[][] game = new byte[][] {
            //    new byte[]{7, 0, 0, 5, 0, 0, 0, 0, 2},
            //    new byte[]{0, 3, 0, 0, 0, 4, 6, 0, 0},
            //    new byte[]{0, 0, 2, 6, 0, 0, 0, 0, 0},
            //    new byte[]{2, 0, 0, 0, 7, 0, 0, 0, 5},
            //    new byte[]{5, 0, 0, 1, 0, 3, 0, 0, 6},
            //    new byte[]{3, 0, 0, 4, 0, 0, 0, 0, 9},
            //    new byte[]{0, 0, 0, 0, 0, 1, 5, 0, 0},
            //    new byte[]{0, 0, 7, 2, 0, 0, 0, 4, 0},
            //    new byte[]{4, 0, 0, 0, 0, 9, 0, 0, 7},};
            //全部要靠尝试来填
            byte[][] game = new byte[][] {
                new byte[]{1, 0, 0, 2, 0, 0, 3, 0, 0},
                new byte[]{0, 4, 0, 5, 0, 0, 0, 6, 0},
                new byte[]{0, 0, 0, 7, 0, 0, 8, 0, 0},
                new byte[]{3, 0, 0, 0, 0, 7, 0, 0, 0},
                new byte[]{0, 9, 0, 0, 0, 0, 0, 5, 0},
                new byte[]{0, 0, 0, 6, 0, 0, 0, 0, 7},
                new byte[]{0, 0, 2, 0, 0, 4, 0, 0, 0},
                new byte[]{0, 5, 0, 0, 0, 6, 0, 9, 0},
                new byte[]{0, 0, 8, 0, 0, 1, 0, 0, 3},};
            var su = new SudokuSolver(game);
            var r = su.Solve();

            Console.WriteLine(r.First());
            Console.ReadKey();
        }
    }
}
