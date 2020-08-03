using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.REG;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDItemPurchaseDetailModel
	{
		[DataMember]
		[ApiMember("UnitPrice")]
		[ApiPrecision(8)]
		[DBField("MPurPrice")]
		public decimal MUnitPrice
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxRate")]
		public REGTaxRateSimpleModel MTaxRate
		{
			get;
			set;
		}

		public List<string> UpdateFieldList
		{
			get;
			set;
		}
	}
}
