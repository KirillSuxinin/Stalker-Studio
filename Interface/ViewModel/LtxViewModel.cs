using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stalker_Studio.StalkerClass;

namespace Stalker_Studio.ViewModel
{

    class LtxViewModel: FileViewModel
    {
        protected LtxModel _file;

        LtxViewModel() : base() { }

        LtxViewModel(LtxModel file) : base()
        {
            _file = file;
        }

        LtxViewModel(string path) : base()
        {
            //_file = file;
        }


    }
}
