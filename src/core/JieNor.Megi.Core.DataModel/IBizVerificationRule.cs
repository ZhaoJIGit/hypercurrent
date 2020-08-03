using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	public interface IBizVerificationRule
	{
		MContext MContext
		{
			get;
			set;
		}

		[DataMember]
		string RuleName
		{
			get;
		}

		BaseModel BizData
		{
			get;
			set;
		}

		OperateTime OperateContent
		{
			get;
			set;
		}

		AlertEnum MLevel
		{
			get;
			set;
		}

		[DataMember]
		string Message
		{
			get;
			set;
		}

		void Verification(MContext ctx);
	}
}
