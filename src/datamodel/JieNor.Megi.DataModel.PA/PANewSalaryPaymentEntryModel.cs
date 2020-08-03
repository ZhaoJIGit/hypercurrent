using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PANewSalaryPaymentEntryModel
	{
		[DataMember]
		public PayrollItemEnum MItemType
		{
			get;
			set;
		}

		[DataMember]
		public string MPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public decimal OrgSSPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal OrgHFPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}
	}
}
