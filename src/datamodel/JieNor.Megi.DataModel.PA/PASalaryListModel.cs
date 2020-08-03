using JieNor.Megi.DataModel.IO.Export.DataRowModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryListModel
	{
		[DataMember]
		public string Title
		{
			get;
			set;
		}

		[DataMember]
		public string Period
		{
			get;
			set;
		}

		[DataMember]
		public string EmployeeNameTitle1
		{
			get;
			set;
		}

		[DataMember]
		public string EmployeeNameTitle2
		{
			get;
			set;
		}

		[DataMember]
		public string JoinTimeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string IDNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BasicSalaryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SalaryBeforePITTitle
		{
			get;
			set;
		}

		[DataMember]
		public string NetSalaryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PITTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TotalSalaryTitle
		{
			get;
			set;
		}

		[DataMember]
		public string AdditionalInfoTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PersonTitle
		{
			get;
			set;
		}

		[DataMember]
		public string EmployerTitle
		{
			get;
			set;
		}

		[DataMember]
		public string EmployeeName1
		{
			get;
			set;
		}

		[DataMember]
		public string EmployeeName2
		{
			get;
			set;
		}

		[DataMember]
		public DateTime JoinTime
		{
			get;
			set;
		}

		[DataMember]
		public string IDNumber
		{
			get;
			set;
		}

		[DataMember]
		public decimal BaseSalaryAmount
		{
			get;
			set;
		}

		[DataMember]
		public string PITDesc
		{
			get;
			set;
		}

		[DataMember]
		public decimal PITAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal SalaryBeforePITAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal NetSalaryAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal TotalSalaryAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployeeSocialSecurityAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal EmployeeHousingProvidentFundAmount
		{
			get;
			set;
		}

		[DataMember]
		public List<SalaryItemModel> SalaryItemModels
		{
			get;
			set;
		}

		[DataMember]
		public List<SSAndHFModel> SSAndHFModels
		{
			get;
			set;
		}

		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeID
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
	}
}
