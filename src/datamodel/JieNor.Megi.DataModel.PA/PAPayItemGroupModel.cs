using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayItemGroupModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

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
		public string MValue
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
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

		[DataMember]
		public string MItemTypeName
		{
			get;
			set;
		}

		[DataMember]
		public int MCoefficient
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanEdit
		{
			get;
			set;
		}

		public string MTemplateID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountId
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public PAPayItemGroupModel()
			: base("T_PA_PayItemGroup")
		{
		}
	}
}
