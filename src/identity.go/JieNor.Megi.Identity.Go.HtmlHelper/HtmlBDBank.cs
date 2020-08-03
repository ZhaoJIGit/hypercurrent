using Autofac;
using Autofac.Builder;
using Autofac.Configuration;
using Autofac.Core;
using JieNor.Megi.Common.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.Identity.Go.AutoManager;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBDBank
	{
		public static MvcHtmlString SelectOptions()
		{
			List<BDBankTypeViewModel> bDBankTypeList = GetBDBankTypeList();
			StringBuilder stringBuilder = new StringBuilder();
			if (bDBankTypeList != null)
			{
				foreach (BDBankTypeViewModel item in bDBankTypeList)
				{
					stringBuilder.Append($"<option value=\"{item.MItemID}\">{item.MName}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static string DataOptions()
		{
			List<BDBankTypeViewModel> bDBankTypeList = GetBDBankTypeList();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("valueField: 'MItemID',textField: 'MName',data:[");
			stringBuilder.Append("{");
			stringBuilder.AppendFormat("MItemID:'',MName:'',MIsSys:'0',LangID:''");
			stringBuilder.Append("},");
			if (bDBankTypeList != null && bDBankTypeList.Count > 0)
			{
				foreach (BDBankTypeViewModel item in bDBankTypeList)
				{
					stringBuilder.Append("{");
					stringBuilder.AppendFormat("MItemID:'{0}',MName:'{1}',MIsSys:'{2}',LangID:'{3}'", item.MItemID, item.MName, item.MIsSys ? "1" : "0", ContextHelper.MContext.MLCID);
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private static List<BDBankTypeViewModel> GetBDBankTypeList()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			ContainerBuilder containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<BDBankTypeManager>();
			containerBuilder.RegisterModule((IModule)new ConfigurationSettingsReader("autofac"));
			containerBuilder.Register((IComponentContext c) => new BDBankTypeManager(c.Resolve<IBDBankType>()));
			using (IContainer context = containerBuilder.Build(ContainerBuildOptions.None))
			{
				return ((IComponentContext)context).Resolve<BDBankTypeManager>().GetBDBankTypeList();
			}
		}

		public static MvcHtmlString AssembleTransAccount(IVBankBillEntryModel data)
		{
			string text = "";
			if (!string.IsNullOrEmpty(data.MTransAcctName))
			{
				text += data.MTransAcctName;
			}
			if (!string.IsNullOrEmpty(data.MTransAcctNo))
			{
				text += ((!string.IsNullOrEmpty(data.MTransAcctName)) ? ("(" + data.MTransAcctNo + ")") : data.MTransAcctNo);
			}
			if (!string.IsNullOrEmpty(data.MTransNo))
			{
				text += ((!string.IsNullOrEmpty(text)) ? (" " + data.MTransNo) : data.MTransNo);
			}
			text = (string.IsNullOrEmpty(data.MDesc) ? (text + "<p>-</p>") : (text + "<p>" + data.MDesc + "</p>"));
			return new MvcHtmlString(text);
		}

		public static MvcHtmlString AssembleMegiAccount(IVReconcileTranstionListModel data)
		{
			string text = "";
			text = ((!string.IsNullOrEmpty(data.MBankAccountName)) ? data.MBankAccountName : ((data.MDescription == null) ? "" : data.MDescription));
			text = ((!(data.MTargetBillType == "Invoice")) ? (text + "<p>" + data.MReference + "</p>") : (text + "<p>[ " + data.MNumber + " ] " + data.MReference + "</p>"));
			return new MvcHtmlString(text);
		}
	}
}
