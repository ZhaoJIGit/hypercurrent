using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IO
{
	public interface IIOSolutionBusiness
	{
		ImportResult ImportData(MContext ctx, ImportTypeEnum type, IOImportDataModel data);

		List<IOSolutionModel> GetSolutionList(MContext ctx, ImportTypeEnum importType);

		OperationResult UpdateSolution(MContext ctx, IOSolutionModel model);

		IOSolutionModel GetSolutionModel(MContext ctx, string solutionId);

		List<IOConfigModel> GetConfigList(MContext ctx, int importType);

		List<IOSolutionConfigModel> GetSolutionConfigList(MContext ctx, ImportTypeEnum importType, string solutionId);

		OperationResult SaveSolution(MContext ctx, IOImportDataModel model, bool isFromUpload = false);
	}
}
