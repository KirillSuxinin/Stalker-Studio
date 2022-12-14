using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Stalker_Studio.StalkerWin
{
    public partial class AddIconWinForms : Form
    {
        public AddIconWinForms(string inputDDS)
        {
            InitializeComponent();
            this.InputDDS = inputDDS;

            
            
        }
        private string InputDDS;

        private void AddIconWinForms_Load(object sender, EventArgs e)
        {
            StalkerClass.DDSImage dds = new StalkerClass.DDSImage(File.ReadAllBytes(InputDDS));
            pictureBox1.Image = dds.BitmapImage;


        }
    }
}
