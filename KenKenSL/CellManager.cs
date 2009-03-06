using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace KenKenSL
{
    internal class CellManager
    {
        private Cell[][] cells;
        private int size;
        public CellManager(int size)
        {
            this.size = size;
            this.cells = new Cell[size][];
            for (int i = 0; i < cells.Length; i++) //row
            {
                this.cells[i] = new Cell[size];
                for (int j = 0; j < cells[i].Length; j++) //col
                {
                    cells[i][j] = new Cell(i, j);
                }
            }
            
        }

        public Cell GetCell(int row, int col)
        {
            return cells[row][col];
        }

        public static bool AreAdjacent(Cell cell1, Cell cell2)
        {
            if (cell1 == null || cell2 == null)
                return false;

            if (cell1.Row == cell2.Row)
            {
                if (-1 <= cell1.Column - cell2.Column && cell1.Column - cell2.Column <= 1)
                    return true;
            }
            if (cell1.Column == cell2.Column)
            {
                if (-1 <= cell1.Row - cell2.Row && cell1.Row - cell2.Row <= 1)
                    return true;
            }
            return false;
        }

        public int AdjacencyCount(Cell cell)
        {
            int adjacencyCount = 0;
            foreach (Cell neighbor in this.GroupNeighbors(cell))
               adjacencyCount++;
            return adjacencyCount;
        }

        public int LowestAvailableColor(IEnumerable<Cell> groupCells)
        {
            bool[] usedBrushes = new bool[Brushes.BrushSet.Length];
            Group otherCellGroup;
            if (cells == null)
                return Brushes.DefaultBrushId;
            foreach (Cell cell in groupCells)
            {
                foreach (Cell neighbor in this.Neighbors(cell))
                {

                    otherCellGroup = neighbor.Group;

                    if (otherCellGroup == null) //no other group
                        continue;
                    if (cell.Group == otherCellGroup) //same group
                        continue;

                    usedBrushes[otherCellGroup.BrushId] = true; //other group
                }
                
            }
            for (int i = 0; i < usedBrushes.Length; i++)
                if (!usedBrushes[i]) return i;
            return -1;
        }

        public IEnumerable<Cell> Neighbors(Cell cell)
        {
            int rindex, cindex;
            for (int i = -1; i <= 1; i++) //row
            {
                rindex = i + cell.Row;
                if (rindex < 0 || rindex >= size) //out of bounds
                    continue;

                for (int j = -1; j <= 1; j++) //col
                {
                    if (i == 0 && j == 0) //self
                        continue;

                    if (!(i == 0 || j == 0)) //disables corner checking
                        continue;

                    cindex = j + cell.Column;
                    if (cindex < 0 || cindex >= size)
                        continue;

                    yield return this.cells[rindex][cindex];
                }
            }
        }

        public IEnumerable<Cell> GroupNeighbors(Cell cell)
        {
            Group cellGroup = cell.Group;
            foreach (Cell neighbor in this.Neighbors(cell))
                if (neighbor.Group == cellGroup)
                    yield return neighbor;
        }
    }

    internal class Cell
    {
        private int row, col, id;
        private ICellRepresentation representation;
        private Group group;
        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        public int Row
        {
            get { return this.row; }
        }
        public int Column
        {
            get { return this.col; }
        }
        public Group Group
        {
            get { return this.group; }
            set { this.group = value; }
        }
        
        /// <summary>
        /// An arbitrary, temporary integer identifier
        /// </summary>
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        public ICellRepresentation Representation
        {
            get { return this.representation; }
            set
            {
                this.representation = value;
                this.representation.Tag = this;
            }
        }

    }
}
