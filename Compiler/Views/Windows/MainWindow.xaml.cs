﻿using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
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
            /*Create(null, null);//создание базового окна*/
        }






        

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            
            
        }



        

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
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

            (tabs.Items[0] as TabItem).IsSelected = true;
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

        /*private void closeTab_Click(object sender, RoutedEventArgs e)
        {
            var tab = ((sender as Button).Parent as StackPanel).Parent as TabItem;
            tab.IsSelected = true;
            var i = tabs.SelectedIndex;
            if (changesFlag[i])
                Save(null, null);
            tabs.Items.Remove(tab);
        }*/
        
        private void task_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
