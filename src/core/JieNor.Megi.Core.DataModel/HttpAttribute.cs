using JieNor.Megi.EntityModel.Enum;
using System;

namespace JieNor.Megi.Core.DataModel
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class HttpAttribute : System.Attribute
	{
		public string PropertyDesc
		{
			get;
			set;
		}

		public OperateTime OperateContent
		{
			get;
			set;
		}

		public HttpAttribute(string properDesc, OperateTime opContent)
		{
			PropertyDesc = properDesc;
			OperateContent = opContent;
		}
	}
}
