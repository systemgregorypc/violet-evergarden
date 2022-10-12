using violetevergardenPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace violetevergardenPortal.Area.Training.InfoMaintain
{
	public partial class TrainingClass : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "TrainingClass"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"訓練課程新增";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				TrainingClassFrame1.Attributes.Add("Src", String.Format("http://{0}/Class_Data_Add.aspx",domainName));
			}
		}
	}
}
