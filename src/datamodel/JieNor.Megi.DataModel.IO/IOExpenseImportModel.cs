using System;

namespace JieNor.Megi.DataModel.IO
{
	public class IOExpenseImportModel : IOImportBaseModel
	{
		public int MRowIndex
		{
			get;
			set;
		}

		public string MEmployee
		{
			get;
			set;
		}

		public string MReference
		{
			get;
			set;
		}

		public DateTime MBizDate
		{
			get;
			set;
		}

		public DateTime MDueDate
		{
			get;
			set;
		}

		public DateTime MExpectedDate
		{
			get;
			set;
		}

		public string MCyID
		{
			get;
			set;
		}

		public string MItemID
		{
			get;
			set;
		}

		public string MDesc
		{
			get;
			set;
		}

		public decimal MQty
		{
			get;
			set;
		}

		public decimal MPrice
		{
			get;
			set;
		}

		public decimal MTaxAmtFor
		{
			get;
			set;
		}

		public string MDebitAccount
		{
			get;
			set;
		}

		public string MCreditAccount
		{
			get;
			set;
		}

		public string MTaxAccount
		{
			get;
			set;
		}

		public DateTime MPaymentDate
		{
			get;
			set;
		}

		public decimal MPaidAmount
		{
			get;
			set;
		}

		public string MBankAccount
		{
			get;
			set;
		}

		public string MTrackItem1
		{
			get;
			set;
		}

		public string MTrackItem2
		{
			get;
			set;
		}

		public string MTrackItem3
		{
			get;
			set;
		}

		public string MTrackItem4
		{
			get;
			set;
		}

		public string MTrackItem5
		{
			get;
			set;
		}
	}
}
