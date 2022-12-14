using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Stalker_Studio.Json
{
    public static class FileImport
    {
        public static void WriteInFileJson(string pathFile, object jsonObj)
        {
            string pathJson = pathFile;
            string j = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            JsonTextWriter wr = new JsonTextWriter(new StreamWriter(pathJson));
            wr.WriteRaw(j);
            wr.Flush();
            wr.Close();
        }

        public static void WriteInFileJson(string pathFile, string textJson)
        {
            string pathJson = pathFile;
            string j = textJson;
            JsonTextWriter wr = new JsonTextWriter(new StreamWriter(pathJson));

            wr.WriteRaw(j);
            wr.Flush();
            wr.Close();
        }
    }

    public class Rootobject
    {
        public Localization Localization { get; set; }
        public Application Application { get; set; }
        public Dialogues Dialogues { get; set; }
        public Tags Tags { get; set; }
    }

    public class Localization
    {
        public string CurrentLocalization { get; set; }
        public string SearchPattern { get; set; }
        public string FilesRelativePath { get; set; }
        public string FileFormat { get; set; }
        public Locale[] Locales { get; set; }
    }

    public class Locale
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public string LanguageCode { get; set; }
    }

    public class Application
    {
        public string GameDirectory { get; set; }
        public bool SaveNodesPosition { get; set; }
        public string NodesPositionFilePath { get; set; }
        public bool ShowChooseRootDirectoryDialogOnStartup { get; set; }
    }

    public class Dialogues
    {
        public string SearchPattern { get; set; }
        public string FilesRelativePath { get; set; }
        public Availableproperty[] AvailableProperties { get; set; }
    }

    public class Availableproperty
    {
        public string PropertyType { get; set; }
        public string Name { get; set; }
        public bool MultipleUse { get; set; }
    }

    public class Tags
    {
        public string DialogList { get; set; }
        public string Dialog { get; set; }
        public string PhraseList { get; set; }
        public string Phrase { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string String { get; set; }
        public string NextConnection { get; set; }
    }

}
