using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVReconcileTranstionListModel
	{
		private decimal _mSpentAmtFor = default(decimal);

		private decimal _mReceiveAmtFor = default(decimal);

		[DataMember]
		public string MBillID
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
		public string MDescription
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
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSpentAmtFor
		{
			get
			{
				return Math.Abs(_mSpentAmtFor);
			}
			set
			{
				_mSpentAmtFor = value;
			}
		}

		[DataMember]
		public decimal MReceiveAmtFor
		{
			get
			{
				return Math.Abs(_mReceiveAmtFor);
			}
			set
			{
				_mReceiveAmtFor = value;
			}
		}

		[DataMember]
		public decimal MSplitSpentAmtFor
		{
			get
			{
				return Math.Abs(_mSpentAmtFor);
			}
			set
			{
				_mSpentAmtFor = value;
			}
		}

		[DataMember]
		public decimal MSplitReceiveAmtFor
		{
			get
			{
				return Math.Abs(_mReceiveAmtFor);
			}
			set
			{
				_mReceiveAmtFor = value;
			}
		}

		[DataMember]
		public string MTargetBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MBankAccountName
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
		public string MContactID
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
		public int MSequence
		{
			get;
			set;
		}
	}
}
