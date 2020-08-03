using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVTransferModel : BizDataModel
	{
		[DataMember]
		public string MFromAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MToAcctID
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
		public string MFromCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MToCyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTransferAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFromTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFromTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MToTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MToTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeLoss
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDiffCurr
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSameTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDiffFromTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDiffToTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFromReconcileAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MFromReconcileAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MToReconcileAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MToReconcileAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public int MFromReconcileStatu
		{
			get;
			set;
		}

		[DataMember]
		public string MFromName
		{
			get;
			set;
		}

		[DataMember]
		public string MToName
		{
			get;
			set;
		}

		[DataMember]
		public int MToReconcileStatu
		{
			get;
			set;
		}

		[DataMember]
		public IVTransferPermissionModel MActionPermission
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

		public IVTransferModel()
			: base("T_IV_Transfer")
		{
		}
	}
}
