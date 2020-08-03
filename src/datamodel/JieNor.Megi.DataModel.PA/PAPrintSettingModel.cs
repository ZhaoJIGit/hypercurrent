using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPrintSettingModel : BDModel
	{
		[DataMember]
		public string MMeasureIn
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTopMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MTopMarginWithUnit
		{
			get
			{
				return string.Concat(MTopMargin, ' ', MMeasureIn);
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MBottomMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MBottomMarginWithUnit
		{
			get
			{
				return string.Concat(MBottomMargin, ' ', MMeasureIn);
			}
			set
			{
			}
		}

		[DataMember]
		public string MLogoID
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowLogo
		{
			get;
			set;
		}

		[DataMember]
		public string MLogoAlignment
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowTitle
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowSalaryMonth
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowEmployeeName
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowCnEnName
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowJoinTime
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowIDNumber
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowEmployeeSocialSecurity
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowEmployeeHousingProvidentFund
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowSalaryBeforeTax
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowPIT
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowNetSalary
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowAdditionalInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowPension
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowMedicalInsurance
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowUmemploymentInsurance
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowMeternityInsurance
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowIndustrialInjury
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowSeriousMedical
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowSocialSecurityOther
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowHousingProvidentFund
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowHousingProvidentFundAdition
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowTotalSalary
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		[ModelEntry]
		public List<PAPrintSettingEntryModel> MEntryList
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
		public string MTitle
		{
			get;
			set;
		}

		[DataMember]
		public string MAdditionalInfo
		{
			get;
			set;
		}

		public PAPrintSettingModel()
			: base("T_PA_PrintSetting")
		{
		}

		public PAPrintSettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
