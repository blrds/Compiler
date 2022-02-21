using Compiler.Infrastructure.Commands;
using Compiler.ViewModels.Base;
using ICSharpCode.AvalonEdit;
using System.Collections.Generic;
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
        #endregion

        #region unicFunctions
        #region Creation of the text tab with all the settings
        void TabCreat(string name)
        {
            TabControl tabs = Application.Current.MainWindow.FindName("tabs") as TabControl;
            tabs.Items.Add(new TabItem
            {
                Header = new StackPanel { Orientation = Orientation.Horizontal },
                Content = new TextEditor { Margin = new Thickness(5), AllowDrop = true, 
                                           ShowLineNumbers = true, SyntaxHighlighting = muplDefenition, 
                                           Visibility=Visibility.Visible, IsEnabled=true }
            });
            var sp = ((tabs.Items[tabs.Items.Count - 1] as TabItem).Header as StackPanel);
            sp.Children.Add(new TextBlock { Text = name });
            var i = sp.Children.Add(new Button { Content = "X" });
            tabs.SelectedIndex = tabs.Items.Count - 1;
            /*(sp.Children[i] as Button).Click += closeTab_Click;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).KeyDown += Input_KeyDown;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).Drop += main_Drop;
            ((tabs.Items[tabs.Items.Count - 1] as TabItem).Content as TextEditor).PreviewDragOver += main_PreviewDragOver;*/
        } 
        #endregion

        #endregion
        #region  Commands

        #region OpenFileCommand
        public ICommand OpenFileCommand { get; }
        private bool CanOpenFileCommnadExecute(object p) => true;
        private void OnOpenFileCommandExecuted(object p) {
            
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

        }
         
    }
}
