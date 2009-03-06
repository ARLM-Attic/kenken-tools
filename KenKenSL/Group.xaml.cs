using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace KenKenSL
{
    public partial class Group : UserControl
    {
        public event EventHandler PaintClicked;
        private List<Cell> cells;
        private int brushId;
        private CellManager cellManager;
        private bool isActiveGroup;
        private Brush tbTotalValidBrush;

        public Group()
        {
            InitializeComponent();
            this.cbOperation.SelectedIndex = 0;
            this.cells = new List<Cell>();
            this.BrushId = Brushes.DefaultBrushId;
            this.tbTotalValidBrush = this.tbTotal.Background;
            this.LayoutRoot.Cursor = Cursors.Hand;
            this.cbOperation.Cursor = Cursors.Arrow;
        }

        public int BrushId
        {
            get
            {
                return this.brushId;
            }
            set
            {
                this.brushId = value;
                this.setCellBrushes();
            }
        }

        public bool IsActiveGroup
        {
            get
            {
                return this.isActiveGroup;
            }
            set
            {
                this.isActiveGroup = value;
                this.setCellBrushes();
            }
        }

        public int Total
        {
            get
            {
                int total = 0;
                try
                {
                    total = int.Parse(this.tbTotal.Text,CultureInfo.CurrentCulture);
                }
                catch (FormatException) { }
                return total;
            }
        }

        public string Operation
        {
            get
            {
                ComboBoxItem op = this.cbOperation.SelectedItem as ComboBoxItem;
                if (op == null)
                    throw new InvalidCastException("Combobox item not a ComboBoxItem");
                return op.Content.ToString();
            }
        }

        internal IEnumerable<Cell> Cells
        {
            get { return this.cells; }
        }

        public int CellCount
        {
            get { return this.cells.Count; }
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(tbTotal.Text))
                    return false;
                else
                {
                    try
                    {
                        int total = int.Parse(this.tbTotal.Text,CultureInfo.CurrentCulture);
                        return true;
                    }
                    catch (FormatException) { }
                    return false;
                }
            }
        }

        internal CellManager CellManager
        {
            set { this.cellManager = value; }
        }

        /// <summary>
        /// Adds a cell to the group
        /// </summary>
        /// <param name="cell">Cell to add</param>
        /// <returns>True if successfully added, otherwise false</returns>
        internal bool AddCell(Cell cell)
        {
            if (this.cells.Count > 0)
            {
                bool adjacent = false;
                foreach (Cell member in this.cells)
                {
                    adjacent |= CellManager.AreAdjacent(cell, member);
                }
                if (!adjacent)
                    return false;
            }
            cell.Group = this;
            this.cells.Add(cell);

            int lowestColor = this.cellManager.LowestAvailableColor(this.cells);
            
            if (lowestColor < Brushes.DefaultBrushId)
                this.brushId = lowestColor;
            cell.Representation.Background = internalBrush;
            setOperationLabel();
            return true;
        }


        /// <summary>
        /// Removes a cell to the group
        /// </summary>
        /// <param name="cell">Cell to remove</param>
        /// <returns>True if successfully removed, otherwise false</returns>
        internal bool RemoveCell(Cell cell)
        {
            bool remove = false;
            int adjacencyCount = this.cellManager.AdjacencyCount(cell);
            if (adjacencyCount > 1 && this.cells.Count > 1)
            {
                remove = shouldRemove(cell);
            }
            else
            {
                remove = true;
            }
            if (remove)
            {
                cell.Representation.GroupLabel = null;
                this.cells.Remove(cell);
                cell.Group = null;
                cell.Representation.Background = Brushes.BrushSet[Brushes.DefaultBrushId];
                if (this.cells.Count == 0)
                    this.BrushId = Brushes.DefaultBrushId;
                setOperationLabel();
            }
            return remove;
        }

        /// <summary>
        /// Checks to see if removing a cell
        /// would split the group into two
        /// disjoint subgraphs.
        /// (i.e. whether cell to be removed
        /// is an articulation point)
        /// Implemented using Sedgewick's
        /// quick-find algorithm because
        /// N is small.
        /// </summary>
        /// <param name="removeCell">cell to remove</param>
        /// <returns>true if can be removed without separating the graph</returns>
        private bool shouldRemove(Cell removeCell)
        {
            //initialize
            int[] root = new int[cells.Count - 1];
            int i = 0;
            foreach (Cell cell in this.cells)
            {
                if (cell == removeCell)
                    continue;
                cell.Id = i;
                root[i] = i;
                i++;
            }

            //expand nodes and root them
            foreach (Cell cell in this.cells)
            {
                if (cell == removeCell) continue;
                foreach (Cell neighbor in this.cellManager.GroupNeighbors(cell))
                {
                    if (neighbor == removeCell) continue;
                    int p = cell.Id;
                    int q = neighbor.Id;
                    int t = root[p];
                    if (t == root[q]) continue;
                    //root all nodes
                    for (i = 0; i < root.Length; i++)
                    {
                        if (root[i] == t)
                            root[i] = root[q];
                    }
                }
            }

            //check all same root
            int onlyRoot = root[0];
            for (i = 0; i < root.Length; i++)
            {
                if (root[i] != onlyRoot)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Stores the brush while the group is the ActiveGroup
        /// </summary>
        private Brush internalBrush
        {
            get
            {
                if (this.IsActiveGroup)
                {
                    return Brushes.BrushSet[Brushes.ActiveBrushId];
                }
                else
                {
                    return Brushes.BrushSet[this.brushId];
                }
            }
        }

        /// <summary>
        /// Places the operation label of the group in the
        /// top-left corner of the topmost leftmost cell
        /// </summary>
        private void setOperationLabel()
        {
            //need this because called at init
            if (this.cells == null || this.cells.Count < 1)
                return;
            ComboBoxItem op = this.cbOperation.SelectedItem as ComboBoxItem;

            string label = this.tbTotal.Text + Operation;
            Cell topMostLeftMost = this.cells[0];
            foreach (Cell cell in this.cells)
            {
                cell.Representation.GroupLabel = null;
                if (cell.Row < topMostLeftMost.Row)
                {
                    topMostLeftMost = cell;
                }
                else if (cell.Row == topMostLeftMost.Row)
                {
                    if (cell.Column < topMostLeftMost.Column)
                    {
                        topMostLeftMost = cell;
                    }
                }
            }
            topMostLeftMost.Representation.GroupLabel = label;
        }

        private void setCellBrushes()
        {
            this.LayoutRoot.Background = internalBrush;

            foreach (Cell cell in this.cells)
            {
                cell.Representation.Background = internalBrush;
            }
        }

        private void cbOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setOperationLabel();
        }

        private void tbTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsValid)
            {
                setOperationLabel();
                this.tbTotal.Background = this.tbTotalValidBrush;
            }
            else
            {
                this.tbTotal.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void components_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PaintClicked != null)
            {
                PaintClicked(this, EventArgs.Empty);
            }
        }
        private void components_Click(object sender, RoutedEventArgs e)
        {
            if (PaintClicked != null)
            {
                PaintClicked(this, EventArgs.Empty);
            }
        }
    }
}
