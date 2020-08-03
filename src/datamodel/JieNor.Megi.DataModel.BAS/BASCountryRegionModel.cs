using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASCountryRegionModel : BDModel
	{
		[DataMember]
		public string MStandardName
		{
			get;
			set;
		}

		[DataMember]
		public string MAlpha2Code
		{
			get;
			set;
		}

		[DataMember]
		public int MNumericCode
		{
			get;
			set;
		}

		[DataMember]
		public string MISO31662Codes
		{
			get;
			set;
		}

		public BASCountryRegionModel()
			: base("T_Bas_CountryRegion")
		{
		}
	}
}
