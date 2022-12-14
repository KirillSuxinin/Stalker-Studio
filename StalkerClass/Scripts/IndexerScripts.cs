using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Stalker_Studio.StalkerClass.Scripts
{
    public class IndexerScripts
    {
        private string Script;
        private FileInfo _File;

        public List<string> _Functions = new List<string>();
        public List<string> _GlobalVariable = new List<string>();

        public IndexerScripts(string script)
        {
            Script = script;
            Initializer();
        }

        public IndexerScripts(FileInfo fls,Encoding _encoding)
        {
            _File = fls;
            Script = File.ReadAllText(fls.FullName, _encoding);
            Initializer();
        }

        private void Initializer()
        {
            //TODO: Сделать загрузку глобальных переменных и таблиц
            //TODO: Сделать отдельной функцией получения списка локальных переменных в опр. функции
            string[] scriptParse = Script.Split('\n');
            foreach(var vE in scriptParse)
            {
                if (vE.Trim().ToUpper().Contains("function".ToUpper()) && vE.Contains("("))
                {
                    string nameFunct = vE.Split('(')[0].Replace("function", "");

                    _Functions.Add(nameFunct.Trim());

                    /*
                                     var regex = new System.Text.RegularExpressions.Regex("<text>(.*?)</text>");
                var m = regex.Match(ExpressionBlock);
                return m.Groups[1].Value;*/
                }
            }
        }
    }
}
