using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Stalker_Studio.StalkerClass.HierarchyLtx
{
    public class LtxFile
    {
        public FileInfo File;
        private List<string> OtherStart = new List<string>();
        public LtxFile(FileInfo file,Encoding _encoding)
        {
            this.File = file;
            try
            {

                string[] lines = System.IO.File.ReadAllLines(File.FullName, _encoding);
                bool v = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim().StartsWith(";") && !v)
                    {

                        OtherStart.Add(lines[i]);
                    }

                    if (lines[i].Trim().StartsWith("["))
                    {
                        v = true;
                        Ltx_Section sect = new Ltx_Section();
                        string _nameSection = lines[i].Split('[')[1].Split(']')[0];
                        // Console.WriteLine(_nameSection);


                        sect.Name_Section = _nameSection;

                        if (lines[i].Contains(":") && lines[i].Split(']').Length >= 1 && lines[i].Split(']')[1].Split(':').Length >= 1)
                        {

                            sect.Heir_Section = lines[i].Split(']')[1].Split(':')[1];
                        }
                        if (lines[i].Contains(";"))
                        {
                            sect.Description_Section = lines[i].Split(';')[1];
                        }

                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            if (lines[j].Trim().StartsWith("["))
                                break;
                            else
                                sect.Parametrs.Add(new Ltx_Parametr(lines[j]));
                        }
                        Sections.Add(sect);

                    }

                }

            }
            catch
            {

            }
        }

        public List<Ltx_Section> Sections = new List<Ltx_Section>();

        public LtxFile(string txt)
        {
            string[] lines = txt.Split('\n');

            bool v = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if ((lines[i].Trim().StartsWith(";") && !v) || lines[i].Trim().StartsWith("#"))
                {
                    OtherStart.Add(lines[i]);
                }

                if (lines[i].Trim().StartsWith("["))
                {
                    v = true;
                    Ltx_Section sect = new Ltx_Section();
                    string _nameSection = lines[i].Split('[')[1].Split(']')[0];


                    sect.Name_Section = _nameSection;
                    if (lines[i].Split(']')[1].Contains(":"))
                    {
                        sect.Heir_Section = lines[i].Split(']')[1].Split(':')[1].Split(';')[0];
                    }
                    if (lines[i].Contains(";"))
                    {
                        sect.Description_Section = lines[i].Split(';')[1];
                    }

                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        if (lines[j].Trim().StartsWith("["))
                            break;
                        else
                            sect.Parametrs.Add(new Ltx_Parametr(lines[j]));
                    }
                    Sections.Add(sect);

                }

            }
        }



        public string ToString(bool UseDescrHeir = false)
        {
            string ltx = "";

            foreach(string v in OtherStart)
            {
                ltx += v + Environment.NewLine;
            }

            for(int i = 0; i < Sections.Count; i++)
            {
                string header = $"[{Sections[i].Name_Section}]";
                if (Sections[i].Heir_Section != null)
                {
                    header += ":" + Sections[i].Heir_Section;
                }
                if (Sections[i].Description_Section != null)
                {
                    Sections[i].Description_Section = Sections[i].Description_Section.TrimStart(';');
                    Sections[i].Description_Section = ";" + Sections[i].Description_Section;

                    if (!header.ToUpper().Contains(Sections[i].Description_Section.ToUpper()) && UseDescrHeir)
                        header += Sections[i].Description_Section;
                    //Console.WriteLine(header + " | " + Sections[i].Name_Section + " | " + Sections[i].Description_Section);
                }
                header += Environment.NewLine;
                string body = "";
                for(int j = 0; j < Sections[i].Parametrs.Count; j++)
                {
                    body += Sections[i].Parametrs[j].ToString() + Environment.NewLine;
                }
                ltx += header + body;
              //  Console.WriteLine(header);
            }


            return ltx.Replace("\r", "").TrimEnd('\n').TrimEnd('\r');
        }

        public void SaveFile(string pathFile = null,Encoding enc = null,bool UseDescrHeir = false)
        {
            string fls = "";
            if (pathFile != null)
                fls = pathFile;
            else if (File != null)
                fls = File.FullName;
            else
                throw new System.IO.IOException($"Stalker_Studio.StalkerClass.HierarchyLtx.LtxFile - PathFile не указан!");

            if (enc == null)
                enc = MainWindow.ProgramData.Encoding_LTX;

            System.IO.File.WriteAllText(fls, ToString(UseDescrHeir), enc);
        }
    }

    public class Ltx_Section
    {
        public List<Ltx_Parametr> Parametrs = new List<Ltx_Parametr>();

        public string Name_Section;
        public string Heir_Section;

        public bool IsHeir
        {
            get
            {
                return Heir_Section != null;
            }
        }
        public string Description_Section;
    }

    public class Ltx_Parametr
    {

        public bool IsValue
        {
            get
            {
                return Name_Parametr != null && Value_Parametr != null;
            }
        }

        public string Name_Parametr;
        public string Value_Parametr;
        public string Desription_Parametr;

        public string Space_IN;
        public string Space_OUT;

        private bool COMMENT = false;

        /// <summary>
        /// Между = SPACE ;
        /// </summary>
        public string Space_By;

        //
        // inv_name     =    5;   setka bitch
        //
        public Ltx_Parametr(string Expression)
        {
            if (Expression.Trim().StartsWith(";"))
            {
                Desription_Parametr = Expression;
                COMMENT = true;
                return;
            }


            if (Expression.Contains("="))
            {
                //parametr with =
                //get name parametr
                string _name = Expression.Split(';')[0].Split('=')[0].Trim();

                string _description = null;
                foreach (var vDescr in Expression.Split(';'))
                {
                    if (vDescr != Expression.Split(';')[0])
                    {
                        _description += vDescr;
                    }
                    else
                        continue;
                }
                bool withDescr = Expression.Contains(';');
                //parse value
                string _value = Expression.Split('=')[1].Split(';')[0].Trim();

                Name_Parametr = _name;
                Value_Parametr = _value;
                if (withDescr)
                    Desription_Parametr = _description;
                //get count Space

                for (int i = 0; i < Expression.Split('=')[0].Length; i++)
                {
                    if (Expression.Split('=')[0][i] == '\t' || Expression.Split('=')[0][i] == ' ')
                    {
                        Space_IN += Expression.Split('=')[0][i];
                    }
                }

                
                for (int i = 0; i < Expression.Split('=')[1].Length; i++)
                {

                    if (Expression.Split('=')[1][i] == ' ' || Expression.Split('=')[1][i] == '\t')
                    {
                        Space_OUT += Expression.Split('=')[1][i];
                    }
                    if (!string.IsNullOrWhiteSpace(_value) && Expression.Split('=')[1][i] == _value[0])
                        break;
                }



                for (int i = Expression.Split('=')[1].LastIndexOf(Value_Parametr); i < Expression.Split('=')[1].Length; i++)
                {

                    if (Expression.Split('=')[1][i] == ';')
                        break;
                    if (Expression.Split('=')[1][i] == '\t' || Expression.Split('=')[1][i] == ' ')
                        Space_By += Expression.Split('=')[1][i];


                }

            }
            else
            {
                //no =
                Name_Parametr = Expression.Trim();
                Value_Parametr = null;
                string _description = null;
                foreach (var vDescr in Expression.Split(';'))
                {
                    if (vDescr != Expression.Split(';')[0])
                    {
                        _description += vDescr;
                    }
                    else
                        continue;
                }

                for(int i = 0; i < Expression.Split(';')[0].Length; i++)
                {
                    if (Expression.Split(';')[0][i] == ' ')
                    {
                        Space_By += " ";
                    }
                    if (Expression.Split(';')[0][i] == '\t')
                        Space_By += "\t";
                }

                if (Expression.Contains(';'))
                    Desription_Parametr = _description;
            }


        }

        public override string ToString()
        {
            if (COMMENT)
            {
                return Desription_Parametr;
            }
            if (IsValue)
            {

                string _val = $"{Name_Parametr}{Space_IN}={Space_OUT}{Value_Parametr}";
                if (Desription_Parametr != null)
                    _val += $"{Space_By};" + Desription_Parametr;
                return _val;
            }
            else
            {
                string _val = $"{Name_Parametr}";
                if (Desription_Parametr != null)
                    _val += Space_By + ";" + Desription_Parametr;
                return _val;
            }

        }
    }
}
