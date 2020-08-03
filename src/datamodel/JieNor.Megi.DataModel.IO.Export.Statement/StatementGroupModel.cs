using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Statement
{
	[DataContract]
	public class StatementGroupModel
	{
		[DataMember]
		public string DateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ActivityTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ReferenceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string DueDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string InvoiceAmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PaymentsTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BalanceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BalanceDueTitle
		{
			get;
			set;
		}

		[DataMember]
		public string GroupTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BalanceSum
		{
			get;
			set;
		}

		[DataMember]
		public StatementGroupRowCollection StatementGroupRows
		{
			get;
			set;
		}
	}
}
