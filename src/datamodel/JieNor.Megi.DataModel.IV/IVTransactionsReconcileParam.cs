using JieNor.Megi.Core.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVTransactionsReconcileParam : ParamBase
	{
		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public IVReconcileStatus MStatu
		{
			get;
			set;
		}
	}
}
