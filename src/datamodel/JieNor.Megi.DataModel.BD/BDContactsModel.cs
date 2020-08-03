using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		public override string[] MMultiLangEncryptColumns
		{
			get
			{
				return new string[1]
				{
					"MName"
				};
			}
		}

		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MPCountryID
		{
			get;
			set;
		}

		[DataMember]
		public string MPCityID
		{
			get;
			set;
		}

		[DataMember]
		public string MPPostalNo
		{
			get;
			set;
		}

		[DataMember]
		public string MRealCountryID
		{
			get;
			set;
		}

		[DataMember]
		public string MRealCityID
		{
			get;
			set;
		}

		[DataMember]
		public string MRealPostalNo
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MPhone
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MFax
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MMobile
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MDirectPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MQQNo
		{
			get;
			set;
		}

		[DataMember]
		public string MWeChatNo
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MSkypeName
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		[Http("Website", OperateTime.Save)]
		public string MWebsite
		{
			get;
			set;
		}

		[DataMember]
		[Email("Email Address", OperateTime.Save)]
		public string MOurEmail
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MBankAcctNo
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBaseSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDiscount
		{
			get;
			set;
		}

		[DataMember]
		public int MPurDueDate
		{
			get;
			set;
		}

		[DataMember]
		public int MSalDueDate
		{
			get;
			set;
		}

		[DataMember]
		public string MPurDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public string MSalDueCondition
		{
			get;
			set;
		}

		[DataMember]
		public string MRecAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MPayAcctID
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MBorrowAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MCCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCustomer
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSupplier
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOther
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
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

		[DataMember]
		public bool MIsNew
		{
			get;
			set;
		}

		[DataMember]
		public bool UpdatedTrackLink
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem1Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem2Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem3Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem4Name
		{
			get;
			set;
		}

		[DataMember]
		public string MTrackItem5Name
		{
			get;
			set;
		}

		public BDContactsModel()
			: base("T_BD_Contacts")
		{
		}
	}
}
