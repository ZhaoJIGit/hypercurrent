using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.EntityModel.Context;
using System;

namespace JieNor.Megi.BusinessService.MI.Biz
{
	public class AccountMigrate : MigrateBase, IMigrate
	{
		public OperationResult SaveMigration(MContext ctx, MigrateDataModel model)
		{
			throw new NotImplementedException();
		}

		public OperationResult DeleteMigration(MContext ctx, string migrationId)
		{
			throw new NotImplementedException();
		}

		public string ViewMigration(MContext ctx, string migrationId)
		{
			throw new NotImplementedException();
		}

		public int GetlistCount(MContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
