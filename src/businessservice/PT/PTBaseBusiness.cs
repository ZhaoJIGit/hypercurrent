using JieNor.Megi.EntityModel.Context;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;

namespace JieNor.Megi.BusinessService.PT
{
	public class PTBaseBusiness
	{
		public static ConcurrentDictionary<string, dynamic> cacheList = new ConcurrentDictionary<string, object>();

		public const string PRT_LAYOUT_KEY = "{0}_PrtLayout";

		public const string PRT_SETTING_KEY = "{0}_PrtSetting_{1}_{2}";

		public Dictionary<string, string> GetKeyValueList(MContext ctx, string bizObject)
		{
			return GetKeyValueList(ctx, bizObject, false);
		}

		public void RemovePrintSettingCache(MContext ctx, string bizObject)
		{
			string key = $"{ctx.MOrgID}_PrtSetting_{bizObject}_{ctx.MLCID}";
			object obj = null;
			((ConcurrentDictionary<string, object>)cacheList).TryRemove(key, out obj);
		}

		public Dictionary<string, string> GetKeyValueList(MContext ctx, string bizObject, bool isUpdate = false)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dynamic pTListFromCache = GetPTListFromCache(ctx, bizObject, isUpdate);
			//if (_003C_003Eo__5._003C_003Ep__4 == null)
			//{
			//	_003C_003Eo__5._003C_003Ep__4 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(PTBaseBusiness)));
			//}
			foreach (dynamic item in pTListFromCache)
			{
				//if (_003C_003Eo__5._003C_003Ep__3 == null)
				//{
				//	_003C_003Eo__5._003C_003Ep__3 = CallSite<Action<CallSite, Dictionary<string, string>, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", null, typeof(PTBaseBusiness), new CSharpArgumentInfo[3]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}
				//Action<CallSite, Dictionary<string, string>, object, object> target = _003C_003Eo__5._003C_003Ep__3.Target;
				//CallSite<Action<CallSite, Dictionary<string, string>, object, object>> _003C_003Ep__ = _003C_003Eo__5._003C_003Ep__3;
				//Dictionary<string, string> arg = dictionary;
				//if (_003C_003Eo__5._003C_003Ep__0 == null)
				//{
				//	_003C_003Eo__5._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "MItemID", typeof(PTBaseBusiness), new CSharpArgumentInfo[1]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}

				//object arg2 = item.MItemID;
				//object arg2 = _003C_003Eo__5._003C_003Ep__0.Target(_003C_003Eo__5._003C_003Ep__0, item);
				//if (_003C_003Eo__5._003C_003Ep__2 == null)
				//{
				//	_003C_003Eo__5._003C_003Ep__2 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "HtmlDecode", null, typeof(PTBaseBusiness), new CSharpArgumentInfo[2]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}
				//Func<CallSite, Type, object, object> target2 = _003C_003Eo__5._003C_003Ep__2.Target;
				//CallSite<Func<CallSite, Type, object, object>> _003C_003Ep__2 = _003C_003Eo__5._003C_003Ep__2;
				//Type typeFromHandle = typeof(HttpUtility);

				//if (_003C_003Eo__5._003C_003Ep__1 == null)
				//{
				//	_003C_003Eo__5._003C_003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "MName", typeof(PTBaseBusiness), new CSharpArgumentInfo[1]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}

				dictionary.Add(item.MItemID, HttpUtility.HtmlDecode(item.MName));

				//target(_003C_003Ep__, arg, arg2, HttpUtility.HtmlDecode(item.MName));


			}
			return dictionary;
		}

		private dynamic GetPTListFromCache(MContext ctx, string bizObject, bool isUpdate = false)
		{
			string text = $"{ctx.MOrgID}_PrtSetting_{bizObject}_{ctx.MLCID}";
			object obj = null;
			if (((ConcurrentDictionary<string, object>)cacheList).ContainsKey(text) && !isUpdate)
			{
				obj = ((ConcurrentDictionary<string, object>)cacheList)[text];
			}
			else
			{
				obj = ((!(bizObject == "PayRun")) ? ((!(bizObject == "Voucher")) ? ((object)new PTBizBusiness().GetList(ctx)) : ((object)new PTVoucherBusiness().GetList(ctx))) : new PTSalaryListBusiness().GetList(ctx));
				if (((ConcurrentDictionary<string, object>)cacheList).ContainsKey(text))
				{
					((ConcurrentDictionary<string, object>)cacheList)[text] = obj;
				}
				else
				{
					cacheList.TryAdd(text, obj);
				}
			}
			return obj;
		}
	}
}
