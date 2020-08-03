using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBaseModel<TEntry> : BizDataModel where TEntry : IVEntryBaseModel
	{
		private decimal _exchangeRate = decimal.One;

		private decimal _currencyRate = decimal.One;

		private decimal _lToORate = decimal.One;

		private decimal _oToLRate = decimal.One;

		private List<TEntry> _entryList = null;

		private string _taxId = "Tax_Exclusive";

		private decimal _totalAmtFor = default(decimal);

		private decimal _totalAmt = default(decimal);

		private decimal _taxTotalAmtFor = default(decimal);

		private decimal _taxTotalAmt = default(decimal);

		private decimal _taxAmtFor = default(decimal);

		private decimal _taxAmt = default(decimal);

		private decimal _verifyAmt = default(decimal);

		private decimal _totalDiscountFor = default(decimal);

		private int _createFrom = Convert.ToInt32(CreateFromType.Normal);

		private bool _isAutoAmount = true;

		private bool _isCalculateMainAmount = true;

		private string _taxType = "";

		[DataMember]
		[InsertOnly]
		public int MCreateFrom
		{
			get
			{
				return _createFrom;
			}
			set
			{
				if (value == 0)
				{
					_createFrom = Convert.ToInt32(CreateFromType.Normal);
				}
				else
				{
					_createFrom = value;
				}
			}
		}

		[DataMember]
		public string MOrgCyID
		{
			get;
			set;
		}

		[DataMember(Order = 202)]
		[ApiMember("ContactID", IgnoreInGet = true)]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember(Order = 203)]
		[ApiMember("Date", IsLocalDate = true)]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsInitBill
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("NumberOfAttachment", IgnoreInPost = true)]
		public int MAttachCount
		{
			get;
			set;
		}

		[DataMember(Order = 210)]
		[ApiMember("CurrencyCode", IgnoreLengthValidate = true)]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MCyName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAutoAmount
		{
			get
			{
				return _isAutoAmount;
			}
			set
			{
				_isAutoAmount = value;
				if (MEntryList.Count > 0)
				{
					foreach (TEntry mEntry in MEntryList)
					{
						mEntry.MIsAutoAmount = value;
					}
				}
			}
		}

		public bool IsCalculateMainAmount
		{
			get
			{
				return _isCalculateMainAmount;
			}
			set
			{
				_isCalculateMainAmount = value;
			}
		}

		[DataMember(Order = 213)]
		[ApiMember("SubTotal", IgnoreInPost = true)]
		public decimal MTotalAmtFor
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MAmountFor));
				}
				return _totalAmtFor;
			}
			set
			{
				_totalAmtFor = value;
			}
		}

		[DataMember(Order = 214)]
		public decimal MTotalAmt
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MAmount));
				}
				return _totalAmt;
			}
			set
			{
				_totalAmt = value;
			}
		}

		[DataMember(Order = 215)]
		[ApiMember("Total", IgnoreInPost = true)]
		public decimal MTaxTotalAmtFor
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MTaxAmountFor));
				}
				return _taxTotalAmtFor;
			}
			set
			{
				_taxTotalAmtFor = value;
			}
		}

		[DataMember(Order = 216)]
		public decimal MTaxTotalAmt
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MTaxAmount));
				}
				return _taxTotalAmt;
			}
			set
			{
				_taxTotalAmt = value;
			}
		}

		[DataMember(Order = 217)]
		[ApiMember("TotalTax", IgnoreInPost = true)]
		public decimal MTaxAmtFor
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MTaxAmtFor));
				}
				return _taxAmtFor;
			}
			set
			{
				_taxAmtFor = value;
			}
		}

		[DataMember(Order = 218)]
		public decimal MTaxAmt
		{
			get
			{
				if (MEntryList.Count > 0)
				{
					return Enumerable.Sum<TEntry>((IEnumerable<TEntry>)MEntryList, (Func<TEntry, decimal>)((TEntry t) => t.MTaxAmt));
				}
				return _taxAmt;
			}
			set
			{
				_taxAmt = value;
			}
		}

		[DataMember(Order = 220)]
		public decimal MVerifyAmt
		{
			get
			{
				if (MEntryList.Count == 0 || !IsCalculateMainAmount)
				{
					return _verifyAmt;
				}
				decimal mTaxTotalAmt = MTaxTotalAmt;
				if (Math.Abs(MVerifyAmtFor) == Math.Abs(MTaxTotalAmtFor))
				{
					return mTaxTotalAmt;
				}
				decimal num = Math.Round(MVerifyAmtFor * MExchangeRate, 2);
				if (Math.Abs(num) > Math.Abs(mTaxTotalAmt))
				{
					return mTaxTotalAmt;
				}
				return num;
			}
			set
			{
				_verifyAmt = value;
			}
		}

		[DataMember(Order = 219)]
		public decimal MVerifyAmtFor
		{
			get;
			set;
		}

		public string MTaxType
		{
			get
			{
				return _taxType;
			}
		}

		[DataMember(Order = 221)]
		[ApiMember("LineAmountTypes")]
		[ApiEnum(EnumMappingType.TaxType)]
		public virtual string MTaxID
		{
			get
			{
				if (string.IsNullOrEmpty(_taxId))
				{
					_taxId = "Tax_Exclusive";
				}
				return _taxId;
			}
			set
			{
				_taxType = value;
				_taxId = value;
				if (value != "Tax_Exclusive" && value != "Tax_Inclusive" && value != "No_Tax")
				{
					_taxId = "Tax_Exclusive";
				}
				if (_entryList != null && _entryList.Count != 0)
				{
					foreach (TEntry entry in _entryList)
					{
						entry.MTaxTypeID = _taxId;
					}
				}
			}
		}

		[DataMember]
		public decimal MVerificationAmt
		{
			get;
			set;
		}

		[DataMember(Order = 211)]
		public decimal MExchangeRate
		{
			get
			{
				if (_exchangeRate <= decimal.Zero)
				{
					_exchangeRate = decimal.One;
				}
				return _exchangeRate;
			}
			set
			{
				if (value <= decimal.Zero)
				{
					value = decimal.One;
				}
				_exchangeRate = value;
				_oToLRate = value;
				_currencyRate = value;
				if (_entryList != null && _entryList.Count != 0)
				{
					foreach (TEntry entry in _entryList)
					{
						entry.MExchangeRate = _exchangeRate;
						entry.MOToLRate = _exchangeRate;
					}
				}
			}
		}

		[DataMember(Order = 211)]
		[ApiMember("CurrencyRate")]
		[ApiPrecision(6)]
		[DBField("MExchangeRate")]
		public decimal MCurrencyRate
		{
			get
			{
				return _currencyRate;
			}
			set
			{
				_currencyRate = value;
				if (value > decimal.Zero)
				{
					MExchangeRate = value;
				}
			}
		}

		[DataMember]
		public decimal MLToORate
		{
			get
			{
				if (_lToORate <= decimal.Zero)
				{
					_lToORate = decimal.One;
				}
				return _lToORate;
			}
			set
			{
				if (value <= decimal.Zero)
				{
					value = decimal.One;
				}
				_lToORate = value;
				if (_entryList != null && _entryList.Count != 0)
				{
					foreach (TEntry entry in _entryList)
					{
						entry.MLToORate = _lToORate;
					}
				}
			}
		}

		[DataMember]
		public decimal MOToLRate
		{
			get
			{
				if (_oToLRate <= decimal.Zero)
				{
					_oToLRate = decimal.One;
				}
				return _oToLRate;
			}
			set
			{
				if (value <= decimal.Zero)
				{
					value = decimal.One;
				}
				_oToLRate = value;
				_exchangeRate = value;
				_currencyRate = value;
				if (_entryList != null && _entryList.Count != 0)
				{
					foreach (TEntry entry in _entryList)
					{
						entry.MOToLRate = _oToLRate;
						entry.MExchangeRate = _oToLRate;
					}
				}
			}
		}

		public List<TEntry> MEntryList
		{
			get
			{
				if (_entryList == null)
				{
					_entryList = new List<TEntry>();
				}
				return _entryList;
			}
			set
			{
				if (value != null && value.Count != 0)
				{
					_entryList = value;
					foreach (TEntry entry in _entryList)
					{
						entry.MExchangeRate = _exchangeRate;
						entry.MLToORate = _lToORate;
						entry.MOToLRate = _oToLRate;
						entry.MTaxTypeID = _taxId;
					}
				}
			}
		}

		[DataMember]
		public List<IVVerificationListModel> Verification
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("TotalDiscount", IgnoreInPost = true)]
		public decimal MTotalDiscountAmtFor
		{
			get
			{
				if (MEntryList.Count == 0)
				{
					return _totalDiscountFor;
				}
				decimal num = default(decimal);
				foreach (TEntry mEntry in MEntryList)
				{
					num += mEntry.MAmountDiscountFor;
				}
				_totalDiscountFor = num;
				return num;
			}
			set
			{
				_totalDiscountFor = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[ApiMember("Url")]
		[AppSource(new string[]
		{
			"Api"
		})]
		public string MUrl
		{
			get;
			set;
		}

		[DataMember(Order = 3)]
		[ApiMember("Reference")]
		public string MReference
		{
			get;
			set;
		}

		public IVBaseModel(string tableName)
			: base(tableName)
		{
		}

		public void AddEntry(object obj)
		{
			TEntry item = obj as TEntry;
			if (obj != null)
			{
				MEntryList.Add(item);
			}
		}
	}
}
