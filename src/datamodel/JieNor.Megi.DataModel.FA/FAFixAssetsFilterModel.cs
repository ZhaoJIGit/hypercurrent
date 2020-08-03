using JieNor.Megi.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FAFixAssetsFilterModel : SqlWhere
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroup
		{
			get;
			set;
		}

		[DataMember]
		public string CheckGroupValue
		{
			get;
			set;
		}

		[DataMember]
		public string OrderField
		{
			get;
			set;
		}

		[DataMember]
		public string OrderType
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
		public decimal? MKeyword
		{
			get;
			set;
		}

		[DataMember]
		public string Number
		{
			get;
			set;
		}

		[DataMember]
		public int Status
		{
			get;
			set;
		}

		[DataMember]
		public int Year
		{
			get;
			set;
		}

		[DataMember]
		public int Period
		{
			get;
			set;
		}

		[DataMember]
		public List<string> ItemIDs
		{
			get;
			set;
		}

		[DataMember]
		public List<string> DepItemIDs
		{
			get;
			set;
		}

		[DataMember]
		public bool NotNeedPage
		{
			get;
			set;
		}

		[DataMember]
		public bool NeedCheckGroupModel
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCalculate
		{
			get;
			set;
		}

		[DataMember]
		public bool NeedAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsRedepreciate
		{
			get;
			set;
		}

		[DataMember]
		public List<FADepreciationModel> DepreciationModels
		{
			get;
			set;
		}

		[DataMember]
		public bool NeedMultiLang
		{
			get;
			set;
		}
	}
}
