using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    internal class Group : IComparable<Group>
    {
        private int[] cellIndicies;
        private readonly int maxIndex;
        private int total;
        private Operation op;
        
        public Group(int[] cellIndicies, int total, Operation op)
        {
            if (cellIndicies.Length < 1)
                throw new ArgumentException("not enough cells in group", "cellIndicies");
            
            this.cellIndicies = cellIndicies;
            this.total = total;
            this.op = op;

            //determine how early in the permutations can
            //validity be determined?
            maxIndex = cellIndicies[0];
            for (int i = 1; i < cellIndicies.Length; i++)
                if (maxIndex < cellIndicies[i])
                    maxIndex = cellIndicies[i];
        }

        public bool IsValid(int[] state)
        {
            int[] sortedValues;
            switch (this.op)
            {
                case Operation.Add:
                    int add = state[cellIndicies[0]] + 1;
                    for (int i = 1; i < cellIndicies.Length; i++)
                        add += state[cellIndicies[i]] + 1;
                    if (add == total)
                        return true;
                    return false;

                case Operation.Multiply:
                    int mul = state[cellIndicies[0]] + 1;
                    for (int i = 1; i < cellIndicies.Length; i++)
                        mul *= state[cellIndicies[i]] + 1;
                    if (mul == total)
                        return true;
                    return false;

                case Operation.Subtract:
                    sortedValues = indexedSortedValues(state);
                    int sub = sortedValues[sortedValues.Length - 1] + 1;
                    for (int i = sortedValues.Length - 2; i >= 0; i--)
                        sub -= sortedValues[i] + 1;
                    if (sub == total)
                        return true;
                    return false;

                case Operation.Divide:
                    sortedValues = indexedSortedValues(state);
                    int div = sortedValues[sortedValues.Length - 1] + 1;
                    for (int i = sortedValues.Length - 2; i >= 0; i--)
                        div /= sortedValues[i] + 1;
                    if (div == total)
                        return true;
                    return false;
                default:
                    throw new NotImplementedException();
            }

        }

        private int[] indexedSortedValues(int[] state)
        {
            int[] sortedStateValues = new int[cellIndicies.Length];
            for (int i = 0; i < cellIndicies.Length; i++)
                sortedStateValues[i] = state[cellIndicies[i]];
            Array.Sort(sortedStateValues);
            return sortedStateValues;
        }
        

        public int MaxIndex
        {
            get { return this.maxIndex; }
        }



        #region IComparable<Group> Members

        public int CompareTo(Group other)
        {
            return this.MaxIndex.CompareTo(other.MaxIndex);
        }

        #endregion
    }
}
