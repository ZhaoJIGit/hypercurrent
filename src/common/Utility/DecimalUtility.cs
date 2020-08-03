using System.Globalization;

namespace JieNor.Megi.Common.Utility
{
	public class DecimalUtility
	{
		private static int _decimalLength = 12;

		public static bool IsDecimalValueTooLong(decimal value)
		{
			string tempValue = value.ToString(CultureInfo.InvariantCulture).Split('.')[0].TrimStart('-');
			if (tempValue.Length > _decimalLength)
			{
				return true;
			}
			return false;
		}
	}
}
