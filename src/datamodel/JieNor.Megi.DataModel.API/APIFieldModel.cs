using JieNor.Megi.Core.Attribute;
using System.Reflection;

namespace JieNor.Megi.DataModel.API
{
	public class APIFieldModel
	{
		public string Name
		{
			get;
			set;
		}

		public string ApiName
		{
			get;
			set;
		}

		public bool IsDetail
		{
			get;
			set;
		}

		public bool IsLang
		{
			get;
			set;
		}

		public bool IsEncrypt
		{
			get;
			set;
		}

		public bool IsList
		{
			get;
			set;
		}

		public ApiMemberAttribute ApiAttr
		{
			get;
			set;
		}

		public BaseDataAttribute BaseDataAttr
		{
			get;
			set;
		}

		public PropertyInfo Prop
		{
			get;
			set;
		}

		public string EnumMappingType
		{
			get;
			set;
		}

		public int Level
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(ApiName)) ? ApiName.Split('_').Length : 0;
			}
		}
	}
}
