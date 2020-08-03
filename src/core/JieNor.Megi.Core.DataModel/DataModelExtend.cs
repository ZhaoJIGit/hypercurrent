using Fasterflect;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.Core.DataModel
{
	public static class DataModelExtend
	{
		public static void AddBizVerificationRule(this BaseModel model, MContext ctx, IBizVerificationRule rule)
		{
			if (rule.BizData == null)
			{
				rule.MContext = ctx;
				rule.BizData = model;
			}
			model.BizVerificationRules.Add(rule);
		}

		public static OperationResult Verification(this BaseModel model, MContext ctx, OperateTime opContent)
		{
			OperationResult operationResult = new OperationResult();
			if (model == null)
			{
				return operationResult;
			}
			List<IBizVerificationRule> bizVerificationRule = ModelInfoManager.GetBizVerificationRule(ctx, model);
			model.BizVerificationRules.AddRange(bizVerificationRule);
			PropertyInfo[] properties = (from f in model.GetType().GetProperties()
			where f.PropertyType.IsGenericType && ModelInfoManager.HasEntryAttribute(f) && f.PropertyType.GetGenericArguments()[0].GetTypeInfo().IsSubclassOf(typeof(BaseModel))
			select f).ToArray();
			GetEntryRowVerification(ctx, model, properties);
			if (model.BizVerificationRules != null && model.BizVerificationRules.Count > 0)
			{
				ExecuteVerification(ctx, opContent, operationResult, model);
			}
			ExecuteEntryVerification(ctx, model, opContent, operationResult, properties);
			return operationResult;
		}

		private static void ExecuteEntryVerification(MContext ctx, BaseModel model, OperateTime opContent, OperationResult verifi, PropertyInfo[] properties)
		{
			if (properties != null || properties.Length != 0)
			{
				foreach (PropertyInfo propertyInfo in properties)
				{
					IEnumerable enumerable = model.GetPropertyValue(propertyInfo.Name) as IEnumerable;
					if (enumerable != null)
					{
						foreach (BaseModel item in enumerable)
						{
							if (item.BizVerificationRules == null || item.BizVerificationRules.Count == 0)
							{
								break;
							}
							ExecuteVerification(ctx, opContent, verifi, item);
						}
					}
				}
			}
		}

		private static void GetEntryRowVerification(MContext ctx, BaseModel model, PropertyInfo[] properties)
		{
			if (properties != null && properties.Length != 0)
			{
				foreach (PropertyInfo propertyInfo in properties)
				{
					IEnumerable enumerable = model.GetPropertyValue(propertyInfo.Name) as IEnumerable;
					if (enumerable != null)
					{
						List<IBizVerificationRule> list = new List<IBizVerificationRule>();
						{
							IEnumerator enumerator = enumerable.GetEnumerator();
							try
							{
								if (enumerator.MoveNext())
								{
									BaseModel model2 = (BaseModel)enumerator.Current;
									list = ModelInfoManager.GetBizVerificationRule(ctx, model2);
								}
							}
							finally
							{
								IDisposable disposable = enumerator as IDisposable;
								if (disposable != null)
								{
									disposable.Dispose();
								}
							}
						}
						if (list != null && list.Count != 0)
						{
							foreach (BaseModel item in enumerable)
							{
								item.BizVerificationRules.AddRange(list);
							}
						}
					}
				}
			}
		}

		private static void ExecuteVerification(MContext ctx, OperateTime opContent, OperationResult verifi, BaseModel modelData)
		{
			foreach (IBizVerificationRule bizVerificationRule in modelData.BizVerificationRules)
			{
				if (!((Enum)(object)bizVerificationRule.OperateContent).HasFlag((Enum)(object)opContent))
				{
					bizVerificationRule.BizData = null;
				}
				else
				{
					bizVerificationRule.MContext = ctx;
					bizVerificationRule.BizData = modelData;
					bizVerificationRule.MLevel = AlertEnum.Success;
					bizVerificationRule.Verification(ctx);
					if (bizVerificationRule.MLevel == AlertEnum.Warning || bizVerificationRule.MLevel == AlertEnum.Danger || bizVerificationRule.MLevel == AlertEnum.Error)
					{
						verifi.VerificationInfor.Add(new BizVerificationInfor
						{
							CheckItem = bizVerificationRule.RuleName,
							Level = bizVerificationRule.MLevel,
							Message = bizVerificationRule.Message
						});
					}
					bizVerificationRule.BizData = null;
				}
			}
		}
	}
}
