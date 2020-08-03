using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleCompare : BizRuleBase
	{
		public string Express
		{
			get;
			set;
		}

		[DataMember]
		public override string RuleName
		{
			get
			{
				return "按表达式校验";
			}
		}

		public BizRuleCompare(string express, string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
			Express = express;
		}

		public override void Verification(MContext ctx)
		{
		}
	}
}
