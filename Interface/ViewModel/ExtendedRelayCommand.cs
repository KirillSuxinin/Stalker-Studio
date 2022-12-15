using System;
using System.Windows.Input;
using System.Media;
using System.Windows.Media.Imaging;

namespace Stalker_Studio.ViewModel
{
    class ExtendedRelayCommand: RelayCommand
    {
        private CroppedBitmap _icon = null;
        private string _toolTip = null;

        public ExtendedRelayCommand(Action<object> execute) : this(execute, null) {}

        public ExtendedRelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute) {}

        public ExtendedRelayCommand(Action<object> execute, Predicate<object> canExecute, CroppedBitmap Icon, string ToolTip = "") : base(execute, canExecute)
        {
            _icon = Icon;
            _toolTip = ToolTip;
        }

        public ExtendedRelayCommand(Action<object> execute, Predicate<object> canExecute, string IconKey, string ToolTip = "") : base(execute, canExecute)
        {
            object resource_icon = App.Current.Resources[IconKey];
            if(resource_icon.GetType() == typeof(CroppedBitmap))
                _icon = resource_icon as CroppedBitmap;
            _toolTip = ToolTip;
        }

        public CroppedBitmap Icon { get { return _icon; } }
        public string ToolTip { get { return _toolTip; } }
    }
}
