using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stalker_Studio.Common;

namespace Stalker_Studio.ViewModel
{
    class TextFileViewModel : FileViewModel
    {
        public TextFileViewModel(FileModel file)
        {
            //Set the icon only for open documents (just a test)
            //IconSource = ISC.ConvertFromInvariantString(@"pack://application:,,/Images/document.png") as ImageSource;
        }
    }
}
