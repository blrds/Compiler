using ICSharpCode.AvalonEdit;
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
        private List<bool> changesFlag = new List<bool>();//флаги изменений
        private List<bool> saveFlag = new List<bool>();//флаги предшествующего сохранения
        private CustomHighlightingDefenition muplDefenition;
        public MainWindow()
        {
            InitializeComponent();
            Create(null, null);//создание базового окна
            muplDefenition = new CustomHighlightingDefenition();
        }
        private void OutputMsg(string text)
        {
            output.Text=text;
        }
        
        void TabCreat(string name)//добавление новой вкладки
        {
            tabs.Items.Add(new TabItem
            {
                Header = new StackPanel { Orientation=Orientation.Horizontal},
                Content = new TextEditor { Margin = new Thickness(5),  AllowDrop=true, ShowLineNumbers=true, SyntaxHighlighting=muplDefenition}
            });
            var sp = ((tabs.Items[tabs.Items.Count - 1] as TabItem).Header as StackPanel);
            sp.Children.Add(new TextBlock { Text = name });
            var i = sp.Children.Add(new Button { Content = "X" });
            (sp.Children[i] as Button).Click += closeTab_Click;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).KeyDown += Input_KeyDown;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).Drop += main_Drop;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).PreviewDragOver += main_PreviewDragOver;
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            TabCreat("Безымянный.mupl");
            changesFlag.Add(false);
            saveFlag.Add(false);
        }

        private void OpenFile(string file) {
            TabCreat(file);
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).Text = File.ReadAllText(file);
            OutputMsg("Успешно");
            saveFlag.Add(true);
            changesFlag.Add(false);
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    OpenFile(ofd.FileName);
                }
                catch (FileFormatException exc) { OutputMsg( "Неверный формат файла"); }
                catch (FileLoadException exc) { OutputMsg("Файл не может быть заргужен"); }
                catch (FileNotFoundException exc) { OutputMsg("Файл не найден"); }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (!saveFlag[tabs.SelectedIndex]) { SaveAs(sender, e); return; }
            else {
                try
                {
                    TabItem tabItem = tabs.SelectedItem as TabItem;
                    TextEditor tb = tabItem.Content as TextEditor;
                    
                    File.WriteAllText((tabItem.Header as TextBlock).Text, tb.Text);
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
            sfd.AddExtension = true;
            sfd.Filter = "mupl files (*.mupl)|*.mupl|txt files (*.txt)|*.txt";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)
            {
                TabItem tab = tabs.SelectedItem as TabItem;
                TextEditor tb = tab.Content as TextEditor;
                File.WriteAllText(sfd.FileName, tb.Text);
                OutputMsg("Успешно");
                (tab.Header as TextBlock).Text = sfd.FileName;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            changesFlag[tabs.SelectedIndex] = true;
        }

        private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < tabs.Items.Count; ++i) {
                if (changesFlag[i])
                {
                    tabs.SelectedIndex = i;
                    Save(null, null);
                }
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void main_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files) {
                    FileInfo fi = new FileInfo(file);
                    if ((fi.Extension == ".txt") || (fi.Extension == ".mupl"))
                        OpenFile(file);
                }
            }
        }
        private void main_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).Undo();
        }

        private void repeat_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).Redo();
        }

        private void erase_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).Cut();
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).Copy();
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).Paste();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).SelectedText = "";
        }

        private void selectAll_Click(object sender, RoutedEventArgs e)
        {
            ((tabs.SelectedItem as TabItem).Content as TextEditor).SelectAll();
        }

        private void closeTab_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((sender as Button).Parent as StackPanel).Parent as TabItem;
            tab.IsSelected = true;
            var i = tabs.SelectedIndex;
            if (changesFlag[i])
                Save(null, null);
            tabs.Items.Remove(tab);
        }
    }
}
