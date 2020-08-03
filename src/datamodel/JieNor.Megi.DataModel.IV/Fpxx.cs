using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class Fpxx
	{
		[DataMember]
		public int Zsl
		{
			get;
			set;
		}

		[DataMember]
		public List<Fp> Fpsj
		{
			get;
			set;
		}
	}
}
