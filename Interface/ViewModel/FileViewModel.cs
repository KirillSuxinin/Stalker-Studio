using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Stalker_Studio.Common;

namespace Stalker_Studio.ViewModel
{
	class FileViewModel : PaneViewModel
	{
		private FileModel _file = null;
		private bool _isModified = false;
		private ExtendedRelayCommand _saveCommand = null;
		private ExtendedRelayCommand _saveAsCommand = null;
		private ExtendedRelayCommand _closeCommand = null;

		public FileViewModel(FileModel file)
		{
			_file = file;
            _file.PropertyChanged += File_PropertyChanged;

			Initialize();
		}
		public FileViewModel(string fullName)
		{
			_file = new FileModel(fullName);
			_file.PropertyChanged += File_PropertyChanged;
			IsModified = true;

			Initialize();
		}
		public FileViewModel()
		{
			_file = new FileModel();
			_file.PropertyChanged += File_PropertyChanged;
			IsModified = true;

			Initialize();
		}

		#region Properties
		/// <summary>
		/// Заголовок
		/// </summary>
		public override string Title
        {
            get
            {
                if (_file.Name == null)
                    return "Noname" + (IsModified ? "*" : "");

                return _file.Name + _file.Extension + (IsModified ? "*" : "");
            }
        }
		/// <summary>
		/// True если файл был изменен
		/// </summary>
		public bool IsModified
		{
			get => _isModified;
			set
			{
				if (_isModified != value)
				{
					_isModified = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(Title));
				}
			}
		}
		/// <summary>
		/// Файл
		/// </summary>
		public FileModel File
		{
			get => _file;
			set
			{
				_file = value;
				OnPropertyChanged();
			}
		}
		public ExtendedRelayCommand SaveCommand
		{
			get
			{
				if (_saveCommand == null)
					_saveCommand = new ExtendedRelayCommand((p) => OnSave(p), (p) => CanSave(p));

				return _saveCommand;
			}
		}
		public ExtendedRelayCommand SaveAsCommand
		{
			get
			{
				if (_saveAsCommand == null)
					_saveAsCommand = new ExtendedRelayCommand((p) => OnSaveAs(p), (p) => CanSaveAs(p));

				return _saveAsCommand;
			}
		}
		public ExtendedRelayCommand CloseCommand
		{
			get
			{
				if (_closeCommand == null)
					_closeCommand = new ExtendedRelayCommand((p) => OnClose(), (p) => CanClose());

				return _closeCommand;
			}
		}

		#endregion  Properties

		private void Initialize() 
		{
			_commands.Add(SaveCommand);
			_commands.Add(SaveAsCommand);
			_commands.Add(CloseCommand);
		}

		private void File_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			IsModified = true;
			OnPropertyChanged(nameof(File));
			OnPropertyChanged(nameof(Title));
		}

		protected virtual bool CanClose()
		{
			return true;
		}

		protected virtual void OnClose()
		{
			Workspace.This.Close(this);
		}

		protected virtual bool CanSave(object parameter)
		{
			return IsModified;
		}

		protected virtual void OnSave(object parameter)
		{
			Workspace.This.Save(this, false);
		}

		protected virtual bool CanSaveAs(object parameter)
		{
			return IsModified;
		}

		protected virtual void OnSaveAs(object parameter)
		{
			Workspace.This.Save(this, true);
		}
	}
}
