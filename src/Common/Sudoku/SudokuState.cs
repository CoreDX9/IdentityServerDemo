using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Sudoku
{
    public class SudokuState
    {
        public SudokuBlock[][] SudokuBoard { get; }
        public SudokuState(SudokuSolver sudoku)
        {
            SudokuBoard = new SudokuBlock[sudoku.SudokuBoard.Length][];
            //初始化数独的行
            for (int i = 0; i < sudoku.SudokuBoard.Length; i++)
            {
                SudokuBoard[i] = new SudokuBlock[sudoku.SudokuBoard[i].Length];
                //初始化每行的列
                for (int j = 0; j < sudoku.SudokuBoard[i].Length; j++)
                {
                    SudokuBoard[i][j] = new SudokuBlock(
                        sudoku.SudokuBoard[i][j].IsCondition
                        , null
                        , sudoku.SudokuBoard[i][j].Number
                        , (byte)i
                        , (byte)j);
                    if (sudoku.SudokuBoard[i][j].IsUnique)
                    {
                        SudokuBoard[i][j].SetUnique();
                    }
                }
            }
        }

        public override string ToString()
        {
            static string Str(SudokuBlock b)
            {
                var n1 = new[] { "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨" };
                var n2 = new[] { "⑴", "⑵", "⑶", "⑷", "⑸", "⑹", "⑺", "⑻", "⑼" };
                return b.Number.HasValue
                    ? b.IsCondition
                        ? " " + b.Number
                        : b.IsUnique
                            ? n1[b.Number.Value - 1]
                            : n2[b.Number.Value - 1]
                    : "▢";
            }
            return
$@"{Str(SudokuBoard[0][0])},{Str(SudokuBoard[0][1])},{Str(SudokuBoard[0][2])},{Str(SudokuBoard[0][3])},{Str(SudokuBoard[0][4])},{Str(SudokuBoard[0][5])},{Str(SudokuBoard[0][6])},{Str(SudokuBoard[0][7])},{Str(SudokuBoard[0][8])}
{Str(SudokuBoard[1][0])},{Str(SudokuBoard[1][1])},{Str(SudokuBoard[1][2])},{Str(SudokuBoard[1][3])},{Str(SudokuBoard[1][4])},{Str(SudokuBoard[1][5])},{Str(SudokuBoard[1][6])},{Str(SudokuBoard[1][7])},{Str(SudokuBoard[1][8])}
{Str(SudokuBoard[2][0])},{Str(SudokuBoard[2][1])},{Str(SudokuBoard[2][2])},{Str(SudokuBoard[2][3])},{Str(SudokuBoard[2][4])},{Str(SudokuBoard[2][5])},{Str(SudokuBoard[2][6])},{Str(SudokuBoard[2][7])},{Str(SudokuBoard[2][8])}
{Str(SudokuBoard[3][0])},{Str(SudokuBoard[3][1])},{Str(SudokuBoard[3][2])},{Str(SudokuBoard[3][3])},{Str(SudokuBoard[3][4])},{Str(SudokuBoard[3][5])},{Str(SudokuBoard[3][6])},{Str(SudokuBoard[3][7])},{Str(SudokuBoard[3][8])}
{Str(SudokuBoard[4][0])},{Str(SudokuBoard[4][1])},{Str(SudokuBoard[4][2])},{Str(SudokuBoard[4][3])},{Str(SudokuBoard[4][4])},{Str(SudokuBoard[4][5])},{Str(SudokuBoard[4][6])},{Str(SudokuBoard[4][7])},{Str(SudokuBoard[4][8])}
{Str(SudokuBoard[5][0])},{Str(SudokuBoard[5][1])},{Str(SudokuBoard[5][2])},{Str(SudokuBoard[5][3])},{Str(SudokuBoard[5][4])},{Str(SudokuBoard[5][5])},{Str(SudokuBoard[5][6])},{Str(SudokuBoard[5][7])},{Str(SudokuBoard[5][8])}
{Str(SudokuBoard[6][0])},{Str(SudokuBoard[6][1])},{Str(SudokuBoard[6][2])},{Str(SudokuBoard[6][3])},{Str(SudokuBoard[6][4])},{Str(SudokuBoard[6][5])},{Str(SudokuBoard[6][6])},{Str(SudokuBoard[6][7])},{Str(SudokuBoard[6][8])}
{Str(SudokuBoard[7][0])},{Str(SudokuBoard[7][1])},{Str(SudokuBoard[7][2])},{Str(SudokuBoard[7][3])},{Str(SudokuBoard[7][4])},{Str(SudokuBoard[7][5])},{Str(SudokuBoard[7][6])},{Str(SudokuBoard[7][7])},{Str(SudokuBoard[7][8])}
{Str(SudokuBoard[8][0])},{Str(SudokuBoard[8][1])},{Str(SudokuBoard[8][2])},{Str(SudokuBoard[8][3])},{Str(SudokuBoard[8][4])},{Str(SudokuBoard[8][5])},{Str(SudokuBoard[8][6])},{Str(SudokuBoard[8][7])},{Str(SudokuBoard[8][8])}";
        }
    }
}
