using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVVerificationInforModel
	{
		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MContactType
		{
			get;
			set;
		}

		public string MContactName
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
		public string MBizBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MBizType
		{
			get;
			set;
		}

		[DataMember]
		public decimal Amount
		{
			get;
			set;
		}
	}
}
