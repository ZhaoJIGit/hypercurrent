using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FA
{
	[DataContract]
	public class FADepreciationTypeModel : BDModel
	{
		[DataMember]
		public string MYearDepreciationAlgorithm
		{
			get;
			set;
		}

		[DataMember]
		public string MPeriodDepreciationAlgorithm
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MDesc", ApiMemberType.MultiLang, false, false)]
		public string MDesc
		{
			get;
			set;
		}

		public FADepreciationTypeModel()
			: base("T_FA_DepreciationType")
		{
		}

		public FADepreciationTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
