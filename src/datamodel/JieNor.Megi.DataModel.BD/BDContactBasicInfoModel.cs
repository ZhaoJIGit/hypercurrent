using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactBasicInfoModel
	{
		[DataMember]
		public string MDefaultCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDefaultDiscount
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultSaleTaxID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultPurchaseTaxID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime BillDefaultDueDate
		{
			get;
			set;
		}

		[DataMember]
		public int MSaleDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public string MPurDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public int MPurchaseDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public DateTime InvoiceDefaultDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseTrackItem5
		{
			get;
			set;
		}
	}
}
