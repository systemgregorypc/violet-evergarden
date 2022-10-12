using violetevergardenPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violetevergardenPortal.Area.Training.InfoMaintain
{
	public partial class UnitOrder : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "UnitOrder"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"單位需求調查維護";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				UnitOrderFrame1.Attributes.Add("Src", String.Format("http://{0}/webform6.aspx", domainName));
			}
		}
	}
}
