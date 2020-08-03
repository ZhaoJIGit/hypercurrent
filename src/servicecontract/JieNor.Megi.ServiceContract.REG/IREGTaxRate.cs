using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.REG
{
	[ServiceContract]
	public interface IREGTaxRate
	{
		[OperationContract]
		MActionResult<List<REGTaxRateModel>> GetTaxRateList(bool ignoreLocale = false, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<REGTaxRateModel>> GetTaxRateListByPage(REGTaxTateListFilterModel filter, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<BDIsCanDeleteModel> IsCanDeleteOrInactive(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveTaxRate(string keyIDs, bool isActive, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> GetUpdateTaxInfo(int changeTaxType, string accessToken = null);

		[OperationContract]
		MActionResult<REGTaxRateModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Delete(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(REGTaxRateModel modelData, string fields = null, string accessToken = null);
	}
}
