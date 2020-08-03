using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.SYS
{
	public class SYSCouponUserRepository : DataServiceT<SYSCouponUserModel>
	{
		public SYSCouponUserModel GetCouponUserModel(string couponID, string userID)
		{
			return base.GetDataModelByFilter(new MContext
			{
				IsSys = true
			}, new SqlWhere().Equal("MUserID", userID));
		}
	}
}
