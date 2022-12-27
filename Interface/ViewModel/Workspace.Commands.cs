using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Stalker_Studio.ViewModel
{
    internal partial class Workspace : ViewModelBase
	{
		private RelayCommand _openFileCommand = null;
		private RelayCommand _openGamedataCommand = null;
		private RelayCommand _newCommand = null;

		public ICommand OpenFileCommand
		{
			get
			{
				if (_openFileCommand == null)
					_openFileCommand = new ExtendedRelayCommand((p) => OnOpen(p), (p) => CanOpen(p), "Открыть файл", "Открыть файл", "icon_OpenFile");
				return _openFileCommand;
			}
		}
		public ICommand OpenGamedataCommand
		{
			get
			{
				if (_openGamedataCommand == null)
					_openGamedataCommand = new ExtendedRelayCommand((p) => OnOpenGamedata(p), (p) => CanOpenGamedata(p), "Открыть gamedata", "Открыть gamedata", "icon_OpenProjectFolder");
				return _openGamedataCommand;
			}
		}
		public ICommand NewCommand
		{
			get
			{
				if (_newCommand == null)
					_newCommand = new ExtendedRelayCommand((p) => OnNew(p), (p) => CanNew(p), "Создать", "Создать файл", "icon_NewFile");
				return _newCommand;
			}
		}

		#region OpenCommand

		private bool CanOpen(object parameter) => true;

		private void OnOpen(object parameter)
		{
			if (parameter is string)
				if (parameter != null && parameter as string != "")
				{
					var fileViewModel = Open(parameter as string);
					ActiveDocument = fileViewModel;
					return;
				}

			var dlg = new OpenFileDialog();
			if (StalkerClass.GamedataManager.This.Root != null)
				dlg.InitialDirectory = StalkerClass.GamedataManager.This.Root.FullName;

			string filter = "";
			foreach (string extension in StalkerClass.GamedataManager.FileExtentions)
				filter += $"(*.{extension})|*.{extension}|";
			filter += "Все файлы (*.*)|*.*";
			dlg.Filter = filter;

			if (dlg.ShowDialog().GetValueOrDefault())
			{
				var fileViewModel = Open(dlg.FileName);
				ActiveDocument = fileViewModel;
			}

		}

		private bool CanOpenGamedata(object parameter) => true;

		private void OnOpenGamedata(object parameter)
		{
			System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
			folder.Description = "Выберите путь к gamedata для работы";
			if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				StalkerClass.GamedataManager.This.SetRootAtPath(folder.SelectedPath);
				This.Browser.Root = StalkerClass.GamedataManager.This.Root;
			}
		}

		#endregion OpenCommand

		#region NewCommand

		private bool CanNew(object parameter)
		{
			return true;
		}

		private void OnNew(object parameter)
		{
			FileViewModel fileVM = null;
			if (parameter is string)
				fileVM = new FileViewModel(StalkerClass.GamedataManager.CreateFileSystemNodeFromExtension(parameter as string));
			else
				fileVM = new FileViewModel(StalkerClass.GamedataManager.CreateFileSystemNodeFromExtension("txt"));

			_files.Add(fileVM);
			ActiveDocument = _files.Last();
		}

		#endregion NewCommand
	}
}
