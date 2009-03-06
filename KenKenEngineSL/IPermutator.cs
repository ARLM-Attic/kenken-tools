using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    internal interface IPermutator
    {
        IEnumerable<int[]> Permute(int[] state);

        int RecursiveCallsMetric
        {
            get;
        }

        int GeneratedStatesMetric
        {
            get;
        }

        long ElapsedTimeMetric
        {
            get;
        }
    }
}
