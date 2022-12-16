using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Stalker_Studio.Common
{
	/// <summary>
	/// Элемент, область, часть текста
	/// </summary>
	public class TextNode : Hierarchical
	{
		protected int _startLineIndex = -1;
		protected int _endLineIndex = -1;
		protected int _startIndex = -1;
		protected int _endIndex = -1;

		public TextNode() { }

		public TextNode(int startIndex, int endIndex, int startLineIndex, int endLineIndex)
        {
            _startLineIndex = startLineIndex;
            _endLineIndex = endLineIndex;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }
		/// <summary>
		/// Индекс символа на котором заканичавется элемент в конечной строке в тексте
		/// </summary>
		public int EndIndex
		{
			get { return _endIndex; }
			set
			{
				_endIndex = value;
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Индекс строки в которой начинается элемент в тексте
		/// </summary>
		public int StartLineIndex
		{
			get { return _startLineIndex; }
			set
			{
				_startLineIndex = value;
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Индекс строки в которой заканчивается элемент в тексте
		/// </summary>
		public int EndLineIndex
		{
			get { return _endLineIndex; }
			set
			{
				_endLineIndex = value;
				OnPropertyChanged();
			}
		}

		#region Определения Hierarchical

		// в элементе текста не бывает структуры, то есть нет Nodes

		public override IEnumerable<Type> GetNodeTypes() { return new Type[0]; }

		public override IHierarchical this[int index]
		{
			get { return null; }
			set { }
		}
		public override IEnumerable<IHierarchical> Nodes
		{
			get { return null; }
			set { }
		}

		protected override void OnAddingNode(IHierarchical node) { }
		protected override void OnAddingNodeAt(IHierarchical node, int index) { }
		protected override void OnRemoveNode(IHierarchical node, bool recursively) { }
		protected override void OnRemoveNodeAt(int index) { }

		#endregion
	}

	/// <summary>
	/// Элемент, область, часть текста, описывающая объект
	/// </summary>
	public class TextObject : TextNode
	{
		protected string _name = "";
		protected string _comment = "";

		public TextObject(string name = "") : base() 
		{
			_name = name;
		}
		public TextObject(int startIndex, int endIndex, int startLineIndex, int endLineIndex, string name = "") : base(startIndex, endIndex, startLineIndex, endLineIndex)
		{
			_startLineIndex = startLineIndex;
			_endLineIndex = endLineIndex;
			_startIndex = startIndex;
			_endIndex = endIndex;
			_name = name;
		}
		/// <summary>
		/// Имя объекта
		/// </summary>
		public string Name {
			get { return _name; }
			set {
				_name = value;
				OnPropertyChanged();
			} 
		}
		/// <summary>
		/// Комментарий, связанный с объектом
		/// </summary>
		public string Comment
		{
			get { return _comment; }
			set
			{
				_comment = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// Элемент, область, часть текста, описывающая объект с подчиненными элементами типа TNode
	/// </summary>
	public class TextObject<TNode> : TextObject
		where TNode : IHierarchical
	{
		protected ObservableCollection<TNode> _nodes = new ObservableCollection<TNode>();

		public TextObject(string name = "", IEnumerable<TNode> nodes = null) : base(name)
		{
			if(nodes != null)
				_nodes = new ObservableCollection<TNode>(nodes);
		}
		public TextObject(int startIndex, int endIndex, int startLineIndex, int endLineIndex, string name = "", IEnumerable<TNode> nodes = null) : base(startIndex, endIndex, startLineIndex, endLineIndex, name)
		{
			if (nodes != null)
				_nodes = new ObservableCollection<TNode>(nodes);
		}

		#region Определения Hierarchical

		public override IEnumerable<Type> GetNodeTypes() { return new Type[] { typeof(TNode) }; }

		public override IHierarchical this[int index]
		{
			get { return Nodes.ElementAt(index); }
			set
			{
				CheckNodeAndThrowException(value);

				_nodes[index] = (TNode)value;
				OnPropertyChanged("Nodes");
			}
		}
		public override IEnumerable<IHierarchical> Nodes
		{
			get
			{
				return _nodes as IEnumerable<IHierarchical>;
			}
		}

		protected override void OnAddingNode(IHierarchical node)
		{
			_nodes.Add((TNode)node);
		}
		protected override void OnAddingNodeAt(IHierarchical node, int index)
		{
			_nodes.Insert(index, (TNode)node);
		}
		protected override void OnRemoveNode(IHierarchical node, bool recursively)
		{
			_nodes.Remove((TNode)node);
		}
		protected override void OnRemoveNodeAt(int index)
		{
			_nodes.RemoveAt(index);
		}

		#endregion
	}

	/// <summary>
	/// Элемент, область, часть текста, описывающая объект с подчиненными элементами типа TNode и с указанием родителя типа TParent
	/// </summary>
	public class TextObject<TParent, TNode> : TextObject<TNode>
		where TNode : IHierarchical
		where TParent : TextObject
	{
		protected TParent _parent = default;

		public TextObject(string name = "", IEnumerable<TNode> nodes = null, TParent parent = default) : base(name, nodes)
		{
			if (parent != default)
				_parent = parent;
		}
		public TextObject(int startIndex, int endIndex, int startLineIndex, int endLineIndex, string name = "", IEnumerable<TNode> nodes = null, TParent parent = default) : base(startIndex, endIndex, startLineIndex, endLineIndex, name, nodes)
		{
			if (parent != default)
				_parent = parent;
		}
		/// <summary>
		/// Родитель 
		/// </summary>
		public TParent Parent
		{
			get { return _parent; }
			set
			{
				_parent = value;
				OnPropertyChanged();
			}
		}
	}
}
