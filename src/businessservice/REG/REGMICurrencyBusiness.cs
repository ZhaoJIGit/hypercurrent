using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.MI;
using JieNor.Megi.DataModel.MI.Log;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.MI;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.REG
{
	public class REGMICurrencyBusiness : IBasicBusiness<MICurrencyModel>
	{
		private readonly REGCurrencyRepository dal = new REGCurrencyRepository();

		private REGCurrencyRepository repository = new REGCurrencyRepository();

		private BASCurrencyRepository basDal = new BASCurrencyRepository();

		public DataGridJson<MICurrencyModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<MICurrencyModel> dataGridJson = new DataGridJson<MICurrencyModel>();
			List<MICurrencyModel> list = new List<MICurrencyModel>();
			List<BASCurrencyModel> list2 = new List<BASCurrencyModel>();
			if (param.Where == "MI.Log")
			{
				list = MigrateLogRepository.GetMigrateLogList<MICurrencyModel>(ctx, MigrateTypeEnum.Currency);
			}
			else if (param.Where == "REG")
			{
				List<REGCurrencyViewModel> currencyViewList = dal.GetCurrencyViewList(ctx, ctx.DateNow, true, null);
				foreach (REGCurrencyViewModel item in currencyViewList)
				{
					MICurrencyModel mICurrencyModel = new MICurrencyModel();
					mICurrencyModel.MCurrencyID = item.MCurrencyID;
					mICurrencyModel.MCurrencyName = $"{item.MCurrencyID} {item.MName}";
					list.Add(mICurrencyModel);
				}
			}
			else
			{
				list2 = basDal.GetList(ctx);
				foreach (BASCurrencyModel item2 in list2)
				{
					list.Add(new MICurrencyModel
					{
						MCurrencyID = item2.MItemID,
						MCurrencyName = item2.MName
					});
				}
			}
			dataGridJson.rows = list;
			dataGridJson.total = ((list != null) ? list.Count : 0);
			return dataGridJson;
		}

		public List<MICurrencyModel> Post(MContext ctx, PostParam<MICurrencyModel> param)
		{
			List<MICurrencyModel> list = (from f in param.DataList
			where !string.IsNullOrWhiteSpace(f.MMegiCode)
			select f).ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			if (!list.Any() || string.IsNullOrWhiteSpace(list[0].MMigrationID))
			{
				return new List<MICurrencyModel>();
			}
			List<CommandInfo> list3 = new List<CommandInfo>();
			GetParam getParam = new GetParam();
			getParam.Where = "MI.Log";
			List<MICurrencyModel> rows = Get(ctx, getParam).rows;
			if (rows.Any())
			{
				list3.AddRange(ModelInfoManager.GetDeleteFlagCmd<MICurrencyModel>(ctx, (from f in rows
				select f.MItemID).ToList()));
				list3.AddRange(ModelInfoManager.GetDeleteFlagCmd<REGCurrencyModel>(ctx, (from f in rows
				select f.MMegiID).ToList()));
			}
			bool flag = true;
			List<REGCurrencyModel> list4 = new List<REGCurrencyModel>();
			BASCurrencyViewModel @base = dal.GetBase(ctx, false, null, null);
			foreach (MICurrencyModel item in list)
			{
				if (item.MMegiCode != @base.MCurrencyID)
				{
					string guid = UUIDHelper.GetGuid();
					list4.Add(new REGCurrencyModel
					{
						MItemID = guid,
						IsNew = true,
						MCurrencyID = item.MMegiCode
					});
					item.MMegiID = guid;
				}
			}
			list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true));
			list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list4, null, true));
			string mMigrationID = list[0].MMigrationID;
			ctx.IsSys = true;
			MigrateConfigModel dataEditModel = ModelInfoManager.GetDataEditModel<MigrateConfigModel>(ctx, mMigrationID, false, true);
			ctx.IsSys = false;
			dataEditModel.MType = 1;
			list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<MigrateConfigModel>(ctx, dataEditModel, new List<string>
			{
				"MType"
			}, true));
			MultiDBCommand[] cmdArray = new MultiDBCommand[2]
			{
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Sys,
					CommandList = list2
				},
				new MultiDBCommand(ctx)
				{
					DBType = SysOrBas.Bas,
					CommandList = list3
				}
			};
			if (!flag || DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray))
			{
				;
			}
			return list;
		}

		public MICurrencyModel Delete(MContext ctx, DeleteParam param)
		{
			MICurrencyModel mICurrencyModel = new MICurrencyModel();
			List<MICurrencyModel> dataModelList = ModelInfoManager.GetDataModelList<MICurrencyModel>(ctx, param.ElementID);
			if (dataModelList == null || !dataModelList.Any())
			{
				return null;
			}
			List<string> list = new List<string>();
			List<string> itemIds = (from f in dataModelList
			select f.MMegiID).ToList();
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Currency", itemIds, out list);
			if (list.Count > 0)
			{
				ModelInfoManager.DeleteFlag<MICurrencyModel>(ctx, list);
			}
			else
			{
				mICurrencyModel.ValidationErrors.Add(new ValidationError
				{
					Message = operationResult.Message
				});
			}
			return mICurrencyModel;
		}
	}
}
