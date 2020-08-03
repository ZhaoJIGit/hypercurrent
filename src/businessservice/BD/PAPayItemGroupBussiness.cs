using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class PAPayItemGroupBussiness : IPAPayItemGroupBussiness, IDataContract<PAPayItemGroupModel>
	{
		private PAPayItemGroupRepository dal = new PAPayItemGroupRepository();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		private PTBaseBusiness _ptBaseBiz = new PTBaseBusiness();

		private PTSalaryListBusiness _ptSalaryListBiz = new PTSalaryListBusiness();

		private PAPayItemRepository _payItemDal = new PAPayItemRepository();

		public List<PAPayItemGroupModel> GetSalaryItemList(MContext ctx)
		{
			return PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.All);
		}

		public PAPayItemGroupModel GetSalaryGroupItemById(MContext ctx, string id)
		{
			PAPayItemGroupModel dataModel = dal.GetDataModel(ctx, id, false);
			if (dataModel != null && ctx.MRegProgress == 15)
			{
				string value = (dataModel.MAccountCode == null) ? "2211" : dataModel.MAccountCode;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MCode", value);
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() > 0)
				{
					BDAccountModel bDAccountModel = baseBDAccountList.First();
					dataModel.MAccountId = bDAccountModel.MItemID;
				}
				bool flag = IsPayItemHaveAmount(ctx, id);
				dataModel.MIsCanEdit = !flag;
			}
			return dataModel;
		}

		private bool IsPayItemHaveAmount(MContext ctx, string id)
		{
			return ModelInfoManager.ExistsByFilter<PASalaryPaymentEntryModel>(ctx, new SqlWhere().Equal("MPayItemID", id).GreaterThen("MAmount", 0));
		}

		private void setMulitLang(PAPayItemGroupModel model)
		{
			throw new NotImplementedException();
		}

		public OperationResult UpdateModel(MContext ctx, PAPayItemGroupModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> codeList = new List<string>
			{
				model.MAccountCode
			};
			if (!_accountBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				return operationResult;
			}
			if (ModelInfoManager.IsLangColumnValueExists<PAPayItemGroupModel>(ctx, "MName", model.MultiLanguage, model.MItemID, string.Empty, string.Empty, false))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PayItemExist", "同一级中存在名称相同的工资项目，请录入其他名称。")
				});
				return operationResult;
			}
			model.MOrgID = ctx.MOrgID;
			List<CommandInfo> list = new List<CommandInfo>();
			bool flag = string.IsNullOrEmpty(model.MItemID);
			if (!flag)
			{
				PAPayItemGroupModel dataModel = dal.GetDataModel(ctx, model.MItemID, false);
				dataModel.MultiLanguage = model.MultiLanguage;
				dataModel.MAccountCode = model.MAccountCode;
				if (dataModel.MItemType == 1023 || dataModel.MItemType == 1067)
				{
					List<string> list2 = new List<string>();
					if (model.MCoefficient != dataModel.MCoefficient && IsPayItemHaveAmount(ctx, model.MItemID))
					{
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangKey.DataHasReference)
						});
						return operationResult;
					}
					dataModel.MCoefficient = model.MCoefficient;
					dataModel.MItemType = ((model.MCoefficient > 0) ? 1023 : 1067);
				}
				if (ctx.MRegProgress >= 13)
				{
					list.AddRange(GetPayItemUpdateCmdList(ctx, model, dataModel));
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayItemGroupModel>(ctx, dataModel, null, true));
			}
			else
			{
				model.MItemType = ((model.MCoefficient > 0) ? 1023 : 1067);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayItemGroupModel>(ctx, model, null, true));
				list.AddRange(GetPrintSettingEntryInsertCmdList(ctx, model));
			}
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list) > 0);
			if (operationResult.Success & flag)
			{
				_ptBaseBiz.RemovePrintSettingCache(ctx, "PayRun");
			}
			return operationResult;
		}

		private List<CommandInfo> GetPayItemUpdateCmdList(MContext ctx, PAPayItemGroupModel model, PAPayItemGroupModel oldModel)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<PAPayItemModel> salaryItemList = _payItemDal.GetSalaryItemList(ctx, model.MItemID, null);
			if (salaryItemList != null && salaryItemList.Count() > 0)
			{
				List<string> list2 = new List<string>();
				list2.Add("MAccountCode");
				foreach (PAPayItemModel item in salaryItemList)
				{
					if (string.IsNullOrWhiteSpace(item.MAccountCode))
					{
						item.MAccountCode = oldModel.MAccountCode;
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayItemModel>(ctx, item, list2, true));
					}
				}
			}
			return list;
		}

		private List<CommandInfo> GetPrintSettingEntryInsertCmdList(MContext ctx, PAPayItemGroupModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<PAPrintSettingModel> list2 = _ptSalaryListBiz.GetList(ctx);
			foreach (PAPrintSettingModel item in list2)
			{
				PAPrintSettingEntryModel modelData = new PAPrintSettingEntryModel
				{
					MOrgID = ctx.MOrgID,
					MItemID = item.MItemID,
					MPayItemID = model.MItemID,
					MIsShow = true
				};
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPrintSettingEntryModel>(ctx, modelData, null, true));
			}
			return list;
		}

		public void CheckSalaryItemExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult, string fieldName = "MPaItemID")
		{
			List<PAPayItemGroupModel> allPayItemList = dal.GetAllPayItemList(ctx, false);
			foreach (T model in modelList)
			{
				CheckSalaryItemExist(ctx, model, ref allPayItemList, validationResult, fieldName, -1);
			}
		}

		public void CheckSalaryItemExist<T>(MContext ctx, T model, ref List<PAPayItemGroupModel> payItemList, List<IOValidationResultModel> validationResult, string fieldName = "MPaItemID", int rowIndex = -1)
		{
			if (payItemList == null)
			{
				payItemList = dal.GetAllPayItemList(ctx, false);
			}
			string itemName = ModelHelper.GetModelValue(model, fieldName);
			if (!string.IsNullOrWhiteSpace(itemName))
			{
				PAPayItemGroupModel pAPayItemGroupModel = payItemList.FirstOrDefault((PAPayItemGroupModel f) => !string.IsNullOrWhiteSpace(f.MName) && HttpUtility.HtmlDecode(f.MName.Trim()) == itemName);
				if (pAPayItemGroupModel != null)
				{
					ModelHelper.SetModelValue(model, fieldName, pAPayItemGroupModel.MItemID, null);
				}
				else
				{
					int rowIndex2 = 0;
					if (rowIndex != -1)
					{
						rowIndex2 = rowIndex;
					}
					else
					{
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex2);
					}
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.SalaryItem,
						FieldValue = itemName,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "SalaryItemNotFound", "The Salary Item:{0} can't be found!"),
						RowIndex = rowIndex2
					});
				}
			}
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdate(MContext ctx, PAPayItemGroupModel modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<PAPayItemGroupModel> modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			throw new NotImplementedException();
		}

		public OperationResult Delete(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			if (!ValidateDelete(ctx, param, operationResult))
			{
				return operationResult;
			}
			List<string> list = new List<string>();
			OperationResult operationResult2 = BDRepository.IsCanDelete(ctx, "PayRun", param.KeyIDs, out list);
			List<CommandInfo> list2 = new List<CommandInfo>();
			if (list.Any())
			{
				list2.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPayItemGroupModel>(ctx, list));
				List<PASalaryPaymentEntryModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentEntryModel>(ctx, new SqlWhere().In("MPayItemID", list), false, false);
				if (dataModelList.Any())
				{
					List<string> pkIDS = (from f in dataModelList
					select f.MEntryID).ToList();
					list2.AddRange(ModelInfoManager.GetDeleteFlagCmd<PASalaryPaymentEntryModel>(ctx, pkIDS));
				}
				List<PAPrintSettingEntryModel> dataModelList2 = ModelInfoManager.GetDataModelList<PAPrintSettingEntryModel>(ctx, new SqlWhere().In("MPayItemID", list), false, false);
				if (dataModelList2.Any())
				{
					List<string> pkIDS2 = (from f in dataModelList2
					select f.MEntryID).ToList();
					list2.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPrintSettingEntryModel>(ctx, pkIDS2));
				}
				operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list2) > 0);
				if (operationResult.Success)
				{
					_ptBaseBiz.RemovePrintSettingCache(ctx, "PayRun");
				}
			}
			return operationResult;
		}

		private bool ValidateDelete(MContext ctx, ParamBase param, OperationResult result)
		{
			List<PAPayItemModel> payItemList = PAPayItemRepository.GetPayItemList(ctx, null);
			int[] unSupportDelGroupTypes = new int[5]
			{
				1035,
				1055,
				2015,
				2000,
				3010
			};
			IEnumerable<PAPayItemModel> source = from f in payItemList
			where param.MKeyIDList.Contains(f.MItemID)
			select f;
			if (source.Any((PAPayItemModel f) => !f.MIsActive))
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DelDisabledPayItemMsg", "所选的工资项目中包含禁用的工资项目，请先恢复后再进行操作。");
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
			}
			if (source.Any((PAPayItemModel f) => !string.IsNullOrWhiteSpace(f.MGroupID) || unSupportDelGroupTypes.Contains(f.MItemType)))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DelSSOrHFPayItemMsg", "所选的工资项目中包含社保或公积金，不能进行删除。");
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
			}
			return result.Success;
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			throw new NotImplementedException();
		}

		public PAPayItemGroupModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public PAPayItemGroupModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public List<PAPayItemGroupModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<PAPayItemGroupModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}
	}
}
