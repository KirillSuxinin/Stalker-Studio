using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Stalker_Studio.StalkerClass
{
    public class WordDescription
    {
        public static string PathToFolder = $"{Application.StartupPath}\\Descr";

        public Dictionary<string, LtxLanguage.LtxSectionData> AllWords = new System.Collections.Generic.Dictionary<string, LtxLanguage.LtxSectionData>();

        public WordDescription()
        {
            string[] flsIn = Directory.GetFiles(PathToFolder, "*.ltx");


            
            foreach(var vF in flsIn)
            {
                LtxLanguage.LtxData data = new LtxLanguage.LtxData(File.ReadAllText(vF, Encoding.UTF8));
                foreach(var vSect in data.LtxSectionDatas)
                {
                    AllWords.Add(vSect.NameSection, vSect);
                }
            }


        }

        public class XmlDataParamDescription
        {
            public List<string> Param = new List<string>();
            public List<string> Text = new List<string>();
            public List<bool> UP = new List<bool>();
            private bool Open = false;

            private string DeleteStartSpace(string txt)
            {
                if (txt != null)
                {
                    txt = txt.Replace("\t", "");
                    if (txt.StartsWith(" "))
                    {
                        int Count = 0;
                        for (int i = 0; i < txt.Length; i++)
                        {
                            if (txt[i] == ' ') Count++; else break;
                        }
                        txt = txt.Remove(0, Count);
                        return txt;
                    }
                    else
                    {
                        return txt;
                    }
                }
                return "";
            }

            public XmlDataParamDescription(string txt)
            {
                string[] str = txt.Split('\n');
                bool Open = false;
                for (int i = 0; i < str.Length; i++)
                {
                    string tm = str[i].Replace(" ", "");
                    if (tm.ToUpper().StartsWith("<STRING>"))
                    {
                        Open = true;
                        bool Texts = false;
                        bool Anm = false;

                        for (int j = i; j < str.Length; j++)
                        {
                            string vt = DeleteStartSpace(str[j].Replace(" ", "").Replace("\t", ""));
                            if (vt.ToUpper().StartsWith("<TEXT>"))
                            {
                                string[] vv = str[j].Split('>');
                                string[] vvv = vv[0].Split('<');

                                if (!Texts)
                                {

                                    string text = str[j].Replace($"<{vvv[1]}>", "").Replace($"</{vvv[1]}>", "");
                                    Text.Add(DeleteStartSpace(text));
                                    Texts = true;
                                }
                                else
                                {
                                    string text = str[j].Replace($"<{vvv[1]}>", "").Replace($"</{vvv[1]}>", "");
                                    Text[Text.Count - 1] = DeleteStartSpace(text);
                                }
                            }
                            if (vt.ToUpper().StartsWith("<PARAM"))
                            {
                                string[] vv = str[j].Split('>');
                                string[] vvv = vv[0].Split('<');

                                string[] _vs = vt.Split('/');
                                string end = $"</{_vs[1]}";
                                bool GetD = false;
                                string anm = str[j].Replace($"{end.Replace("/", "")}", "").Replace($"{end}", "");
                                //anm = anm.Replace(vvv[1], "").Replace("<", "").Replace(">", "");
                                if (anm.ToUpper().Contains("UP"))
                                {
                                    try
                                    {
                                        GetD = true;
                                        string dat = anm.Split('>')[0].Split('=')[1];
                                        anm = anm.Split('>')[1];
                                        bool s = bool.Parse(DeleteStartSpace(dat));
                                        UP.Add(s);
                                    }
                                    catch
                                    {
                                        UP.Add(true);
                                    }
                                }
                                else
                                {
                                    anm = anm.Replace(vvv[1], "").Replace("<", "").Replace(">", "");
                                }

                                if (!Anm)
                                {
                                    if (!GetD)
                                        UP.Add(true);
                                    Param.Add(DeleteStartSpace(anm));
                                    Anm = true;
                                }
                                else
                                {
                                    Param[Param.Count - 1] = DeleteStartSpace(anm);
                                }
                            }
                            if (Texts && Anm)
                            {
                                break;
                            }

                        }
                    }
                    if (tm.ToUpper().StartsWith("</STRING>"))
                    {
                        if (Open)
                        {
                            Open = false;
                        }
                    }
                }
                this.Open = Open;

                for (int i = 0; i < Text.Count; i++)
                {
                    Text[i] = Text[i].Replace("\\n", $"{Environment.NewLine}");
                }
            }


        }
    }
}
