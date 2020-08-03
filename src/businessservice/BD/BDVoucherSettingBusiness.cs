using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDVoucherSettingBusiness : IBDVoucherSettingBusiness, IDataContract<BDVoucherSettingModel>
	{
		private BDVoucherSettingRepository dal = new BDVoucherSettingRepository();

		public List<BDVoucherSettingCategoryModel> GetVoucherSettingCategoryList(MContext ctx)
		{
			return dal.GetVoucherSettingCategoryList(ctx);
		}

		public OperationResult SaveVoucherSetting(MContext ctx, List<BDVoucherSettingModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<BDVoucherSettingModel> list2 = (from x in list
			where !string.IsNullOrWhiteSpace(x.MItemID)
			select x).ToList();
			if (!CheckIsEmptySetting(ctx, list2, ref operationResult))
			{
				return operationResult;
			}
			List<string> fields = new List<string>
			{
				"MStatus"
			};
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, fields, true);
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmds) > 0);
			return operationResult;
		}

		private bool CheckIsEmptySetting(MContext ctx, List<BDVoucherSettingModel> list, ref OperationResult ret)
		{
			GLUtility gLUtility = new GLUtility();
			List<BDVoucherSettingCategoryModel> voucherSettingCategoryList = dal.GetVoucherSettingCategoryList(ctx);
			foreach (BDVoucherSettingModel item in list)
			{
				bool flag = false;
				foreach (BDVoucherSettingCategoryModel item2 in voucherSettingCategoryList)
				{
					foreach (BDVoucherSettingModel mSetting in item2.MSettingList)
					{
						if (mSetting.MItemID == item.MItemID)
						{
							mSetting.MStatus = item.MStatus;
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			List<BDVoucherSettingCategoryModel> list2 = new List<BDVoucherSettingCategoryModel>();
			foreach (BDVoucherSettingCategoryModel item3 in voucherSettingCategoryList)
			{
				if (!item3.MSettingList.Any((BDVoucherSettingModel t) => t.MColumnID == BDVoucherSettingColumnEnum.ExplanationSetting && t.MStatus && t.MDC == 1))
				{
					list2.Add(new BDVoucherSettingCategoryModel
					{
						MModuleID = item3.MModuleID,
						MDC = 1
					});
				}
				if (!item3.MSettingList.Any((BDVoucherSettingModel t) => t.MColumnID == BDVoucherSettingColumnEnum.ExplanationSetting && t.MStatus && t.MDC == -1))
				{
					list2.Add(new BDVoucherSettingCategoryModel
					{
						MModuleID = item3.MModuleID,
						MDC = -1
					});
				}
			}
			if (list2 != null && list2.Count > 0)
			{
				ret.Success = false;
				ret.Message = "";
				foreach (BDVoucherSettingCategoryModel item4 in list2)
				{
					if (ret.Message.Length > 0)
					{
						OperationResult obj = ret;
						obj.Message += "<br>";
					}
					OperationResult obj2 = ret;
					obj2.Message += COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.GL, "ExplanationSettingNoEmpty", "{0}{1}摘要至少要选择一个项目！", gLUtility.GetModuleNameByModuleID(ctx, item4.MModuleID), (item4.MDC == 1) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Debit", "借方") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Credit", "贷方"));
				}
				return false;
			}
			return true;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDVoucherSettingModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDVoucherSettingModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public BDVoucherSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDVoucherSettingModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDVoucherSettingModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDVoucherSettingModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
