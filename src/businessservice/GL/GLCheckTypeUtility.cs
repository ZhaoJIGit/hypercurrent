using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLCheckTypeUtility
	{
		private GLUtility dal = new GLUtility();

		public GLCheckTypeDataModel GetCheckTypeDataByType(MContext ctx, int type, bool includeDisabled = false)
		{
			switch (type)
			{
			case 0:
				return dal.GetContactCheckTypeData(ctx);
			case 1:
				return dal.GetEmployeeCheckTypeData(ctx);
			case 2:
				return dal.GetMerItemCheckTypeData(ctx);
			case 3:
				return dal.GetExpItemCheckTypeData(ctx);
			case 4:
				return dal.GetPaItemCheckTypeData(ctx);
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				return dal.GetTrackCheckTypeData(ctx, type, includeDisabled);
			default:
				return null;
			}
		}

		public string GetCheckTypeName(MContext ctx, int type)
		{
			return dal.GetCheckTypeName(ctx, type);
		}
	}
}
