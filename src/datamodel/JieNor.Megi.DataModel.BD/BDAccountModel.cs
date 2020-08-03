using JieNor.Megi.Core;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("AccountID", IsPKField = true)]
		public string MAccountID
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
		public string MApiCode
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
		[ApiMember("ParentAccountCode")]
		public string MParentNumber
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
		[ApiMember("FullName", ApiMemberType.MultiLang, false, false)]
		public string MFullName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Class")]
		[ApiEnum(EnumMappingType.AccountGroup)]
		public string MAccountGroupID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Type")]
		[ApiEnum(EnumMappingType.AccountType)]
		public string MAccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Direction")]
		[ApiEnum(EnumMappingType.AccountDirection)]
		public int MDC
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsForeignCurrencyAccounting")]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		[ApiDetail]
		[ApiMember("AccountingDimension")]
		public List<BDAccountDimensionModel> MAccountDimensions
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
		[ApiEnum(EnumMappingType.AccountingStandards)]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsBankAccount
		{
			get;
			set;
		}

		[DataMember]
		public int MRank
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalance
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCanRelateContact
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupID
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupModel MCheckGroupModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupNames
		{
			get;
			set;
		}

		[DataMember]
		public GLCurrencyDataModel MCurrencyDataModel
		{
			get;
			set;
		}

		[DataMember]
		public GLInitBalanceModel MInitBalanceModel
		{
			get;
			set;
		}

		[DataMember]
		public List<GLInitBalanceModel> MInitBalanceModels
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> MCheckTypeNameRelationList
		{
			get;
			set;
		}

		[DataMember]
		public string CurrentAcctNumber
		{
			get;
			set;
		}

		[DataMember]
		public bool MCreateInitBill
		{
			get;
			set;
		}

		[DataMember]
		public string MCreateInitBillName
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

		public string MOrderFiled
		{
			get;
			set;
		}

		public BDAccountModel()
			: base("T_BD_Account")
		{
		}

		public BDAccountModel(string tableName)
			: base(tableName)
		{
		}
	}
}
