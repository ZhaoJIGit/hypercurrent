using JieNor.Megi.Core.Attribute;
using JieNor.Megi.DataModel.REG;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDItemSalesDetailModel
	{
		[DataMember]
		[ApiMember("UnitPrice")]
		[ApiPrecision(8)]
		[DBField("MSalPrice")]
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
