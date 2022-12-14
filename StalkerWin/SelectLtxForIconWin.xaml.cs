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
    /// Логика взаимодействия для SelectLtxForIconWin.xaml
    /// </summary>
    public partial class SelectLtxForIconWin : Window
    {
        public SelectLtxForIconWin(int[] inv,BitmapImage iconByCut,string pathToItem)
        {
            InitializeComponent();
            LoadDataBrowser(pathToItem);
            img_icon.Source = iconByCut;
            inv_grid = inv;
        }

        private int[] inv_grid;

        private void LoadDataBrowser(string path)
        {

            treeBrowser.Items.Clear();
            TreeViewItem fItem = new TreeViewItem();
            fItem.Tag = path;
            fItem.Header = MainWindow.ProgramData.GetLastSplash(path.Trim('\\'));
            AddRecurceFolders(path, fItem);
            treeBrowser.Items.Add(fItem);
        }
        private void AddRecurceFolders(string path, TreeViewItem item)
        {
            DirectoryInfo f = new DirectoryInfo(path);
            foreach (var v in f.GetFiles())
            {
                TreeViewItem fS = new TreeViewItem();
                fS.Tag = v.FullName;
                fS.Header = v.Name;
                item.Items.Add(fS);
            }
            foreach (var v in f.GetDirectories())
            {

                TreeViewItem nE = new TreeViewItem();
                nE.Tag = v.FullName;
                nE.Header = v.Name;

                item.Items.Add(nE);
                AddRecurceFolders(v.FullName, nE);
            }
        }

        private void treeBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(treeBrowser.SelectedItem != null)
            {
                string namefile = (((TreeViewItem)(treeBrowser.SelectedItem)).Header.ToString());

                labContent.Text = namefile;

            }
        }

        private void btn_invoke_Click(object sender, RoutedEventArgs e)
        {
            if (treeBrowser.SelectedItem != null)
            {
                string fullname = (((TreeViewItem)(treeBrowser.SelectedItem)).Tag.ToString());
                
                StalkerClass.HierarchyLtx.LtxFile data = new StalkerClass.HierarchyLtx.LtxFile(new FileInfo(fullname), MainWindow.ProgramData.Encoding_LTX);
                if (data.Sections.Count <= 0)
                    return;

                string section = data.Sections[0].Name_Section;

                data.Sections.Where(x => x.Name_Section == section).First().Parametrs.Where(x => x.Name_Parametr == "inv_grid_x").First().Value_Parametr = inv_grid[0].ToString();
                data.Sections.Where(x => x.Name_Section == section).First().Parametrs.Where(x => x.Name_Parametr == "inv_grid_y").First().Value_Parametr = inv_grid[1].ToString();
                data.Sections.Where(x => x.Name_Section == section).First().Parametrs.Where(x => x.Name_Parametr == "inv_grid_width").First().Value_Parametr = inv_grid[2].ToString();
                data.Sections.Where(x => x.Name_Section == section).First().Parametrs.Where(x => x.Name_Parametr == "inv_gridinv_grid_height_x").First().Value_Parametr = inv_grid[3].ToString();


                File.WriteAllText(data.File.FullName, data.ToString(), MainWindow.ProgramData.Encoding_LTX);
                MessageBox.Show("Иконка изменена!", "Изменение иконки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
