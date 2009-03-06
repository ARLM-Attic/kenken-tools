using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace KenKenEngine
{
    internal class PruningPermutator : IPermutator
    {
        //metrics
        private int numcalls;
        private int numgen;
        private long timeElapsed;
        

        private IStateValidator validator;

        //recursive state
        private List<int[]> validStates;
        private PermutationPrefixTrie trie;

        //async
        private BackgroundWorker worker;
        private bool asyncStop;

        public PruningPermutator(IStateValidator validator)
        {
            this.validator = validator;
        }

        public IEnumerable<int[]> PermuteAsync(BackgroundWorker worker, int[] state)
        {
            if (worker.WorkerSupportsCancellation)
                this.worker = worker;
            return Permute(state);
        }

        /// <summary>
        /// Permutes a pre-normalized set, pruning invalid permutations
        /// and duplicates due to repetition
        /// </summary>
        /// <param name="state">The state to permute</param>
        /// <returns>Valid unique permutations of the state</returns>
        public IEnumerable<int[]> Permute(int[] state)
        {
            this.asyncStop = false;

            //initialize return value
            this.validStates = new List<int[]>();
            
            //initialize metrics
            this.numcalls = 0;
            this.numgen = 0;
            this.timeElapsed = 0;
            
            //set up trie
            Dictionary<int,int> unique = new Dictionary<int,int>();
            foreach(int num in state){
                unique[num]=1;
            }
            this.trie = new PermutationPrefixTrie(unique.Keys.Count);

            //execute
            rPermute(state, 0);
            return validStates;
        }

        private void rPermute(int[] state, int remainingStartsAt)
        {
            this.numcalls++;
            
            //base case
            if (remainingStartsAt == state.Length - 1)
            {
                this.numgen++;
                this.validStates.Add(state);
            }

            //noop case, has already been explored
            else if (trie.HasPath(state, remainingStartsAt))
            {
                return;
            }

            //permute
            else
            {
                if (this.worker != null && this.worker.CancellationPending)
                {
                    this.asyncStop = true;
                    return;
                }
                for (int i = remainingStartsAt; i < state.Length; i++)
                {
                    //check for cancellation
                    if(this.asyncStop)
                        return;

                    //make a copy
                    int[] newstate = new int[state.Length];
                    for (int j = 0; j < state.Length; j++) newstate[j] = state[j];

                    //swap in 
                    newstate[remainingStartsAt] = newstate[i];
                    newstate[i] = state[remainingStartsAt];

                    //execute
                    if (this.validator.IsValid(newstate, remainingStartsAt + 1))
                        rPermute(newstate, remainingStartsAt + 1);
                }
                this.trie.SetPath(state, remainingStartsAt);
            }
        }

        public int RecursiveCallsMetric
        {
            get { return this.numcalls; }
        }

        public int GeneratedStatesMetric
        {
            get { return this.numgen; }
        }

        public long ElapsedTimeMetric
        {
            get { return this.timeElapsed; }
        }
    }
}
