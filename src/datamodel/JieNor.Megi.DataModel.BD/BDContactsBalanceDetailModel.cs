using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsBalanceDetailModel
	{
		[DataMember]
		[ApiMember("Outstanding")]
		public decimal MOutstanding
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Overdue")]
		public decimal MOverdue
		{
			get;
			set;
		}
	}
}
