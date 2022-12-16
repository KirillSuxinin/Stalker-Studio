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

		#region fields
		private FileModel file = null;
		private static ImageSourceConverter ISC = new ImageSourceConverter();
		private bool _isModified = false;
		private ExtendedRelayCommand _saveCommand = null;
		private ExtendedRelayCommand _saveAsCommand = null;
		private ExtendedRelayCommand _closeCommand = null;
		#endregion fields

		public FileViewModel(FileModel file)
		{
			
			//Set the icon only for open documents (just a test)
			//IconSource = ISC.ConvertFromInvariantString(@"pack://application:,,/Images/document.png") as ImageSource;
		}

		/// <summary>
		/// Default class constructor
		/// </summary>
		public FileViewModel()
		{
			IsModified = true;
			//Title = FileName;
		}

		#region Properties

		//public string Title
		//{
		//	get
		//	{
		//		if (file.FullName == null)
		//			return "Noname" + (IsModified ? "*" : "");

		//		return file.Name + (IsModified ? "*" : "");
		//	}
		//}

		public bool IsModified
		{
			get => _isModified;
			set
			{
				if (_isModified != value)
				{
					_isModified = value;
					OnPropertyChanged(nameof(IsModified));
					//OnPropertyChanged(nameof(Title));
				}
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				if (_saveCommand == null)
					_saveCommand = new ExtendedRelayCommand((p) => OnSave(p), (p) => CanSave(p));

				return _saveCommand;
			}
		}

		public ICommand SaveAsCommand
		{
			get
			{
				if (_saveAsCommand == null)
					_saveAsCommand = new ExtendedRelayCommand((p) => OnSaveAs(p), (p) => CanSaveAs(p));

				return _saveAsCommand;
			}
		}

		public ICommand CloseCommand
		{
			get
			{
				if (_closeCommand == null)
					_closeCommand = new ExtendedRelayCommand((p) => OnClose(), (p) => CanClose());

				return _closeCommand;
			}
		}

		public override ObservableCollection<ICommand> Commands { 
			get {
				if (_commands.Count == 0)
				{
					_commands.Add(SaveCommand);
					_commands.Add(SaveAsCommand);
					_commands.Add(CloseCommand);
				}
				return _commands;
			} 
		}

		#endregion  Properties

		#region methods
		private bool CanClose()
		{
			return true;
		}

		private void OnClose()
		{
			Workspace.This.Close(this);
		}

		private bool CanSave(object parameter)
		{
			return IsModified;
		}

		private void OnSave(object parameter)
		{
			Workspace.This.Save(this, false);
		}

		private bool CanSaveAs(object parameter)
		{
			return IsModified;
		}

		private void OnSaveAs(object parameter)
		{
			Workspace.This.Save(this, true);
		}
		#endregion methods
	}
}
