using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVRPBaseModel<TEntry> : IVBaseModel<TEntry> where TEntry : IVEntryBaseModel
	{
		[DataMember(Order = 201)]
		public string MType
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
		public int MSource
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
		public decimal MReconcileAmt
		{
			get
			{
				return Math.Round(MReconcileAmtFor * base.MExchangeRate, 2);
			}
			set
			{
			}
		}

		[DataMember]
		public decimal MReconcileAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public int MReconcileStatu
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

		public IVRPBaseModel(string tableName)
			: base(tableName)
		{
		}
	}
}
