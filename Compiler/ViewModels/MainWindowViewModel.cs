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



        //переменные данного региона являются мостами связывающими основную программу и интерфейс
        #region bindingVars

        /// <summary>
        /// Вкладки с текстовыми редакоторами
        /// </summary>
        public ObservableCollection<TabItem> TabItems { get; set; }


        //функции и переменне необходимые дял контроля и работы вкладок
        #region propsForTabs

        /// <summary>
        /// Выбранная вклдака
        /// </summary>
        private TabItem SelectedItem => TabItems.Where(x => x.IsSelected).First();

        /// <summary>
        /// Выбранный индекс
        /// </summary>
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

        /// <summary>
        /// downcast контента вкладки до текстового поля
        /// </summary>
        /// <param name="tab">вкладка, внутри которой есть поле ввода</param>
        /// <returns> поле ввода</returns>
        private TextEditor TextEditor(TabItem tab)
        {
            return tab.Content as TextEditor;
        }

        /// <summary>
        /// downcast контента вкладки до текстового поля
        /// </summary>
        /// <param name="tab">индекс вкладки</param>
        /// <returns> поле ввода</returns>
        private TextEditor TextEditor(int index)
        {
            return TextEditor(TabItems[index]);
        }


        #endregion
        /// <summary>
        /// Текст окна вывода
        /// </summary>
        public TextDocument OutputText { get; private set; } = new TextDocument();

        #endregion
        #endregion

        #region unicFunctions

        #region Creation of the text tab with all the settings
        /// <summary>
        /// Функция создания новой вкладки
        /// </summary>
        /// <param name="name">имя новой вкладки</param>
        void TabCreat(string name)
        {

            TabItems.Add(new TabItem//в коллекцию допсывае новую вкладку с заданными параметрами
            {
                Header = new StackPanel { Orientation = Orientation.Horizontal },
                Content = new TextEditor
                {
                    Margin = new Thickness(5),
                    AllowDrop = true,
                    ShowLineNumbers = true,
                    SyntaxHighlighting = muplDefenition,
                    FontSize = 20
                }
            });
            var sp = (TabItems.Last().Header as StackPanel);//в загаловке вкладки пишем имя и создаем кнопку закрытия данной вкладки
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

            (sp.Children[i] as Button).Click += closeTab_Click;//событие зарктытия вкладки
            (TabItems.Last().Content as TextEditor).KeyDown += Input_KeyDown; //событие обработки нажатий на кнопку
            (TabItems.Last().Content as TextEditor).Drop += main_Drop;//событие, когда файл, который хотят открыть переносят на окно программы
            (TabItems.Last().Content as TextEditor).PreviewDragOver += main_PreviewDragOver;//проверка перед вносом файла на окно программы
        }


        #region events of the tabs
        /// <summary>
        /// собыитие открытия брошенного файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))//Проверка если это событие есть бросок файла, а не чего-то еще
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);//получаем массив строк характеризующих бросаемые файлы
                foreach (var file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if ((fi.Extension == ".txt") || (fi.Extension == ".mupl"))//если файлы соответсвуют требуемым расширениям
                        OpenFile(file);//отркываем файлы
                }
            }
        }
        /// <summary>
        /// Разрешение броска файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        /// <summary>
        /// событие нажатия на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            changesFlag[SelectedIndex] = true;
        }
        /// <summary>
        /// событие закрытия вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Открытие файла
        /// </summary>
        /// <param name="file">полное имя файла</param>
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

        /// <summary>
        /// Открытие файла
        /// </summary>
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

        /// <summary>
        /// Создание файла
        /// </summary>
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

        /// <summary>
        /// Сохранение файла
        /// </summary>
        #region SaveCommand
        public ICommand SaveCommand { get; }
        private bool CanSaveCommnadExecute(object p) => (TabItems.Count > 0 ? true : false) && (SelectedIndex != -1);
        private void OnSaveCommandExecuted(object p)
        {
            if (!saveFlag[SelectedIndex]) { OnSaveAsCommandExecuted(p); return; }//если до этого файл нигде не был сохранен, то вызываем сохранение с диалогом
            else
            {
                try
                {
                    TextEditor tb = TextEditor(SelectedItem);

                    File.WriteAllText(((SelectedItem.Header as StackPanel).Children[0] as TextBlock).Text, tb.Text);//пишем весь текст из вкладки в файл
                }
                //ловим все ошибки
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

        /// <summary>
        /// Сохраить файл через диалог сохранения
        /// </summary>
        #region SaveAsCommand
        public ICommand SaveAsCommand { get; }
        private bool CanSaveAsCommnadExecute(object p) => (TabItems.Count > 0 ? true : false) && (SelectedIndex != -1);
        private void OnSaveAsCommandExecuted(object p)
        {
            SaveFileDialog sfd = new SaveFileDialog();//создаем диалог сохранения файла и настраиваем его
            sfd.AddExtension = true;
            sfd.Filter = "mupl files (*.mupl)|*.mupl|txt files (*.txt)|*.txt";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == true)//показываем диалог и ждем пока пользователь выйдет из него
            {//если ответ на запрос сохранения да
                TextEditor tb = TextEditor(SelectedItem);
                File.WriteAllText(sfd.FileName, tb.Text);//пишем весь текст в файл
                (SelectedItem.Header as TextBlock).Text = sfd.FileName; //замеяем имяфайла на новое, соответсвующее имени из диалога сохранения
            }
        }
        #endregion

        /// <summary>
        /// команда выхода
        /// </summary>
        #region ExitCommand
        public ICommand ExitCommand { get; }
        private bool CanExitCommnadExecute(object p) => true;
        private void OnExitCommandExecuted(object p)
        {
            Application.Current.MainWindow.Close();
        }
        #endregion

        /// <summary>
        /// вызов справки
        /// </summary>
        #region ReferenceCommand
        public ICommand ReferenceCommand { get; }
        private bool CanReferenceCommnadExecute(object p) => true;
        private void OnReferenceCommandExecuted(object p)
        {
            TextMessage message = new TextMessage("reference.txt");
            message.Show();
        }
        #endregion

        /// <summary>
        /// вызов о программе
        /// </summary>
        #region AboutCommand
        public ICommand AboutCommand { get; }
        private bool CanAboutCommnadExecute(object p) => true;
        private void OnAboutCommandExecuted(object p)
        {
            System.Windows.Forms.MessageBox.Show(File.ReadAllText(about), "О программе",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
        #endregion

        /// <summary>
        /// Разложение строки через сканер
        /// </summary>
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
            else fullText = TextEditor(SelectedItem).Text; //на обработку поступает выбранный текст или весь текст файла, если выбранного нет
            if (StructureDefinition == null) StructureDefinition = new StructureDefinition();//создаем экземпляр лексера
            var lines = StructureDefinition.Decomposite(fullText);//получаем строку из кодов распознанных лексем
            var answer = GrammarChecker.VariablesDecloration(lines);//получаем набор переменных после обработки автоматом
            OutputText.Text = "";
            foreach (var a in answer)
            {//записываем переменные и ошибки в вывод
                OutputText.Text += a.ToString() + "\n";
                foreach (var b in a.DeclorationErrors)
                    OutputText.Text += b.Message + "\n";
            }
        }
        #endregion


        /// <summary>
        /// Все команды данного региона открывают один и тот же пдф файл в разных позициях
        /// </summary>
        #region PDF File work
        #region StatementCommand

        public ICommand StatementCommand { get; }
        private bool CanStatementCommnadExecute(object p) => true;
        private void OnStatementCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=3"));
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
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=8"));
        }

        #endregion
        #region ErrorsResolveCommand

        public ICommand ErrorsResolveCommand { get; }
        private bool CanErrorsResolveCommnadExecute(object p) => true;
        private void OnErrorsResolveCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=9"));
        }

        #endregion
        #region TestsCommand

        public ICommand TestsCommand { get; }
        private bool CanTestsCommnadExecute(object p) => true;
        private void OnTestsCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=11"));
        }

        #endregion
        #region LiteratureCommand

        public ICommand LiteratureCommand { get; }
        private bool CanLiteratureCommnadExecute(object p) => true;
        private void OnLiteratureCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=10"));
        }

        #endregion
        #region CodeCommand

        public ICommand CodeCommand { get; }
        private bool CanCodeCommnadExecute(object p) => true;
        private void OnCodeCommandExecuted(object p)
        {
            FileInfo fileInfo = new FileInfo("kr.pdf");
            Process.Start(new ProcessStartInfo("cmd", $"/c start microsoftedge file://" + fileInfo.FullName + "#page=16"));
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Собития 
        /// </summary>
        #region Events
        /// <summary>
        /// Собитие закрытия текстового поля
        /// </summary>
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
