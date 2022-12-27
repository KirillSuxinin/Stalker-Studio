using System.Windows.Media;
using PropertyTools.DataAnnotations;

namespace Stalker_Studio.ViewModel
{
	class PaneViewModel : ViewModelBase
	{
		#region fields
		private string _title = null;
		private string _contentId = null;
		private bool _isSelected = false;
		private bool _isActive = false;
		#endregion fields

		public PaneViewModel()
		{
		}

		#region Properties
		[System.ComponentModel.ReadOnly(true)]
		public virtual string Title
		{
			get => _title;
			set
			{
				if (_title != value)
				{
					_title = value;
					OnPropertyChanged(nameof(Title));
				}
			}
		}

		public ImageSource IconSource { get; protected set; }

		public string ContentId
		{
			get => _contentId;
			set
			{
				if (_contentId != value)
				{
					_contentId = value;
					OnPropertyChanged(nameof(ContentId));
				}
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(nameof(IsActive));
				}
			}
		}
		#endregion Properties
	}
}
