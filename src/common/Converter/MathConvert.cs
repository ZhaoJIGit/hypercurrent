using System;

namespace JieNor.Megi.Common.Converter
{
	public static class MathConvert
	{
		private static decimal _decimalAjust = 0.000000001m;

		public static decimal ToRound(this decimal num, int decimals = 2)
		{
			if (num > decimal.Zero)
			{
				return Math.Round(num + _decimalAjust, decimals);
			}
			return Math.Round(num - _decimalAjust, decimals);
		}

		public static decimal ToTruncate(this decimal num)
		{
			if (num > decimal.Zero)
			{
				return Math.Truncate(num + _decimalAjust);
			}
			return Math.Truncate(num - _decimalAjust);
		}
	}
}
