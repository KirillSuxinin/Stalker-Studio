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

namespace Stalker_Studio.StalkerClass
{
    /// <summary>
    /// Логика взаимодействия для HintsWin.xaml
    /// </summary>
    public partial class HintsWin : Window
    {
        public delegate void PasteDelegate(string InWord,string Ower);
        public HintsWin(string[] items,PasteDelegate pasteLogic,string ower)
        {
            InitializeComponent();
            foreach(var i in items)
            {
                list_hints.Items.Add(i);
            }
            Ower = ower;
            past = pasteLogic;
            list_hints.Focus();
            if (list_hints.Items.Count > 0)
                list_hints.SelectedIndex = 0;
        }
        private PasteDelegate past;
        public Point SetLocatePoint
        {
            set
            {
                this.Left = value.X;
                this.Top = value.Y;
            }
            get
            {
                return new Point(this.Left, this.Top);
            }
        }



        private string Ower;

        private void list_hints_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(list_hints.SelectedIndex != -1)
            {
                past.Invoke(list_hints.SelectedItem.ToString(), Ower);
                this.Close();
            }
        }

        private void list_hints_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && list_hints.SelectedItem != null)
            {
                past.Invoke(list_hints.SelectedItem.ToString(), Ower);
                this.Close();
            }
        }
    }
}
