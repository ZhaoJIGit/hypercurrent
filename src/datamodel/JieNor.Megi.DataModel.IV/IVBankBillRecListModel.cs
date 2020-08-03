using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillRecListModel : IVBankBillEntryModel
	{
		[DataMember]
		public List<IVReconcileTranstionListModel> MMatchList
		{
			get;
			set;
		}
	}
}
