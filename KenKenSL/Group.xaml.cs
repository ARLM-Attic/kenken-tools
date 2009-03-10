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
        public event EventHandler UpGroupPressed;
        public event EventHandler DownGroupPressed;
        
        private List<Cell> cells;
        private int brushId;
        private CellManager cellManager;
        private bool isActiveGroup;
        
        //validation
        private Brush tbTotalValidBrush;
        private static readonly Brush tbTotalNotValidBrush = new SolidColorBrush(Colors.Red);
        private const int notValidTotal = int.MinValue;

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
            get { return getTotal(); }
        }

        public bool IsValid
        {
            get { return getTotal() != notValidTotal; }
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

        public int CellCount
        {
            get { return this.cells.Count; }
        }

        internal IEnumerable<Cell> Cells
        {
            get { return this.cells; }
        }

        public void FocusTotal()
        {
            this.tbTotal.Focus();
        }

        #region cell management
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
        #endregion

        #region private methods

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

        /// <summary>
        /// gets the total entered by the user
        /// </summary>
        /// <returns>total, Group.notValidTotal if invalid</returns>
        private int getTotal()
        {
            if (string.IsNullOrEmpty(tbTotal.Text))
                return notValidTotal;
            else
            {
                try
                {
                    int total = int.Parse(this.tbTotal.Text, CultureInfo.CurrentCulture);
                    this.tbTotal.Background = this.tbTotalValidBrush;
                    return total;
                }
                catch (FormatException) { }
                this.tbTotal.Background = tbTotalNotValidBrush;
                return notValidTotal;
            }
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
            if (!string.IsNullOrEmpty(this.tbTotal.Text))
            {
                int len = this.tbTotal.Text.Length;
                string strOp = this.tbTotal.Text.Substring(len - 1, 1);
                bool opFound = true;
                switch (strOp)
                {
                    case "+":
                        this.cbOperation.SelectedIndex = 0;
                        break;
                    case "-":
                        this.cbOperation.SelectedIndex = 1;
                        break;
                    case "*":
                        this.cbOperation.SelectedIndex = 2;
                        break;
                    case "/":
                        this.cbOperation.SelectedIndex = 3;
                        break;
                    case " ":
                        this.cbOperation.SelectedIndex = 4;
                        break;
                    default:
                        opFound = false;
                        break;
                }
                if (opFound)
                {
                    this.tbTotal.Text = this.tbTotal.Text.Substring(0, len - 1);
                }
            }
            
            if (this.IsValid)
            {
                setOperationLabel();
            }
        }

        private void components_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PaintClicked != null)
            {
                PaintClicked(this, EventArgs.Empty);
            }
        }

        private void layout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PaintClicked != null)
            {
                PaintClicked(this, EventArgs.Empty);
                this.tbTotal.Focus();
            }
        }

        private void components_Focused(object sender, RoutedEventArgs e)
        {
            if (PaintClicked != null)
            {
                PaintClicked(this, EventArgs.Empty);
            }
        }

        #endregion

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (UpGroupPressed != null)
                {
                    UpGroupPressed(this, EventArgs.Empty);
                }
            }
            else if (e.Key == Key.Down)
            {
                if (DownGroupPressed != null)
                {
                    DownGroupPressed(this, EventArgs.Empty);
                }
            }
        }
    }
}
