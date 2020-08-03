using JieNor.Megi.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPFapiaoFilterModel : SqlWhere
	{
		[DataMember]
		public int _categoryType = -1;

		[DataMember]
		public string Sort
		{
			get;
			set;
		}

		[DataMember]
		public string Order
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public string MKeyword
		{
			get;
			set;
		}

		[DataMember]
		public decimal? MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MReconcileStatus
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
		public string MCodingStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MVerifyStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MImportID
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
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MVerifyDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowAll
		{
			get;
			set;
		}

		[DataMember]
		public bool MFindFapiao
		{
			get;
			set;
		}

		[DataMember]
		public bool MOnlyFapiao
		{
			get;
			set;
		}

		[DataMember]
		public int MFapiaoCategory
		{
			get
			{
				return _categoryType;
			}
			set
			{
				_categoryType = value;
			}
		}

		[DataMember]
		public List<string> MFapiaoIDs
		{
			get;
			set;
		}

		[DataMember]
		public new int page
		{
			get;
			set;
		}

		[DataMember]
		public new int rows
		{
			get;
			set;
		}

		[DataMember]
		public List<FPFapiaoModel> MFapiaos
		{
			get;
			set;
		}

		[DataMember]
		public int MSaveType
		{
			get;
			set;
		}

		[DataMember]
		public int MMaxEntryCount
		{
			get;
			set;
		}

		[DataMember]
		public List<FPCodingModel> MCodings
		{
			get;
			set;
		}

		[DataMember]
		public int[] MZipCodings
		{
			get;
			set;
		}

		public string OrgVersion
		{
			get;
			set;
		}

		[DataMember]
		public string ConfirmPara
		{
			get;
			set;
		}

		[DataMember]
		public List<string> IdList
		{
			get;
			set;
		}
	}
}
