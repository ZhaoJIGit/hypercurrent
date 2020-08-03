using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.SYS
{
	public class SYSOrgAppRepository : DataServiceT<SYSOrgAppModel>
	{
		public SYSOrgAppModel GetOrgAppModel(string orgId)
		{
			return base.GetDataModelByFilter(new MContext
			{
				IsSys = true
			}, new SqlWhere().Equal("MOrgID", orgId));
		}
	}
}
