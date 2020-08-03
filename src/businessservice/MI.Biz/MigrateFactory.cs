using JieNor.Megi.DataModel.MI;

namespace JieNor.Megi.BusinessService.MI.Biz
{
	public class MigrateFactory
	{
		public static IMigrate CreateInstance(MigrateTypeEnum type)
		{
			switch (type)
			{
			case MigrateTypeEnum.Currency:
				return new CurrencyMigrate();
			case MigrateTypeEnum.Account:
				return new AccountMigrate();
			default:
				return null;
			}
		}
	}
}
