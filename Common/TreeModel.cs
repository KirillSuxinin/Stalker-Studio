using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stalker_Studio.Common
{

    /// <summary>
    /// Интерфейс элемента иерархии
    /// </summary>
    public interface IHierarchical : INotifyPropertyChanged
    {
        IHierarchical this[int index] { get; set; }
        /// <summary>
        /// Подчиненные элементы
        /// </summary>
        IEnumerable<IHierarchical> Nodes { get; }
        /// <summary>
        /// Подчиненных элементов нет
        /// </summary>
        bool IsLast { get; }

        /// <summary>
        /// Добавить подчиненный элемент
        /// </summary>
        /// <param name="node">Подчиненный элемент, который надо добавить</param>
        void AddNode(IHierarchical node);
        /// <summary>
        /// Удалить подчиненный элемент
        /// Не выполняется когда Nodes == null
        /// </summary>
        /// <param name="node">Подчиненный элемент, который надо удалить</param>
        /// <param name="recursively">Удалять в подчиненных элементах (рекурсивно)</param>
        void RemoveNode(IHierarchical node, bool recursively = true);
        /// <summary>
        /// Вставить подчиненный элемент по индексу
        /// <param name="node">Подчиненный элемент, который надо добавить</param>
        /// </summary>
        void AddNodeAt(IHierarchical node, int index);
        /// <summary>
        /// Удалить подчиненный элемент по индексу
        /// Не выполняется когда Nodes == null
        /// </summary>
        void RemoveNodeAt(int index);

        /// <summary>
        /// Возвращает возможные типы подчиненных элементов
        /// </summary>
        IEnumerable<Type> GetNodeTypes();
        /// <summary>
        /// Проверяет соответствие типов подчиненных элементов типу type
        /// </summary>
        /// <param name="type">Проверяемый</param>
        bool AllowNodeType(Type type);
    }

    /// <summary>
    /// Элемента иерархии.
    /// Предопределяет некоторый общий функционал для удобства при наследовании
    /// </summary>
    public abstract class Hierarchical : IHierarchical
    {
        public abstract IHierarchical this[int index] { get; set; }

        public abstract IEnumerable<IHierarchical> Nodes { get; set; }
        public virtual bool IsLast { 
            get {
                IEnumerable<IHierarchical> nodes = Nodes;
                return Nodes == null ? true : nodes.Count() == 0;
            } 
        }

        public virtual void AddNode(IHierarchical node) 
        {
            CheckNodeAndThrowException(node);
            OnAddingNode(node);
            if (Nodes != null)
                OnPropertyChanged("Nodes");
        }
        public virtual void RemoveNode(IHierarchical node, bool recursively = true) 
        {
            if (Nodes == null)
                return;
            CheckNodeAndThrowException(node);
            OnRemoveNode(node, recursively);
            OnPropertyChanged("Nodes");
        }
        public virtual void AddNodeAt(IHierarchical node, int index)
        {
            CheckNodeAndThrowException(node);
            OnAddingNodeAt(node, index);
            if (Nodes != null)
                OnPropertyChanged("Nodes");
        }
        public virtual void RemoveNodeAt(int index) 
        {
            if (Nodes == null)
                return;
            OnRemoveNodeAt(index);
            OnPropertyChanged("Nodes");
        }

        protected abstract void OnAddingNode(IHierarchical node);
        protected abstract void OnAddingNodeAt(IHierarchical node, int index);
        protected abstract void OnRemoveNode(IHierarchical node, bool recursively);
        protected abstract void OnRemoveNodeAt(int index);

        public abstract IEnumerable<Type> GetNodeTypes();

        public virtual bool AllowNodeType(Type type) 
        {
            foreach (Type nodeType in GetNodeTypes())
                if (type.IsAssignableFrom(nodeType))
                    return true;
            return false;
        }
        /// <summary>
        /// Проверяет тип подчиненного элемента и выбрасывает исключение в случае ошибки.
        /// Вызывается по умолчанию при добавлении и удалении элементов
        /// </summary>
        protected virtual void CheckNodeAndThrowException(IHierarchical node) 
        {
            if (!AllowNodeType(node.GetType()))
                throw new System.IO.IOException($"Значение не является { GetNodeTypes() } <{ this.GetType().FullName }>");
        }
        /// <summary>
        /// Событие изменения свойства класса
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Интерфейс элемента дерева с отдельно хранимым значением
    /// </summary>
    public interface ITreeNode : INotifyPropertyChanged
    {
        /// <summary>
        /// Подчиненные элементы
        /// </summary>
        ObservableCollection<ITreeNode> Nodes { get; set; }
        /// <summary>
        /// Подчиненные элементов нет
        /// </summary>
        bool IsLast { get; }
        /// <summary>
        /// Хранимое значение
        /// </summary>
        object Value { get; set; }
    }

    /// <summary>
    /// Элемента дерева с отдельно хранимым значением
    /// </summary>
    public class TreeNode<TType> : ITreeNode
    {
        protected ObservableCollection<TreeNode<TType>> _nodes = new ObservableCollection<TreeNode<TType>>();
        protected TType _value;

        public TreeNode() { }
        public TreeNode(IEnumerable<TType> values)
        {
            foreach (TType value in values)
                _nodes.Add(new TreeNode<TType>(value));
        }
        public TreeNode(IEnumerable<TreeNode<TType>> nodes)
        {
            _nodes = new ObservableCollection<TreeNode<TType>>(nodes);
        }
        public TreeNode(TType value)
        {
            _value = value;
        }

        /// <summary>
        /// Событие изменения свойства класса
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual ObservableCollection<ITreeNode> Nodes { 
            get { return _nodes as ObservableCollection<ITreeNode>; }
            set { 
                _nodes = value as ObservableCollection<TreeNode<TType>>;
                OnPropertyChanged();
            }
        }
        public virtual bool IsLast
        {
            get { return _nodes.Count == 0; }
        }
        public object Value { 
            get { return _value; }
            set { 
                _value = (TType)value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Возвращает элементы дерева в соответсвии с типом
        /// </summary>
        public ObservableCollection<TreeNode<TType>> GetNodes() 
        {
            return _nodes;
        }
        /// <summary>
        /// Возвращает хранимое значение в соответсвии с типом
        /// </summary>
        public TType GetValue()
        {
            return _value;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Интерфейс дерева
    /// </summary>
    public interface ITree : INotifyPropertyChanged
    {
        /// <summary>
        /// Подчиненные элементы
        /// </summary>
        ITreeNode Root { get; set; }

        /// <summary>
        /// Возвращает элемент дерева с указанным хранимым значением
        /// </summary>
        object GetNodeAtValue(object value);
        /// <summary>
        /// Возвращает элемент дерева с указанным условием predicate
        /// </summary>
        object GetNodeAtValue(Predicate<object> predicate);
    }

    public class TreeModel<TType> : ITree
    {
        TreeNode<TType> _root = null;

        public TreeModel() 
        { 
            
        }
        /// <summary>
        /// Событие изменения свойства класса
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public ITreeNode Root 
        {
            get { return _root; }
            set { 
                _root = value as TreeNode<TType>;
                OnPropertyChanged();
            }    
        }

        public object GetNodeAtValue(object value) 
        {
            return FindValueInNodes(_root, value);
        }
        public object GetNodeAtValue(Predicate<object> predicate) 
        {
            return FindValueInNodes(_root, predicate as Predicate<TType>);
        }

        public object GetNodeAtValue(Predicate<TType> predicate)
        {
            return FindValueInNodes(_root, predicate);
        }

        /// <summary>
        /// Рекурсивная функция поиска значения по условию predicate
        /// </summary>
        TType FindValueInNodes(TreeNode<TType> node, Predicate<TType> predicate) 
        {
            foreach (TreeNode<TType> child in node.Nodes)
            {
                TType value = child.GetValue();
                if (predicate(value))
                    return value;
            }

            foreach (TreeNode<TType> child in node.Nodes)
                FindValueInNodes(child, predicate);

            return default;
        }
        /// <summary>
        /// Рекурсивная функция поиска значения
        /// </summary>
        TType FindValueInNodes(TreeNode<TType> node, object value)
        {
            foreach (TreeNode<TType> child in node.Nodes)
                if (child.Value == value)
                    return child.GetValue();

            foreach (TreeNode<TType> child in node.Nodes)
                FindValueInNodes(child, value);

            return default;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
