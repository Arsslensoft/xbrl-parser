using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diwen.Xbrl;
using iTextSharp.text.pdf;

namespace SysBanking.Core.Reporting
{
  public  class PDFExporter : Exporter
  {
      public override string ExporterName { get; set; } = "PDF";
        public override string FileExtensions { get; set; } = "pdf";

        string ConcatStrings(string left, string right)
        {

            if ((left.Length + right.Length) < 110)
            {
                for (int i = (left.Length + right.Length); i < 110 - right.Length; i++)
                    left += " ";
            }
            return left + right;
        }
      void AddEntry(PdfPTable table,string a, string date, object tag, string value = null, string decimals = null, string unit = null)
      {
          table.AddCell(new iTextSharp.text.Phrase(a));

            double d = 0;

          if (value != null && decimals != null && double.TryParse(value, out d))
          {
              var dec = 0.0;
              if (double.TryParse(decimals, out dec))
                  table.AddCell(new iTextSharp.text.Phrase((d * Math.Pow(10, dec)) + " " + (unit != null ? unit.Remove(0, unit.IndexOf(':') + 1) : "")));
              else table.AddCell((d) + " " + (unit != null ? unit.Remove(0, unit.IndexOf(':') + 1) : ""));
          }
          else
          if (value != null)
              table.AddCell(new iTextSharp.text.Phrase(value));
          
          else
              table.AddCell(new iTextSharp.text.Phrase(""));
            
      }
        void ExportReport(iTextSharp.text.Document doc, XBRLFile dt, string additionalData = null)
        {

            iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(2f, 100f, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.Element.ALIGN_CENTER, -1);
            iTextSharp.text.Chunk linebreak = new iTextSharp.text.Chunk(line);
            iTextSharp.text.Font black = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var logo = new iTextSharp.text.Paragraph() { Alignment = 0 };
            logo.Add(new iTextSharp.text.Chunk("XBRL", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 36, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            logo.Add(new iTextSharp.text.Chunk("Viewer", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 36, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(26, 188, 156))));
            doc.Add(logo);
            doc.Add(new iTextSharp.text.Chunk(line));
            doc.Add(new iTextSharp.text.Paragraph(new iTextSharp.text.Chunk(ConcatStrings("Financial Statement", DateTime.Now.ToString()), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK))) { Alignment = 0 });
          
           

            var bf = BaseFont.CreateFont(Environment.CurrentDirectory + @"\arial.ttf", BaseFont.IDENTITY_H, true);
            iTextSharp.text.Font NormalFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font BFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font AssetFont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            iTextSharp.text.Font XFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.RED);
        


            foreach (var iContext in dt.Instance.Contexts)
            {
            

                if (iContext.Entity.Segment.ExplicitMembers.Count == 0) continue;
                doc.NewPage();
                doc.Add(new iTextSharp.text.Paragraph("ASSETS ", BFont));
                doc.Add(new iTextSharp.text.Paragraph(" ", BFont));
                doc.Add(new iTextSharp.text.Paragraph(new iTextSharp.text.Chunk(iContext.Period.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.ITALIC, iTextSharp.text.BaseColor.BLACK))) { Alignment = 2 });
                var ctx_name = iContext.Entity.Segment.ExplicitMembers.FirstOrDefault().Value.Name;
                if (dt.ResolveLabel("us-gaap_" +ctx_name) != null)
                    ctx_name = dt.ResolveLabel("us-gaap_" + ctx_name);
       


                doc.Add(new iTextSharp.text.Paragraph(new iTextSharp.text.Chunk(ConcatStrings("Context",ctx_name), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK))) { Alignment = 0 });
                doc.Add(new iTextSharp.text.Paragraph(" ", BFont));
                doc.Add(new iTextSharp.text.Paragraph(" ", BFont));
                doc.Add(new iTextSharp.text.Paragraph(" ", BFont));
                doc.Add(new iTextSharp.text.Paragraph(" ", BFont));

                PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
                table.AddCell(new PdfPCell() { Phrase = new iTextSharp.text.Phrase("Asset"), BackgroundColor = iTextSharp.text.BaseColor.GRAY });
                table.AddCell(new PdfPCell() { Phrase = new iTextSharp.text.Phrase("Value"), BackgroundColor = iTextSharp.text.BaseColor.GRAY });


                foreach (var iFact in dt.Instance.Facts.Where(x => x.Context != null && x.Context.Id == iContext.Id))
                {

                    if (iFact.Value.Length > 30) continue;
                    var fact_name = iFact.Metric.Name;
                    if (dt.ResolveLabel(fact_name.Replace(":", "_")) != null)
                        fact_name = dt.ResolveLabel(fact_name.Replace(":", "_"));
                    else fact_name = fact_name.Remove(0, fact_name.IndexOf(':') + 1);

                    AddEntry(table, fact_name, iFact.Context.Period.ToString(), iFact, iFact.Value, iFact.Decimals, iFact.Unit?.Measure);
                }
                doc.Add(table);
            }
            //PdfPTable table = new PdfPTable(dt.Columns.Count) { WidthPercentage = 100 };
            //for (int i = 0; i < dt.Columns.Count; i++)
            //    table.AddCell(new PdfPCell() { Phrase = new iTextSharp.text.Phrase(dt.Columns[i].ColumnName), BackgroundColor = iTextSharp.text.BaseColor.GRAY });





            ////table.SetWidths(new float[] { 3, 25, 25, 8 });

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    DataRow GridRow = dt.Rows[i];
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //        table.AddCell(new iTextSharp.text.Phrase(GridRow.ItemArray[j].ToString(), NormalFont));

            //}

            //doc.Add(table);


        }
        public override void Export(string f, XBRLFile dt, string additionalData = null)
        {
            if (File.Exists(f))
                File.Delete(f);
            using (FileStream fs = File.OpenWrite(f))
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(document, fs);
                PageEventHelper pageEventHelper = new PageEventHelper();
                writer.PageEvent = pageEventHelper;
                document.Open();
                ExportReport(document, dt, additionalData);
                document.Close();
            }
        }
    }
    public class PageEventHelper : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;

        iTextSharp.text.Font RunDateFont;
        public PageEventHelper()
        {
            BaseFont bf = BaseFont.CreateFont(Environment.CurrentDirectory + @"\arial.ttf", BaseFont.IDENTITY_H, true);
            RunDateFont = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        }
        public override void OnOpenDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }

        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN.ToString() + " of ";
            float len = this.RunDateFont.BaseFont.GetWidthPoint(text, this.RunDateFont.Size);

            iTextSharp.text.Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(this.RunDateFont.BaseFont, this.RunDateFont.Size);
            cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
            cb.ShowText(text);

            cb.EndText();

            cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));
        }

        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(this.RunDateFont.BaseFont, this.RunDateFont.Size);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }
    }
}
