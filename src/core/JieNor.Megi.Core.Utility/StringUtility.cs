using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Enum;
using System.Text;

namespace JieNor.Megi.Core.Utility
{
	public class StringUtility
	{
		public static string GetMessageBar(string message, AlertEnum alertEnum = AlertEnum.Default, bool enableClose = true)
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			string arg = alertEnum.ToString().EqualsIgnoreCase("default") ? "" : string.Format(" alert-{0}", alertEnum.ToString().ToLower());
			stringBuilder.AppendFormat("<div class=\"alert{0}\">", arg);
			if (enableClose)
			{
				stringBuilder.Append("<button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button>");
			}
			stringBuilder.AppendFormat("<span class=\"message\">{0}</span></div>", message);
			return stringBuilder.ToString();
		}
	}
}
