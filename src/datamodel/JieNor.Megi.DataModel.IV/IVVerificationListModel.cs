using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVVerificationListModel
	{
		[DataMember]
		public string VerificationID
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
		public string MContactName
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
		public string MBillID
		{
			get;
			set;
		}

		[DataMember]
		public string MBillNo
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
		public decimal MRate
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
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountTotalFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHaveVerificationAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHaveVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerificationAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceBillID
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillID
		{
			get;
			set;
		}

		[DataMember]
		public List<IVVerificationListModel> HaveVerificationBillList
		{
			get;
			set;
		}

		[DataMember]
		public List<IVVerificationListModel> CanVerificationBillList
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public bool IsMergePay
		{
			get;
			set;
		}

		public IVVerificationListModel()
		{
			HaveVerificationBillList = new List<IVVerificationListModel>();
			CanVerificationBillList = new List<IVVerificationListModel>();
		}
	}
}
