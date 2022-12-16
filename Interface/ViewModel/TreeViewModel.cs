using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stalker_Studio.Common;
using System.Collections.ObjectModel;

namespace Stalker_Studio.ViewModel
{
    class HierarchicalViewModel : PaneViewModel
    {
        Hierarchical _root = null; 
        string _search = "";
        ObservableCollection<string> _searchHistory = new ObservableCollection<string>();
        TreeNode<IHierarchical> _filteredTree = null;

        public HierarchicalViewModel() { }
        public HierarchicalViewModel(Hierarchical root) 
        {
            _root = root;
        }

        public object Root 
        {
            get {
                ApplyFilter();
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
        public string Search
        {
            get {
                return _search;
            }
            set {
                if (_search == value)
                    return;
                if (_search != "")
                {
                    if (_searchHistory.Contains(_search))
                        _searchHistory.Remove(_search);
                    _searchHistory.Insert(0, _search);
                }

                _search = value;
                OnPropertyChanged(nameof(Root));
            }
        }
        public ReadOnlyObservableCollection<string> SearchHistory
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_searchHistory);
            }
        }
        protected void ApplyFilter() 
        {
            _filteredTree = null;

            if (_search != "")
                _filteredTree = new TreeNode<IHierarchical>(_root.FilteredNodes(x => x.ToString().Contains(_search), true, true));
        }
    }
}
