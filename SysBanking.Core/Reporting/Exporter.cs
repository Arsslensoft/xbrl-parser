using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diwen.Xbrl;

namespace SysBanking.Core.Reporting
{
   public abstract class Exporter
    { 
        public abstract string ExporterName { get; set; }
        public abstract string FileExtensions { get; set; }
        public abstract void Export(string f, XBRLFile dt, string additionalData = null);
      
    }
}
