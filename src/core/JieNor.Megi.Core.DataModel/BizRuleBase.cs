using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public abstract class BizRuleBase : IBizVerificationRule
	{
		[DataMember]
		public MContext MContext
		{
			get;
			set;
		}

		[DataMember]
		protected string PropertyName
		{
			get;
			set;
		}

		[DataMember]
		protected string ProperDesc
		{
			get;
			set;
		}

		[DataMember]
		public BaseModel BizData
		{
			get;
			set;
		}

		[DataMember]
		public OperateTime OperateContent
		{
			get;
			set;
		}

		[DataMember]
		public virtual string RuleName
		{
			get
			{
				return "字段校验";
			}
		}

		[DataMember]
		public AlertEnum MLevel
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		public BizRuleBase(string propertyName, string properDesc)
		{
			PropertyName = propertyName;
			ProperDesc = properDesc;
			MLevel = AlertEnum.Success;
			Message = "校验通过";
		}

		public virtual void Verification(MContext ctx)
		{
		}

		protected void SetMessageInfor(string message = "", AlertEnum level = AlertEnum.Error)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				message = string.Format("{0}输入的值有误，请重新录入{0}", ProperDesc);
			}
			MLevel = level;
			Message = message;
		}
	}
}
