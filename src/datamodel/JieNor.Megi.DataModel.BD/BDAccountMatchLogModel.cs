using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountMatchLogModel : BDModel
	{
		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public int MMatchResult
		{
			get;
			set;
		}

		[DataMember]
		public string MMatchNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MNewNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MParentNumber
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
		public string MSourceCheckType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceCurrency
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTypeId
		{
			get;
			set;
		}

		[DataMember]
		public int MDC
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public string MSysNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MSysName
		{
			get;
			set;
		}

		[DataMember]
		public string MSysAccountTypeId
		{
			get;
			set;
		}

		[DataMember]
		public bool MSysIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public int MSysDC
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckType
		{
			get;
			set;
		}

		[DataMember]
		public string MMigrationID
		{
			get;
			set;
		}

		[DataMember]
		public int MSourceBillKey
		{
			get;
			set;
		}

		[DataMember]
		public string MMegiID
		{
			get;
			set;
		}

		public BDAccountMatchLogModel()
			: base("t_bd_accountmatchlog")
		{
		}
	}
}
