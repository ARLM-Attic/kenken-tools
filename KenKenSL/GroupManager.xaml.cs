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
        private EventHandler handlerUpGroupPressed;
        private EventHandler handlerDownGroupPressed;

        public GroupManager()
        {
            InitializeComponent();
            this.groups = new List<Group>();
            this.bdrGroup.BorderThickness = new Thickness(0);
            this.handlerGroupPaintClicked = new EventHandler(group_PaintClicked);
            this.handlerUpGroupPressed = new EventHandler(group_UpGroupPressed);
            this.handlerDownGroupPressed = new EventHandler(group_DownGroupPressed);
            this.Loaded += new RoutedEventHandler(GroupManager_Loaded);
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
                    AddAndFocusGroup();
                }
            }
        }

        public void AddAndFocusGroup()
        {
            Group group = addGroup();
            group_PaintClicked(group, EventArgs.Empty);
            group.FocusTotal();
        }

        private Group addGroup()
        {
            Group group = new Group();
            group.CellManager = this.CellManager;
            group.PaintClicked += handlerGroupPaintClicked;
            group.UpGroupPressed += handlerUpGroupPressed;
            group.DownGroupPressed += handlerDownGroupPressed;
            if (groups.Count == 0)
                this.bdrGroup.BorderThickness = new Thickness(1);
            this.groups.Add(group);
            this.spGroups.Children.Add(group);
            this.svGroups.ScrollToVerticalOffset(this.svGroups.ExtentHeight);
            return group;
        }

        #region event handlers
        private void GroupManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.groups.Count > 0)
            {
                if (this.groups[0] != null)
                {
                    this.groups[0].FocusTotal();
                }
            }
        }

        private void bAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddAndFocusGroup();
        }

        private void group_PaintClicked(object sender, EventArgs e)
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

        private void group_UpGroupPressed(object sender, EventArgs e)
        {
            Group senderGroup = sender as Group;
            if (senderGroup == null)
                return;
            int index = this.groups.IndexOf(senderGroup);
            if (index > -1) //is valid
            {
                if (index > 0) //is not first group
                {
                    Group upGroup = this.groups[index - 1];
                    group_PaintClicked(upGroup, EventArgs.Empty);
                    upGroup.FocusTotal();
                }
            }
        }

        private void group_DownGroupPressed(object sender, EventArgs e)
        {
            Group senderGroup = sender as Group;
            if (senderGroup == null)
                return;
            int index = this.groups.IndexOf(senderGroup);
            if (index > -1) //is valid
            {
                if (index < this.groups.Count - 1) //is not last group
                {
                    Group downGroup = this.groups[index + 1];
                    group_PaintClicked(downGroup, EventArgs.Empty);
                    downGroup.FocusTotal();
                }
                else
                {
                    AddAndFocusGroup();
                }
            }
        }

        #endregion
    }
}
