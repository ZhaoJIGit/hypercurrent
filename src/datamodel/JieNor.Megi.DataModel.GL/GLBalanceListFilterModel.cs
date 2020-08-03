using JieNor.Megi.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	public class GLBalanceListFilterModel : SqlWhere
	{
		[DataMember]
		public string searchFilter
		{
			get;
			set;
		}

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
		public int StartYear
		{
			get;
			set;
		}

		[DataMember]
		public int EndYear
		{
			get;
			set;
		}

		[DataMember]
		public int StartPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int EndPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int StartYearPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int EndYearPeriod
		{
			get;
			set;
		}

		[DataMember]
		public List<string> AccountIDS
		{
			get;
			set;
		}

		[DataMember]
		public List<string> AccountTypes
		{
			get;
			set;
		}

		[DataMember]
		public string KeyWord
		{
			get;
			set;
		}

		[DataMember]
		public string Status
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public bool RequestIsNotFromFormula
		{
			get;
			set;
		}

		[DataMember]
		public bool IsIncludeChildrenAccount
		{
			get;
			set;
		}

		[DataMember]
		public string ApiModule
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeCheckType
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> CheckTypeValueList
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
		public bool ExcludeCLPVoucher
		{
			get;
			set;
		}

		[DataMember]
		public int FormaluDataType
		{
			get;
			set;
		}

		[DataMember]
		public string From
		{
			get;
			set;
		}
	}
}
