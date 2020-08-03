using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVViewStatementModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MDate
		{
			get;
			set;
		}

		[DataMember]
		public string MActivity
		{
			get;
			set;
		}

		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public string MDueDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInvoiceAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPayments
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalance
		{
			get;
			set;
		}
	}
}
