using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IV;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryPaymentModel : BizDataModel
	{
		private string _dateFormat = string.Empty;

		[DataMember]
		public string MRunID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public string MDateFormat
		{
			get
			{
				DateTime mDate = MDate;
				object arg = mDate.Year;
				mDate = MDate;
				return string.Format("{0}-{1}", arg, mDate.Month);
			}
			set
			{
				_dateFormat = value;
			}
		}

		[DataMember]
		public string MEmployeeID
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
		public decimal MTaxSalary
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPITThresholdAmount
		{
			get;
			set;
		}

		[DataMember]
		public List<PAPITTaxRateModel> MPITTaxRateList
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNetSalary
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
		public decimal MVerificationAmt
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

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsSent
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
		[ModelEntry]
		[ApiDetail]
		public List<PASalaryPaymentEntryModel> SalaryPaymentEntry
		{
			get;
			set;
		}

		[DataMember]
		public List<IVVerificationListModel> Verification
		{
			get;
			set;
		}

		[DataMember]
		public PASalaryPaymentPermissionModel MActionPermission
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
		public string MDesc
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
		public bool IsCalculatePIT
		{
			get;
			set;
		}

		public PASalaryPaymentModel()
			: base("T_PA_SalaryPayment")
		{
			SalaryPaymentEntry = new List<PASalaryPaymentEntryModel>();
		}
	}
}
