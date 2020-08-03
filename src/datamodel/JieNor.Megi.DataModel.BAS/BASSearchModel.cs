using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASSearchModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
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
		public string ContactID
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
		public string MAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public string MBizDate
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
		public string MDesc
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
		public string MBankID
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
		public string MValue1
		{
			get;
			set;
		}

		[DataMember]
		public string MValue2
		{
			get;
			set;
		}

		[DataMember]
		public string MValue3
		{
			get;
			set;
		}
	}
}
