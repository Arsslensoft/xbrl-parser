using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Diwen.Xbrl;
namespace SysBanking.Core
{
    
    public class XBRLFile
    {
        public string FileName { get; set; }
        public Instance Instance { get; set; }
        public XBRLFile(string f, string label = null)
        {
            Instance = Instance.FromFile(f);
            FileName = f;
            if(label != null)
                LoadLabels(label);
        }
        Dictionary<string,string> labels = new Dictionary<string, string>();
        Dictionary<string, string> labelArcs = new Dictionary<string, string>();

        void LoadLabels(string file)
        {
            XmlDocument x = new XmlDocument();
            x.Load(file);
            foreach (var el in x.GetElementsByTagName("label"))
            {
                var node = el as XmlElement;
                if (node.HasAttribute("xlink:label") && node.HasAttribute("id"))
                {
                    if(!labels.ContainsKey(node.GetAttribute("xlink:label")))
                        labels.Add(node.GetAttribute("xlink:label"), node.InnerText);
                }
            }
            foreach (var el in x.GetElementsByTagName("labelArc"))
            {
                var node = el as XmlElement;
                if (node.HasAttribute("xlink:from") && node.HasAttribute("xlink:to"))
                {
                    if (!labelArcs.ContainsKey(node.GetAttribute("xlink:from")))
                        labelArcs.Add(node.GetAttribute("xlink:from"), node.GetAttribute("xlink:to"));
                }
            }
        }

        public string ResolveLabel(string query)
        {
            if (labels.ContainsKey(query))
                return labels[query];
            else if (labelArcs.ContainsKey(query) && labels.ContainsKey(labelArcs[query]))
                return labels[labelArcs[query]];
            return null;
        }

    }
}
