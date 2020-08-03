using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FC
{
	public interface IFCVoucherModuleBusiness : IDataContract<FCVoucherModuleModel>
	{
		List<FCVoucherModuleModel> GetVoucherModuleListWithNoEntry(MContext ctx);

		FCVoucherModuleModel GetVoucherModelWithEntry(MContext ctx, string PKID);

		List<FCVoucherModuleModel> GetVoucherModuleList(MContext ctx, List<string> pkIDS);

		DataGridJson<FCVoucherModuleModel> GetVoucherModuleModelPageList(MContext ctx, GLVoucherListFilterModel filter);

		FCVoucherModuleModel UpdateVoucherModuleModel(MContext ctx, FCVoucherModuleModel model);

		FCVoucherModuleModel GetVoucherModule(MContext ctx, string pkID = null);
	}
}
