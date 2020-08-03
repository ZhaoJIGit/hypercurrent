using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Reflection;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleRangeNumber : BizRuleBase
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "字段值范围校验";
			}
		}

		public decimal MinValue
		{
			get;
			set;
		}

		public decimal MaxValue
		{
			get;
			set;
		}

		public bool IncludeMinValue
		{
			get;
			set;
		}

		public bool IncludeMaxValue
		{
			get;
			set;
		}

		public BizRuleRangeNumber(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName))
			{
				string message = string.Format("{0}的输入范围是{1} ~ {2}，请重新录入{0}", base.ProperDesc, MinValue, MaxValue);
				object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
				if (propertyValue == null)
				{
					base.SetMessageInfor(message, AlertEnum.Error);
				}
				PropertyInfo property = base.BizData.GetType().GetProperty(base.PropertyName);
				if (property.PropertyType == typeof(short) || property.PropertyType == typeof(long) || property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(float) || property.PropertyType == typeof(short) || property.PropertyType == typeof(long) || property.PropertyType == typeof(uint) || property.PropertyType == typeof(ushort) || property.PropertyType == typeof(uint) || property.PropertyType == typeof(ulong))
				{
					decimal d = default(decimal);
					if (decimal.TryParse(propertyValue.ToString(), out d))
					{
						if ((IncludeMinValue && d < MinValue) || (!IncludeMinValue && d <= MinValue))
						{
							base.SetMessageInfor(message, AlertEnum.Error);
						}
						if ((IncludeMaxValue && d > MaxValue) || (!IncludeMaxValue && d >= MaxValue))
						{
							base.SetMessageInfor(message, AlertEnum.Error);
						}
					}
				}
			}
		}
	}
}
