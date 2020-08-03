using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class DateRangeAttribute : System.Attribute
	{
		private string range = "";

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
					throw new Exception("Error Range");
				}
				DateTime minValue = DateTime.MinValue;
				DateTime minValue2 = DateTime.MinValue;
				DateTime.TryParse(array[0], out minValue);
				DateTime.TryParse(array[1], out minValue2);
				MinValue = minValue;
				MaxValue = minValue2;
			}
		}

		public OperateTime OperateContent
		{
			get;
			set;
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

		public DateRangeAttribute(string properDesc, OperateTime opContent, string range)
		{
			PropertyDesc = properDesc;
			OperateContent = opContent;
			this.range = range;
		}
	}
}
