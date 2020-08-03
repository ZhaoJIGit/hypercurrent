using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.PA
{
	[DataContract]
	public class ImportPaySettingModel
	{
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
		public decimal MEmpProvidentFundPer
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEmpAddProvidentFundPer
		{
			get;
			set;
		}
	}
}
