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
using Metodichka_AIS.Entities;

namespace Metodichka_AIS
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private is31nesinaContext _dbContext = new is31nesinaContext(); // Контекст БД
        private bool _isLogin = false; //Залогинились ли
        public static UsersM CurrentUser; //Текущий пользователь

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersM user = _dbContext.UsersMs.Where(
                    (usr) => usr.Login == loginTextBox.Text && usr.Password == passwordTextBox.Text
                    ).Single();
                MessageBox.Show($"Привет, {user.Login}!", "Успешно!");

                _isLogin = true;
                CurrentUser = user;
                Close(); //Закрываем окно
            }
            catch {
                MessageBox.Show("Ошибка!", "Неверный логин или пароль!");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!_isLogin)
                App.Current.Shutdown(); //Завершение работы приложения
        }
    }
}
