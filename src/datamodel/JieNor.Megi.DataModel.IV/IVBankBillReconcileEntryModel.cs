using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillReconcileEntryModel : BizEntryDataModel
	{
		[DataMember]
		public string MTargetBillType
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
		public decimal MSpentAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReceiveAmtFor
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
		public DateTime MDate
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
		public string MRef
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAdjustAmt
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBankFeeAmt
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsInterestAmt
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

		public IVBankBillReconcileEntryModel()
			: base("T_IV_BankBillReconcileEntry")
		{
		}
	}
}
