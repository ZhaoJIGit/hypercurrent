using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.BusinessService
{
	public class APIBusinessBase<T> : BusinessServiceBase, IBasicBusiness<T> where T : BaseModel, new()
	{
		protected static readonly bool isAPITestMode = ConfigurationManager.AppSettings["IsAPITestMode"] == "1";

		protected const string DELETE_METHOD_NAME = "OnDeleteGetCmd";

		protected const int MaxVoucherAttachmentCount = 999;

		protected const string POST_METHOD_NAME = "OnPostGetCmd";

		protected bool IgnoreCommandValidate
		{
			get;
			set;
		}

		public Type InstanceType => base.GetType();

		public T Delete(MContext ctx, DeleteParam param)
		{
			CheckEndpointAvailable(ctx, -1, null);
			CheckRequestMethodImplemented(ctx, "OnDeleteGetCmd", param.MGroupName);
			List<CommandInfo> list = new List<CommandInfo>();
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			string text = (!string.IsNullOrWhiteSpace(param.MGroupName)) ? param.MGroupID : param.ElementID;
			GetParam getParam = new GetParam();
			getParam.IncludeDisabled = true;
			getParam.ElementID = param.ElementID;
			T val = string.IsNullOrWhiteSpace(param.ElementID) ? null : Enumerable.FirstOrDefault<T>((IEnumerable<T>)Get(ctx, getParam).rows);
			bool flag = val != null;
			OnDeleteEntryValidate(ctx, param, instance, val, ref flag);
			CheckRequestResourceExist(ctx, !flag);
			OnDeleteValidate(ctx, param, instance, val);
			if (val.ValidationErrors.Count > 0)
			{
				return val;
			}
			list.AddRange(OnDeleteGetCmd(ctx, param, instance, val));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			APIValidator.Validate<T>(val, ctx, num == 0, "DeleteFailed", "删除失败！", LangModule.Common, new object[0]);
			return val;
		}

		protected virtual void OnDeleteEntryValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, T model, ref bool isExist)
		{
		}

		protected virtual void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, T model)
		{
		}

		protected virtual List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, T model)
		{
			return new List<CommandInfo>();
		}

		public virtual void CheckEndpointAvailable(MContext ctx, int version = -1, string endPointName = null)
		{
			if (ctx.MOrgVersionID != version || string.IsNullOrWhiteSpace(endPointName))
			{
				return;
			}
			string text = (version == 0) ? "STANDARD" : "SMARTLEDGE";
			string textFormat = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "EndpointNotAvailable", "请求的接口{0}在{1}版中不存在。", endPointName, text);
			throw new ApiEndpoitUnavaliableException(textFormat);
		}

		public void CheckRequestResourceExist(MContext ctx, bool notFound)
		{
			if (!notFound)
			{
				return;
			}
			throw new ApiNotFoundException();
		}

		public virtual void CheckRequestMethodImplemented(MContext ctx, string methodName, string groupName)
		{
			if (!(InstanceType != typeof(APIBusinessBase<T>)) || InstanceType.GetMember(methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).Length != 1)
			{
				ThrowNotImplementedException(ctx);
			}
		}

		public void ThrowNotImplementedException(MContext ctx)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "RequestMethodNotImplemented", "请求的接口方法还未实现。");
			throw new NotImplementedException(text);
		}

		public DataGridJson<T> Get(MContext ctx, GetParam param)
		{
			CheckEndpointAvailable(ctx, -1, null);
			OnGetBefore(ctx, param);
			DataGridJson<T> dataGridJson = OnGet(ctx, param);
			CheckRequestResourceExist(ctx, !string.IsNullOrWhiteSpace(param.ElementID) && dataGridJson.rows.Count == 0);
			if (dataGridJson.rows.Count > 0)
			{
				APIDataPool instance = APIDataPool.GetInstance(ctx);
				foreach (T row in dataGridJson.rows)
				{
					OnGetAfter(ctx, param, instance, row);
				}
			}
			return dataGridJson;
		}

		protected virtual void OnGetBefore(MContext ctx, GetParam param)
		{
		}

		protected virtual DataGridJson<T> OnGet(MContext ctx, GetParam param)
		{
			return new DataGridJson<T>();
		}

		protected virtual void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, T model)
		{
		}

		public List<T> Post(MContext ctx, PostParam<T> param)
		{
			CheckEndpointAvailable(ctx, -1, null);
			CheckRequestMethodImplemented(ctx, "OnPostGetCmd", param.MGroupName);
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			OnPostBefore(ctx, param, instance);
			List<CommandInfo> list = new List<CommandInfo>();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (T data in param.DataList)
			{
				OnPostValidate(ctx, param, instance, data, param.IsPut, ref dictionary, ref dictionary2);
				if (data.ValidationErrors.Count <= 0)
				{
					data.UpdateFieldList.Add("MModifyDate");
					List<CommandInfo> collection = OnPostGetCmd(ctx, param, instance, data);
					list.AddRange(collection);
				}
			}
			if (Enumerable.Any<CommandInfo>((IEnumerable<CommandInfo>)list))
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			OnPostAfter(ctx, param, instance);
			return param.DataList;
		}

		protected virtual void OnPostBefore(MContext ctx, PostParam<T> param, APIDataPool dataPool)
		{
		}

		protected virtual void OnPostValidate(MContext ctx, PostParam<T> param, APIDataPool dataPool, T model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
		}

		protected virtual List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<T> param, APIDataPool dataPool, T model)
		{
			return new List<CommandInfo>();
		}

		protected virtual void OnPostAfter(MContext ctx, PostParam<T> param, APIDataPool dataPool)
		{
		}
	}
}
