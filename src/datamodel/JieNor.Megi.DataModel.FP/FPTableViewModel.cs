using JieNor.Megi.DataModel.IV;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPTableViewModel : FPTableModel
	{
		[DataMember]
		public int MFapiaoType
		{
			get;
			set;
		}
		[DataMember]
		public string MBankId
		{
			get;
			set;
		}
		[DataMember]
		public decimal IssuedAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MIssuedAmount
		{
			get
			{
				return (fapiaoList == null) ? decimal.Zero : fapiaoList.Sum((FPFapiaoModel x) => x.MTotalAmount);
			}
			set
			{
			}
		}

		[DataMember]
		public int MInvoiceCount
		{
			get
			{
				return (invoiceList != null) ? invoiceList.Count : 0;
			}
			set
			{
			}
		}

		[DataMember]
		public int MFapiaoCount
		{
			get
			{
				return (fapiaoList != null) ? fapiaoList.Count : 0;
			}
			set
			{
			}
		}

		[DataMember]
		public List<IVInvoiceModel> invoiceList
		{
			get;
			set;
		}

		[DataMember]
		public List<FPFapiaoModel> fapiaoList
		{
			get;
			set;
		}

		[DataMember]
		public List<string> invoiceIdList
		{
			get;
			set;
		}

		[DataMember]
		public List<string> fapiaoIdList
		{
			get;
			set;
		}
	}
}
