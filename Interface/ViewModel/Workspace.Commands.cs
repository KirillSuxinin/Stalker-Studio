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
		private RelayCommand _openCommand = null;
		private RelayCommand _newCommand = null;

		public ICommand OpenCommand
		{
			get
			{
				if (_openCommand == null)
					_openCommand = new ExtendedRelayCommand((p) => OnOpen(p), (p) => CanOpen(p), "Открыть", "Открыть файл");

				return _openCommand;
			}
		}
		public ICommand NewCommand
		{
			get
			{
				if (_newCommand == null)
					_newCommand = new ExtendedRelayCommand((p) => OnNew(p), (p) => CanNew(p), "Создать", "Создать файл");

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
			if (Gamedata != null)
				dlg.InitialDirectory = StalkerClass.GamedataManager.This.Root.FullName;
			if (dlg.ShowDialog().GetValueOrDefault())
			{
				var fileViewModel = Open(dlg.FileName);
				ActiveDocument = fileViewModel;
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
			_files.Add(new FileViewModel());
			ActiveDocument = _files.Last();
		}

		#endregion NewCommand
	}
}
