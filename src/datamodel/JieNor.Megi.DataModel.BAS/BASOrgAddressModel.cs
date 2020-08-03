using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgAddressModel : BDModel
	{
		[DataMember]
		public string MAddressType
		{
			get;
			set;
		}

		[DataMember]
		public string MStreet
		{
			get;
			set;
		}

		[DataMember]
		public string MTownCity
		{
			get;
			set;
		}

		[DataMember]
		public string MStateRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MCountry
		{
			get;
			set;
		}

		[DataMember]
		public string MAttention
		{
			get;
			set;
		}

		[DataMember]
		public string MZipcode
		{
			get;
			set;
		}

		public BASOrgAddressModel()
			: base("T_Bas_OrgAddress")
		{
		}
	}
}
