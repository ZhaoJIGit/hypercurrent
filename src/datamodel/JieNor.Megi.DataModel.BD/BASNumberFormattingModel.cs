using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BASNumberFormattingModel : BDModel
	{
		[DataMember]
		public string MLCID
		{
			get;
			set;
		}

		[DataMember]
		public double MPositive
		{
			get;
			set;
		}

		[DataMember]
		public double MNegative
		{
			get;
			set;
		}

		[DataMember]
		public string MDecimalSeparator
		{
			get;
			set;
		}

		[DataMember]
		public int MDigitsAfterDecimalSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string MDigitGrouping
		{
			get;
			set;
		}

		[DataMember]
		public string MThousandsSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string MNegativeFormatSymbol
		{
			get;
			set;
		}

		[DataMember]
		public string MNumberListSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string MLeadingZeros
		{
			get;
			set;
		}

		[DataMember]
		public string MStandardDigits
		{
			get;
			set;
		}

		[DataMember]
		public string MSystemOfMeasurements
		{
			get;
			set;
		}

		[DataMember]
		public bool MAvailable
		{
			get;
			set;
		}

		[DataMember]
		public string MCreateUserID
		{
			get;
			set;
		}

		[DataMember]
		public string MUpdateLastUserID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MUpdateLastDate
		{
			get;
			set;
		}

		public BASNumberFormattingModel()
			: base("t_bas_NumberFormatting")
		{
		}
	}
}
