using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenKenEngine
{
    internal class GroupManager
    {
        private List<Group> groups;
        private int groupMaxIndexMin;
        private const int indexInvalid = -1;

        public GroupManager()
        {
            this.groups = new List<Group>();
            this.groupMaxIndexMin = indexInvalid;
        }
        
        public void AddGroup(Group group)
        {
            this.groups.Add(group);
            
            //calculate the minimum of the max indexes,
            //determines whether any groups are validatable
            //at a certain point in the permutations
            if (groupMaxIndexMin == indexInvalid)
                groupMaxIndexMin = group.MaxIndex;
            else if (group.MaxIndex < this.groupMaxIndexMin)
                groupMaxIndexMin = group.MaxIndex;
            
            this.groups.Sort();
        }

        public int GetNumValidatableGroups(int statelength)
        {
            int grouplength = 0;
            
            //no groups validatable yet
            if (groupMaxIndexMin >= statelength)
                return grouplength;
            
            foreach (Group g in this.groups)
            {
                if (g.MaxIndex < statelength)
                    grouplength++;
                else
                    break;
            }

            return grouplength;
        }

        public List<Group> Groups
        {
            get { return this.groups; }
        }
    }
}
