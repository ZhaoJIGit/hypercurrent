using JieNor.Megi.Common.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.Go.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDOrgUser
	{
		public static MvcHtmlString SelectOptions()
		{
			List<BDEmployeesModel> orgUserList = BDOrgUserManager.GetOrgUserList();
			StringBuilder stringBuilder = new StringBuilder();
			if (orgUserList != null)
			{
				MContext mContext = ContextHelper.MContext;
				stringBuilder.AppendFormat("<option value=\"{0}\" selected=\"selected\">{1}</option>", "0", LangHelper.GetText(mContext.MLCID, LangKey.None));
				for (int i = 0; i < orgUserList.Count; i++)
				{
					BDEmployeesModel bDEmployeesModel = orgUserList[i];
					stringBuilder.Append($"<option value=\"{bDEmployeesModel.MUserID}\">{bDEmployeesModel.MEmail}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
