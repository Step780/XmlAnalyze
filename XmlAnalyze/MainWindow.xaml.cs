using ModernWpf;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace XmlAnalyze
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Frame MainFrameInstance;
        public string rr = "";
        public int theme = 0;
        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            MainFrameInstance = MainFrame;

            if (File.Exists("baseSave.xml"))
            {
                MainFrame.Navigate(new Main(rr, theme));


            }
            else
            {
                MainFrame.Navigate(new Base(theme));
            }



        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.IO.File.Delete("fromDesktop.xml");

        }
    }
}
