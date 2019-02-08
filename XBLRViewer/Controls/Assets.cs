using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Diwen.Xbrl;
using SysBanking.Core.Reporting;
using XBLRViewer.Interfaces;

namespace XBLRViewer.Controls
{
    public partial class Assets : BaseControl, IFilterControl, IExportable
    {
        public Assets()
        {
            InitializeComponent();
        }

        void AddEntry(string a, string date,object tag, string value=null, string decimals = null,  string unit = null)
        {
            var item = listViewEx1.Items.Add(new ListViewItem(a));
            item.Tag = tag;
            double d = 0;

            if (value != null && decimals != null && double.TryParse(value, out d))
            {
                var dec = 0.0;
                if(double.TryParse(decimals, out dec))
                    item.SubItems.Add((d * Math.Pow(10, dec)) + " " + (unit != null ? unit.Remove(0, unit.IndexOf(':') + 1): ""));
                else item.SubItems.Add((d ) + " " + (unit != null ? unit.Remove(0, unit.IndexOf(':') + 1) : ""));

                item.SubItems.Add(date);
            }
            else
            if (value != null )
            {
                 item.SubItems.Add(value);
                item.SubItems.Add(date);
            }
            else
            {
                item.SubItems.Add("");
                item.SubItems.Add(date);
            }
        }
        public override void OnUnload()
        {
           listViewEx1.Items.Clear();
        }
        public override void OnLoad()
        {
            foreach (var iContext in XBRL.Instance.Contexts)
            {
                if (iContext.Entity.Segment.ExplicitMembers.Count == 0) continue;
                AddEntry(iContext.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name, iContext.Period.ToString(), iContext);
                foreach (var iFact in XBRL.Instance.Facts.Where(x => x.Context != null && x.Context.Id == iContext.Id))
                {

                    if (iFact.Value.Length > 30) continue;
                        AddEntry("        " + iFact.Metric.Name.Remove(0, iFact.Metric.Name.IndexOf(':') + 1), iFact.Context.Period.ToString(), iFact, iFact.Value, iFact.Decimals, iFact.Unit?.Measure);
                }
            }
        }

        private void listViewEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEx1.SelectedIndices.Count > 0)
            {
                var item = listViewEx1.SelectedItems[0].Tag;
                foreach (var assetChangedControl in Program.MainInstance.GetControls<IAssetChangedControl>())
                    assetChangedControl.SelectedAssetChanged(item);
                
            }
        }

        public void FilterChanged(string query)
        {
            listViewEx1.Items.Clear();
            if (string.IsNullOrWhiteSpace(query))
            {
                OnLoad();
                return;
            }
            foreach (var iContext in XBRL.Instance.Contexts)
            {
                if (iContext.Entity.Segment.ExplicitMembers.Count == 0) continue;
                if (iContext.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name.ToLower()
                    .Contains(query.ToLower()) || iContext.Period.ToString().ToLower().Contains(query.ToLower()))
                {
                    // include all its facts
                    AddEntry(iContext.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name, iContext.Period.ToString(), iContext);
                    foreach (var iFact in XBRL.Instance.Facts.Where(x => x.Context != null && x.Context.Id == iContext.Id))
                    {
                        if (iFact.Value.Length > 30) continue;
                        AddEntry("        " + iFact.Metric.Name.Remove(0, iFact.Metric.Name.IndexOf(':') + 1), iFact.Context.Period.ToString(), iFact, iFact.Value, iFact.Decimals, iFact.Unit?.Measure);
                    }
                }
                else if (XBRL.Instance.Facts.Count(x =>
                             x.Context != null && x.Context.Id == iContext.Id &&
                             (x.Metric.Name.ToLower().Contains(query.ToLower()) || (x.Value.ToLower().Contains(query.ToLower())) || x.Context.Period.ToString().ToLower().Contains(query.ToLower()))) > 0)
                {
                    foreach (var iFact in XBRL.Instance.Facts.Where(x =>
                        x.Context != null && x.Context.Id == iContext.Id &&
                        x.Metric.Name.ToLower().Contains(query.ToLower())))
                    {
                        if (iFact.Context.Entity.Segment.ExplicitMembers.Count == 0) continue;
                        AddEntry(iFact.Context.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name, iFact.Context.Period.ToString(), iFact.Context);

                        if (iFact.Value.Length > 30) continue;
                        AddEntry("        " + iFact.Metric.Name.Remove(0, iFact.Metric.Name.IndexOf(':') + 1), iFact.Context.Period.ToString(), iFact, iFact.Value, iFact.Decimals, iFact.Unit?.Measure);
                    }
                }
              
            }
        }

        public void Export(string f,Exporter e)
        {
            e.Export(f, XBRL);
        }
    }
}
