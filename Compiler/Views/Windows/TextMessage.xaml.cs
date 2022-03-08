using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Compiler.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TextMessage.xaml
    /// </summary>
    public partial class TextMessage : Window
    {
        public TextMessage()
        {
            InitializeComponent();
        }

        public TextMessage(string fileName)
        {
            InitializeComponent();
            box.AppendText(File.ReadAllText(fileName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
