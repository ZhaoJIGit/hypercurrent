using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVMakePaymentModel
	{
		[DataMember]
		public string MObjectID
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
		public decimal MPaidAmount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MPaidDate
		{
			get;
			set;
		}

		[DataMember]
		public string MRef
		{
			get;
			set;
		}

		[DataMember]
		public bool MRefFromBill
		{
			get;
			set;
		}

		[DataMember]
		public string MObjectType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsPayAll
		{
			get;
			set;
		}

		public object MObject
		{
			get;
			set;
		}

		public decimal MLToORate
		{
			get;
			set;
		}

		public decimal MOToLRate
		{
			get;
			set;
		}

		public decimal MPaidAmtFor
		{
			get;
			set;
		}

		public decimal MPaidAmt
		{
			get;
			set;
		}

		[DataMember]
		public bool IsPayRun
		{
			get;
			set;
		}

		[DataMember]
		public string PayRunID
		{
			get;
			set;
		}

		[DataMember]
		public string SalaryPaymentIDLists
		{
			get;
			set;
		}
	}
}
