using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Identity.AutoManager;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlDataDict
	{
		public static MvcHtmlString SelectOptions(string dictTypeCode)
		{
			return SelectOptions(dictTypeCode, (string[])null);
		}

		public static MvcHtmlString SelectOptions(string dictTypeCode, params string[] values)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			List<BASDataDictionaryModel> dictList = BASDataDictManager.GetDictList(dictTypeCode, values);
			StringBuilder stringBuilder = new StringBuilder();
			if (dictList != null)
			{
				foreach (BASDataDictionaryModel item in dictList)
				{
					stringBuilder.Append($"<option value=\"{item.DictValue}\">{item.DictName}</option>");
				}
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}

		public static MvcHtmlString DataOptions(string dictTypeCode)
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			string arg = "";
			List<BASDataDictionaryModel> dictList = BASDataDictManager.GetDictList(dictTypeCode);
			StringBuilder stringBuilder = new StringBuilder();
			if (dictList != null)
			{
				for (int i = 0; i < dictList.Count; i++)
				{
					BASDataDictionaryModel bASDataDictionaryModel = dictList[i];
					if (i == 0)
					{
						arg = bASDataDictionaryModel.DictValue;
					}
					stringBuilder.Append("{");
					stringBuilder.Append($"id: '{bASDataDictionaryModel.DictValue}', name: '{bASDataDictionaryModel.DictName}'");
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return new MvcHtmlString($"valueField: 'id',textField: 'name',data: [{stringBuilder.ToString()}],defaultValue:'{arg}'");
		}

		public static string DataOptionsByGroup(string dictTypeCode)
		{
			List<BASDataDictionaryModel> dictList = BASDataDictManager.GetDictList(dictTypeCode);
			if (dictList == null || dictList.Count == 0)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (BASDataDictionaryModel item in dictList)
			{
				stringBuilder.Append("{");
				stringBuilder.Append($"id: '{item.DictValue}', name: '{item.DictName}', group: '{item.ParentDictCode}'");
				stringBuilder.Append("},");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return $"valueField: 'id',textField: 'name',groupField:'group',data: [{stringBuilder.ToString()}]";
		}
	}
}
