
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FifteenImage
{
    public class Res : INotifyPropertyChanged
    {
        public string Sfn = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.FullName}\Images\1.jpg";
        public  event PropertyChangedEventHandler PropertyChanged;
        private  ImageSource _imsource;

        //Выгрузка изображения
        public ImageSource ImSource
        {
            get
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();
                _imsource = (ImageSource)imgConv.ConvertFromString(Sfn);
                return _imsource;
            }
            set { _imsource = value; }
        }

        //Начало новой игры
        public string SourceFileName
        {
             get { return Sfn; }
             set 
             {
                 Sfn = value;
                 var imgConv = new ImageSourceConverter();
                 _imsource = (ImageSource)imgConv.ConvertFromString(value);
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceFileName"));
             }
        }
    }

    //Класс конвертирующий значения
   public class DConverter : IMultiValueConverter
   {
       public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
       {
           int par1 = System.Convert.ToInt32(value[0]);
           int par2 = System.Convert.ToInt32(value[1]);
           return $"{par1 * 0.25:0.00},{(par2 - 1) * 0.25:0.00},0.25,0.25";
       }

       public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
       {
           return null;
       }
   }

    //Класс реализации команд
   public class DataCommands
   {
       private static RoutedUICommand escape;
       private static RoutedUICommand newGame;
       private static RoutedUICommand mix;
       private static RoutedUICommand solve;
       private static RoutedUICommand showSolution;
       static DataCommands()
       {
           //// Инициализация команды
           InputGestureCollection inputs = new InputGestureCollection();
           inputs.Add(new KeyGesture(Key.Escape, ModifierKeys.None, "Esc"));
           escape = new RoutedUICommand(
             "Escape", "Escape", typeof(DataCommands), inputs);

           InputGestureCollection inputs2 = new InputGestureCollection
           {
               new KeyGesture(Key.N, ModifierKeys.Control, "Ctr+N")
           };
           newGame = new RoutedUICommand(
             "NewGame", "NewGame", typeof(DataCommands), inputs2);

           InputGestureCollection inputs3 = new InputGestureCollection
           {
               new KeyGesture(Key.M, ModifierKeys.Control, "Ctr+M")
           };
           mix = new RoutedUICommand(
             "Mix", "Mix", typeof(DataCommands), inputs3);

           InputGestureCollection inputs4 = new InputGestureCollection
           {
               new KeyGesture(Key.S, ModifierKeys.Control, "Ctr+S")
           };
           solve = new RoutedUICommand(
             "Solve", "Solve", typeof(DataCommands), inputs4);

           InputGestureCollection inputs6 = new InputGestureCollection {new KeyGesture(Key.P, ModifierKeys.Control)};
           showSolution = new RoutedUICommand(
             "ShowSolution", "ShowSolution", typeof(DataCommands), inputs6);
       }
       public static RoutedUICommand Escape
       {
           get { return escape; }
       }
       public static RoutedUICommand NewGame => newGame;

       public static RoutedUICommand Mix
       {
           get { return mix; }
       }
       public static RoutedUICommand Solve
       {
           get { return solve; }
       }

       public static RoutedUICommand ShowSolution
       {
           get { return showSolution; }
       }
   }
}
