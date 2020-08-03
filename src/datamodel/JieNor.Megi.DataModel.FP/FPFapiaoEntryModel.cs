using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPFapiaoEntryModel : BizEntryDataModel
	{
		[DataMember]
		[ApiMember("LineItemID")]
		public string MFapiaoLineID
		{
			get
			{
				return base.MEntryID;
			}
			set
			{
				base.MEntryID = value;
			}
		}

		[DataMember]
		[ApiMember("ProductName")]
		public string MItemName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ItemCategoryName")]
		public string MItemCategoryCode
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ItemSpecification")]
		public string MItemType
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Unit")]
		public string MUnit
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Quantity")]
		[ApiPrecision(10)]
		public decimal MQuantity
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("UnitPrice")]
		[ApiPrecision(10)]
		public decimal MPrice
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LineAmount")]
		[ApiPrecision(10)]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxRate")]
		public decimal MTaxPercent
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxAmount")]
		[ApiPrecision(10)]
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmount
		{
			get;
			set;
		}

		public FPFapiaoEntryModel()
			: base("T_FP_FapiaoEntry")
		{
		}

		public FPFapiaoEntryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
