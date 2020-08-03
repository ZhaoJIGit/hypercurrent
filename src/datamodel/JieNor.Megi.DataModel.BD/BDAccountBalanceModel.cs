using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	public class BDAccountBalanceModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSystemBalance
		{
			get;
			set;
		}

		[DataMember]
		public decimal MStateBalance
		{
			get;
			set;
		}
	}
}
