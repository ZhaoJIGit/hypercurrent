using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public enum BASOrgScheduleTypeEnum
	{
		[EnumMember]
		OrgSetting = 1,
		[EnumMember]
		FinancialSetting,
		[EnumMember]
		TaxRateSetting,
		[EnumMember]
		Finish,
		[EnumMember]
		Success,
		[EnumMember]
		GLBasicInfo = 11,
		[EnumMember]
		GLChartOfAccount,
		[EnumMember]
		GLOpeningBalance,
		[EnumMember]
		GLFinish,
		[EnumMember]
		GLSuccess
	}
}
