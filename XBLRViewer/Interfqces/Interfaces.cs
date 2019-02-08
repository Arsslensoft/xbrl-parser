using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysBanking.Core;
using SysBanking.Core.Reporting;

namespace XBLRViewer.Interfaces
{
    public interface IControlRegistry
    {
        List<Control> RegistredControls { get; set; }
        IEnumerable<T> GetControls<T>();
        void InitializeRegistry();

      
    }
    public interface IAccountControl
    {
        IControlRegistry Registry { get; }
        XBRLFile XBRL {get;}
        void OnLoad();
        void OnUnload();
    }

    public interface IFilterControl
    {
        void FilterChanged(string query);
    }
    public interface IAssetChangedControl
    {
        void SelectedAssetChanged(object asset);
    }
    public interface IExportable
    {
        void Export(string file,Exporter e);
    }

}
