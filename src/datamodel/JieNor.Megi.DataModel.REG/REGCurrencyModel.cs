using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGCurrencyModel : BDModel
	{
		[ApiMember("Code", IsPKField = true)]
		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", MemberType = ApiMemberType.MultiLang)]
		public string MName
		{
			get;
			set;
		}

		public REGCurrencyModel()
			: base("T_REG_Currency")
		{
		}
	}
}
