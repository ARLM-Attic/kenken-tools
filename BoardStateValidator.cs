using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    internal class BoardStateValidator : IStateValidator
    {
        protected int size;
        protected GroupManager groupManager;

        public BoardStateValidator(GroupManager groupManager, int size)
        {
            this.size = size;
            this.groupManager = groupManager;
        }

        protected virtual bool areGroupsValid(int[] cells, int length)
        {
            int numusablegroups = this.groupManager.GetNumValidatableGroups(length);
            for (int i = 0; i < numusablegroups; i++)
            {
                if (!groupManager.Groups[i].IsValid(cells))
                    return false;
            }
            return true;
        }

        protected virtual bool areRowsValid(int[] cells, int length)
        {
            bool[] used = new bool[size];
            for (int i = 0; i < length; i++)
            {
                if (i % size == 0)
                {
                    for (int j = 0; j < size; j++)
                        used[j] = false;
                }
                if (used[cells[i]])
                    return false;
                used[cells[i]] = true;
            }
            return true;
        }

        protected virtual bool areColsValid(int[] cells, int length)
        {
            //fixup removal of last layer of recursion tree
            if (length == cells.Length - 1)
                length = cells.Length;

            //no columns?
            if (!(length > size))
                return true;

            bool[] used = new bool[size];
            for (int i = 0; i < size; i++)
            {

                //column tallness has thinned to 1, so valid
                if (i + size >= length)
                    return true;

                //initialize column
                for (int j = 0; j < size; j++)
                    used[j] = false;

                //do check
                int k = i;
                while (k < length)
                {
                    if (used[cells[k]])
                        return false;
                    used[cells[k]] = true;
                    k += size;
                }
            }
            return true;
        }

        #region IStateValidator Members

        public virtual bool IsValid(int[] state, int length)
        {
            return this.areRowsValid(state, length) &&
                this.areColsValid(state, length) &&
                this.areGroupsValid(state, length);
        }

        #endregion
    }
}
