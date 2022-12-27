using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Stalker_Studio.ViewModel
{
	class ToolViewModel : PaneViewModel
	{
		private bool _isVisible = true;
		private string _initLocationName = null;
		protected FrameworkElement _mainControl = null;

		public ToolViewModel(string name)
		{
			Name = name;
			Title = name;
		}

		#region Properties
		public string Name { get; private set; }

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if (_isVisible != value)
				{
					_isVisible = value;
					OnPropertyChanged(nameof(IsVisible));
				}
			}
		}
		public FrameworkElement MainControl
		{
			get => _mainControl;
			set
			{
				_mainControl = value;
			}
		}

		public string InitLocationName { get => _initLocationName; set => _initLocationName = value; }
        #endregion Properties
    }
}
