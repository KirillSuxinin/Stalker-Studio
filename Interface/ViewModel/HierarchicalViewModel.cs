using System.Collections.Generic;
using System.Linq;
using Stalker_Studio.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Stalker_Studio.ViewModel
{
    /// <summary>
    /// Модель представления для иерархии
    /// </summary>
    partial class HierarchicalViewModel : ToolViewModel
    {
        Hierarchical _root = null; 
        string _search = "";
        object _selectedItem = null;
        bool _isFiltered = false;
        ObservableCollection<string> _searchHistory = new ObservableCollection<string>();
        TreeNode<IHierarchical> _filteredTree = null;

        public HierarchicalViewModel(string name) : base(name) 
        {
            Initialize();
        }
        public HierarchicalViewModel(string name, Hierarchical root = null) : base(name)
        {
            if(root != null)
                _root = root;
            Initialize();
        }
        /// <summary>
        /// Корневой элемент иерархии либо IHerarchical, либо TreeNode<IHierarchical> в зависимости от настроек
        /// </summary>
        public object Root 
        {
            get {
                if (_filteredTree == null)
                    return _root;
                else
                    return _filteredTree;
            }
            set {
                _root = value as Hierarchical;
                OnPropertyChanged(nameof(Root));
            }
        }
        /// <summary>
        /// Строка поиска по представлению элементов
        /// </summary>
        public string Search
        {
            get {
                return _search;
            }
            set {
                if (_search == value)
                    return;

                _search = value;

                if (_search != "")
                {
                    IsFiltered = true;
                    if (_searchHistory.Contains(_search))
                        _searchHistory.Remove(_search);
                    _searchHistory.Insert(0, _search);
                }
                else
                    IsFiltered = false;
                
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Строка поиска по представлению элементов
        /// </summary>
        public bool IsFiltered
        {
            get
            {
                return _isFiltered;
            }
            set
            {               
                _isFiltered = value;
                if (!_isFiltered)
                {
                    _search = "";
                    OnPropertyChanged(nameof(Search));
                }

                ApplyFilter();
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// История поиска
        /// </summary>
        public ReadOnlyObservableCollection<string> SearchHistory
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_searchHistory);
            }
        }
        /// <summary>
        /// Выбранный элемент
        /// </summary>
        public object SelectedItem 
        { 
            get => _selectedItem;
            set
            { 
                _selectedItem = value;
                OnPropertyChanged();
            } 
        }

        private void Initialize()
        {
            _commands.Add(ExpandAllCommand);
            _commands.Add(CollapseAllCommand);
            _commands.Add(FilterCommand);
            _itemCommands.Add(OpenValueCommand);
            _itemCommands.Add(RemoveCommand);
        }

        /// <summary>
        /// Применить фильтр к иерархии
        /// </summary>
        protected void ApplyFilter() 
        {
            _filteredTree = null;

            if (IsFiltered)
            {
                _filteredTree = new TreeNode<IHierarchical>(_root.FilteredNodes(x => x.ToString().Contains(_search), true, true));
                _filteredTree.Value = _root;
            }
            //else
            //    ApplyOrder();
            OnPropertyChanged(nameof(Root));
        }
        /// <summary>
        /// Применить порядок, сортировку к иерархии
        /// </summary>
        protected void ApplyOrder()
        {
            if (Workspace.This.FixedFiles.Count == 0)
                return;

            _filteredTree = new TreeNode<IHierarchical>(_root.Nodes);
            _filteredTree.Value = _root;
            IEnumerable<IHierarchical> fixedNodes = _root.FindNodes(x =>
            {
                if (!(x is Common.FileModel))
                    return false;
                else
                    return Workspace.This.FixedFiles.Contains(x);
            },
                true);
            foreach (IHierarchical node in fixedNodes)
            {
                _filteredTree.GetNodes().Insert(0, new TreeNode<IHierarchical>(node));
            }
        }
    }
}
