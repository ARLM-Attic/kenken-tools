using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KenKenSL
{
    internal interface ICellRepresentation
    {
        event MouseButtonEventHandler Clicked;
        
        Brush Background
        {
            get;
            set;
        }

        string Text
        {
            get;
            set;
        }

        object Tag
        {
            get;
            set;
        }

        string GroupLabel
        {
            set;
        }

        FrameworkElement Element
        {
            get;
        }
    }

    internal class BorderedTextBlockRepresentation : ICellRepresentation
    {
        private TextBlock textBlock;
        private Border border;
        private Canvas canvas;
        private Border groupTextBlockBorder;
        private Thickness borderThickness;

        public BorderedTextBlockRepresentation(
            double height, 
            double width, 
            Thickness borderThickness, 
            Brush borderBrush)
        {
            this.borderThickness = borderThickness;
            this.textBlock = new TextBlock();
            this.textBlock.Height = height - borderThickness.Bottom - borderThickness.Top;
            this.textBlock.Width = width - borderThickness.Left - borderThickness.Right;
            this.textBlock.FontSize = height * .50;
            this.textBlock.TextAlignment = TextAlignment.Center;
            this.textBlock.Padding = new Thickness(0.0, height * .25, 0.0, height * .25);

            this.border = new Border();
            this.border.BorderThickness = borderThickness;
            this.border.BorderBrush = borderBrush;
            this.border.Child = this.textBlock;
            this.canvas = new Canvas();
            this.canvas.Height = height;
            this.canvas.Width = width;
            this.canvas.Children.Add(border);
            this.textBlock.MouseLeftButtonUp += new MouseButtonEventHandler(textBlock_MouseLeftButtonUp);
            this.textBlock.Cursor = Cursors.Hand;
        }

        void textBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Clicked != null)
                this.Clicked(sender, e);
        }

        #region ICellRepresentation Members

        public event MouseButtonEventHandler Clicked;

        public Brush Background
        {
            get { return this.border.Background; }
            set { this.border.Background = value; }
        }

        public string Text
        {
            get { return this.textBlock.Text; }
            set { this.textBlock.Text = value; }
        }

        public object Tag
        {
            get { return this.textBlock.Tag; }
            set { this.textBlock.Tag = value; }
        }

        public string GroupLabel
        {
            set
            {
                if (value == null || this.groupTextBlockBorder != null)
                {
                    this.canvas.Children.Remove(this.groupTextBlockBorder);
                    this.groupTextBlockBorder = null;
                }
                
                if (value != null)
                {
                    TextBlock groupTextBlock = new TextBlock();
                    groupTextBlock.Padding = new Thickness(3.0);
                    groupTextBlock.Text = value;
                    this.groupTextBlockBorder = new Border();
                    this.groupTextBlockBorder.Margin = new Thickness(borderThickness.Left, borderThickness.Top, 0.0, 0.0);
                    this.groupTextBlockBorder.Child = groupTextBlock;
                    this.groupTextBlockBorder.Background = Brushes.BrushSet[Brushes.DefaultBrushId];
                    this.groupTextBlockBorder.Background.Opacity = .5;
                    this.groupTextBlockBorder.SetValue(Canvas.TopProperty, 0.0);
                    this.groupTextBlockBorder.SetValue(Canvas.LeftProperty, 0.0);
                    this.groupTextBlockBorder.SetValue(Canvas.ZIndexProperty, 1);
                    this.canvas.Children.Add(groupTextBlockBorder);


                }
            }
        }

        public FrameworkElement Element
        {
            get { return this.canvas; }
        }



        #endregion
    }

}