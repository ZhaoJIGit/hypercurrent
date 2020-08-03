using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountDimensionModel
	{
		[DataMember]
		[ApiMember("Type")]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Value")]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("InputType")]
		[ApiEnum(EnumMappingType.AccountInputType)]
		public string MInputType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status")]
		[ApiEnum(EnumMappingType.CommonStatus)]
		public bool MStatus
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Seq", IgnoreInGet = true)]
		public int MSeq
		{
			get;
			set;
		}
	}
}
