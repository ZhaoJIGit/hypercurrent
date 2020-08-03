using JieNor.Megi.Common.MResource;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MResource;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.Core.MResource
{
	public class MResourceHelper
	{
		public static object _lock = new object();

		public static Hashtable _orgLocks = new Hashtable();

		public static Dictionary<Type, MModelInfo> ModelInfoCache = new Dictionary<Type, MModelInfo>();

		public static object _modelInfoLock = new object();

		public static Hashtable GLDataPoolCache = new Hashtable();

		public static Hashtable GLDataPoolCacheByToken = new Hashtable();

		public static List<string> releasePool = new List<string>();

		public static List<JieNor.Megi.EntityModel.MResource.MResource> GetByOrg(MContext ctx)
		{
			if (ctx == null || string.IsNullOrWhiteSpace(ctx.MOrgID))
			{
				return null;
			}
			return MResourceManager.Get(null, null, ctx.MOrgID);
		}

		public static void Save(MContext ctx, List<JieNor.Megi.EntityModel.MResource.MResource> resources)
		{
			if (resources != null && resources.Any())
			{
				resources.ForEach(delegate(JieNor.Megi.EntityModel.MResource.MResource x)
				{
					x.MID = UUIDHelper.GetGuid();
					x.MUserID = (x.MUserID ?? ctx.MUserID);
					x.MOrgID = (x.MOrgID ?? ctx.MOrgID);
					x.MAccessToken = (x.MAccessToken ?? ctx.MAccessToken);
					x.MCreateDate = ((x.MCreateDate.Year <= 1900) ? DateTime.Now : x.MCreateDate);
				});
				ctx.MResourceIds = (ctx.MResourceIds ?? new List<string>());
				ctx.MResourceIds.AddRange((from x in resources
				select x.MID).ToList());
				MResourceManager.Save(resources);
			}
		}

		public static void RemoveByIDs(List<string> ids)
		{
			if (ids != null && ids.Any())
			{
				MResourceManager.RemoveWithIDs(ids);
			}
		}

		public static void RemoveByContext(MContext ctx)
		{
			if (ctx != null && !string.IsNullOrWhiteSpace(ctx.MOrgID) && ctx.MResourceIds != null && ctx.MResourceIds.Count != 0)
			{
				MResourceManager.RemoveWithIDs(ctx.MResourceIds);
			}
		}

		public static void RemoveByOrg(MContext ctx)
		{
			if (ctx != null && !string.IsNullOrWhiteSpace(ctx.MOrgID))
			{
				MResourceManager.Remove(null, null, ctx.MOrgID);
			}
		}

		public static void RemoveByAccessToken(MContext ctx)
		{
			if (ctx != null && !string.IsNullOrWhiteSpace(ctx.MAccessToken))
			{
				MResourceManager.Remove(ctx.MAccessToken, null, null);
			}
		}

		public static List<JieNor.Megi.EntityModel.MResource.MResource> GetResource(MContext ctx, MResourceFilter filter)
		{
			List<JieNor.Megi.EntityModel.MResource.MResource> usedResource = GetUsedResource(ctx, filter);
			List<JieNor.Megi.EntityModel.MResource.MResource> list = filter.Adapter.Adapter(ctx, filter, usedResource);
			list = ((list == null) ? new List<JieNor.Megi.EntityModel.MResource.MResource>() : (from x in list
			where x.MFieldValue != null
			select x).ToList());
			Save(ctx, list);
			return list;
		}

		public static void TryLockResource(MContext ctx, MResourceFilter filter, List<JieNor.Megi.EntityModel.MResource.MResource> resouce)
		{
			List<JieNor.Megi.EntityModel.MResource.MResource> src = FetchResource(ctx);
			bool flag = false;
			for (int i = 0; i < resouce.Count; i++)
			{
				flag = (flag || filter.Adapter.Contains(ctx, src, resouce[i]));
			}
			if (flag)
			{
				throw new MActionException(new List<MActionResultCodeEnum>
				{
					filter.Adapter.ResourceUsedException
				});
			}
			Save(ctx, resouce);
		}

		public static List<JieNor.Megi.EntityModel.MResource.MResource> GetUsedResource(MContext ctx, MResourceFilter filter)
		{
			List<JieNor.Megi.EntityModel.MResource.MResource> first = FetchResource(ctx) ?? new List<JieNor.Megi.EntityModel.MResource.MResource>();
			List<JieNor.Megi.EntityModel.MResource.MResource> second = GetUsedResourceFromDB(ctx, filter) ?? new List<JieNor.Megi.EntityModel.MResource.MResource>();
			return first.Concat(second).ToList();
		}

		public static JieNor.Megi.EntityModel.MResource.MResource GetResource(MContext ctx, MResourceFilter filter, object value)
		{
			return new JieNor.Megi.EntityModel.MResource.MResource
			{
				MAccessToken = ctx.MAccessToken,
				MOrgID = ctx.MOrgID,
				MUserID = ctx.MUserID,
				MPrefix = filter.Prefix,
				MResourcePrefix = filter.ResourcePrefix,
				MField = filter.FieldName,
				MTableName = filter.TableName,
				MFieldValue = value.ToString().ToFixLengthString(filter.MaxLength, filter.FilledChar)
			};
		}

		public static bool IsResourceUsed(MContext ctx, MResourceFilter filter, object value)
		{
			List<JieNor.Megi.EntityModel.MResource.MResource> usedResource = GetUsedResource(ctx, filter);
			JieNor.Megi.EntityModel.MResource.MResource resource = GetResource(ctx, filter, value);
			return filter.Adapter.Contains(ctx, usedResource, resource);
		}

		public static void ReleaseResource(MContext ctx)
		{
			AddReleaseGLDataPool(ctx);
			try
			{
				RemoveByContext(ctx);
			}
			catch (Exception)
			{
				try
				{
					RemoveByAccessToken(ctx);
				}
				catch (Exception)
				{
					RemoveByOrg(ctx);
				}
			}
		}

		public static void ReleaseGLDataPool()
		{
			int num = 8;
			while (true)
			{
				if (releasePool.Count <= 0)
				{
					break;
				}
				MResouceBase mResouceBase = (MResouceBase)GLDataPoolCacheByToken[releasePool[0]];
				if (mResouceBase == null)
				{
					GLDataPoolCacheByToken.Remove(releasePool[0]);
					releasePool.RemoveAt(0);
				}
				else
				{
					DateTime dateTime = mResouceBase.ModifyTime;
					dateTime = dateTime.AddSeconds((double)num);
					if (dateTime.CompareTo(DateTime.Now) >= 0)
					{
						break;
					}
					GLDataPoolCacheByToken.Remove(releasePool[0]);
					mResouceBase.Dispose();
					mResouceBase = null;
					releasePool.RemoveAt(0);
				}
			}
		}

		public static void AddReleaseGLDataPool(MContext ctx)
		{
			if (ctx != null)
			{
				MResouceBase mResouceBase = (MResouceBase)GLDataPoolCache[ctx];
				if (mResouceBase != null)
				{
					GLDataPoolCache.Remove(ctx);
					mResouceBase.Dispose();
					mResouceBase = null;
				}
				if (!string.IsNullOrEmpty(ctx.MAccessToken) && GLDataPoolCacheByToken.ContainsKey(ctx.MAccessToken))
				{
					releasePool.Add(ctx.MAccessToken);
				}
			}
		}

		public static void RemoveOrgLock(MContext ctx)
		{
			if (ctx != null && !string.IsNullOrEmpty(ctx.MOrgID) && _orgLocks.ContainsKey(ctx.MOrgID))
			{
				_orgLocks.Remove(ctx.MOrgID);
			}
		}

		public static object GetOrgLock(string orgID)
		{
			object obj = null;
			if (_orgLocks.ContainsKey(orgID))
			{
				obj = _orgLocks[orgID];
				if (obj != null)
				{
					return obj;
				}
			}
			lock (_lock)
			{
				if (!_orgLocks.ContainsKey(orgID))
				{
					obj = new object();
					_orgLocks.Add(orgID, obj);
				}
				else
				{
					obj = _orgLocks[orgID];
				}
			}
			if (obj == null)
			{
				return GetOrgLock(orgID);
			}
			return obj;
		}

		private static List<JieNor.Megi.EntityModel.MResource.MResource> FetchResource(MContext ctx)
		{
			return GetByOrg(ctx);
		}

		public static List<JieNor.Megi.EntityModel.MResource.MResource> GetUsedResourceFromDB(MContext ctx, MResourceFilter filter)
		{
			MModelInfo modelInfo = GetModelInfo(filter.Type);
			BaseModel baseModel = (BaseModel)modelInfo.Instance;
			List<JieNor.Megi.EntityModel.MResource.MResource> list = new List<JieNor.Megi.EntityModel.MResource.MResource>();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string text = string.Format(" select {0},{1} from {2} where MOrgID = @MOrgID and MIsDelete = 0 ", baseModel.PKFieldName, filter.FieldName, baseModel.TableName);
			if (filter.SqlFilter != null)
			{
				text = text + " " + filter.SqlFilter;
				if (filter.SqlFitlerParams != null && filter.SqlFitlerParams.Count() > 0)
				{
					list2.AddRange(filter.SqlFitlerParams);
				}
			}
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(text, list2.ToArray());
			return BindData2Resource(ctx, modelInfo, filter, ds);
		}

		private static List<JieNor.Megi.EntityModel.MResource.MResource> BindData2Resource(MContext ctx, MModelInfo info, MResourceFilter filter, DataSet ds)
		{
			List<JieNor.Megi.EntityModel.MResource.MResource> list = new List<JieNor.Megi.EntityModel.MResource.MResource>();
			if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				DataRow dataRow = ds.Tables[0].Rows[i];
				JieNor.Megi.EntityModel.MResource.MResource item = new JieNor.Megi.EntityModel.MResource.MResource
				{
					MUserID = ctx.MUserID,
					MOrgID = ctx.MOrgID,
					MTableName = info.TableName,
					MField = filter.FieldName,
					MPrefix = filter.Prefix,
					MResourcePrefix = filter.ResourcePrefix,
					MAccessToken = ctx.MAccessToken,
					MPKFieldValue = (dataRow.IsNull(filter.PKFieldName) ? null : dataRow[filter.PKFieldName].ToString()),
					MFieldValue = dataRow[filter.FieldName].ToString()
				};
				list.Add(item);
			}
			return list;
		}

		private static MModelInfo GetModelInfo(Type type)
		{
			if (ModelInfoCache.Count == 0 || !ModelInfoCache.ContainsKey(type) || ModelInfoCache[type] == null)
			{
				lock (_modelInfoLock)
				{
					if (!ModelInfoCache.ContainsKey(type))
					{
						ModelInfoCache.Add(type, new MModelInfo(type));
					}
					else if (ModelInfoCache[type] == null)
					{
						ModelInfoCache[type] = new MModelInfo(type);
					}
				}
			}
			return ModelInfoCache[type];
		}
	}
}
