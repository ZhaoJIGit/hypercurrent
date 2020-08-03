using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.MI
{
	[DataContract]
	public enum MigrateProgressEnum
	{
		None,
		ConnectDB,
		BasicData,
		Voucher,
		Finish
	}
}
