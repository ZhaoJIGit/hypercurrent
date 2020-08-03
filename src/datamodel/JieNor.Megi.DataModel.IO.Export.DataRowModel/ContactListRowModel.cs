using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.DataRowModel
{
	[DataContract]
	public class ContactListRowModel
	{
		[DisplayName("Contact Name")]
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DisplayName("Contact Type")]
		[DataMember]
		public string ContactType
		{
			get;
			set;
		}

		[DisplayName("First Name")]
		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DisplayName("Last Name")]
		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DisplayName("Email")]
		[DataMember]
		public string MEmail
		{
			get;
			set;
		}

		[DisplayName("PO Attention")]
		[DataMember]
		public string MPAttention
		{
			get;
			set;
		}

		[DisplayName("PO Street Address or PO Box")]
		[DataMember]
		public string MPStreet
		{
			get;
			set;
		}

		[DisplayName("PO Town / City")]
		[DataMember]
		public string MPCityID
		{
			get;
			set;
		}

		[DisplayName("PO State / Region")]
		[DataMember]
		public string MPRegion
		{
			get;
			set;
		}

		[DisplayName("PO Postal / Zip Code")]
		[DataMember]
		public string MPPostalNo
		{
			get;
			set;
		}

		[DisplayName("PO Country")]
		[DataMember]
		public string MPCountryID
		{
			get;
			set;
		}

		[DisplayName("Physical Attention")]
		[DataMember]
		public string MRealAttention
		{
			get;
			set;
		}

		[DisplayName("Physical Street Address or PO Box")]
		[DataMember]
		public string MRealStreet
		{
			get;
			set;
		}

		[DisplayName("Physical Town / City")]
		[DataMember]
		public string MRealCityID
		{
			get;
			set;
		}

		[DisplayName("Physical State / Region")]
		[DataMember]
		public string MRealRegion
		{
			get;
			set;
		}

		[DisplayName("Physical Postal / Zip Code")]
		[DataMember]
		public string MRealPostalNo
		{
			get;
			set;
		}

		[DisplayName("Physical Country")]
		[DataMember]
		public string MRealCountryID
		{
			get;
			set;
		}

		[DisplayName("Phone Number")]
		[DataMember]
		public string MPhone
		{
			get;
			set;
		}

		[DisplayName("Fax Number")]
		[DataMember]
		public string MFax
		{
			get;
			set;
		}

		[DisplayName("Mobile Number")]
		[DataMember]
		public string MMobile
		{
			get;
			set;
		}

		[DisplayName("Direct Dial Number")]
		[DataMember]
		public string MDirectPhone
		{
			get;
			set;
		}

		[DisplayName("Skype Name/Number")]
		[DataMember]
		public string MSkypeName
		{
			get;
			set;
		}

		[DisplayName("Website")]
		[DataMember]
		public string MWebsite
		{
			get;
			set;
		}

		[DisplayName("For Sales Tracking Name1")]
		[DataMember]
		public string MSalTrackEntry1
		{
			get;
			set;
		}

		[DisplayName("For Sales Tracking Name2")]
		[DataMember]
		public string MSalTrackEntry2
		{
			get;
			set;
		}

		[DisplayName("For Sales Tracking Name3")]
		[DataMember]
		public string MSalTrackEntry3
		{
			get;
			set;
		}

		[DisplayName("For Sales Tracking Name4")]
		[DataMember]
		public string MSalTrackEntry4
		{
			get;
			set;
		}

		[DisplayName("For Sales Tracking Name5")]
		[DataMember]
		public string MSalTrackEntry5
		{
			get;
			set;
		}

		[DisplayName("For Purchases Tracking Name1")]
		[DataMember]
		public string MPurTrackEntry1
		{
			get;
			set;
		}

		[DisplayName("For Purchases Tracking Name2")]
		[DataMember]
		public string MPurTrackEntry2
		{
			get;
			set;
		}

		[DisplayName("For Purchases Tracking Name3")]
		[DataMember]
		public string MPurTrackEntry3
		{
			get;
			set;
		}

		[DisplayName("For Purchases Tracking Name4")]
		[DataMember]
		public string MPurTrackEntry4
		{
			get;
			set;
		}

		[DisplayName("For Purchases Tracking Name5")]
		[DataMember]
		public string MPurTrackEntry5
		{
			get;
			set;
		}

		[DisplayName("Tax ID Number")]
		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DisplayName("Sales Tax")]
		[DataMember]
		public string MSalTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("Purchases Tax")]
		[DataMember]
		public string MPurTaxTypeID
		{
			get;
			set;
		}

		[DisplayName("Discount %")]
		[DataMember]
		public string MDiscount
		{
			get;
			set;
		}

		[DisplayName("Default Currency")]
		[DataMember]
		public string MDefaultCyID
		{
			get;
			set;
		}

		[DisplayName("Bank Account Number")]
		[DataMember]
		public string MBankAcctNo
		{
			get;
			set;
		}

		[DisplayName("Account Name")]
		[DataMember]
		public string MBankAccName
		{
			get;
			set;
		}

		[DisplayName("Details")]
		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DisplayName("Due Date Bill Day")]
		[DataMember]
		public string MPurDueDate
		{
			get;
			set;
		}

		[DisplayName("Due Date Bill Term")]
		[DataMember]
		public string MPurDueCondition
		{
			get;
			set;
		}

		[DisplayName("Due Date Sales Day")]
		[DataMember]
		public string MSalDueDate
		{
			get;
			set;
		}

		[DisplayName("Due Date Sales Term")]
		[DataMember]
		public string MSalDueCondition
		{
			get;
			set;
		}

		[DisplayName("CustomerCurrentAccount")]
		[DataMember]
		public string MCCurrentAccountCode
		{
			get;
			set;
		}

		[DisplayName("SupplierCurrentAccount")]
		[DataMember]
		public string MSCurrentAccountCode
		{
			get;
			set;
		}

		[DisplayName("SaleIncomeAccount")]
		[DataMember]
		public string MSaleIncomeAccountCode
		{
			get;
			set;
		}

		[DisplayName("Sale Due Amount")]
		[DataMember]
		public decimal? MSaleDueAmt
		{
			get;
			set;
		}

		[DisplayName("Sale Over Due Amount")]
		[DataMember]
		public decimal? MSaleOverDueAmt
		{
			get;
			set;
		}

		[DisplayName("Bill Due Amount")]
		[DataMember]
		public decimal? MBillDueAmt
		{
			get;
			set;
		}

		[DisplayName("Bill Over Due Amount")]
		[DataMember]
		public decimal? MBillOverDueAmt
		{
			get;
			set;
		}
	}
}
