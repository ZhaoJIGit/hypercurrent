using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Import.PA
{
	[DataContract]
	public class UserPayItemColumnRangeModel
	{
		[DataMember]
		public PayrollItemEnum Type
		{
			get;
			set;
		}

		[DataMember]
		public int StartColumnIndex
		{
			get;
			set;
		}

		[DataMember]
		public int EndColumnIndex
		{
			get;
			set;
		}
	}
}
