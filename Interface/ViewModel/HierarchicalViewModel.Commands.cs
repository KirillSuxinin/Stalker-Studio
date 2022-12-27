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
        ObservableCollection<ExtendedRelayCommand> _itemCommands = new ObservableCollection<ExtendedRelayCommand>();
        ExtendedRelayCommand _openValueCommand = null;
        ExtendedRelayCommand _addCommand = null;
        ExtendedRelayCommand _removeCommand = null;
        ExtendedRelayCommand _collapseAllCommand = null;
        ExtendedRelayCommand _expandAllCommand = null;
        ToggleRelayCommand _filterCommand = null;

        /// <summary>
        /// Команды, применимые для элементов
        /// </summary>
        public ObservableCollection<ExtendedRelayCommand> ItemCommands
        {
            get
            { return _itemCommands; }
        }
        /// <summary>
        /// Команда открытия элемента иерархии
        /// </summary>
        public ExtendedRelayCommand OpenValueCommand
        {
            get
            {
                if (_openValueCommand == null)
                    _openValueCommand = new ExtendedRelayCommand((p) => OnOpenValue(p), (p) => CanOpenValue(p), "Открыть", "Открыть элемент", "icon_Open");
                return _openValueCommand;
            }
        }
        /// <summary>
        /// Команда добавления элемента иерархии
        /// </summary>
        public ExtendedRelayCommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                    _addCommand = new ExtendedRelayCommand((p) => OnAdd(p), (p) => CanAdd(p), "Добавить", "Добавить в выбранный элемент", "icon_NewFile");
                return _addCommand;
            }
        }
        /// <summary>
        /// Команда удаления элемента иерархии
        /// </summary>
        public ExtendedRelayCommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                    _removeCommand = new ExtendedRelayCommand((p) => OnRemove(p), (p) => CanRemove(p), "Удалить", "Удалить выбранный элемент", "icon_Remove");
                return _removeCommand;
            }
        }
        /// <summary>
        /// Команда сворачивания всех элементов иерархии
        /// </summary>
        public ExtendedRelayCommand CollapseAllCommand
        {
            get
            {
                if (_collapseAllCommand == null)
                    _collapseAllCommand = new ExtendedRelayCommand((p) => OnCollapseAll(p), (p) => CanCollapseAll(p), "Свернуть все", "Свернуть все элементы", "icon_CollapseAll");
                return _collapseAllCommand;
            }
        }
        /// <summary>
        /// Команда развернуть всех элементов
        /// </summary>
        public ExtendedRelayCommand ExpandAllCommand
        {
            get
            {
                if (_expandAllCommand == null)
                    _expandAllCommand = new ExtendedRelayCommand((p) => OnExpandAll(p), (p) => CanExpandAll(p), "Развернуть все", "Развернуть все элементы", "icon_ExpandAll");
                return _expandAllCommand;
            }
        }
        /// <summary>
        /// Команда развернуть всех элементов
        /// </summary>
        public ToggleRelayCommand FilterCommand
        {
            get
            {
                if (_filterCommand == null)
                    _filterCommand = new ToggleRelayCommand(this, (p) => OnFilter(p), (p) => CanFilter(p), "Включить\\выключить фильтрацию", "icon_Filter", "IsFiltered", "Выключить фильтрацию", "Включить фильтрацию");
                return _filterCommand;
            }
        }

        protected virtual bool CanOpenValue(object parameter)
        {
            if (parameter == null)
                return false;
            else if (parameter is ITreeNode)
                return (parameter as ITreeNode).Value is FileSystemNode;
            else 
                return parameter is FileSystemNode;
        }
        protected virtual void OnOpenValue(object parameter)
        {
            if (parameter is ITreeNode)
                parameter = (parameter as ITreeNode).Value;

            if (parameter is DirectoryModel)
                Process.Start((parameter as DirectoryModel).FullName);
            else if(parameter is FileModel)
                Workspace.This.Open(parameter as FileModel);
        }
        protected virtual bool CanAdd(object parameter)
        {
            return true;
        }
        protected virtual void OnAdd(object parameter)
        {
            
        }
        protected virtual bool CanRemove(object parameter)
        {
            return parameter != null;
        }
        protected virtual void OnRemove(object parameter)
        {

        }

        protected virtual bool CanCollapseAll(object parameter)
        {
            return _mainControl != null;
        }
        protected virtual void OnCollapseAll(object parameter)
        {
            if (!(_mainControl is System.Windows.Controls.TreeView))
                return;
            InterfaceHelper.SetPropertyValueRecursively(_mainControl as System.Windows.Controls.TreeView, "IsExpanded", false);
        }
        protected virtual bool CanExpandAll(object parameter)
        {
            return _mainControl != null;
        }
        protected virtual void OnExpandAll(object parameter)
        {
            if (!(_mainControl is System.Windows.Controls.TreeView))
                return;

            InterfaceHelper.SetPropertyValueRecursively(_mainControl as System.Windows.Controls.TreeView, "IsExpanded", true);
        }
        protected virtual bool CanFilter(object parameter)
        {
            if(Root is ITreeNode)
                return !(Root as ITreeNode).IsLast;
            else if (Root is IHierarchical)
                return !(Root as IHierarchical).IsLast;

            return false;
        }
        protected virtual void OnFilter(object parameter)
        {
            IsFiltered = !_isFiltered;
        }
    }
}
