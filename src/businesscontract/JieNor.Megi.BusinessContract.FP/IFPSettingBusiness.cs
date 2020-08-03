using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.FP
{
	public interface IFPSettingBusiness : IDataContract<FPImportTypeConfigModel>
	{
		OperationResult SaveImportTypeConfig(MContext ctx, FPImportTypeConfigModel model);

		OperationResult SaveFaPiaoSetting(MContext ctx, FPConfigSettingSaveModel model);
	}
}
