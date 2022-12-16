using AvalonDock.Themes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Stalker_Studio.Common;

namespace Stalker_Studio.ViewModel
{
	internal partial class Workspace : ViewModelBase
	{
        private ToolViewModel[] _tools = null;
		private ObservableCollection<string> _lastFiles = new ObservableCollection<string>();
		private ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();

		private FileViewModel _activeDocument = null;
		private HierarchicalViewModel _browser = null;
		//private FileStatsViewModel _fileStats = null;
		private Tuple<string, Theme> selectedTheme;

		protected Workspace()
		{
			this.Themes = new List<Tuple<string, Theme>>
			{
				new Tuple<string, Theme>(nameof(AvalonDockDarkTheme), new AvalonDockDarkTheme()),
			};
			this.SelectedTheme = Themes.First();

			string[] elements = Properties.Settings.Default.LastOpenIndex.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string vEl in elements)
				_lastFiles.Add(vEl);
		}

		public event EventHandler ActiveDocumentChanged;

        #region properties

        public static Workspace This { get; } = new Workspace();
		/// <summary>
		/// Заголовок рабочего пространства (главной формы)
		/// </summary>
		public string Title
		{
			get {
				string title = "Stalker studio";
				if (Gamedata.Root != null)
					title += "  -  " + Gamedata.Root.FullName;
				return title;
			}
		}
		/// <summary>
		/// Текущие открытые файлы
		/// </summary>
		public ReadOnlyObservableCollection<FileViewModel> Files
		{
			get => new ReadOnlyObservableCollection<FileViewModel>(_files);
		}
		/// <summary>
		/// Инструменты
		/// </summary>
		public IEnumerable<ToolViewModel> Tools
		{
			get
			{
				//if (_tools == null)
				//	_tools = new ToolViewModel[] { FileStats };
				return _tools;
			}
		}
		/// <summary>
		/// Текущий менеджер геймдаты (для привязки)
		/// </summary>
		public StalkerClass.GamedataManager Gamedata
		{
			get => StalkerClass.GamedataManager.This;
		}
		/// <summary>
		/// Последние файлы
		/// </summary>
		public ReadOnlyObservableCollection<string> LastFiles
		{
			get => new ReadOnlyObservableCollection<string>(_lastFiles);
		}
		/// <summary>
		/// Текущий файл
		/// </summary>
		public FileViewModel ActiveDocument
		{
			get => _activeDocument;
			set
			{
				if (_activeDocument != value)
				{
					_activeDocument = value;
					OnPropertyChanged(nameof(ActiveDocument));
					if (ActiveDocumentChanged != null)
						ActiveDocumentChanged(this, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Доступные темы
		/// </summary>
		public List<Tuple<string, Theme>> Themes { get; set; }
		/// <summary>
		/// Текущя тема
		/// </summary>
		public Tuple<string, Theme> SelectedTheme
		{
			get => selectedTheme;
			set
			{
				selectedTheme = value;
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Инструменты
		/// </summary>
		public HierarchicalViewModel Browser
		{
			get
			{
				if (_browser == null)
					_browser = new HierarchicalViewModel(StalkerClass.GamedataManager.This.Root);
				return _browser;
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Закрывает файл
		/// </summary>
		internal void Close(FileViewModel fileToClose)
		{
			if (fileToClose.IsModified)
			{
				var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.Title), "AvalonDock Test App", MessageBoxButton.YesNoCancel);
				if (res == MessageBoxResult.Cancel)
					return;
				if (res == MessageBoxResult.Yes)
				{
					Save(fileToClose);
				}
			}

			_files.Remove(fileToClose);
		}
		/// <summary>
		/// Сохраняет файл
		/// </summary>
		internal void Save(FileViewModel fileToSave, bool saveAsFlag = false)
		{
			//if (fileToSave.FilePath == null || saveAsFlag)
			//{
			//	var dlg = new SaveFileDialog();
			//	if (dlg.ShowDialog().GetValueOrDefault())
			//		fileToSave.FilePath = dlg.SafeFileName;
			//}
			//if (fileToSave.FilePath == null)
			//{
			//	return;
			//}
			//File.WriteAllText(fileToSave.FilePath, fileToSave.TextContent);
			ActiveDocument.IsModified = false;
		}
		/// <summary>
		/// Отркывает файл по пути
		/// </summary>
		internal FileViewModel Open(string filepath)
		{
			//var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filepath);
			//if (fileViewModel != null)
			//	return fileViewModel;

			//fileViewModel = new FileViewModel(filepath);
			//_files.Add(fileViewModel);
			//return fileViewModel;

			AddLastFile(filepath);

			return null;
		}
		/// <summary>
		/// Добавляет путь в последние открытые файлы и сохраняет это в настройках
		/// </summary>
		internal void AddLastFile(string filepath)
		{
			if (_lastFiles.Contains(filepath))
				_lastFiles.Remove(filepath);
			_lastFiles.Insert(0, filepath);

			if (_lastFiles.Contains(filepath))
			{
				string lastOpenfiles = "";
				foreach (string file in _lastFiles)
					lastOpenfiles += file + ';';
				Properties.Settings.Default.LastOpenIndex = lastOpenfiles;
			}
			else
				Properties.Settings.Default.LastOpenIndex = filepath + ';' + Properties.Settings.Default.LastOpenIndex;
			OnPropertyChanged("LastFiles");
		}

		#endregion methods
	}
}
