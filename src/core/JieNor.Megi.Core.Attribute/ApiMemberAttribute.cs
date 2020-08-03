using JieNor.Megi.Core.Const;
using JieNor.Megi.Core.DataModel;
using System;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ApiMemberAttribute : System.Attribute
	{
		public string Name
		{
			get;
			set;
		}

		private string _listAddItemMethod
		{
			get;
			set;
		}

		public string ListAddItemMethod
		{
			get
			{
				return string.IsNullOrWhiteSpace(_listAddItemMethod) ? "AddEntry" : _listAddItemMethod;
			}
			set
			{
				_listAddItemMethod = value;
			}
		}

		public ApiMemberType MemberType
		{
			get;
			set;
		}

		public bool IsEncrypt
		{
			get;
			set;
		}

		public int MaxLength
		{
			get;
			set;
		}

		public bool IsPKField
		{
			get;
			set;
		}

		public bool IsReference
		{
			get;
			set;
		}

		public bool IsLocalDate
		{
			get;
			set;
		}

		public bool IgnoreLengthValidate
		{
			get;
			set;
		}

		public bool IgnoreInPost
		{
			get;
			set;
		}

		public bool IgnoreInSmart
		{
			get;
			set;
		}

		public bool IgnoreInGet
		{
			get;
			set;
		}

		public ValidationErrorType ErrorType
		{
			get;
			set;
		}

		public bool IsObjectCanUpdate
		{
			get;
			set;
		}

		public bool IgnoreInSubModel
		{
			get;
			set;
		}

		public string RecordFilterField
		{
			get;
			set;
		}

		public bool IsDynamicShow
		{
			get;
			set;
		}

		public bool IsManyToOneDbField
		{
			get;
			set;
		}

		public ApiMemberAttribute(string name)
		{
			Name = name;
			MemberType = ApiMemberType.Normal;
			ListAddItemMethod = "AddEntry";
		}

		public ApiMemberAttribute(string name, bool reference)
		{
			Name = name;
			MemberType = ApiMemberType.Normal;
			ListAddItemMethod = "AddEntry";
			IsReference = reference;
		}

		public ApiMemberAttribute(string name, ApiMemberType type, bool isEncrypt = false, bool reference = false)
		{
			Name = name;
			MemberType = type;
			IsEncrypt = isEncrypt;
			IsReference = reference;
		}
	}
}
