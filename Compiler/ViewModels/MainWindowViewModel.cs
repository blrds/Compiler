using Compiler.Infrastructure.Commands;
using Compiler.ViewModels.Base;
using FontAwesome5;
using ICSharpCode.AvalonEdit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Compiler.ViewModels
{
    internal class MainWindowViewModel:ViewModel
    {
        #region variables
        CustomHighlightingDefenition muplDefenition = new CustomHighlightingDefenition();
        private readonly List<bool> changesFlag = new List<bool>();//флаги изменений
        private readonly List<bool> saveFlag = new List<bool>();//флаги предшествующего сохранения


        #region bindingVars

        /// <summary>Collection of the tabs</summary>
        public ObservableCollection<TabItem> TabItems { get; set; }

        /// <summary>OutPut  Text</summary>
        public string OutputText { get; private set; } = "Here is your output";

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
            var i = sp.Children.Add(new Button { Content = new ImageAwesome {Icon=EFontAwesomeIcon.Regular_ClosedCaptioning } });;
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
        #endregion
        #endregion

        private void OpenFile(string file)
        {
            TabCreat(file);
            (TabItems.Last().Content as TextEditor).Text = File.ReadAllText(file);
            OutputText = "Успешно";
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
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    OpenFile(ofd.FileName);
                }
                catch (FileFormatException exc) { OutputText="Неверный формат файла"; }
                catch (FileLoadException exc) { OutputText = "Файл не может быть заргужен"; }
                catch (FileNotFoundException exc) { OutputText = "Файл не найден"; }
            }
        }
        #endregion

        #region CreateCommand
        public ICommand CreateCommand { get; }
        private bool CanCreateCommnadExecute(object p) => true;
        private void OnCreateCommandExecuted(object p)
        {
            TabCreat("NoName.mupl");
            changesFlag.Add(false);
            saveFlag.Add(false);
        }
        #endregion
        #endregion
        public MainWindowViewModel() {
            #region Commands
            OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommnadExecute);
            CreateCommand = new LambdaCommand(OnCreateCommandExecuted, CanCreateCommnadExecute);
            #endregion
            TabItems = new ObservableCollection<TabItem>();
        }
         
    }
}
