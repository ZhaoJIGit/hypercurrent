using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVStatementsModel
	{
		[DataMember]
		public string MItemID
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
		public string MEmail
		{
			get;
			set;
		}

		[DataMember]
		public string MAddress
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MOverdue
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get;
			set;
		}
	}
}
