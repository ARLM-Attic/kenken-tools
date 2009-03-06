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

namespace KenKenSL
{
    /// <summary>
    /// Board is a creator class for creating sets of cells
    /// with a particular representation
    /// </summary>
    internal partial class Board : UserControl
    {
        private int size;
        private GroupManager groupManager;
        private CellManager cellManager;
        private MouseButtonEventHandler handlerCellMouseDown;
        

        public Board()
        {
            InitializeComponent();
            this.handlerCellMouseDown = new MouseButtonEventHandler(cell_MouseLeftButtonDown);
        }
        
        /// <summary>
        /// Sets the size, which triggers creation
        /// </summary>
        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                buildSolveBoard();
            }
        }

        /// <summary>
        /// GroupManager must be supplied to handle cell click events
        /// </summary>
        public GroupManager GroupManager
        {
            set { this.groupManager = value; }
        }

        /// <summary>
        /// CellManager must be supplied to associate backend representation
        /// with visual cells
        /// </summary>
        public CellManager CellManager
        {
            set { this.cellManager = value; }
        }


        private void buildSolveBoard()
        {
            this.canvasBoard.Children.Clear();
            double cellSize = this.canvasBoard.Height / size;
            double borderThickness=1.0;
            for (int i = 0; i < this.size; i++)//row
            {
                for (int j = 0; j < this.size; j++)//column
                {
                    ICellRepresentation cell = new BorderedTextBlockRepresentation(
                        cellSize, cellSize, 
                        new Thickness(borderThickness), 
                        new SolidColorBrush(Colors.DarkGray));

                    cell.Clicked += this.handlerCellMouseDown;
                    this.cellManager.GetCell(i, j).Representation = cell;
                    cell.Element.SetValue(Canvas.LeftProperty, (double) j * cellSize);
                    cell.Element.SetValue(Canvas.TopProperty, (double) i * cellSize);
                    this.canvasBoard.Children.Add(cell.Element);
                }
            }
        }


        private void cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;
            
            FrameworkElement senderControl = sender as FrameworkElement;
            if (senderControl == null)
                return;

            Cell cell = senderControl.Tag as Cell;
            if (cell == null || this.groupManager.ActiveGroup == null)
                return;
            
            if (cell.Group != this.groupManager.ActiveGroup)
            {
                if (cell.Group != null)
                {

                    if (cell.Group.RemoveCell(cell))
                    {
                        this.groupManager.ActiveGroup.AddCell(cell);
                    }
                }
                else
                {
                    this.groupManager.ActiveGroup.AddCell(cell);
                }
            }
            else //like undo
            {
                this.groupManager.ActiveGroup.RemoveCell(cell);
            }
        }
            
        
    }
}
