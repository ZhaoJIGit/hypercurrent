using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLCheckTypeBusiness : IDataContract<GLCheckTypeModel>
	{
		GLCheckTypeDataModel GetCheckTypeDataByType(MContext ctx, int type, bool includeDisabled = false);
	}
}
