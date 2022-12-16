using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Stalker_Studio.Common
{

    /// <summary>
    /// Класс работы с элементами файловой системы
    /// </summary>
    public abstract class FileSystemNode : Hierarchical
    {
        protected string _fullName = "";

        public FileSystemNode() : base() { }
        public FileSystemNode(string path) : base()
        {
            _fullName = path;
        }
        public FileSystemNode(FileSystemInfo file) : base()
        {
            _fullName = file.FullName;
        }

        // если Nodes возвращает null - значит документ не загружен
        public override IEnumerable<IHierarchical> Nodes
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Возвращает полное имя
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged();
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Возвращает имя из полного имени
        /// </summary>
        public string Name
        {
            get {  return Path.GetFileNameWithoutExtension(_fullName); }
            set
            {
                string normalName = value.Replace(Path.DirectorySeparatorChar,' ');
                if(value.Length - normalName.Length > 1)
                    throw new IOException($"Ошибка установки имени {value}. Строка содержит недопустимые символы");

                int lastIndex = _fullName.LastIndexOf(Path.DirectorySeparatorChar);
                FullName = _fullName.Substring(0, lastIndex) + value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Возвращает информацию
        /// </summary>
        public abstract FileSystemInfo Info { get; }

        /// <summary>
        /// Устанавливает FullName и вызывает Load()
        /// </summary>
        public void Load(string fullName)
        {
            FullName = fullName;
            Load();
        }
        /// <summary>
        /// Основная процедура загрузки данных из файла в данный объект, FullName должен быть заполнен
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// Основная процедура сохранения данных из данного объекта в файл, FullName должен быть заполнен
        /// </summary>
        public abstract void Save();

        public override string ToString() 
        {
            return Name;
        }
    }

    /// <summary>
    /// Класс работы с файлом
    /// </summary>
    public class FileModel : FileSystemNode
    {

        public FileModel() { }
        public FileModel(string fullName) : base(fullName) { }
        public FileModel(FileInfo file) : base(file) { }

        public override FileSystemInfo Info
        {
            get { return new FileInfo(_fullName); }
        }
        /// <summary>
        /// Расширение из полного имени файла, без точки
        /// </summary>
        public string Extension
        {
            get
            {
                int index = _fullName.LastIndexOf('.');
                if (index == 0)
                    return "";
                return _fullName.Substring(index);
            }
            set
            {
                int index = _fullName.LastIndexOf('.');
                if (index == 0)
                    FullName = _fullName + '.' + value;
                else
                    FullName = _fullName.Substring(0, index + 1) + value;
            }
        }

        public override void Load() { }
        public override void Save() { }

        #region Определения Hierarchical

        // в файле неизвестного типа не бывает структуры, то есть нет Nodes

        public override IEnumerable<Type> GetNodeTypes() { return new Type[0]; }

        public override IHierarchical this[int index]
        {
            get { return null; }
            set { }
        }

        protected override void OnAddingNode(IHierarchical node) { }
        protected override void OnAddingNodeAt(IHierarchical node, int index) { }
        protected override void OnRemoveNode(IHierarchical node, bool recursively) { }
        protected override void OnRemoveNodeAt(int index) { }

        #endregion

        public override string ToString()
        {
            return Name + Extension;
        }
    }

    /// <summary>
    /// Класс работы с директорией
    /// </summary>
    public class DirectoryModel : FileSystemNode
    {
        protected List<FileSystemNode> _nodes = null;

        public DirectoryModel() { }
        public DirectoryModel(string fullName) : base(fullName) { }
        public DirectoryModel(DirectoryInfo directory) : base(directory) { }
        public DirectoryModel(FileSystemInfo directory) : base(directory) { }

        public override IEnumerable<IHierarchical> Nodes
        {
            get {
                if (_nodes == null)
                    Load();
                return _nodes;
            }
        }
        public override bool IsLast
        {
            get { return _nodes == null || _nodes.Count == 0; }
        }
        public override FileSystemInfo Info
        {
            get {
                if (_fullName != null && _fullName != "")
                    return new DirectoryInfo(_fullName);
                else
                    return null;
            }
        }

        public override void Load() 
        {
            _nodes = new List<FileSystemNode>();

            if (_fullName == null || _fullName == "")
                return;

            DirectoryInfo info = new DirectoryInfo(_fullName);
            FileSystemInfo[] files = info.GetFileSystemInfos();
            foreach (FileSystemInfo file in files)
            {
                if (file.Attributes.HasFlag(FileAttributes.Directory))
                    _nodes.Add(new DirectoryModel(file.FullName));
                else if(file.Extension.ToLower() == ".ltx")
                {
                    StalkerClass.LtxModel ltx = new StalkerClass.LtxModel(file.FullName);
                    ltx.Load();
                    _nodes.Add(ltx);
                }
                else
                    _nodes.Add(new FileModel(file.FullName));
            }
            OnPropertyChanged("Nodes");
        }

        public override void Save() 
        { 
        }

        #region Определения Hierarchical

        public override IEnumerable<Type> GetNodeTypes() { return new Type[]{ typeof(FileSystemNode) }; }

        public override IHierarchical this[int index] { 
            get { return Nodes.ElementAt(index); } 
            set {
                CheckNodeAndThrowException(value);

                if (_nodes == null)
                    Load();

                _nodes[index] = value as FileSystemNode;
                OnPropertyChanged("Nodes");
            } 
        }

        protected override void OnAddingNode(IHierarchical node) 
        {
            if (_nodes == null)
                Load();
            _nodes.Add(node as FileSystemNode);
        }
        protected override void OnAddingNodeAt(IHierarchical node, int index)
        {
            if (_nodes == null)
                Load();
            _nodes.Insert(index, node as FileSystemNode);
        }
        protected override void OnRemoveNode(IHierarchical node, bool recursively) 
        {
            _nodes.Remove(node as FileSystemNode);
        }
        protected override void OnRemoveNodeAt(int index)
        {
            _nodes.RemoveAt(index);
        }

        #endregion
    }
}
