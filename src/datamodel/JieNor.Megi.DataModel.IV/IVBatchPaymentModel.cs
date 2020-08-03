using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBatchPaymentModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public object MObject
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
		public string MContactName
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
		public DateTime MDueDate
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerifyAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MNoVerifyAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPayAmount
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
	}
}
