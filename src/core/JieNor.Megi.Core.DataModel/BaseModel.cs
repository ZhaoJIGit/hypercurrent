using JieNor.Megi.Core.Attribute;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BaseModel
	{
		private List<MultiLanguageFieldList> _multiLanguage;

		private List<IBizVerificationRule> _bizVerificationRules;

		public virtual string[] MMultiLangEncryptColumns
		{
			get
			{
				return null;
			}
		}

		[InsertOnly]
		[DataMember(Order = 1000, EmitDefaultValue = true)]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[ApiMember("UpdatedDateUTC", IgnoreInPost = true)]
		[DataMember(Order = 1001, EmitDefaultValue = true)]
		public DateTime MModifyDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? UpdatedDateUTC
		{
			get;
			set;
		}

		[DataMember]
		[InsertOnly]
		public string MCreatorID
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

		[ApiMember("CreateBy", IgnoreInGet = true)]
		[DataMember]
		[AppSource(new string[]
		{
			"Api"
		})]
		[InsertOnly]
		public string MCreateBy
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOnlyReturnEntry
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDelete
		{
			get;
			set;
		}

		[ApiMember("ReturnData")]
		[DataMember(Order = 1002, EmitDefaultValue = true)]
		public string ReturnData
		{
			get;
			set;
		}

		[ApiMember("ValidationErrors", ApiMemberType.ObjectList, false, false, IgnoreInGet = true)]
		[DataMember(Order = 1003, EmitDefaultValue = true)]
		public List<ValidationError> ValidationErrors
		{
			get;
			set;
		}

		[DataMember]
		public List<MultiLanguageFieldList> MultiLanguage
		{
			get
			{
				return _multiLanguage ?? new List<MultiLanguageFieldList>();
			}
			set
			{
				_multiLanguage = value;
				if (MMultiLangEncryptColumns != null && MMultiLangEncryptColumns.Length != 0 && value != null && value.Count != 0)
				{
					string[] mMultiLangEncryptColumns = MMultiLangEncryptColumns;
					foreach (string text in mMultiLangEncryptColumns)
					{
						foreach (MultiLanguageFieldList item in _multiLanguage)
						{
							if (item.MFieldName.ToLower() == text.ToLower())
							{
								item.IsEncrypt = true;
								break;
							}
						}
					}
				}
			}
		}

		public string TableName
		{
			get;
			set;
		}

		public virtual string PKFieldName
		{
			get
			{
				return "";
			}
		}

		public virtual string FKFieldName
		{
			get
			{
				return "";
			}
		}

		public virtual string PKFieldValue
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		[DataMember]
		public bool IsUpdate
		{
			get;
			set;
		}

		[DataMember]
		public bool IsNew
		{
			get;
			set;
		}

		[DataMember]
		public bool IsDelete
		{
			get;
			set;
		}

		public List<IBizVerificationRule> BizVerificationRules
		{
			get
			{
				return _bizVerificationRules ?? new List<IBizVerificationRule>();
			}
			set
			{
				_bizVerificationRules = value;
			}
		}

		public int UniqueIndex
		{
			get;
			set;
		}

		public List<string> UpdateFieldList
		{
			get;
			set;
		}

		public bool ShowPartFields
		{
			get;
			set;
		} = true;


		public BaseModel(string tableName)
		{
			TableName = tableName;
			MIsDelete = false;
			ValidationErrors = new List<ValidationError>();
		}

		public void AddMultiLanguage(object obj)
		{
			MultiLanguageFieldList multiLanguageFieldList = obj as MultiLanguageFieldList;
			if (multiLanguageFieldList != null)
			{
				if (_multiLanguage == null)
				{
					_multiLanguage = new List<MultiLanguageFieldList>();
				}
				_multiLanguage.Add(multiLanguageFieldList);
			}
		}

		public void AddLang(string fieldName, string localeId, string value)
		{
			if (_multiLanguage == null)
			{
				_multiLanguage = new List<MultiLanguageFieldList>();
			}
			MultiLanguageFieldList multiLanguageFieldList = _multiLanguage.FirstOrDefault((MultiLanguageFieldList t) => t.MFieldName == fieldName);
			if (multiLanguageFieldList == null)
			{
				multiLanguageFieldList = new MultiLanguageFieldList();
				_multiLanguage.Add(multiLanguageFieldList);
			}
			if (multiLanguageFieldList.MMultiLanguageField == null)
			{
				multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			}
			MultiLanguageField multiLanguageField = multiLanguageFieldList.MMultiLanguageField.FirstOrDefault((MultiLanguageField t) => t.MLocaleID == localeId);
			if (multiLanguageField == null)
			{
				multiLanguageField = new MultiLanguageField
				{
					MLocaleID = localeId,
					MValue = value
				};
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			else
			{
				multiLanguageField.MValue = value;
			}
		}

		public string GetMultiLanguageValue(MContext ctx, string fieldName)
		{
			if (MultiLanguage == null || MultiLanguage.Count == 0)
			{
				return string.Empty;
			}
			MultiLanguageFieldList multiLanguageFieldList = (from t in MultiLanguage
			where t.MFieldName == fieldName
			select t).FirstOrDefault();
			if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return string.Empty;
			}
			MultiLanguageField multiLanguageField = (from t in multiLanguageFieldList.MMultiLanguageField
			where t.MLocaleID == ctx.MLCID
			select t).FirstOrDefault();
			if (multiLanguageField != null)
			{
				return multiLanguageField.MValue;
			}
			multiLanguageField = (from t in multiLanguageFieldList.MMultiLanguageField
			where t.MLocaleID == "0x0009"
			select t).FirstOrDefault();
			if (multiLanguageField != null)
			{
				return multiLanguageField.MValue;
			}
			return multiLanguageFieldList.MMultiLanguageField[0].MValue;
		}

		public string GetMultiLanguageValue(string localeId, string fieldName)
		{
			if (MultiLanguage == null || MultiLanguage.Count == 0)
			{
				return string.Empty;
			}
			MultiLanguageFieldList multiLanguageFieldList = (from t in MultiLanguage
			where t.MFieldName == fieldName
			select t).FirstOrDefault();
			if (multiLanguageFieldList == null || multiLanguageFieldList.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return string.Empty;
			}
			MultiLanguageField multiLanguageField = (from t in multiLanguageFieldList.MMultiLanguageField
			where t.MLocaleID == localeId
			select t).FirstOrDefault();
			if (multiLanguageField != null)
			{
				return multiLanguageField.MValue;
			}
			return string.Empty;
		}

		public bool IsUpdateFieldExists(string fieldName)
		{
			if (UpdateFieldList == null || UpdateFieldList.Count == 0)
			{
				return false;
			}
			if (UpdateFieldList.Contains(fieldName))
			{
				return true;
			}
			return false;
		}

		public bool IsOnlySetDisbled()
		{
			if (UpdateFieldList == null || UpdateFieldList.Count != 3)
			{
				return false;
			}
			if (UpdateFieldList.Contains("MIsActive"))
			{
				return true;
			}
			return false;
		}
	}
}
