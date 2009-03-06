using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KenKenEngine
{
    
    /// <summary>
    /// Interface to the KenKen Engine
    /// </summary>
    public class Solver
    {
        private int size;
        private bool[] groupMap;
        private GroupManager groupManager;
        private BackgroundWorker solveWorker;

        

        /// <summary>
        /// Called when the all solutions are discovered or when the solve
        /// action is cancelled.  The callback is safe to execute in the UI thread.
        /// </summary>
        public event EventHandler<SolveCompletedEventArgs> SolveCompleted;

        /// <summary>
        /// Construct a KenKen Solving Engine
        /// </summary>
        /// <param name="size">The size of the board.  A 4x4 board would have size 4</param>
        public Solver(int size)
        {
            if (size < 0 || size > byte.MaxValue)
                throw new ArgumentOutOfRangeException("size");
            this.size = size;
            this.groupMap = new bool[size * size];
            this.groupManager = new GroupManager();
        }

        /// <summary>
        /// Add an arithmetic group contraint
        /// </summary>
        /// <param name="coordinatesSet">List of coordinates of each group member.  
        /// Coordinates have a top-left (0,0) origin</param>
        /// <param name="operation">Arithmetic operation</param>
        /// <param name="total">Desired result of the arithmetic operation</param>
        public void AddGroup(IEnumerable<Coordinates> coordinatesSet, Operation operation, int total)
        {
            List<int> cellIndicies = new List<int>();
            foreach (Coordinates coord in coordinatesSet)
            {
                if (coord.Column < 0 ||
                    coord.Column >= size ||
                    coord.Row < 0 ||
                    coord.Row >= size)
                    throw new ArgumentOutOfRangeException(coord.ToString());
                int index = (coord.Row * size) + coord.Column;
                if (groupMap[index])
                    throw new ArgumentException("overlapping groups");
                else
                {
                    cellIndicies.Add(index);
                    groupMap[(coord.Row * size) + coord.Column] = true;
                }
            }
            Group group = new Group(cellIndicies.ToArray(), total, operation);
            groupManager.AddGroup(group);
        }

        /// <summary>
        /// Solves the board with the given constraints
        /// </summary>
        /// <returns>A set of solutions to the problem</returns>
        public IEnumerable<Board> Solve()
        {
            List<Board> possibleBoards = new List<Board>();

            BoardStateValidator boardValidator = new BoardStateValidator(this.groupManager, this.size);
            PruningPermutator permutator = new PruningPermutator(boardValidator);
            foreach (int[] permutedState in permutator.Permute(initialBoard))
            {
                possibleBoards.Add(new Board(permutedState,this.size));
            }
            return possibleBoards;
        }

        #region Asynchronous Interface

        /// <summary>
        /// Solves the board with the given constraints asynchronously.
        /// Results are returned through the SolveCompleted event.
        /// Only one asynchronous process can occur at a time.
        /// </summary>
        public void SolveAsync()
        {
            if (this.solveWorker != null)
                throw new NotSupportedException("Cannot support more than one asynchronous process at a time.  Cancel the pending process or wait for it to complete.");
            this.solveWorker = new BackgroundWorker();
            this.solveWorker.DoWork += new DoWorkEventHandler(internalSolveAsync);
            this.solveWorker.WorkerSupportsCancellation = true;
            this.solveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(solveWorker_RunWorkerCompleted);
            this.solveWorker.RunWorkerAsync();
        }

        private void internalSolveAsync(object sender, DoWorkEventArgs e)
        {
            List<Board> possibleBoards = new List<Board>();
            BoardStateValidator boardValidator = new BoardStateValidator(this.groupManager, this.size);
            PruningPermutator permutator = new PruningPermutator(boardValidator);
            if (this.solveWorker.CancellationPending)
                e.Cancel = true;
            foreach (int[] permutedState in permutator.PermuteAsync(this.solveWorker, initialBoard))
            {
                possibleBoards.Add(new Board(permutedState, this.size));
            }
            e.Result = possibleBoards;
        }

        public void CancelAsync()
        {
            if (this.solveWorker != null && this.solveWorker.IsBusy)
                this.solveWorker.CancelAsync();
        }

        /// <summary>
        /// Indicates the state of the Aynchronous operation.
        /// True if an asynchronous operation is in process.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.solveWorker != null && this.solveWorker.IsBusy;
            }
        }

        private void solveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IEnumerable<Board> solutions = e.Result as IEnumerable<Board>;
            this.solveWorker = null;
            if (SolveCompleted != null)
            {
                SolveCompleted(this, new SolveCompletedEventArgs(solutions));
            }
        }

        #endregion

        private int[] initialBoard
        {
            get
            {
                int[] board = new int[size * size];
                for (int i = 0; i < board.Length; i++)
                    board[i] = i % size;
                return board;
            }
        }
    }

    public class SolveCompletedEventArgs : EventArgs
    {
        private IEnumerable<Board> solutions;
        internal SolveCompletedEventArgs(IEnumerable<Board> solutions)
        {
            this.solutions = solutions;
        }

        public IEnumerable<Board> Solutions
        {
            get { return this.solutions; }
        }
    }
}
