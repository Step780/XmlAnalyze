using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace XmlAnalyze
{
    /// <summary>
    /// Логика взаимодействия для SaveType.xaml
    /// </summary>
    public partial class SaveType : Window
    {
        public string txt = "";
        public string ph = "";

        public SaveType(string text, string path)
        {
            txt = text;
            ph = path;
            InitializeComponent();
        }

        private async void rewritingBtn_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(ph, false))
            {
                await writer.WriteLineAsync(txt);
            }
            
            this.Close();
        }

        private void newfileBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML документы(*.xml;)|*.xml;";

            if (saveFileDialog.ShowDialog() == true)
            {
                // получаем выбранный файл
                string filename = saveFileDialog.FileName;
                // сохраняем текст в файл
                System.IO.File.WriteAllText(filename, txt);
                
            }

            this.Close();

        }
    }
}
