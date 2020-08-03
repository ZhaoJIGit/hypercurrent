using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleMustInput : BizRuleBase
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "必录项检查";
			}
		}

		public BizRuleMustInput(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName))
			{
				MyPropertyInfo[] bizModelProperty = ModelInfoManager.GetBizModelProperty(ctx, base.BizData);
				MyPropertyInfo myPropertyInfo = bizModelProperty.FirstOrDefault((MyPropertyInfo f) => f.Property.Name.EqualsIgnoreCase(base.PropertyName));
				if (myPropertyInfo != null && myPropertyInfo.HaveBDField)
				{
					object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
					if (propertyValue == null)
					{
						base.SetMessageInfor("", AlertEnum.Error);
					}
					else
					{
						string message = string.Format("{0}是必录项，请录入{0}", base.ProperDesc);
						PropertyInfo property = myPropertyInfo.Property;
						if (property.PropertyType == typeof(string))
						{
							if (string.IsNullOrWhiteSpace(propertyValue.ToString()))
							{
								base.SetMessageInfor(message, AlertEnum.Error);
							}
						}
						else if (property.PropertyType == typeof(DateTime))
						{
							DateTime d;
							if (!DateTime.TryParse(propertyValue.ToString(), out d))
							{
								base.SetMessageInfor(message, AlertEnum.Error);
							}
							else if (d == DateTime.MinValue || d == DateTime.MaxValue)
							{
								base.SetMessageInfor(string.Format("{0}：错误的日期值，请重新输入日期", base.ProperDesc), AlertEnum.Error);
							}
						}
						else if ((property.PropertyType == typeof(short) || property.PropertyType == typeof(long) || property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(float) || property.PropertyType == typeof(short) || property.PropertyType == typeof(long) || property.PropertyType == typeof(uint) || property.PropertyType == typeof(ushort) || property.PropertyType == typeof(uint) || property.PropertyType == typeof(ulong)) && propertyValue.ToString().Trim().Equals(0))
						{
							base.SetMessageInfor(message, AlertEnum.Error);
						}
					}
				}
			}
		}
	}
}
