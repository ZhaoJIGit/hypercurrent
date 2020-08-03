using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryPaymentTreeModel
	{
		[DataMember]
		public string MPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MPayItemName
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
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MQZDAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSalTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public int MCoefficient
		{
			get;
			set;
		}
	}
}
