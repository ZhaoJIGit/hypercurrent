using System;
using System.Security.Cryptography;

namespace JieNor.Megi.Common.Utility
{
	public class AuthorizationManager
	{
		internal static readonly RandomNumberGenerator CryptoRandomDataGenerator = new RNGCryptoServiceProvider();

		public static bool CheckStateCodeValid(string code, string ip)
		{
			return true;
		}

		public static string GenerateStateCode(string ip)
		{
			return Guid.NewGuid().ToString("N");
		}

		public static string GenerateAccessToken()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static string GenerateAccessCode()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
