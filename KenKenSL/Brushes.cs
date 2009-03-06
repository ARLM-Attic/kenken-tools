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
    internal class Brushes
    {
        private Brushes() { }

        private static readonly Brush[] brushes = new Brush[]{
            new SolidColorBrush(Colors.Blue),   //0
            new SolidColorBrush(Colors.Red),    //1
            new SolidColorBrush(Colors.Green),  //2
            new SolidColorBrush(Colors.Purple), //3
            new SolidColorBrush(Colors.Orange), //4
            new SolidColorBrush(Colors.Brown),  //5
            new SolidColorBrush(Colors.Magenta),//6
            new SolidColorBrush(Colors.Cyan),   //7
            new SolidColorBrush(Colors.Black),  //8
            new SolidColorBrush(Colors.LightGray), //9
            new SolidColorBrush(Colors.White),  //10 not assigned
            new SolidColorBrush(Colors.Yellow)  //Active
        };

        public static Brush[] BrushSet
        {
            get { return brushes; }
        }

        public static int DefaultBrushId
        {
            get { return brushes.Length-2; }
        }

        public static int ActiveBrushId
        {
            get { return brushes.Length-1; }
        }

    }
}
