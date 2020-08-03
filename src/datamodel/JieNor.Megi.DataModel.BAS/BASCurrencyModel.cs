using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASCurrencyModel : BDModel
	{
		[DataMember]
		[ApiMember("Name", MemberType = ApiMemberType.MultiLang)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public int MNoCode
		{
			get;
			set;
		}

		[DataMember]
		public int MDecimalDigits
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		public BASCurrencyModel()
			: base("T_Bas_Currency")
		{
		}
	}
}
