using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Metro;
using SysBanking.Core;
using SysBanking.Core.Reporting;
using XBLRViewer.Controls;
using XBLRViewer.Interfaces;

namespace XBLRViewer
{
    public partial class MainForm : MetroForm, IControlRegistry
    {
        public XBRLFile XBRL { get; set; }
        public MainForm()
        {
            InitializeComponent();
        }
        public List<Control> RegistredControls { get; set; } = new List<Control>();
        public IEnumerable<T> GetControls<T>()
        {
            return RegistredControls.Where(x => x is T).Cast<T>();
        }

        public void InitializeRegistry()
        {
            var a = new Controls.Assets();
            a.BindTo(assets);
            RegistredControls.Add(a);

            var d = new Controls.Details();
            d.BindTo(Properties);
            RegistredControls.Add(d);
        }

        void SetupXBRL()
        {
            foreach (var accountControl in GetControls<IAccountControl>())
            {
                accountControl.OnLoad();
            }
        }

        void UnloadXBRL()
        {
            listViewEx1.Items.Clear();
            foreach (var accountControl in GetControls<IAccountControl>())
            {
                accountControl.OnUnload();
            }
        }
        private void buttonItem18_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var file = openFileDialog1.FileName;
                string label = null;
                if(MessageBoxEx.Show("Do you have a label file?", "Label Loader",  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && openFileDialog1.ShowDialog() == DialogResult.OK)
                     label = openFileDialog1.FileName;
                try
                {
                    UnloadXBRL();
                    XBRL = new XBRLFile(file, label);
                    SetupXBRL();
                }
                catch (Exception ex)
                {
                    listViewEx1.Items.Clear();
                    listViewEx1.Items.Add(ex.Message);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeRegistry();
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            foreach (var filterControl in GetControls<IFilterControl>())
                filterControl.FilterChanged(textBoxItem1.Text);
            
        }

        private void buttonItem10_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var exportable in GetControls<IExportable>())
                {
                    
                    exportable.Export(saveFileDialog1.FileName, new PDFExporter());
                }
            }
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            var f =new Form1();
            f.ShowDialog();
            string label = null;
            if (MessageBoxEx.Show("Do you have a label file?", "Label Loader", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && openFileDialog1.ShowDialog() == DialogResult.OK)
                label = openFileDialog1.FileName;
      
            try
            {
                UnloadXBRL();
                XBRL = new XBRLFile(f.file, label);
                SetupXBRL();
            }
            catch (Exception ex)
            {
                listViewEx1.Items.Clear();
                listViewEx1.Items.Add(ex.Message);
            }
        }
    }
}