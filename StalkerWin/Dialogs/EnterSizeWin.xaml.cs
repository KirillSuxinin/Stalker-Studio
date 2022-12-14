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
    /// Логика взаимодействия для EnterSizeWin.xaml
    /// </summary>
    public partial class EnterSizeWin : Window
    {
        public EnterSizeWin(string description)
        {
            InitializeComponent();
            this.lab_descr.Content = description;
        }

        public bool IsOk = false;

        public float Si_Width = 0f;
        public float Si_Heigth = 0f;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(float.TryParse(txt_width.Text,out Si_Width) && float.TryParse(txt_heigth.Text,out Si_Heigth))
            {
                IsOk = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Данные не введены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                IsOk = false;
            }
        }
    }
}
