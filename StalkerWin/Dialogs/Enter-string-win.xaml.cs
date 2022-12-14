using System;
using System.Collections.Generic;
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

namespace Stalker_Studio.StalkerWin.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для Enter_string_win.xaml
    /// </summary>
    public partial class Enter_string_win : Window
    {
        public Enter_string_win(string title,string description,string text = "")
        {
            InitializeComponent();
            this.Title = title;
            this.lab_Descr.Content = description;
            this.txt_Text.Text = text;
        }

        public bool IsOk = false;

        public string TextBody
        {
            get
            {
                return txt_Text.Text;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            this.Close();
        }
    }
}
