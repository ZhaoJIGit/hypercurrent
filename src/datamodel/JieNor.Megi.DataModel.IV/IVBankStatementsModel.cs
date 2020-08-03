using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankStatementsModel
	{
		[DataMember]
		public string MID
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
		public string MStartBalance
		{
			get;
			set;
		}

		[DataMember]
		public string MEndBalance
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
		public string MUser
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
		public string MBankTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MHasRecID
		{
			get;
			set;
		}
	}
}
