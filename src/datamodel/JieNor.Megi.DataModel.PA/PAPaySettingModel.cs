using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	public class PAPaySettingModel : BDModel
	{
		[DataMember]
		public string MSocialSecurityAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MRetirementSecurityPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEmpRetirementSecurityPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMedicalInsurancePer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEmpMedicalInsurancePer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUmemploymentInsurancePer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEmpUmemploymentInsurancePer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMaternityInsurancePer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MIndustrialInjuryPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSeriousIiinessInjuryPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOtherPer
		{
			get;
			set;
		}

		[DataMember]
		public string MProvidentFundAccount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentFundPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAddProvidentFundPer
		{
			get;
			set;
		}

		[DataMember]
		public string MStartPayMonth
		{
			get;
			set;
		}

		[DataMember]
		public string MStartPayDay
		{
			get;
			set;
		}

		[DataMember]
		public string MEndPayMonth
		{
			get;
			set;
		}

		[DataMember]
		public string MEndPayDay
		{
			get;
			set;
		}

		[DataMember]
		public string MPaymentMonth
		{
			get;
			set;
		}

		[DataMember]
		public string MPaymentDay
		{
			get;
			set;
		}

		[DataMember]
		public string MPayItemIDs
		{
			get;
			set;
		}

		public PAPaySettingModel()
			: base("T_PA_PaySetting")
		{
		}
	}
}
