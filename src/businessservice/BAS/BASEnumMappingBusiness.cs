using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASEnumMappingBusiness
	{
		public string GetInternalValue(MContext ctx, string type, object publicValue, string apiFieldName = null, bool isRequired = true)
		{
			return BASEnumMappingRepository.GetInternalValue(ctx, type, publicValue, apiFieldName, isRequired);
		}

		public object GetPublicValue(MContext ctx, string type, object internalValue)
		{
			return BASEnumMappingRepository.GetPublicValue(ctx, type, internalValue);
		}
	}
}
