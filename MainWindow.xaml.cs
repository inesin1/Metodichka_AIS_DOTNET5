using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metodichka_AIS.Entities;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace Metodichka_AIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private is31nesinaContext _dbContext = new is31nesinaContext(); // Контекст БД
        private string _currentTable; // Имя текущей таблицы

        public MainWindow()
        {
            //new LoginWindow().ShowDialog(); //Создаем окно авторизации и открываем его
            LoginWindow.CurrentUser = new UsersM { Id = 1, Login = "q", Password = "1", Role = null, RoleId = 1};
            InitializeComponent();

            CheckUser();

            //Устанавливаем текущей таблицей "Пользователи"
            _currentTable = "Пользователи"; 
            //Обновляем текущую таблицу
            RefreshTable(_currentTable);
        }

        /// <summary>
        /// Проверяет роль пользователя и задает параметры отображения элементов для каждого
        /// </summary>
        private void CheckUser() {
            switch (LoginWindow.CurrentUser.RoleId) {
                case 1: //Если директор
                    break;
                case 2: //Если продавец
                    //Отключаем видимость вкладки "Пользователи"
                    usersTab.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// Обновляет данные в переданной таблице
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        private void RefreshTable(string tableName) {
            switch (tableName) { //Проверяем пришедшую переменную tableName
                case "Пользователи": //В случае если имя таблицы - Пользователи
                    _dbContext.UsersMs.Load(); //Загружаем данные таблицы из БД
                    //Устанавливаем загруженные данные таблицы из БД как источник для
                    //вывода в DataGrid. Вспоминайте, DataGrid не хранит данные, он их
                    //только выводит
                    usersDG.ItemsSource = _dbContext.UsersMs.Local.ToObservableCollection();
                    break;
                case "Продажи": //В случае если имя таблицы - Пользователи
                    _dbContext.Sales.Load(); //Загружаем данные таблицы из БД
                    //Устанавливаем загруженные данные таблицы из БД как источник для
                    //вывода в DataGrid. Вспоминайте, DataGrid не хранит данные, он их
                    //только выводит
                    salesDG.ItemsSource = _dbContext.Sales.Local.ToObservableCollection();
                    break;
            }
        }

        private void usersDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Имя столбца
            string headerName = e.Column.Header.ToString(); 

            //Проверяем имя столбца
            switch (headerName) {
                case "Login":
                    //Меняем имя
                    e.Column.Header = "Логин";
                    break;
                case "Password":
                    e.Column.Header = "Пароль";
                    break;
                case "Role":
                    //Скрываем столбец
                    e.Column.Visibility = Visibility.Collapsed;

                    _dbContext.Roles.Load(); //Подгружаем данные из таблицы Roles

                    Binding binding = new Binding(); //Создаем новый биндинг для подвязки роли
                    binding.Path = new PropertyPath("RoleId"); //В путь подвязки указываем поле RoleId

                    //Создаем новый столбец типа ComboBox для 
                    //возможности выбора роли и настраиваем его
                    DataGridComboBoxColumn col = new DataGridComboBoxColumn { 
                        Header = "Роль", //Название столбца
                        DisplayMemberPath = "Name", //Отображаем именно поле Name, а не ID
                        SelectedValuePath = "Id", //А выбираем по ID
                        ItemsSource = _dbContext.Roles.ToArray(), //Подвязываем эти данные в выпадающий список выбора
                        SelectedValueBinding = binding //Устанавливаем созданный ранее биндинг к столбцу
                    };

                    ((DataGrid)sender).Columns.Add(col); //Добавляем созданный столбец в DataGrid
                    break;
                case "Sales":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "RoleId":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.SaveChanges();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentTable) {
                case "Пользователи":
                    _dbContext.UsersMs.Local.Remove(usersDG.SelectedItem as UsersM);
                    break;
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            //Получаем путь к файлу для экспорта
            string filePath = GetUserFile();

            //Если вернулся null (Ничего не выбрали), выходим из метода 
            if (filePath == null)
                return;

            //Открываем поток на запись
            StreamWriter file = new StreamWriter(filePath, false);

            //Проверяем какую таблицу будем экспортировать
            switch (_currentTable) {
                case "Пользователи":
                    //Сохраняем таблицу в коллекцию table для удобства
                    ObservableCollection<UsersM> table = _dbContext.UsersMs.Local.ToObservableCollection();

                    file.WriteLine($"ID;Логин;Пароль;РольИД"); //Записываем заголовки
                    //Проходим по всем элементам таблицы
                    foreach (UsersM elem in table)
                    {
                        //Записываем каждое поле элемента в файл
                        file.WriteLine($"{elem.Id};{elem.Login};{elem.Password};{elem.RoleId}");
                    }
                    break;
            }

            file.Close(); //Закрываем файл
            MessageBox.Show("Экспорт успешно завершен", "Успешно!");
        }

        /// <summary>
        /// Возвращает полный путь к файлу, выбранному пользователем
        /// </summary>
        /// <returns>Полный путь к файлу</returns>
        private string GetUserFile() { 
            //Создаем OpenFileDialog для выбора файла
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Файлы | *.csv"; //Фильтр на CSV файлы
            ofd.Title = "Выберите файл для экспорта"; 

            //Открываем его, и если выбрали файл, то возвращаем путь до него
            if (ofd.ShowDialog() == true) { 
                return ofd.FileName;
            }

            return null; //Иначе вернется null
        }

        private void usersTab_GotFocus(object sender, RoutedEventArgs e)
        {
            _currentTable = ((TabItem)sender).Header.ToString();
            RefreshTable(_currentTable);
        }
    }
}
