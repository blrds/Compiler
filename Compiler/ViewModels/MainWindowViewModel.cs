using Compiler.Infrastructure.Commands;
using Compiler.Infrastructure.StructureDefinitions.Base;
using Compiler.Infrastructure.GrammarCheckerStructure;
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
using System.Diagnostics;

namespace Compiler.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        const string about = "about.txt";
        const string reference = "reference.txt";
        private readonly HighlightingRuleSet ruleSet;

        #region variables
        CustomHighlightingDefenition muplDefenition;
        private readonly List<bool> changesFlag = new List<bool>();//флаги изменений
        private readonly List<bool> saveFlag = new List<bool>();//флаги предшествующего сохранения
        private StructureDefinition StructureDefinition = null;


        #region bindingVars

        /// <summary>Collection of the tabs</summary>
        public ObservableCollection<TabItem> TabItems { get; set; }
        #region propsForTabs
        private TabItem SelectedItem => TabItems.Where(x => x.IsSelected).First();
        private int SelectedIndex
        {
            get
            {
                for (int i = 0; i < TabItems.Count; i++)
                {
                    if (TabItems[i].IsSelected) return i;
                }
                return -1;
            }
            set => TabItems[value].IsSelected = true;
        }

        private TextEditor TextEditor(TabItem tab)
        {
            return tab.Content as TextEditor;
        }

        private TextEditor TextEditor(int index)
        {
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
            var i = sp.Children.Add(new Button
            {
                Content = new ImageAwesome
                {
                    Icon = EFontAwesomeIcon.Regular_WindowClose,
                    Height = 15,
                },
                BorderThickness = new Thickness(0)
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
        private void OnOpenFileCommandExecuted(object p)
        {
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
                catch (ArgumentException exp) { OutputText.Text = "Данный путь недопустим или содержит недопустимые символы"; }
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
        private bool CanSaveAsCommnadExecute(object p) => (TabItems.Count > 0 ? true : false) && (SelectedIndex != -1);
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
                OutputText.Text = "Успешно";
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
        private bool CanStringDecompilationCommnadExecute(object p)
        {
            var a = TextEditor(SelectedItem);
            return (a.Text != "" || a.SelectedText != "");

        }
        private void OnStringDecompilationCommandExecuted(object p)
        {
            string fullText = "";
            if (TextEditor(SelectedItem).SelectedText != "")
                fullText = TextEditor(SelectedItem).SelectedText;
            else fullText = TextEditor(SelectedItem).Text;
            if (StructureDefinition == null) StructureDefinition = new StructureDefinition();
            var lines = StructureDefinition.Decomposite(fullText);
            var answer = GrammarChecker.VariablesDecloration(lines);
            OutputText.Text = "";
            foreach (var a in answer)
            {
                OutputText.Text += a.ToString() + "\n";
                foreach (var b in a.DeclorationErrors)
                    OutputText.Text += b.Message + "\n";
            }
        }
        #endregion

        #region StatementCommand

        public ICommand StatementCommand { get; }
        private bool CanStatementCommnadExecute(object p) => true;
        private void OnStatementCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://"+fileInfo.FullName+"#page=3"));
        }

        #endregion
        #region GrammarCommand

        public ICommand GrammarCommand { get; }
        private bool CanGrammarCommnadExecute(object p) => true;
        private void OnGrammarCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=4"));
        }

        #endregion

        #region GrammarClassificationCommand

        public ICommand GrammarClassificationCommand { get; }
        private bool CanGrammarClassificationCommnadExecute(object p) => true;
        private void OnGrammarClassificationCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=5"));
        }

        #endregion
        #region AnalysisMethodCommand

        public ICommand AnalysisMethodCommand { get; }
        private bool CanAnalysisMethodCommnadExecute(object p) => true;
        private void OnAnalysisMethodCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=7"));
        }

        #endregion
        #region ErrorsResolveCommand

        public ICommand ErrorsResolveCommand { get; }
        private bool CanErrorsResolveCommnadExecute(object p) => true;
        private void OnErrorsResolveCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=8"));
        }

        #endregion
        #region TestsCommand

        public ICommand TestsCommand { get; }
        private bool CanTestsCommnadExecute(object p) => true;
        private void OnTestsCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=9"));
        }

        #endregion
        #region LiteratureCommand

        public ICommand LiteratureCommand { get; }
        private bool CanLiteratureCommnadExecute(object p) => true;
        private void OnLiteratureCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=28"));
        }

        #endregion
        #region CodeCommand

        public ICommand CodeCommand { get; }
        private bool CanCodeCommnadExecute(object p) => true;
        private void OnCodeCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=11"));
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
        public MainWindowViewModel()
        {
            #region Commands
            OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommnadExecute);
            CreateCommand = new LambdaCommand(OnCreateCommandExecuted, CanCreateCommnadExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommnadExecute);
            SaveAsCommand = new LambdaCommand(OnSaveAsCommandExecuted, CanSaveAsCommnadExecute);
            ExitCommand = new LambdaCommand(OnExitCommandExecuted, CanExitCommnadExecute);
            ReferenceCommand = new LambdaCommand(OnReferenceCommandExecuted, CanReferenceCommnadExecute);
            AboutCommand = new LambdaCommand(OnAboutCommandExecuted, CanAboutCommnadExecute);
            StringDecompilationCommand = new LambdaCommand(OnStringDecompilationCommandExecuted, CanStringDecompilationCommnadExecute);
            StatementCommand = new LambdaCommand(OnStatementCommandExecuted, CanStatementCommnadExecute);
            GrammarCommand = new LambdaCommand(OnGrammarCommandExecuted, CanGrammarCommnadExecute);
            GrammarClassificationCommand = new LambdaCommand(OnGrammarClassificationCommandExecuted, CanGrammarClassificationCommnadExecute);
            AnalysisMethodCommand = new LambdaCommand(OnAnalysisMethodCommandExecuted, CanAnalysisMethodCommnadExecute);
            ErrorsResolveCommand = new LambdaCommand(OnErrorsResolveCommandExecuted, CanErrorsResolveCommnadExecute);
            TestsCommand = new LambdaCommand(OnTestsCommandExecuted, CanTestsCommnadExecute);
            LiteratureCommand = new LambdaCommand(OnLiteratureCommandExecuted, CanLiteratureCommnadExecute);
            CodeCommand = new LambdaCommand(OnCodeCommandExecuted, CanCodeCommnadExecute);
            #endregion
            TabItems = new ObservableCollection<TabItem>();
            ruleSet = RuleSetCreator.ExtractRuleSet();
            muplDefenition = new CustomHighlightingDefenition(ruleSet);
            Application.Current.MainWindow.Closing += main_Closing;

            OnCreateCommandExecuted(this);
        }

    }
}
