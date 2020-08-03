using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Identity.Go.AutoManager;
using System.Collections.Generic;
using System.Text;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlBASCountry
	{
		public static string DataOptions()
		{
			List<BASCountryModel> countryList = BASCountryManager.GetCountryList();
			if (countryList != null && countryList.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (BASCountryModel item in countryList)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(string.Format("id: '{0}', name: '{1}'", item.MItemID, item.MCountryName.Replace("'", "\\'")));
					stringBuilder.Append("},");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				return $"valueField: 'id',textField: 'name',data: [{stringBuilder.ToString()}]";
			}
			return "";
		}

		public static string ProvinceDataOptions()
		{
			List<BASProvinceModel> provinceList = BASCountryManager.GetProvinceList("106");
			if (provinceList != null && provinceList.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (BASProvinceModel item in provinceList)
				{
					stringBuilder.Append("{");
					stringBuilder.Append($"id: '{item.MItemID}', name: '{item.MName}'");
					stringBuilder.Append("},");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				return $"valueField: 'id',textField: 'name',data: [{stringBuilder.ToString()}]";
			}
			return "";
		}
	}
}
