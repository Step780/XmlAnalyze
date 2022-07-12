using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using ModernWpf;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace XmlAnalyze
{
    public partial class Main : Page
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        List<TextLines> list3 = new List<TextLines>();

        List<int> changes = new List<int>();
        List<int> linesMore = new List<int>();

        List<int> changesNull = new List<int>();
        public string path = "";
        public int nextLine = -1;
        public int moreLine = -2;
        public int nullLine = -1;
        public int nullLine2 = -1;
        public int moreLine2 = -1;
        public int moreLine3 = -1;
        public string selecteddFile = "";
        List<TextLines> linesList = new List<TextLines>();
        List<TextLines> linesBase = new List<TextLines>();

        public Main(string resultBase, int theme)
        {
            InitializeComponent();
            fromBase.Document.Text = resultBase;


            deskToJsonBtn.IsEnabled = false;
            if (resultBase == "")
            {
                baseToJsonBtn.IsEnabled = false;

            }
            else
            {
                baseToJsonBtn.IsEnabled = true;

            }

            fromDesktop.FontSize = 15;
            fromDesktop.TextArea.Options.HighlightCurrentLine = true;
            fromDesktop.TextArea.Options.HideCursorWhileTyping = true;

            fromBase.FontSize = 15;
            fromBase.TextArea.Options.HighlightCurrentLine = true;
            fromBase.TextArea.Options.HideCursorWhileTyping = true;


            linesCountBase.Text = "Количество строк: " + fromBase.LineCount.ToString();

            if (theme == 0)
            {
                BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/brightness.png", UriKind.Relative));
                themeImg.Source = image;
                var brush = new SolidColorBrush(Color.FromRgb(236, 236, 236));
                fromDesktop.Background = brush;
                fromBase.Background = brush;
                fromDesktop.Foreground = Brushes.Black;
                fromBase.Foreground = Brushes.Black;
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/sleep-mode-light.png", UriKind.Relative));
                themeImg.Source = image;
                fromDesktop.Background = Brushes.Black;
                fromBase.Background = Brushes.Black;
                fromDesktop.Foreground = Brushes.White;
                fromBase.Foreground = Brushes.White;
            }

            if (File.Exists("fromDesktop.xml"))
            {
                using (StreamReader reader = new StreamReader("fromDesktop.xml"))
                {
                    fromDesktop.Text = reader.ReadToEnd();
                }

            }

            File.Delete("fromDesktop.xml");

            sliyanie.IsEnabled = false;
            btnSave.IsEnabled = false;
            sliyanieTxt.Opacity = 0.20;
            sliyanieImg.Opacity = 0.20;
            btnGoLine.IsEnabled = false;
            imgGo.Opacity = 0.20;
            saveImg.Opacity = 0.20;
            txtGo.Opacity = 0.20;
            saveTxt.Opacity = 0.20;

            if (fromBase.Document.Text != "" && fromDesktop.Document.Text != "")
            {
                comapreBtn.IsEnabled = true;
            }
            else
            {
                comapreBtn.IsEnabled = false;

            }
        }


        public class TextLines
        {
            public int Number { get; set; }
            public string Line { get; set; }
        }

        public class FileUpload
        {
            public string FilePath { get; set; }
        }

        //Добавление XML файла с компьютера
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            warningBox.Text = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML документы(*.xml;)|*.xml;";

            if (openFileDialog.ShowDialog() == true)
            {
                string phrase = File.ReadAllText(openFileDialog.FileName);

                path = openFileDialog.FileName;

                fromDesktop.Text = phrase;

                linesCountDesktop.Text = "";

                linesCountDesktop.Text += "Количество строк: " + fromDesktop.LineCount;


            };


            using (StreamWriter writer = new StreamWriter("fromDesktop.xml", false))
            {
                await writer.WriteLineAsync(fromDesktop.Text);
            }

            if (fromBase.Document.Text == "" || fromDesktop.Document.Text == "")
            {
                comapreBtn.IsEnabled = false;
                logger.Debug("log {0}", this.comapreBtn);

            }
            else
            {
                comapreBtn.IsEnabled = true;
                logger.Debug("log {0}", this.comapreBtn);

            }

            logger.Debug("log {0}", this.fromBase);
            logger.Debug("log {0}", this.fromDesktop);

            deskToJsonBtn.IsEnabled = true;

        }

        //Переход в окно подключения к серверу
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int theme = 0;
            warningBox.Text = "";


            if (themeButton.IsChecked == true)
            {
                theme = 1;
            }
            else
            {
                theme = 0;
            }

            MainWindow.MainFrameInstance.Navigate(new Base(theme));

            if (fromBase.Document.Text == "" || fromDesktop.Document.Text == "")
            {
                comapreBtn.IsEnabled = false;
            }
            else
            {
                comapreBtn.IsEnabled = true;
            }
        }

        //Метод для сравнения XML файлов
        private void comapreBtn_Click(object sender, RoutedEventArgs e)
        {
            warningBox.Text = "";
            list3.Clear();
            changes.Clear();
            changesNull.Clear();
            linesMore.Clear();
            try
            {
                GetLinesCollectionFromTextBox(fromDesktop);
                GetLinesFromBase(fromBase);

                if (linesList.Count >= linesBase.Count)
                {
                    for (int i = 0; i <= linesList.Count; i++)
                    {
                        try
                        {

                            if (linesBase[i].Line != linesList[i].Line)
                            {
                                if (linesList[i].Line == "  " || linesList[i].Line == "" || linesList[i].Line == " ")
                                {
                                    changesNull.Add(linesList[i].Number);
                                }

                                if (linesBase[i].Line == "  " || linesBase[i].Line == "" || linesBase[i].Line == " ")
                                {
                                    changesNull.Add(linesBase[i].Number);
                                }

                                else
                                {
                                    changes.Add(linesList[i].Number);
                                }


                            }


                        }
                        catch (Exception msg)
                        {
                            logger.Error(msg);
                        }


                    }
                }

                else
                {
                    for (int i = 0; i <= linesBase.Count; i++)
                    {
                        try
                        {

                            if (linesList[i].Line != linesBase[i].Line)
                            {
                                if (linesBase[i].Line == "  " || linesBase[i].Line == "" || linesBase[i].Line == " ")
                                {
                                    changesNull.Add(linesBase[i].Number);
                                }

                                if (linesList[i].Line == "  " || linesList[i].Line == "" || linesList[i].Line == " ")
                                {
                                    changesNull.Add(linesList[i].Number);
                                }

                                else
                                {
                                    changes.Add(linesBase[i].Number);
                                }


                            }


                        }
                        catch (Exception msg)
                        {
                            logger.Error(msg);
                        }


                    }
                }


                if (linesList.Count > linesBase.Count)
                {
                    list3.AddRange(linesList.ToArray());
                    list3.RemoveAll(el => linesBase.Exists(el2 => el2.Number == el.Number));

                    for (int ii = 0; ii < list3.Count; ii++)
                    {
                        linesMore.Add(list3[ii].Number);
                    }

                }

                if (linesBase.Count > linesList.Count)
                {
                    list3.AddRange(linesBase.ToArray());

                    list3.RemoveAll(el => linesList.Exists(el2 => el2.Number == el.Number));

                    for (int ii = 0; ii < list3.Count; ii++)
                    {
                        linesMore.Add(list3[ii].Number);
                    }
                }

                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizer(changes));
                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerDesktopMore(linesMore));

                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNull(changesNull));

                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerBase(changes));
                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNull(changesNull));
                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerDesktopMore(linesMore));

                sliyanie.IsEnabled = true;
                sliyanieTxt.Opacity = 1;
                sliyanieImg.Opacity = 1;
                btnGoLine.IsEnabled = true;
                txtGo.Opacity = 1;
                imgGo.Opacity = 1;

                logger.Debug("log {0}", fromDesktop);
            }

            catch (Exception msg)
            {
                logger.Error(msg);

            }
        }

        //Метод для получения класса со строками XML файла конвертированной таблицы
        StringCollection GetLinesFromBase(TextEditor textBox)
        {
            linesBase.Clear();
            StringCollection offset = new StringCollection();
            int lineCount = fromBase.LineCount;
            for (int line = 1; line <= lineCount; line++)
            {
                int off = textBox.Document.GetLineByNumber(line).Offset;
                int len = textBox.Document.GetLineByNumber(line).Length;
                int linec = textBox.Document.GetLineByNumber(line).LineNumber;
                string linee = textBox.Document.GetText(off, len);

                TextLines textLines = new TextLines
                {
                    Number = linec,
                    Line = linee
                };
                linesBase.Add(textLines);

            }

            return offset;
        }

        //Метод для получения класса со строками XML файла с компьютера
        StringCollection GetLinesCollectionFromTextBox(TextEditor textBox)
        {
            linesList.Clear();
            StringCollection offset = new StringCollection();
            int lineCount = fromDesktop.LineCount;
            string linee = "";



            for (int line = 1; line <= lineCount; line++)
            {
                int off = textBox.Document.GetLineByNumber(line).Offset;
                int len = textBox.Document.GetLineByNumber(line).Length;
                linee = textBox.Document.GetText(off, len);
                int linec = textBox.Document.GetLineByNumber(line).LineNumber;
                TextLines textLines = new TextLines
                {
                    Number = linec,
                    Line = linee
                };
                linesList.Add(textLines);


            }
            return offset;

        }

        //Метод для переключения темы программы
        private void themeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            var brush = new SolidColorBrush(Color.FromRgb(236, 236, 236));
            fromDesktop.Background = brush;
            fromBase.Background = brush;
            BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/brightness.png", UriKind.Relative));
            themeImg.Source = image;
            fromDesktop.Foreground = Brushes.Black;
            fromBase.Foreground = Brushes.Black;
            warningBox.Text = "";

        }

        //Метод для переключения темы программы
        private void themeButton_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            fromDesktop.Background = Brushes.Black;
            fromBase.Background = Brushes.Black;
            BitmapImage image = new BitmapImage(new Uri("/XmlAnalyze;component/Images/sleep-mode-light.png", UriKind.Relative));
            themeImg.Source = image;
            fromDesktop.Foreground = Brushes.White;
            fromBase.Foreground = Brushes.White;
            warningBox.Text = "";

        }

        //Метод для слияния XML файлов
        private void sliyanie_Click(object sender, RoutedEventArgs e)
        {
            linesBase.Clear();
            linesList.Clear();
            GetLinesCollectionFromTextBox(fromDesktop);
            GetLinesFromBase(fromBase);

            fromDesktop.Text = "";

            for (int i = 0; i < linesList.Count; i++)
            {
                try
                {

                    if (linesBase[i].Line != linesList[i].Line)
                    {
                        linesList[i].Line = linesBase[i].Line;
                    }




                }
                catch (Exception msg)
                {
                    logger.Error(msg);
                }

            }



            for (int io = 0; io < linesList.Count; io++)
            {
                fromDesktop.Text += linesList[io].Line + '\n';


            }

            if (linesList.Count > linesBase.Count)
            {
                for (int i = linesBase.Count; i < linesList.Count; i++)
                {
                    try
                    {
                        fromDesktop.Text += linesBase[i].Line + '\n';
                    }
                    catch (Exception msg)
                    {
                        linesList[i].Line = "";
                        fromDesktop.Text += linesList[i].Line;
                        logger.Error(msg);
                    }
                }
            }

            else
            {

                for (int i = linesList.Count; i < linesBase.Count; i++)
                {
                    fromDesktop.Text += linesBase[i].Line + '\n';
                }
            }



            btnSave.IsEnabled = true;
            saveImg.Opacity = 1;
            saveTxt.Opacity = 1;
            warningBox.Text = "";

        }

        //Метод для открытия окна с возможностью сохранения
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            warningBox.Text = "";

            OpenWindow();
        }

        //Метод для перехода к отличающимся строкам
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            warningBox.Text = "";
            
            if (changes.Count != 0)
            {
                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesList.Count));
                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevBase(linesBase.Count));
                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(changes[changes.Count - 1]));
                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevBase(changes[changes.Count - 1]));

                if (nextLine == -1)
                {
                    fromDesktop.ScrollToLine(changes[0]);
                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changes[0]));

                    fromBase.ScrollToLine(changes[0]);
                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changes[0]));

                    nextLine = 0;
                }

                else
                {
                    try
                    {
                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine3]));
                    }
                    catch
                    {

                    }

                    if (linesMore.Count != 0 && changesNull.Count == 0 && nextLine == changes.Count - 1)
                    {


                        if (moreLine3 == -1)
                        {
                            if (linesList.Count > linesBase.Count)
                            {


                                try
                                {
                                    fromDesktop.ScrollToLine(linesMore[0]);
                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));

                                }
                                catch (Exception msg)
                                {
                                    logger.Error(msg);
                                }

                                moreLine3 = 0;
                            }
                            else
                            {
                                try
                                {
                                    fromBase.ScrollToLine(linesMore[0]);
                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));

                                }
                                catch (Exception msg)
                                {
                                    logger.Error(msg);
                                }

                                moreLine3 = 0;
                            }
                        }
                        else
                        {
                            {
                                if (moreLine3 < linesMore.Count - 1)
                                {
                                    if (linesList.Count > linesBase.Count)
                                    {
                                        try
                                        {
                                            fromDesktop.ScrollToLine(linesMore[moreLine3 + 1]);
                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine3 + 1]));
                                            moreLine3 += 1;

                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine3 - 1]));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            fromBase.ScrollToLine(linesMore[moreLine3 + 1]);
                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine3 + 1]));
                                            moreLine3 += 1;

                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine3 - 1]));
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                else
                                {

                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(changes[nextLine]));
                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevBase(changes[nextLine]));


                                    nextLine = -1;
                                    nullLine = -1;
                                    moreLine3 = -1;

                                }

                                if (moreLine3 + 1 == linesMore.Count)
                                {

                                    nextLine = -1;
                                    nullLine = -1;
                                    moreLine3 = -1;

                                }

                            }

                        }

                    }


                    if (nextLine <= changes.Count - 1)
                    {
                        try
                        {
                            fromDesktop.ScrollToLine(changes[nextLine + 1]);
                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changes[nextLine + 1]));

                            fromBase.ScrollToLine(changes[nextLine + 1]);
                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changes[nextLine + 1]));

                            nextLine += 1;

                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(changes[nextLine - 1]));
                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevBase(changes[nextLine - 1]));

                            if (nextLine + 1 >= changes.Count)
                            {
                                nullLine = -1;
                            }
                        }
                        catch
                        {
                            if (changesNull.Count != 0)
                            {
                                if (nullLine == -1)
                                {
                                    try
                                    {
                                        fromDesktop.ScrollToLine(changesNull[0]);
                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[0]));

                                    }
                                    catch (Exception msg)
                                    {
                                        logger.Error(msg);
                                    }
                                    nullLine = 0;

                                }
                                else
                                {

                                    if (nullLine + 1 == changesNull.Count && moreLine == -2)
                                    {

                                        moreLine = -1;
                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine]));

                                    }

                                    if (nullLine < changesNull.Count - 1)
                                    {

                                        fromDesktop.ScrollToLine(changesNull[nullLine + 1]);
                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[nullLine + 1]));
                                        nullLine += 1;

                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine - 1]));
                                    }
                                    else
                                    {
                                        if (linesMore.Count != 0)
                                        {


                                            if (linesMore.Count != 0)
                                            {


                                                if (moreLine == -1)
                                                {
                                                    if (linesList.Count > linesBase.Count)
                                                    {

                                                        try
                                                        {
                                                            fromDesktop.ScrollToLine(linesMore[0]);
                                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));

                                                        }
                                                        catch (Exception msg)
                                                        {
                                                            logger.Error(msg);
                                                        }

                                                        moreLine = 0;
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            fromBase.ScrollToLine(linesMore[0]);
                                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));

                                                        }
                                                        catch (Exception msg)
                                                        {
                                                            logger.Error(msg);
                                                        }

                                                        moreLine = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (moreLine + 1 == linesMore.Count)
                                                    {

                                                        nextLine = -1;
                                                        if (linesList.Count > linesBase.Count)
                                                        {
                                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));
                                                        }
                                                        else
                                                        {
                                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));

                                                        }
                                                    }
                                                    if (moreLine < linesMore.Count - 1)
                                                    {
                                                        if (linesList.Count > linesBase.Count)
                                                        {
                                                            fromDesktop.ScrollToLine(linesMore[moreLine + 1]);
                                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                            moreLine += 1;

                                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                        }
                                                        else
                                                        {
                                                            fromBase.ScrollToLine(linesMore[moreLine + 1]);
                                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                            moreLine += 1;

                                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (linesList.Count > linesBase.Count)
                                                        {
                                                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));

                                                        }
                                                        else
                                                        {
                                                            fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));

                                                        }
                                                        nullLine = -1;
                                                        moreLine = -2;

                                                        nextLine = -1;

                                                    }

                                                }

                                            }
                                        }
                                        else
                                        {
                                            nullLine = -1;
                                            moreLine = -2;
                                            if (linesMore.Count == 0)
                                            {
                                                nextLine = -1;
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }


                    else if (changesNull.Count != 0)
                    {
                        if (nullLine == -1)
                        {
                            try
                            {
                                fromDesktop.ScrollToLine(changesNull[0]);
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[0]));

                            }
                            catch (Exception msg)
                            {
                                logger.Error(msg);
                            }
                            nullLine = 0;

                        }
                        else
                        {

                            if (nullLine + 1 == changesNull.Count && moreLine == -2)
                            {

                                moreLine = -1;
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine]));

                            }

                            if (nullLine < changesNull.Count - 1)
                            {

                                fromDesktop.ScrollToLine(changesNull[nullLine + 1]);
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[nullLine + 1]));
                                nullLine += 1;

                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine - 1]));
                            }
                            else
                            {
                                if (linesMore.Count != 0)
                                {


                                    if (linesMore.Count != 0)
                                    {


                                        if (moreLine == -1)
                                        {
                                            try
                                            {
                                                if (linesList.Count > linesBase.Count)
                                                {
                                                    fromDesktop.ScrollToLine(linesMore[0]);
                                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                                                }
                                                else
                                                {
                                                    fromBase.ScrollToLine(linesMore[0]);
                                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                                                }
                                            }
                                            catch (Exception msg)
                                            {
                                                logger.Error(msg);
                                            }

                                            moreLine = 0;
                                        }
                                        else
                                        {
                                            {

                                                if (moreLine + 1 == linesMore.Count)
                                                {
                                                    if (linesList.Count > linesBase.Count)
                                                    {
                                                        nextLine = -1;
                                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));
                                                    }
                                                    else
                                                    {
                                                        nextLine = -1;
                                                        fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));
                                                    }
                                                }
                                                if (moreLine < linesMore.Count - 1)
                                                {
                                                    if (linesList.Count > linesBase.Count)
                                                    {
                                                        fromDesktop.ScrollToLine(linesMore[moreLine + 1]);
                                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                        moreLine += 1;

                                                        fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                    }
                                                    else
                                                    {
                                                        fromBase.ScrollToLine(linesMore[moreLine + 1]);
                                                        fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                        moreLine += 1;

                                                        fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                    }
                                                }
                                                else
                                                {
                                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));

                                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));
                                                    nullLine = -1;
                                                    moreLine = -2;

                                                    nextLine = -1;

                                                }

                                            }

                                        }

                                    }
                                }
                                else
                                {
                                    nullLine = -1;
                                    moreLine = -2;
                                }
                            }

                        }

                    }


                    if (nextLine + 1 >= changes.Count && changesNull.Count == 0 && linesMore.Count == 0)
                    {
                        nextLine = -1;
                        nullLine = -1;
                        moreLine = -2;
                        moreLine3 = -1;
                    }
                }
            }

            else
            {
                
                if (changesNull.Count != 0)
                {
                    if (nullLine2 == -1)
                    {
                        try
                        {
                            fromDesktop.ScrollToLine(changesNull[0]);
                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[0]));

                        }
                        catch (Exception msg)
                        {
                            logger.Error(msg);
                        }
                        nullLine2 = 0;

                    }
                    else
                    {

                        if (nullLine2 + 1 == changesNull.Count && moreLine == -2)
                        {

                            moreLine = -1;
                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine2]));

                        }

                        if (nullLine2 < changesNull.Count - 1)
                        {

                            fromDesktop.ScrollToLine(changesNull[nullLine2 + 1]);
                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(changesNull[nullLine2 + 1]));
                            nullLine2 += 1;

                            fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrevNull(changesNull[nullLine2 - 1]));
                        }
                        else
                        {
                            if (linesMore.Count != 0)
                            {


                                if (linesMore.Count != 0)
                                {


                                    if (moreLine == -1)
                                    {
                                        try
                                        {
                                            if (linesList.Count > linesBase.Count)
                                            {
                                                fromDesktop.ScrollToLine(linesMore[0]);
                                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                                            }
                                            else
                                            {
                                                fromBase.ScrollToLine(linesMore[0]);
                                                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                                            }
                                        }
                                        catch (Exception msg)
                                        {
                                            logger.Error(msg);
                                        }

                                        moreLine = 0;
                                    }
                                    else
                                    {
                                        {

                                            if (moreLine + 1 == linesMore.Count)
                                            {

                                                nextLine = -1;
                                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));

                                            }
                                            if (moreLine < linesMore.Count - 1)
                                            {
                                                if (linesList.Count > linesBase.Count)
                                                {
                                                    fromDesktop.ScrollToLine(linesMore[moreLine + 1]);
                                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                    moreLine += 1;

                                                    fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                }
                                                else
                                                {
                                                    fromBase.ScrollToLine(linesMore[moreLine + 1]);
                                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine + 1]));
                                                    moreLine += 1;

                                                    fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine - 1]));
                                                }
                                            }
                                            else
                                            {

                                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine]));
                                                nullLine2 = -1;
                                                moreLine = -2;

                                                nextLine = -1;

                                            }

                                        }

                                    }

                                }
                            }
                            else
                            {
                                nextLine = -1;
                                nullLine2 = -1;
                                moreLine = -2;
                            }
                        }

                    }


                }

                if (linesMore.Count != 0 && changesNull.Count == 0)
                {


                    if (moreLine2 == -1)
                    {
                        try
                        {
                            if (linesList.Count > linesBase.Count)
                            {

                                fromDesktop.ScrollToLine(linesMore[0]);
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                            }
                            else
                            {
                                fromBase.ScrollToLine(linesMore[0]);
                                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[0]));
                            }
                        }
                        catch (Exception msg)
                        {
                            logger.Error(msg);
                        }

                        moreLine2 = 0;
                    }
                    else
                    {

                        if (moreLine2 < linesMore.Count - 1)
                        {
                            if (linesList.Count > linesBase.Count)
                            {
                                fromDesktop.ScrollToLine(linesMore[moreLine2 + 1]);
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine2 + 1]));
                                moreLine2 += 1;

                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine2 - 1]));
                            }
                            else
                            {
                                fromBase.ScrollToLine(linesMore[moreLine2 + 1]);
                                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerNext(linesMore[moreLine2 + 1]));
                                moreLine2 += 1;

                                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine2 - 1]));
                            }
                        }
                        else
                        {
                            if (linesList.Count > linesBase.Count)
                            {
                                fromDesktop.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine2]));
                            }
                            else
                            {
                                fromBase.TextArea.TextView.LineTransformers.Add(new LineColorizerPrev(linesMore[moreLine2]));

                            }

                            nextLine = -1;
                            nullLine2 = -1;
                            moreLine2 = -1;

                        }

                    }

                }


            }

        }

        //Метод для обработки события открытия окна
        private void OpenWindow()
        {
            bool isWindowOpen = false;

            foreach (Window w in Application.Current.Windows)
            {
                if (w is SaveType)
                {
                    isWindowOpen = true;
                    w.Activate();
                }
            }

            if (!isWindowOpen)
            {
                SaveType newwindow = new SaveType(fromDesktop.Text, path);
                newwindow.Show();
            }
            warningBox.Text = "";

        }

        //Метод для очистки редактора с XML файлом с компьютера
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            fromDesktop.Text = "";
            comapreBtn.IsEnabled = false;
            linesCountDesktop.Text = "Количество строк: ";
            warningBox.Text = "";


        }

        //Метод для очистки редактора с XML файлом конвертированной таблицы
        private void btnClearBase_Click(object sender, RoutedEventArgs e)
        {
            fromBase.Text = "";
            comapreBtn.IsEnabled = false;
            linesCountBase.Text = "Количество строк: ";
            warningBox.Text = "";

        }

        //Метод для быстрого перемещения по XML файлу с компьютера
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            int lines = fromDesktop.LineCount;
            fromDesktop.ScrollToLine(lines);
            warningBox.Text = "";

        }

        //Метод для быстрого перемещения по XML файлу конвертированной таблицы
        private void downBtn_Click(object sender, RoutedEventArgs e)
        {
            int lines = fromBase.LineCount;
            fromBase.ScrollToLine(lines);
            warningBox.Text = "";

        }

        //Метод для быстрого перемещения по XML файлу конвертированной таблицы
        private void upBtn_Click(object sender, RoutedEventArgs e)
        {
            fromBase.ScrollToLine(1);
            warningBox.Text = "";

        }

        //Метод для быстрого перемещения по XML файлу с компьютера
        private void upBtnDesktop_Click(object sender, RoutedEventArgs e)
        {
            fromDesktop.ScrollToLine(1);
            warningBox.Text = "";

        }

        //Метод для конвертации XML файла с компьютера в JSON
        private void deskToJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                warningBox.Text = "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fromDesktop.Text);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                fromDesktop.Text = "";
                fromDesktop.Text = jsonText;
                // To convert JSON text contained in string json into an XML node
                //XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            }
            catch
            {
                warningBox.Text = "Неправильная структура файла!";

            }


        }

        //Метод для конвертации XML файла конвертированной таблицы в JSON
        private void baseToJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                warningBox.Text = "";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fromBase.Text);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                fromBase.Text = "";
                fromBase.Text = jsonText;
            }
            catch
            {
                warningBox.Text = "Неправильная структура файла!";
            }
        }
    }
}
