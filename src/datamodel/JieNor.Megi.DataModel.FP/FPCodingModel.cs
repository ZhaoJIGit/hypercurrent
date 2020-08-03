using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPCodingModel : BDModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public int MInvoiceType
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MFapiaoNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MInventoryName
		{
			get;
			set;
		}

		[DataMember]
		public string MPSContactName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBizDate
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

		[DataMember]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFixedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFixedTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFixedTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxPercent
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

		[DataMember]
		public string MExplanation
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
		public string MContactIDName
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
		public string MMerItemIDName
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
		public string MTrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Name
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
		public string MDebitAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccountName
		{
			get;
			set;
		}

		[DataMember]
		public int MIndex
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsTop
		{
			get;
			set;
		}

		public FPCodingModel()
			: base("T_FP_Coding")
		{
		}

		public FPCodingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
