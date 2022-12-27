﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Reflection;

namespace Stalker_Studio
{
    /// <summary>
    /// Вспомогательные функции по интерфейсу
    /// </summary>
    class InterfaceHelper
    {
        /// <summary>
        /// Соответствие ИмяТипа -> КлючИконки (ImageSource)
        /// </summary>
        public static Dictionary<string, object> TypesIconKeys = new Dictionary<string, object>() 
            {
                { nameof(Common.FileSystemNode), App.Current.FindResource("icon_Document")},
                { nameof(Common.DirectoryModel), App.Current.FindResource("icon_FolderClosed")},
                { nameof(StalkerClass.LtxModel), App.Current.FindResource("icon_LTXFile")},
                { nameof(StalkerClass.LtxSection), App.Current.FindResource("icon_Class")},
                { nameof(StalkerClass.LtxParameter), App.Current.FindResource("icon_Field")},
                { nameof(Common.TextFileModel), App.Current.FindResource("icon_TextFile")}
            };

        public static void InitializeWindow(Window window)
        {
            // обработчики команд окна

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.CloseWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    window.Close();
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.MaximizeWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    if (window.WindowState == WindowState.Maximized)
                        window.WindowState = WindowState.Normal;
                    else
                        window.WindowState = WindowState.Maximized;
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.MinimizeWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    window.WindowState = WindowState.Minimized;
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));
        }


        //protected void SetIsExpandedRecursively(ItemsControl itemsControl, bool isExpanded)
        //{
        //    if (!itemsControl.HasItems) return;
        //    ItemContainerGenerator itemContainerGenerator = itemsControl.ItemContainerGenerator;
        //    for (int i = itemsControl.Items.Count - 1; i >= 0; --i)
        //    {
        //        ItemsControl childControl = itemContainerGenerator.ContainerFromIndex(i) as ItemsControl;
        //        if (childControl != null) SetIsExpandedRecursively(childControl, isExpanded);
        //    }
        //    TreeViewItem treeViewItem = itemsControl as TreeViewItem;
        //    if (treeViewItem != null)
        //        treeViewItem.IsExpanded = isExpanded;
        //}

        /// <summary>
        /// Рекурсивно устанавливает значение value свойства с именем propertyName для элемента-коллекции и подчиненных элементов
        /// </summary>
        public static void SetPropertyValueRecursively(ItemsControl itemsControl, string propertyName, object value)
        {
            if (!itemsControl.HasItems) return;
            ItemContainerGenerator itemContainerGenerator = itemsControl.ItemContainerGenerator;
            int count = itemsControl.Items.Count;
            for (int i = 0; i < count; i++)
            {
                FrameworkElement childControl = itemContainerGenerator.ContainerFromIndex(i) as ItemsControl;
                if (childControl is ItemsControl)
                    SetPropertyValueRecursively(childControl as ItemsControl, propertyName, value);
            }
            PropertyInfo property = itemsControl.GetType().GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
                property.SetValue(itemsControl, value);
        }
        /// <summary>
        /// Рекурсивно ищет элемент с соответствующим DataContext в элементе-коллекции и подчиненных элементах
        /// </summary>
        public static FrameworkElement FindElementAtDataContextRecursively(ItemsControl itemsControl, object DataContext)
        {
            if (!itemsControl.HasItems) return null;
            ItemContainerGenerator itemContainerGenerator = itemsControl.ItemContainerGenerator;
            for (int i = itemsControl.Items.Count - 1; i >= 0; --i)
            {
                ItemsControl childControl = itemContainerGenerator.ContainerFromIndex(i) as ItemsControl;
                if (childControl != null)
                {
                    if (childControl.DataContext == DataContext)
                        return childControl;
                    FrameworkElement element = FindElementAtDataContextRecursively(childControl, DataContext);
                    if (element != null)
                        return element;
                }
            }
            return null;
        }
        /// <summary>
        /// Рекурсивно ищет элемент по имени в элементе-коллекции и подчиненных элементах
        /// </summary>
        public static FrameworkElement FindChildAtNameRecursively(FrameworkElement element, string name)
        {

            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child.Name == name)
                    return child;
                FrameworkElement finded = FindChildAtNameRecursively(VisualTreeHelper.GetChild(element, i) as FrameworkElement, name);
                if (finded != null)
                    return finded;
            }
            return null;
        }
    }
    /// <summary>
    /// Промежуточный класс для привязки к DataContext родительского элемента на любом уровне, используется через ресурсы
    /// Паттерн прокси
    /// </summary>
    public class RenameableHeaderDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate String { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ContentPresenter || !(item is string))
                return null;
            return String;
        }
    }
    /// <summary>
    /// Промежуточный класс для привязки к DataContext родительского элемента на любом уровне, используется через ресурсы
    /// Паттерн прокси
    /// </summary>
    public class ProxyDependency : DependencyObject
    {
        public static readonly DependencyProperty DataProperty =
                   DependencyProperty.Register("Data", typeof(object), typeof(ProxyDependency), new UIPropertyMetadata(null));
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
    /// <summary>
    /// Конвертирует Type в ImageSource, то есть возвращает иконку соответствующую типу
    /// </summary>
    public class TypeIconConverter : IValueConverter
    {
        /// <summary>
        /// Кеш имен базовых типов для которых определены исконки в Interface.TypesIconKeys. Что бы каждый раз не искать по иерархии наследования в Type.
        /// </summary>
        protected static Dictionary<Type, string> _typesСache = new Dictionary<Type, string>();

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            object Key = FindResource(InterfaceHelper.TypesIconKeys, value.GetType());
            if(Key == default)
                return DependencyProperty.UnsetValue;

            //return App.Current.FindResource(Key);
            return Key;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        protected virtual object FindResource(Dictionary<string, object> typeResourceDictionary, Type targetType)
        {
            string typeName;
            if (!_typesСache.TryGetValue(targetType, out typeName))
                typeName = targetType.Name;

            object value;
            if (!typeResourceDictionary.TryGetValue(typeName, out value))
            {
                Type current = targetType.BaseType;
                for (; current != default;)
                {
                    if (typeResourceDictionary.TryGetValue(current.Name, out value))
                    {
                        _typesСache.Add(targetType, current.Name);
                        break;
                    }
                    else
                        current = current.BaseType;
                }
            }
            return value;
        }
        protected virtual object FindResourceKey(Dictionary<string, object> typeResourceDictionary, object value)
        {
            foreach (KeyValuePair<string, object> keyValue in typeResourceDictionary)
                if (keyValue.Value == value)
                    return keyValue.Key;
            return DependencyProperty.UnsetValue;
        }
    }

    public class TreeNodeIconConverter : TypeIconConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            if (value is Common.ITreeNode)
                value = (value as Common.ITreeNode).Value;

            return base.Convert(value, targetType, parameter, culture);
        }
    }
    /// <summary>
    /// Устарел
    /// </summary>
    public class FileFixedFlagConverter : IMultiValueConverter
    {
        public virtual object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Length < 2) 
                return null;

            IEnumerable<Common.FileModel> valueList = null;

            if (value[0] is IEnumerable<Common.FileModel>)
                valueList = value[0] as IEnumerable<Common.FileModel>;
            else if (value[1] is IEnumerable<Common.FileModel>)
                valueList = value[1] as IEnumerable<Common.FileModel>;

            if (valueList == null)
                return null;

            Common.FileModel file = null;

            if (value[0] is Common.FileModel)
                file = value[0] as Common.FileModel;
            else if(value[0] is Common.ITreeNode)
                file = (value[0] as Common.ITreeNode).Value as Common.FileModel;
            else if (value[1] is Common.FileModel)
                file = value[1] as Common.FileModel;
            else if (value[1] is Common.ITreeNode)
                file = (value[1] as Common.ITreeNode).Value as Common.FileModel;

            if (file == null)
                return null;

            return valueList.Contains(file);
        }

        public virtual object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {

            return null;
        }
    }
    /// <summary>
    /// Извлекает флаг IsChecked для ToggleButton из источника, 
    /// если источником является список, то в параметр передается возможный элемент этого списка
    /// </summary>
    public class IsCheckedToggleButtonConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is DataContextSpy)
                parameter = (parameter as DataContextSpy).DataContext;
            if (value is IEnumerable<object>)
            {
                IEnumerable<object> valueList = value as IEnumerable<object>;
                return valueList.Contains(parameter);
            }
            else
                return value;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// Конвертирует bool в Visibility, то есть возвращает состояние видимости в зависимости от флага видимости
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool) || targetType != typeof(Visibility))
                return DependencyProperty.UnsetValue;
            bool visible = (bool)value;

            if (visible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Visibility) || targetType != typeof(bool))
                return false;

            return (Visibility)value == Visibility.Visible;
        }
    }
    /// <summary>
    /// Конвертирует bool в Visibility, то есть возвращает состояние видимости в зависимости от флага видимости
    /// </summary>
    public class CollectionToStringConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IList))
                return value;
            IList collection = value as IList;

            string result = "";
            string splitter = parameter is string ? parameter as string : ", ";

            foreach (object item in collection)
            {
                result += item.ToString() + splitter;
            }

            if(result.Length > splitter.Length)
                return result.Substring(0, result.Length - splitter.Length);
            else
                return result;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
