using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV.Invoice
{
	[DataContract]
	public class Sph
	{
		[DataMember]
		public int Xh
		{
			get;
			set;
		}

		[DataMember]
		public string Spmc
		{
			get;
			set;
		}

		[DataMember]
		public string Ggxh
		{
			get;
			set;
		}

		[DataMember]
		public string Jldw
		{
			get;
			set;
		}

		[DataMember]
		public decimal Dj
		{
			get;
			set;
		}

		[DataMember]
		public decimal Sl
		{
			get;
			set;
		}

		[DataMember]
		public decimal Je
		{
			get;
			set;
		}

		[DataMember]
		public decimal Slv
		{
			get;
			set;
		}
	}
}
