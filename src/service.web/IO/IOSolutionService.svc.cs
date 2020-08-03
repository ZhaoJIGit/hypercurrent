using JieNor.Megi.BusinessContract.IO;
using JieNor.Megi.BusinessService.IO;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.ServiceContract.IO;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IO
{
	public class IOSolutionService : ServiceT<IOSolutionModel>, IIOSolution
	{
		private readonly IIOSolutionBusiness biz = new IOSolutionBusiness();

		public MActionResult<ImportResult> ImportData(ImportTypeEnum type, IOImportDataModel data, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.ImportData, type, data, accessToken);
		}

		public MActionResult<List<IOSolutionModel>> GetSolutionList(ImportTypeEnum importType, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.GetSolutionList, importType, accessToken);
		}

		public MActionResult<OperationResult> UpdateSolution(IOSolutionModel model, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.UpdateSolution, model, accessToken);
		}

		public MActionResult<IOSolutionModel> GetSolutionModel(string solutionId, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.GetSolutionModel, solutionId, accessToken);
		}

		public MActionResult<List<IOConfigModel>> GetConfigList(int importType, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.GetConfigList, importType, accessToken);
		}

		public MActionResult<List<IOSolutionConfigModel>> GetSolutionConfigList(ImportTypeEnum importType, string solutionId, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.GetSolutionConfigList, importType, solutionId, accessToken);
		}

		public MActionResult<OperationResult> SaveSolution(IOImportDataModel model, bool isFromUpload = false, string accessToken = null)
		{
			IIOSolutionBusiness iIOSolutionBusiness = biz;
			return base.RunFunc(iIOSolutionBusiness.SaveSolution, model, isFromUpload, accessToken);
		}
	}
}
