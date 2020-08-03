using JieNor.Megi.DataModel.GL;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.InitDocument
{
	[DataContract]
	public class BDInitBillViewModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MReference
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxTotalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxTotalAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public bool MSingleRow
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MVerifyAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MExchangeRate
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDueDate
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
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MBizBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MContactType
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		public GLCheckGroupModel MCheckGroupModel
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupValueModel MCheckGroupValueModel
		{
			get;
			set;
		}

		[DataMember]
		public List<BDInitBillEntryViewModel> MInitBillEntryList
		{
			get;
			set;
		}
	}
}
