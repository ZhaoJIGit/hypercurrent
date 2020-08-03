using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASNationalLanguageModel : BDModel
	{
		[DataMember]
		public string MCultureName
		{
			get;
			set;
		}

		[DataMember]
		public string MUsedCountryRegion
		{
			get;
			set;
		}

		[DataMember]
		public string MStandardName
		{
			get;
			set;
		}

		[DataMember]
		public string MLocalName
		{
			get;
			set;
		}

		[DataMember]
		public double MANSICode
		{
			get;
			set;
		}

		[DataMember]
		public double MOEMCode
		{
			get;
			set;
		}

		[DataMember]
		public string MCountryRegionID
		{
			get;
			set;
		}

		[DataMember]
		public string MLanguageID
		{
			get;
			set;
		}

		public BASNationalLanguageModel()
			: base("T_Bas_NationalLanguage")
		{
		}
	}
}
