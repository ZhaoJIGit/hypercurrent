using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankAccountModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		[ApiMember("BankID", IsPKField = true)]
		public string MBankID
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
		[ApiMember("BankAccountType")]
		[ApiEnum(EnumMappingType.BankAccountType)]
		public int MBankAccountType
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get
			{
				return MName;
			}
			set
			{
				MName = value;
			}
		}

		[ApiMember("BankAccountName", ApiMemberType.MultiLang, false, false)]
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[ApiMember("BankAccountNumber")]
		public string MBankNo
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("CurrencyCode")]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("BankType")]
		[BaseData("MItemID", new string[]
		{

		})]
		public BDBankTypeModel MBankType
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsShowInHome")]
		public bool MIsShowInHome
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsNeedReconcile")]
		public bool MIsNeedReconcile
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsNeedImportBankStatement")]
		public bool MIsNeedImportBankStatement
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("AccountCode")]
		public string MAccountNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		[DataMember]
		public string _bankTypeName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankTypeName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_bankTypeName) && MBankType != null)
				{
					return MBankType.MName;
				}
				return _bankTypeName;
			}
			set
			{
				_bankTypeName = value;
			}
		}

		[DataMember]
		public string MCurrencyName
		{
			get;
			set;
		}

		[DataMember]
		public string MPKID
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

		public BDBankAccountModel()
			: base("T_BD_BankAccount")
		{
		}
	}
}
