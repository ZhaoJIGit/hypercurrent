using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.REG
{
	public class REGGlobalizationRepository : DataServiceT<REGGlobalizationModel>
	{
		private static ConcurrentDictionary<string, string> _clientGlobal = new ConcurrentDictionary<string, string>();

		private static object _lock = new object();

		public OperationResult GlobalizationUpdate(MContext ctx, REGGlobalizationModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(model.MOrgID))
			{
				model.MOrgID = ctx.MOrgID;
			}
			List<string> fields = string.IsNullOrWhiteSpace(model.MUpdateFields) ? null : model.MUpdateFields.Split(',').ToList();
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<REGGlobalizationModel>(ctx, model, fields, true);
			MultiDBCommand[] cmdArray = new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					CommandList = insertOrUpdateCmd,
					DBType = SysOrBas.Sys
				},
				new MultiDBCommand(ctx)
				{
					CommandList = insertOrUpdateCmd,
					DBType = SysOrBas.Bas
				}
			};
			operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray);
			if (operationResult.Success && !string.IsNullOrWhiteSpace(model.MSystemLanguage))
			{
				ctx.MActiveLocaleIDS = model.MSystemLanguage.Split(',').ToList();
				if (!ctx.MActiveLocaleIDS.Contains(ctx.MLCID))
				{
					ctx.MLCID = ctx.MActiveLocaleIDS[0];
					operationResult.ObjectID = "1";
				}
				ContextHelper.MContext = ctx;
			}
			return operationResult;
		}

		public REGGlobalizationModel GetOrgGlobalizationDetail(MContext ctx, string orgid)
		{
			return GetDataModelByFilter(ctx, new SqlWhere().AddDeleteFilter("MIsDelete", SqlOperators.Equal, false).Equal("MOrgID", orgid));
		}

		public string GetClientGlobalInfo(MContext ctx)
		{
			if (ctx == null || string.IsNullOrEmpty(ctx.MOrgID))
			{
				return string.Empty;
			}
			if (_clientGlobal.Count > 0 && _clientGlobal.Keys.Contains(ctx.MOrgID))
			{
				return _clientGlobal[ctx.MOrgID];
			}
			REGGlobalizationModel orgGlobalizationDetail = GetOrgGlobalizationDetail(ctx, ctx.MOrgID);

			MLogger.Log($"GetClientGlobalInfo->orgGlobalizationDetail is null?{null == orgGlobalizationDetail}");
			if (orgGlobalizationDetail == null)
			{
				return string.Empty;
			}
			MLogger.Log($"GetClientGlobalInfo->orgGlobalizationDetail ->{JsonConvert.SerializeObject(orgGlobalizationDetail)}");

			BASGlobalClientModel bASGlobalClientModel = new BASGlobalClientModel
			{
				MDateFormat = orgGlobalizationDetail.MSystemDate,
				MTimeFormat = orgGlobalizationDetail.MSystemTime,
				MDigitDot = orgGlobalizationDetail.MSystemDigitDot,
				MDigitGroupingSymbol = orgGlobalizationDetail.MSystemDigitGroupingSymbol,
				MDigitNegative = orgGlobalizationDetail.MSystemDigitNegative,
				MBeginDate = ctx.MBeginDate
			};
			string mCurrencyID = new REGCurrencyRepository().GetBase(ctx, false, null, null).MCurrencyID;
			bASGlobalClientModel.BaseCurrencyID = (string.IsNullOrEmpty(mCurrencyID) ? string.Empty : mCurrencyID);
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			string text = javaScriptSerializer.Serialize(bASGlobalClientModel);
			_clientGlobal.TryAdd(ctx.MOrgID, text);
			return text;
		}

		public static void ClearCache(MContext context)
		{
			lock (_lock)
			{
				if (_clientGlobal.Keys != null && _clientGlobal.Keys.Contains(context.MOrgID))
				{
					string empty = string.Empty;
					_clientGlobal.TryRemove(context.MOrgID, out empty);
				}
			}
		}
	}
}
