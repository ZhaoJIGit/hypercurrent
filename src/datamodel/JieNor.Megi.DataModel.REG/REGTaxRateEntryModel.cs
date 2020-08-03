using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGTaxRateEntryModel : BDEntryModel
	{
		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCompound
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

		public REGTaxRateEntryModel()
			: base("T_REG_TaxRateEntry")
		{
		}
	}
}
