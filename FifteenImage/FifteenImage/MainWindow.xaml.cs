using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Npgsql;

namespace FifteenImage
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public NpgsqlCommand com = null;
        static public NpgsqlDataAdapter dataAdapter = null;

        DispatcherTimer timer1 ;
        Res helpRes;
        public string info;

        public event PropertyChangedEventHandler PropertyChanged;
        //Позиция текущего отображения
        int count_pos;

        //Высчитывает позицию
        public int Count_pos
        {
            get { return count_pos; }
            set
            {
                count_pos = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count_pos"));
            }
        }

        //Главный метод окна, инициализирующий высчитывание позиций и времени
        public MainWindow()
        {
            InitializeComponent();
            helpRes = new Res();
            helpRes.PropertyChanged += onPropertyChanged;
            PropertyChanged += onCount_pos_Changed;
            Initial();
            timer1 = new DispatcherTimer();
            timer1.Tick += timer1_Tick;
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _buttons[15].Loaded += btnSpinner_Loaded;
          
        }

        //Метод, который перемешивает пятнашки
        public int[] Started()
        {
            List<int> ar;
            List<int> buf_ar;
            do
            {
                ar = new List<int>();
                Random rand = new Random();
                int buf = rand.Next(0, 16);
                buf_ar = new List<int>(new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
                while (buf_ar.Count != 0)
                {

                    if (!buf_ar.Contains(buf))
                    {
                        buf = rand.Next(0, 16);
                    }
                    else
                    {
                        ar.Add(buf);
                        buf_ar.Remove(buf);
                    }

                }

            } while (!IDA.DoHaveResolve(ar.ToArray()));
            return ar.ToArray();
        }

        //Метод, который говорит "остановиться" высчитывать результат игры в текстовом блоке справа окна
        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (ThreadStart)delegate {
                    Tblock.Text = IDA.DoStop();
                }
              , DispatcherPriority.Normal);
        }

        //При загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Очищаем все поля
            _ida.AlgFinished += OnFinSolve;
            _ida.AlgProgressing += OnChangeText;
            _ida.AlgStarted += OnChangeText;
            try
            {
                Avtoriz.con.Open(); 
                //Обновляем дату пользователя в бд последнего захода в приложение
                com = new NpgsqlCommand($"UPDATE Users SET last_login_date = CURRENT_DATE WHERE UserID = '{Global.globalid}' ", Avtoriz.con);
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        // Обработчик события, которое срабатывает при изменении свойства
        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetBackground();
        }

        // Обработчик события, которое срабатывает при изменении свойства Count_pos
        private void onCount_pos_Changed(object sender, PropertyChangedEventArgs e)
        {
            label1.Content = String.Format("Текущий ход:      {0}", count_pos);
        }

        //Высчитывание времени
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (count_pos < IDA.ArResult.Length)
            {
                PlaceCells(IDA.ArResult[count_pos]);
                _ida._start = IDA.ArResult[count_pos];
                progressBar1.Value = CalcProgrValue();
                Count_pos++;
            }
            else
            {
                timer1.Stop();
            }
        }

        //Обработчик команды для создания новой игры
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaceCells(_targetStat);
            SetBackground();
            progressBar1.Value = CalcProgrValue();
            Count_pos = 0;
        }

        //Обработчик команды для выхода из приложения
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        //Обработчик команды для перемешивания пятнашек
        private void MixCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ida._start = Started();
            ShowSolve.Visibility = Visibility.Hidden;
            PlaceCells(_ida._start);
            progressBar1.Value = CalcProgrValue();
            Count_pos = 0;
            Tblock.Text = "";
        }

        //Обработчик команды нажатия на кнопку решения в верхнем меню
        private void SolveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ = Dispatcher.BeginInvoke(
                (ThreadStart)delegate
                {
                    //Запуск метода решения пятнашек
                    _ida.DoStart();
                }
              , DispatcherPriority.Normal);
            try
            {
                Avtoriz.con.Open();
                //Прибавление к записи количества всех игр в бд при запуске игры
                com = new NpgsqlCommand($"UPDATE Users SET total_games = 1 + total_games WHERE UserID = '{Global.globalid}' ", Avtoriz.con);
                com.ExecuteNonQuery();
                //Добавление новой игры в бд
                com = new NpgsqlCommand($"INSERT INTO Game (userid, moves, researched_states) VALUES ('{Global.globalid}','{Global.globalmoves}','{Global.globalCountStates}') ", Avtoriz.con);
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        //Обработчик команды нажатия на кнопку показа решения
        private void ShowSolutionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Count_pos = 0;
            timer1.Start();
        }

        //Обработчик нажатия на кнопку показа правил в верхнем меню
        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                com = new NpgsqlCommand($"SELECT text_info FROM Information WHERE Informationid=1", Avtoriz.con);
                info = Convert.ToString(com.ExecuteScalar());
                MessageBox.Show(info);
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void progressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        //Обработчик кнопки перехода на окно статистики
        private void Button_Stat_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переходим на окно статистики...");
            Statistics stat = new Statistics();
            stat.Show();
            Close();
        }

        //Ниже представлено несколько бработчиков выбранного значения из Combo_box
        private void Info1_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                com = new NpgsqlCommand($"SELECT text_info FROM Information WHERE Informationid=1", Avtoriz.con);
                Tblock.Text = Convert.ToString(com.ExecuteScalar());
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        private void Info2_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                com = new NpgsqlCommand($"SELECT text_info FROM Information WHERE Informationid=2", Avtoriz.con);
                Tblock.Text = Convert.ToString(com.ExecuteScalar());
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        private void Info3_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                com = new NpgsqlCommand($"SELECT text_info FROM Information WHERE Informationid=3", Avtoriz.con);
                Tblock.Text = Convert.ToString(com.ExecuteScalar());
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        private void Info4_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                com = new NpgsqlCommand($"SELECT text_info FROM Information WHERE Informationid=4", Avtoriz.con);
                Tblock.Text = Convert.ToString(com.ExecuteScalar());
                com.ExecuteNonQuery();
                dataAdapter = new NpgsqlDataAdapter(com);
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }
    }
}
