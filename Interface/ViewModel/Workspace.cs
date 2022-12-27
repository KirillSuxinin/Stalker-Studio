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
        private ObservableCollection<ToolViewModel> _tools = null;
		private ObservableCollection<string> _lastFiles = new ObservableCollection<string>();
		private ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();
		private ObservableCollection<FileModel> _fixedFiles = new ObservableCollection<FileModel>();

		private object _activeContent = null;
		private FileViewModel _activeDocument = null;
		private BrowserViewModel _browser = null;
		private PropertiesViewModel _propertiesTool = null;
		//private FileStatsViewModel _fileStats = null;
		private Tuple<string, Theme> _selectedTheme;

		protected Workspace()
		{
			Themes = new List<Tuple<string, Theme>>
			{
				new Tuple<string, Theme>(nameof(AvalonDockDarkTheme), new AvalonDockDarkTheme()),
			};
			_selectedTheme = Themes.First();

			string[] elements = Properties.Settings.Default.LastOpenIndex.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string vEl in elements)
				_lastFiles.Add(vEl);
		}

		public event EventHandler ActiveDocumentChanged;

        #region properties

        public static Workspace This { get; } = new Workspace();

		/// <summary>
		/// Текущие открытые файлы
		/// </summary>
		public ReadOnlyObservableCollection<FileViewModel> Files
		{
			get => new ReadOnlyObservableCollection<FileViewModel>(_files);
		}
		/// <summary>
		/// Текущие закрепленные файлы
		/// </summary>
		public ObservableCollection<FileModel> FixedFiles
		{
			get => _fixedFiles;
		}
		/// <summary>
		/// Инструменты
		/// </summary>
		public IEnumerable<ToolViewModel> Tools
		{
			get
			{
                if (_tools == null)
                    _tools = new ObservableCollection<ToolViewModel> { Browser, PropertiesTool };
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
		/// Текущая тема
		/// </summary>
		public Tuple<string, Theme> SelectedTheme
		{
			get => _selectedTheme;
			set
			{
				_selectedTheme = value;
				OnPropertyChanged();
			}
		}
		/// <summary>
		/// Обозреватель
		/// </summary>
		public BrowserViewModel Browser
		{
			get
			{
				if (_browser == null)
				{
					_browser = new BrowserViewModel("Обозреватель", StalkerClass.GamedataManager.This.Root);
					_browser.FixedNodes = _fixedFiles;
					_browser.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
					{
						if (e.PropertyName == nameof(HierarchicalViewModel.SelectedItem))
							PropertiesTool.Source = (sender as HierarchicalViewModel).SelectedItem;
					};
				}
				return _browser;
			}
		}
		/// <summary>
		/// Обозреватель свойств
		/// </summary>
		public PropertiesViewModel PropertiesTool
		{
			get
			{
				if (_propertiesTool == null)
				{
					_propertiesTool = new PropertiesViewModel("Свойства");
				}
				return _propertiesTool;
			}
		}

        public object ActiveContent 
		{ 
			get => _activeContent;
			set
			{
				if (_activeContent is PaneViewModel)
					(_activeContent as PaneViewModel).IsSelected = false;

				_activeContent = value;

				if (_activeContent is PaneViewModel)
					(_activeContent as PaneViewModel).IsSelected = true;

				OnPropertyChanged();
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
		/// Отркывает файл
		/// </summary>
		internal FileViewModel Open(string filepath)
		{
			FileViewModel fileViewModel = _files.SingleOrDefault(x => x.File.FullName == filepath);
			if (fileViewModel == default)
			{
				FileModel file = StalkerClass.GamedataManager.This.GetFileAtPath(filepath);
				if (file == default)
					file = StalkerClass.GamedataManager.CreateFileSystemNodeFromExtension(filepath);
				fileViewModel = Open(file);
			}
			ActiveDocument = fileViewModel;
			return fileViewModel;
		}
		/// <summary>
		/// Отркывает файл
		/// </summary>
		internal FileViewModel Open(FileModel file)
		{
			FileViewModel fileViewModel = _files.SingleOrDefault(x => x.File == file);
			if (fileViewModel == default)
			{
				if (file is TextFileModel)
					fileViewModel = new TextFileViewModel(file as TextFileModel);
				else
					fileViewModel = new FileViewModel(file);

				_files.Add(fileViewModel);
			}

			ActiveDocument = fileViewModel;
			AddLastFile(file.FullName);
			return fileViewModel;
		}
		/// <summary>
		/// Добавляет путь в последние открытые файлы и сохраняет это в настройках
		/// </summary>
		internal void AddLastFile(string filepath)
		{
			_lastFiles.Remove(filepath);
			_lastFiles.Insert(0, filepath);
			
			string lastOpenFiles = "";
			foreach (string file in _lastFiles)
				lastOpenFiles += file + ';';
			Properties.Settings.Default.LastOpenIndex = lastOpenFiles;

			OnPropertyChanged(nameof(LastFiles));
		}

		#endregion methods
	}
}
