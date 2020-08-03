using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.Const;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsPaymentTermModel
	{
		[DataMember]
		[ApiMember("Bills", ErrorType = ValidationErrorType.ContactBillsPaymentTerms)]
		public BDContactsBillsPaymentTermModel MBills
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Sales", ErrorType = ValidationErrorType.ContactSalesPaymentTerms)]
		public BDContactsSalesPaymentTermModel MSales
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
