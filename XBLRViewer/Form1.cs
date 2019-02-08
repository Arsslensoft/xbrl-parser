using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar.Metro;

namespace XBLRViewer
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string file;
        private void buttonX1_Click(object sender, EventArgs e)
        {
            file = Path.GetTempFileName();
            new WebClient().DownloadFile(textBoxX1.Text, file);
            this.Close();
        }
    }
}
