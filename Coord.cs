using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    /// <summary>
    /// A container class that represents coordinates the origin, (0,0)
    /// is the top left corner.  (size, size) is the bottom right corner
    /// </summary>
    public class Coordinates
    {
        private int row, column;
        public Coordinates(int row, int column)
        {
            if (row < 0 || column < 0)
                throw new ArgumentException("row and/or column cannot be negative");
            
            this.row = row;
            this.column = column;
        }
        public int Row
        {
            get { return this.row; }
        }
        public int Column
        {
            get { return this.column; }
        }

        public override string ToString()
        {
            return "(row: " + this.row + " column:" + this.column + ")";
        }
    }
}
