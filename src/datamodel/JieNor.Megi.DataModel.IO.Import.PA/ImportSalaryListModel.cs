using JieNor.Megi.DataModel.PA;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.PA
{
	[DataContract]
	public class ImportSalaryListModel
	{
		[DataMember]
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public string MDate
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
		public decimal MBaseSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSocialSecurityBase
		{
			get;
			set;
		}

		[DataMember]
		public decimal MHosingProvidentFundBase
		{
			get;
			set;
		}

		[DataMember]
		public decimal MIncomeTaxThreshold
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAllowance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MCommission
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBonus
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOverTime
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAdjustment
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAttendance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOther
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRetirementSecurityAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMedicalInsuranceAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUmemploymentAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAdditionalAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSalaryBeforeTax
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNetSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRetirementSecurityAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMedicalInsuranceAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUmemploymentInsuranceAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMaternityInsuranceAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MIndustrialInjuryAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSeriousIiinessInjuryAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSocialSecurityOtherAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentFundAmountC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAddProvidentFundAmountC
		{
			get;
			set;
		}

		[DataMember]
		public List<PAPayItemGroupAmtModel> UserPayItemList
		{
			get;
			set;
		}
	}
}
