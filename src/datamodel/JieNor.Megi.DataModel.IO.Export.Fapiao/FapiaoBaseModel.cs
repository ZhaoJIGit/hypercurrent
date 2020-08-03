using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Fapiao
{
	public class FapiaoBaseModel
	{
		[DataMember]
		public string BillNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string FapiaoNumberTitle
		{
			get;
			set;
		}

		[DataMember]
		public string FapiaoCodeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string BizDateTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemVersionTitle
		{
			get;
			set;
		}

		[DataMember]
		public string RemarkTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemNameTitle
		{
			get;
			set;
		}

		[DataMember]
		public string ItemTypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string UnitTitle
		{
			get;
			set;
		}

		[DataMember]
		public string QuantityTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PriceTitle
		{
			get;
			set;
		}

		[DataMember]
		public string AmountTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxPercentTitle
		{
			get;
			set;
		}

		[DataMember]
		public string TaxAmountTitle
		{
			get;
			set;
		}
	}
}
