using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.ServiceContract.BAS;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.AutoManager
{
	public class BASDataDictManager
	{
		public static List<BASDataDictionaryModel> GetDictList(string dictTypeCode)
		{
			IBASDataDictionary sysService = ServiceHostManager.GetSysService<IBASDataDictionary>();
			using (sysService as IDisposable)
			{
				return sysService.GetDictList(dictTypeCode, null).ResultData;
			}
		}

		public static List<BASDataDictionaryModel> GetDictList(string dictTypeCode, params string[] valueArray)
		{
			IBASDataDictionary sysService = ServiceHostManager.GetSysService<IBASDataDictionary>();
			using (sysService as IDisposable)
			{
				if (valueArray == null || valueArray.Length == 0)
				{
					return sysService.GetDictList(dictTypeCode, null).ResultData;
				}
				List<string> list = new List<string>();
				foreach (string item in valueArray)
				{
					list.Add(item);
				}
				return sysService.GetDictListByValues(dictTypeCode, list, null).ResultData;
			}
		}
	}
}
