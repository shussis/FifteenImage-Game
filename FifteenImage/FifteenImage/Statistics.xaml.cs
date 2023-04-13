using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;
using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using LiveCharts.Charts;
using LiveCharts.Definitions.Charts;

namespace FifteenImage
{
    /// <summary>
    /// Логика взаимодействия для Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        static public NpgsqlCommand com = null;
        static public NpgsqlDataAdapter dataAdapter = null;
        //static public DataSet dataSet = null;
        static public DataTable table = null;
        public string username1;
        public Statistics()
        {
            InitializeComponent();
        }

        //Обработчик события кнопки возвращения на предыдущее окно
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переходим назад...");
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }

        //Обработчик события кнопки выхода
        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //При загрузке окна
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                username1 = Global.globalusername;
                //Выбор и вставка значений пользователя и его игр для личной статистики
                TextName.Text = username1;
                com = new NpgsqlCommand($"SELECT date_registration FROM Users WHERE Username = '{username1}'", Avtoriz.con);
                TextDateRegist .Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT total_games FROM Users WHERE Username = '{username1}'", Avtoriz.con);
                TextTotalGames.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT date_game FROM Game WHERE Username = '{username1}' ORDER BY date_game DESC LIMIT 1", Avtoriz.con);
                TextPoslGameDate.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT researched_states FROM Game WHERE Username = '{username1}' ORDER BY date_game DESC LIMIT 1", Avtoriz.con);
                TextIssled.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT moves FROM Game WHERE Username = '{username1}' ORDER BY date_game DESC LIMIT 1", Avtoriz.con);
                TextMoves.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT moves FROM Game WHERE Username = '{username1}'", Avtoriz.con);
                dataAdapter = new NpgsqlDataAdapter(com);
                //dataSet = new DataSet();                
                CartesianChart1.LegendLocation = LegendLocation.Bottom;

                //Построение графика статистики
                //dataSet.Tables["Game"]?.Clear();
                //if (dataSet.Tables["Game"] != null)
                //    dataSet.Tables["Game"].Clear();
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Game");
                table = dataSet.Tables["Game"];
                SeriesCollection series = new SeriesCollection();
                ChartValues<int> moveValues = new ChartValues<int>();
                List<string> dates = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    moveValues.Add(Convert.ToInt32(row["moves"]));
                    dates.Add(Convert.ToString(row["date_game"]));
                }
                CartesianChart1.AxisX.Clear();
                CartesianChart1.AxisX.Add(new Axis()
                {
                    Name = "Даты игр",
                    Labels = dates
                });
                LineSeries line = new LineSeries();
                line.Title = "'{username1}'";
                line.Values = moveValues;
                series.Add(line);
                CartesianChart1.Series = series;
                com.ExecuteNonQuery();
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
