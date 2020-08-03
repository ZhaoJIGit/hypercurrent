using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASOrgInitSettingBusiness : IDataContract<BASOrgInitSettingModel>
	{
		OperationResult GLSetupSuccess(MContext ctx);
	}
}
