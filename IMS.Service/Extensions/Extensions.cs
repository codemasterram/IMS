using Softech.Reporting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IMS.Logic.Extensions
{
    public static class Extensions
    {
        public static string ToHTML(this Softech.Reporting.Report report, string link = "", string linktext = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div>");
            sb.Append("<div class='box-header with-border'>");
            sb.AppendFormat(@"<h3 class='box-title'>{0}</h3>", report.Header1);
            sb.Append("</div>");
            sb.Append("<table class='table table-condensed table-striped excel-data' style=''>");
            sb.Append("<thead class='bg-light-blue'>");
            string headerText = string.Empty;
            XElement tr = XElement.Parse("<tr />");

            foreach (Column col in report.Columns)
            {
                if (col.Visible == true)
                {
                    if (col.HeaderText != headerText)
                    {
                        XElement td = XElement.Parse(string.Format(@"<th style="""">{0}</th>", col.HeaderText.ValidateHtmlEntityCharacter()));
                        tr.Add(td);
                        headerText = col.HeaderText;
                    }
                    else
                    {
                        if (tr.Elements("th").Last().Attribute("colspan") != null)
                        {
                            tr.Elements("th").Last().Attribute("colspan").Value = (Convert.ToInt32(tr.Elements("th").Last().Attribute("colspan").Value) + 1).ToString();
                        }
                        else
                        {
                            tr.Elements("th").Last().SetAttributeValue("colspan", "2");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(link))
            {
                XElement tdLink = XElement.Parse("<th />");
                tr.Add(tdLink);

                foreach (var subHead in report.SubHeaders)
                {
                    if (subHead.Value.Last() == "\t")
                        subHead.Value.Add("\t\t");
                    else
                        subHead.Value.Add("\t");
                }
            }

            sb.Append(tr.ToString());

            foreach (var subHead in report.SubHeaders)
            {
                headerText = string.Empty;
                tr = XElement.Parse("<tr />");
                foreach (string s in subHead.Value)
                {
                    if (s != headerText)
                    {
                        XElement td = XElement.Parse(string.Format(@"<th style="""">{0}</th>", s.ValidateHtmlEntityCharacter())); //#81ADC5
                        tr.Add(td);
                        headerText = s;
                    }
                    else
                    {
                        if (tr.Elements("th").Last().Attribute("colspan") != null)
                        {
                            tr.Elements("th").Last().Attribute("colspan").Value = (Convert.ToInt32(tr.Elements("th").Last().Attribute("colspan").Value) + 1).ToString();
                        }
                        else
                        {
                            tr.Elements("th").Last().SetAttributeValue("colspan", "2");
                        }
                    }
                }
                sb.Append(tr.ToString());
            }
            sb.Append("</thead>");

            sb.Append("<tbody>");
            foreach (Row ro in report.Rows)
            {
                sb.Append("<tr>");
                foreach (Column col in report.Columns)
                {
                    if (col.Visible == true)
                    {
                        XElement td = XElement.Parse(@"<td style="""" />");
                        XAttribute style = td.Attribute("style");
                        td.Value = ro.Cells[col.Name].Value == null ? "" : ro.Cells[col.Name].Value.ValidateHtmlEntityCharacter();

                        if (ro.Cells[col.Name].BackColor != null && ro.Cells[col.Name].BackColor.A != 0)
                        {
                            //style.Value += ";";// + string.Format("background:#{0:X2}{1:X2}{2:X2}", ro.Cells[col.Name].BackColor.R, ro.Cells[col.Name].BackColor.G, ro.Cells[col.Name].BackColor.B, ro.Cells[col.Name].BackColor.A);
                        }

                        if (ro.Cells[col.Name].Value != null && ro.Cells[col.Name].Value.StartsWith("Rem:"))
                        {
                            //style.Value += ";";//+ "font-size:10px; color:#2C4D5F; padding:2px 5px 0 15px";
                        }


                        if (ro.Cells[col.Name].Font != null)
                        {
                            if (ro.Cells[col.Name].Font.Bold)
                                style.Value += ";" + "font-weight:bold";

                            //style.Value += ";"; // + "font-family:" + ro.Cells[col.Name].Font.OriginalFontName;
                        }

                        if (ro.Cells[col.Name].Alignment == StringAlignment.Far)
                        {
                            style.Value += ";" + "text-align:right";
                        }
                        else
                        {
                            //style.Value += ";" + "text-align:left";
                        }

                        if (ro.Cells[col.Name].Border.Top == BorderStyle.Single)
                        {
                            style.Value += ";";// + "border-top:1px solid " + gridLineColor;
                        }

                        if (ro.Cells[col.Name].Border.Right == BorderStyle.Single)
                        {
                            //style.Value += ";" + "border-right:1px solid " + gridLineColor;
                        }

                        if (ro.Cells[col.Name].Border.Buttom == BorderStyle.Single)
                        {
                            //style.Value += ";" + "border-bottom:1px solid " + gridLineColor;
                        }

                        if (ro.Cells[col.Name].Border.Left == BorderStyle.Single)
                        {
                            //style.Value += ";" + "border-left:1px solid " + gridLineColor;
                        }

                        td.SetAttributeValue("style", style.Value.TrimStart(';'));
                        sb.Append(td.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(link))
                {
                    XElement td = XElement.Parse(@"<td style="""" />");

                    int id = 0;
                    if (ro.Cells["Id"].Value != null && int.TryParse(ro.Cells["Id"].Value.ToString(), out id))
                    {
                        string linkCopy = link;
                        if (link.Contains("/0?"))
                        {
                            linkCopy = linkCopy.Replace("/0?", "/" + ro.Cells["Id"].Value + "?");
                        }
                        td.Add(XElement.Parse(string.Format(@"<a href=""{0}"" class=""report-detail-link"">{1}</a>", linkCopy, linktext)));
                    }

                    sb.Append(td.ToString());
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody>");

            sb.Append("</table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        public static bool IsNumeric(this string s)
        {
            int i;
            return int.TryParse(s, out i);
        }
    }
}
