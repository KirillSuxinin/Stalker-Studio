using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Stalker_Studio.Common;

namespace Stalker_Studio.ViewModel
{
    class BrowserViewModel : HierarchicalViewModel
    {
        IEnumerable<IHierarchical> _fixedNodes = null;
        bool _fixedNodesGroupVisible = true;

        ToggleRelayCommand _fixNodeCommand = null;
        ExtendedRelayCommand _renameNodeCommand = null;

        ToggleRelayCommand _сhangeFixedNodesVisibilityCommand = null;

        public BrowserViewModel(string name, Hierarchical root) : base(name, root)
        {
            ContentId = "Browser";
            InitLocationName = "MainAnchorablePane";
            Initialize();
        }

        /// <summary>
        /// Закрепленные элементы иерархии или особые, отдельно обособленные
        /// </summary>
        public IEnumerable<IHierarchical> FixedNodes
        {
            get
            {
                if (_fixedNodes == null)
                    _fixedNodes = new ObservableCollection<IHierarchical>();
                return _fixedNodes;
            }
            set
            {
                _fixedNodes = value;
            }
        }
        /// <summary>
        /// Признак видимости группы закрепленных элементов
        /// </summary>
        public bool FixedNodesGroupVisible
        {
            get { return _fixedNodesGroupVisible; }
            set
            {
                _fixedNodesGroupVisible = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Команда закрпеления/открепления элемента иерархии
        /// </summary>
        public ToggleRelayCommand FixNodeCommand
        {
            get
            {
                if (_fixNodeCommand == null)
                {
                    _fixNodeCommand = new ToggleRelayCommand(this, (p) => OnFix(p), (p) => CanFix(p),
                    "Закрепить\\открепить элемент", "icon_Favorite", 
                    nameof(FixedNodes),
                    "Открепить",
                    "Закрепить");
                }
                return _fixNodeCommand;
            }
        }
        /// <summary>
        /// Команда переименования элемента иерархии
        /// </summary>
        public ExtendedRelayCommand RenameNodeCommand
        {
            get
            {
                if (_renameNodeCommand == null)
                {
                    _renameNodeCommand = new ExtendedRelayCommand((p) => OnRenameNode(p), (p) => CanRenameNode(p),
                        "Переименовать","Переименовать элемент", "icon_Rename");
                }
                return _renameNodeCommand;
            }
        }
        /// <summary>
        /// Команда изменения видимости закрепленных элементов в начале списка
        /// </summary>
        public ToggleRelayCommand ChangeFixedNodesVisibilityCommand
        {
            get
            {
                if (_сhangeFixedNodesVisibilityCommand == null)
                {
                    _сhangeFixedNodesVisibilityCommand = new ToggleRelayCommand(this, (p) => OnChangeFixedNodesVisibility(p), (p) => CanChangeFixedNodesVisibility(p),
                        "Включает\\выключает видимость закрепленных элементов в начале списка",
                        "icon_Favorite",
                        nameof(FixedNodesGroupVisible),
                        "Скрыть закрепленные элементы",
                        "Показать закрепленные элементы","123");
                }
                return _сhangeFixedNodesVisibilityCommand;
            }
        }

        private void Initialize()
        {
            //Commands.Add(FixNodeCommand);
            Commands.Add(ChangeFixedNodesVisibilityCommand);
            ItemCommands.Add(FixNodeCommand);
            ItemCommands.Add(RenameNodeCommand);
        }

        private bool CanFix(object parameter)
        {
            if (parameter == null)
                return false;
            else if (parameter is ITreeNode)
                return (parameter as ITreeNode).Value is FileModel;
            else return parameter is FileModel;
        }

        private void OnFix(object parameter)
        {
            FileModel file = null;
            if (parameter is ITreeNode)
                file = (parameter as ITreeNode).Value as FileModel;
            else
                file = parameter as FileModel;

            if (Workspace.This.FixedFiles.Contains(file))
                Workspace.This.FixedFiles.Remove(file);
            else
                Workspace.This.FixedFiles.Add(file);
            OnPropertyChanged(nameof(FixedNodes));
        }

        private bool CanChangeFixedNodesVisibility(object parameter)
        {
            return true;
        }

        private void OnChangeFixedNodesVisibility(object parameter)
        {
            FixedNodesGroupVisible = !FixedNodesGroupVisible;
        }

        private bool CanRenameNode(object parameter)
        {
            return true;
        }

        private void OnRenameNode(object parameter)
        {
            if (parameter == null)
                return;
            FrameworkElement element = InterfaceHelper.FindElementAtDataContextRecursively(_mainControl as ItemsControl, parameter);
            if (!(element is TreeViewItem))
                return;
            TreeViewItem item = element as TreeViewItem;
            item.IsSelected = true;
            FrameworkElement textBoxElement = InterfaceHelper.FindChildAtNameRecursively(item, "RenameableTextBox");
            if (textBoxElement == null || !(textBoxElement is TextBox))
                return;
            TextBox textBox = textBoxElement as TextBox;
            textBox.IsReadOnly = false;
            textBoxElement.Focus();
            
            (textBoxElement as TextBox).LostFocus += BrowserViewModel_LostFocus;
            //(textBoxElement as TextBox).LostFocus += (object sender, RoutedEventArgs e) => {
            //    textBlockElement.Visibility = Visibility.Visible;
            //    textBoxElement.Visibility = Visibility.Collapsed; 
            //};
        }

        private void BrowserViewModel_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).IsReadOnly = true;
            (sender as TextBox).LostFocus -= BrowserViewModel_LostFocus;
        }
    }
}
