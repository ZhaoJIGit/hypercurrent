using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RI
{
	[DataContract]
	public class RICategorySettingParamModel : BDModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

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
		public string MParamType
		{
			get;
			set;
		}

		[DataMember]
		public string MParamName
		{
			get;
			set;
		}

		[DataMember]
		public string MParamValue
		{
			get;
			set;
		}

		[DataMember]
		public string MCompareType
		{
			get;
			set;
		}

		[DataMember]
		public int MOperator
		{
			get;
			set;
		}

		[DataMember]
		public string MCompareValue
		{
			get;
			set;
		}

		[DataMember]
		public string MFuncName
		{
			get;
			set;
		}

		[DataMember]
		public string MToken
		{
			get;
			set;
		}

		public RICategorySettingParamModel()
			: base("T_RI_CategorySettingParam")
		{
		}

		public RICategorySettingParamModel(string tableName)
			: base(tableName)
		{
		}
	}
}
