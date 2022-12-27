using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using Stalker_Studio.Common;

namespace Stalker_Studio.ViewModel
{
    class TextFileViewModel : FileViewModel
    {
        TextDocument _document = new TextDocument();
        public TextFileViewModel(TextFileModel file) : base(file)
        {
            _document.Text = file.Text;
            _document.TextChanged += Document_TextChanged;
        }
        public TextFileViewModel(string fullName) : base(new TextFileModel(fullName))
        {
            _document.TextChanged += Document_TextChanged;
        }
        public TextFileViewModel() : base(new TextFileModel())
        {
            _document.TextChanged += Document_TextChanged;
        }

        public TextDocument Document 
        {
            get { return _document; }
            set { _document = value; }
        }

        private void Document_TextChanged(object sender, EventArgs e)
        {
            (File as TextFileModel).Text = _document.Text;
        }
    }
}
