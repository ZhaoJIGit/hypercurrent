using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDItemModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("ItemID", IsPKField = true, IgnoreLengthValidate = true, RecordFilterField = "Code")]
		public string MProductItemID
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
		[ApiMember("Code")]
		[DBField("MNumber")]
		public string MProductNumber
		{
			get
			{
				return base.MNumber;
			}
			set
			{
				base.MNumber = value;
			}
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsService")]
		public bool MIsExpenseItem
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("PurchaseDetails")]
		public BDItemPurchaseDetailModel MPurchaseDetails
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("SalesDetails")]
		public BDItemSalesDetailModel MSalesDetails
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPurPrice
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSalPrice
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MText
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(base.MNumber)) ? (base.MNumber + ":" + MDesc) : MDesc;
			}
			set
			{
			}
		}

		[DataMember]
		[ApiMember("InventoryAccountCode", IgnoreLengthValidate = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MInventoryAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MInventoryAccountId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IncomeAccountCode", IgnoreLengthValidate = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MIncomeAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MIncomeAccountId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CostAccountCode", IgnoreLengthValidate = true, IsManyToOneDbField = true)]
		[DBFieldValidate(IgnoreLengthValidate = true)]
		public string MCostAccountCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ExpenseAccountCode", IgnoreLengthValidate = true, IsManyToOneDbField = true)]
		[DBField("MCostAccountCode")]
		public string MExpenseAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MCostAccountId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status", IgnoreInPost = true)]
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
		public bool MIsNew
		{
			get;
			set;
		}

		public BDItemModel()
			: base("T_BD_Item")
		{
		}
	}
}
