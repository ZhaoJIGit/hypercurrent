using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class BankBillImportSolutionModel : BDModel
	{
		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDefault
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastUsedTime
		{
			get;
			set;
		}

		[DataMember]
		public int MHeaderRowStart
		{
			get;
			set;
		}

		[DataMember]
		public int MDataRowStart
		{
			get;
			set;
		}

		[DataMember]
		public string MDate
		{
			get;
			set;
		}

		[DataMember]
		public string MDateFormat
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
		public string MSpentAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MReceivedAmt
		{
			get;
			set;
		}

		[DataMember]
		public string MBalance
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
		public string MRef
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MTransAcctName2
		{
			get;
			set;
		}

		[DataMember]
		public string MTransAcctNo2
		{
			get;
			set;
		}

		public BankBillImportSolutionModel()
			: base("T_BD_BankBillImportSolution")
		{
		}
	}
}
