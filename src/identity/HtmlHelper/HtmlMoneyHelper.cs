namespace JieNor.Megi.Identity.HtmlHelper
{
	public static class HtmlMoneyHelper
	{
		public static string ToQuantityString(decimal quantity)
		{
			string text = quantity.ToString();
			int num = text.IndexOf('.') + 2;
			string text2 = text.Substring(0);
			for (int num2 = text.Length - 1; num2 > num; num2--)
			{
				if (!(text.Substring(num2, 1) == "0"))
				{
					return text2;
				}
				text2 = text2.Substring(0, text2.Length - 1);
			}
			return text2;
		}
	}
}
