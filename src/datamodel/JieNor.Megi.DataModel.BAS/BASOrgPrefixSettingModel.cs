using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgPrefixSettingModel : BDModel
	{
		[DataMember]
		public string MPrefixModule
		{
			get;
			set;
		}

		[DataMember]
		public string MPrefixName
		{
			get;
			set;
		}

		[DataMember]
		public int MNumberCount
		{
			get;
			set;
		}

		[DataMember]
		public string MSplitString
		{
			get;
			set;
		}

		[DataMember]
		public int MStartIndex
		{
			get;
			set;
		}

		[DataMember]
		public string MFillBlankChar
		{
			get;
			set;
		}

		[DataMember]
		public int MValue
		{
			get;
			set;
		}

		[DataMember]
		public string MPrefix
		{
			get
			{
				return MPrefixName + MSplitString;
			}
			set
			{
			}
		}

		[DataMember]
		public string GetNumString
		{
			get
			{
				string text = MValue.ToString();
				return MPrefixName + text.PadLeft(MNumberCount, '0');
			}
			set
			{
			}
		}

		[DataMember]
		public int MConversionYear
		{
			get;
			set;
		}

		[DataMember]
		public int MConversionMonth
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MFABeginDate
		{
			get;
			set;
		}

		[DataMember]
		public int ActionType
		{
			get;
			set;
		}

		public BASOrgPrefixSettingModel()
			: base("T_BAS_OrgPrefixSetting")
		{
		}
	}
}
