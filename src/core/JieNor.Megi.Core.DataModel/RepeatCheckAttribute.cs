using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public class RepeatCheckAttribute : System.Attribute
	{
		public OperateTime OperateContext
		{
			get;
			set;
		}

		public string Properties
		{
			get;
			set;
		}

		public string PropertyDesc
		{
			get;
			set;
		}

		public RepeatCheckAttribute(string properties, string propertyDesc, OperateTime operateContext = OperateTime.Save)
		{
			OperateContext = operateContext;
			Properties = properties;
			PropertyDesc = propertyDesc;
		}
	}
}
