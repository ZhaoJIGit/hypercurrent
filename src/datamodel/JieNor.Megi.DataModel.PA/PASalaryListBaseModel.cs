using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryListBaseModel : BaseModel
	{
		[DataMember]
		public decimal BaseSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal PIT
		{
			get;
			set;
		}

		[DataMember]
		public decimal SalaryBeforePIT
		{
			get;
			set;
		}

		[DataMember]
		public decimal SalaryAfterPIT
		{
			get;
			set;
		}

		[DataMember]
		public decimal TotalSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployeeSocialSecurity
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployeeHousingProvidentFund
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployerSocialSecurity
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployerHousingProvidentFund
		{
			get;
			set;
		}

		[DataMember]
		public decimal SSWithHFEmployee
		{
			get;
			set;
		}

		[DataMember]
		public decimal SSWithHFEmployer
		{
			get;
			set;
		}

		[DataMember]
		public decimal SSWithHFTotal
		{
			get;
			set;
		}

		[DataMember]
		public List<PAPayItemModel> DisableItemList
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

		[DataMember]
		public List<NameValueModel> MultiLangGroupNameList
		{
			get;
			set;
		}

		[DataMember]
		public decimal Allowance
		{
			get;
			set;
		}

		[DataMember]
		public decimal Bonus
		{
			get;
			set;
		}

		[DataMember]
		public decimal Commission
		{
			get;
			set;
		}

		[DataMember]
		public decimal OverTime
		{
			get;
			set;
		}

		[DataMember]
		public decimal UserAddItem
		{
			get;
			set;
		}

		[DataMember]
		public decimal TaxAdjustment
		{
			get;
			set;
		}

		[DataMember]
		public decimal IncreaseItem
		{
			get;
			set;
		}

		[DataMember]
		public decimal Attendance
		{
			get;
			set;
		}

		[DataMember]
		public decimal UserSubtractItem
		{
			get;
			set;
		}

		[DataMember]
		public decimal Other
		{
			get;
			set;
		}

		public PASalaryListBaseModel()
			: base("T_PA_PayRun")
		{
		}
	}
}
