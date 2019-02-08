using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diwen.Xbrl;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
           var i = Instance.FromFile("msft-20141231.xml");
            foreach (var iContext in i.Contexts)
            {
                if (iContext.Entity.Segment.ExplicitMembers.Count == 0) continue;
                Console.WriteLine(iContext.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name);
                foreach (var iFact in i.Facts.Where(x => x.Context != null && x.Context.Id == iContext.Id))
                {
                    Console.WriteLine("\t" + iFact.Metric.Name.Remove(0, iFact.Metric.Name.IndexOf(':') + 1) + " \t" + iFact.Value);
                }
            }
        }
    }
}
