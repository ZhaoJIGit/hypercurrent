using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Reflection;

namespace JieNor.Megi.Core
{
	public class MyPropertyInfo
	{
		public PropertyInfo Property
		{
			get;
			set;
		}

		public bool IsEncrypt
		{
			get;
			set;
		}

		public bool HaveBDField
		{
			get;
			set;
		}

		public bool InsertOnly
		{
			get;
			set;
		}

		public List<string> AppSourceList
		{
			get;
			set;
		}

		public string AppSourceString
		{
			get
			{
				if (AppSourceList == null || AppSourceList.Count == 0)
				{
					return "";
				}
				return "'" + string.Join(",", AppSourceList) + "'";
			}
			private set
			{
			}
		}

		public List<IBizVerificationRule> VerificationRules
		{
			get;
			set;
		}

		public BizRuleAutoMakeNo AutoMakeNo
		{
			get;
			set;
		}

		public MyPropertyInfo(PropertyInfo property, bool isEncrypt)
		{
			Property = property;
			IsEncrypt = isEncrypt;
			VerificationRules = new List<IBizVerificationRule>();
		}
	}
}
