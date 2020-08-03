using JieNor.Megi.DataModel.GL;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDAccountListModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public string MItemID
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
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
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
		public string MAccountGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctGroupName
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctTypeName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCheckForCurrency
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public int MDC
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalance
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCanRelateContact
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebitFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCredit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCreditFor
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MCheckGroupName
		{
			get;
			set;
		}

		[DataMember]
		public List<string> MCheckGroupNameList
		{
			get;
			set;
		}

		[DataMember]
		public GLCheckGroupModel MCheckGroupModel
		{
			get;
			set;
		}

		[DataMember]
		public bool MCreateInitBill
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x0009
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x7804
		{
			get;
			set;
		}

		[DataMember]
		public string MName0x7C04
		{
			get;
			set;
		}
	}
}
