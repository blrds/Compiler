using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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

namespace Compiler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool saveFlag = false;
        private bool changesFlag = false;
        private void Create(object sender, RoutedEventArgs e)
        {

            //if (changesFlag)
            tabs.Items.Add(new TabItem { 
                Header = new TextBlock {Text="Безымянный.mupl" },
                Content = new RichTextBox { Margin=new Thickness(5)}
            });
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as RichTextBox).KeyDown += Input_KeyDown;
            saveFlag = false;
            changesFlag = false;
        }

        private void OutputMsg(string text) {
            output.Document.Blocks.Clear();
            output.Document.Blocks.Add(new Paragraph(new Run(text)));
        }
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    tabs.Items.Add(new TabItem
                    {
                        Header = new TextBlock { Text = ofd.FileName },
                        Content = new RichTextBox { Margin = new Thickness(5) }
                    });
                    ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as RichTextBox).KeyDown += Input_KeyDown;
                    ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as RichTextBox).Document.Blocks.Add(new Paragraph(new Run(File.ReadAllText(ofd.FileName))));
                    OutputMsg("Успешно");
                }
                catch (FileFormatException exc) { OutputMsg( "Неверный формат файла"); }
                catch (FileLoadException exc) { OutputMsg("Файл не может быть заргужен"); }
                catch (FileNotFoundException exc) { OutputMsg("Файл не найден"); }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (saveFlag) { SaveAs(sender, e); return; }
            else {
                try
                {
                    TabItem tabItem = tabs.SelectedItem as TabItem;
                    RichTextBox rtb = tabItem.Content as RichTextBox;
                    
                    File.WriteAllText((tabItem.Header as TextBlock).Text, StringFromRichTextBox(rtb));
                }
                catch (ArgumentException exp) { OutputMsg("Данный путь недопустим или содержит недопустимые символы"); }
                catch (PathTooLongException exp) { OutputMsg("Путь или имя файла превышают допустимую длину"); }
                catch (DirectoryNotFoundException exp) { OutputMsg("Указан недопустимый путь (например, он ведет на несопоставленный диск)"); }
                catch (IOException exp) { OutputMsg("При открытии файла произошла ошибка ввода-вывода"); }
                catch (UnauthorizedAccessException exp) { OutputMsg(""); }
                catch (NotSupportedException exp) { OutputMsg("Неверный формат файла"); }
                catch (SecurityException exp) { OutputMsg("Неверный формат файла"); }
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, (new TextRange(input.Document.ContentStart, input.Document.ContentEnd)).Text);
                output.Document.Blocks.Add(new Paragraph(new Run("Успешно")));
                this.Title = sfd.FileName;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            changesFlag = true;
        }
    }
}
