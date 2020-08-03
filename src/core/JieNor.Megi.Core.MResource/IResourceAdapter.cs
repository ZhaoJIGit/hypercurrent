using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MResource;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Core.MResource
{
	public abstract class IResourceAdapter
	{
		public virtual string FieldName
		{
			get;
			set;
		}

		public virtual string PKFieldName
		{
			get;
			set;
		}

		public virtual string Prefix
		{
			get;
			set;
		}

		public virtual Type Type
		{
			get;
			set;
		}

		public virtual int MaxLength
		{
			get;
			set;
		}

		public virtual int StartWith
		{
			get;
			set;
		}

		public virtual string FilledChar
		{
			get;
			set;
		}

		public virtual string TableName
		{
			get;
			set;
		}

		public virtual bool StartAfterMax
		{
			get;
			set;
		}

		public virtual MActionResultCodeEnum ResourceUsedException
		{
			get;
			set;
		}

		public abstract List<JieNor.Megi.EntityModel.MResource.MResource> Adapter(MContext ctx, MResourceFilter filter, List<JieNor.Megi.EntityModel.MResource.MResource> used = null);

		public bool Contains(MContext ctx, List<JieNor.Megi.EntityModel.MResource.MResource> src, JieNor.Megi.EntityModel.MResource.MResource target)
		{
			if (src == null || src.Count == 0)
			{
				return false;
			}
			return src.Exists((JieNor.Megi.EntityModel.MResource.MResource x) => x.MOrgID == ctx.MOrgID && x.MTableName == target.MTableName && x.MField == target.MField && x.MPrefix == target.MPrefix && x.MFieldValue.ToString() == target.MFieldValue.ToString() && x.MResourcePrefix == target.MResourcePrefix);
		}

		public void InsertIfNotContains(MContext ctx, List<JieNor.Megi.EntityModel.MResource.MResource> src, JieNor.Megi.EntityModel.MResource.MResource target, ref List<JieNor.Megi.EntityModel.MResource.MResource> result)
		{
			if (!Contains(ctx, src, target))
			{
				result.Add(target);
			}
		}
	}
}
