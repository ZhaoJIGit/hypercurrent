using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.MultiLanguage
{
	[DataContract]
	public class MultiLanguageField
	{
		private string mValue;

		[DataMember]
		public string MPKID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string MValue
		{
			get
			{
				return string.IsNullOrWhiteSpace(mValue) ? "" : mValue.Trim();
			}
			set
			{
				mValue = value;
			}
		}

		[DataMember]
		public bool MIsDelete
		{
			get;
			set;
		}

		[DataMember]
		public string MModifierID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MModifyDate
		{
			get;
			set;
		}

		[DataMember]
		public string MCreatorID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}
	}
}
