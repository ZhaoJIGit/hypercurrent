using JieNor.Megi.Common.Converter;
using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVEntryBaseModel : BizEntryDataModel
	{
		private decimal _taxAmtFor = default(decimal);

		private decimal _amountFor;

		private decimal amount;

		[DataMember]
		[ApiMember("LineItemID", IsPKField = true)]
		public string MLineItemID
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
		[ApiMember("ItemID", IgnoreInGet = true)]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ItemCode")]
		public string MItemCode
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Description")]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctID
		{
			get;
			set;
		}

		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MLToORate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOToLRate
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxTypeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Quantity")]
		[ApiPrecision(4)]
		public decimal MQty
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("UnitPrice")]
		[ApiPrecision(8)]
		public decimal MPrice
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("DiscountRate")]
		[ApiPrecision(6)]
		public decimal MDiscount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("LineAmount")]
		[ApiPrecision(2)]
		public decimal MAmountFor
		{
			get
			{
				decimal d = MQty * MPrice;
				return (d * (decimal.One - MDiscount / 100m)).ToRound(2);
			}
			set
			{
				_amountFor = value;
			}
		}

		[DataMember]
		public decimal MAmountDiscountFor
		{
			get
			{
				return (MQty * MPrice).ToRound(2) - MAmountFor;
			}
			set
			{
			}
		}

		public decimal MLineAmountFor
		{
			get
			{
				return _amountFor;
			}
		}

		[DataMember]
		public decimal MAmount
		{
			get
			{
				if (!MIsAutoAmount)
				{
					return amount;
				}
				return (MAmountFor * MExchangeRate).ToRound(2) + MAjustAmount;
			}
			set
			{
				if (!MIsAutoAmount)
				{
					amount = value;
				}
			}
		}

		public decimal MAjustAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmountFor
		{
			get
			{
				if (MTaxTypeID == "Tax_Exclusive")
				{
					return MAmountFor + MTaxAmtFor;
				}
				if (MTaxTypeID == "Tax_Inclusive")
				{
					return MAmountFor;
				}
				if (MTaxTypeID == "No_Tax")
				{
					return MAmountFor;
				}
				throw new Exception("含税类型错误");
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MTaxAmount
		{
			get
			{
				if (MTaxTypeID == "Tax_Exclusive")
				{
					return MAmount + MTaxAmt;
				}
				if (MTaxTypeID == "Tax_Inclusive")
				{
					return MAmount;
				}
				if (MTaxTypeID == "No_Tax")
				{
					return MAmount;
				}
				throw new Exception("含税类型错误");
			}
			set
			{
			}
		}

		[DataMember]
		[ApiMember("TaxAmount")]
		[ApiPrecision(2)]
		public decimal MTaxAmtFor
		{
			get
			{
				if (MTaxTypeID == "No_Tax")
				{
					return decimal.Zero;
				}
				return _taxAmtFor;
			}
			set
			{
				_taxAmtFor = value.ToRound(2);
			}
		}

		[DataMember]
		public decimal MTaxAmt
		{
			get
			{
				return (MTaxAmtFor * MExchangeRate).ToRound(2);
			}
			set
			{
			}
		}

		[DataMember]
		[ApiMember("MTaxID", IgnoreInGet = true)]
		public virtual string MTaxID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TaxRate", true, MemberType = ApiMemberType.Object)]
		[BaseData("MItemID", new string[]
		{
			"MName"
		})]
		public REGTaxRateModel MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem1", IgnoreInGet = true)]
		public string MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem2", IgnoreInGet = true)]
		public string MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem3", IgnoreInGet = true)]
		public string MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem4", IgnoreInGet = true)]
		public string MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TrackItem5", IgnoreInGet = true)]
		public string MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAutoAmount
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Tracking", true)]
		public List<BDTrackSelectModel> MTracking
		{
			get;
			set;
		}

		public IVEntryBaseModel(string tableName)
			: base(tableName)
		{
			MExchangeRate = decimal.One;
			MTaxTypeID = "Tax_Exclusive";
			MIsAutoAmount = true;
			MTracking = new List<BDTrackSelectModel>();
		}
	}
}
