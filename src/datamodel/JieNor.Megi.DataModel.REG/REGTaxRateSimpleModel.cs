using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGTaxRateSimpleModel : BDModel
	{
		[DataMember]
		[ApiMember("TaxRateID", IsPKField = true)]
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
		[ApiMember("Name", ApiMemberType.MultiLang, false, false, IgnoreLengthValidate = true)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("EffectiveRate")]
		public decimal MEffectiveTaxRate
		{
			get;
			set;
		}

		public REGTaxRateSimpleModel()
			: base("T_REG_TaxRate")
		{
		}
	}
}
