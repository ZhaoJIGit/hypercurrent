using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RI
{
	public class RIInvoiceModel
	{
		[DataMember]
		public int RowCounts
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankIDFrom
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeIDFrom
		{
			get;
			set;
		}
	}
}
