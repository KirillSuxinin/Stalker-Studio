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
    /// Логика взаимодействия для PaternWin.xaml
    /// </summary>
    public partial class PaternWin : Window
    {
        public PaternWin(string PathWork,string poly)
        {
            InitializeComponent();
            //need path to gamedata
            this.Title = $"Вставка ({poly})";
            WorkPath = PathWork;


        }


        private string WorkPath;

        public bool IsOk = false;
        public bool Add = false;

        public string TextBody
        {
            get;
            private set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

                MainWindow.ProgramData.LoadDataBrowser(WorkPath, treeBrowser, null, this);
                if (treeBrowser.Items.Count > 0)
                {
                    ((TreeViewItem)(treeBrowser.Items[0])).IsExpanded = true;
                }
            
        }

        private string LastInput { get { return _LastInput; } set { _LastInput = value;txt_ret_value.Text = value; } }
        private string _LastInput = null;

        private void treeBrowser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(treeBrowser.SelectedItem != null)
            {
                FileX = null;

                string path = ((TreeViewItem)(treeBrowser.SelectedItem)).Tag.ToString();
                //cut path for game
                string vsP = "";
                bool vr = false;
                for(int i = 0; i < path.Split('\\').Length; i++)
                {
                    
                    if (path.Split('\\')[i].ToUpper().Contains("gamedata".ToUpper()))
                    {
                        vr = true;
                        if (i + 2 < path.Split('\\').Length)
                            i += 2;
                        else
                        {
                            vsP = new FileInfo(path).Name;
                            break;
                        }

                    }
                    if (vr)
                    {
                        vsP += path.Split('\\')[i] + "\\";
                    }

                }
                vsP = vsP.Trim('\\');
                LastInput = vsP;
                list_section.Items.Clear();
                combo.Items.Clear();
                if (path.Contains(".ltx"))
                {
                    StalkerClass.HierarchyLtx.LtxFile f = new StalkerClass.HierarchyLtx.LtxFile(File.ReadAllText(path, MainWindow.ProgramData.Encoding_LTX));
                    
                    foreach(var v in f.Sections)
                    {
                        list_section.Items.Add(v.Name_Section);
                    }
                }

                if (path.Contains(".xml"))
                {
                    FileX = new StalkerClass.Xml.Xml_Text_File(new FileInfo(path), MainWindow.ProgramData.Encoding_XML);
                    foreach(var v in FileX.ExpressionsBlocks)
                    {
                        list_section.Items.Add(v.Id);
                    }
                }

            }
        }

        private StalkerClass.Xml.Xml_Text_File FileX;

        private void list_section_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(list_section.SelectedIndex != -1)
            {
                if (FileX == null)
                {
                    LastInput = list_section.Items[list_section.SelectedIndex].ToString();
                    string path = ((TreeViewItem)(treeBrowser.SelectedItem)).Tag.ToString();
                    StalkerClass.HierarchyLtx.LtxFile f = new StalkerClass.HierarchyLtx.LtxFile(File.ReadAllText(path, MainWindow.ProgramData.Encoding_LTX));
                    combo.Items.Clear();

                    int x = -1;
                    int y = -1;
                    int w = 1;
                    int h = 1;

                    foreach (var it in f.Sections[list_section.SelectedIndex].Parametrs)
                    {
                        if (it.Name_Parametr != null && it.IsValue)
                        {
                            combo.Items.Add(it.Name_Parametr);


                                if (it.Name_Parametr == "inv_grid_x")
                                {
                                    int.TryParse(it.Value_Parametr, out x);
                                }
                                if (it.Name_Parametr == "inv_grid_y")
                                {
                                    int.TryParse(it.Value_Parametr, out y);
                                }
                            
                            if (it.Name_Parametr == "inv_grid_width")
                            {
                                int.TryParse(it.Value_Parametr, out w);
                            }
                            if (it.Name_Parametr == "inv_grid_height")
                            {
                                int.TryParse(it.Value_Parametr, out h);
                            }
                        }
                    }

                    if (x != -1 && y != -1)
                    {
                        if (MainWindow.ProgramData.MainWinThread.FindIconDDS() != null && File.Exists(MainWindow.ProgramData.MainWinThread.FindIconDDS()))
                        {
                            StalkerClass.DDSImage dds = new StalkerClass.DDSImage(File.ReadAllBytes(MainWindow.ProgramData.MainWinThread.FindIconDDS()));
                            if (dds.IsValid)
                            {


                                img_icon_ltx.Source = MainWindow.BitmapToImageSource(StalkerClass.DDSImage.GetIcon(dds.BitmapImage, x, y, w, h));


                            }
                            else
                            {
                                img_icon_ltx.Source = null;
                            }
                        }
                        else
                        {
                            img_icon_ltx.Source = null;
                        }
                    }
                    else
                    {
                        img_icon_ltx.Source = null;
                    }


                }
                else if(FileX != null)
                {
                    LastInput = list_section.Items[list_section.SelectedIndex].ToString();
                    combo.Items.Clear();
                    combo.Items.Add("Text value");
                }
            }
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(combo.SelectedItem != null)
            {
                string path = ((TreeViewItem)(treeBrowser.SelectedItem)).Tag.ToString();
                if (FileX == null)
                {

                    StalkerClass.HierarchyLtx.LtxFile f = new StalkerClass.HierarchyLtx.LtxFile(File.ReadAllText(path, MainWindow.ProgramData.Encoding_LTX));

                    LastInput = f.Sections[list_section.SelectedIndex].Parametrs.Where(x => x.Name_Parametr == combo.SelectedItem.ToString()).First().Value_Parametr;
                }
                else
                {
                    LastInput = FileX.ExpressionsBlocks[list_section.SelectedIndex].Text.ToString();

                }
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.IsOk = false;
            this.Close();
        }

        private void btn_add_element_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Add = true;
            TextBody = txt_ret_value.Text;
            this.Close();
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Add = false;
            TextBody = txt_ret_value.Text;
            this.Close();
        }

        private void mn_load_xml_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.ProgramData.xmlStrings.Count > 0)
            {
                List<FileInfo> patternFiles = new List<FileInfo>();
                foreach (var vF in MainWindow.ProgramData.xmlStrings)
                    patternFiles.Add(vF._File);
                MainWindow.ProgramData.LoadDataBrowser(MainWindow.ProgramData.xmlStrings[0]._File.Directory.FullName, treeBrowser,patternFiles);
                if (treeBrowser.Items.Count > 0)
                    ((TreeViewItem)(treeBrowser.Items[0])).IsExpanded = true;
            }
        }

        private void nm_markers_Click(object sender, RoutedEventArgs e)
        {
            StalkerClass.HierarchyLtx.LtxFile markers = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{System.Windows.Forms.Application.StartupPath}\\Markers.ltx"), Encoding.UTF8);
            StalkerClass.HierarchyLtx.Ltx_Section markSection = markers.Sections.Where(x => x.Name_Section == "Markers").First();


            if (markSection.Parametrs.Count > 0)
            {
              //  string dirs = $"{markSection.Parametrs[0].Name_Parametr.Replace($"{MainWindow.ProgramData.GetLastSplash(markSection.Parametrs[0].Name_Parametr)}","")}";

                //List<FileInfo> patternFiles = new List<FileInfo>();
                TreeViewItem parent = new TreeViewItem();
                parent.Header = "Markers";

                foreach (var vF in markSection.Parametrs)
                {
                    if (System.IO.File.Exists(vF.Name_Parametr))
                    {
                        TreeViewItem it = new TreeViewItem();
                        it.Header = new FileInfo(vF.Name_Parametr).Name;
                        it.Tag = vF.Name_Parametr;
                        it.Foreground = AddonClass.ColorMarker.GetMarkerByPathFile(vF.Name_Parametr);
                        parent.Items.Add(it);
                    }
                   // patternFiles.Add(new FileInfo(vF.Name_Parametr));

                }
                treeBrowser.Items.Clear();
                treeBrowser.Items.Add(parent);
                parent.IsExpanded = true;


              //  MainWindow.ProgramData.LoadDataBrowser(dirs, treeBrowser, patternFiles);
              //  if (treeBrowser.Items.Count > 0)
                //    ((TreeViewItem)(treeBrowser.Items[0])).IsExpanded = true;
            }
        }

        private void nm_rootpath_Click(object sender, RoutedEventArgs e)
        {
            string PathWork = WorkPath;
            StalkerWin.Dialogs.MessageOkCancelWin message = new Dialogs.MessageOkCancelWin("Использовать корень папки?", "Использовать корневую папку gamedata?\nМожет занять время для загрузки", null);
            message.Owner = this;
            message.ShowDialog();
            if (message.IsOk)
            {
                string gamedata = "";
            HERE:

                for (int i = 0; i < PathWork.Split('\\').Length; i++)
                {
                    if (PathWork.Split('\\')[i].ToUpper().Contains("gamedata".ToUpper()))
                    {
                        gamedata += PathWork.Split('\\')[i];
                        break;
                    }
                    else
                        gamedata += PathWork.Split('\\')[i] + "\\";
                }
                Console.WriteLine(gamedata);

                if (!gamedata.ToUpper().Contains("gamedata".ToUpper()))
                {
                    if (MessageBox.Show("Извольтe выбрать полный путь к gamedata\nЧтобы найти папки для текстур,моделей,звука", "Полный путь", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        System.Windows.Forms.FolderBrowserDialog fold = new System.Windows.Forms.FolderBrowserDialog();
                        if (fold.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            gamedata = fold.SelectedPath;
                            goto HERE;
                        }
                    }
                    else
                        WorkPath = MainWindow.ProgramData.Gamedata;
                }

                WorkPath = gamedata;
            }

            MainWindow.ProgramData.LoadDataBrowser(WorkPath, treeBrowser,null,this,false);
            if (treeBrowser.Items.Count > 0)
            {

                ((TreeViewItem)(treeBrowser.Items[0])).IsExpanded = true;
            }
        }

        private void nm_refresh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ProgramData.LoadDataBrowser(WorkPath, treeBrowser, null, this);
            if (treeBrowser.Items.Count > 0)
            {

                ((TreeViewItem)(treeBrowser.Items[0])).IsExpanded = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
