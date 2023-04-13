using Microsoft.Win32;
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
    /// Логика взаимодействия для Avtoriz.xaml
    /// </summary>
    public partial class Avtoriz : Window
    {

        //Создание переменных для подключения и выполнения команд в бд
        static public NpgsqlCommand com = new NpgsqlCommand();
        static public NpgsqlConnection con = new NpgsqlConnection(@"Server=localhost; Port=5432; User Id=postgres; Password=Gfhjkm137!; Database = Fifteen;");
        static public NpgsqlDataReader reader = null;

        //Глобальная переменная, значение которой возьмётся из этого окна
        public String GlobalUsername { get; set; }

        public Avtoriz()
        {
            InitializeComponent();
            com.Connection = con;
            
        }

        //Обработчик события нажатия на кнопку входа
        private void Button_Vhod_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                bool ch = false;
                con.Open();
                //Выбор имени из бд в таблице пользователей
                //Проверка совпадения логина и пароля для входа
                com.CommandText = $"SELECT Username FROM Users WHERE Login='{TextLogin.Text}' and Password_='{TextPassword.Password}'";
                Global.globalusername = Convert.ToString (com.ExecuteScalar());
                if (TextLogin.Text == "")
                    {
                    TextLogin.Background = Brushes.IndianRed;
                    MessageBox.Show("Введите логин");
                    }
                else if (TextPassword.Password == "")
                    {
                    TextPassword.Background = Brushes.IndianRed;
                    MessageBox.Show("Введите пароль");
                    }
                reader = com.ExecuteReader();
                while (reader.Read()) { ch = true; }
                con.Close();
                if (ch == true)
                    {
                    MessageBox.Show("Добро пожаловать в игру Пятнашки !!");
                    MainWindow data = new MainWindow();
                    data.Show();
                    Close();
                    }
                else if (ch == false)
                    {
                    MessageBox.Show("Данные введены неверно");
                    }
                Avtoriz.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.Message);
                Avtoriz.con.Close();
            }
        }
        //Обработчик события нажатия на кнопку регистрации
        private void Button_Regist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переходим на окно регистрации");
            Regist reg = new Regist();
            reg.Show();
            Close();
        }
        //Обработчик события нажатия на кнопку выхода
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
