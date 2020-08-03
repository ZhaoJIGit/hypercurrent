using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleEmail : BizRuleBase
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "Email地址合法性检查";
			}
		}

		public BizRuleEmail(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName))
			{
				object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
				if (propertyValue != null)
				{
					PropertyInfo property = base.BizData.GetType().GetProperty(base.PropertyName);
					if (property.PropertyType == typeof(string) && !string.IsNullOrWhiteSpace(propertyValue.ToString()) && !Regex.IsMatch(propertyValue.ToString(), "^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$"))
					{
						string message = string.Format("{0}：输入的Email地址不合法，请重新输入", base.ProperDesc);
						base.SetMessageInfor(message, AlertEnum.Error);
					}
				}
			}
		}
	}
}
