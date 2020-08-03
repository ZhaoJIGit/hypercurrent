using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGTaxRateModel : BDModel
	{
		public int __active__ = 1;

		[DataMember]
		[ApiMember("TaxRateID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MTaxRateID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		public string MAppID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false, IgnoreLengthValidate = true)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("EffectiveRate")]
		[ApiPrecision(2)]
		public decimal MEffectiveTaxRate
		{
			get;
			set;
		}

		[ModelEntry]
		[ApiDetail]
		[DataMember]
		public List<REGTaxRateEntryModel> TaxRateDetail
		{
			get;
			set;
		}

		[DataMember]
		public string MText
		{
			get
			{
				return MName + "(" + MTaxRate.ToString("#0.00") + "%)";
			}
			set
			{
			}
		}

		[DataMember]
		public string MTaxText
		{
			get
			{
				return MTaxRate.ToString("#0") + "%";
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MTaxRateDecimal
		{
			get
			{
				return MTaxRate / 100m;
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MEffectiveTaxRateDecimal
		{
			get
			{
				return MEffectiveTaxRate / 100m;
			}
			set
			{
			}
		}

		[DataMember]
		[ApiMember("SalTaxAccountCode", IgnoreInSubModel = true)]
		public string MSaleTaxAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MSaleTaxAccountId
		{
			get;
			set;
		}

		[DataMember(EmitDefaultValue = true)]
		[ApiMember("PurTaxAccountCode", IgnoreInSubModel = true)]
		public string MPurchaseAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MPurchaseAccountId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("VATPaidAccountCode", IgnoreInSubModel = true)]
		public string MPayDebitAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MPayDebitAccountId
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsSystem", IgnoreInSubModel = true)]
		public bool MIsSysData
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Status", IgnoreInSubModel = true)]
		[ApiEnum(EnumMappingType.CommonStatus)]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public REGTaxRateModel()
			: base("T_REG_TaxRate")
		{
		}
	}
}
