using violet-evergardenPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violet-evergardenPortal.Area.Report
{
    public partial class DeliveryStaffPerformanceDetailsBase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "InventorySummary"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "配送人員業績明細表 > 基本";
            if (!IsPostBack)
            {
                DeliveryStaffPerformanceDetailsBaseFrame.Attributes.Add("Src", @"http://rpt.i.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%E9%85%8D%E9%80%81%E4%BA%BA%E5%93%A1%E6%A5%AD%E7%B8%BE%E6%98%8E%E7%B4%B0%E8%A1%A8&rs:Command=Render");
            }
        }
    }
}
