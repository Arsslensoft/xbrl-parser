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
using XBLRViewer.Interfaces;

namespace XBLRViewer.Controls
{
    public partial class Details : BaseControl, IAssetChangedControl
    {
        public Details()
        {
            InitializeComponent();
        }
        void UpdateColumnsWidth()
        {
            foreach (var messagesListColumn in listViewEx1.Columns)
                (messagesListColumn as ColumnHeader).Width = listViewEx1.Width / (listViewEx1.Columns.Count);
        }

        void AddEntry(string key, string value)
        {
        var item =            listViewEx1.Items.Add(new ListViewItem(key));
            item.SubItems.Add(value);
        }
        public void SelectedAssetChanged(object asset)
        {
            listViewEx1.Items.Clear();
            if (asset is Context)
            {
                var ctx = asset as Context;
                AddEntry("Id", ctx.Id);
                AddEntry("Period", ctx.Period.ToString());
                if (ctx.Entity != null)
                {
                    if (ctx.Entity.Identifier != null)
                    {
                        AddEntry("Identifier", ctx.Entity.Identifier.Value);
                        AddEntry("Identifier Scheme", ctx.Entity.Identifier.Scheme);
                    }
                    if (ctx.Entity.Segment != null)
                    {
                        AddEntry("Has Members", ctx.Entity.Segment.HasMembers.ToString());
                        if (ctx.Entity.Segment.ExplicitMembers.Count > 0)
                        {
                            foreach (var segmentExplicitMember in ctx.Entity.Segment.ExplicitMembers)
                                AddEntry("Explicit Member ", "Code=" + segmentExplicitMember.MemberCode + ", Value="+segmentExplicitMember.Value);
                            
                        }
                    }
                }
            
            }
            else if (asset is Fact)
            {
                var a = asset as Fact;
                AddEntry("Name", a.Metric.Name);
                if(a.Value != null)
                    AddEntry("Value", a.Value);
                if (a.Decimals != null)
                    AddEntry("Decimals", a.Decimals);
                if (a.Unit != null)
                {
                    AddEntry("Unit Id", a.Unit.Id);
                    AddEntry("Unit Measure", a.Unit.Measure);
                }

                var ctx = a.Context;
                AddEntry("Id", ctx.Id);
                AddEntry("Period", ctx.Period.ToString());
                if (ctx.Entity != null)
                {
                    if (ctx.Entity.Identifier != null)
                    {
                        AddEntry("Identifier", ctx.Entity.Identifier.Value);
                        AddEntry("Identifier Scheme", ctx.Entity.Identifier.Scheme);
                    }
                    if (ctx.Entity.Segment != null)
                    {
                        AddEntry("Has Members", ctx.Entity.Segment.HasMembers.ToString());
                        if (ctx.Entity.Segment.ExplicitMembers.Count > 0)
                        {
                            foreach (var segmentExplicitMember in ctx.Entity.Segment.ExplicitMembers)
                                AddEntry("Explicit Member ", "Code=" + segmentExplicitMember.MemberCode + ", Value=" + segmentExplicitMember.Value);

                        }
                    }
                }
            }
        }

        private void Details_SizeChanged(object sender, EventArgs e)
        {
            UpdateColumnsWidth();
        }
    }
}
