using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVReceiveEntryModel : IVEntryBaseModel
	{
		public IVReceiveEntryModel()
			: base("T_IV_ReceiveEntry")
		{
		}
	}
}
