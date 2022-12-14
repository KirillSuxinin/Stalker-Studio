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
using System.Drawing;

namespace Stalker_Studio.StalkerWin.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для MessageOkCancelWin.xaml
    /// </summary>
    public partial class MessageOkCancelWin : Window
    {
        public MessageOkCancelWin(string title,string textDialog,string pathImg)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            this.Title = title;
            this.labText.Content = textDialog;

            if (pathImg != null)
            {
                Bitmap btsI = new Bitmap(pathImg);
                img.Source = MainWindow.BitmapToImageSource(btsI);
            }
        }

        public bool IsOk = false;

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();

        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            this.Close();
        }
    }
}
