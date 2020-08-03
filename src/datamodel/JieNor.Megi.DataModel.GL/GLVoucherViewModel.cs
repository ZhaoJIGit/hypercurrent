using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLVoucherViewModel
	{
		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MRNumber
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
		public decimal MDebitTotal
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCreditTotal
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public int MTransferTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MRVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public int MRVoucherDir
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string PageInfo
		{
			get;
			set;
		}

		[DataMember]
		public int MAttachments
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorName
		{
			get;
			set;
		}

		[DataMember]
		public string MTransferTypeName
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public List<GLVoucherEntryViewModel> MVoucherEntrys
		{
			get;
			set;
		}

		[DataMember]
		public string MAuditor
		{
			get;
			set;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
