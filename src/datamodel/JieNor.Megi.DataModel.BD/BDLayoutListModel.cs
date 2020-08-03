using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDLayoutListModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPortal
		{
			get;
			set;
		}

		[DataMember]
		public string MSource
		{
			get;
			set;
		}

		[DataMember]
		public string MRefID
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
		public string MPath
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAcctName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankNo
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
		public int MBankRecCount
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
		public decimal MBankBalance
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeName
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
		public bool MIsCreditCard
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCash
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShowInHome
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
		public DateTime MModifyDate
		{
			get;
			set;
		}
	}
}
