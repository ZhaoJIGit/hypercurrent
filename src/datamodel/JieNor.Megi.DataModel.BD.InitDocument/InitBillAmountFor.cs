using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD.InitDocument
{
	[DataContract]
	public class InitBillAmountFor
	{
		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MInitBalanceFor
		{
			get;
			set;
		}

		[DataMember]
		public bool MHasYtdData
		{
			get;
			set;
		}
	}
}
