using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.PA;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDPayrollDetailModel : BDModel
	{
		[DataMember]
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MJoinTime
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
		public List<PAPITThresholdModel> PITThresholdList
		{
			get;
			set;
		}

		[DataMember]
		public string MIDType
		{
			get;
			set;
		}

		[DataMember]
		public string MIDNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MSocialSecurityAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRetirementSecurityPercentage
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
		public decimal MMedicalInsurancePercentage
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
		public decimal MUmemploymentPercentage
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
		public string MProvidentAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentPercentage
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
		public decimal MProvidentAdditionalPercentage
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

		public BDPayrollDetailModel()
			: base("T_BD_EmpPayrollBasicSet")
		{
		}
	}
}
