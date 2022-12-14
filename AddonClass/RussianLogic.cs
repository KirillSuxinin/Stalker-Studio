using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stalker_Studio.AddonClass
{
    public static class RussianLogic
    {



        public static string GetFirstSection(string pathToFile)
        {

            string[] file = System.IO.File.ReadAllLines(pathToFile, Stalker_Studio.MainWindow.ProgramData.Encoding_LTX);
            for(int i = 0; i < file.Length; i++)
            {
                if (file[i].Trim().StartsWith("[") && file[i].Contains("]"))
                {
                    return file[i].Trim().Split(':')[0].Replace("[", "").Replace("]", "");
                }
            }
            
            return Stalker_Studio.MainWindow.ProgramData.GetLastSplash(pathToFile);
        }

        public static string GetInvNameFirstSection(string PathToFile)
        {
            if (Properties.Settings.Default.IndexXmlBrowser)
                MainWindow.ProgramData.MainWinThread.Title = "Stalker Studio (Инициализация обозревателя)";
            string text = MainWindow.ProgramData.GetLastSplash(PathToFile);
            if (PathToFile.EndsWith(".ltx"))
            {
                if (System.IO.File.ReadAllText(PathToFile, MainWindow.ProgramData.Encoding_LTX).Contains("inv_name"))
                {
                    try
                    {
                        StalkerClass.HierarchyLtx.LtxFile fls = new StalkerClass.HierarchyLtx.LtxFile(new System.IO.FileInfo(PathToFile), MainWindow.ProgramData.Encoding_LTX);
                        foreach (var vSect in fls.Sections)
                        {
                            if (vSect.Parametrs.Where(x => x.Name_Parametr == "inv_name").Count() > 0)
                            {
                                string link = vSect.Parametrs.Where(x => x.Name_Parametr == "inv_name").First().Value_Parametr;

                                string vs = MainWindow.ProgramData.GetValueByLinkText(link);

                                text = vs;
                                if (string.IsNullOrWhiteSpace(vs))
                                    text = link;
                                break;
                                // text = MainWindow.ProgramData.GetLastSplash(PathToFile);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (!Properties.Settings.Default.IndexXmlBrowser)
                    return text;
                if (System.IO.File.ReadAllText(PathToFile, MainWindow.ProgramData.Encoding_XML).Contains("string") && (MainWindow.ProgramData.GetLastSplash(PathToFile).ToUpper().Contains("task".ToUpper()) || MainWindow.ProgramData.GetLastSplash(PathToFile).ToUpper().Contains("dialogs".ToUpper())))
                {
                    StalkerClass.Xml.Xml_Text_File fls = new StalkerClass.Xml.Xml_Text_File(new System.IO.FileInfo(PathToFile), MainWindow.ProgramData.Encoding_XML, true);
                    if (fls != null && fls.ExpressionsBlocks != null && fls.ExpressionsBlocks.Count > 0)
                        text += " (" + fls.ExpressionsBlocks[0].Id + " - " + fls.ExpressionsBlocks[fls.ExpressionsBlocks.Count - 1].Id + ")";
                }
            }

            return text;
        }

        public static string GetRussianName(string pathToFolder)
        {
            string vs = MainWindow.ProgramData.GetLastSplash(pathToFolder.Trim('\\'));

            if (pathToFolder.ToUpper().EndsWith("config".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs".ToUpper()))
            {
                return "Конфиги";
            }

            if (pathToFolder.ToUpper().EndsWith("config\\scripts".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\scripts".ToUpper()))
            {
                return "Скрипты";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\ui".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\ui".ToUpper()))
            {
                return "Разметка";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\models".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\models".ToUpper()))
            {
                return "Ltx модели";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\weapons".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\weapons".ToUpper()))
            {
                return "Оружия";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\mp".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\mp".ToUpper()))
            {
                return "Сетевая Игра";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\text".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\text".ToUpper()))
            {
                return "Текст";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\weathers".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\weathers".ToUpper()))
            {
                return "Конфиги погоды";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\prefetch".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\prefetch".ToUpper()))
            {
                return "Ссылки Include";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\creatures".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\creatures".ToUpper()))
            {
                return "Существа";
            }
            else if (pathToFolder.ToUpper().EndsWith("config\\gameplay".ToUpper()) || pathToFolder.ToUpper().EndsWith("configs\\gameplay".ToUpper()))
            {
                return "Основное";
            }

            string file = $"{System.Windows.Forms.Application.StartupPath}\\Pather\\Russian.ltx";
            LtxLanguage.LtxData dat = new LtxLanguage.LtxData(System.IO.File.ReadAllText(file, Encoding.UTF8));
            foreach(var v in dat.LtxSectionDatas)
            {
                foreach(var dop in v.Datas)
                {
                    if (pathToFolder.ToUpper().EndsWith(dop.NameParametr.ToUpper()) || pathToFolder.ToUpper().EndsWith(dop.NameParametr.ToUpper().Replace("CONFIG","CONFIGS")))
                    {
                        return dop.DataParametr;
                    }
                }
            }

            return vs;
        }
    }
}
