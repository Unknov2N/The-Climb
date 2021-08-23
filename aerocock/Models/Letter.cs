using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace aerocock.Models
{
    internal class Letter
    {
        public TextBlock textBlock;

        public Letter(Hook _hook, TextBlock _block)
        {
            coord = new Vector(_hook.coord.X, _hook.coord.Y);
            var fontsize = 40;
            var rd = new Random();
            SolidColorBrush[] colors =
            {
                Brushes.LightBlue, Brushes.LightYellow, Brushes.LightGreen, Brushes.Crimson,
                Brushes.Blue, Brushes.Yellow, Brushes.Green, Brushes.Red,
                Brushes.Aqua, Brushes.Coral
            };
            textBlock = new TextBlock
            {
                Text = _block.Text,
                //Margin = _block.Margin,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = fontsize,
                Margin = new Thickness(fontsize * coord.X / 1.5, fontsize * coord.Y / 1.5, 0, 0),
                Foreground = Brushes.White // colors[rd.Next(10)]
            };


            // letter = textBlock.Text[0];
        }

        public Letter(Grid _grid, List<Hook> _hooks, TextBlock _block, int _height)
        {
            while (true)
            {
                var rd = new Random();
                var hookRd = rd.Next(_hooks.Count);

                if (!_hooks[hookRd].isBusy)
                {
                    //Thread.Sleep(500);
                    _hooks[hookRd].isBusy = true;
                    coord = new Vector(_hooks[hookRd].coord.X, _hooks[hookRd].coord.Y);
                    //coord.i = _hooks[hookRd].coord.i;
                    //j = _hooks[hookRd].j;
                    var fontsize = 40;
                    //Random colorRd = new Random();
                    SolidColorBrush[] colors =
                    {
                        Brushes.LightBlue, Brushes.LightYellow, Brushes.LightGreen, Brushes.Crimson,
                        Brushes.Blue, Brushes.Yellow, Brushes.Green, Brushes.Red,
                        Brushes.Aqua, Brushes.Coral
                    };
                    textBlock = new TextBlock
                    {
                        Text = _block.Text,
                        //Margin = _block.Margin,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = fontsize,
                        Margin = new Thickness(_height / 12 * coord.X / 0.125, -_height / 12 * (coord.Y / 0.125 + 1), 0,
                            0), ///поменять всё на константы!!!!
                        Foreground = Brushes.White // colors[rd.Next(10)]
                    };
                    _grid.Children.Add(textBlock);
                    //Panel.SetZIndex(Start.)
                    break;
                }
            }
        }

        public Letter(double _x, double _y, char _c)
        {
            coord = new Vector(_x, _y);
            //i = _i;
            //j = _j;
            //letter = _c;
            textBlock = new TextBlock();
            textBlock.Text = _c.ToString();
            //textBlock.TextAlignment = TextAlignment.Center;
            //textBlock.VerticalAlignment = VerticalAlignment.Top;
            //textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            //textBlock.Visibility = Visibility.Visible;
            textBlock.Foreground = Brushes.Red;
            var fontsize = 40;
            textBlock.FontSize = fontsize;
            //textBlock.Margin = new Thickness(fontsize * (i - words[numWord].Length / 2), 0, 0, 0);
        }

        //public int i { get; set; }//x-индекс зацепа от -11 до 11 (включая 0)
        //public int j { get; set; }
        //public char letter;
        public Vector coord { get; set; }
    }
}