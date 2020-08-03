using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.IO
{
	public interface IImport
	{
		ImportResult ImportData(MContext ctx, IOImportDataModel data);
	}
}
