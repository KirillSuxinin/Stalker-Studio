using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace Stalker_Studio.StalkerClass
{
    public class HintWordStack
    {
        public List<string> WHints = new List<string>();

        public HintWordStack(string pathHint)
        {
            WHints = new List<string>();
            StalkerClass.HierarchyLtx.LtxFile fF = new HierarchyLtx.LtxFile(new System.IO.FileInfo(pathHint), Encoding.UTF8);
            foreach(var v in fF.Sections)
            {
                foreach(var v2 in v.Parametrs)
                {
                    WHints.Add(v2.Name_Parametr);
                }
            }
        }

        public List<string> WHints_Local = new List<string>();
        public List<string> WHints_Function = new List<string>();

        public string[] words_spliter = new string[]
{
            ' '.ToString(),
            '('.ToString(),
            ')'.ToString(),
            '|'.ToString(),
            '['.ToString(),
            ']'.ToString(),
            '{'.ToString(),
            '}'.ToString()
            
};

       public Key[] clsKey = new Key[]
        {
            Key.Space,
            Key.Enter,
            Key.Up,
            Key.Left,
            Key.Right,
            Key.Down,
            Key.Tab,
            Key.Back
        };

        private void InitializerSectionHints()
        {
            string path = MainWindow.ProgramData.GetGamedataFromInCatalogs(MainWindow.ProgramData.Gamedata);
            if (path != null)
            {
                string configs = MainWindow.ProgramData.GetConfigOrConfigs(path);

                RecursiveLoadAllSection(configs);
            }
                
        }

        private void RecursiveLoadAllSection(string dirs)
        {
            string[] fls = Directory.GetFiles(dirs);
            foreach(var vF in fls)
            {
                if (MainWindow.ProgramData.GetLastSplash(vF).Split('.')[MainWindow.ProgramData.GetLastSplash(vF).Split('.').Length - 1].ToUpper() == "ltx".ToUpper())
                {
                    HierarchyLtx.LtxFile f = new HierarchyLtx.LtxFile(new FileInfo(vF),MainWindow.ProgramData.Encoding_LTX);
                    foreach (var sect in f.Sections)
                        WHints.Add(sect.Name_Section);
                }
            }

            foreach (var di in Directory.GetDirectories(dirs))
                RecursiveLoadAllSection(di);
        }

        private void InitializerLoadInfoportion()
        {
            //game_information_portions
            string path = MainWindow.ProgramData.GetGamedataFromInCatalogs(MainWindow.ProgramData.Gamedata);
            if (path != null)
            {
                string configs = MainWindow.ProgramData.GetConfigOrConfigs(path);
                RecursiveLoadAllInfoPortion(configs);
            }
        }

        private void RecursiveLoadAllInfoPortion(string dirs)
        {
            string[] vs = Directory.GetFiles(dirs);
            foreach(var vF in vs)
            {
                string text = File.ReadAllText(vF, MainWindow.ProgramData.Encoding_XML);
                if (text.Contains("game_information_portions"))
                {
                    string[] splt = text.Split('\n');
                    for (int i = 0; i < splt.Length; i++)
                    {
                        if (splt[i].Trim().StartsWith("<info_portion id"))
                        {
                            string exp = splt[i].Trim().Split('=')[1].TrimEnd('>').TrimStart('\"').TrimEnd('\"');
                            WHints.Add(exp);
                        }
                    }
                }
                else
                    continue;
            }
            foreach (var d in Directory.GetDirectories(dirs))
                RecursiveLoadAllInfoPortion(d);
        }

        public void InitializerDopHints(string script)
        {
            string[] ep = script.Split('\n');


            if (Properties.Settings.Default.LoadSectHints)
                InitializerSectionHints();

            if (Properties.Settings.Default.LoadInfoportionHints)
                InitializerLoadInfoportion();


            foreach (var v in ep)
            {
                if (v.ToUpper().Contains("function".ToUpper()) && v.Contains("(") && v.Contains(")"))
                {
                    string nameEp = v.Split('(')[0].Replace("function", "").Split(':')[0].Trim();
                    if (WHints_Function.Where(x => x == nameEp).Count() <= 0)
                    {
                        if (script.Contains($"function {nameEp}"))
                            WHints_Function.Add(nameEp);
                    }
                }

                if (v.ToUpper().Contains("local".ToUpper()))
                {
                    string nameEp = v.Split('=')[0].Replace("local", "").Trim();
                    if (WHints_Local.Where(x => x == nameEp).Count() <= 0)
                    {
                        if (script.Contains($"local {nameEp}"))
                        {
                            WHints_Local.Add(nameEp);
                        }
                    }
                }


            }

            List<string> removeFunc = new List<string>();
            List<string> removeLoc = new List<string>();

            foreach (var vF in WHints_Function)
            {
                if (!script.Contains("function " + vF))
                {
                    removeFunc.Add(vF);
                }
            }
            if (WHints_Local != null)
                foreach (var vF in WHints_Local)
                {
                    if (!script.Contains("local " + vF))
                    {
                        WHints_Local.Add(vF);
                    }
                }

            foreach (var vR in removeFunc)
            {
                WHints_Function.Remove(vR);
            }

            foreach (var vR in removeLoc)
            {
                WHints_Local.Remove(vR);
            }

        }

        public void ClearDopHints()
        {
            WHints_Function.Clear();
            WHints_Local.Clear();
        }

    }
}
