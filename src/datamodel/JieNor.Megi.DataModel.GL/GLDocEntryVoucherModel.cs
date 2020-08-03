using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLDocEntryVoucherModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		private string _itemName
		{
			get;
			set;
		}

		[DataMember]
		public string MItemName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_itemName) && _itemName.Trim() == ":")
				{
					return "";
				}
				return _itemName;
			}
			set
			{
				_itemName = value;
			}
		}

		[DataMember]
		public string MReference
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
		public DateTime MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MModifyDate
		{
			get;
			set;
		}

		[DataMember]
		public string MFirstName
		{
			get;
			set;
		}

		[DataMember]
		public string MLastName
		{
			get;
			set;
		}

		[DataMember]
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string MEmployeeName
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
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MDocID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTax
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTaxAmt
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
		public int MDocType
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
		public int MergeStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MDocVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherNumber
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public int MSettleStatus
		{
			get;
			set;
		}

		[DataMember]
		public int MVoucherStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccountFullName
		{
			get;
			set;
		}

		[DataMember]
		public bool Create
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmt
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
		public string MFromAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MToAccountID
		{
			get;
			set;
		}

		[DataMember]
		public string MFromAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MToAccountName
		{
			get;
			set;
		}

		[DataMember]
		public int MFromAccountType
		{
			get;
			set;
		}

		[DataMember]
		public int MToAccountType
		{
			get;
			set;
		}

		[DataMember]
		public int MDocStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool CanCreateVoucher
		{
			get
			{
				return MDocStatus != 1 && MDocStatus != 2 && MVoucherStatus != 1 && string.IsNullOrWhiteSpace(MVoucherNumber);
			}
			private set
			{
			}
		}
	}
}
