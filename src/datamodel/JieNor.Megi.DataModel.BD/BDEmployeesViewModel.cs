using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	public class BDEmployeesViewModel
	{
		[DataMember]
		public string MDeptIDS
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
		public string MUserID
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
		public string MPhone
		{
			get;
			set;
		}

		[DataMember]
		public string MFax
		{
			get;
			set;
		}

		[DataMember]
		public string MMobile
		{
			get;
			set;
		}

		[DataMember]
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
		public string MSkypeName
		{
			get;
			set;
		}

		[DataMember]
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
		public string MDefaultSaleTaxID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultPurchaseTaxID
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
		public string MBorrowAcctID
		{
			get;
			set;
		}

		[DataMember]
		public string MDefaultCurrencyID
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
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MSex
		{
			get;
			set;
		}

		[DataMember]
		public string MPAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MPStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MPRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MRealAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MRealStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MRealRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAccName
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		public string MStatus
		{
			get;
			set;
		}
	}
}
