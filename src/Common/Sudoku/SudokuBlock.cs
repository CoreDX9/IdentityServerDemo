using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Sudoku
{
    public class SudokuBlock
    {
        /// <summary>
        /// 填入的数字
        /// </summary>
        public byte? Number { get; private set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public byte X { get; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public byte Y { get; }

        /// <summary>
        /// 候选数字，下标所示状态表示数字“下标加1”是否能填入
        /// </summary>
        public BitArray Candidate { get; private set; }

        /// <summary>
        /// 是否为条件（题目）给出数字的单元格
        /// </summary>
        public bool IsCondition { get; }

        /// <summary>
        /// 是否为游戏开始就能确定唯一可填数字的单元格
        /// </summary>
        public bool IsUnique { get; private set; }

        public SudokuBlock(bool isCondition, BitArray candidate, byte? number, byte x, byte y)
        {
            IsCondition = isCondition;
            Candidate = candidate;
            Number = number;
            IsUnique = false;
            X = x;
            Y = y;
        }

        public void SetNumber(byte? number)
        {
            Number = number;
        }

        public void SetCandidate(BitArray candidate)
        {
            Candidate = candidate;
        }

        public void SetUnique()
        {
            IsUnique = true;
        }
    }
}
