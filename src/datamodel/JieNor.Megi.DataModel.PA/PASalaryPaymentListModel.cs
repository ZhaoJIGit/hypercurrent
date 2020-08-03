using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryPaymentListModel : PASalaryListBaseModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
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
		public int MStatus
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
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public int MRowNo
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
		public decimal MRetirementSecurityAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MMedicalInsuranceAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MUmemploymentAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MProvidentAdditionalAmount
		{
			get;
			set;
		}

		public DateTime MDate
		{
			get;
			set;
		}
	}
}
