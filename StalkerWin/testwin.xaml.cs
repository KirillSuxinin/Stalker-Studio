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
using System.Drawing.Imaging;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace Stalker_Studio.StalkerWin
{
    /// <summary>
    /// Логика взаимодействия для testwin.xaml
    /// </summary>
    public partial class testwin : Window
    {
        public testwin()
        {
            InitializeComponent();


            string origUI = @"C:\Games\S.T.A.L.K.E.R. - Shadow Of Chernobyl\gamedata\textures\ui\ui_icon_equipment_orig — копия.dds";
            string addUIP = @"C:\Users\Кирилл\Desktop\2.png";


        }



        private List<string> GetPathTexturesByModel(FileInfo pathModels)
        {
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
                        if (ls.Where(x => x == v.Trim().Replace("Map_Kd", "").Replace("Map_Ka", "")).Count() <= 0)
                            ls.Add(v.Trim().Replace("Map_Kd", "").Replace("Map_Ka", ""));
                    }
                }
            }



            return ls;
        }
    }
}
