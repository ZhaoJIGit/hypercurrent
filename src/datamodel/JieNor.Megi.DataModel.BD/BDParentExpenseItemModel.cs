using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDParentExpenseItemModel
	{
		[DataMember]
		[ApiMember("ExpenseItemID", IsPKField = true)]
		public string MExpenseItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public dynamic MName
		{
			get;
			set;
		}
	}
}
