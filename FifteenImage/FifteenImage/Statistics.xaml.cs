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
using System.Data;

namespace FifteenImage
{
    /// <summary>
    /// Логика взаимодействия для Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        static public NpgsqlCommand com = null;
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
                //Выбор и вставка значений пользователя и его игр для личной статистики
                com = new NpgsqlCommand($"SELECT username FROM Users WHERE UserId = '{Global.globalid}'", Avtoriz.con);
                TextName.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT date_registration FROM Users WHERE UserId = '{Global.globalid}'", Avtoriz.con);
                TextDateRegist .Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT total_games FROM Users WHERE UserId = '{Global.globalid}'", Avtoriz.con);
                TextTotalGames.Text = Convert.ToString(com.ExecuteScalar());
                com = new NpgsqlCommand($"SELECT date_game FROM Game WHERE UserId = '{Global.globalid}' ORDER BY date_game DESC LIMIT 1", Avtoriz.con);
                TextPoslGameDate.Text = Convert.ToString(com.ExecuteScalar());
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
