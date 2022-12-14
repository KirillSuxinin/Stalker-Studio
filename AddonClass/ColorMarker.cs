using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace Stalker_Studio.AddonClass
{
    public static class ColorMarker
    {
        public static void RemoveMarkerByPathFile(string path)
        {
            StalkerClass.HierarchyLtx.LtxFile markers = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{Application.StartupPath}\\Markers.ltx"), Encoding.UTF8);
            StalkerClass.HierarchyLtx.Ltx_Section markSection = markers.Sections.Where(x => x.Name_Section == "Markers").First();

            if(markSection.Parametrs.Where(x => x.Name_Parametr == path).Count() > 0)
            {
                markSection.Parametrs.Remove(markSection.Parametrs.Where(x => x.Name_Parametr == path).First());
            }
            System.IO.File.WriteAllText($"{Application.StartupPath}\\Markers.ltx", markers.ToString(), Encoding.UTF8);
        }

        public static SolidColorBrush GetMarkerByPathFile(string path)
        {
            StalkerClass.HierarchyLtx.LtxFile markers = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{Application.StartupPath}\\Markers.ltx"), Encoding.UTF8);
            StalkerClass.HierarchyLtx.Ltx_Section markSection = markers.Sections.Where(x => x.Name_Section == "Markers").First();
            foreach(var v in markSection.Parametrs)
            {
                if (v.Name_Parametr == path)
                    return ParseColor(v.Value_Parametr);
            }
            return null;
        }

        public static void AddMarkerByPathFile(string path,string color)
        {
            StalkerClass.HierarchyLtx.LtxFile markers = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo($"{Application.StartupPath}\\Markers.ltx"), Encoding.UTF8);
            StalkerClass.HierarchyLtx.Ltx_Section markSection = markers.Sections.Where(x => x.Name_Section == "Markers").First();
            bool find = false;
            foreach (var v in markSection.Parametrs)
            {
                if (v.Name_Parametr == path)
                {
                    v.Value_Parametr = color;
                    find = true;
                }
            }
            if (!find)
            {
                markSection.Parametrs.Add(new StalkerClass.HierarchyLtx.Ltx_Parametr($"{path} = {color}"));
            }

            System.IO.File.WriteAllText($"{Application.StartupPath}\\Markers.ltx", markers.ToString(), Encoding.UTF8);
        }

        private static SolidColorBrush ParseColor(string value)
        {
            byte r = byte.Parse(value.Split(',')[0]);
            byte g = byte.Parse(value.Split(',')[1]);
            byte b = byte.Parse(value.Split(',')[2]);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

    }
}
