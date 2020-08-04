using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.COM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Service.Webapi.Helper
{
	public class AccessHelper
	{
		public static MAccessResponseModel GetAccess(string token)
		{
			MAccessResponseModel mAccessResponseModel = new MAccessResponseModel();
			ICOMAccess sysService = ServiceHostManager.GetSysService<ICOMAccess>();
			using (sysService as IDisposable)
			{
				return sysService.GetAccessResultByListNoByCacche(false, null, token).ResultData;
			}
		}

		public static bool HaveAccess(string accessKey, string token)
		{
			try
			{
				if (GetAccess(token).Access.FirstOrDefault((KeyValuePair<string, bool> m) => m.Key == accessKey).Value)
				{
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool HaveAccess(List<string> accessKeys, string token, string logic = "and")
		{
			try
			{
				Dictionary<string, bool> access = GetAccess(token).Access;
				bool result = false;
				List<string>.Enumerator enumerator;
				KeyValuePair<string, bool> keyValuePair;
				if (logic == "and")
				{
					result = true;
					enumerator = accessKeys.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string item = enumerator.Current;
							keyValuePair = access.FirstOrDefault((KeyValuePair<string, bool> m) => m.Key == item);
							if (!keyValuePair.Value)
							{
								result = false;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				else if (logic == "or")
				{
					enumerator = accessKeys.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string item2 = enumerator.Current;
							keyValuePair = access.FirstOrDefault((KeyValuePair<string, bool> m) => m.Key == item2);
							if (keyValuePair.Value)
							{
								result = true;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				return result;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
