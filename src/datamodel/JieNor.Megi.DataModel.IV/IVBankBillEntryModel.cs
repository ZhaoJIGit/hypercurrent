using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillEntryModel : BizEntryDataModel
	{
		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MSrcEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MRootID
		{
			get;
			set;
		}

		[DataMember]
		public string MMatchBillID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public string MTime
		{
			get;
			set;
		}

		[DataMember]
		public string MTransType
		{
			get;
			set;
		}

		[DataMember]
		public string MTransNo
		{
			get;
			set;
		}

		[DataMember]
		public string MTransAcctName
		{
			get;
			set;
		}

		[DataMember]
		public string MTransAcctNo
		{
			get;
			set;
		}

		[DataMember]
		public string MBillType
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSpentAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSpentSplitAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReceivedAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReceivedSplitAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcSpentAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSrcReceivedAmt
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

		[DataMember]
		public decimal MCheckBalance
		{
			get;
			set;
		}

		[DataMember]
		public int MCheckState
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckerID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCheckDate
		{
			get;
			set;
		}

		[DataMember]
		public string MRef
		{
			get;
			set;
		}

		[DataMember]
		public string MUserRef
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public int MPrevState
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
		public bool MIsCustomer
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSupplier
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOther
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxID
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
		public decimal MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsGLOpen
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

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public int MVoucherStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MFastCodeID
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public IVBankBillEntryModel()
			: base("T_IV_BankBillEntry")
		{
		}
	}
}
