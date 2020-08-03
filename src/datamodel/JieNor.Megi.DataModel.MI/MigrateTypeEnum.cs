using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public enum MigrateTypeEnum
	{
		None,
		Currency,
		BankAccount,
		Account,
		Contact,
		Employee,
		Item,
		Tracking,
		Voucher,
		AccountBalance,
		BasicData = 99
	}
}
