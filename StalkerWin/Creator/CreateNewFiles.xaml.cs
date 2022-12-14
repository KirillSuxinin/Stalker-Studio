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
using System.Diagnostics;

namespace Stalker_Studio.StalkerWin.Creator
{
    /// <summary>
    /// Логика взаимодействия для CreateNewFiles.xaml
    /// </summary>
    public partial class CreateNewFiles : Window
    {
        public CreateNewFiles(string pathWork)
        {
            InitializeComponent();
            Init_MenuItem();
            PathWork = pathWork;
            this.Title = $"Создания файла в \"{pathWork}\"";


        }

        private string PathWork;

        private void Init_MenuItem()
        {
            foreach(MenuItem vs in txt_heir.ContextMenu.Items)
            {
                vs.Click += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(txt_heir.Text))
                    {
                        txt_heir.Text += "," + vs.Header.ToString();
                    }
                    else
                    {
                        txt_heir.Text = vs.Header.ToString();
                    }
                };
            }

            foreach(MenuItem vs in txt_pack_ltx.ContextMenu.Items)
            {
                vs.Click += (sender, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(txt_pack_ltx.Text))
                    {
                        txt_pack_ltx.Text += "," + vs.Header.ToString();
                    }
                    else
                    {
                        txt_pack_ltx.Text = vs.Header.ToString();
                    }
                };
            }
        }

        private void HideFunctionChild()
        {
            foreach(FrameworkElement vs in Grid_Function.Children)
            {
                if (vs is Grid)
                    vs.Visibility = Visibility.Hidden;
            }
        }


        
        private void list_mode_creat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideFunctionChild();
            if (list_mode_creat.SelectedIndex == 0)
                Grid_Create_LTX.Visibility = Visibility.Visible;

            
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            if(Grid_Create_LTX.Visibility == Visibility.Visible)
            {
                //create only one ltx
                StalkerClass.HierarchyLtx.Ltx_Section mSect = new StalkerClass.HierarchyLtx.Ltx_Section();
                mSect.Heir_Section = txt_heir.Text;
                mSect.Name_Section = txt_name.Text;
                if (mSect.Description_Section == null || !mSect.Description_Section.Contains("Stalker_Studio"))
                    mSect.Description_Section = "       Create by Stalker_Studio";

                mSect.Parametrs = new List<StalkerClass.HierarchyLtx.Ltx_Parametr>();

                List<StalkerClass.HierarchyLtx.Ltx_Section> other = new List<StalkerClass.HierarchyLtx.Ltx_Section>();

                
                StalkerClass.HierarchyLtx.LtxFile const_f = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo(System.Windows.Forms.Application.StartupPath + "\\Pack_parametr\\OriginalPacks.ltx"), Encoding.UTF8);
                foreach(var vs in txt_pack_ltx.Text.Split(','))
                {
                    if(const_f.Sections.Where(x => x.Name_Section == vs).Count() > 0)
                    {
                        StalkerClass.HierarchyLtx.Ltx_Section sect = const_f.Sections.Where(x => x.Name_Section == vs).First();
                        if(sect.Heir_Section != null && sect.Heir_Section.Contains("$NEW"))
                        {
                            StalkerClass.HierarchyLtx.Ltx_Section newSect = new StalkerClass.HierarchyLtx.Ltx_Section();
                            newSect.Name_Section = txt_name.Text + sect.Heir_Section.Split('|')[1];


                            if (newSect.Description_Section == null || !newSect.Description_Section.Contains("Stalker_Studio"))
                                newSect.Description_Section = "         Create by Stalker_Studio";
                            newSect.Parametrs = new List<StalkerClass.HierarchyLtx.Ltx_Parametr>();
                            foreach(var v in sect.Parametrs)
                            {
                                newSect.Parametrs.Add(v);
                            }
                            other.Add(newSect);
                        }
                        else if(sect.Heir_Section == null)
                        {
                            //add to main
                            foreach (var v in sect.Parametrs)
                            {
                                if (v.Value_Parametr != null)
                                    v.Value_Parametr = v.Value_Parametr.Replace("$NAME", txt_name.Text);
                                mSect.Parametrs.Add(v);
                            }
                        }

                    }

                    StalkerClass.HierarchyLtx.LtxFile f = new StalkerClass.HierarchyLtx.LtxFile("");
                    f.Sections.Add(mSect);
                    foreach (var v in other)
                    {
                        f.Sections.Add(v);
                    }

                    if (!txt_name_file.Text.EndsWith(".ltx"))
                        txt_name_file.Text += ".ltx";

                    System.IO.File.WriteAllText(PathWork + "\\" + txt_name_file.Text, f.ToString(), MainWindow.ProgramData.Encoding_LTX);
                    FileName = txt_name_file.Text;
           //         MessageBox.Show("Новый файл успешно создан!", "Создание", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();

                }



                //work a with textures,models,sounds
                IndexFiles();
            }
        }

        private void IndexFiles()
        {
            //need path to gamedata
            string gamedata = "";
        HERE:

            for(int i =0; i < PathWork.Split('\\').Length; i++)
            {
                if (PathWork.Split('\\')[i].ToUpper().Contains("gamedata".ToUpper()))
                {
                    gamedata += PathWork.Split('\\')[i];
                    break;
                }
                else
                    gamedata += PathWork.Split('\\')[i] + "\\";
            }

            if (!gamedata.EndsWith("gamedata"))
            {
                if (MessageBox.Show("Извольти выбрать полный путь к gamedata\nЧтобы найти папки для текстур,моделей,звука", "Полный путь", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    System.Windows.Forms.FolderBrowserDialog fold = new System.Windows.Forms.FolderBrowserDialog();
                    if(fold.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        gamedata = fold.SelectedPath;
                        goto HERE;
                    }
                }
                else
                    return;
            }
            else
            {
                string textures = $"{gamedata}\\textures";
                if (!Directory.Exists(textures))
                    Directory.CreateDirectory(textures);

                string sounds = $"{gamedata}\\sounds\\weapons";
                if (!Directory.Exists(sounds))
                    Directory.CreateDirectory(sounds);

                string meshes = $"{gamedata}\\meshes\\weapons";
                if (Directory.Exists($"{gamedata}\\meshes\\dynamics\\weapons"))
                    meshes = $"{gamedata}\\meshes\\dynamics\\weapons";
                else if (!Directory.Exists(meshes))
                    Directory.CreateDirectory(meshes);




                foreach(var v in Sounds)
                {
                    try
                    {

                        File.Copy(v.FullName, $"{sounds}\\{v.FullName}");
                    }
                    catch
                    {

                    }
                }
                Directory.CreateDirectory($"{meshes}\\{txt_name.Text}");
                string convert = $"{System.Windows.Forms.Application.StartupPath}\\Convertor";

                string[] ogfDirs = Directory.GetFiles(convert, "*.ogf");
                foreach (var v in ogfDirs)
                {
                    try
                    {
                        File.Delete(v);
                    }
                    catch
                    {

                    }
                }

                string[] mtl = Directory.GetFiles(convert, "*.mtl");
                foreach (var v in mtl)
                {
                    try
                    {
                        File.Delete(v);
                    }
                    catch
                    {

                    }
                }



                convert += "\\ogf2obj.exe";


                List<string> notFoundTextures = new List<string>();

                meshes += "\\" + txt_name.Text;
                foreach(var v in Models)
                {
                    
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(v.FullName);
                        Console.ForegroundColor = ConsoleColor.White;

                        if (!File.Exists($"{meshes}\\{v.Name}"))
                            File.Copy(v.FullName, $"{meshes}\\{v.Name}");


                        List<string> txte = GetPathTexturesByModel(new FileInfo(v.FullName));
                        foreach(var vText in txte)
                        {
                            bool find = false;
                            foreach(var _textus in Textures)
                            {
                                if(vText.ToUpper().EndsWith(_textus.Name.ToUpper()))
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    if (!Directory.Exists($"{textures}\\{vText.Replace(MainWindow.ProgramData.GetLastSplash(vText), "")}"))
                                        Directory.CreateDirectory($"{textures}\\{vText.Replace(MainWindow.ProgramData.GetLastSplash(vText), "")}");
                                    if (!File.Exists($"{textures}\\{vText}"))
                                        File.Copy(_textus.FullName, $"{textures}\\{vText}");
    
                                    Console.ForegroundColor = ConsoleColor.White;
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                                
                                notFoundTextures.Add(vText);
                            }
                        }
                    }
                    catch(Exception g)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    //    Console.WriteLine(g);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                if (notFoundTextures.Count > 0)
                {
                    string _TXT = "";
                    foreach(var v in notFoundTextures)
                    {
                        _TXT += v + Environment.NewLine;
                    }

                    MessageBox.Show($"Нету след. текстур:\n{_TXT}", "Текстуры!", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

                if(MessageBox.Show($"Файл создан \'{txt_name_file.Text}\'\nПерезапустить обозреватель ресурсов?", "Файл успешно создан", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Restart = true;
                }

            }
        }

        public bool Restart = false;

        private List<string> GetPathTexturesByModel(FileInfo pathModels)
        {

            if (File.Exists($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\ogfmodel.mtl"))
                File.Delete($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\ogfmodel.mtl");

            List<string> ls = new List<string>();
            string convert = $"{System.Windows.Forms.Application.StartupPath}\\Convertor\\ogf2obj.exe";

            if (!File.Exists($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\{pathModels.Name}"))
            {
                File.Copy(pathModels.FullName, $"{System.Windows.Forms.Application.StartupPath}\\Convertor\\{pathModels.Name}");
            }


            string bat = $"cd Convertor{Environment.NewLine}ogf2obj.exe {pathModels.Name} invoke.obj";


            File.WriteAllText($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\bat.bat", bat, Encoding.Default);


            ProcessStartInfo batS = new ProcessStartInfo($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\bat.bat") { CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true };

            Process proc = Process.Start(batS);

            proc.StandardOutput.ReadToEnd();



            if (File.Exists($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\ogfmodel.mtl"))
            {
                string[] fls = File.ReadAllLines($"{System.Windows.Forms.Application.StartupPath}\\Convertor\\ogfmodel.mtl", Encoding.UTF8);
                foreach (var v in fls)
                {
                    if (v.Contains("\\") || v.Contains(".dds"))
                    {
                        if (ls.Where(x => x == v.Trim().Replace("Map_Kd", "").Replace("Map_Ka", "").Trim()).Count() <= 0)
                            ls.Add(v.Trim().Replace("Map_Kd", "").Replace("Map_Ka", "").Trim());
                    }
                }
            }



            return ls;
        }


        public string FileName;


        private List<FileInfo> Textures = new List<FileInfo>();
        private List<FileInfo> Models = new List<FileInfo>();
        private List<FileInfo> Sounds = new List<FileInfo>();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //models add
            System.Windows.Forms.OpenFileDialog fls = new System.Windows.Forms.OpenFileDialog();
            fls.Multiselect = true;
            fls.Filter = "OGF|*.ogf|All Files|*.*";
            if(fls.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var v in fls.FileNames)
                {
                    Models.Add(new FileInfo(v));
                }
                Init_Models_lst();
            }
        }

        private void Init_Models_lst()
        {
            lst_models.Items.Clear();
            foreach(var v in Models)
            {
                lst_models.Items.Add(v.Name);
            }
        }

        private void Init_Textures_lst()
        {
            lst_textures.Items.Clear();
            foreach (var v in Textures)
            {
                lst_textures.Items.Add(v.Name);
            }
        }

        private void Init_Sounds_lst()
        {
            lst_sounds.Items.Clear();
            foreach (var v in Sounds)
            {
                lst_sounds.Items.Add(v.Name);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if(lst_models.SelectedIndex != -1)
            {
                Models.RemoveAt(lst_models.SelectedIndex);
                Init_Models_lst();
            }

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fls = new System.Windows.Forms.OpenFileDialog();
            fls.Multiselect = true;
            fls.Filter = "DDS|*.dds|All Files|*.*";
            if (fls.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Textures.Add(new FileInfo(fls.FileName));
                foreach(var v in fls.FileNames)
                {
                    Textures.Add(new FileInfo(v));
                }

                Init_Textures_lst();
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (lst_textures.SelectedIndex != -1)
            {
                Textures.RemoveAt(lst_textures.SelectedIndex);
                Init_Textures_lst();
            }

        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fls = new System.Windows.Forms.OpenFileDialog();
            fls.Multiselect = true;
            fls.Filter = "ogg|*.ogg|All Files|*.*";
            if (fls.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach(var v in fls.FileNames)
                {
                    Sounds.Add(new FileInfo(v));
                }
                Init_Sounds_lst();
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if (lst_sounds.SelectedIndex != -1)
            {
                Sounds.RemoveAt(lst_sounds.SelectedIndex);
                Init_Sounds_lst();
            }
        }
    }
}
