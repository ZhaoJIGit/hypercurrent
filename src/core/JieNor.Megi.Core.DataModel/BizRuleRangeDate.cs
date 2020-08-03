using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleRangeDate : BizRuleBase
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "字段值范围校验";
			}
		}

		public DateTime MinValue
		{
			get;
			set;
		}

		public DateTime MaxValue
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

		public BizRuleRangeDate(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			DateTime dateTime;
			string message;
			DateTime minValue;
			int num;
			if (base.BizData != null && !string.IsNullOrWhiteSpace(base.PropertyName))
			{
				object propertyValue = base.BizData.GetPropertyValue(base.PropertyName);
				if (propertyValue != null)
				{
					string properDesc = base.ProperDesc;
					dateTime = MinValue;
					string arg = dateTime.ToShortDateString();
					dateTime = MaxValue;
					message = string.Format("{0}的输入范围是{1} ~ {2}，请重新录入{0}", properDesc, arg, dateTime.ToShortDateString());
					PropertyInfo property = base.BizData.GetType().GetProperty(base.PropertyName);
					if (!(property.PropertyType == typeof(DateTime)))
					{
						return;
					}
					minValue = DateTime.MinValue;
					if (!DateTime.TryParse(propertyValue.ToString(), out minValue))
					{
						return;
					}
					if (IncludeMinValue)
					{
						DateTime t = new DateTime(minValue.Year, minValue.Month, minValue.Day);
						dateTime = MinValue;
						int year = dateTime.Year;
						dateTime = MinValue;
						int month = dateTime.Month;
						dateTime = MinValue;
						if (!(t < new DateTime(year, month, dateTime.Day)))
						{
							goto IL_0121;
						}
						num = 1;
						goto IL_0180;
					}
					goto IL_0121;
				}
			}
			return;
			IL_01ec:
			int num2;
			if (!IncludeMaxValue)
			{
				DateTime t2 = new DateTime(minValue.Year, minValue.Month, minValue.Day);
				dateTime = MaxValue;
				int year2 = dateTime.Year;
				dateTime = MaxValue;
				int month2 = dateTime.Month;
				dateTime = MaxValue;
				num2 = ((t2 >= new DateTime(year2, month2, dateTime.Day)) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			goto IL_024b;
			IL_0121:
			if (!IncludeMinValue)
			{
				DateTime t3 = new DateTime(minValue.Year, minValue.Month, minValue.Day);
				dateTime = MinValue;
				int year3 = dateTime.Year;
				dateTime = MinValue;
				int month3 = dateTime.Month;
				dateTime = MinValue;
				num = ((t3 <= new DateTime(year3, month3, dateTime.Day)) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			goto IL_0180;
			IL_0180:
			if (num != 0)
			{
				base.SetMessageInfor(message, AlertEnum.Error);
			}
			if (IncludeMaxValue)
			{
				DateTime t4 = new DateTime(minValue.Year, minValue.Month, minValue.Day);
				dateTime = MaxValue;
				int year4 = dateTime.Year;
				dateTime = MaxValue;
				int month4 = dateTime.Month;
				dateTime = MaxValue;
				if (!(t4 > new DateTime(year4, month4, dateTime.Day)))
				{
					goto IL_01ec;
				}
				num2 = 1;
				goto IL_024b;
			}
			goto IL_01ec;
			IL_024b:
			if (num2 != 0)
			{
				base.SetMessageInfor(message, AlertEnum.Error);
			}
		}
	}
}
