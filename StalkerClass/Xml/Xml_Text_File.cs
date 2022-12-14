using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Stalker_Studio.StalkerClass.Xml
{
    public class Xml_Text_File
    {
        public static object[] CheckStringTextXml(string txtXml)
        {

       
            return new object[] { true, null };
        }
        public Xml_Text_File(string textXml,bool IgnoreException = false)
        {
            ParseExpressions(textXml,IgnoreException);
        }

        public Xml_Text_File(FileInfo fXml, Encoding encoding, bool IgnoreException = false)
        {
            string txt = File.ReadAllText(fXml.FullName, encoding);
            _File = fXml;
            ParseExpressions(txt, IgnoreException);
        }

        public FileInfo _File;

        private string OriginalXml;

        private string Header;

        public string head_version
        {
            get
            {
                var regex = new System.Text.RegularExpressions.Regex("<?xml version=\"(.*?)\" encoding=\"(.*?)\"?>");
                var m = regex.Match(Header);
                return m.Groups[1].Value;
            }
            set
            {
                var regex = new System.Text.RegularExpressions.Regex("<?xml version=\"(.*?)\" encoding=\"(.*?)\"?>");
                var m = regex.Match(Header);
                Header = Header.Replace(m.Groups[1].Value, value);
            }
        }

        public string head_encoding
        {
            get
            {
                var regex = new System.Text.RegularExpressions.Regex("<?xml version=\"(.*?)\" encoding=\"(.*?)\"");
                var m = regex.Match(Header);
                return m.Groups[2].Value;
            }
            set
            {
                var regex = new System.Text.RegularExpressions.Regex("<?xml version=\"(.*?)\" encoding=\"(.*?)\"");
                var m = regex.Match(Header);
                Header = Header.Replace(m.Groups[2].Value, value);
            }
        }


        public List<Xml_Text_String> ExpressionsBlocks = new List<Xml_Text_String>();

        private void ParseExpressions(string txtxml, bool ignoreException = false)
        {
            string[] txt = txtxml.Replace("\r","").Split('\n');
            bool OpenFile = false;
            bool CloseFile = false;
            OriginalXml = txtxml;
            
            for(int cursor = 0; cursor < txt.Length; cursor++)
            {
                //<string_table>
                if (txt[cursor].ToUpper().Trim().StartsWith("<string_table>".ToUpper()))
                {
                    if (OpenFile && !ignoreException)
                        throw new Exception($"Файл xml имеет больше одного открывающего тэга![{txt[cursor]}]-[{cursor}]");
                    OpenFile = true;
                    continue;
                }
                if (txt[cursor].ToUpper().Trim().StartsWith("<?xml".ToUpper()))
                {

                    Header = txt[cursor];
                    continue;
                    
                }
                if (txt[cursor].ToUpper().Trim().StartsWith("</string_table>".ToUpper()))
                {
                    if(CloseFile && !ignoreException)
                        throw new Exception($"Файл xml имеет больше одного закрывающего тэга![{txt[cursor]}]-[{cursor}]");
                    CloseFile = true;
                    continue;
                }
                if (txt[cursor].ToUpper().Trim().StartsWith("<string".ToUpper()))
                {
                    string block = txt[cursor];
                    for(int i = cursor+1; i < txt.Length; i++)
                    {
                        if (txt[i].ToUpper().StartsWith("<string".ToUpper()))
                        {
                            if (!ignoreException)
                                throw new Exception($"Файл xml имеет больше одного окрывающего блока![{txt[i]}]-[{i}]");
                        }
                        else if (txt[i].ToUpper().StartsWith("</string"))
                            break;
                        else
                            block += txt[i] + "\n";
                    }

                    Xml_Text_String _block = new Xml_Text_String(block);
                    if (_block.Id != null && _block.Text != null)
                        ExpressionsBlocks.Add(_block);

                }
            }

            string er = "";
            if (!OpenFile)
            {
                er += "Нету открывающего тэга![line: 0]\n";
            }
            else if (!CloseFile)
            {
                er += "Нету закрывающего тэга\n";
            }
            if (!string.IsNullOrWhiteSpace(er) && !ignoreException)
                throw new Exception($"Ошибка!: {er}");
        }


        public override string ToString()
        {
            string txt = $"{Header}{Environment.NewLine}";
            txt += "<string_table>"+Environment.NewLine;

            foreach(var v in ExpressionsBlocks)
            {
                txt += $"\t<string id=\"{v.Id}\">" + Environment.NewLine;
                txt += "\t\t" + $"<text>{v.Text}</text>" + Environment.NewLine;
                txt += "\t</string>" + Environment.NewLine;

            }
            txt += "</string_table>";

            return txt;
        }
    }

    public class Xml_Text_String
    {
        public string ExpressionBlock;

        public string Id
        {
            get
            {
                var regex = new System.Text.RegularExpressions.Regex("<string id=\"(.*?)\">");
                var m = regex.Match(ExpressionBlock);
                return m.Groups[1].Value;
            }
            set
            {
                var regex = new System.Text.RegularExpressions.Regex("<string id=\"(.*?)\">");
                var m = regex.Match(ExpressionBlock);
                ExpressionBlock = ExpressionBlock.Replace(m.Groups[1].Value, value);
            }
        }

        public string Text
        {
            get
            {
                var regex = new System.Text.RegularExpressions.Regex("<text>(.*?)</text>");
                var m = regex.Match(ExpressionBlock);
                return m.Groups[1].Value;
            }
            set
            {
                var regex = new System.Text.RegularExpressions.Regex("<text>(.*?)</text>");
                var m = regex.Match(ExpressionBlock);
                ExpressionBlock = ExpressionBlock.Replace(m.Groups[1].Value, value);
            }
        }

        public Xml_Text_String(string expressionBlockring )
        {
            ExpressionBlock = expressionBlockring;
        }

        public Xml_Text_String(string id,string value)
        {
            ExpressionBlock = $"\n<string id=\"{id}\">\n<text>{value}</text>\n</string>";
        }

    }
}
