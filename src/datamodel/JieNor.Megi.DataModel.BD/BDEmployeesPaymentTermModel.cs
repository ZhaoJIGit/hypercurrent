using JieNor.Megi.Core.Attribute;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDEmployeesPaymentTermModel
	{
		[DataMember]
		[ApiMember("Expense")]
		public BDContactsBillsPaymentTermModel MExpense
		{
			get;
			set;
		}

		public List<string> UpdateFieldList
		{
			get;
			set;
		}
	}
}
