using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Stalker_Studio.StalkerClass.Xml
{
    public class XML_Characters
    {
        private class ElementDouble
        {
            public string Key;
            public string Value;
        }
        public string PathFile;

        private Encoding _Encoding;
        /// <summary>
        /// Обрабатывает текст Xml
        /// </summary>
        /// <param name="textXml">Сам текст</param>
        /// <returns>Текст без лишних символов</returns>
        private string HandlerXml(string[] textXml)
        {
            string _text = "";
            string tmp_id = "";
            for (int j = 0; j < textXml.Length; j++)
            {
                _text += textXml[j] + "\n";

                
                if (textXml[j].Trim().StartsWith("<!--") && textXml[j].Trim().EndsWith("-->"))
                {
                    _text = _text.Replace(textXml[j], "");

                    for(int i = j; i < textXml.Length; i++)
                    {
                        if (textXml[i].Trim().StartsWith("<specific_character"))
                        {
                            tmp_id = textXml[i].Split('=')[1].Split('\"')[1];
                            break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(tmp_id))
                        Comments.Add(new ElementDouble() { Key = tmp_id, Value = textXml[j] });
                    
                }

                if (textXml[j].Trim().StartsWith("<specific_character"))
                {
                    tmp_id = textXml[j].Split('=')[1].Split('\"')[1];
                }

                if (textXml[j].Trim().StartsWith("#include"))
                {
                    
                    bool containInSuplies = false;
                    for(int d = j+1; d < textXml.Length; d++)
                    {
                        if (textXml[d].Trim().Contains("</specific_character>") || textXml[d].Trim().Contains("<specific_character>"))
                            break;
                        if (textXml[d].Trim().Contains("supplies"))
                        {
                            containInSuplies = true;
                            break;
                        }
                    }
                    if (!containInSuplies)
                    {
                        Includes.Add(new ElementDouble() { Key = tmp_id, Value = textXml[j] });
                    }
                }

            }
            _text = Regex.Replace(_text, @"\b\<!--.+?\-->", "");


            return _text;
        }
        private List<ElementDouble> Includes = new List<ElementDouble>();
        private List<ElementDouble> Comments = new List<ElementDouble>();

        private static Json.XML.Rootobject HandlerJsonXmlObject(Json.XML.Rootobject obj)
        {
            foreach(var vElement in obj.xml.specific_character)
            {
                if (vElement.supplies != null)
                    vElement.supplies = vElement.supplies.Replace("\\n", "\\n\n").Replace("[spawn]", "\n\t\t\t[spawn]").Replace("#include","\n#include").Replace("</supplies>", "\n\t</supplies>");
            }
            return obj;
        }

        public Json.XML.Rootobject Object_Charactes;

        public XML_Characters(FileInfo file,Encoding encoding)
        {
            this.PathFile = file.FullName;
            this._Encoding = encoding;
            XmlDocument docx = new XmlDocument();
            string textXml = HandlerXml(File.ReadAllLines(file.FullName, encoding));
            docx.LoadXml(textXml);
            string json = JsonConvert.SerializeXmlNode(docx);
            Json.XML.Rootobject root = JsonConvert.DeserializeObject<Json.XML.Rootobject>(json);
            Object_Charactes = HandlerJsonXmlObject(root);


            //json = JsonConvert.SerializeObject(jS, Newtonsoft.Json.Formatting.Indented);
            // File.WriteAllText("testjson.json", json,encoding);
            // XmlDocument inDoc = JsonConvert.DeserializeXmlNode(json);
            // inDoc.Save("test.xml");

            // To convert an XML node contained in string xml into a JSON string   
            //  XmlDocument doc = new XmlDocument();
            //  doc.LoadXml(xml);
            // string jsonText = JsonConvert.SerializeXmlNode(doc);

            // To convert JSON text contained in string json into an XML node
            // XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
        }

        public void SaveXml(string filename = null,Encoding enc = null)
        {
            if (filename == null)
                filename = PathFile;
            if (enc == null)
                enc = _Encoding;

            string jsonT = JsonConvert.SerializeObject(Object_Charactes);
            XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(jsonT);

            if (File.Exists("tempXML.tmp"))
                File.Delete("tempXML.tmp");

            // xmlDoc.Save("tempXML.tmp");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = enc;

            XmlWriter xmlwriter = XmlWriter.Create("tempXML.tmp", settings);
            xmlDoc.Save(xmlwriter);
            xmlwriter.Close();
            xmlwriter.Dispose();
            string[] linesReader = File.ReadAllLines("tempXML.tmp", enc);

            List<string> linesXml = new List<string>();
            if (linesReader.Where(x => x.Contains("<?xml version")).Count() <= 0)
                linesXml.Add("<?xml version='1.0' encoding=\"windows - 1251\"?>");
            string tmp_id = "";
            for(int i = 0; i < linesReader.Length; i++)
            {
                if (linesReader[i].Trim().StartsWith("<specific_character"))
                {
                    tmp_id = linesReader[i].Split('=')[1].Split('\"')[1];
                    if (Comments.Where(x => x.Key == tmp_id).Count() > 0)
                    {
                        linesXml.Add(Comments.Where(x => x.Key == tmp_id).First().Value);
                    }
                }
                if (linesReader[i].Trim().StartsWith("</specific_character>"))
                {
                    if (!string.IsNullOrWhiteSpace(tmp_id) && Includes.Where(x => x.Key == tmp_id).Count() > 0)
                    {
                        foreach(var vEl in Includes.Where(x => x.Key == tmp_id))
                        {
                            linesXml.Add(vEl.Value);
                        }
                    }
                }
                linesXml.Add(linesReader[i]);

            }
            try
            {
                if (File.Exists("tempXML.tmp"))
                    File.Delete("tempXML.tmp");
            }
            catch
            {

            }

            File.WriteAllLines(filename, linesXml.ToArray(), enc);



        }

    }
}
