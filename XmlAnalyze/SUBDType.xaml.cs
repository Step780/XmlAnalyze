using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace XmlAnalyze
{
    
    public partial class SUBDType : Window
    {
        public int themenow = 0;
        public SUBDType(int theme)
        {
            InitializeComponent();
            themenow = theme;
            

            if (theme == 0)
            {
                this.Background = Brushes.White;

            }
            else
            {
                this.Background = Brushes.Black;

            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionClass connectionClass = new ConnectionClass("1");

            if (File.Exists("connectionType.xml"))
            {
                using (StreamWriter writer = new StreamWriter("connectionType.xml", false))
                {
                    await writer.WriteLineAsync("1");
                }
            }
            


                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionClass));

                using (FileStream fs = new FileStream("connectionType.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, connectionClass);

                }
            

            SUBDType type = new SUBDType(themenow);
            type.Close();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {


            ConnectionClass connectionClass = new ConnectionClass("2");


            if (File.Exists("connectionType.xml"))
            {

                using (StreamWriter writer = new StreamWriter("connectionType.xml", false))
                {
                    await writer.WriteLineAsync("2");
                }
            }

            

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionClass));

                using (FileStream fs = new FileStream("connectionType.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, connectionClass);

                }
            
            SUBDType type = new SUBDType(themenow);
            type.Close();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ConnectionClass connectionClass = new ConnectionClass("3");

            if (System.IO.File.Exists("connectionType.xml"))
            {
                using (StreamWriter writer = new StreamWriter("connectionType.xml", false))
                {
                    await writer.WriteLineAsync("3");
                }
            }

            
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionClass));

                using (FileStream fs = new FileStream("connectionType.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, connectionClass);

                }
            

            SUBDType type = new SUBDType(themenow);
            type.Close();

        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ConnectionClass connectionClass = new ConnectionClass("4");

            if (File.Exists("connectionType.xml"))
            {
                using (StreamWriter writer = new StreamWriter("connectionType.xml", false))
                {
                    await writer.WriteLineAsync("4");
                }
            }

            
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionClass));

                using (FileStream fs = new FileStream("connectionType.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, connectionClass);

                }
            

            SUBDType type = new SUBDType(themenow);
            type.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int theme = 0;
            MainWindow.MainFrameInstance.Navigate(new Base(theme));

        }
    }
}
