using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayItemModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("SalaryItemID", IsPKField = true)]
		public string MSalaryItemID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		public bool MIsSys
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
		[ApiMember("ParentSalaryItem")]
		public PAPayItemGroupSimpleModel MParentSalaryItemModel
		{
			get;
			set;
		}

		[DataMember]
		public string MGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MGroupName
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
		[ApiMember("AccountCode")]
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
		[ApiMember("Status")]
		[ApiEnum(EnumMappingType.CommonStatus)]
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

		public PAPayItemModel()
			: base("T_PA_PayItem")
		{
		}
	}
}
