using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class NumberRangeAttribute : System.Attribute
	{
		protected string range = string.Empty;

		public string PropertyDesc
		{
			get;
			set;
		}

		public string RangeString
		{
			get
			{
				return range;
			}
			set
			{
				range = value;
				if (range.IndexOf('(') > -1 || range.IndexOf('（') > -1)
				{
					IncludeMinValue = false;
				}
				else if (range.IndexOf('[') > -1 || range.IndexOf('【') > -1)
				{
					IncludeMinValue = true;
				}
				if (range.IndexOf(')') > -1 || range.IndexOf('）') > -1)
				{
					IncludeMaxValue = false;
				}
				else if (range.IndexOf(']') > -1 || range.IndexOf('】') > -1)
				{
					IncludeMaxValue = true;
				}
				string text = range.Replace("(", "").Replace(")", "").Replace("（", "")
					.Replace("）", "");
				text = text.Replace("[", "").Replace("]", "").Replace("【", "")
					.Replace("】", "")
					.Replace("，", ",");
				string[] array = text.Split(',');
				if (array.Length < 2)
				{
					throw new Exception("Error range");
				}
				decimal minValue = decimal.MinValue;
				decimal maxValue = decimal.MinValue;
				decimal.TryParse(array[0], out minValue);
				decimal.TryParse(array[1], out maxValue);
				MinValue = minValue;
				MaxValue = maxValue;
			}
		}

		public OperateTime OperateContent
		{
			get;
			set;
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

		public NumberRangeAttribute(string properDesc, OperateTime opContent, string range)
		{
			PropertyDesc = properDesc;
			OperateContent = opContent;
			this.range = range;
		}
	}
}
