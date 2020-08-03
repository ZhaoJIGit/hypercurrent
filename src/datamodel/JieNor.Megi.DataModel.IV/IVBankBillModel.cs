using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillModel : BizDataModel
	{
		[DataMember]
		public string MBankTypeID
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
		public string MFileName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MImportDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MStartBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMegiBalance
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
		public decimal MSpentAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReceivedCheckAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSpentCheckAmt
		{
			get;
			set;
		}

		[DataMember]
		public bool MCheckState
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
		public string MReference
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
		public BankBillImportSolutionModel ImportSolutionModel
		{
			get;
			set;
		}

		public IVBankBillModel()
			: base("T_IV_BankBill")
		{
		}
	}
}
