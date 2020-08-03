using JieNor.Megi.Common.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.Go.AutoManager;
using JieNor.Megi.Identity.HtmlHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDAccount
	{
		public static string DataOptions()
		{
			List<BDAccountTypeListModel> bDAccountTypeList = BDAccountManager.GetBDAccountTypeList("");
			if (bDAccountTypeList != null && bDAccountTypeList.Count != 0)
			{
				bDAccountTypeList = (from x in bDAccountTypeList
				orderby x.MAccountGroupID
				select x).ToList();
				var o = (from t in bDAccountTypeList
				select new
				{
					id = t.MItemID,
					name = t.MName,
					@group = t.MAcctGroupName
				}).ToList();
				return $"valueField: 'id',textField: 'name',groupField:'group',data: {MText.ToJson(o)}";
			}
			return "";
		}

		public static MvcHtmlString AccountGroupOptions()
		{
			List<BDAccountGroupEditModel> accountGroupList = BDAccountManager.GetAccountGroupList();
			StringBuilder stringBuilder = new StringBuilder();
			if (accountGroupList != null)
			{
				foreach (BDAccountGroupEditModel item in accountGroupList)
				{
					stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", item.MItemID, MText.Encode(item.MName));
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string BankDataOptions()
		{
			return GetBankDataOptions(BDAccountManager.GetBDBankAccountList(), "");
		}

		public static MvcHtmlString RuleBankOptions()
		{
			List<BDBankAccountEditModel> bDBankAccountList = BDAccountManager.GetBDBankAccountList();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<option value=\"all\">{0}</option>", HtmlLang.Write(LangModule.Bank, "AllBankAccounts", "All bank Accounts").ToString());
			if (bDBankAccountList != null && bDBankAccountList.Count > 0)
			{
				foreach (BDBankAccountEditModel item in bDBankAccountList)
				{
					stringBuilder.AppendFormat("<option value=\"{0}\">{1} ({2})</option>", item.MItemID, MText.Encode(item.MBankName), item.MCyID);
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string BankDataOptions(string currencyId)
		{
			List<BDBankAccountEditModel> bDBankAccountList = BDAccountManager.GetBDBankAccountList();
			if (bDBankAccountList != null && bDBankAccountList.Count != 0)
			{
				return GetBankDataOptions((from t in bDBankAccountList
				where t.MCyID == currencyId
				select t).ToList(), "");
			}
			return "";
		}

		private static string GetBankDataOptions(List<BDBankAccountEditModel> list, string extItems)
		{
			if (list != null && list.Count != 0)
			{
				var o = (from t in list
				select new
				{
					id = t.MItemID,
					bankName = t.MBankName,
					name = $"{t.MBankName}({t.MCyID})",
					currency = t.MCyID
				}).ToList();
				return $"valueField: 'id',textField: 'name',data: {MText.ToJson(o)}";
			}
			return "";
		}
	}
}
