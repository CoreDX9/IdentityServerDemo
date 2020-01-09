using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace CoreDX.Sudoku
{
    public class SudokuSolver
    {
        /// <summary>
        /// 题目面板
        /// </summary>
        public SudokuBlock[][] SudokuBoard { get; }

        public SudokuSolver(byte[][] board)
        {
            SudokuBoard = new SudokuBlock[board.Length][];
            //初始化数独的行
            for (int i = 0; i < board.Length; i++)
            {
                SudokuBoard[i] = new SudokuBlock[board[i].Length];
                //初始化每行的列
                for (int j = 0; j < board[i].Length; j++)
                {
                    SudokuBoard[i][j] = new SudokuBlock(
                        board[i][j] > 0
                        , board[i][j] <= 0 ? new BitArray(board.Length) : null
                        , board[i][j] > 0 ? (byte?)board[i][j] : null
                        , (byte)i
                        , (byte)j);
                }
            }
        }

        /// <summary>
        /// 求解数独
        /// </summary>
        /// <returns>获得的解</returns>
        public IEnumerable<(SudokuState sudoku, PathTree path)> Solve(bool multiAnswer = false)
        {
            //初始化各个单元格能填入的数字
            InitCandidate();

            var pathRoot0 = new PathTree(null, -1, -1, -1); //填写路径树，在非递归方法中用于记录回退路径和其他有用信息，初始生成一个根
            var path0 = pathRoot0;

            //循环填入能填入的数字只有一个的单元格，每次填入都可能产生新的唯一数单元格，直到没有唯一数单元格可填
            while (true)
            {
                if (!FillUniqueNumber(ref path0))
                {
                    break;
                }
            }

            //检查是否在填唯一数单元格时就已经把所有单元格填满了
            var finish = true;
            foreach (var row in SudokuBoard)
            {
                foreach (var cell in row)
                {
                    if (!cell.IsCondition && !cell.IsUnique)
                    {
                        finish = false;
                        break;
                    }
                }
                if (!finish)
                {
                    break;
                }
            }
            if (finish)
            {
                yield return (new SudokuState(this), path0);
                yield break;
            }

            var pathRoot = new PathTree(null, -1, -1, -1); //填写路径树，在非递归方法中用于记录回退路径和其他有用信息，初始生成一个根
            var path = pathRoot;
            var toRe = new List<(SudokuState sudoku, PathTree path)>();
            //还存在需要试数才能求解的单元格，开始暴力搜索
            int i = 0, j = 0;
            while (true)
            {
                (i, j) = NextBlock(i, j);

                //正常情况下返回-1表示已经全部填完
                if (i == -1 && j == -1 && !multiAnswer)
                {
                    var pathLast = path;//记住最后一步
                    var path1 = path;
                    while(path1.Parent.X != -1 && path1.Parent.Y != -1)
                    {
                        path1 = path1.Parent;
                    }

                    //将暴力搜索的第一步追加到唯一数单元格的填写步骤的最后一步之后，连接成完整的填数步骤
                    path0.Children.Add(path1);
                    path1.Parent = path0;
                    yield return (new SudokuState(this), pathLast);
                    break;
                }

                var numNode = path.Children.LastOrDefault();
                //确定要从哪个数开始进行填入尝试
                var num = numNode == null
                    ? 0
                    : numNode.Number;

                bool filled = false; //是否发现可以填入的数
                //循环查看从num开始接下来的候选数是否能填（num是最后一次填入的数，传到Candidate[]的索引器中刚好指向 num + 1是否能填的存储位，对于标准数独，候选数为 1~9，Candidate的索引范围就是 0~8）
                for (; !SudokuBoard[i][j].IsCondition && !SudokuBoard[i][j].IsUnique && num < SudokuBoard[i][j].Candidate.Length; num++)
                {
                    //如果有可以填的候选数，理论上不会遇见没有可以填的情况，这种死路情况已经在UpdateCandidate时检查了
                    if (SudokuBoard[i][j].Candidate[num] && !path.Children.Any(x => x.Number - 1 == num && !x.Pass))
                    {
                        filled = true; //进来了说明单元格有数可以填
                        //记录步骤
                        var node = new PathTree(SudokuBoard[i][j], i, j, num + 1, path);
                        path = node;
                        //如果更新相关单元格的候选数时发现死路（更新函数会在发现死路时自动撤销更新）
                        (bool canFill, (byte x, byte y)[] setList) updateResult = UpdateCandidate(i, j, (byte)(num + 1));
                        if (!updateResult.canFill)
                        {
                            //记录这条路是死路
                            path.SetPass(false);
                        }
                        //仅在确认是活路时设置填入数字
                        if (path.Pass)
                        {
                            SudokuBoard[i][j].SetNumber((byte)(num + 1));
                            path.SetList = updateResult.setList;//记录相关单元格可填数更新记录，方便在回退时撤销更新
                        }
                        else //出现死路，要进行回退，重试这个单元格的其他可填数字
                        {
                            path.Block.SetNumber(null);
                            path = path.Parent;
                        }
                        //填入一个候选数后跳出循环，不再继续尝试填入之后的候选数
                        break;
                    }
                }
                if (!filled)//如果没有成功填入数字，说明上一步填入的单元格就是错的，会导致后面的单元格怎么填都不对，要回退到上一个单元格重新填
                {
                    path.SetPass(false);
                    path.Block.SetNumber(null);
                    foreach (var pos in path.SetList)
                    {
                        SudokuBoard[pos.x][pos.y].Candidate.Set(path.Number - 1, true);
                    }
                    path = path.Parent;
                    i = path.X < 0 ? 0 : path.X;
                    j = path.Y < 0 ? 0 : path.Y;
                }
            }
        }

        /// <summary>
        /// 初始化候选项
        /// </summary>
        private void InitCandidate()
        {
            //初始化每行空缺待填的数字
            var rb = new List<BitArray>();
            for (int i = 0; i < SudokuBoard.Length; i++)
            {
                var r = new BitArray(SudokuBoard.Length);
                r.SetAll(true);
                for (int j = 0; j < SudokuBoard[i].Length; j++)
                {
                    //如果i行j列是条件（题目）给出的数，设置数字不能再填（r[x] == false 表示 i 行不能再填 x + 1，下标加1表示数独可用的数字，下标对应的值表示下标加1所表示的数是否还能填入该行）
                    if (SudokuBoard[i][j].IsCondition || SudokuBoard[i][j].IsUnique)
                    {
                        r.Set(SudokuBoard[i][j].Number.Value - 1, false);
                    }
                }
                rb.Add(r);
            }

            //初始化每列空缺待填的数字
            var cb = new List<BitArray>();
            for (int j = 0; j < SudokuBoard[0].Length; j++)
            {
                var c = new BitArray(SudokuBoard[0].Length);
                c.SetAll(true);
                for (int i = 0; i < SudokuBoard.Length; i++)
                {
                    if (SudokuBoard[i][j].IsCondition || SudokuBoard[i][j].IsUnique)
                    {
                        c.Set(SudokuBoard[i][j].Number.Value - 1, false);
                    }
                }
                cb.Add(c);
            }

            //初始化每宫空缺待填的数字（目前只能算标准 n×n 数独的宫）
            var gb = new List<BitArray>();
            //n表示每宫应有的行、列数（标准数独行列、数相同）
            var n = (int)Sqrt(SudokuBoard.Length);
            for (int g = 0; g < SudokuBoard.Length; g++)
            {
                var gba = new BitArray(SudokuBoard.Length);
                gba.SetAll(true);
                for (int i = g / n * n; i < g / n * n + n; i++)
                {
                    for (int j = g % n * n; j < g % n * n + n; j++)
                    {
                        if (SudokuBoard[i][j].IsCondition || SudokuBoard[i][j].IsUnique)
                        {
                            gba.Set(SudokuBoard[i][j].Number.Value - 1, false);
                        }
                    }
                }
                gb.Add(gba);
            }

            //初始化每格可填的候选数字
            for (int i = 0; i < SudokuBoard.Length; i++)
            {
                for (int j = 0; j < SudokuBoard[i].Length; j++)
                {

                    if (!SudokuBoard[i][j].IsCondition)
                    {
                        var c = SudokuBoard[i][j].Candidate;
                        c.SetAll(true);
                        //当前格能填的数为其所在行、列、宫同时空缺待填的数字，按位与运算后只有同时能填的候选数保持1（可填如当前格），否则变成0
                        // i / n * n + j / n：根据行号列号计算宫号，
                        c = c.And(rb[i]).And(cb[j]).And(gb[i / n * n + j / n]);
                        SudokuBoard[i][j].SetCandidate(c);
                    }
                }
            }
        }

        /// <summary>
        /// 求解开始时寻找并填入单元格唯一可填的数，减少解空间
        /// </summary>
        /// <returns>是否填入过数字，如果为false，表示能立即确定待填数字的单元格已经没有，要开始暴力搜索了</returns>
        private bool FillUniqueNumber(ref PathTree path)
        {
            var filled = false;
            for (int i = 0; i < SudokuBoard.Length; i++)
            {
                for (int j = 0; j < SudokuBoard[i].Length; j++)
                {
                    if (!SudokuBoard[i][j].IsCondition && !SudokuBoard[i][j].IsUnique)
                    {
                        var canFillCount = 0;
                        var index = -1;
                        for (int k = 0; k < SudokuBoard[i][j].Candidate.Length; k++)
                        {
                            if (SudokuBoard[i][j].Candidate[k])
                            {
                                index = k;
                                canFillCount++;
                            }
                            if (canFillCount > 1)
                            {
                                break;
                            }
                        }
                        if (canFillCount == 0)
                        {
                            throw new Exception("有单元格无法填入任何数字，数独无解");
                        }
                        if (canFillCount == 1)
                        {
                            var num = (byte)(index + 1);
                            SudokuBoard[i][j].SetNumber(num);
                            SudokuBoard[i][j].SetUnique();
                            filled = true;
                            var upRes = UpdateCandidate(i, j, num);
                            if (!upRes.canFill)
                            {
                                throw new Exception("有单元格无法填入任何数字，数独无解");
                            }
                            path = new PathTree(SudokuBoard[i][j], i, j, num, path);
                            path.SetList = upRes.setList;
                        }
                    }
                }
            }
            return filled;
        }

        /// <summary>
        /// 更新单元格所在行、列、宫的其它单元格能填的数字候选，如果没有，会撤销更新
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="column">列号</param>
        /// <param name="canNotFillNumber">要剔除的候选数字</param>
        /// <returns>更新候选数后，所有被更新的单元格是否都有可填的候选数字</returns>
        private (bool canFill, (byte x, byte y)[] setList) UpdateCandidate(int row, int column, byte canNotFillNumber)
        {
            var canFill = true;
            var list = new List<SudokuBlock>(); // 记录修改过的单元格，方便撤回修改

            bool CanFillNumber(int i, int j)
            {
                var re = true;
                var _canFill = false;
                for (int k = 0; k < SudokuBoard[i][j].Candidate.Length; k++)
                {
                    if (SudokuBoard[i][j].Candidate[k])
                    {
                        _canFill = true;
                        break;
                    }
                }
                if (!_canFill)
                {
                    re = false;
                }

                return re;
            }
            bool Update(int i, int j)
            {
                if (!(i == row && j == column) && !SudokuBoard[i][j].IsCondition && !SudokuBoard[i][j].IsUnique && SudokuBoard[i][j].Candidate[canNotFillNumber - 1])
                {
                    SudokuBoard[i][j].Candidate.Set(canNotFillNumber - 1, false);
                    list.Add(SudokuBoard[i][j]);

                    return CanFillNumber(i, j);
                }
                else
                {
                    return true;
                }
            }

            //更新该行其余列
            for (int j = 0; j < SudokuBoard[row].Length; j++)
            {
                canFill = Update(row, j);
                if (!canFill)
                {
                    break;
                }
            }

            if (canFill) //只在行更新时没发现无数可填的单元格时进行列更新才有意义
            {
                //更新该列其余行
                for (int i = 0; i < SudokuBoard.Length; i++)
                {
                    canFill = Update(i, column);
                    if (!canFill)
                    {
                        break;
                    }
                }
            }

            if (canFill)//只在行、列更新时都没发现无数可填的单元格时进行宫更新才有意义
            {
                //更新该宫其余格
                //n表示每宫应有的行、列数（标准数独行列、数相同）
                var n = (int)Sqrt(SudokuBoard.Length);
                //g为宫的编号，根据行号列号计算
                var g = row / n * n + column / n;
                for (int i = g / n * n; i < g / n * n + n; i++)
                {
                    for (int j = g % n * n; j < g % n * n + n; j++)
                    {
                        canFill = Update(i, j);
                        if (!canFill)
                        {
                            goto canNotFill;
                        }
                    }
                }
                canNotFill:;
            }

            //如果发现存在没有任何数字可填的单元格，撤回所有候选修改
            if (!canFill)
            {
                foreach (var cell in list)
                {
                    cell.Candidate.Set(canNotFillNumber - 1, true);
                }
            }

            return (canFill, list.Select(x => (x.X, x.Y)).ToArray());
        }

        /// <summary>
        /// 寻找下一个要尝试填数的格
        /// </summary>
        /// <param name="i">起始行号</param>
        /// <param name="j">起始列号</param>
        /// <returns>找到的下一个行列号，没有找到返回-1</returns>
        private (int x, int y) NextBlock(int i = 0, int j = 0)
        {
            for (; i < SudokuBoard.Length; i++)
            {
                for (; j < SudokuBoard[i].Length; j++)
                {
                    if (!SudokuBoard[i][j].IsCondition && !SudokuBoard[i][j].IsUnique && !SudokuBoard[i][j].Number.HasValue)
                    {
                        return (i, j);
                    }
                }
                j = 0;
            }

            return (-1, -1);
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
