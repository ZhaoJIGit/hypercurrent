using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.DataRepository.Log.TableLog;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.FP
{
	public class FPFapiaoBusiness : IFPFapiaoBusiness, IDataContract<FPFapiaoModel>, IBasicBusiness<FPFapiaoModel>
	{
		private readonly FPFapiaoRepository dal = new FPFapiaoRepository();

		private readonly FPLogRepository dalLog = new FPLogRepository();

		private readonly FPImportRepository dalImport = new FPImportRepository();

		private readonly BDContactsRepository dalContact = new BDContactsRepository();

		private readonly BDItemRepository dalItem = new BDItemRepository();

		private readonly REGTaxRateBusiness bizTaxRate = new REGTaxRateBusiness();

		private readonly GLVoucherRepository dalVoucher = new GLVoucherRepository();

		private readonly GLUtility utility = new GLUtility();

		public OperationResult SaveFapiao(MContext ctx, List<FPFapiaoModel> fapiaos)
		{
			return dal.InsertOrUpdateModels(ctx, fapiaos, "");
		}

		public List<FPFapiaoModel> GetFapiaoByIds(MContext ctx, List<string> fapiaoIds, bool setDefault = true, string contactID = null)
		{
			return dal.GetFapiaoByIds(ctx, fapiaoIds, setDefault, contactID);
		}

		public FPFapiaoModel SaveFapiao(MContext ctx, FPFapiaoModel fapiao)
		{
			return dal.SaveFapiao(ctx, fapiao);
		}

		public OperationResult DeleteFapiaoByFapiaoIds(MContext ctx, FPFapiaoFilterModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> mFapiaoIDs = model.MFapiaoIDs;
			if (!FlagIdsNotEmpty(mFapiaoIDs))
			{
				string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "PleaseSelectOneOrMoreItems", "请选择一个或多个项目！");
				operationResult.Success = false;
				return operationResult;
			}
			mFapiaoIDs = mFapiaoIDs.Distinct().ToList();
			int count = mFapiaoIDs.Count;
			List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, mFapiaoIDs, false, null);
			if (count != fapiaoByIds.Count())
			{
				string text4 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistFapionData", "操作的数据已被删除或者不存在，请当前页面再操作！");
				operationResult.Success = false;
				return operationResult;
			}
			if (fapiaoByIds.Any((FPFapiaoModel a) => a.MSource == 1))
			{
				string text6 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFapiaoIsExistAutoData", "存在自动获取的发票，不能删除！");
				operationResult.Success = false;
				return operationResult;
			}
			if (ctx.MOrgVersionID == 0)
			{
				if (fapiaoByIds.Any((FPFapiaoModel a) => a.MReconcileStatus == 1))
				{
					string text8 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFapiaoIsExistCheckData", "存在已勾对的发票，不能删除！");
					operationResult.Success = false;
					return operationResult;
				}
			}
			else if (fapiaoByIds.Any((FPFapiaoModel a) => a.MCodingStatus == 1))
			{
				string text10 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFapiaoIsExistVoucherData", "存在已生成凭证的发票，不能删除！");
				operationResult.Success = false;
				return operationResult;
			}
			return dal.DeleteFapiaoByFapiaoIds(ctx, mFapiaoIDs);
		}

		public OperationResult BatchUpdateFPStatusByIds(MContext ctx, FPFapiaoFilterModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> mFapiaoIDs = model.MFapiaoIDs;
			if (!FlagIdsNotEmpty(mFapiaoIDs))
			{
				string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "PleaseSelectOneOrMoreItems", "请选择一个或多个项目！");
				operationResult.Success = false;
				return operationResult;
			}
			mFapiaoIDs = mFapiaoIDs.Distinct().ToList();
			int count = mFapiaoIDs.Count;
			int num = Convert.ToInt32(model.MStatus);
			if (num != 1 && num != 0 && num != 2 && num != 3 && num != 4)
			{
				string text4 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "BatchUpdateFPStatusByIdsStatusError", "修改为的状态必须是正常/作废/失控/异常其中的一种！");
				operationResult.Success = false;
				return operationResult;
			}
			List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, mFapiaoIDs, false, null);
			if (count != fapiaoByIds.Count())
			{
				string text6 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistFapionData", "操作的数据已被删除或者不存在，请当前页面再操作！");
				operationResult.Success = false;
				return operationResult;
			}
			if (fapiaoByIds.Any((FPFapiaoModel a) => a.MSource == 1))
			{
				string text8 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistAutoData", "存在自动获取的发票，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			if (ctx.MOrgVersionID == 0)
			{
				string confirmPara = model.ConfirmPara;
				if (fapiaoByIds.Any((FPFapiaoModel a) => a.MReconcileStatus == 1) && (string.IsNullOrEmpty(confirmPara) || confirmPara.Trim().ToLower() != "sureeditobsolete") && num == 0)
				{
					string text10 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoSureEditObsolete", "存在已勾对发票，将发票状态修改为作废会导致发票与开票单的勾对关系自动解除，请确认是否继续？");
					operationResult.ObjectID = "SureEditObsolete";
					return operationResult;
				}
			}
			else if (fapiaoByIds.Any((FPFapiaoModel a) => a.MCodingStatus == 1))
			{
				string text12 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistVoucherData", "存在已生成凭证的发票，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			switch (num)
			{
			case 1:
				if (fapiaoByIds.Any((FPFapiaoModel a) => a.MStatus == 4))
				{
					string text16 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistRedData", "存在红冲状态的发票，不能修改为正常状态！");
					operationResult.Success = false;
					return operationResult;
				}
				if (fapiaoByIds.Any((FPFapiaoModel a) => a.MTotalAmount < decimal.Zero))
				{
					string text18 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistNegativeData", "存在负数发票，不能修改为正常状态！");
					operationResult.Success = false;
					return operationResult;
				}
				goto default;
			case 4:
				if (fapiaoByIds.Any((FPFapiaoModel a) => a.MTotalAmount >= decimal.Zero))
				{
					string text14 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFPAmountPositiveData", "存在非负数发票，不能修改为红冲状态！");
					operationResult.Success = false;
					return operationResult;
				}
				goto default;
			default:
				return dal.BatchUpdateFPStatusByIds(ctx, mFapiaoIDs, num);
			}
		}

		public OperationResult BatchUpdateFPVerifyType(MContext ctx, List<FPFapiaoModel> modelList)
		{
			OperationResult operationResult = new OperationResult();
			if (modelList == null || !modelList.Any())
			{
				string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "PleaseSelectOneOrMoreItems", "请选择一个或多个项目！");
				operationResult.Success = false;
				return operationResult;
			}
			modelList = modelList.Distinct().ToList();
			if (modelList.Any(delegate(FPFapiaoModel a)
			{
				bool result2 = false;
				int mVerifyType2 = a.MVerifyType;
				if (mVerifyType2 != 2 && mVerifyType2 != 1 && mVerifyType2 != 0)
				{
					result2 = true;
				}
				return result2;
			}))
			{
				string text4 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagVerifyStatus", "存在认证状态不是勾选认证、扫描认证、未认证其中的一项，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			List<string> list = (from a in modelList
			select a.MID).Distinct().ToList();
			List<FPFapiaoModel> fapiaos = GetFapiaoByIds(ctx, list, false, null);
			if (list.Count() != fapiaos.Count())
			{
				string text6 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistFapionData", "操作的数据已被删除或者不存在，请当前页面再操作！");
				operationResult.Success = false;
				return operationResult;
			}
			if (fapiaos.Any((FPFapiaoModel a) => a.MSource == 1))
			{
				string text8 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistAutoData", "存在自动获取的发票，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			if (fapiaos.Any((FPFapiaoModel a) => a.MType != 1 || a.MInvoiceType != 1))
			{
				string text10 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagCommonFapiaoType", "存在增值税普票，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			if (fapiaos.Any((FPFapiaoModel a) => a.MStatus != 1 && a.MStatus != 4))
			{
				string text12 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagNotNormalData", "存在发票状态不是正常状态的发票，不能修改！");
				operationResult.Success = false;
				return operationResult;
			}
			if (modelList.Any(delegate(FPFapiaoModel a)
			{
				bool result = false;
				int mVerifyType = a.MVerifyType;
				if (mVerifyType == 2 || mVerifyType == 1)
				{
					DateTime t = a.MVerifyDate;
					if (t <= DateTime.MinValue)
					{
						result = true;
					}
					else
					{
						FPFapiaoModel fPFapiaoModel = fapiaos.FirstOrDefault((FPFapiaoModel b) => b.MID == a.MID);
						if (fPFapiaoModel != null)
						{
							DateTime t2 = fPFapiaoModel.MBizDate;
							t2 = Convert.ToDateTime(t2.ToString("yyyy-MM-01"));
							t = Convert.ToDateTime(t.ToString("yyyy-MM-01"));
							if (t < t2)
							{
								result = true;
							}
						}
						else
						{
							result = true;
						}
					}
				}
				return result;
			}))
			{
				string text14 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFapiaoVerifyDate", "认证方式为扫描认证或勾选认证时，认证年月必填且晚于或等于开票日期！");
				operationResult.Success = false;
				return operationResult;
			}
			return dal.BatchUpdateFPVerifyType(ctx, modelList);
		}

		public OperationResult DeleteFPImportByIds(MContext ctx, FPFapiaoFilterModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> idList = model.IdList;
			if (!FlagIdsNotEmpty(idList))
			{
				string text2 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagSelectedData", "没有操作的数据！");
				operationResult.Success = false;
				return operationResult;
			}
			idList = idList.Distinct().ToList();
			int count = idList.Count;
			List<FPImpportModel> fPImportDataByImportIds = dalImport.GetFPImportDataByImportIds(ctx, idList);
			if (count != fPImportDataByImportIds.Count())
			{
				string text4 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagExistFapionData", "操作的数据已被删除或者不存在，请当前页面再操作！");
				operationResult.Success = false;
				return operationResult;
			}
			if (dalImport.FlagExistAutoData(ctx, idList))
			{
				string text6 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFPImportIsExistAutoData", "删除的发票清单下存在自动获取的发票，不能删除！");
				operationResult.Success = false;
				return operationResult;
			}
			if (ctx.MOrgVersionID == 0)
			{
				if (dalImport.FlagExisCheckData(ctx, idList))
				{
					string text8 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFPImportIsExistCheckData", "删除的发票清单下存在已勾对的发票，不能删除！");
					operationResult.Success = false;
					return operationResult;
				}
			}
			else if (dalImport.FlagExisVoucherData(ctx, idList))
			{
				string text10 = operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FlagFPImportIsExistVoucherData", "删除的发票清单下存在生成凭证的发票，不能删除！");
				operationResult.Success = false;
				return operationResult;
			}
			return dalImport.DeleteFPImportByIds(ctx, idList);
		}

		private bool FlagIdsNotEmpty(List<string> idList)
		{
			bool result = false;
			if (idList?.Any() ?? false)
			{
				result = idList.Any((string a) => !string.IsNullOrEmpty(a));
			}
			return result;
		}

		public FPFapiaoModel GetFapiaoModel(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dal.GetDataModel(ctx, filter.MFapiaoIDs[0], false);
		}

		public DataGridJson<FPFapiaoReconcileModel> GetReconcileList(MContext ctx, FPFapiaoFilterModel filter)
		{
			DataGridJson<FPFapiaoReconcileModel> dataGridJson = new DataGridJson<FPFapiaoReconcileModel>
			{
				total = 0,
				rows = new List<FPFapiaoReconcileModel>()
			};
			if (!filter.MOnlyFapiao)
			{
				DataGridJson<FPTableViewModel> tableViewModelGrid = new FPTableBusiness().GetTableViewModelGrid(ctx, new FPTableViewFilterModel
				{
					MStartDate = filter.MStartDate,
					MEndDate = filter.MEndDate,
					page = filter.page,
					rows = filter.rows,
					MItemID = filter.MTableID,
					MInvoiceType = filter.MFapiaoCategory.ToString(),
					MIssueStatus = 0 + "," + 1
				});
				if (tableViewModelGrid.total > 0)
				{
					filter.MTableID = string.Join(",", from x in tableViewModelGrid.rows
					select x.MItemID);
				}
			}
			if (!string.IsNullOrWhiteSpace(filter.MTableID))
			{
				dataGridJson.rows = (from x in dal.GetFapiaoReconcileList(ctx, filter)
				orderby int.Parse(x.MTable.MNumber) descending
				select x).ToList();
				if (filter.MFindFapiao)
				{
					for (int i = 0; i < dataGridJson.rows.Count; i++)
					{
						dataGridJson.rows[i].MReconciledFapiaoList = ((dataGridJson.rows[i].MTable.MIssueStatus == 0) ? new List<FPFapiaoModel>() : GetReconciledFapiaoList(ctx, dataGridJson.rows[i].MTable.MItemID));
					}
				}
				dataGridJson.total = dataGridJson.rows.Count;
			}
			dataGridJson.rows.Sort();
			return dataGridJson;
		}

		public List<FPFapiaoModel> GetReconciledFapiaoList(MContext ctx, string tableID)
		{
			return dal.GetReconciledFapiaoList(ctx, tableID);
		}

		public DataGridJson<FPImpportModel> GetStatementList(MContext ctx, FPFapiaoFilterModel filter)
		{
			int importListCountByFilter = dalImport.GetImportListCountByFilter(ctx, filter);
			List<FPImpportModel> rows = new List<FPImpportModel>();
			if (importListCountByFilter > 0)
			{
				rows = dalImport.GetImportListByFilter(ctx, filter);
			}
			return new DataGridJson<FPImpportModel>
			{
				total = importListCountByFilter,
				rows = rows
			};
		}

		public DataGridJson<FPFapiaoModel> GetTransactionList(MContext ctx, FPFapiaoFilterModel filter)
		{
			return GetFapiaoPageList(ctx, filter);
		}

		public List<FPFapiaoModel> GetFapiaoList(MContext ctx, FPFapiaoFilterModel filter)
		{
			return new List<FPFapiaoModel>();
		}

		public void ValidateFapiaoList(MContext ctx, List<FPFapiaoModel> fapiaoList)
		{
		}

		public DataGridJson<FPFapiaoModel> GetFapiaoPageList(MContext ctx, FPFapiaoFilterModel filter)
		{
			int fapiaoListCountByFilter = dal.GetFapiaoListCountByFilter(ctx, filter);
			List<FPFapiaoModel> fapiaoListByFilter = dal.GetFapiaoListByFilter(ctx, filter);
			return new DataGridJson<FPFapiaoModel>
			{
				total = fapiaoListCountByFilter,
				rows = fapiaoListByFilter
			};
		}

		public List<FPFapiaoModel> GetFapiaoListIncludeEntry(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dal.GetFapiaoListIncludeEntry(ctx, filter);
		}

		public List<FPFapiaoModel> GetFapiaoListByFilter(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dal.GetFapiaoListByFilter(ctx, filter);
		}

		public OperationResult SaveReconcile(MContext ctx, FPFapiaoReconcileModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			FPTableRepository fPTableRepository = new FPTableRepository();
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<FPFapiaoModel> fapiaos = GetFapiaoListByFilter(ctx, new FPFapiaoFilterModel
			{
				MFapiaoIDs = (from x in model.MFapiaoList
				select x.MID).ToList()
			});
			FPTableViewModel table = new FPTableRepository().GetViewModelByTableID(ctx, model.MTable.MItemID);
			table.MAjustAmount = model.MTable.MAjustAmount;
			List<FPFapiaoModel> reconciledFapiaos = GetReconciledFapiaoList(ctx, model.MTable.MItemID);
			if (reconciledFapiaos != null && reconciledFapiaos.Any())
			{
				List<FPFapiaoModel> list3 = (from x in reconciledFapiaos
				where !(from y in fapiaos
				select y.MID).Contains(x.MID)
				select x).ToList();
				if (list3.Any())
				{
					list2.AddRange(FPTableLogHelper.GetRemoveReoncileLogCmd(ctx, new FPFapiaoReconcileModel
					{
						MTable = table,
						MFapiaoList = list3
					}));
				}
			}
			reconciledFapiaos = (reconciledFapiaos ?? new List<FPFapiaoModel>());
			List<FPFapiaoModel> list4 = (from x in fapiaos
			where !(from y in reconciledFapiaos
			select y.MID).Contains(x.MID)
			select x).ToList();
			if (list4.Any())
			{
				list2.AddRange(FPTableLogHelper.GetManualReconcileLogCmd(ctx, new FPFapiaoReconcileModel
				{
					MTable = table,
					MFapiaoList = list4
				}));
			}
			ValidateReconcile(ctx, table, fapiaos);
			List<CommandInfo> deleteReconcileCmds = fPTableRepository.GetDeleteReconcileCmds(ctx, null, new List<string>
			{
				model.MTable.MItemID
			});
			fPTableRepository.UpdateTableIssueStatus(table, fapiaos);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, new FPTableModel
			{
				MItemID = table.MItemID,
				MIssueStatus = table.MIssueStatus,
				MRAmount = table.MRAmount,
				MRTaxAmount = table.MRTaxAmount,
				MRTotalAmount = table.MRTotalAmount,
				MAjustAmount = table.MAjustAmount
			}, new List<string>
			{
				"MIssueStatus",
				"MAjustAmount",
				"MRAmount",
				"MRTaxAmount",
				"MRTotalAmount"
			}, true);
			fapiaos.ForEach(delegate(FPFapiaoModel x)
			{
				x.MReconcileStatus = 1;
			});
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, fapiaos, new List<string>
			{
				"MReconcileStatus"
			}, true);
			List<CommandInfo> updateReconcileCmds = new List<CommandInfo>();
			model.MFapiaoList.ForEach(delegate(FPFapiaoModel x)
			{
				List<CommandInfo> insertOrUpdateCmd2 = ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoTableModel>(ctx, new FPFapiaoTableModel
				{
					MTableID = table.MItemID,
					MFapiaoID = x.MID,
					MFapiaoType = table.MInvoiceType
				}, null, true);
				updateReconcileCmds.AddRange(insertOrUpdateCmd2);
			});
			list.AddRange(deleteReconcileCmds);
			list.AddRange(insertOrUpdateCmd);
			list.AddRange(insertOrUpdateCmds);
			list.AddRange(updateReconcileCmds);
			list.AddRange(list2);
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public OperationResult RemoveReconcile(MContext ctx, FPFapiaoReconcileModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			FPTableRepository fPTableRepository = new FPTableRepository();
			List<string> removeFapiaoIDs = (from x in model.MFapiaoList
			select x.MID).ToList();
			FPTableViewModel viewModelByTableID = new FPTableRepository().GetViewModelByTableID(ctx, model.MTable.MItemID);
			List<FPFapiaoModel> fapiaoListDataByTableIds = dal.GetFapiaoListDataByTableIds(ctx, new List<string>
			{
				viewModelByTableID.MItemID
			});
			List<FPFapiaoModel> fapiaos = (from x in fapiaoListDataByTableIds
			where !removeFapiaoIDs.Contains(x.MID)
			select x).ToList();
			List<FPFapiaoModel> list2 = (from x in fapiaoListDataByTableIds
			where removeFapiaoIDs.Contains(x.MID)
			select x).ToList();
			List<CommandInfo> deleteReconcileCmds = fPTableRepository.GetDeleteReconcileCmds(ctx, removeFapiaoIDs, null);
			fPTableRepository.UpdateTableIssueStatus(viewModelByTableID, fapiaos);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, new FPTableModel
			{
				MItemID = viewModelByTableID.MItemID,
				MIssueStatus = viewModelByTableID.MIssueStatus,
				MRTotalAmount = viewModelByTableID.MRTotalAmount,
				MRTaxAmount = viewModelByTableID.MRTaxAmount,
				MRAmount = viewModelByTableID.MRAmount,
				MAjustAmount = viewModelByTableID.MAjustAmount
			}, new List<string>
			{
				"MIssueStatus",
				"MAjustAmount",
				"MRAmount",
				"MRTaxAmount",
				"MRTotalAmount"
			}, true);
			list2.ForEach(delegate(FPFapiaoModel x)
			{
				x.MReconcileStatus = 0;
			});
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, new List<string>
			{
				"MReconcileStatus"
			}, true);
			List<CommandInfo> removeReoncileLogCmd = FPTableLogHelper.GetRemoveReoncileLogCmd(ctx, new FPFapiaoReconcileModel
			{
				MTable = viewModelByTableID,
				MFapiaoList = list2
			});
			list.AddRange(deleteReconcileCmds);
			list.AddRange(insertOrUpdateCmd);
			list.AddRange(insertOrUpdateCmds);
			list.AddRange(removeReoncileLogCmd);
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public void ValidateReconcile(MContext ctx, FPTableModel table, List<FPFapiaoModel> fapiaos)
		{
			new GLUtility().ValidateFapiaoReconcile(ctx, table, fapiaos);
		}

		public OperationResult SetReconcileStatus(MContext ctx, FPFapiaoFilterModel filter)
		{
			new GLUtility().ValidateSetFapiaoNoReconcieStatus(ctx, filter.MFapiaoIDs);
			List<FPFapiaoModel> fapiaos = new List<FPFapiaoModel>();
			filter.MFapiaoIDs.ForEach(delegate(string x)
			{
				fapiaos.Add(new FPFapiaoModel
				{
					MID = x,
					MReconcileStatus = int.Parse(filter.MReconcileStatus)
				});
			});
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, fapiaos, new List<string>
			{
				"MReconcileStatus"
			}, true);
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmds);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public DataGridJson<FPLogModel> GetFapiaoLogList(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dalLog.GetPageList(ctx, filter);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FPFapiaoModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FPFapiaoModel> modelData, string fields = null)
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

		public FPFapiaoModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FPFapiaoModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FPFapiaoModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FPFapiaoModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public FPFapiaoModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<FPFapiaoModel> Get(MContext ctx, GetParam param)
		{
			return dal.Get(ctx, param);
		}

		public List<FPFapiaoModel> Post(MContext ctx, PostParam<FPFapiaoModel> param)
		{
			List<FPFapiaoModel> dataList = param.DataList;
			dataList = (from f in dataList
			where f.MBizDate >= ctx.MGLBeginDate
			select f).ToList();
			if (!dataList.Any())
			{
				return new List<FPFapiaoModel>();
			}
			List<CommandInfo> list = new List<CommandInfo>();
			SetFapiaoBasicDataID(ctx, dataList, list);
			OperationResult operationResult = new OperationResult();
			dal.ValidateFapiaoList(ctx, dataList);
			if (dataList.Exists((FPFapiaoModel x) => x.ValidationErrors.Count > 0))
			{
				return dataList;
			}
			dataList = (from f in dataList
			where string.IsNullOrWhiteSpace(f.MID) || (!string.IsNullOrWhiteSpace(f.MID) && (f.MChangeToObsolete || f.MIsVerifyTypeChanged))
			select f).ToList();
			if (!dataList.Any())
			{
				return new List<FPFapiaoModel>();
			}
			foreach (FPFapiaoModel item in dataList)
			{
				List<string> fields = null;
				if (!string.IsNullOrWhiteSpace(item.MID))
				{
					item.MFapiaoEntrys = null;
					fields = new List<string>
					{
						"MStatus",
						"MVerifyType",
						"MVerifyDate",
						"MSource"
					};
				}
				DateTime mBizDate = item.MBizDate;
				item.MBizDate = new DateTime(mBizDate.Year, mBizDate.Month, mBizDate.Day);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoModel>(ctx, item, fields, true));
			}
			List<string> fapiaoIDs = (from x in dataList
			where !string.IsNullOrWhiteSpace(x.MID) && x.MChangeToObsolete
			select x.MID).ToList();
			list.AddRange(dal.GetDeleteFapiaoTableReconcileCmds(ctx, fapiaoIDs));
			if (dataList.Any((FPFapiaoModel f) => f.ValidationErrors.Any()))
			{
				return dataList;
			}
			if (list.Any())
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
				if (!operationResult.Success)
				{
					throw new Exception(operationResult.Message);
				}
			}
			return dataList;
		}

		public FPCodingPageModel GetCodingPageList(MContext ctx, FPFapiaoFilterModel filter)
		{
			return dal.GetCodingPageList(ctx, filter);
		}

		public OperationResult SaveCodingStatus(MContext ctx, FPFapiaoFilterModel filter)
		{
			if (filter.MFapiaoIDs == null || filter.MFapiaoIDs.Count == 0)
			{
				return new OperationResult
				{
					Success = true
				};
			}
			new GLUtility().ValidateSetFapiaoNoCreateVoucherStatus(ctx, filter.MFapiaoIDs);
			List<FPFapiaoModel> list = new List<FPFapiaoModel>();
			List<MySqlParameter> list2 = ctx.GetParameters("@MCodingStatus", int.Parse(filter.MCodingStatus)).ToList();
			string commandText = " update t_fp_fapiao set MCodingStatus = @MCodingStatus where MOrgID = @MOrgID and MIsDelete = 0 and MID " + GLUtility.GetInFilterQuery(filter.MFapiaoIDs, ref list2, "M_ID");
			List<CommandInfo> list3 = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list3.Add(obj);
			List<CommandInfo> cmdList = list3;
			int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(cmdList);
			return new OperationResult
			{
				Success = (num > 0)
			};
		}

		public OperationResult SaveCoding(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<FPCodingModel> mCodings = filter.MCodings;
			List<string> fapiaoIds = (from x in mCodings
			select x.MID).Distinct().ToList();
			OperationResult operationResult = CheckHasFapiaoCreatedFapiao(ctx, fapiaoIds);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<CommandInfo> list = PrehandleFapiaoCoding(ctx, mCodings, filter.MFapiaoCategory);
			ValidateCoding(ctx, mCodings, filter.MFapiaoCategory);
			List<DateTime> list2 = new List<DateTime>();
			List<FPCodingModel> list3 = (from x in filter.MCodings
			where x.MStatus == 4
			select x).ToList();
			List<FPCodingModel> list4 = (from x in filter.MCodings
			where x.MStatus != 4
			select x).ToList();
			List<CommandInfo> list5 = new List<CommandInfo>();
			if (list3.Any())
			{
				List<CommandInfo> collection = (filter.MSaveType == 0) ? SaveCodingMerge(ctx, list3, filter.MFapiaoCategory, true, ref list2) : SaveCodingNoMerge(ctx, list3, filter.MFapiaoCategory, true, ref list2);
				list5.AddRange(collection);
			}
			if (list4.Any())
			{
				List<CommandInfo> collection2 = (filter.MSaveType == 0) ? SaveCodingMerge(ctx, list4, filter.MFapiaoCategory, false, ref list2) : SaveCodingNoMerge(ctx, list4, filter.MFapiaoCategory, false, ref list2);
				list5.AddRange(collection2);
			}
			list.AddRange(list5);
			operationResult = CheckHasFapiaoCreatedFapiao(ctx, fapiaoIds);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0);
			return operationResult;
		}

		public OperationResult ResetCodingData(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<CommandInfo> deleteFapaioCodingDateCmds = dal.GetDeleteFapaioCodingDateCmds(ctx, filter.MFapiaoIDs);
			new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteFapaioCodingDateCmds);
			return new OperationResult
			{
				Success = true
			};
		}

		public OperationResult SaveCodingRow(MContext ctx, List<FPCodingModel> rows)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			for (int i = 0; i < rows.Count; i++)
			{
				FPCodingModel fPCodingModel = rows[i];
				if (string.IsNullOrWhiteSpace(fPCodingModel.MItemID) && fPCodingModel.MIndex != 0)
				{
					List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
					list2.Add(new MySqlParameter("@MID", fPCodingModel.MID));
					list2.Add(new MySqlParameter("@MIndex", fPCodingModel.MIndex));
					string text = " update t_fp_coding t set t.MIndex = t.MIndex + 1 where MOrgID = @MOrgID and MIsDelete = 0 and MID = @MID and MIndex >= @MIndex ";
					if (!string.IsNullOrWhiteSpace(fPCodingModel.MEntryID))
					{
						text += " and MEntryID = @MEntryID";
						list2.Add(new MySqlParameter("@MEntryID", fPCodingModel.MEntryID));
					}
					List<CommandInfo> list3 = list;
					CommandInfo commandInfo = new CommandInfo();
					DbParameter[] array = commandInfo.Parameters = list2.ToArray();
					commandInfo.CommandText = text;
					list3.Add(commandInfo);
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, rows, null, true));
			bool success = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0;
			return new OperationResult
			{
				Success = success,
				ObjectID = string.Join(",", (from x in rows
				select x.MItemID).ToList())
			};
		}

		public OperationResult DeleteCodingRow(MContext ctx, FPCodingModel row)
		{
			List<CommandInfo> deleteCmd = ModelInfoManager.GetDeleteCmd<FPCodingModel>(ctx, row.MItemID);
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.Add(new MySqlParameter("@MID", row.MID));
			list.Add(new MySqlParameter("@MIndex", row.MIndex));
			list.Add(new MySqlParameter("@MTotalAmount", row.MTotalAmount));
			list.Add(new MySqlParameter("@MTaxAmount", row.MTaxAmount));
			list.Add(new MySqlParameter("@MAmount", row.MAmount));
			string text = " update t_fp_coding t set t.MIndex = t.MIndex - 1 where MOrgID = @MOrgID and MIsDelete = 0 and MID = @MID and MIndex > @MIndex ";
			if (string.IsNullOrWhiteSpace(row.MEntryID))
			{
				text += " and MEntryID = @MEntryID ";
				list.Add(new MySqlParameter("@MEntryID", row.MEntryID));
			}
			List<CommandInfo> list2 = deleteCmd;
			CommandInfo commandInfo = new CommandInfo();
			DbParameter[] array = commandInfo.Parameters = list.ToArray();
			commandInfo.CommandText = text;
			list2.Add(commandInfo);
			bool success = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteCmd) > 0;
			return new OperationResult
			{
				Success = success
			};
		}

		public void ValidateCoding(MContext ctx, List<FPCodingModel> fapiaos, int category)
		{
			new GLUtility().ValidateFapiaoCoding(ctx, fapiaos, category);
		}

		public OperationResult CheckHasFapiaoCreatedFapiao(MContext ctx, List<string> fapiaoIds)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (fapiaoIds == null || !fapiaoIds.Any())
			{
				return operationResult;
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "SELECT \r\n                                t1.MNumber\r\n                            FROM\r\n                                t_fp_fapiao t1\r\n                                    INNER JOIN\r\n                                t_fp_fapiao_voucher t2 ON t1.MID = t2.MFapiaoID\r\n                                    AND t1.MOrgId = t2.MOrgId\r\n                                    AND t1.MisDelete = t2.MIsDelete\r\n                                    INNER JOIN\r\n                                t_gl_voucher t3 ON t3.MOrgID = t1.MOrgID\r\n                                    AND t3.MItemID = t2.MVoucherID\r\n                                    AND t3.MIsDelete = t1.MIsDelete\r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 and t1.MID " + GLUtility.GetInFilterQuery(fapiaoIds.Distinct().ToList(), ref list, "M_ID") + "  limit 0,4";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return operationResult;
			}
			List<string> list2 = new List<string>();
			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				if (!dataSet.Tables[0].Rows[i].IsNull("MNumber"))
				{
					list2.Add(dataSet.Tables[0].Rows[i].MField("MNumber"));
				}
			}
			list2 = (from x in list2
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (!list2.Any())
			{
				return operationResult;
			}
			operationResult.Success = false;
			operationResult.Message = "<div>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "ExistsFapiaoCreatedFapiao", "存在如下已经生成凭证的发票，请刷新发票列表重新操作:") + "<br>" + string.Join(",", list2.Take(3)) + ((list2.Count > 3) ? "..." : "") + "</div>";
			return operationResult;
		}

		public List<CommandInfo> SaveCodingMerge(MContext ctx, List<FPCodingModel> fapiaos, int categoryTye, bool isRed, ref List<DateTime> dates)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> contactIds = new List<string>();
			if (categoryTye == 1)
			{
				contactIds = (from x in fapiaos
				select x.MContactID).Distinct().ToList();
			}
			else
			{
				contactIds = fapiaos.Select(delegate(FPCodingModel x)
				{
					string mContactID2 = x.MContactID;
					DateTime mBizDate2 = x.MBizDate;
					int num2 = mBizDate2.Year * 100;
					mBizDate2 = x.MBizDate;
					return mContactID2 + ":" + (num2 + mBizDate2.Month);
				}).Distinct().ToList();
			}
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			List<DateTime> settledPeriodFromBeginDate = new GLSettlementRepository().GetSettledPeriodFromBeginDate(ctx, false, false);
			GLUtility gLUtility = new GLUtility();
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<GLCheckGroupValueModel> list3 = new List<GLCheckGroupValueModel>();
			bool flag = contactIds.Count >= 50;
			int i;
			for (i = 0; i < contactIds.Count; i++)
			{
				List<FPCodingModel> list4 = fapiaos.Where(delegate(FPCodingModel x)
				{
					object a;
					if (categoryTye != 1)
					{
						string mContactID = x.MContactID;
						DateTime mBizDate = x.MBizDate;
						int num = mBizDate.Year * 100;
						mBizDate = x.MBizDate;
						a = mContactID + ":" + (num + mBizDate.Month);
					}
					else
					{
						a = x.MContactID;
					}
					return (string)a == contactIds[i];
				}).ToList();
				GLVoucherModel fapiaoCreateVoucher = GetFapiaoCreateVoucher(ctx, list4, categoryTye, settledPeriodFromBeginDate, instance, flag, list3, isRed);
				GLVoucherRepository.ProcessVoucher(ctx, fapiaoCreateVoucher);
				fapiaoCreateVoucher.MRowIndex = i;
				list2.Add(fapiaoCreateVoucher);
				List<CommandInfo> updateFapiaoCodingStatusCmds = GetUpdateFapiaoCodingStatusCmds(ctx, (from x in list4
				select x.MID).Distinct().ToList(), fapiaoCreateVoucher.MItemID);
				dates.Add(new DateTime(fapiaoCreateVoucher.MYear, fapiaoCreateVoucher.MPeriod, 1));
				list.AddRange(updateFapiaoCodingStatusCmds);
			}
			FillVoucherNumber(ctx, list2);
			if (flag && list3.Count > 0)
			{
				List<CommandInfo> batchInsertOrUpdateCmds = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list3, null, null);
				list.AddRange(batchInsertOrUpdateCmds);
			}
			List<CommandInfo> batchInsertOrUpdateCmds2 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list2, null, null);
			list.AddRange(batchInsertOrUpdateCmds2);
			List<CommandInfo> createLogCmds = GlVoucherLogHelper.GetCreateLogCmds(ctx, list2);
			list.AddRange(createLogCmds);
			List<GLVoucherEntryModel> entrys = new List<GLVoucherEntryModel>();
			list2.ForEach(delegate(GLVoucherModel x)
			{
				entrys.AddRange(x.MVoucherEntrys);
			});
			List<CommandInfo> batchInsertOrUpdateCmds3 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, entrys, null, null);
			list.AddRange(batchInsertOrUpdateCmds3);
			return list;
		}

		public List<CommandInfo> SaveCodingNoMerge(MContext ctx, List<FPCodingModel> fapiaos, int categoryTye, bool isRed, ref List<DateTime> dates)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> fapiaoIds = (from x in fapiaos
			select x.MID).Distinct().ToList();
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			List<DateTime> settledPeriodFromBeginDate = new GLSettlementRepository().GetSettledPeriodFromBeginDate(ctx, false, false);
			GLUtility gLUtility = new GLUtility();
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<GLCheckGroupValueModel> list3 = new List<GLCheckGroupValueModel>();
			bool flag = fapiaoIds.Count >= 50;
			int i;
			for (i = 0; i < fapiaoIds.Count; i++)
			{
				List<FPCodingModel> codings = (from x in fapiaos
				where x.MID == fapiaoIds[i]
				select x).ToList();
				GLVoucherModel fapiaoCreateVoucher = GetFapiaoCreateVoucher(ctx, codings, categoryTye, settledPeriodFromBeginDate, instance, flag, list3, isRed);
				GLVoucherRepository.ProcessVoucher(ctx, fapiaoCreateVoucher);
				list2.Add(fapiaoCreateVoucher);
				List<CommandInfo> updateFapiaoCodingStatusCmds = GetUpdateFapiaoCodingStatusCmds(ctx, new List<string>
				{
					fapiaoIds[i]
				}, fapiaoCreateVoucher.MItemID);
				dates.Add(new DateTime(fapiaoCreateVoucher.MYear, fapiaoCreateVoucher.MPeriod, 1));
				list.AddRange(updateFapiaoCodingStatusCmds);
			}
			FillVoucherNumber(ctx, list2);
			if (flag && list3.Count > 0)
			{
				List<CommandInfo> batchInsertOrUpdateCmds = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list3, null, null);
				list.AddRange(batchInsertOrUpdateCmds);
			}
			List<CommandInfo> batchInsertOrUpdateCmds2 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list2, null, null);
			list.AddRange(batchInsertOrUpdateCmds2);
			List<CommandInfo> createLogCmds = GlVoucherLogHelper.GetCreateLogCmds(ctx, list2);
			list.AddRange(createLogCmds);
			List<GLVoucherEntryModel> entrys = new List<GLVoucherEntryModel>();
			list2.ForEach(delegate(GLVoucherModel x)
			{
				entrys.AddRange(x.MVoucherEntrys);
			});
			List<CommandInfo> batchInsertOrUpdateCmds3 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, entrys, null, null);
			list.AddRange(batchInsertOrUpdateCmds3);
			return list;
		}

		private void FillVoucherNumber(MContext ctx, List<GLVoucherModel> vouchers)
		{
			List<int> periods = (from x in (from x in vouchers
			select x.MYear * 100 + x.MPeriod).Distinct()
			orderby x
			select x).ToList();
			int i;
			for (i = 0; i < periods.Count; i++)
			{
				List<GLVoucherModel> list = (from x in vouchers
				where x.MYear * 100 + x.MPeriod == periods[i]
				orderby x.MRowIndex
				select x).ToList();
				List<string> nextVoucherNumbers = COMResourceHelper.GetNextVoucherNumbers(ctx, periods[i] / 100, periods[i] % 100, list.Count, null, null);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].MNumber = nextVoucherNumbers[j];
				}
			}
		}

		public List<CommandInfo> GetUpdateFapiaoCodingStatusCmds(MContext ctx, List<string> fapiaoIds, string voucherID)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			DbParameter[] array;
			for (int i = 0; i < fapiaoIds.Count; i++)
			{
				string commandText = " insert into t_fp_fapiao_voucher\r\n                    (MID, MOrgID, MFapiaoID, MVoucherID,MIsDelete,MCreatorID,MModifierID,MModifyDate,MCreateDate) \r\n                    values\r\n                    (@MID, @MOrgID, @MFapiaoID, @MVoucherID,@MIsDelete,@MCreatorID,@MModifierID,@MModifyDate,@MCreateDate)";
				List<MySqlParameter> list2 = new List<MySqlParameter>
				{
					new MySqlParameter("@MID", UUIDHelper.GetGuid()),
					new MySqlParameter("@MOrgID", ctx.MOrgID),
					new MySqlParameter("@MFapiaoID", fapiaoIds[i]),
					new MySqlParameter("@MVoucherID", voucherID),
					new MySqlParameter("@MIsDelete", false),
					new MySqlParameter("@MCreatorID", ctx.MUserID),
					new MySqlParameter("@MModifierID", ctx.MUserID),
					new MySqlParameter("@MModifyDate", ctx.DateNow),
					new MySqlParameter("@MCreateDate", ctx.DateNow)
				};
				List<CommandInfo> list3 = list;
				CommandInfo obj = new CommandInfo
				{
					CommandText = commandText
				};
				array = (obj.Parameters = list2.ToArray());
				list3.Add(obj);
			}
			CommandInfo commandInfo = new CommandInfo();
			List<MySqlParameter> list4 = ctx.GetParameters((MySqlParameter)null).ToList();
			string commandText2 = " update t_fp_fapiao set MCodingStatus = 1 where MOrgID = @MOrgID and MIsDelete = 0 and MID " + GLUtility.GetInFilterQuery(fapiaoIds, ref list4, "M_ID");
			List<CommandInfo> list5 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = commandText2
			};
			array = (obj2.Parameters = list4.ToArray());
			list5.Add(obj2);
			return list;
		}

		private GLVoucherModel GetFapiaoCreateVoucher(MContext ctx, List<FPCodingModel> codings, int categoryType, List<DateTime> closedPeriods, GLDataPool pool, bool isBigData, List<GLCheckGroupValueModel> list, bool isRed)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<BDAccountModel> accountList = pool.AccountList;
			GLUtility gLUtility = new GLUtility();
			bool isIn = categoryType == 1;
			DateTime dateTime;
			DateTime dateTime2;
			if (closedPeriods.Count != 0)
			{
				dateTime = closedPeriods.Max();
			}
			else
			{
				dateTime2 = ctx.MGLBeginDate;
				dateTime = dateTime2.AddMonths(-1);
			}
			DateTime dateTime3 = dateTime;
			DateTime dateTime4 = dateTime3.AddMonths(1);
			DateTime dateTime5 = codings.Max((FPCodingModel x) => x.MBizDate);
			DateTime mDate = dateTime5;
			if (isIn && dateTime5.Year * 100 + dateTime5.Month < dateTime4.Year * 100 + dateTime4.Month)
			{
				int num = dateTime4.Year * 100 + dateTime4.Month;
				dateTime2 = ctx.DateNow;
				int num2 = dateTime2.Year * 100;
				dateTime2 = ctx.DateNow;
				if (num < num2 + dateTime2.Month)
				{
					dateTime2 = dateTime4.AddMonths(1);
					dateTime2 = dateTime2.AddDays(-1.0);
					mDate = dateTime2.Date;
				}
				else
				{
					int num3 = dateTime4.Year * 100 + dateTime4.Month;
					dateTime2 = ctx.DateNow;
					int num4 = dateTime2.Year * 100;
					dateTime2 = ctx.DateNow;
					if (num3 == num4 + dateTime2.Month)
					{
						dateTime2 = ctx.DateNow;
						mDate = dateTime2.Date;
					}
					else
					{
						mDate = dateTime4.Date;
					}
				}
			}
			GLVoucherModel voucher = new GLVoucherModel
			{
				MYear = mDate.Year,
				MPeriod = mDate.Month,
				MDate = mDate,
				MTransferTypeID = -1
			};
			voucher.MItemID = UUIDHelper.GetGuid();
			voucher.IsNew = true;
			GLVoucherModel gLVoucherModel = voucher;
			int num5 = 2;
			gLVoucherModel.MSourceBillKey = num5.ToString();
			List<GLVoucherEntryModel> list3 = new List<GLVoucherEntryModel>();
			int i = 0;
			while (i < codings.Count)
			{
				FPCodingModel coding = codings[i];
				BDContactsModel bDContactsModel = pool.ContactList.FirstOrDefault((BDContactsModel x) => x.MItemID == codings[i].MContactID) ?? new BDContactsModel();
				BDAccountModel account = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == coding.MDebitAccount) ?? new BDAccountModel();
				GLCheckGroupValueModel value = new GLCheckGroupValueModel
				{
					MOrgID = ctx.MOrgID,
					MContactID = coding.MContactID,
					MEmployeeID = null,
					MMerItemID = coding.MMerItemID,
					MExpItemID = null,
					MPaItemID = null,
					MTrackItem1 = coding.MTrackItem1,
					MTrackItem2 = coding.MTrackItem2,
					MTrackItem3 = coding.MTrackItem3,
					MTrackItem4 = coding.MTrackItem4,
					MTrackItem5 = coding.MTrackItem5
				};
				GLVoucherEntryModel item = new GLVoucherEntryModel
				{
					MID = voucher.MItemID,
					MAccountID = coding.MDebitAccount,
					MDC = 1,
					MDebit = ((isIn && coding.MType != 0) ? coding.MAmount : coding.MTotalAmount),
					MAmount = ((isIn && coding.MType != 0) ? coding.MAmount : coding.MTotalAmount),
					MAmountFor = ((isIn && coding.MType != 0) ? coding.MAmount : coding.MTotalAmount),
					MExchangeRate = 1.0m,
					MCheckGroupValueID = (isBigData ? FilterCheckGroupValueModelByCheckGroup(ctx, value, account, pool.MCheckGroupValue, list).MItemID : gLUtility.FilterCheckGroupValueModelByCheckGroup(ctx, value, account, true).MItemID),
					MExplanation = gLUtility.GetCodingVoucherEntryExplanation(ctx, categoryType, 1, bDContactsModel.MName, coding.MFapiaoNumber, coding.MExplanation)
				};
				list3.Add(item);
				if (coding.MTaxAmount != decimal.Zero && (!isIn || coding.MType != 0))
				{
					BDAccountModel account2 = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == coding.MTaxAccount) ?? new BDAccountModel();
					GLVoucherEntryModel item2 = new GLVoucherEntryModel
					{
						MID = voucher.MItemID,
						MAccountID = coding.MTaxAccount,
						MDC = (isIn ? 1 : (-1)),
						MDebit = (isIn ? coding.MTaxAmount : decimal.Zero),
						MCredit = (isIn ? decimal.Zero : coding.MTaxAmount),
						MAmount = coding.MTaxAmount,
						MAmountFor = coding.MTaxAmount,
						MExchangeRate = 1.0m,
						MCheckGroupValueID = (isBigData ? FilterCheckGroupValueModelByCheckGroup(ctx, value, account2, pool.MCheckGroupValue, list).MItemID : gLUtility.FilterCheckGroupValueModelByCheckGroup(ctx, value, account2, true).MItemID),
						MExplanation = new GLUtility().GetCodingVoucherEntryExplanation(ctx, categoryType, isIn ? 1 : (-1), bDContactsModel.MName, coding.MFapiaoNumber, coding.MExplanation)
					};
					list3.Add(item2);
				}
				BDAccountModel account3 = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == coding.MCreditAccount) ?? new BDAccountModel();
				GLVoucherEntryModel item3 = new GLVoucherEntryModel
				{
					MID = voucher.MItemID,
					MAccountID = coding.MCreditAccount,
					MDC = -1,
					MCredit = (isIn ? coding.MTotalAmount : coding.MAmount),
					MAmount = (isIn ? coding.MTotalAmount : coding.MAmount),
					MAmountFor = (isIn ? coding.MTotalAmount : coding.MAmount),
					MExchangeRate = 1.0m,
					MCheckGroupValueID = (isBigData ? FilterCheckGroupValueModelByCheckGroup(ctx, value, account3, pool.MCheckGroupValue, list).MItemID : gLUtility.FilterCheckGroupValueModelByCheckGroup(ctx, value, account3, true).MItemID),
					MExplanation = new GLUtility().GetCodingVoucherEntryExplanation(ctx, categoryType, -1, bDContactsModel.MName, coding.MFapiaoNumber, coding.MExplanation)
				};
				list3.Add(item3);
				num5 = ++i;
			}
			if (codings.Count == 1)
			{
				voucher.MVoucherEntrys = list3;
			}
			else
			{
				BDVoucherSettingCategoryModel bDVoucherSettingCategoryModel = pool.VoucherSettingCategoryList.FirstOrDefault((BDVoucherSettingCategoryModel x) => x.MModuleID == (isIn ? 5 : 4));
				List<GLVoucherEntryModel> source = (!bDVoucherSettingCategoryModel.MSettingList.FirstOrDefault((BDVoucherSettingModel x) => x.MColumnID == BDVoucherSettingColumnEnum.EntryMergeSetting && x.MTypeID == BDVoucherSettingEumn.AccountDescCheckTypeSame).MStatus) ? (from x in list3
				group x by new
				{
					x.MAccountID,
					x.MCheckGroupValueID
				} into y
				select new GLVoucherEntryModel
				{
					MID = voucher.MItemID,
					MAccountID = y.Key.MAccountID,
					MExplanation = string.Join("·", (from m in y
					select m.MExplanation).Distinct()),
					MCheckGroupValueID = y.Key.MCheckGroupValueID,
					MDC = (((y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? 1 : (-1)),
					MExchangeRate = 1.0m,
					MCurrencyID = ctx.MBasCurrencyID,
					MDebit = (((y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? (y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) : decimal.Zero),
					MCredit = (((y.Sum((GLVoucherEntryModel m) => m.MCredit) - y.Sum((GLVoucherEntryModel n) => n.MDebit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? (y.Sum((GLVoucherEntryModel m) => m.MCredit) - y.Sum((GLVoucherEntryModel n) => n.MDebit)) : decimal.Zero)
				}).ToList() : (from x in list3
				group x by new
				{
					x.MExplanation,
					x.MAccountID,
					x.MCheckGroupValueID
				} into y
				select new GLVoucherEntryModel
				{
					MID = voucher.MItemID,
					MAccountID = y.Key.MAccountID,
					MExplanation = y.Key.MExplanation,
					MCheckGroupValueID = y.Key.MCheckGroupValueID,
					MDC = (((y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? 1 : (-1)),
					MExchangeRate = 1.0m,
					MCurrencyID = ctx.MBasCurrencyID,
					MDebit = (((y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? (y.Sum((GLVoucherEntryModel m) => m.MDebit) - y.Sum((GLVoucherEntryModel n) => n.MCredit)) : decimal.Zero),
					MCredit = (((y.Sum((GLVoucherEntryModel m) => m.MCredit) - y.Sum((GLVoucherEntryModel n) => n.MDebit)) * (isRed ? decimal.MinusOne : decimal.One) > decimal.Zero) ? (y.Sum((GLVoucherEntryModel m) => m.MCredit) - y.Sum((GLVoucherEntryModel n) => n.MDebit)) : decimal.Zero)
				}).ToList();
				source = (from x in source
				orderby x.MDC descending
				select x).ToList();
				for (int j = 0; j < source.Count; j++)
				{
					source[j].MAmount = ((source[j].MDC == 1) ? source[j].MDebit : source[j].MCredit);
					source[j].MAmountFor = source[j].MAmount;
				}
				voucher.MVoucherEntrys = source;
			}
			int entryIndex = 0;
			voucher.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
			{
				x.MEntrySeq = ++entryIndex;
			});
			voucher.MDebitTotal = voucher.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MDebit);
			voucher.MCreditTotal = voucher.MVoucherEntrys.Sum((GLVoucherEntryModel x) => x.MCredit);
			voucher.MStatus = 0;
			return voucher;
		}

		private GLCheckGroupValueModel FilterCheckGroupValueModelByCheckGroup(MContext ctx, GLCheckGroupValueModel value, BDAccountModel account, Hashtable values, List<GLCheckGroupValueModel> list)
		{
			List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>();
			GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
			{
				MOrgID = ctx.MOrgID
			};
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			gLCheckGroupValueModel.MContactID = utility.GetValueByGroupEnableStatus(value.MContactID, mCheckGroupModel.MContactID, CheckTypeEnum.MContactID, ref list2);
			gLCheckGroupValueModel.MEmployeeID = utility.GetValueByGroupEnableStatus(value.MEmployeeID, mCheckGroupModel.MEmployeeID, CheckTypeEnum.MEmployeeID, ref list2);
			gLCheckGroupValueModel.MMerItemID = utility.GetValueByGroupEnableStatus(value.MMerItemID, mCheckGroupModel.MMerItemID, CheckTypeEnum.MMerItemID, ref list2);
			gLCheckGroupValueModel.MExpItemID = utility.GetValueByGroupEnableStatus(value.MExpItemID, mCheckGroupModel.MExpItemID, CheckTypeEnum.MExpItemID, ref list2);
			gLCheckGroupValueModel.MPaItemID = utility.GetValueByGroupEnableStatus(value.MPaItemID, mCheckGroupModel.MPaItemID, CheckTypeEnum.MPaItemID, ref list2);
			gLCheckGroupValueModel.MTrackItem1 = utility.GetValueByGroupEnableStatus(value.MTrackItem1, mCheckGroupModel.MTrackItem1, CheckTypeEnum.MTrackItem1, ref list2);
			gLCheckGroupValueModel.MTrackItem2 = utility.GetValueByGroupEnableStatus(value.MTrackItem2, mCheckGroupModel.MTrackItem2, CheckTypeEnum.MTrackItem2, ref list2);
			gLCheckGroupValueModel.MTrackItem3 = utility.GetValueByGroupEnableStatus(value.MTrackItem3, mCheckGroupModel.MTrackItem3, CheckTypeEnum.MTrackItem3, ref list2);
			gLCheckGroupValueModel.MTrackItem4 = utility.GetValueByGroupEnableStatus(value.MTrackItem4, mCheckGroupModel.MTrackItem4, CheckTypeEnum.MTrackItem4, ref list2);
			gLCheckGroupValueModel.MTrackItem5 = utility.GetValueByGroupEnableStatus(value.MTrackItem5, mCheckGroupModel.MTrackItem5, CheckTypeEnum.MTrackItem5, ref list2);
			if (list2.Count > 0)
			{
				MActionException ex = new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MCheckGroupValueNotMatchWithAccount
				});
				ex.Messages = new List<string>
				{
					"<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountName", "科目名称:") + MText.Encode(account.MFullName) + "</div>"
				};
				ex.Messages.AddRange(utility.AssembleErrorMessage(ctx, list2));
				ex.Messages.Add("<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PleaseModifyYourBill", "请在单据补充相应信息或者修改科目信息!") + "</div>");
				throw ex;
			}
			int hashCode = gLCheckGroupValueModel.GetHashCode();
			object obj = values[hashCode];
			if (obj != null)
			{
				return obj as GLCheckGroupValueModel;
			}
			gLCheckGroupValueModel.MItemID = UUIDHelper.GetGuid();
			gLCheckGroupValueModel.IsNew = true;
			values[hashCode] = gLCheckGroupValueModel;
			list.Add(gLCheckGroupValueModel);
			return gLCheckGroupValueModel;
		}

		public FPCodingSettingModel GetCodingSetting(MContext ctx)
		{
			return dal.GetCodingSetting(ctx);
		}

		public OperationResult SaveCodingSetting(MContext ctx, FPCodingSettingModel model)
		{
			model.MOrgID = ctx.MOrgID;
			model.MUserID = ctx.MUserID;
			if (string.IsNullOrWhiteSpace(model.MID))
			{
				FPCodingSettingModel codingSetting = GetCodingSetting(ctx);
				model.MID = codingSetting.MID;
			}
			else
			{
				FPCodingSettingModel fPCodingSettingModel = dal.ExamineFPCodingSettingModel(ctx, model);
				if (model.MID != fPCodingSettingModel.MID)
				{
					string str = new JavaScriptSerializer().Serialize(model);
					MLogger.Log("保存发票coding设置字段显示异常值:" + str, (MContext)null);
				}
				model = dal.CompareOperateModelAndDefaultModel(model, fPCodingSettingModel);
			}
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<FPCodingSettingModel>(ctx, model, null, true);
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmd) > 0)
			};
		}

		private bool IsCurrentAccount(string code)
		{
			string[] array = new string[6]
			{
				"1122",
				"2203",
				"2202",
				"1123",
				"1221",
				"2241"
			};
			if (string.IsNullOrWhiteSpace(code))
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (code.StartsWith(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		private List<CommandInfo> PrehandleFapiaoCoding(MContext ctx, List<FPCodingModel> fapiaos, int categoryType)
		{
			bool flag = categoryType == 0;
			bool isIn = categoryType == 1;
			FPUtility fPUtility = new FPUtility();
			BDContactsRepository bDContactsRepository = new BDContactsRepository();
			List<DateTime> settledPeriodFromBeginDate = new GLSettlementBusiness().GetSettledPeriodFromBeginDate(ctx, false);
			FPBaseDataModel baseData = fPUtility.GetBaseData(ctx);
			List<BDContactsModel> mContact = baseData.MContact;
			List<BDItemModel> mMerItem = baseData.MMerItem;
			GLCheckTypeDataModel mTrackItem = baseData.MTrackItem1;
			GLCheckTypeDataModel mTrackItem2 = baseData.MTrackItem2;
			GLCheckTypeDataModel mTrackItem3 = baseData.MTrackItem3;
			GLCheckTypeDataModel mTrackItem4 = baseData.MTrackItem4;
			GLCheckTypeDataModel mTrackItem5 = baseData.MTrackItem5;
			List<BDAccountModel> mAccount = baseData.MAccount;
			List<BDContactsModel> list = new List<BDContactsModel>();
			List<BDItemModel> list2 = new List<BDItemModel>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			int i;
			for (i = 0; i < fapiaos.Count; i++)
			{
				BDContactsModel contact = mContact.FirstOrDefault((BDContactsModel x) => x.MItemID == fapiaos[i].MContactID || x.MName == fapiaos[i].MContactIDName) ?? new BDContactsModel();
				BDAccountModel bDAccountModel = string.IsNullOrWhiteSpace(fapiaos[i].MDebitAccount) ? new BDAccountModel() : (mAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == fapiaos[i].MDebitAccount) ?? new BDAccountModel());
				BDAccountModel bDAccountModel2 = string.IsNullOrWhiteSpace(fapiaos[i].MCreditAccount) ? new BDAccountModel() : (mAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == fapiaos[i].MCreditAccount) ?? new BDAccountModel());
				BDAccountModel bDAccountModel3 = string.IsNullOrWhiteSpace(fapiaos[i].MTaxAccount) ? new BDAccountModel() : (mAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == fapiaos[i].MTaxAccount) ?? new BDAccountModel());
				BDItemModel bDItemModel = mMerItem.FirstOrDefault((BDItemModel x) => x.MItemID == fapiaos[i].MMerItemID) ?? new BDItemModel();
				List<CommandInfo> insertNewTrackOptionsCmds = fPUtility.GetInsertNewTrackOptionsCmds(ctx, baseData, fapiaos[i]);
				list3.AddRange(insertNewTrackOptionsCmds);
				GLTreeModel trackItem9 = (string.IsNullOrWhiteSpace(fapiaos[i].MTrackItem1) || mTrackItem == null || mTrackItem.MDataList == null || mTrackItem.MDataList.Count == 0) ? new GLTreeModel() : (mTrackItem.MDataList.FirstOrDefault((GLTreeModel x) => x.id == fapiaos[i].MTrackItem1) ?? new GLTreeModel());
				GLTreeModel trackItem8 = (string.IsNullOrWhiteSpace(fapiaos[i].MTrackItem2) || mTrackItem2 == null || mTrackItem2.MDataList == null || mTrackItem2.MDataList.Count == 0) ? new GLTreeModel() : (mTrackItem2.MDataList.FirstOrDefault((GLTreeModel x) => x.id == fapiaos[i].MTrackItem2) ?? new GLTreeModel());
				GLTreeModel trackItem7 = (string.IsNullOrWhiteSpace(fapiaos[i].MTrackItem3) || mTrackItem3 == null || mTrackItem3.MDataList == null || mTrackItem3.MDataList.Count == 0) ? new GLTreeModel() : (mTrackItem3.MDataList.FirstOrDefault((GLTreeModel x) => x.id == fapiaos[i].MTrackItem3) ?? new GLTreeModel());
				GLTreeModel trackItem6 = (string.IsNullOrWhiteSpace(fapiaos[i].MTrackItem4) || mTrackItem4 == null || mTrackItem4.MDataList == null || mTrackItem4.MDataList.Count == 0) ? new GLTreeModel() : (mTrackItem4.MDataList.FirstOrDefault((GLTreeModel x) => x.id == fapiaos[i].MTrackItem4) ?? new GLTreeModel());
				GLTreeModel trackItem5 = (string.IsNullOrWhiteSpace(fapiaos[i].MTrackItem5) || mTrackItem5 == null || mTrackItem5.MDataList == null || mTrackItem5.MDataList.Count == 0) ? new GLTreeModel() : (mTrackItem5.MDataList.FirstOrDefault((GLTreeModel x) => x.id == fapiaos[i].MTrackItem5) ?? new GLTreeModel());
				fapiaos[i].MDebitAccount = bDAccountModel.MItemID;
				fapiaos[i].MCreditAccount = bDAccountModel2.MItemID;
				fapiaos[i].MTaxAccount = bDAccountModel3.MItemID;
				fapiaos[i].MTrackItem1 = trackItem9.id;
				fapiaos[i].MTrackItem2 = trackItem8.id;
				fapiaos[i].MTrackItem3 = trackItem7.id;
				fapiaos[i].MTrackItem4 = trackItem6.id;
				fapiaos[i].MTrackItem5 = trackItem5.id;
				string empty = string.Empty;
				string text = isIn ? bDAccountModel2.MCode : bDAccountModel.MCode;
				string text2 = IsCurrentAccount(text) ? text : null;
				if (string.IsNullOrWhiteSpace(contact.MItemID))
				{
					if (!string.IsNullOrWhiteSpace(fapiaos[i].MContactIDName))
					{
						contact = new BDContactsModel
						{
							IsNew = true,
							MItemID = fapiaos[i].MContactID,
							MName = fapiaos[i].MContactIDName,
							MultiLanguage = fPUtility.GetMultiLanguage(ctx, "MName", fapiaos[i].MContactIDName),
							MIsSupplier = isIn,
							MIsCustomer = flag,
							MCCurrentAccountCode = text2
						};
						empty = fapiaos[i].MContactIDName;
						mContact.Add(contact);
						BDContactsInfoModel bDContactsInfoModel = new BDContactsInfoModel
						{
							IsNew = contact.IsNew,
							MItemID = contact.MItemID,
							MName = contact.MName,
							MultiLanguage = contact.MultiLanguage,
							MIsSupplier = contact.MIsSupplier,
							MIsCustomer = contact.MIsCustomer,
							MIsOther = contact.MIsOther,
							MCCurrentAccountCode = contact.MCCurrentAccountCode,
							MTrackHead1 = trackItem9.parentId,
							MTrackHead2 = trackItem8.parentId,
							MTrackHead3 = trackItem7.parentId,
							MTrackHead4 = trackItem6.parentId,
							MTrackHead5 = trackItem5.parentId,
							MPurTrackEntry1 = (isIn ? trackItem9.id : null),
							MPurTrackEntry2 = (isIn ? trackItem8.id : null),
							MPurTrackEntry3 = (isIn ? trackItem7.id : null),
							MPurTrackEntry4 = (isIn ? trackItem6.id : null),
							MPurTrackEntry5 = (isIn ? trackItem5.id : null),
							MSalTrackEntry1 = (flag ? trackItem9.id : null),
							MSalTrackEntry2 = (flag ? trackItem8.id : null),
							MSalTrackEntry3 = (flag ? trackItem7.id : null),
							MSalTrackEntry4 = (flag ? trackItem6.id : null),
							MSalTrackEntry5 = (flag ? trackItem5.id : null)
						};
						new BDContactsRepository().MultiLanguageAdd(bDContactsInfoModel);
						List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDContactsInfoModel>(ctx, bDContactsInfoModel, null, true);
						List<CommandInfo> saveContactTrackCommandList = bDContactsRepository.GetSaveContactTrackCommandList(ctx, bDContactsInfoModel);
						CommandInfo addLogCommand = OptLog.GetAddLogCommand(OptLogTemplate.Contact_Created, ctx, bDContactsInfoModel.MItemID, bDContactsInfoModel.MName);
						list3.AddRange(insertOrUpdateCmd);
						list3.AddRange(saveContactTrackCommandList);
						list3.Add(addLogCommand);
						contact.IsNew = false;
						AddTrackLinkToCache(baseData.MTrackLink, bDContactsInfoModel);
					}
					else
					{
						fapiaos[i].MContactID = null;
						empty = null;
					}
				}
				else
				{
					empty = contact.MName;
					fapiaos[i].MContactID = contact.MItemID;
					List<string> list4 = new List<string>();
					if (categoryType == 1 && !contact.MIsSupplier)
					{
						contact.MIsSupplier = true;
						list4.Add("MIsSupplier");
					}
					if (flag && !contact.MIsCustomer)
					{
						contact.MIsCustomer = true;
						list4.Add("MIsCustomer");
					}
					if (string.IsNullOrWhiteSpace(contact.MCCurrentAccountCode) && !string.IsNullOrWhiteSpace(text2))
					{
						contact.MCCurrentAccountCode = text2;
						list4.Add("MCCurrentAccountCode");
					}
					if (list4.Count > 0)
					{
						list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDContactsModel>(ctx, contact, list4, true));
					}
					if (!contact.UpdatedTrackLink)
					{
						List<CommandInfo> list5 = new List<CommandInfo>();
						List<BDContactsTrackLinkModel> list6 = null;
						if (!string.IsNullOrWhiteSpace(trackItem9.id) || !string.IsNullOrWhiteSpace(trackItem8.id) || !string.IsNullOrWhiteSpace(trackItem7.id) || !string.IsNullOrWhiteSpace(trackItem6.id) || !string.IsNullOrWhiteSpace(trackItem5.id))
						{
							list6 = (from x in baseData.MTrackLink
							where x.MContactID == contact.MItemID
							select x).ToList();
						}
						if (!string.IsNullOrWhiteSpace(trackItem9.id) && (list6 == null || !list6.Exists((BDContactsTrackLinkModel x) => x.MTrackID == trackItem9.parentId && baseData.MTrackItem1 != null && IsTrackEntryExists(baseData.MTrackItem1.MDataList, isIn ? x.MPurTrackId : x.MSalTrackId))))
						{
							string purTrackId = isIn ? trackItem9.id : null;
							string salTrackId = flag ? trackItem9.id : null;
							FPUtility.ReWriteTrack(ctx, list6, list5, isIn, trackItem9.parentId, contact.MItemID, salTrackId, purTrackId);
						}
						if (!string.IsNullOrWhiteSpace(trackItem8.id) && (list6 == null || !list6.Exists((BDContactsTrackLinkModel x) => x.MTrackID == trackItem8.parentId && baseData.MTrackItem2 != null && IsTrackEntryExists(baseData.MTrackItem2.MDataList, isIn ? x.MPurTrackId : x.MSalTrackId))))
						{
							string purTrackId2 = isIn ? trackItem8.id : null;
							string salTrackId2 = flag ? trackItem8.id : null;
							FPUtility.ReWriteTrack(ctx, list6, list5, isIn, trackItem8.parentId, contact.MItemID, salTrackId2, purTrackId2);
						}
						if (!string.IsNullOrWhiteSpace(trackItem7.id) && (list6 == null || !list6.Exists((BDContactsTrackLinkModel x) => x.MTrackID == trackItem7.parentId && baseData.MTrackItem3 != null && IsTrackEntryExists(baseData.MTrackItem3.MDataList, isIn ? x.MPurTrackId : x.MSalTrackId))))
						{
							string purTrackId3 = isIn ? trackItem7.id : null;
							string salTrackId3 = flag ? trackItem7.id : null;
							FPUtility.ReWriteTrack(ctx, list6, list5, isIn, trackItem7.parentId, contact.MItemID, salTrackId3, purTrackId3);
						}
						if (!string.IsNullOrWhiteSpace(trackItem6.id) && (list6 == null || !list6.Exists((BDContactsTrackLinkModel x) => x.MTrackID == trackItem6.parentId && baseData.MTrackItem4 != null && IsTrackEntryExists(baseData.MTrackItem4.MDataList, isIn ? x.MPurTrackId : x.MSalTrackId))))
						{
							string purTrackId4 = isIn ? trackItem6.id : null;
							string salTrackId4 = flag ? trackItem6.id : null;
							FPUtility.ReWriteTrack(ctx, list6, list5, isIn, trackItem6.parentId, contact.MItemID, salTrackId4, purTrackId4);
						}
						if (!string.IsNullOrWhiteSpace(trackItem5.id) && (list6 == null || !list6.Exists((BDContactsTrackLinkModel x) => x.MTrackID == trackItem5.parentId && baseData.MTrackItem5 != null && IsTrackEntryExists(baseData.MTrackItem5.MDataList, isIn ? x.MPurTrackId : x.MSalTrackId))))
						{
							string purTrackId5 = isIn ? trackItem5.id : null;
							string salTrackId5 = flag ? trackItem5.id : null;
							FPUtility.ReWriteTrack(ctx, list6, list5, isIn, trackItem5.parentId, contact.MItemID, salTrackId5, purTrackId5);
						}
						contact.UpdatedTrackLink = true;
						list3.AddRange(list5);
					}
				}
				BDItemModel bDItemModel2 = mMerItem.FirstOrDefault((BDItemModel x) => x.MItemID == fapiaos[i].MMerItemID);
				if (bDItemModel2 == null)
				{
					if (!string.IsNullOrWhiteSpace(fapiaos[i].MMerItemIDName))
					{
						string nextItemNumber = fPUtility.GetNextItemNumber(mMerItem);
						bDItemModel = new BDItemModel
						{
							IsNew = true,
							MItemID = fapiaos[i].MMerItemID,
							MNumber = nextItemNumber,
							MultiLanguage = fPUtility.GetMultiLanguage(ctx, "MDesc", fapiaos[i].MMerItemIDName),
							MIncomeAccountCode = (isIn ? null : bDAccountModel2.MCode),
							MInventoryAccountCode = (isIn ? bDAccountModel.MCode : null),
							MIsExpenseItem = false
						};
						mMerItem.Add(bDItemModel);
						list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, bDItemModel, null, true));
					}
				}
				else
				{
					List<string> list7 = new List<string>();
					if (flag && !bDItemModel.MIsExpenseItem && string.IsNullOrWhiteSpace(bDItemModel.MIncomeAccountCode))
					{
						bDItemModel.MIncomeAccountCode = bDAccountModel2.MCode;
						list7.Add("MIncomeAccountCode");
					}
					if (isIn && bDItemModel.MIsExpenseItem && string.IsNullOrWhiteSpace(bDItemModel.MCostAccountCode))
					{
						bDItemModel.MCostAccountCode = bDAccountModel.MCode;
						list7.Add("MCostAccountCode");
					}
					if (isIn && !bDItemModel.MIsExpenseItem && string.IsNullOrWhiteSpace(bDItemModel.MInventoryAccountCode))
					{
						bDItemModel.MInventoryAccountCode = bDAccountModel.MCode;
						list7.Add("MInventoryAccountCode");
					}
					if (list7.Count > 0)
					{
						list3.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDItemModel>(ctx, bDItemModel, list7, true));
					}
				}
			}
			return list3;
		}

		private void AddTrackLinkToCache(List<BDContactsTrackLinkModel> mTrackLink, BDContactsInfoModel contactInfo)
		{
			if (!string.IsNullOrEmpty(contactInfo.MTrackHead1) && (!string.IsNullOrEmpty(contactInfo.MPurTrackEntry1) || !string.IsNullOrEmpty(contactInfo.MSalTrackEntry1)))
			{
				mTrackLink.Add(new BDContactsTrackLinkModel
				{
					MTrackID = contactInfo.MTrackHead1,
					MContactID = contactInfo.MItemID,
					MSalTrackId = contactInfo.MSalTrackEntry1,
					MPurTrackId = contactInfo.MPurTrackEntry1
				});
			}
			if (!string.IsNullOrEmpty(contactInfo.MTrackHead2) && (!string.IsNullOrEmpty(contactInfo.MPurTrackEntry2) || !string.IsNullOrEmpty(contactInfo.MSalTrackEntry2)))
			{
				mTrackLink.Add(new BDContactsTrackLinkModel
				{
					MTrackID = contactInfo.MTrackHead1,
					MContactID = contactInfo.MItemID,
					MSalTrackId = contactInfo.MSalTrackEntry2,
					MPurTrackId = contactInfo.MPurTrackEntry2
				});
			}
			if (!string.IsNullOrEmpty(contactInfo.MTrackHead3) && (!string.IsNullOrEmpty(contactInfo.MPurTrackEntry3) || !string.IsNullOrEmpty(contactInfo.MSalTrackEntry3)))
			{
				mTrackLink.Add(new BDContactsTrackLinkModel
				{
					MTrackID = contactInfo.MTrackHead1,
					MContactID = contactInfo.MItemID,
					MSalTrackId = contactInfo.MSalTrackEntry2,
					MPurTrackId = contactInfo.MPurTrackEntry2
				});
			}
			if (!string.IsNullOrEmpty(contactInfo.MTrackHead4) && (!string.IsNullOrEmpty(contactInfo.MPurTrackEntry4) || !string.IsNullOrEmpty(contactInfo.MSalTrackEntry4)))
			{
				mTrackLink.Add(new BDContactsTrackLinkModel
				{
					MTrackID = contactInfo.MTrackHead1,
					MContactID = contactInfo.MItemID,
					MSalTrackId = contactInfo.MSalTrackEntry2,
					MPurTrackId = contactInfo.MPurTrackEntry2
				});
			}
			if (!string.IsNullOrEmpty(contactInfo.MTrackHead5) && (!string.IsNullOrEmpty(contactInfo.MPurTrackEntry5) || !string.IsNullOrEmpty(contactInfo.MSalTrackEntry5)))
			{
				mTrackLink.Add(new BDContactsTrackLinkModel
				{
					MTrackID = contactInfo.MTrackHead1,
					MContactID = contactInfo.MItemID,
					MSalTrackId = contactInfo.MSalTrackEntry2,
					MPurTrackId = contactInfo.MPurTrackEntry2
				});
			}
		}

		public bool IsTrackEntryExists(List<GLTreeModel> list, string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return false;
			}
			return list.Exists((GLTreeModel x) => x.id == id);
		}

		public FPBaseDataModel GetBaseData(MContext ctx)
		{
			return new FPUtility().GetBaseData(ctx);
		}

		public void SetFapiaoBasicDataID(MContext ctx, List<FPFapiaoModel> fpList, List<CommandInfo> cmdList)
		{
			List<REGTaxRateModel> taxRateList = bizTaxRate.GetTaxRateList(ctx, false);
			List<string> list = new List<string>();
			IEnumerable<IGrouping<int, FPFapiaoModel>> enumerable = from f in fpList
			group f by f.MInvoiceType;
			foreach (IGrouping<int, FPFapiaoModel> item in enumerable)
			{
				switch (item.Key)
				{
				case 1:
					list.AddRange(from f in item.ToList()
					where !string.IsNullOrWhiteSpace(f.MSContactName)
					select f.MSContactName.Trim());
					break;
				case 0:
					list.AddRange(from f in item.ToList()
					where !string.IsNullOrWhiteSpace(f.MPContactName)
					select f.MPContactName.Trim());
					break;
				}
			}
			List<BDContactsModel> contactListByNameOrId = dalContact.GetContactListByNameOrId(ctx, list, false, true);
			List<BDItemModel> itemListIgnoreLocale = dalItem.GetItemListIgnoreLocale(ctx, true);
			List<REGTaxRateModel> list2 = new List<REGTaxRateModel>();
			foreach (FPFapiaoModel fp in fpList)
			{
				bool flag = fp.MInvoiceType == 1;
				string name = flag ? fp.MSContactName : fp.MPContactName;
				BDContactsModel bDContactsModel = contactListByNameOrId.FirstOrDefault((BDContactsModel f) => !string.IsNullOrWhiteSpace(f.MName) && f.MName.ToUpper().Trim() == name.ToUpper().Trim());
				if (bDContactsModel != null && ((flag && bDContactsModel.MIsSupplier) || (!flag && bDContactsModel.MIsCustomer)))
				{
					fp.MContactID = bDContactsModel.MItemID;
				}
				if (fp.MFapiaoEntrys != null)
				{
					foreach (FPFapiaoEntryModel mFapiaoEntry in fp.MFapiaoEntrys)
					{
						if (itemListIgnoreLocale.Any())
						{
							BDItemModel bDItemModel = itemListIgnoreLocale.FirstOrDefault((BDItemModel f) => !string.IsNullOrWhiteSpace(f.MName) && (f.MName.ToUpper().Trim() == mFapiaoEntry.MItemName.ToUpper().Trim() || (f.MNumber + " " + f.MName).ToUpper().Trim() == mFapiaoEntry.MItemName.ToUpper().Trim()));
							if (bDItemModel != null)
							{
								mFapiaoEntry.MItemID = bDItemModel.MItemID;
							}
						}
						REGTaxRateModel rEGTaxRateModel = taxRateList.FirstOrDefault((REGTaxRateModel f) => f.MEffectiveTaxRate == mFapiaoEntry.MTaxPercent);
						if (rEGTaxRateModel == null && !list2.Exists((REGTaxRateModel f) => f.MEffectiveTaxRate == mFapiaoEntry.MTaxPercent))
						{
							string name2 = mFapiaoEntry.MTaxPercent.ToString("G29") + "%";
							if (string.IsNullOrWhiteSpace(ctx.MAppID) && taxRateList.Any())
							{
								ctx.MAppID = taxRateList[0].MAppID;
							}
							REGTaxRateModel newTaxRate = bizTaxRate.GetNewTaxRate(ctx, name2, mFapiaoEntry.MTaxPercent);
							list2.Add(newTaxRate);
							cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<REGTaxRateModel>(ctx, newTaxRate, null, true));
						}
					}
				}
			}
		}
	}
}
