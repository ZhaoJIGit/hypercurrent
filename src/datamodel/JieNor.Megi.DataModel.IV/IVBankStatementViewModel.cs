using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankStatementViewModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
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
		public string MTransType
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
		public string MCode
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
		public string MUserRef
		{
			get;
			set;
		}

		[DataMember]
		public string MAnalysisCode
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
		public string MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckState
		{
			get;
			set;
		}
	}
}
