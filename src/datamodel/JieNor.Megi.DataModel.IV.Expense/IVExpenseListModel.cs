using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Expense
{
	[DataContract]
	public class IVExpenseListModel
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
		public string MType
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
		public DateTime MDueDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpectedDate
		{
			get;
			set;
		}

		[DataMember]
		public string MBranding
		{
			get;
			set;
		}

		[DataMember]
		public int MAttachCount
		{
			get;
			set;
		}

		[DataMember]
		public string MAttachIDs
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
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgCyID
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
		public decimal MTotalAmtFor
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
		public decimal MTaxTotalAmtFor
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
		public decimal MVerificationAmt
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
		public string MStatus
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
		public string MContactName
		{
			get;
			set;
		}

		[DataMember]
		public string DepartmentName
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
		public decimal MVerifyAmt
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
	}
}
