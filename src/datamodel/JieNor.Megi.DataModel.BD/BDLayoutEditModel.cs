using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDLayoutEditModel : BDModel
	{
		[DataMember]
		public string MPortal
		{
			get;
			set;
		}

		[DataMember]
		public string MSource
		{
			get;
			set;
		}

		[DataMember]
		public string MRefID
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
		{
			get;
			set;
		}

		public BDLayoutEditModel()
			: base("T_BD_Layout")
		{
		}
	}
}
