using ModernWpf;
using MySqlConnector;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace XmlAnalyze
{

    public partial class Base : Page
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        int themenow = 0;
        public Base(int theme)
        {
            InitializeComponent();
            themenow = theme;
            enterBtn.IsEnabled = false;

            baseBox.SelectedIndex = -1;
            tableBox.SelectedIndex = -1;
            if (theme == 0)
            {
                BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/brightness.png", UriKind.Relative));
                themeImg.Source = image;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/sleep-mode-light.png", UriKind.Relative));
                themeImg.Source = image;

            }

            if (File.Exists("baseSave.xml"))
            {
                XmlSerializer reader = new XmlSerializer(typeof(BaseSaving));
                StreamReader file = new StreamReader(@"C:\Users\step4\source\repos\XmlAnalyze\XmlAnalyze\bin\Debug\baseSave.xml");
                BaseSaving baseSaving = (BaseSaving)reader.Deserialize(file);
                file.Close();

                serverBox.Text = baseSaving.server;
                loginBox.Text = baseSaving.login;
                passwordBox.Password = baseSaving.password;

            }

            if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
            {
                convertBtn.IsEnabled = false;
            }
            else
            {
                convertBtn.IsEnabled = true;

            }

            if (File.Exists("connectionType.xml"))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionClass));

                using (FileStream fs = new FileStream("connectionType.xml", FileMode.OpenOrCreate))
                {
                    ConnectionClass connectionType = xmlSerializer.Deserialize(fs) as ConnectionClass;
                    type = connectionType.ConnectionString;

                    switch (type)
                    {
                        case "1":
                            subdTxt.Text += " Microsoft SQL Server";
                            return;
                        case "2":
                            subdTxt.Text += " Postgre SQL";
                            serverBox.IsEnabled = false;
                            checkTxt.Visibility = Visibility.Hidden;
                            radioYes.Visibility = Visibility.Hidden;
                            radioNo.Visibility = Visibility.Hidden;
                            return;
                        case "4":
                            subdTxt.Text += " MySQL";
                            serverBox.IsEnabled = false;
                            checkTxt.Visibility = Visibility.Hidden;
                            radioYes.Visibility = Visibility.Hidden;
                            radioNo.Visibility = Visibility.Hidden;
                            return;
                    }

                }
            }
            else
            {
                SUBDType type = new SUBDType(themenow);
                type.Show();
            }
        }


        string catalog = "";
        string resultBase = "";
        string type = "";


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainFrameInstance.Navigate(new Main(resultBase, themenow));
        }

        //Подключение к серверу
        [Obsolete]
        private async void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "";
            List<string> nameBase = new List<string>();

            switch (type)
            {
                //MS SQL
                case "1":
                    if (radioYes.IsChecked == true)
                    {
                        connectionString = string.Format($"Data Source={serverBox.Text};Initial Catalog=master; Integrated Security=True;");
                        enterBtn.IsEnabled = true;
                        try
                        {
                            catchTxt.Text = "";

                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                SqlCommand sqlCommand = new SqlCommand($"select * from sys.databases", connection);

                                SqlDataReader reader = sqlCommand.ExecuteReader();

                                while (reader.Read())
                                {
                                    string l = (String.Format("{0}", reader[0]));
                                    nameBase.Add(l);
                                }

                                baseBox.ItemsSource = nameBase;

                                BaseSaving baseSaving = new BaseSaving()
                                {
                                    server = serverBox.Text,
                                    login = "",
                                    password = ""

                                };


                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseSaving));

                                using (FileStream fs = new FileStream("baseSave.xml", FileMode.OpenOrCreate))
                                {
                                    xmlSerializer.Serialize(fs, baseSaving);

                                }
                            }
                        }

                        catch (Exception msg)
                        {
                            logger.Error(msg);
                            catchTxt.Text = "Название сервера указано неправильно!";
                        }
                    }
                    if (radioNo.IsChecked == true)
                    {
                        connectionString = string.Format($"Data Source={serverBox.Text};Initial Catalog=master; User Id={loginBox.Text}; Password={passwordBox.Password} Trusted Connection=True;");
                        enterBtn.IsEnabled = true;

                        try
                        {
                            catchTxt.Text = "";

                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                SqlDataAdapter adapter = new SqlDataAdapter($"select * from sys.databases", connection);

                                var table = new DataTable();
                                adapter.Fill(table);

                                baseBox.ItemsSource = table.DefaultView;

                                catchTxt.Text = "";

                                baseBox.ItemsSource = nameBase;

                                BaseSaving baseSaving = new BaseSaving()
                                {
                                    server = serverBox.Text,
                                    login = loginBox.Text,
                                    password = passwordBox.Password

                                };


                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseSaving));

                                using (FileStream fs = new FileStream("baseSave.xml", FileMode.OpenOrCreate))
                                {
                                    xmlSerializer.Serialize(fs, baseSaving);

                                }

                            }
                        }

                        catch (Exception msg)
                        {
                            logger.Error(msg);
                            catchTxt.Text = "Неправильные данные пользователя!";

                        }
                    }
                    return;

                //PostgreSQL
                case "2":

                    try
                    {

                        connectionString = string.Format($"Host=localhost;Port=5432;Database=postgres;Username={loginBox.Text};Password={passwordBox.Password}");


                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();

                            NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT datname FROM pg_database;", connection);

                            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                nameBase.Add(l);
                            }

                            baseBox.ItemsSource = nameBase;

                            catchTxt.Text = "";

                            baseBox.ItemsSource = nameBase;

                            BaseSaving baseSaving = new BaseSaving()
                            {
                                server = "",
                                login = loginBox.Text,
                                password = passwordBox.Password

                            };


                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseSaving));

                            using (FileStream fs = new FileStream("baseSave.xml", FileMode.OpenOrCreate))
                            {
                                xmlSerializer.Serialize(fs, baseSaving);

                            }

                        }
                    }
                    catch (Exception msg)
                    {
                        logger.Error(msg);


                        catchTxt.Text = "Неправильные данные пользователя!";
                    }
                    return;


                //MySQL
                case "4":
                    connectionString = string.Format($"Server=localhost;Port=3306;Database=sys;Uid={loginBox.Text};Pwd={passwordBox.Password};");

                    try
                    {

                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            MySqlCommand sqlCommand = new MySqlCommand($"show databases", connection);

                            MySqlDataReader reader = sqlCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                nameBase.Add(l);
                            }

                            baseBox.ItemsSource = nameBase;
                            catchTxt.Text = "";

                            baseBox.ItemsSource = nameBase;

                            BaseSaving baseSaving = new BaseSaving()
                            {
                                server = "",
                                login = loginBox.Text,
                                password = passwordBox.Password

                            };


                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseSaving));

                            using (FileStream fs = new FileStream("baseSave.xml", FileMode.OpenOrCreate))
                            {
                                xmlSerializer.Serialize(fs, baseSaving);

                            }
                        }
                    }
                    catch (Exception msg)
                    {
                        logger.Error(msg);


                        catchTxt.Text = "Неправильные данные пользователя!";

                    }
                    return;

            }

            if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
            {
                convertBtn.IsEnabled = false;
            }
            else
            {
                convertBtn.IsEnabled = true;

            }




        }


        //Выбор базы данных
        private async void baseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> nameTable = new List<string>();
            string connectionString = "";
            tableBox.IsEnabled = false;

            switch (type)
            {
                //MS SQL
                case "1":
                    connectionString = string.Format($"Data Source={serverBox.Text};Initial Catalog=master; Integrated Security=True;");

                    try
                    {

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            SqlCommand adapter = new SqlCommand($"select TABLE_NAME from {baseBox.SelectedValue}.information_schema.tables", connection);


                            SqlDataReader reader = adapter.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                nameTable.Add(l);
                            }

                            tableBox.IsEnabled = true;

                            catalog = baseBox.SelectedValue.ToString();
                            tableBox.ItemsSource = nameTable;

                        }
                    }

                    catch (Exception msg)
                    {
                        logger.Error(msg);


                        catchTxt.Text = "Что-то пошло не так";
                    }
                    return;
                //PostgreSQL
                case "2":

                    connectionString = string.Format($"Host=localhost;Port=5432;Database={baseBox.SelectedValue};Username=postgres;Password=100kfnbyf100");

                    try
                    {

                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();


                            NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema', 'pg_toast', 'pgagent')", connection);

                            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                nameTable.Add(l);
                            }
                            tableBox.IsEnabled = true;

                            tableBox.ItemsSource = nameTable;

                        }
                    }

                    catch (Exception msg)
                    {
                        logger.Error(msg);


                        catchTxt.Text = "Что-то пошло не так";

                    }
                    return;
                //MySQL
                case "4":
                    connectionString = string.Format($"Server=localhost;Port=3306;Database={baseBox.SelectedValue};Uid=root;Pwd=100kfnbyf100;");

                    try
                    {

                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            await connection.OpenAsync();

                            MySqlCommand sqlCommand = new MySqlCommand($"show tables from {baseBox.SelectedValue}", connection);

                            MySqlDataReader reader = sqlCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                nameTable.Add(l);
                            }
                            tableBox.IsEnabled = true;

                            tableBox.ItemsSource = nameTable;

                        }
                    }

                    catch (Exception msg)
                    {
                        logger.Error(msg);


                        catchTxt.Text = "Что-то пошло не так";
                    }
                    return;
            }


            if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
            {
                convertBtn.IsEnabled = false;
            }
            else
            {
                convertBtn.IsEnabled = true;

            }
        }


        //Выбор таблицы
        private async void tableBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string connectionString = "";

            switch (type)
            {
                //MS SQL
                case "1":
                    connectionString = string.Format($"Data Source={serverBox.Text};Initial Catalog={catalog}; Integrated Security=True;");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        SqlDataAdapter adapter = new SqlDataAdapter($"select * from [dbo].{tableBox.SelectedValue}", connection);

                        var table = new DataTable();
                        adapter.Fill(table);
                        table.TableName = tableBox.SelectedValue.ToString();

                        string result;
                        using (StringWriter sw = new StringWriter())
                        {
                            table.WriteXml(sw);
                            result = sw.ToString();
                            resultBase = result;
                        }


                    }

                    previewBox.Text = resultBase;


                    if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
                    {
                        convertBtn.IsEnabled = false;
                    }
                    else
                    {
                        convertBtn.IsEnabled = true;

                    }
                    return;

                //PostgreSQL
                case "2":

                    connectionString = string.Format($"Host=localhost;Port=5432;Database={baseBox.SelectedValue};Username=postgres;Password=100kfnbyf100");

                    try
                    {

                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();

                            NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT table_to_xml('{tableBox.SelectedValue}', true, false, '')", connection);

                            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();

                            while (reader.Read())
                            {
                                string l = (String.Format("{0}", reader[0]));
                                resultBase += l;
                            }

                        }

                        previewBox.Text = resultBase;

                    }

                    catch (Exception msg)
                    {
                        logger.Error(msg);


                    }

                    if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
                    {
                        convertBtn.IsEnabled = false;
                    }
                    else
                    {
                        convertBtn.IsEnabled = true;

                    }
                    return;
                //MySQL
































                case "4":
                    connectionString = string.Format($"Server=localhost;Port=3306;Database={baseBox.SelectedValue};Uid=root;Pwd=100kfnbyf100;");

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        MySqlDataAdapter adapter = new MySqlDataAdapter($"select * from {tableBox.SelectedValue}", connection);

                        var table = new DataTable();
                        adapter.Fill(table);
                        table.TableName = tableBox.SelectedValue.ToString();

                        string result;
                        using (StringWriter sw = new StringWriter())
                        {
                            table.WriteXml(sw);
                            result = sw.ToString();
                            resultBase = result;
                        }

                    }

                    previewBox.Text = resultBase;

                    if (baseBox.SelectedIndex == -1 || tableBox.SelectedIndex == -1)
                    {
                        convertBtn.IsEnabled = false;
                    }
                    else
                    {
                        convertBtn.IsEnabled = true;

                    }
                    return;
            }




        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            loginBox.IsEnabled = true;
            passwordBox.IsEnabled = true;

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            loginBox.IsEnabled = false;
            passwordBox.IsEnabled = false;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string rr = "";
            MainWindow.MainFrameInstance.Navigate(new Main(rr, themenow));

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenWindow();
        }

        private void themeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            themenow = 0;
            BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/brightness.png", UriKind.Relative));
            themeImg.Source = image;

        }

        private void themeButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            themenow = 1;
            BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/sleep-mode-light.png", UriKind.Relative));
            themeImg.Source = image;

        }

        private void OpenWindow()
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is SUBDType)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                SUBDType newwindow = new SUBDType(themenow);
                newwindow.Show();
            }
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (type == "1")
            {
                if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                    {
                        enterBtn.IsEnabled = true;
                    }

                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }

        }

        private void loginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (type == "1")
            {
                if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                    {
                        enterBtn.IsEnabled = true;
                    }

                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }


        }

        private void passwordBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (type == "1")
            {
                if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                    {
                        enterBtn.IsEnabled = true;
                    }

                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }
        }

        private void serverBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (type == "1")
            {
                if (radioYes.IsChecked == true)
                {
                    if (serverBox.Text != "")
                    {
                        enterBtn.IsEnabled = true;

                    }
                }

                else
                {
                    if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                    {
                        enterBtn.IsEnabled = false;
                    }
                    else
                    {


                        if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                        {
                            enterBtn.IsEnabled = true;
                        }

                    }
                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }
        }

        private void serverBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (type == "1")
            {
                if (radioYes.IsChecked == true)
                {
                    if (serverBox.Text != "")
                    {
                        enterBtn.IsEnabled = true;

                    }
                }

                else
                {
                    if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                    {
                        enterBtn.IsEnabled = false;
                    }
                    else
                    {


                        if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                        {
                            enterBtn.IsEnabled = true;
                        }

                    }
                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }
        }

        private void loginBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (type == "1")
            {
                if (loginBox.Text == "" || passwordBox.Password == "" || serverBox.Text == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    if ((radioYes.IsChecked == true) || (radioNo.IsChecked == true))
                    {
                        enterBtn.IsEnabled = true;
                    }

                }
            }

            else
            {
                if (loginBox.Text == "" || passwordBox.Password == "")
                {
                    enterBtn.IsEnabled = false;
                }
                else
                {
                    enterBtn.IsEnabled = true;
                }
            }
        }
    }
}
