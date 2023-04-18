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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace FifteenImage
{
    /// <summary>
    /// Логика взаимодействия для Regist.xaml
    /// </summary>
    public partial class Regist : Window
    {
        static public NpgsqlCommand com = null;        
        public Regist()
        {
            InitializeComponent();
        }

        //Обработчик события нажатия на кнопку "зарегистрироваться"
        private void Button_Regist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Avtoriz.con.Open();
                //Добавление нового пользователя в бд
                com = new NpgsqlCommand($"INSERT INTO Users (username, login, password_) VALUES ('{TextName.Text}', " +
                    $"'{TextLogin.Text}', '{TextPassword.Password}')", Avtoriz.con);
                if (TextName.Text == "")
                {
                    TextLogin.Background = Brushes.IndianRed;
                    MessageBox.Show("Введите имя пользователя");
                }
                else if (TextLogin.Text == "")
                {
                    TextLogin.Background = Brushes.IndianRed;
                    MessageBox.Show("Введите логин");
                }
                else if (TextPassword.Password == "")
                {
                    TextPassword.Background = Brushes.IndianRed;
                    MessageBox.Show("Введите пароль");
                }
                com.ExecuteNonQuery();
                Avtoriz.con.Close();
                MessageBox.Show("Регистрация успешно завершилась!");
                Avtoriz avt = new Avtoriz();
                avt.Show();
                Close();  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }

        //Обработчик кнопки выхода
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
