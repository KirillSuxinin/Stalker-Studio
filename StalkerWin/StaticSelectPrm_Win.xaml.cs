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
using System.IO;

namespace Stalker_Studio.StalkerWin
{
    /// <summary>
    /// Логика взаимодействия для StaticSelectPrm_Win.xaml
    /// </summary>
    public partial class StaticSelectPrm_Win : Window
    {
        public StaticSelectPrm_Win()
        {
            InitializeComponent();
        }

        public StaticSelectPrm_Win(List<FileInfo> ltxFiles)
        {
            InitializeComponent();
            ListLtxFiles = ltxFiles;
        }

        private List<FileInfo> ListLtxFiles = null;

        public bool IsOk = false;

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsOk = false;
            this.Close();
        }


        public string TextBody
        {
            get
            {
                if (ListLtxFiles == null)
                    return list_stat_prm.Items[list_stat_prm.SelectedIndex].ToString();
                else
                    return txt_stat.Text;
            }
        }




        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            if (list_stat_prm.SelectedIndex != -1)
            {
                this.IsOk = true;
                this.Close();
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //here loader
            if (ListLtxFiles == null)
            {
                StalkerClass.HierarchyLtx.LtxFile fls = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{System.Windows.Forms.Application.StartupPath}\\static_prm\\static_hud.ltx"), Encoding.UTF8);
                foreach (var v in fls.Sections)
                {
                    list_stat_prm.Items.Add(v.Name_Section);
                }
            }
            else
            {
                foreach(var vFiles in ListLtxFiles)
                {
                    StalkerClass.HierarchyLtx.LtxFile fls = new StalkerClass.HierarchyLtx.LtxFile(vFiles, MainWindow.ProgramData.Encoding_LTX);

                    foreach(var vSect in fls.Sections)
                    {
                        list_stat_prm.Items.Add(vSect.Name_Section);
                    }
                }
            }
        }

        private void list_stat_prm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txt_stat.Text = "";
            if(list_stat_prm.SelectedIndex != -1 )
            {
                if (ListLtxFiles == null)
                {
                    StalkerClass.HierarchyLtx.LtxFile fls = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{System.Windows.Forms.Application.StartupPath}\\static_prm\\static_hud.ltx"), Encoding.UTF8);
                    foreach (var vP in fls.Sections[list_stat_prm.SelectedIndex].Parametrs)
                    {
                        txt_stat.Text += vP.ToString() + Environment.NewLine;
                    }
                }
                else
                {
                    foreach(var vFile in ListLtxFiles)
                    {
                        StalkerClass.HierarchyLtx.LtxFile fls = new StalkerClass.HierarchyLtx.LtxFile(vFile, Encoding.UTF8);
                        foreach (var vP in fls.Sections[list_stat_prm.SelectedIndex].Parametrs)
                        {
                            txt_stat.Text += vP.ToString() + Environment.NewLine;
                        }
                    }
                }
            }
        }
    }

}
