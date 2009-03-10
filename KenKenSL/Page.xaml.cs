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
using KenKenEngine;
using System.Threading;
using System.ComponentModel;
using System.Globalization;

namespace KenKenSL
{
    public partial class Page : UserControl
    {
        private int size;
        private Board board;
        private CellManager cellManager;
        private GroupManager groupManager;
        private List<int[][]> solutions;
        private int currentSolutionIndex;
        private Solver solver;

        public Page()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Page_KeyDown);
        }

        void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.G &&
                (Keyboard.Modifiers == ModifierKeys.Control) ||
                (Keyboard.Modifiers == ModifierKeys.Apple))
            {
                if (this.groupManager != null)
                {
                    this.groupManager.AddAndFocusGroup();
                }
            }
        }

        private void createComponents(int size)
        {
            this.gridOuter.Children.Remove(this.tbTitle);
            this.size = size;
            this.cellManager = new CellManager(size);
            this.gridComponents.Children.Clear();
            this.groupManager = new GroupManager();
            this.groupManager.SetValue(Grid.ColumnProperty, 0);
            this.groupManager.SetValue(Grid.RowProperty, 0);
            this.groupManager.CellManager = this.cellManager;
            this.gridComponents.Children.Add(this.groupManager);
            this.board = new Board();
            this.board.GroupManager = this.groupManager;
            this.board.SetValue(Grid.ColumnProperty, 1);
            this.board.SetValue(Grid.RowProperty, 0);
            this.board.CellManager = this.cellManager;
            this.board.HorizontalAlignment = HorizontalAlignment.Right;
            this.gridComponents.Children.Add(this.board);
            this.board.Size = size;
            this.tbSolveStatus.Visibility = Visibility.Visible;
            this.bSolve.Visibility = Visibility.Visible;
            
        }

        private void bSize4_Click(object sender, RoutedEventArgs e)
        {
            createComponents(4);
        }
        
        private void bSize6_Click(object sender, RoutedEventArgs e)
        {
            createComponents(6);
        }

        private void bSize8_Click(object sender, RoutedEventArgs e)
        {
            createComponents(8);
        }

        private void bSolnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentSolutionIndex > 0 && this.solutions.Count > 0)
            {
                if (this.currentSolutionIndex == 1)
                    bSolnPrev.IsEnabled = false;
                showSolution(this.currentSolutionIndex--);
                if (this.currentSolutionIndex < this.solutions.Count - 1)
                    this.bSolnNext.IsEnabled = true;
            }
        }

        private void bSolnNext_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentSolutionIndex < this.solutions.Count - 1)
            {
                if (this.currentSolutionIndex == this.solutions.Count - 2)
                    bSolnNext.IsEnabled = false;
                showSolution(this.currentSolutionIndex++);
                if (this.currentSolutionIndex > 0)
                    this.bSolnPrev.IsEnabled = true;
            }

        }

        private void bSolve_Click(object sender, RoutedEventArgs e)
        {
            if (cellManager == null || groupManager == null)
                return;

            if (this.solver!=null && this.solver.IsBusy)
            {
                this.solver.CancelAsync();
                return;
            }

            solver = new Solver(this.size);
            foreach (Group group in this.groupManager.Groups)
            {
                if (group.IsValid == false && group.CellCount != 0)
                {
                    this.tbSolveStatus.Text = "One or more groups are not valid: ";
                    this.solver = null;
                    return;
                }

                if (group.CellCount == 0)
                    continue;

                List<Coordinates> coordinatesSet = new List<Coordinates>();
                foreach (Cell cell in group.Cells)
                {
                    coordinatesSet.Add(new Coordinates(cell.Row, cell.Column));
                }
                Operation op;
                switch (group.Operation)
                {
                    case "+":
                        op = Operation.Add;
                        break;
                    case "-":
                        op = Operation.Subtract;
                        break;
                    case "\u00D7":
                        op = Operation.Multiply;
                        break;
                    case "\u00F7":
                        op = Operation.Divide;
                        break;
                    case " ":
                        op = Operation.Add;
                        break;
                    default:
                        throw new NotSupportedException("Unknown Operand " + group.Operation);
                }
                solver.AddGroup(coordinatesSet, op, group.Total);
            }
            solver.SolveCompleted +=new EventHandler<SolveCompletedEventArgs>(solver_SolveCompleted);
            solver.SolveAsync();
            
            this.bSolve.Content = "Stop";
            
        }

        void solver_SolveCompleted(object sender, SolveCompletedEventArgs e)
        {
            processSolutions(e.Solutions);
        }

        private void processSolutions(IEnumerable<KenKenEngine.Board> solns)
        {
            clearSolutions();
            this.solutions = new List<int[][]>();
            foreach (KenKenEngine.Board soln in solns)
            {
                this.solutions.Add(soln.ToMatrix());
            }
            this.currentSolutionIndex = 0;
            if (this.solutions.Count > 0)
            {
                
                if (this.solutions.Count == 1)
                    this.tbSolveStatus.Text = this.solutions.Count + " Solution: ";
                else // # of solns > 1
                {
                    this.tbSolveStatus.Text = this.solutions.Count + " Solutions: ";
                    this.bSolnPrev.Visibility = Visibility.Visible;
                    this.bSolnNext.Visibility = Visibility.Visible;
                    this.bSolnPrev.IsEnabled = false;
                    this.bSolnNext.IsEnabled = true;
                }
                showSolution(0);
            }
            else
            {
                this.tbSolveStatus.Text = "Hmm. No Solutions: ";
            }
        }

        private void showSolution(int index)
        {
            int[][] matrix = this.solutions[index];
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    this.cellManager.GetCell(i, j).Representation.Text = matrix[i][j].ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        private void clearSolutions()
        {
            this.solutions = null;
            this.currentSolutionIndex = 0;
            this.bSolnPrev.Visibility = Visibility.Collapsed;
            this.bSolnNext.Visibility = Visibility.Collapsed;
            this.bSolve.Content = "Solve";
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    this.cellManager.GetCell(i, j).Representation.Text = string.Empty;
                }
            }
        }

    }
}
