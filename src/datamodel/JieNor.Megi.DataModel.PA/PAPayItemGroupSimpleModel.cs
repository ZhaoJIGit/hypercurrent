using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayItemGroupSimpleModel
	{
		[DataMember]
		[ApiMember("SalaryItemID", IsPKField = true)]
		public string MSalaryItemID
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
		public int MItemType
		{
			get;
			set;
		}
	}
}
