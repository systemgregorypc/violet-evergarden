using violetevergardenPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violetevergardenPortal.Area.Training.InfoMaintain
{
	public partial class UnitBasicSetting : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "UnitBasicSetting"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"訓練機構基本資料維護";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				UnitBasicSettingFrame1.Attributes.Add("Src", String.Format("http://{0}/webform8.aspx", domainName));
			}
		}
	}
}
