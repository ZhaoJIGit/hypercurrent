using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.SEC
{
	[DataContract]
	public class SECRelyPartyModel : BDModel
	{
		[DataMember]
		public string MClientID
		{
			get;
			set;
		}

		[DataMember]
		public string MClientName
		{
			get;
			set;
		}

		[DataMember]
		public string MClientSecret
		{
			get;
			set;
		}

		[DataMember]
		public string MClientScope
		{
			get;
			set;
		}

		public SECRelyPartyModel()
			: base("T_SEC_RelyParty")
		{
		}
	}
}
