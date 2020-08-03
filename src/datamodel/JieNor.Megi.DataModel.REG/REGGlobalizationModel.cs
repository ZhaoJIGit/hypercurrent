using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.REG
{
	[DataContract]
	public class REGGlobalizationModel : BDModel
	{
		[DataMember]
		public string MSystemLanguage
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemZone
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDate
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemTime
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDigitDot
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDigitGroupingSymbol
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDigitGroupingFormat
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDigitNegative
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemDigitNegativeNumFormat
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsShowCSymbol
		{
			get;
			set;
		}

		[DataMember]
		public string MUpdateFields
		{
			get;
			set;
		}

		public REGGlobalizationModel()
			: base("T_REG_Globalization")
		{
		}
	}
}
