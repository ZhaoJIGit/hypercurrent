using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IO
{
	public interface IIOImportBusiness
	{
		ImportResult ImportData(MContext ctx, ImportTypeEnum type, IOImportDataModel data);

		ImportTemplateModel GetTemplateModel(MContext ctx, ImportTypeEnum type);

		List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, ImportTypeEnum type);

		List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, ImportTypeEnum type);
	}
}
