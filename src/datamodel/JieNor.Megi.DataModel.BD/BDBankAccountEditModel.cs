using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankAccountEditModel : BDBankAccountModel
	{
		[DataMember]
		public string MAccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBankStatement
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
		public int MReconcileCount
		{
			get;
			set;
		}

		[DataMember]
		public bool MBankIsUse
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastUpdateDate
		{
			get;
			set;
		}

		[DataMember]
		public ChartModel MBankChartInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool MHasBankBillData
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckExists
		{
			get;
			set;
		}
	}
}
