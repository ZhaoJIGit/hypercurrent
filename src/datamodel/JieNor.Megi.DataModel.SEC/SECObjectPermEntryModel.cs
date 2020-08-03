using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECObjectPermEntryModel : BDEntryModel
	{
		[DataMember]
		public string MPerMItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MShowIndex
		{
			get;
			set;
		}

		public SECObjectPermEntryModel()
			: base("T_Sec_ObjectPermEntry")
		{
		}
	}
}
