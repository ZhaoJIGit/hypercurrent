using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDExpenseItemModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("ExpenseItemID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MExpenseItemID
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
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MParentItemID
		{
			get;
			set;
		}

		public string MParentItemName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsIncludeAccount
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
		[ApiMember("ParentExpenseItem")]
		public BDParentExpenseItemModel MParentExpenseItemModel
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ExpenseAccountCode")]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MAccountCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Description", ApiMemberType.MultiLang, false, false)]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MexpenseAccountID
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		[ApiDetail]
		public List<BDExpenseItemEntryModel> EntryList
		{
			get;
			set;
		}

		[DataMember]
		public string MText
		{
			get
			{
				return MName;
			}
			set
			{
			}
		}

		[DataMember]
		public string MAccountId
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

		[DataMember]
		public List<BDExpenseItemModel> Chileren
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNumber
		{
			get;
			set;
		}

		public BDExpenseItemModel()
			: base("T_BD_ExpenseItem")
		{
		}
	}
}
