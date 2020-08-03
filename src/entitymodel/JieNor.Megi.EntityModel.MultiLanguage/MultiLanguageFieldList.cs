using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.MultiLanguage
{
	[DataContract]
	public class MultiLanguageFieldList
	{
		[DataMember]
		public string MFieldName
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
		{
			get;
			set;
		}

		[DataMember]
		public List<MultiLanguageField> MMultiLanguageField
		{
			get;
			set;
		}

		[DataMember]
		public string MMultiLanguageValue
		{
			get;
			set;
		}

		public bool IsEncrypt
		{
			get;
			set;
		}

		public MultiLanguageFieldList()
		{
			MMultiLanguageField = new List<MultiLanguageField>();
		}

		public void CreateMultiLanguageFieldValue(string localeID, string value, string pkID)
		{
			MMultiLanguageField.Add(CreateFieldValue(localeID, value, pkID));
		}

		private MultiLanguageField CreateFieldValue(string localeID, string value, string pkID = "")
		{
			MultiLanguageField multiLanguageField = new MultiLanguageField();
			multiLanguageField.MLocaleID = localeID;
			multiLanguageField.MValue = value;
			multiLanguageField.MPKID = pkID;
			return multiLanguageField;
		}
	}
}
