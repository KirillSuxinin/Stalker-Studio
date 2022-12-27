using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stalker_Studio.ViewModel
{
    class PropertiesViewModel : ToolViewModel
    {
        IEnumerable _sourceList = null;
        object _source = null;

        public PropertiesViewModel(string name) : base(name)
        {
            InitLocationName = "DetailAnchorablePane";
        }

        public object Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable SourceList
        {
            get => _sourceList;
            set
            {
                _sourceList = value;
                OnPropertyChanged();
            }
        }
    }
}
