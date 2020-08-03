using JieNor.Megi.DataModel.BD;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class BDBankTypeManager
	{
		private IBDBankType _bankType;

		public BDBankTypeManager(IBDBankType bankType)
		{
			_bankType = bankType;
		}

		public List<BDBankTypeViewModel> GetBDBankTypeList()
		{
			return _bankType.GetBDBankTypeList(null).ResultData;
		}
	}
}
