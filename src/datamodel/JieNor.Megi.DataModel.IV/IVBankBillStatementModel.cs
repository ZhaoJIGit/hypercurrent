using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillStatementModel : IVBankBillReconcileEntryModel
	{
		[DataMember]
		public DateTime MImportDate
		{
			get;
			set;
		}
	}
}
