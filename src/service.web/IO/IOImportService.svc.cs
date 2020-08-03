using JieNor.Megi.BusinessContract.IO;
using JieNor.Megi.BusinessService.IO;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.IO;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IO
{
	public class IOImportService : ServiceT<IOImportDataModel>, IIOImport
	{
		private readonly IIOImportBusiness biz = new IOImportBusiness();

		public MActionResult<ImportResult> ImportData(ImportTypeEnum type, IOImportDataModel data, string accessToken = null)
		{
			IIOImportBusiness iIOImportBusiness = biz;
			return base.RunFunc(iIOImportBusiness.ImportData, type, data, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetTemplateModel(ImportTypeEnum type, string accessToken = null)
		{
			IIOImportBusiness iIOImportBusiness = biz;
			return base.RunFunc(iIOImportBusiness.GetTemplateModel, type, accessToken);
		}

		public MActionResult<List<IOTemplateConfigModel>> GetTemplateConfig(ImportTypeEnum type, string accessToken = null)
		{
			IIOImportBusiness iIOImportBusiness = biz;
			return base.RunFunc(iIOImportBusiness.GetTemplateConfig, type, accessToken);
		}

		public MActionResult<List<ImportTemplateDataSource>> GetTemplateBasicData(ImportTypeEnum type, string accessToken = null)
		{
			IIOImportBusiness iIOImportBusiness = biz;
			return base.RunFunc(iIOImportBusiness.GetTemplateBasicData, type, accessToken);
		}
	}
}
