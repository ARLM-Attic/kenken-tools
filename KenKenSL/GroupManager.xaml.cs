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
    public partial class GroupManager : UserControl
    {
        private List<Group> groups;
        private Group activeGroup;
        private CellManager cellManager;

        private EventHandler handlerGroupPaintClicked;
        public GroupManager()
        {
            InitializeComponent();
            this.groups = new List<Group>();
            this.bdrGroup.BorderThickness = new Thickness(0);
            this.handlerGroupPaintClicked = new EventHandler(group_PaintClicked);
        }

        public Group ActiveGroup
        {
            get { return this.activeGroup; }
        }

        public IEnumerable<Group> Groups
        {
            get { return this.groups; }
        }

        internal CellManager CellManager
        {
            get { 
                return this.cellManager; 
            }
            set
            {
                this.cellManager = value;
                if (this.cellManager != null && this.groups.Count == 0)
                {
                    addGroup();
                    group_PaintClicked(this.groups[0], EventArgs.Empty);
                }
            }
        }

        private void bAddGroup_Click(object sender, RoutedEventArgs e)
        {
            addGroup();
        }

        private void addGroup()
        {
            Group group = new Group();
            group.CellManager = this.CellManager;
            group.PaintClicked += handlerGroupPaintClicked;
            if (groups.Count == 0)
                this.bdrGroup.BorderThickness = new Thickness(1);
            this.groups.Add(group);
            this.spGroups.Children.Add(group);
            this.svGroups.ScrollToVerticalOffset(this.svGroups.ViewportHeight);
        }

        void group_PaintClicked(object sender, EventArgs e)
        {
            Group senderGroup = sender as Group;
            if (senderGroup == null)
                return;
            if (this.activeGroup != null)
            {
                if (senderGroup != this.activeGroup)
                    this.activeGroup.IsActiveGroup = false;
            }
            this.activeGroup = senderGroup;
            this.activeGroup.IsActiveGroup = true;

        }
        
    }
}
