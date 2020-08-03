using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBasicPrintSettingModel
	{
		[DataMember]
		public string MMeasureIn
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTopMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MTopMarginWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBottomMargin
		{
			get;
			set;
		}

		[DataMember]
		public string MBottomMarginWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAddressPadding
		{
			get;
			set;
		}

		[DataMember]
		public string MAddressPaddingWithUnit
		{
			get;
			set;
		}

		[DataMember]
		public string MLogoID
		{
			get;
			set;
		}

		[DataMember]
		public bool MShowLogo
		{
			get;
			set;
		}

		[DataMember]
		public string MLogoAlignment
		{
			get;
			set;
		}
	}
}
