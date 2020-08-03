using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class AutoBillNoAttribute : System.Attribute
	{
		public OperateTime OperateContext
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

		public AutoBillNoAttribute(string propertyName, string propertyDesc, OperateTime operateContext = OperateTime.Save)
		{
			OperateContext = operateContext;
			PropertyName = propertyName;
			PropertyDesc = propertyDesc;
		}
	}
}
