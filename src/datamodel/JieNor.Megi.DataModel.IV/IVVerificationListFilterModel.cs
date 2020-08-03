using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVVerificationListFilterModel
	{
		[DataMember]
		public string MBillID
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
		public string MContactType
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public string MBizBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MBizType
		{
			get;
			set;
		}

		[DataMember]
		public IVVerificationInforViewScopeEnum MViewDataType
		{
			get;
			set;
		}

		[DataMember]
		public bool MViewVerif
		{
			get;
			set;
		}

		[DataMember]
		public bool MViewCanVerif
		{
			get;
			set;
		}

		[DataMember]
		public string MKeyword
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
	}
}
