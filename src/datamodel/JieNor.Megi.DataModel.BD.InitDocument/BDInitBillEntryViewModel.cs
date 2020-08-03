using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.InitDocument
{
	public class BDInitBillEntryViewModel
	{
		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public string MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MExpItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPaItemID
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
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}
	}
}
