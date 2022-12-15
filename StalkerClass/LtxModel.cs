using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Stalker_Studio.Common;

namespace Stalker_Studio.StalkerClass
{

    /// <summary>
    /// Класс работы с Ltx файлом 
    /// Считается сериализуемым,
    /// Результат сериализации: Text
    /// Результатом десериализации: Sections и подчиненные элементы
    /// </summary>
    public class LtxModel: TextFileModel
    {
        private List<string> OtherStart = new List<string>();
        public ObservableCollection<LtxSection> _sections = new ObservableCollection<LtxSection>();

        public LtxModel() : base()
        {
            Initialize();
        }
        public LtxModel(FileInfo file, Encoding encoding) : base(file, encoding)
        {
            Initialize();
        }
        public LtxModel(string fullName, Encoding encoding) : base(fullName)
        {
            _encoding = encoding;
            Initialize();
        }
        public LtxModel(string txt) : base(txt)
        {
            Initialize();
        }

        public override IEnumerable<IHierarchical> Nodes
        {
            get
            {
                UpdateSerialization();// обязательно для поддержки автосериализации
                return _sections as IEnumerable<IHierarchical>;
            }
        }

        //public ObservableCollection<LtxSection> Sections
        //{
        //    set
        //    {
        //        _sections = value;
        //        OnPropertyChanged();

        //        // Поддержка автосериализации
        //        // Поменялись секции, значит надо обновить другие данные, которые получаем с помощью сериализации из секций, то есть текст
        //        SerializationState = SerializationState.NeedSerialize;
        //    }
        //}

        public override void Load()
        {
            StreamReader streamReader = new StreamReader(_fullName, _encoding);
            _text = streamReader.ReadToEnd();
            Deserialize(_text);
        }
        public override void Save()
        {
            StreamWriter streamWriter = new StreamWriter(_fullName, false, _encoding);
            streamWriter.Write(Serialize());
        }

        #region Serialization

        protected override void OnSetSerializationState()
        {
            base.OnSetSerializationState(); // обязательно

            // Добавляем действия, в зависимости от особенностей класса
            if (SerializationState == SerializationState.NeedDeserialize)
                // Так как очередная сериализация должна быть выполнена при обращении к свойству Sections, 
                //  вызываем OnPropertyChanged("Sections") что бы справоцировать 
                //  это обращение из элементов интерфейса (и не только), к которым привязано свойство Sections
                OnPropertyChanged("Sections");
        }
        protected override string ConvertToString()
        {
            return ConvertToStringParams();
        }

        public override void Deserialize(string serializedString)
        {
            string[] lines = serializedString.Split('\n');

            bool v = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if ((lines[i].Trim().StartsWith(";") && !v) || lines[i].Trim().StartsWith("#"))
                {
                    OtherStart.Add(lines[i]);
                }

                int indexComment = lines[i].IndexOf(';');
                int indexStartNewSection = lines[i].IndexOf("[");

                if (indexStartNewSection != -1 && (indexComment > indexStartNewSection || indexComment == -1))
                {
                    v = true;
                    
                    int startLine = i;
                    i++;

                    List<string> sectionLines = new List<string>();

                    for (i++; i < lines.Length; i++)
                    {
                        indexComment = lines[i].IndexOf(';');
                        indexStartNewSection = lines[i].IndexOf("[");

                        if (indexStartNewSection != 1 && (indexComment > indexStartNewSection || indexComment == -1))
                            break;

                        sectionLines.Add(lines[i]);
                    }

                    _sections.Add(new LtxSection(sectionLines, startLine, i - 1));
                }
            }
        }

        #endregion

        #region Определения Hierarchical

        public override IEnumerable<Type> GetNodeTypes() { return new Type[] { typeof(LtxSection) }; }

        public override IHierarchical this[int index]
        {
            get { return Nodes.ElementAt(index); }
            set
            {
                CheckNodeAndThrowException(value);

                if (_sections == null)
                    Load();

                _sections[index] = value as LtxSection;
                OnPropertyChanged("Nodes");
            }
        }

        protected override void OnAddingNode(IHierarchical node)
        {
            if (_sections == null)
                Load();
            _sections.Add(node as LtxSection);
        }
        protected override void OnAddingNodeAt(IHierarchical node, int index)
        {
            if (_sections == null)
                Load();
            _sections.Insert(index, node as LtxSection);
        }
        protected override void OnRemoveNode(IHierarchical node, bool recursively)
        {
            _sections.Remove(node as LtxSection);
        }
        protected override void OnRemoveNodeAt(int index)
        {
            _sections.RemoveAt(index);
        }

        #endregion


