using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.MResource;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MResource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataModel.GL
{
	public class GLVoucherResourceAdapter : IResourceAdapter
	{
		public override string Prefix
		{
			get
			{
				return "GL-";
			}
			set
			{
			}
		}

		public override string FieldName
		{
			get
			{
				return "MNumber";
			}
			set
			{
			}
		}

		public override string PKFieldName
		{
			get
			{
				return "MItemID";
			}
			set
			{
			}
		}

		public override int MaxLength
		{
			get;
			set;
		}

		public override string TableName
		{
			get
			{
				return new MModelInfo(Type).TableName;
			}
			set
			{
			}
		}

		public override int StartWith
		{
			get;
			set;
		}

		public override string FilledChar
		{
			get;
			set;
		}

		public override bool StartAfterMax
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public override Type Type
		{
			get
			{
				return typeof(GLVoucherModel);
			}
			set
			{
			}
		}

		public override MActionResultCodeEnum ResourceUsedException
		{
			get
			{
				return MActionResultCodeEnum.MNumberInvalid;
			}
			set
			{
			}
		}

		public override List<MResource> Adapter(MContext ctx, MResourceFilter filter, List<MResource> resources = null)
		{
			List<MResource> list = new List<MResource>();
			MModelInfo mModelInfo = new MModelInfo(filter.Type);
			int num = int.Parse("9999999999".Substring(0, filter.MaxLength));
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			if (resources != null)
			{
				list3 = (from t in resources
				where t.MResourcePrefix == filter.ResourcePrefix
				select t).Select(delegate(MResource a)
				{
					int result = 0;
					if (RegExp.IsNumeric(a.MFieldValue.ToString()))
					{
						result = Convert.ToInt32(a.MFieldValue);
					}
					return result;
				}).ToList();
			}
			for (int i = 1; i <= num; i++)
			{
				if (!list3.Contains(i))
				{
					list2.Add(i);
				}
				if (list2.Count == filter.Count)
				{
					break;
				}
			}
			list2 = (from a in list2
			orderby a
			select a).ToList();
			foreach (int item in list2)
			{
				MResource resource = MResourceHelper.GetResource(ctx, filter, item);
				base.InsertIfNotContains(ctx, resources, resource, ref list);
				if (list.Count == filter.Count)
				{
					break;
				}
			}
			return list;
		}
	}
}
