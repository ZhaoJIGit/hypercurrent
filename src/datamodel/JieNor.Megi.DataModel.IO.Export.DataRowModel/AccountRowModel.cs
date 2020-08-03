using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class AccountRowModel
	{
		[DisplayName("AccountGroup")]
		[DataMember]
		public string MAcctGroupName
		{
			get;
			set;
		}

		[DisplayName("AccountType")]
		[DataMember]
		public string MAcctTypeName
		{
			get;
			set;
		}

		[DisplayName("AccountStandard")]
		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DisplayName("Direction")]
		[DataMember]
		public string MDC
		{
			get;
			set;
		}

		[DisplayName("IsCheckForCurrency")]
		[DataMember]
		public string MIsCheckForCurrency
		{
			get;
			set;
		}

		[DisplayName("IsSys")]
		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DisplayName("AccountName")]
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DisplayName("Balance")]
		[DataMember]
		public string MBalance
		{
			get;
			set;
		}

		[DisplayName("Number")]
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DisplayName("CheckGroupName")]
		[DataMember]
		public string MCheckGroupName
		{
			get;
			set;
		}
	}
}
