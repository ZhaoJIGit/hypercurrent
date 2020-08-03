using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessService.MI.Biz
{
	public interface IMigrate
	{
		OperationResult SaveMigration(MContext ctx, MigrateDataModel model);

		OperationResult DeleteMigration(MContext ctx, string migrationId);

		string ViewMigration(MContext ctx, string migrationId);

		int GetlistCount(MContext ctx);
	}
}
