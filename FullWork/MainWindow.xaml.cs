using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace FullWork
{
    public static class Ext
    {
        public static void SetRandomPosition(this Button button, Grid grid)
        {
            var hButton = new Random().Next(0, (int)(grid.ActualHeight - button.ActualHeight));
            var wButton = new Random().Next(0, (int)(grid.ActualWidth - button.ActualWidth));

            button.Height = button.Height - (button.Height / 10);
            button.Width = button.Width - (button.Width / 10);
            button.Margin = new Thickness(wButton, hButton, 0, 0);
        }
    }



    public partial class MainWindow : Window
    {
        private const string Path = "settings.json";
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
              {
                  GetFile();
              };
            Closing += (sender, e) =>
              {
                  SaveFile();
              };
            GenerateButtons();
            btnOk.MouseMove += (sender, e) =>
            {
                var button = sender as Button;
                button.SetRandomPosition(gridClicker);
            };
            btnOk.Click += (sender, e) =>
            {
                MessageBox.Show("The button cannot be pressed", "Greate", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            btnFolders.Click += (s, e) =>
            {
                GenerateFolders();
            };
            tbContent.MouseDoubleClick += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tbContent.Text))
                    return;
                DirectoryInfo directoryInfoC = new DirectoryInfo(@"d:\");
                FileInfo[] files = null;
                try
                {

                    files = directoryInfoC.GetFiles("*" + tbContent.Text + "*.*", SearchOption.AllDirectories);
                    filesListlb.ItemsSource = files.OrderByDescending(x => x.CreationTime).Select(x => x.FullName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            foreach (var item in textClac.Children)
            {
                if (item is Button)
                {
                    (item as Button).Click += Button_Click;
                }
            }
        }




        #region Config
        private void SaveFile()
        {
            var fileToSave = new Configuration((bool)cbText.IsChecked, tbText.Text, Height, Width);
            var json = JsonSerializer.Serialize(fileToSave);
            using var sw = new StreamWriter(Path);
            sw.WriteLine(json);
            sw.Dispose();
        }

        private void GetFile()
        {
            if (!File.Exists(Path))
                return;
            using var sr = new StreamReader(Path);
            var json = sr.ReadToEnd();
            var config = JsonSerializer.Deserialize<Configuration>(json);
            cbText.IsChecked = config.CheckBox;
            tbText.Text = config.TextBox;
            Height = config.Height;
            Width = config.Width;
        }
        #endregion


        #region Randomizer

        private int previewNumberClick = -1;
        private int clickCounter = 0;
        private void GenerateButtons()
        {
            var basicTop = 0;
            var basicLeft = 0;
            for (int i = 1; i <= 16; i++)
            {
                var btn = new Button();
                btn.Content = $"{GetRandomNumber()}";
                btn.Height = 30;
                btn.Width = 30;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.Click += ClickButton;
                btn.Margin = new Thickness(basicLeft += 35, 5, 0, 0);
                gridContent.Children.Add(btn);
            }
        }

        private void ClickButton(object sender, EventArgs e)
        {
            var button = sender as Button;
            var value = Convert.ToInt32(button.Content.ToString());
            if (previewNumberClick == -1 && value == 0)
            {
                previewNumberClick = 0;
                button.Visibility = Visibility.Collapsed;
                clickCounter++;
                return;
            }
            if (++previewNumberClick == value)
            {
                previewNumberClick = value;
                button.Visibility = Visibility.Collapsed;
                clickCounter++;
                CheckIsSuccess();
                return;
            }
            Reset();
        }

        private void CheckIsSuccess()
        {
            if (clickCounter == 16)
            {
                var result = MessageBox.Show("Scucess", "Inform", MessageBoxButton.OK, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    Reset();
                }
            }
        }

        private void Reset()
        {
            gridContent.Children.Clear();
            numbers = new List<int>(16);
            GenerateButtons();
            previewNumberClick = -1;
            clickCounter = 0;
        }

        private int GetRandomNumber()
        {
            int result = 0;
            var random = new Random();
            do
            {
                result = random.Next(0, 16);
            } while (numbers.Contains(result));
            numbers.Add(result);
            return result;
        }

        private List<int> numbers = new List<int>(16);
        #endregion


        #region Folders

        private void SelectFiles()
        {
            var path = tbContent.Text;
        }

        private void GenerateFolders()
        {
            var path = Directory.GetCurrentDirectory();
            if (!Environment.MachineName.Contains("Nikita"))
                return;
            if (int.TryParse(tbContent.Text, out int count))
            {
                for (int i = 1; i <= count; i++)
                {
                    path += "\\" + new Random().Next(1, 9999).ToString();
                    Directory.CreateDirectory(path);
                }
            }
        }

        #endregion


        #region Calc

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var str = (string)((Button)e.OriginalSource).Content;
                if (str == "AC")
                {
                    textMain.Text = "";
                }
                else if (str == "=")
                {
                    var values = new DataTable().Compute(textMain.Text, null).ToString();
                    textMain.Text = values;
                }
                else
                    textMain.Text += str;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

    }
}
