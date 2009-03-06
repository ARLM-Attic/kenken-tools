using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    /// <summary>
    /// Represents a single KenKen solution
    /// </summary>
    public class Board
    {
        private int[][] board;
        private int size;
        internal Board(int[] state, int size)
        {
            int[][] board = new int[size][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new int[size];
            for (int i = 0; i < state.Length; i++)
            {
                board[i / size][i % size] = state[i] + 1;
            }
            this.board = board;
            this.size = size;
        }

        /// <summary>
        /// Converts the board into a matrix of integers
        /// </summary>
        /// <returns>An integer matrix representing the solution</returns>
        public int[][] ToMatrix()
        {
            return this.board;
        }

        /// <summary>
        /// Pretty prints the board to a string
        /// </summary>
        /// <returns>A string representing the solution</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(size * size * 3);
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                    sb.Append(board[i][j]).Append(" ");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

    }
}
