using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECPermisionGroupModel : BDModel
	{
		[DataMember]
		public string MAppID
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

		public SECPermisionGroupModel()
			: base("T_Sec_ObjectGroup")
		{
		}
	}
}
