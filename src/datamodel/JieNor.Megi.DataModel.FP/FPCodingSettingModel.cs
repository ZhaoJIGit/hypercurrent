using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPCodingSettingModel : BizDataModel
	{
		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public int MFapiaoNumber
		{
			get;
			set;
		}

		[DataMember]
		public int MBizDate
		{
			get;
			set;
		}

		[DataMember]
		public int MPSContactName
		{
			get;
			set;
		}

		[DataMember]
		public int MInventoryName
		{
			get;
			set;
		}

		[DataMember]
		public int MAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MTaxRate
		{
			get;
			set;
		}

		[DataMember]
		public int MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MFastCode
		{
			get;
			set;
		}

		[DataMember]
		public int MContactID
		{
			get;
			set;
		}

		[DataMember]
		public int MMerItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MExplanation
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem1
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem2
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem3
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem4
		{
			get;
			set;
		}

		[DataMember]
		public int MTrackItem5
		{
			get;
			set;
		}

		[DataMember]
		public int MDebitAccount
		{
			get;
			set;
		}

		[DataMember]
		public int MCreditAccount
		{
			get;
			set;
		}

		[DataMember]
		public int MTaxAccount
		{
			get;
			set;
		}

		public FPCodingSettingModel()
			: base("t_fp_codingsetting")
		{
		}

		public FPCodingSettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
