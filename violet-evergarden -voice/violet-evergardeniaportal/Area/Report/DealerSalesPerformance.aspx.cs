using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violet-evergardenPortal.Area.Report
{
    public partial class DealerSalesPerformance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "DealerSalesPerformance"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "經銷商別銷售業績表";
            if (!IsPostBack)
            {
                DealerSalesPerformanceFrame.Attributes.Add("Src", @"http://rpt.i.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e7%b6%93%e9%8a%b7%e5%95%86%e5%88%a5%e9%8a%b7%e5%94%ae%e6%a5%ad%e7%b8%be%e8%a1%a8&rs:Command=Render");
            }
        }
    }
}
