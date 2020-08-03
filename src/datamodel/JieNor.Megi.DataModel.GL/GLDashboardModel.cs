using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLDashboardModel
	{
		[DataMember]
		public int Year
		{
			get;
			set;
		}

		[DataMember]
		public int Period
		{
			get;
			set;
		}

		[DataMember]
		public bool PeriodBalanceInited
		{
			get;
			set;
		}

		[DataMember]
		public bool MonthProcessFinished
		{
			get;
			set;
		}

		[DataMember]
		public bool Settled
		{
			get;
			set;
		}

		[DataMember]
		public int VoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int ImportedVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int FromAppImportedVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int VoucherSavedCount
		{
			get;
			set;
		}

		[DataMember]
		public int VoucherApprovedCount
		{
			get;
			set;
		}

		[DataMember]
		public int CreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int UncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int SalesCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int PurchaseCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int ExpenseCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int ReceiveCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int PaymentCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int TransferCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int SalaryCreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int SalesUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int PurchaseUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int ExpenseUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int ReceiveUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int PaymentUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int TransferUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public int SalaryUncreatedDocVoucherCount
		{
			get;
			set;
		}

		[DataMember]
		public bool ReconcileFinished
		{
			get;
			set;
		}

		[DataMember]
		public string ClosingPeriod
		{
			get;
			set;
		}
	}
}
