using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public class CompareAttribute : System.Attribute
	{
		public OperateTime OperateContext
		{
			get;
			set;
		}

		public string Express
		{
			get;
			set;
		}

		public string PropertyName
		{
			get;
			set;
		}

		public string PropertyDesc
		{
			get;
			set;
		}

		public CompareAttribute(string express, string desc, OperateTime operateContext = OperateTime.Save)
		{
			OperateContext = operateContext;
			Express = express;
			PropertyDesc = desc;
		}
	}
}
