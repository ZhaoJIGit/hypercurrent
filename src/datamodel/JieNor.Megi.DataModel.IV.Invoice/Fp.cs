using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Invoice
{
	[DataContract]
	public class Fp
	{
		[DataMember]
		public string Djh
		{
			get;
			set;
		}

		[DataMember]
		public string Gfmc
		{
			get;
			set;
		}

		[DataMember]
		public string Gfsh
		{
			get;
			set;
		}

		[DataMember]
		public string Gfyhzh
		{
			get;
			set;
		}

		[DataMember]
		public string Gfdzdh
		{
			get;
			set;
		}

		[DataMember]
		public string Bz
		{
			get;
			set;
		}

		[DataMember]
		public string Fhr
		{
			get;
			set;
		}

		[DataMember]
		public string Skr
		{
			get;
			set;
		}

		[DataMember]
		public List<Sph> Spxx
		{
			get;
			set;
		}
	}
}