        public string ConvertToStringParams(bool UseDescrHeir = false)
        {
            string ltx = "";

            foreach (string v in OtherStart)
            {
                ltx += v + Environment.NewLine;
            }

            for (int i = 0; i < _sections.Count; i++)
            {
                string header = $"[{_sections[i].Name}]";
                if (_sections[i].Name != null)
                {
                    header += ":" + _sections[i].Name;
                }
                if (_sections[i].Comment != null)
                {
                    _sections[i].Comment = _sections[i].Comment.TrimStart(';');
                    _sections[i].Comment = ";" + _sections[i].Comment;

                    if (!header.ToUpper().Contains(_sections[i].Comment.ToUpper()) && UseDescrHeir)
                        header += _sections[i].Comment;
                    //Console.WriteLine(header + " | " + Sections[i].Name_Section + " | " + Sections[i].Description_Section);
                }
                header += Environment.NewLine;
                string body = "";
                for (int j = 0; j < _sections[i].Nodes.Count(); j++)
                {
                    body += _sections[i].Nodes.ElementAt(j).ToString() + Environment.NewLine;
                }
                ltx += header + body;
                //  Console.WriteLine(header);
            }

            return ltx.Replace("\r", "").TrimEnd('\n').TrimEnd('\r');
        }

        public void SaveFile(string pathFile = null, Encoding enc = null, bool UseDescrHeir = false)
        {
            string fls = "";
            if (pathFile != null)
                fls = pathFile;
            else
                throw new System.IO.IOException($"Stalker_Studio.StalkerClass.LtxModel - PathFile не указан!");

            if (enc == null)
                enc = MainWindow.ProgramData.Encoding_LTX;

            File.WriteAllText(fls, ConvertToStringParams(UseDescrHeir), enc);
        }

        void Initialize()
        {
            AutoSerialization = true; // включаем автосериализацию

            _sections.CollectionChanged +=
                (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                => SerializationState = SerializationState.NeedSerialize;
        }
    }

    public class LtxSection : TextObject<LtxParameter>
    {
        protected ObservableCollection<string> _parents = new ObservableCollection<string>();

        public LtxSection() : base() { }
        public LtxSection(string name) : base(name) { }
        public LtxSection(string name, string[] parents) : base(name)
        {
            _parents = new ObservableCollection<string>(parents);
        }
        public LtxSection(int startIndex, int endIndex, int startLineIndex, int endLineIndex, string name, string[] parents) : base(startIndex, endIndex, startLineIndex, endLineIndex, name)
        {
            _parents = new ObservableCollection<string>(parents);
        }
        public LtxSection(IEnumerable<string> lines, int startLineIndex, int endLineIndex) : base(0, 0, startLineIndex, endLineIndex)
        {
            if (lines.Count() > endLineIndex - startLineIndex)
                throw new IOException($"Ошибка десериализации секции LTX {startLineIndex}-{endLineIndex}. Не соответствие диапозона строк секции сериализованным строкам");

            string current = "";

            for (int i = 0; i < lines.Count(); i++)
            {
                current = lines.ElementAt(i);

                if (_startIndex == -1) // если начало [ до комментария не найдено, то ищем
                {
                    int indexComment = current.IndexOf(';');
                    int indexStartName = current.IndexOf('[');
                    int indexEndName = current.IndexOf(']');

                    if (indexStartName != -1 && (indexComment > indexStartName || indexComment == -1))
                    {
                        if (indexEndName == -1 || (indexComment < indexEndName && indexComment != -1))
                            throw new IOException($"Ошибка десериализации секции LTX {startLineIndex}-{endLineIndex}. Не найден закрывающий символ имени секции");
                       
                        _name = current.Substring(indexStartName, indexEndName - indexStartName).Trim();
                        _startIndex = indexStartName;

                        int indexParentsStart = current.IndexOf(':');
                        if (indexParentsStart != -1 && (indexComment > indexParentsStart || indexComment == -1))
                        {
                            string parents = current.Substring(
                                indexParentsStart, 
                                (indexComment == -1 ? current.Length - 1 : indexComment) - indexParentsStart
                                );

                            _parents = new ObservableCollection<string>(parents.Trim().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }

                    if (indexComment != -1)
                        _comment = current.Substring(indexComment, current.Length - indexComment);
                }
                else
                    _nodes.Add(new LtxParameter(current));
            }

            if (_startIndex == -1) 
                throw new IOException($"Ошибка десериализации секции LTX {startLineIndex}-{endLineIndex}. Не найдено начало секции");

            _endIndex = current.Length - 1;
        }


        public bool IsHeir => _parents.Count != 0;

        public ObservableCollection<string> Parents
        {
            get { return _parents; }
            set
            {
                _parents = value;
                OnPropertyChanged();
            }
        }
    }

    public class LtxParameter : TextObject<LtxSection, LtxParameter>
    {
        public bool IsValue
        {
            get
            {
                return _name != null && Value_Parametr != null;
            }
        }

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
        public LtxParameter(string Expression)
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
                string name = Expression.Split(';')[0].Split('=')[0].Trim();

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

                _name = name;
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
                _name = Expression.Trim();
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

                for (int i = 0; i < Expression.Split(';')[0].Length; i++)
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

                string _val = $"{_name}{Space_IN}={Space_OUT}{Value_Parametr}";
                if (Desription_Parametr != null)
                    _val += $"{Space_By};" + Desription_Parametr;
                return _val;
            }
            else
            {
                string _val = $"{_name}";
                if (Desription_Parametr != null)
                    _val += Space_By + ";" + Desription_Parametr;
                return _val;
            }

        }
    }
}
