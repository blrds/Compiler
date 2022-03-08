using Compiler.Infrastructure.Commands;
using Compiler.Models;
using Compiler.ViewModels.Base;
using Compiler.Views.Windows;
using FontAwesome5;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Compiler.ViewModels
{
    internal class MainWindowViewModel:ViewModel
    {
        const string about = "about.txt";
        const string reference = "reference.txt";
        private readonly HighlightingRuleSet ruleSet;

        #region variables
        CustomHighlightingDefenition muplDefenition;
        private readonly List<bool> changesFlag = new List<bool>();//флаги изменений
        private readonly List<bool> saveFlag = new List<bool>();//флаги предшествующего сохранения


        #region bindingVars

        /// <summary>Collection of the tabs</summary>
        public ObservableCollection<TabItem> TabItems { get; set; }
        #region propsForTabs
        private TabItem SelectedItem=>TabItems.Where(x => x.IsSelected).First();
        private int SelectedIndex { 
            get{
                for (int i = 0; i < TabItems.Count; i++)
                {
                    if (TabItems[i].IsSelected) return i;
                }
                return -1;
            }
            set => TabItems[value].IsSelected = true;
        }

        private TextEditor TextEditor(TabItem tab) {
            return tab.Content as TextEditor; 
        }

        private TextEditor TextEditor(int index) {
            return TextEditor(TabItems[index]);
        }


        #endregion
        /// <summary>OutPut  Text</summary>
        public TextDocument OutputText { get; private set; } = new TextDocument();
    
        #endregion
        #endregion

        #region unicFunctions
        #region Creation of the text tab with all the settings
        void TabCreat(string name)
        {

            TabItems.Add(new TabItem
            {
                Header = new StackPanel { Orientation = Orientation.Horizontal },
                Content = new TextEditor
                {
                    Margin = new Thickness(5),
                    AllowDrop = true,
                    ShowLineNumbers = true,
                    SyntaxHighlighting = muplDefenition
                }
            });
            var sp = (TabItems.Last().Header as StackPanel);
            sp.Children.Add(new TextBlock { Text = name });
            var i = sp.Children.Add(new Button { 
                Content = new ImageAwesome {
                    Icon=EFontAwesomeIcon.Regular_WindowClose, 
                    Height=15,
                },
                BorderThickness=new Thickness(0)
            });
            TabItems.Last().IsSelected = true;

            (sp.Children[i] as Button).Click += closeTab_Click;
            (TabItems.Last().Content as TextEditor).KeyDown += Input_KeyDown;
            (TabItems.Last().Content as TextEditor).Drop += main_Drop;
            (TabItems.Last().Content as TextEditor).PreviewDragOver += main_PreviewDragOver;
        }


        #region events of the tabs
        private void main_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
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

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            changesFlag[SelectedIndex] = true;
        }

        private void closeTab_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((sender as Button).Parent as StackPanel).Parent as TabItem;
            tab.IsSelected = true;
            if (changesFlag[SelectedIndex])
                OnSaveCommandExecuted(sender);
            TabItems.Remove(tab);
        }
        #endregion
        #endregion

        private void OpenFile(string file)
        {
            TabCreat(file);
            (TabItems.Last().Content as TextEditor).Text = File.ReadAllText(file);
            OutputText.Text = "Упешно";
            saveFlag.Add(true);
            changesFlag.Add(false);
        }
        #endregion
        #region  Commands

        #region OpenFileCommand
        public ICommand OpenFileCommand { get; }
        private bool CanOpenFileCommnadExecute(object p) => true;
        private void OnOpenFileCommandExecuted(object p) {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "mupl files (*.mupl)|*.mupl|txt files (*.txt)|*.txt";
            ofd.AddExtension = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    OpenFile(ofd.FileName);
                }
                catch (FileFormatException exc) { OutputText.Text = "Неверный формат файла"; }
                catch (FileLoadException exc) { OutputText.Text = "Файл не может быть заргужен"; }
                catch (FileNotFoundException exc) { OutputText.Text = "Файл не найден"; }
            }
        }
        #endregion

        #region CreateCommand
        public ICommand CreateCommand { get; }
        private bool CanCreateCommnadExecute(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {
            TabCreat("noname.mupl");
            changesFlag.Add(false);
            saveFlag.Add(false);
        }
        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }
        private bool CanSaveCommnadExecute(object p) => (TabItems.Count > 0 ? true : false) && (SelectedIndex != -1);
        private void OnSaveCommandExecuted(object p)
        {
            if (!saveFlag[SelectedIndex]) { OnSaveAsCommandExecuted(p); return; }
            else
            {
                try
                {
                    TextEditor tb = TextEditor(SelectedItem);

                    File.WriteAllText(((SelectedItem.Header as StackPanel).Children[0] as TextBlock).Text, tb.Text);
                }
                catch (ArgumentException exp) { OutputText.Text="Данный путь недопустим или содержит недопустимые символы"; }
                catch (PathTooLongException exp) { OutputText.Text = "Путь или имя файла превышают допустимую длину"; }
                catch (DirectoryNotFoundException exp) { OutputText.Text = "Указан недопустимый путь (например, он ведет на несопоставленный диск)"; }
                catch (IOException exp) { OutputText.Text = "При открытии файла произошла ошибка ввода-вывода"; }
                catch (UnauthorizedAccessException exp) { OutputText.Text = ""; }
                catch (NotSupportedException exp) { OutputText.Text = "Неверный формат файла"; }
                catch (SecurityException exp) { OutputText.Text = "Неверный формат файла"; }
            }
        }

        #endregion

        #region SaveAsCommand
        public ICommand SaveAsCommand { get; }
        private bool CanSaveAsCommnadExecute(object p) => (TabItems.Count > 0 ? true : false)&&(SelectedIndex!=-1);
        private void OnSaveAsCommandExecuted(object p)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "mupl files (*.mupl)|*.mupl|txt files (*.txt)|*.txt";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)
            {
                TextEditor tb = TextEditor(SelectedItem);
                File.WriteAllText(sfd.FileName, tb.Text);
                OutputText.Text="Успешно";
                (SelectedItem.Header as TextBlock).Text = sfd.FileName;
            }
        }
        #endregion

        #region ExitCommand
        public ICommand ExitCommand { get; }
        private bool CanExitCommnadExecute(object p) => true;
        private void OnExitCommandExecuted(object p)
        {
            Application.Current.MainWindow.Close();
        }
        #endregion

        #region ReferenceCommand
        public ICommand ReferenceCommand { get; }
        private bool CanReferenceCommnadExecute(object p) => true;
        private void OnReferenceCommandExecuted(object p)
        {
            TextMessage message = new TextMessage("reference.txt");
            message.Show();
        }
        #endregion

        #region AboutCommand
        public ICommand AboutCommand { get; }
        private bool CanAboutCommnadExecute(object p) => true;
        private void OnAboutCommandExecuted(object p)
        {
            System.Windows.Forms.MessageBox.Show(File.ReadAllText(about), "О программе",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        #endregion

        #region StringDecompilationCommand
        public ICommand StringDecompilationCommand { get; }
        private bool CanStringDecompilationCommnadExecute(object p) => true;
        private void OnStringDecompilationCommandExecuted(object p)
        {
            
        }
        #endregion
        #endregion

        #region Events
        public void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < TabItems.Count; ++i)
            {
                if (changesFlag[i])
                {
                    SelectedIndex = i;
                    OnSaveCommandExecuted(sender);
                }
            }
        }
        #endregion
        public MainWindowViewModel() {
            #region Commands
            OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommnadExecute);
            CreateCommand = new LambdaCommand(OnCreateCommandExecuted, CanCreateCommnadExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommnadExecute);
            SaveAsCommand = new LambdaCommand(OnSaveAsCommandExecuted, CanSaveAsCommnadExecute);
            ExitCommand = new LambdaCommand(OnExitCommandExecuted, CanExitCommnadExecute);
            ReferenceCommand = new LambdaCommand(OnReferenceCommandExecuted, CanReferenceCommnadExecute);
            AboutCommand = new LambdaCommand(OnAboutCommandExecuted, CanAboutCommnadExecute);
            StringDecompilationCommand = new LambdaCommand(OnStringDecompilationCommandExecuted, CanStringDecompilationCommnadExecute);
            #endregion
            TabItems = new ObservableCollection<TabItem>();
            ruleSet = RuleSetCreator.ExtractRuleSet();
            muplDefenition = new CustomHighlightingDefenition(ruleSet);
            Application.Current.MainWindow.Closing += main_Closing;

            OnCreateCommandExecuted(this);
        }
         
    }
}
