using Fasterflect;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizRuleMustInputEntry : BizRuleMustInput
	{
		[DataMember]
		public override string RuleName
		{
			get
			{
				return "分录必录检查";
			}
		}

		public BizRuleMustInputEntry(string propertyName, string properDesc)
			: base(propertyName, properDesc)
		{
		}

		public override void Verification(MContext ctx)
		{
			string message = string.Format("{0}是必录项，请录入明细的分录数据", base.ProperDesc);
			if (base.BizData == null)
			{
				base.SetMessageInfor(message, AlertEnum.Error);
			}
			if (!string.IsNullOrWhiteSpace(base.PropertyName))
			{
				IEnumerable enumerable = base.BizData.GetPropertyValue(base.PropertyName) as IEnumerable;
				bool flag = false;
				foreach (object item in enumerable)
				{
					if (item != null)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					base.SetMessageInfor(message, AlertEnum.Error);
				}
			}
		}
	}
}
