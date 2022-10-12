using violet-evergardenPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violet-evergardenPortal.Area.Training.InfoMaintain
{
	public partial class UnitTranslate : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "UnitTranslate"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"訓練課程維護";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				UnitTranslateFrame1.Attributes.Add("Src", String.Format("http://{0}/Class_Data_Edit.aspx", domainName));
			}
		}
	}
}
