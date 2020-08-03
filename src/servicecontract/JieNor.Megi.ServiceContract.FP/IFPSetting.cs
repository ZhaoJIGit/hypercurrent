using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FP
{
	[ServiceContract]
	public interface IFPSetting
	{
		[OperationContract]
		MActionResult<OperationResult> SaveImportTypeConfig(FPImportTypeConfigModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveFaPiaoSetting(FPConfigSettingSaveModel model, string accessToken = null);

		[OperationContract]
		MActionResult<FPImportTypeConfigModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<FPImportTypeConfigModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
