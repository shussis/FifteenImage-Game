using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FifteenImage
{
    
    public partial class MainWindow
    {
        Point _zeroPosition,_butPosition;
        List<Button> _buttons;
        IDA _ida;
        int[] _targetStat;

        //Сама сетка пятнашек представлена в виде массива кнопок
        public void Initial()
        {
            _buttons = new List<Button>(16);
            _targetStat = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            _ida = new IDA(_targetStat);
            for (int i = 0; i < 16; i++)
            {
                Button bt = new Button();
                bt.Click += button_Click;
                _buttons.Add(bt);
                grid1.Children.Add(bt);
            }
            PlaceCells(_targetStat);
            SetBackground();
            progressBar1.Value = CalcProgrValue();
         }

        //Изменение пустышки
        private void SetBackground() 
        {
            for (int i = 0; i < 16; i++)
            {
                ImageBrush ibr = new ImageBrush();
                ibr.ImageSource = helpRes.ImSource;
                ibr.Viewbox = new Rect(i % 4 * 0.25, i / 4 * 0.25, 0.25, 0.25);
                _buttons[i].Background = ibr;
            }
        }

        //Изменение текста в правом текстовом блоке
        private void OnChangeText(object sender, SearchEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (ThreadStart)delegate {
                Tblock.Text = e.Mes;
            }
              ,DispatcherPriority.Normal  );

        }
        //В какой момент показывать кнопку "показать решение"
        private void OnFinSolve(object sender, SearchEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (ThreadStart)delegate {
                    ShowSolve.Visibility = Visibility.Visible;
                }
              , DispatcherPriority.Normal);
        }

        //Алгоритм перемещения пятнашек по координатам
        private void button_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;
            _zeroPosition.X = (int)_buttons[15].GetValue(Grid.ColumnProperty) ;
            _zeroPosition.Y = (int)_buttons[15].GetValue(Grid.RowProperty) ;
            _butPosition.X = (int)but.GetValue(Grid.ColumnProperty);
            _butPosition.Y = (int)but.GetValue(Grid.RowProperty);
            if (Math.Abs(_butPosition.X - _zeroPosition.X) + Math.Abs(_butPosition.Y - _zeroPosition.Y) == 1)
            {
                _buttons[15].SetValue(Grid.RowProperty, (int)_butPosition.Y);
                _buttons[15].SetValue(Grid.ColumnProperty, (int)_butPosition.X);
                but.SetValue(Grid.RowProperty, (int)_zeroPosition.Y);
                but.SetValue(Grid.ColumnProperty, (int)_zeroPosition.X);
                int nullPos = (int)(_zeroPosition.Y * 4 + _zeroPosition.X);
                int clikPos = (int)(_butPosition.Y * 4 + _butPosition.X);
                _ida._start[nullPos] = _ida._start[clikPos];
                _ida._start[clikPos] = 0;
                progressBar1.Value = CalcProgrValue();
            }
            Count_pos++;
        }

        //Подсчёт ходов
        private int CalcProgrValue()
        {
            int val = 0;
            for (int i = 0; i < 16; i++)
            {
                if (_targetStat[i] == _ida._start[i]) val++;
            }
            return val;
        }
       private void PlaceCells(int[] val) 
        {
            for (int i = 0; i < val.Length;i++ )
            {
                _buttons[val[i]].SetValue(Grid.RowProperty, i / 4);
                _buttons[val[i]].SetValue(Grid.ColumnProperty, i % 4);
            }
        }

       private void btnSpinner_Loaded(object sender, RoutedEventArgs e)
       {
           DoubleAnimation dblAnim = new DoubleAnimation
           {
               From = 1.0,
               To = 0.0,
               AutoReverse = true,
               RepeatBehavior = RepeatBehavior.Forever
           };

           _buttons[15].BeginAnimation(OpacityProperty, dblAnim);
       }     
    }
}
