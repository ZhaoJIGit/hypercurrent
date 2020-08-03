using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IO
{
	[ServiceContract]
	public interface IIOSolution
	{
		[OperationContract]
		MActionResult<ImportResult> ImportData(ImportTypeEnum type, IOImportDataModel data, string accessToken = null);

		[OperationContract]
		MActionResult<List<IOSolutionModel>> GetSolutionList(ImportTypeEnum importType, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateSolution(IOSolutionModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IOSolutionModel> GetSolutionModel(string solutionId, string accessToken = null);

		[OperationContract]
		MActionResult<List<IOConfigModel>> GetConfigList(int importType, string accessToken = null);

		[OperationContract]
		MActionResult<List<IOSolutionConfigModel>> GetSolutionConfigList(ImportTypeEnum importType, string solutionId, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveSolution(IOImportDataModel data, bool isFromUpload = false, string accessToken = null);
	}
}
