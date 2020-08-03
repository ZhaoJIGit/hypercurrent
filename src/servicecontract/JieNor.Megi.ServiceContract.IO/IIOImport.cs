using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IO
{
	[ServiceContract]
	public interface IIOImport
	{
		[OperationContract]
		MActionResult<ImportResult> ImportData(ImportTypeEnum type, IOImportDataModel data, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetTemplateModel(ImportTypeEnum type, string accessToken = null);

		[OperationContract]
		MActionResult<List<IOTemplateConfigModel>> GetTemplateConfig(ImportTypeEnum type, string accessToken = null);

		[OperationContract]
		MActionResult<List<ImportTemplateDataSource>> GetTemplateBasicData(ImportTypeEnum type, string accessToken = null);
	}
}
