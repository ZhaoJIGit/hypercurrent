using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RPT
{
	[DataContract]
	public class RPTBalanceSheetModel
	{
		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountName
		{
			get;
			set;
		}

		[DataMember]
		public string MCode
		{
			get;
			set;
		}

		[DataMember]
		public decimal MClosingBalAmt
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYTDAmt
		{
			get;
			set;
		}
	}
}
