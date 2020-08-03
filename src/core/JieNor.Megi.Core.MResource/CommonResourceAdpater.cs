using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MResource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Core.MResource
{
	public class CommonResourceAdpater : IResourceAdapter
	{
		public override List<JieNor.Megi.EntityModel.MResource.MResource> Adapter(MContext ctx, MResourceFilter filter, List<JieNor.Megi.EntityModel.MResource.MResource> resources = null)
		{
			MModelInfo mModelInfo = new MModelInfo(filter.Type);
			double num = Math.Pow(10.0, (double)filter.MaxLength) - 1.0;
			if (num < (double)filter.StartWith)
			{
				throw new Exception("资源的最大值不可小于开始值");
			}
			List<JieNor.Megi.EntityModel.MResource.MResource> list = new List<JieNor.Megi.EntityModel.MResource.MResource>();
			int num2 = filter.StartAfterMax ? ((from x in resources
			select int.Parse(x.MFieldValue.ToString())).Max() + 1) : filter.StartWith;
			for (int i = num2; (double)i < Math.Pow(10.0, (double)filter.MaxLength); i++)
			{
				if (!((double)list.Count < num))
				{
					break;
				}
				JieNor.Megi.EntityModel.MResource.MResource target = new JieNor.Megi.EntityModel.MResource.MResource
				{
					MUserID = ctx.MUserID,
					MAccessToken = ctx.MAccessToken,
					MOrgID = ctx.MOrgID,
					MTableName = mModelInfo.TableName,
					MField = filter.FieldName,
					MPrefix = filter.Prefix,
					MResourcePrefix = filter.ResourcePrefix,
					MFieldValue = (object)i
				};
				base.InsertIfNotContains(ctx, resources, target, ref list);
			}
			return list;
		}
	}
}
