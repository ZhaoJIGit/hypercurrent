using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryToIVPayModel : BizDataModel
	{
		[DataMember]
		public int MEmployeeCount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalTaxSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalNetSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalVerificationAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalVerifyAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalVerifyAmt
		{
			get;
			set;
		}

		public PASalaryToIVPayModel()
			: base("T_PA_SalaryToIVPay")
		{
		}
	}
}
