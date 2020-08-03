using JieNor.Megi.EntityModel.MultiLanguage;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlMonth
	{
		public static MvcHtmlString SelectOptions(string language = "0x0009")
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected O, but got Unknown
			string[] array = new string[12]
			{
				"Month_Jan",
				"Month_Feb",
				"Month_Mar",
				"Month_Apr",
				"Month_May",
				"Month_Jun",
				"Month_Jul",
				"Month_Aug",
				"Month_Sep",
				"Month_Oct",
				"Month_Nov",
				"Month_Dec"
			};
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				string defaultValue = array[i].Split('_')[1];
				string text = LangHelper.GetText(language, LangModule.BD, array[i], defaultValue);
				stringBuilder.AppendLine($"<option value='{(i + 1).ToString()}'>{text}</option>");
			}
			return new MvcHtmlString(stringBuilder.ToString());
		}
	}
}
