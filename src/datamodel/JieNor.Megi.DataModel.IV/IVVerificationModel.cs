using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVVerificationModel : BizDataModel
	{
		[DataMember]
		public string MSourceBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceBillEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MSourceBillID
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillID
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillEntryID
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
		public int MDirection
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
		public decimal MAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmtFor
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

		public IVVerificationModel()
			: base("T_IV_Verification")
		{
		}
	}
}
