using JieNor.Megi.Core;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountListFilterModel : SqlWhere
	{
		[DataMember]
		public string Group
		{
			get;
			set;
		}

		[DataMember]
		public string GroupID
		{
			get;
			set;
		}

		[DataMember]
		public string Keyword
		{
			get;
			set;
		}

		[DataMember]
		public bool IsActive
		{
			get;
			set;
		}

		[DataMember]
		public bool IsAll
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
		public string ParentCodes
		{
			get;
			set;
		}

		[DataMember]
		public string NotParentCodes
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MStartAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MEndAccountID
		{
			get;
			set;
		}

		public bool IncludeCheckType
		{
			get;
			set;
		}

		public int StartYearPeriod
		{
			get;
			set;
		}

		public int EndYearPeriod
		{
			get;
			set;
		}

		public bool IncludeAllLangName
		{
			get;
			set;
		}

		[DataMember]
		public int AccountLevel
		{
			get;
			set;
		}

		[DataMember]
		public bool IsLeafAccount
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeDisable
		{
			get;
			set;
		}

		[DataMember]
		public int AccountStartIndex
		{
			get;
			set;
		}

		[DataMember]
		public int AccountEndIndex
		{
			get;
			set;
		}
	}
}
