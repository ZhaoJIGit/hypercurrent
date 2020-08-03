using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPVoucherModel
	{
		[DataMember]
		public string MExplanation
		{
			get;
			set;
		}

		public string MNContactID
		{
			get;
			set;
		}

		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxRate
		{
			get;
			set;
		}
	}
}
