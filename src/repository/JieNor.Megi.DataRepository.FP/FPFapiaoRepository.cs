using JieNor.Megi.Common.Logger;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.DataRepository.Log.TableLog;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace JieNor.Megi.DataRepository.FP
{
	public class FPFapiaoRepository : DataServiceT<FPFapiaoModel>
	{
		public List<FPFapiaoModel> GetFapiaoListDataByTableIds(MContext ctx, List<string> tableIds)
		{
			string sql = "select t1.MFapiaoID as MID, t1.MTableID from t_fp_fapiao_table t1 where t1.MOrgID = @MOrgID and t1.MIsDelete = 0  and t1.MTableID in ('" + string.Join("','", tableIds.ToArray()) + "')";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			List<FPFapiaoModel> fapiaos = ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, cmdParms);
			if (fapiaos != null && fapiaos.Count > 0)
			{
				List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, (from x in fapiaos
				select x.MID).ToList(), false, string.Empty);
				fapiaoByIds.ForEach(delegate(FPFapiaoModel x)
				{
					x.MTableID = fapiaos.FirstOrDefault((FPFapiaoModel y) => y.MID == x.MID).MTableID;
				});
				return fapiaoByIds;
			}
			return new List<FPFapiaoModel>();
		}

		public List<FPFapiaoModel> GetFPTableByFPIds(MContext ctx, List<string> fpIds)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(fpIds, ref list, "M_ID");
			string sql = "SELECT t1.MFapiaoID as MID, t1.MTableID FROM t_fp_fapiao_table t1 WHERE t1.MOrgID = @MOrgID AND t1.MIsDelete = 0  AND t1.MFapiaoID" + inFilterQuery;
			return ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, list.ToArray());
		}

		public List<FPFapiaoModel> GetFapiaoListByTableIds(MContext ctx, List<string> tableIds)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "SELECT\r\n                            t0.*, t1.MTableID\n                        FROM\n                            t_fp_fapiao t0\n                                INNER JOIN\n                            t_fp_fapiao_table t1 ON t1.MFapiaoID = t0.MID\n                                AND t1.MOrgID = t0.MOrgID\n                                AND t0.MIsDelete = t1.MIsDelete\n                        WHERE\n                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                AND t1.MTableID " + GLUtility.GetInFilterQuery(tableIds, ref list, "M_ID");
			return ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, list.ToArray());
		}

		public List<FPFapiaoModel> GetFapiaoByIds(MContext ctx, List<string> fapiaoIds, bool setDefault = true, string contactID = null)
		{
			SqlWhere sqlWhere = new SqlWhere();
			List<FPFapiaoModel> list = new List<FPFapiaoModel>();
			fapiaoIds = ((fapiaoIds == null) ? new List<string>() : (from x in fapiaoIds
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList());
			if (fapiaoIds != null && fapiaoIds.Count > 0)
			{
				sqlWhere.AddFilter("MID", SqlOperators.In, fapiaoIds.ToArray()).AddFilter("MIsDelete", SqlOperators.Equal, 0);
				list = ModelInfoManager.GetDataModelList<FPFapiaoModel>(ctx, sqlWhere, false, true);
			}
			if ((list == null || list.Count == 0) & setDefault)
			{
				list = new List<FPFapiaoModel>
				{
					new FPFapiaoModel
					{
						MContactID = contactID,
						MBizDate = ctx.DateNow,
						MType = 0,
						MFapiaoEntrys = new List<FPFapiaoEntryModel>(),
						MStatus = 1
					}
				};
			}
			List<BDContactsInfoModel> contacts = new BDContactsRepository().GetContactsListByContactType(ctx, 0, null, 0, true, false);
			if (string.IsNullOrWhiteSpace(contactID))
			{
				BDContactsInfoModel contact = contacts.FirstOrDefault((BDContactsInfoModel y) => y.MItemID == contactID);
				list.ForEach(delegate(FPFapiaoModel x)
				{
					x.MContactName = ((contact == null) ? string.Empty : contact.MContactName);
				});
			}
			else
			{
				list.ForEach(delegate(FPFapiaoModel x)
				{
					BDContactsInfoModel bDContactsInfoModel = contacts.FirstOrDefault((BDContactsInfoModel y) => y.MItemID == x.MContactID);
					x.MContactName = ((bDContactsInfoModel == null) ? string.Empty : bDContactsInfoModel.MContactName);
				});
			}
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (string.IsNullOrWhiteSpace(list[i].MItemID) && list[i].MFapiaoEntrys.Count > 0)
					{
						list[i].MItemID = list[i].MFapiaoEntrys[0].MItemID;
					}
				}
			}
			return list;
		}

		public void PrehandleFapiao(MContext ctx, List<FPFapiaoModel> fapiaos)
		{
			if (fapiaos != null && fapiaos.Any())
			{
				for (int i = 0; i < fapiaos.Count; i++)
				{
					fapiaos[i].MPContactName = ((fapiaos[i].MInvoiceType == 1) ? ctx.MOrgName : fapiaos[i].MPContactName);
					fapiaos[i].MSContactName = ((fapiaos[i].MInvoiceType == 0) ? ctx.MOrgName : fapiaos[i].MSContactName);
					fapiaos[i].MPContactTaxCode = ((fapiaos[i].MInvoiceType == 1) ? ctx.MTaxCode : fapiaos[i].MPContactTaxCode);
					fapiaos[i].MSContactTaxCode = ((fapiaos[i].MInvoiceType == 0) ? ctx.MTaxCode : fapiaos[i].MSContactTaxCode);
					fapiaos[i].MStatus = ((fapiaos[i].MTotalAmount < decimal.Zero) ? 4 : fapiaos[i].MStatus);
				}
			}
		}

		public OperationResult GetUpdateFapiaoList(MContext ctx, List<FPFapiaoModel> fapiaos)
		{
			PrehandleFapiao(ctx, fapiaos);
			OperationResult operationResult = new OperationResult
			{
				Success = true,
				OperationCommands = new List<CommandInfo>()
			};
			List<string> list = new List<string>();
			int i;
			for (i = 0; i < fapiaos.Count; i++)
			{
				if (IsFapiaoNumberDuplicated(ctx, fapiaos[i]) || (from x in fapiaos
				where !string.IsNullOrWhiteSpace(x.MNumber) && x.MNumber == fapiaos[i].MNumber
				select x).Count() > 1)
				{
					throw new MActionException
					{
						Codes = new List<MActionResultCodeEnum>
						{
							MActionResultCodeEnum.MNumberInvalid
						},
						Messages = new List<string>
						{
							string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoNumberDuplicated", "发票号:{0}重复重复了"), fapiaos[i].MNumber)
						}
					};
				}
				fapiaos[i].MOrgID = ctx.MOrgID;
				operationResult.ObjectID = fapiaos[i].MID;
				bool isNew = string.IsNullOrWhiteSpace(fapiaos[i].MID);
				operationResult.OperationCommands.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoModel>(ctx, fapiaos[i], null, true));
				operationResult.OperationCommands.AddRange(GetSaveFapiaoTable(ctx, fapiaos[i]));
				operationResult.OperationCommands.AddRange(FPFapiaoLogHelper.GetEditOrAddLogCmd(ctx, new FPFapiaoModel
				{
					MID = fapiaos[i].MID,
					IsNew = isNew,
					MNumber = fapiaos[i].MNumber,
					MBizDate = fapiaos[i].MBizDate,
					MExplanation = fapiaos[i].MExplanation
				}));
			}
			return operationResult;
		}

		public FPFapiaoModel SaveFapiao(MContext ctx, FPFapiaoModel fapiao)
		{
			FPTableRepository fPTableRepository = new FPTableRepository();
			OperateVerifyType(fapiao);
			OperationResult updateFapiaoList = GetUpdateFapiaoList(ctx, new List<FPFapiaoModel>
			{
				fapiao
			});
			if (!string.IsNullOrWhiteSpace(fapiao.MTableID))
			{
				updateFapiaoList.OperationCommands.AddRange(fPTableRepository.GetUpdateTableIssueStatus(ctx, new List<string>
				{
					fapiao.MTableID
				}, null, new List<FPFapiaoModel>
				{
					fapiao
				}));
			}
			if (updateFapiaoList.Success && updateFapiaoList.OperationCommands.Count > 0)
			{
				int num = new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(updateFapiaoList.OperationCommands);
				if (num <= 0)
				{
					fapiao.MID = string.Empty;
				}
				return fapiao;
			}
			fapiao.MNumber = string.Empty;
			return fapiao;
		}

		public List<CommandInfo> GetSaveFapiaoListCmds(MContext ctx, List<FPFapiaoModel> fapiaos)
		{
			ValidateFapiaoList(ctx, fapiaos);
			if (fapiaos.Exists((FPFapiaoModel x) => x.ValidationErrors.Count > 0))
			{
				return new List<CommandInfo>();
			}
			return ModelInfoManager.GetInsertOrUpdateCmds(ctx, fapiaos, null, true);
		}

		public void ValidateFapiaoList(MContext ctx, List<FPFapiaoModel> fapiaos)
		{
			if (fapiaos != null && fapiaos.Count != 0)
			{
				GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
				List<FPFapiaoModel> fapiaoWithFields = GetFapiaoWithFields(ctx, (from x in fapiaos
				select x.MCode + x.MNumber).ToList(), new List<string>
				{
					"MID",
					"MNumber",
					"MCode",
					"MStatus",
					"MReconcileStatus",
					"MCodingStatus",
					"MVerifyType"
				}, "");
				for (int i = 0; i < fapiaos.Count; i++)
				{
					FPFapiaoModel fapiao = fapiaos[i];
					if (fapiao.MVerifyDate.Year > 1900)
					{
						fapiao.MDeductionDate = fapiao.MVerifyDate;
					}
					FPFapiaoModel fPFapiaoModel = fapiaoWithFields.FirstOrDefault((FPFapiaoModel x) => x.MCode == fapiao.MCode && x.MNumber == fapiao.MNumber);
					if (fPFapiaoModel != null)
					{
						fapiao.MID = fPFapiaoModel.MID;
						fapiao.MReconcileStatus = fPFapiaoModel.MReconcileStatus;
						fapiao.MCodingStatus = fPFapiaoModel.MCodingStatus;
						fapiao.MChangeToObsolete = (fPFapiaoModel.MStatus != 0 && fapiao.MStatus == 0);
						fapiao.MIsVerifyTypeChanged = (fPFapiaoModel.MVerifyType != fapiao.MVerifyType);
					}
					fapiao.GUID = UUIDHelper.GetGuid();
					if (fapiaos.Exists((FPFapiaoModel x) => x.GUID != fapiao.GUID && x.MCode == fapiao.MCode && x.MNumber == fapiao.MNumber))
					{
						fapiao.ValidationErrors.Add(new ValidationError
						{
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoNumberDuplicatedWithImported", "以下导入发票编号重复:") + fapiao.MNumber
						});
					}
					if (fapiaoWithFields.Exists((FPFapiaoModel x) => x.MCode == fapiao.MCode && x.MNumber == fapiao.MNumber && fapiao.MID != x.MID))
					{
						fapiao.ValidationErrors.Add(new ValidationError
						{
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoNumberDuplicatedWithSystem", "以下导入发票与系统发票编号重复:") + fapiao.MNumber
						});
					}
					if (!string.IsNullOrWhiteSpace(fapiao.MContactID) && !instance.ContactList.Exists((BDContactsModel x) => x.MItemID == fapiao.MContactID))
					{
						fapiao.ValidationErrors.Add(new ValidationError
						{
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoContactNotExists", "以下导入发票联系人不存在") + fapiao.MNumber
						});
					}
					if (fapiao.MFapiaoEntrys != null && fapiao.MFapiaoEntrys.Count > 0)
					{
						for (int j = 0; j < fapiao.MFapiaoEntrys.Count; j++)
						{
							FPFapiaoEntryModel entry = fapiao.MFapiaoEntrys[j];
							if (!string.IsNullOrWhiteSpace(entry.MItemID) && !instance.MerItemList.Exists((BDItemModel x) => x.MItemID == entry.MItemID))
							{
								fapiao.ValidationErrors.Add(new ValidationError
								{
									Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoItemNotExists", "以下导入发票商品项目不存在:") + fapiao.MNumber
								});
							}
						}
					}
				}
			}
		}

		public List<FPFapiaoModel> GetFapiaoWithFields(MContext ctx, List<string> codeNumbers, List<string> fields, string otherFilter = "")
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string empty = string.Empty;
			empty += GLUtility.GetInFilterQuery(codeNumbers, ref list, "M_ID");
			string sql = "select " + string.Join(",", fields) + " from t_fp_fapiao where morgid = @MOrgID and MIsDelete = 0  and concat(MCode, MNumber) " + empty + otherFilter;
			return ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, list.ToArray());
		}

		public List<CommandInfo> GetSaveFapiaoTable(MContext ctx, FPFapiaoModel fapiao)
		{
			if (!string.IsNullOrWhiteSpace(fapiao.MTableID))
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MTableID", SqlOperators.Equal, fapiao.MTableID).AddFilter("MFapiaoID", SqlOperators.Equal, fapiao.MID).AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
				List<FPFapiaoTableModel> dataModelList = ModelInfoManager.GetDataModelList<FPFapiaoTableModel>(ctx, sqlWhere, false, false);
				if (dataModelList != null && dataModelList.Count > 0)
				{
					FPFapiaoTableModel fPFapiaoTableModel = dataModelList[0];
					fPFapiaoTableModel.MFapiaoType = fapiao.MType;
					return ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoTableModel>(ctx, fPFapiaoTableModel, null, true);
				}
				FPFapiaoTableModel modelData = new FPFapiaoTableModel
				{
					MFapiaoID = fapiao.MID,
					MTableID = fapiao.MTableID,
					MFapiaoType = fapiao.MType,
					MOrgID = ctx.MOrgID
				};
				return ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoTableModel>(ctx, modelData, null, true);
			}
			return new List<CommandInfo>();
		}

		public bool IsFapiaoNumberDuplicated(MContext ctx, FPFapiaoModel fapiao)
		{
			if (!string.IsNullOrWhiteSpace(fapiao.MNumber))
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MNumber", SqlOperators.Equal, fapiao.MNumber);
				sqlWhere.AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID);
				sqlWhere.AddFilter("MIsDelete", SqlOperators.Equal, 0);
				if (!string.IsNullOrWhiteSpace(fapiao.MID))
				{
					sqlWhere.AddFilter("MID", SqlOperators.NotEqual, fapiao.MID);
				}
				return ExistsByFilter(ctx, sqlWhere);
			}
			return false;
		}

		public OperationResult DeleteFapiaoByFapiaoIds(MContext ctx, List<string> fapiaoIdList)
		{
			List<CommandInfo> deleteFapiaoCmd = GetDeleteFapiaoCmd(ctx, fapiaoIdList);
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteFapiaoCmd) > 0)
			};
		}

		public List<CommandInfo> GetDeleteFapiaoCmd(MContext ctx, List<string> fapiaoIdList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			fapiaoIdList = fapiaoIdList.Distinct().ToList();
			List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, fapiaoIdList, false, null);
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(fapiaoIdList, ref list2, "M_ID");
			List<CommandInfo> list3 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = "UPDATE t_fp_fapiaoentry SET MIsDelete = 1 WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list3.Add(obj);
			List<CommandInfo> list4 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "UPDATE t_fp_fapiao SET MIsDelete = 1 WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery
			};
			array = (obj2.Parameters = list2.ToArray());
			list4.Add(obj2);
			for (int i = 0; i < fapiaoByIds.Count; i++)
			{
				FPFapiaoLogHelper.DeleteLog(ctx, fapiaoByIds[i]);
			}
			CommandInfo commandInfo = new FPImportRepository().DeleteImportByDeleteFapiaoIds(ctx, fapiaoByIds);
			if (commandInfo != null)
			{
				list.Add(commandInfo);
			}
			return list;
		}

		public OperationResult BatchUpdateFPStatusByIds(MContext ctx, List<string> fapiaoIds, int status)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(fapiaoIds, ref list2, "M_ID");
			list2.Add(new MySqlParameter("@MStatus", status));
			List<CommandInfo> list3 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = "UPDATE t_fp_fapiao SET MStatus = @MStatus WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list3.Add(obj);
			List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, fapiaoIds, false, null);
			if (ctx.MOrgVersionID == 0 && status == 0)
			{
				List<CommandInfo> list4 = list;
				CommandInfo obj2 = new CommandInfo
				{
					CommandText = "UPDATE t_fp_fapiao_table SET MIsDelete = 1 WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MFapiaoID " + inFilterQuery
				};
				array = (obj2.Parameters = list2.ToArray());
				list4.Add(obj2);
				List<CommandInfo> list5 = list;
				CommandInfo obj3 = new CommandInfo
				{
					CommandText = "UPDATE t_fp_fapiao SET MReconcileStatus = 0 WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID " + inFilterQuery
				};
				array = (obj3.Parameters = list2.ToArray());
				list5.Add(obj3);
				if (fapiaoByIds.Any())
				{
					List<FPFapiaoModel> source = (from a in fapiaoByIds
					where a.MReconcileStatus == 1
					select a).ToList();
					if (source.Any())
					{
						List<FPFapiaoModel> fPTableByFPIds = GetFPTableByFPIds(ctx, (from a in source
						select a.MID).ToList());
						if (fPTableByFPIds != null)
						{
							IEnumerable<IGrouping<string, FPFapiaoModel>> enumerable = from a in fPTableByFPIds
							group a by a.MTableID;
							list.AddRange(GetUpdateTableIssueStatusCMDByRemoverFP(ctx, (from a in source
							select a.MID).ToList()));
							foreach (IGrouping<string, FPFapiaoModel> item in enumerable)
							{
								IEnumerable<string> itemFP = from a in item
								select a.MID;
								List<FPFapiaoModel> list6 = (from a in source
								where itemFP.Contains(a.MID)
								select a).ToList();
								if (list6 != null)
								{
									List<CommandInfo> removeReoncileLogCmd = FPTableLogHelper.GetRemoveReoncileLogCmd(ctx, new FPFapiaoReconcileModel
									{
										MTable = new FPTableViewModel
										{
											MItemID = item.Key
										},
										MFapiaoList = list6
									});
									list.AddRange(removeReoncileLogCmd);
								}
							}
						}
					}
				}
			}
			list.AddRange(FPFapiaoLogHelper.BatchUpdateFPStatusLog(ctx, fapiaoByIds, status));
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0)
			};
		}

		private List<CommandInfo> GetUpdateTableIssueStatusCMDByRemoverFP(MContext ctx, List<string> removerFPs)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (removerFPs.Any())
			{
				FPTableRepository fPTableRepository = new FPTableRepository();
				List<FPTableModel> tableListByFapiaoIds = fPTableRepository.GetTableListByFapiaoIds(ctx, removerFPs);
				if (tableListByFapiaoIds.Any())
				{
					List<FPFapiaoModel> fapiaoListDataByTableIds = GetFapiaoListDataByTableIds(ctx, (from a in tableListByFapiaoIds
					select a.MItemID).ToList());
					List<string> fields = new List<string>
					{
						"MIssueStatus",
						"MRAmount",
						"MRTaxAmount",
						"MRTotalAmount"
					};
					foreach (FPTableModel item in tableListByFapiaoIds)
					{
						List<FPFapiaoModel> source = (from a in fapiaoListDataByTableIds
						where a.MTableID == item.MItemID
						select a).ToList();
						source = (from a in source
						where !removerFPs.Contains(a.MID)
						select a).ToList();
						fPTableRepository.UpdateTableIssueStatus(item, source);
						list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, item, fields, true));
					}
				}
			}
			return list;
		}

		public OperationResult BatchUpdateFPVerifyType(MContext ctx, List<FPFapiaoModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> fapiaoIds = (from a in modelList
			select a.MID).ToList();
			List<FPFapiaoModel> fapiaoByIds = GetFapiaoByIds(ctx, fapiaoIds, false, null);
			foreach (FPFapiaoModel model in modelList)
			{
				DateTime? nullable = null;
				List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
				list2.Add(new MySqlParameter("@MID", model.MID));
				string str = "UPDATE t_fp_fapiao SET MVerifyType = @MVerifyType ";
				list2.Add(new MySqlParameter("@MVerifyType", model.MVerifyType));
				bool flag = false;
				if (model.MVerifyType == 0)
				{
					FPFapiaoModel fPFapiaoModel = fapiaoByIds.FirstOrDefault((FPFapiaoModel a) => a.MID == model.MID);
					if (fPFapiaoModel != null && (fPFapiaoModel.MVerifyType == 1 || fPFapiaoModel.MVerifyType == 2))
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
					nullable = model.MVerifyDate;
				}
				if (flag)
				{
					str += ",MVerifyDate=@MVerifyDate,MDeductionDate=@MVerifyDate ";
					list2.Add(new MySqlParameter("@MVerifyDate", nullable));
				}
				str += " WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MID=@MID";
				List<CommandInfo> list3 = list;
				CommandInfo obj = new CommandInfo
				{
					CommandText = str
				};
				DbParameter[] array = obj.Parameters = list2.ToArray();
				list3.Add(obj);
			}
			list.AddRange(FPFapiaoLogHelper.BatchUpdateFPVerifyTypeLog(ctx, fapiaoByIds, modelList));
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0)
			};
		}

		public List<FPFapiaoModel> GetReconciledFapiaoList(MContext ctx, string tableID)
		{
			string sql = "SELECT\r\n                                t1.*\n                            FROM\n                                t_fp_fapiao t1\n                                    INNER JOIN\n                                t_fp_fapiao_Table t2 ON t1.MID = t2.MFapiaoID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                                    AND t1.MOrgID = t2.MOrgID\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                    AND t2.MTableID = @MTableID";
			MySqlParameter[] parameters = ctx.GetParameters("@MTableID", tableID);
			return ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, sql, parameters);
		}

		public List<FPFapiaoReconcileModel> GetFapiaoReconcileList(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@MStartDate", filter.MStartDate),
				new MySqlParameter("@MEndDate", filter.MEndDate),
				new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory),
				new MySqlParameter("@MKeyword", filter.MKeyword),
				new MySqlParameter("@MTotalAmount", filter.MTotalAmount)
			});
			string text = (filter.MFapiaoCategory == 0) ? "P" : "S";
			string str = "SELECT\r\n                                t1.MItemID,\n                                t2.MID,\n                                t1.MContactTaxCode,\n                                t1.MTotalAmount AS MTableTotalAmount,\n                                t2.MTotalAmount,\n                                t2.MAmount,\n                                t2.MTaxAmount,\n                                t1.MAjustAmount,\n                                t1.MBizDate as MTableBizDate,\n                                t2.MBizDate,\n                                t1.MNumber AS MTableNumber,\n                                t2.MNumber,\n                                t1.MRTaxAmount,\n                                t1.MRAmount,\n                                t1.MRTotalAmount,\n                                t1.MIssueStatus,\n                                CONVERT( AES_DECRYPT(t3.MName, '{0}') USING UTF8) AS MContactName,\n                                t2.MSContactName,\n                                t2.MSContactTaxCode,\n                                t2.MPContactName,\n                                t2.MPContactTaxCode\n                            FROM\n                                t_fp_table t1\n                                    INNER JOIN\n                                t_bd_contacts_l t3 ON t1.MContactID = t3.MParentID\n                                    AND t1.MIsDelete = t3.MIsDelete\n                                    AND t1.MOrgID = t3.MOrgID\n                                    AND t3.MLocaleID = @MLocaleID\n                                    LEFT JOIN\n                                t_fp_fapiao t2 ON t1.MOrgID = t2.MOrgID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                                    AND t2.MStatus != 0\n                                    AND t2.MReconcileStatus = 0\n                                    AND t1.MInvoiceType = t2.MInvoiceType\n                                    {1}\n                            WHERE\n                                    t1.MOrgID = @MOrgID\n                                    AND t1.MInvoiceType = @MFapiaoCategory\n                                    AND t1.MIsDelete = 0\r\n                            ";
			if (filter.MStartDate > new DateTime(1900, 1, 1))
			{
				str += " AND t1.MBizDate >= @MStartDate";
			}
			if (filter.MEndDate > new DateTime(1900, 1, 1))
			{
				str += " AND t1.MBizDate <= @MEndDate";
			}
			if (!string.IsNullOrWhiteSpace(filter.MTableID))
			{
				str = str + " and t1.MItemID in ('" + string.Join("','", filter.MTableID.Split(',')) + "') ";
			}
			string arg = string.Empty;
			if (!filter.MShowAll)
			{
				arg = ((!filter.MFindFapiao) ? ((filter.MFapiaoCategory != 1) ? " AND t1.MContactTaxCode = t2.M{1}ContactTaxCode  AND (t1.MTotalAmount - t1.MAjustAmount - t1.MRTotalAmount) = t2.MTotalAmount " : " AND CONVERT(AES_DECRYPT(t3.MName, '{0}') USING UTF8) = t2.M{1}ContactName AND (t1.MTotalAmount - t1.MAjustAmount - t1.MRTotalAmount) = t2.MTotalAmount ") : ((filter.MFapiaoCategory != 1) ? " AND t1.MContactTaxCode = t2.M{1}ContactTaxCode " : " AND CONVERT(AES_DECRYPT(t3.MName, '{0}') USING UTF8) = t2.M{1}ContactName "));
				arg = string.Format(arg, "JieNor-001", text);
			}
			if (filter.MTotalAmount.HasValue)
			{
				str += " and t2.MTotalAmount = @MTotalAmount";
			}
			if (!string.IsNullOrWhiteSpace(filter.MKeyword))
			{
				str += " and (";
				str += " t2.MNumber like concat('%', @MKeyword,'%') or t2.M{2}ContactName like concat('%', @MKeyword,'%')";
				if (DateTime.TryParse(filter.MKeyword, out DateTime _))
				{
					str += " OR date(t2.MBizDate) = @MKeyword ";
				}
				str += ") ";
			}
			str += " order by t1.MItemID ";
			str = string.Format(str, "JieNor-001", arg, text);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(str, list.ToArray());
			return (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0) ? new List<FPFapiaoReconcileModel>() : BindDataTable2Model(ctx, dataSet.Tables[0]);
		}

		public List<CommandInfo> GetDeleteFapiaoVoucherCmds(MContext ctx, List<string> voucherIDs)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string commandText = " UPDATE t_fp_fapiao t1,\n                                t_fp_fapiao_voucher t2\r\n                            SET\r\n                                t1.MCodingStatus = 0\n                            WHERE\n                                t1.MID = t2.MFapiaoID\n                                    AND t1.MOrgID = t2.MOrgID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                                    AND t1.MOrgID = @MOrgID\n                                    AND t1.MIsDelete = 0\n                                    AND t2.MVoucherID " + GLUtility.GetInFilterQuery(voucherIDs, ref list, "M_ID");
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string commandText2 = " update t_fp_fapiao_voucher set MIsDelete = 1 where MOrgID = @MOrgID and MIsDelete = 0 and MVoucherID " + GLUtility.GetInFilterQuery(voucherIDs, ref list2, "M_ID");
			List<CommandInfo> list3 = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = list.ToArray();
			list3.Add(obj);
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = commandText2
			};
			array = (obj2.Parameters = list2.ToArray());
			list3.Add(obj2);
			return list3;
		}

		public List<FPFapiaoModel> GetFapiaoListByFilter(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			string fapiaoListByFilterQuerySql = GetFapiaoListByFilterQuerySql(ctx, filter, out list, false);
			return ModelInfoManager.GetDataModelBySql<FPFapiaoModel>(ctx, fapiaoListByFilterQuerySql, list.ToArray());
		}

		public List<FPFapiaoModel> GetFapiaoListIncludeEntry(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string str = (filter.MFapiaoCategory == 1) ? "S" : "P";
			string str2 = "SELECT\r\n\t                            t1.MID , t1.MOrgID , t1.MCode , t1.MNumber , t1.MBizDate , t1.MPContactName , t1.MPContactTaxCode ,t1.MType , t1.MVerifyType, t1.MVerifyDate,t1.MRemark , t1.MBizDate,\r\n                                t1.MPContactAddressPhone,t1.MPContactBankInfo,t1.MSContactAddressPhone,t1.MSContactBankInfo,\r\n                                t4.MItemName,t4.MItemType,t4.MTaxPercent,t4.MTaxAmount,t4.MAmount,t4.MPrice,t4.MQuantity,t4.MUnit,t1.MExplanation,t1.MStatus,t1.MSContactName,t1.MSContactTaxCode\r\n                             FROM\r\n\t                            t_fp_fapiao t1\r\n                                INNER JOIN t_fp_fapiaoentry t4 ON t1.MOrgID = t4.MOrgID AND t1.MID = t4.MID AND t1.MIsDelete = t4.MIsDelete\r\n                                WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0  ";
			if (filter.MFapiaoIDs != null && filter.MFapiaoIDs.Count > 0)
			{
				str2 = str2 + " and t1.MID in ('" + string.Join("','", filter.MFapiaoIDs) + "') ";
			}
			if (filter.MStartDate > new DateTime(1900, 1, 1))
			{
				str2 += " and t1.MBizDate >= @MStartDate ";
				list.Add(new MySqlParameter("@MStartDate", filter.MStartDate));
			}
			if (filter.MEndDate > new DateTime(1900, 1, 1))
			{
				str2 += " and t1.MBizDate <= @MEndDate ";
				list.Add(new MySqlParameter("@MEndDate", filter.MEndDate));
			}
			if (!string.IsNullOrWhiteSpace(filter.MReconcileStatus))
			{
				str2 += " and t1.MReconcileStatus = @MReconcileStatus ";
				list.Add(new MySqlParameter("@MReconcileStatus", filter.MReconcileStatus));
			}
			if (!string.IsNullOrWhiteSpace(filter.MCodingStatus))
			{
				str2 += " and t1.MCodingStatus = @MCodingStatus ";
				list.Add(new MySqlParameter("@MCodingStatus", filter.MCodingStatus));
			}
			if (filter.MFapiaoCategory >= 0)
			{
				str2 += " and t1.MInvoiceType = @MFapiaoCategory ";
				list.Add(new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory));
			}
			if (!string.IsNullOrWhiteSpace(filter.MImportID))
			{
				str2 += " and t1.MImportID = @MImportID";
				list.Add(new MySqlParameter("@MImportID", filter.MImportID));
			}
			if (!string.IsNullOrWhiteSpace(filter.MStatus))
			{
				str2 = str2 + " and t1.MStatus in(" + string.Join(",", filter.MStatus.Split(',')) + ") ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MVerifyStatus))
			{
				string[] array = filter.MVerifyStatus.Split(',');
				string text = "";
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!string.IsNullOrWhiteSpace(text2))
					{
						text = ((!(text2 == "0")) ? (text + $" MVerifyType = {text2} OR") : (text + " IFNULL(MVerifyType,'') ='' OR MVerifyType=0 OR"));
					}
				}
				if (text.Length > 2)
				{
					text = text.Substring(0, text.Length - 2);
				}
				str2 += $" AND ({text}) ";
			}
			if (filter.MVerifyDate > new DateTime(1900, 1, 1))
			{
				DateTime dateTime = filter.MVerifyDate;
				string value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				dateTime = filter.MVerifyDate;
				dateTime = dateTime.AddMonths(1);
				dateTime = dateTime.AddSeconds(-1.0);
				string value2 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				str2 += " AND  MVerifyDate >= @StartVerifyDate  AND MVerifyDate <= @EndVerifyDate ";
				list.Add(new MySqlParameter("@StartVerifyDate", value));
				list.Add(new MySqlParameter("@EndVerifyDate", value2));
			}
			if (!string.IsNullOrWhiteSpace(filter.MKeyword))
			{
				str2 += " and (";
				str2 = str2 + " t1.MNumber like concat('%', @Keyword, '%')\r\n                               OR t1.M" + str + "ContactName like concat('%', @Keyword, '%') OR t4.MItemName  like concat('%', @Keyword, '%')";
				if (DateTime.TryParse(filter.MKeyword, out DateTime dateTime2))
				{
					str2 += " OR date(t1.MBizDate) = @KeywordDate ";
					list.Add(new MySqlParameter("@KeywordDate", dateTime2));
				}
				if (filter.MTotalAmount.HasValue)
				{
					str2 += " OR t1.MTotalAmount = @MTotalAmount OR t4.MPrice=@MTotalAmount OR t4.MAmount=@MTotalAmount ";
					list.Add(new MySqlParameter("@MTotalAmount", filter.MTotalAmount.Value));
				}
				str2 += " ) ";
				list.Add(new MySqlParameter("@Keyword", filter.MKeyword));
			}
			string field = string.IsNullOrWhiteSpace(filter.Sort) ? "MBizDate" : filter.Sort;
			string orderBy = string.IsNullOrWhiteSpace(filter.Order) ? " desc" : filter.Order;
			str2 += GetOrderString(field, orderBy, (filter.MFapiaoCategory == 1) ? "S" : "P", "t1.");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(str2, list.ToArray());
			return ConvertToFapiaoList(ds);
		}

		private List<FPFapiaoModel> ConvertToFapiaoList(DataSet ds)
		{
			List<FPFapiaoModel> list = new List<FPFapiaoModel>();
			if (ds == null || ds.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			DataTable dataTable = ds.Tables[0];
			string text = "";
			FPFapiaoModel fPFapiaoModel = new FPFapiaoModel();
			fPFapiaoModel.MFapiaoEntrys = new List<FPFapiaoEntryModel>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				text = dataRow.Field<string>("MID");
				if (fPFapiaoModel.MItemID != text)
				{
					fPFapiaoModel = new FPFapiaoModel();
					fPFapiaoModel.MFapiaoEntrys = new List<FPFapiaoEntryModel>();
					fPFapiaoModel.MItemID = text;
					fPFapiaoModel.MCode = dataRow.MField<string>("MCode");
					fPFapiaoModel.MNumber = dataRow.MField<string>("MNumber");
					fPFapiaoModel.MBizDate = dataRow.MField<DateTime>("MBizDate");
					fPFapiaoModel.MPContactName = dataRow.MField<string>("MPContactName");
					fPFapiaoModel.MPContactTaxCode = dataRow.MField<string>("MPContactTaxCode");
					fPFapiaoModel.MPContactAddressPhone = dataRow.MField<string>("MPContactAddressPhone");
					fPFapiaoModel.MPContactBankInfo = dataRow.MField<string>("MPContactBankInfo");
					fPFapiaoModel.MSContactName = dataRow.MField<string>("MSContactName");
					fPFapiaoModel.MSContactTaxCode = dataRow.MField<string>("MSContactTaxCode");
					fPFapiaoModel.MSContactAddressPhone = dataRow.MField<string>("MSContactAddressPhone");
					fPFapiaoModel.MSContactBankInfo = dataRow.MField<string>("MSContactBankInfo");
					fPFapiaoModel.MType = ((!(Convert.ToString(dataRow["MType"]) == "0")) ? 1 : 0);
					fPFapiaoModel.MVerifyType = dataRow.MField<int>("MVerifyType");
					fPFapiaoModel.MVerifyDate = dataRow.MField<DateTime>("MVerifyDate");
					fPFapiaoModel.MRemark = dataRow.MField<string>("MRemark");
					fPFapiaoModel.MExplanation = dataRow.MField<string>("MExplanation");
					fPFapiaoModel.MStatus = dataRow.MField<int>("MStatus");
					list.Add(fPFapiaoModel);
				}
				FPFapiaoEntryModel fPFapiaoEntryModel = new FPFapiaoEntryModel();
				fPFapiaoEntryModel.MItemName = dataRow.MField<string>("MItemName");
				fPFapiaoEntryModel.MItemType = dataRow.MField<string>("MItemType");
				fPFapiaoEntryModel.MTaxPercent = dataRow.MField<decimal>("MTaxPercent");
				fPFapiaoEntryModel.MTaxAmount = dataRow.MField<decimal>("MTaxAmount");
				fPFapiaoEntryModel.MAmount = dataRow.MField<decimal>("MAmount");
				fPFapiaoEntryModel.MPrice = dataRow.MField<decimal>("MPrice");
				fPFapiaoEntryModel.MQuantity = dataRow.MField<decimal>("MQuantity");
				fPFapiaoEntryModel.MUnit = dataRow.MField<string>("MUnit");
				fPFapiaoModel.MFapiaoEntrys.Add(fPFapiaoEntryModel);
			}
			return list;
		}

		public int GetFapiaoListCountByFilter(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			string fapiaoListByFilterQuerySql = GetFapiaoListByFilterQuerySql(ctx, filter, out list, true);
			return int.Parse(new DynamicDbHelperMySQL(ctx).GetSingle(fapiaoListByFilterQuerySql, list.ToArray()).ToString());
		}

		public string GetFapiaoListByFilterQuerySql(MContext ctx, FPFapiaoFilterModel filter, out List<MySqlParameter> parameters, bool count = false)
		{
			parameters = ctx.GetParameters((MySqlParameter)null).ToList();
			string str = "SELECT\r\n                                {0}{1}\n                            FROM\n                                t_fp_fapiao t1 {2}\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(filter.MTableID))
			{
				str += " AND EXISTS( SELECT\r\n                                                1\n                                            FROM\n                                                t_fp_fapiao_table t2\n                                            WHERE\n                                                t2.MOrgID = t1.MOrgID\n                                                    AND t2.MIsDelete = t1.MIsDelete\n                                                    AND t2.MFapiaoID = t1.MID\n                                                    and t2.MTableID = @MTableID )";
				parameters.Add(new MySqlParameter("@MTableID", filter.MTableID));
			}
			string text = (filter.MFapiaoCategory == 1) ? "S" : "P";
			string arg = string.Empty;
			string arg2 = string.Empty;
			if (ctx.MOrgVersionID == 1 && !count)
			{
				arg = ", t3.MNumber as MVoucherNumber, t3.MItemID as MVoucherID ";
				arg2 = " LEFT JOIN\n                    t_fp_fapiao_voucher t2 ON t2.MOrgID = t1.MOrgID\n                        AND t2.MIsDelete = t1.MIsDelete\n                        AND t2.MFapiaoID = t1.MID\n                        LEFT JOIN\n                    t_gl_voucher t3 ON t2.MVoucherID = t3.MItemID\n                        AND t3.MOrgID = t1.MOrgID\n                        AND t3.MIsDelete = t1.MIsDelete ";
			}
			if (filter.MFapiaoIDs != null && filter.MFapiaoIDs.Count > 0)
			{
				str = str + " and t1.MID in ('" + string.Join("','", filter.MFapiaoIDs) + "') ";
			}
			if (filter.MStartDate > new DateTime(1900, 1, 1))
			{
				str += " and t1.MBizDate >= @MStartDate ";
				parameters.Add(new MySqlParameter("@MStartDate", filter.MStartDate));
			}
			if (filter.MEndDate > new DateTime(1900, 1, 1))
			{
				str += " and t1.MBizDate <= @MEndDate ";
				parameters.Add(new MySqlParameter("@MEndDate", filter.MEndDate));
			}
			if (!string.IsNullOrWhiteSpace(filter.MReconcileStatus))
			{
				str += " and t1.MReconcileStatus = @MReconcileStatus ";
				parameters.Add(new MySqlParameter("@MReconcileStatus", filter.MReconcileStatus));
			}
			if (!string.IsNullOrWhiteSpace(filter.MCodingStatus))
			{
				str += " and t1.MCodingStatus = @MCodingStatus ";
				parameters.Add(new MySqlParameter("@MCodingStatus", filter.MCodingStatus));
			}
			if (filter.MFapiaoCategory >= 0)
			{
				str += " and t1.MInvoiceType = @MFapiaoCategory ";
				parameters.Add(new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory));
			}
			if (!string.IsNullOrWhiteSpace(filter.MImportID))
			{
				str += " and t1.MImportID = @MImportID";
				parameters.Add(new MySqlParameter("@MImportID", filter.MImportID));
			}
			if (!string.IsNullOrWhiteSpace(filter.MStatus))
			{
				str = str + " and t1.MStatus in(" + string.Join(",", filter.MStatus.Split(',')) + ") ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MVerifyStatus))
			{
				string[] array = filter.MVerifyStatus.Split(',');
				string text2 = "";
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					if (!string.IsNullOrWhiteSpace(text3))
					{
						text2 = ((!(text3 == "0")) ? (text2 + $" t1.MVerifyType = {text3} OR") : (text2 + " IFNULL(t1.MVerifyType,'') ='' OR t1.MVerifyType=0 OR"));
					}
				}
				if (text2.Length > 2)
				{
					text2 = text2.Substring(0, text2.Length - 2);
				}
				str += $" AND ({text2}) ";
			}
			if (filter.MVerifyDate > new DateTime(1900, 1, 1))
			{
				DateTime dateTime = filter.MVerifyDate;
				string value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				dateTime = filter.MVerifyDate;
				dateTime = dateTime.AddMonths(1);
				dateTime = dateTime.AddSeconds(-1.0);
				string value2 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				str += " AND  t1.MVerifyDate >= @StartVerifyDate  AND t1.MVerifyDate <= @EndVerifyDate ";
				parameters.Add(new MySqlParameter("@StartVerifyDate", value));
				parameters.Add(new MySqlParameter("@EndVerifyDate", value2));
			}
			if (!string.IsNullOrWhiteSpace(filter.MKeyword))
			{
				str += " and (";
				str = str + " t1.MNumber like concat('%', @Keyword, '%')\r\n                               OR t1.M" + text + "ContactName like concat('%', @Keyword, '%')  ";
				if (DateTime.TryParse(filter.MKeyword, out DateTime dateTime2))
				{
					str += " OR date(t1.MBizDate) = @KeywordDate ";
					parameters.Add(new MySqlParameter("@KeywordDate", dateTime2));
				}
				if (filter.MTotalAmount.HasValue)
				{
					str += " OR t1.MTotalAmount = @MTotalAmount ";
					parameters.Add(new MySqlParameter("@MTotalAmount", filter.MTotalAmount.Value));
				}
				str += " OR t1.MID IN ( Select MID from t_fp_fapiaoentry where MOrgID=@MOrgID and MIsDelete=0  and ( MItemName like concat('%', @Keyword, '%') ";
				if (filter.MTotalAmount.HasValue)
				{
					str += " OR MPrice=@MTotalAmount OR MAmount=@MEntryTotalAmount ";
					parameters.Add(new MySqlParameter("@MEntryTotalAmount", filter.MTotalAmount.Value));
				}
				str += " ) )";
				str += " ) ";
				parameters.Add(new MySqlParameter("@Keyword", filter.MKeyword));
			}
			string field = string.IsNullOrWhiteSpace(filter.Sort) ? "MBizDate" : filter.Sort;
			string orderBy = string.IsNullOrWhiteSpace(filter.Order) ? " desc" : filter.Order;
			str += GetOrderString(field, orderBy, text, "t1.");
			if (filter.rows > 0 && !count)
			{
				str = str + " limit " + (filter.page - 1) * filter.rows + "," + filter.rows;
			}
			return string.Format(str, count ? " count(t1.MID) as MCount " : " t1.* ", arg, arg2);
		}

		public List<FPFapiaoReconcileModel> BindDataTable2Model(MContext ctx, DataTable dt)
		{
			FPFapiaoReconcileModel fPFapiaoReconcileModel = null;
			List<FPFapiaoReconcileModel> list = new List<FPFapiaoReconcileModel>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = dt.Rows[i].MField("MItemID");
				string text2 = dt.Rows[i].MField("MID");
				if (fPFapiaoReconcileModel == null || fPFapiaoReconcileModel.MTable.MItemID != text)
				{
					fPFapiaoReconcileModel = new FPFapiaoReconcileModel
					{
						MTable = new FPTableViewModel
						{
							MItemID = text,
							MContactName = dt.Rows[i].MField("MContactName"),
							MBizDate = dt.Rows[i].MField<DateTime>("MTableBizDate"),
							MNumber = dt.Rows[i].MField("MTableNumber"),
							MTotalAmount = dt.Rows[i].MField<decimal>("MTableTotalAmount"),
							MAjustAmount = dt.Rows[i].MField<decimal>("MAjustAmount"),
							MContactTaxCode = dt.Rows[i].MField("MContactTaxCode"),
							MIssueStatus = dt.Rows[i].MField<int>("MIssueStatus"),
							MRAmount = dt.Rows[i].MField<decimal>("MRAmount"),
							MRTaxAmount = dt.Rows[i].MField<decimal>("MRTaxAmount"),
							MRTotalAmount = dt.Rows[i].MField<decimal>("MRTotalAmount")
						},
						MFapiaoList = new List<FPFapiaoModel>()
					};
					list.Add(fPFapiaoReconcileModel);
				}
				if (!string.IsNullOrWhiteSpace(text2))
				{
					fPFapiaoReconcileModel.MFapiaoList.Add(new FPFapiaoModel
					{
						MID = text2,
						MBizDate = dt.Rows[i].MField<DateTime>("MBizDate"),
						MNumber = dt.Rows[i].MField("MNumber"),
						MTotalAmount = dt.Rows[i].MField<decimal>("MTotalAmount"),
						MPContactTaxCode = dt.Rows[i].MField("MPContactTaxCode"),
						MSContactTaxCode = dt.Rows[i].MField("MSContactTaxCode"),
						MPContactName = dt.Rows[i].MField("MPContactName"),
						MSContactName = dt.Rows[i].MField("MSContactName")
					});
				}
			}
			return list;
		}

		public DataGridJson<FPFapiaoModel> Get(MContext ctx, GetParam param)
		{
			StringBuilder stringBuilder = new StringBuilder();
			SqlQuery sqlQuery = new SqlQuery();
			SqlWhere sqlWhere = new SqlWhere();
			if (param.Where == 0.ToString())
			{
				stringBuilder.Append("select MOrgID,MBizDate,MCode,MNumber from T_FP_Fapiao where MOrgID = @MOrgID and MIsDelete=0 and MStatus=0");
				sqlWhere.PageSize = 2147483647;
			}
			else
			{
				stringBuilder.Append("select * T_FP_Fapiao where MOrgID = @MOrgID and MIsDelete=0");
				sqlWhere.PageSize = param.PageSize;
				sqlWhere.PageIndex = param.PageIndex;
				sqlWhere.WhereSqlString = param.Where;
			}
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			sqlQuery.SqlWhere = sqlWhere;
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddOrderBy(" MCreateDate ", SqlOrderDir.Desc);
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<FPFapiaoModel>(ctx, sqlQuery);
		}

		public DataTable GetFapiaoSummaryByDate(MContext ctx, FPFapiaoFilterModel filter)
		{
			DataTable result = new DataTable();
			string sql = "SELECT MBizDate,MONTH (MBizDate) AS MONTH,SUM(MTotalAmount) as MTotalAmount\r\n                            FROM\r\n\t                        t_fp_fapiao\r\n                            WHERE\r\n\t                            MBizDate >= @StartDate\r\n                                AND MBizDate <= @EndDate\r\n                                AND MOrgID = @MOrgID\r\n                                AND (MStatus = 1 OR MStatus=2 OR MStatus=3 OR MStatus=4)\r\n                                AND MIsDelete = 0\r\n                                AND MInvoiceType = @InvoiceType\r\n                            GROUP BY\r\n\t                            (YEAR(MBizDate) + MONTH(MBizDate))\r\n                            ORDER BY\r\n\t                            MBizDate";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@StartDate", filter.MStartDate),
				new MySqlParameter("@EndDate", filter.MEndDate),
				new MySqlParameter("@InvoiceType", filter.MFapiaoCategory)
			});
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
			if (dataSet != null && dataSet.Tables != null)
			{
				result = dataSet.Tables[0];
			}
			return result;
		}

		public DataTable GetFapiaoTaxByMonth(MContext ctx, DateTime dateTime)
		{
			DataTable result = new DataTable();
			string sql = "SELECT\r\n\t                         MInvoiceType,\r\n\t                         SUM(MTaxAmount) AS TaxAmount\r\n                           FROM\r\n\t                         t_fp_fapiao\r\n                           WHERE\r\n\t                         MOrgID = @MOrgID\r\n                             AND YEAR (MBizDate) = @Year\r\n                             AND MONTH (MBizDate) = @Month\r\n                             AND (MStatus = 1 OR MStatus=2 OR MStatus=3 OR MStatus=4)\r\n                             AND MIsDelete = 0\r\n                           GROUP BY\r\n\t                         MInvoiceType";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@Year", dateTime.Year),
				new MySqlParameter("@Month", dateTime.Month)
			});
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, list.ToArray());
			if (dataSet != null && dataSet.Tables != null)
			{
				result = dataSet.Tables[0];
			}
			return result;
		}

		public FPCodingSettingModel GetCodingSetting(MContext ctx)
		{
			string sql = "select * from t_fp_codingsetting where MOrgID = @MOrgID and MUserID = @MUserID and MIsDelete = 0 ";
			MySqlParameter[] parameters = ctx.GetParameters((MySqlParameter)null);
			FPCodingSettingModel dataModel = ModelInfoManager.GetDataModel<FPCodingSettingModel>(ctx, sql, parameters);
			if (dataModel == null || string.IsNullOrWhiteSpace(dataModel.MID))
			{
				string sql2 = "select * from t_fp_codingsetting where MOrgID = '0' and MUserID = '0' and MIsDelete = 0 ";
				FPCodingSettingModel dataModel2 = ModelInfoManager.GetDataModel<FPCodingSettingModel>(ctx, sql2, new MySqlParameter[0]);
				if (dataModel2 == null || string.IsNullOrWhiteSpace(dataModel2.MID))
				{
					dataModel = GetDefaultCodingSettingModel(ctx);
					dataModel.MOrgID = "0";
					dataModel.MUserID = "0";
					ModelInfoManager.InsertOrUpdate<FPCodingSettingModel>(ctx, dataModel, null);
					dataModel2 = GetDefaultCodingSettingModel(ctx);
					ModelInfoManager.InsertOrUpdate<FPCodingSettingModel>(ctx, dataModel2, null);
					return dataModel2;
				}
				FPCodingSettingModel otherModel = ExamineFPCodingSettingModel(ctx, dataModel2);
				dataModel2 = CompareOperateModelAndDefaultModel(dataModel2, otherModel);
				dataModel2.MOrgID = ctx.MOrgID;
				dataModel2.MUserID = ctx.MUserID;
				ModelInfoManager.InsertOrUpdate<FPCodingSettingModel>(ctx, dataModel2, null);
				return dataModel2;
			}
			return dataModel;
		}

		public FPCodingSettingModel CompareOperateModelAndDefaultModel(FPCodingSettingModel model, FPCodingSettingModel otherModel)
		{
			FPCodingSettingModel result = model;
			if (model.MID != otherModel.MID)
			{
				string mID = model.MID;
				model = otherModel;
				model.MID = mID;
				model.IsUpdate = true;
				model.IsNew = false;
				result = model;
			}
			return result;
		}

		public FPCodingSettingModel GetDefaultCodingSettingModel(MContext ctx)
		{
			return new FPCodingSettingModel
			{
				MOrgID = ctx.MOrgID,
				MID = UUIDHelper.GetGuid(),
				MUserID = ctx.MUserID,
				MFapiaoNumber = 2,
				IsNew = true,
				MBizDate = 1,
				MPSContactName = 0,
				MInventoryName = 0,
				MContactID = 2,
				MMerItemID = 1,
				MAmount = 2,
				MTaxRate = 2,
				MTaxAmount = 2,
				MTotalAmount = 0,
				MFastCode = 0,
				MExplanation = 1,
				MTrackItem1 = 1,
				MTrackItem2 = 1,
				MTrackItem3 = 1,
				MTrackItem4 = 1,
				MTrackItem5 = 1,
				MDebitAccount = 2,
				MCreditAccount = 2,
				MTaxAccount = 2
			};
		}

		public FPCodingSettingModel ExamineFPCodingSettingModel(MContext ctx, FPCodingSettingModel model)
		{
			FPCodingSettingModel result = model;
			string mID = model.MID;
			if (model.MFapiaoNumber == 0 || model.MContactID == 0 || model.MAmount == 0 || model.MTaxRate == 0 || model.MTaxAmount == 0 || model.MDebitAccount == 0 || model.MCreditAccount == 0 || model.MTaxAccount == 0)
			{
				string str = new JavaScriptSerializer().Serialize(model);
				MLogger.Log("发票coding设置字段显示异常值:" + str, (MContext)null);
				result = GetDefaultCodingSettingModel(ctx);
			}
			return result;
		}

		public List<CommandInfo> GetDeleteFapaioCodingDateCmds(MContext ctx, List<string> fapiaoIds)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string commandText = " update t_fp_coding set MIsDelete = 1 where MOrgID = @MOrgID and  MID " + GLUtility.GetInFilterQuery(fapiaoIds, ref list, "M_ID") + " and MIsDelete = 0 ";
			List<CommandInfo> list2 = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = list.ToArray();
			list2.Add(obj);
			return list2;
		}

		public FPCodingPageModel GetCodingPageList(MContext ctx, FPFapiaoFilterModel filter)
		{
			FPCodingPageModel fPCodingPageModel = new FPCodingPageModel
			{
				Codings = new DataGridJson<FPCodingModel>(),
				BaseData = new FPUtility().GetBaseData(ctx),
				Success = true
			};
			fPCodingPageModel.Codings.total = GetCodingPageCount(ctx, filter);
			List<FPCodingModel> list = GetCodingList(ctx, filter);
			if (list.Count > 0)
			{
				list = FillCodingListWithCodingRows(ctx, list);
				FillCodingListWithBaseData(ctx, list, filter.MFapiaoCategory, fPCodingPageModel.BaseData);
				FillFastCodeWithBaseData(fPCodingPageModel.BaseData);
				FillContactWithBaseData(fPCodingPageModel.BaseData, filter.MFapiaoCategory);
			}
			foreach (FPCodingModel item in list)
			{
				if (item.MInvoiceType == 1 && item.MType == 0)
				{
					item.MTaxAccount = "";
					item.MTaxAccountName = "";
				}
			}
			fPCodingPageModel.Codings.rows = list;
			if (filter.MMaxEntryCount > 0 && fPCodingPageModel.Codings.rows.Count > filter.MMaxEntryCount)
			{
				fPCodingPageModel.Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FapiaoEntryNumberOutOfRange", "当前页发票分录数[{0}]已经超过当前浏览器设置的最大值[{1}],请在页面左下方调减单页发票数以减少总分录数!"), fPCodingPageModel.Codings.rows.Count, filter.MMaxEntryCount);
				fPCodingPageModel.Success = false;
				fPCodingPageModel.Codings.rows = new List<FPCodingModel>();
			}
			fPCodingPageModel.Codings.rows = SortCodingRows(ctx, fPCodingPageModel.Codings.rows, filter);
			return fPCodingPageModel;
		}

		private List<FPCodingModel> SortCodingRows(MContext ctx, List<FPCodingModel> rows, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = GetCodingQueryParameters(ctx, filter).ToList();
			string codingSubQuerySql = GetCodingSubQuerySql(ctx, filter, ref list, false, true);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(codingSubQuerySql, list.ToArray());
			List<FPCodingModel> list2 = new List<FPCodingModel>();
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
				{
					string id = dataSet.Tables[0].Rows[i]["MID"]?.ToString();
					list2.AddRange(from x in rows
					where x.MID == id
					select x);
				}
			}
			return list2;
		}

		public bool HasChinese(string str)
		{
			return Regex.IsMatch(str, "[\\u4e00-\\u9fa5]");
		}

		private void FillContactWithBaseData(FPBaseDataModel baseData, int category)
		{
			if (baseData.MContact == null || baseData.MContact.Count != 0)
			{
				bool isIn = category == 1;
				int i;
				for (i = 0; i < baseData.MContact.Count; i++)
				{
					if (baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail && !string.IsNullOrWhiteSpace(baseData.MTrackItem1.MCheckTypeGroupID))
					{
						BDContactsTrackLinkModel bDContactsTrackLinkModel = baseData.MTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == baseData.MContact[i].MItemID && x.MTrackID == baseData.MTrackItem1.MCheckTypeGroupID && !string.IsNullOrWhiteSpace(isIn ? x.MPurTrackId : x.MSalTrackId));
						if (bDContactsTrackLinkModel != null)
						{
							baseData.MContact[i].MTrackItem1 = (isIn ? bDContactsTrackLinkModel.MPurTrackId : bDContactsTrackLinkModel.MSalTrackId);
							baseData.MContact[i].MTrackItem1Name = (baseData.MTrackItem1.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MContact[i].MTrackItem1) ?? new GLTreeModel()).text;
						}
						if (baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail && !string.IsNullOrWhiteSpace(baseData.MTrackItem2.MCheckTypeGroupID))
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel2 = baseData.MTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == baseData.MContact[i].MItemID && x.MTrackID == baseData.MTrackItem2.MCheckTypeGroupID && !string.IsNullOrWhiteSpace(isIn ? x.MPurTrackId : x.MSalTrackId));
							if (bDContactsTrackLinkModel2 != null)
							{
								baseData.MContact[i].MTrackItem2 = (isIn ? bDContactsTrackLinkModel2.MPurTrackId : bDContactsTrackLinkModel2.MSalTrackId);
								baseData.MContact[i].MTrackItem2Name = (baseData.MTrackItem2.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MContact[i].MTrackItem2) ?? new GLTreeModel()).text;
							}
							if (baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail && !string.IsNullOrWhiteSpace(baseData.MTrackItem3.MCheckTypeGroupID))
							{
								BDContactsTrackLinkModel bDContactsTrackLinkModel3 = baseData.MTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == baseData.MContact[i].MItemID && x.MTrackID == baseData.MTrackItem3.MCheckTypeGroupID && !string.IsNullOrWhiteSpace(isIn ? x.MPurTrackId : x.MSalTrackId));
								if (bDContactsTrackLinkModel3 != null)
								{
									baseData.MContact[i].MTrackItem3 = (isIn ? bDContactsTrackLinkModel3.MPurTrackId : bDContactsTrackLinkModel3.MSalTrackId);
									baseData.MContact[i].MTrackItem3Name = (baseData.MTrackItem3.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MContact[i].MTrackItem3) ?? new GLTreeModel()).text;
								}
								if (baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail && !string.IsNullOrWhiteSpace(baseData.MTrackItem4.MCheckTypeGroupID))
								{
									BDContactsTrackLinkModel bDContactsTrackLinkModel4 = baseData.MTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == baseData.MContact[i].MItemID && x.MTrackID == baseData.MTrackItem4.MCheckTypeGroupID && !string.IsNullOrWhiteSpace(isIn ? x.MPurTrackId : x.MSalTrackId));
									if (bDContactsTrackLinkModel4 != null)
									{
										baseData.MContact[i].MTrackItem4 = (isIn ? bDContactsTrackLinkModel4.MPurTrackId : bDContactsTrackLinkModel4.MSalTrackId);
										baseData.MContact[i].MTrackItem4Name = (baseData.MTrackItem4.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MContact[i].MTrackItem4) ?? new GLTreeModel()).text;
									}
									if (baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail && !string.IsNullOrWhiteSpace(baseData.MTrackItem5.MCheckTypeGroupID))
									{
										BDContactsTrackLinkModel bDContactsTrackLinkModel5 = baseData.MTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == baseData.MContact[i].MItemID && x.MTrackID == baseData.MTrackItem5.MCheckTypeGroupID && !string.IsNullOrWhiteSpace(isIn ? x.MPurTrackId : x.MSalTrackId));
										if (bDContactsTrackLinkModel5 != null)
										{
											baseData.MContact[i].MTrackItem5 = (isIn ? bDContactsTrackLinkModel5.MPurTrackId : bDContactsTrackLinkModel5.MSalTrackId);
											baseData.MContact[i].MTrackItem5Name = (baseData.MTrackItem5.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MContact[i].MTrackItem5) ?? new GLTreeModel()).text;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void FillFastCodeWithBaseData(FPBaseDataModel baseData)
		{
			if (baseData.MFastCode != null && baseData.MFastCode.Count != 0)
			{
				int i;
				for (i = 0; i < baseData.MFastCode.Count; i++)
				{
					baseData.MFastCode[i].MMerItemIDName = (baseData.MMerItem.FirstOrDefault((BDItemModel x) => x.MItemID == baseData.MFastCode[i].MMerItemID) ?? new BDItemModel()).MText;
					baseData.MFastCode[i].MDebitAccountName = (baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == baseData.MFastCode[i].MDebitAccount) ?? new BDAccountModel()).MFullName;
					baseData.MFastCode[i].MCreditAccountName = (baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == baseData.MFastCode[i].MCreditAccount) ?? new BDAccountModel()).MFullName;
					baseData.MFastCode[i].MTaxAccountName = (baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == baseData.MFastCode[i].MTaxAccount) ?? new BDAccountModel()).MFullName;
					if (baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail)
					{
						baseData.MFastCode[i].MTrackItem1Name = (baseData.MTrackItem1.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MFastCode[i].MTrackItem1) ?? new GLTreeModel()).text;
					}
					if (baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail)
					{
						baseData.MFastCode[i].MTrackItem2Name = (baseData.MTrackItem2.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MFastCode[i].MTrackItem2) ?? new GLTreeModel()).text;
					}
					if (baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail)
					{
						baseData.MFastCode[i].MTrackItem3Name = (baseData.MTrackItem3.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MFastCode[i].MTrackItem3) ?? new GLTreeModel()).text;
					}
					if (baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail)
					{
						baseData.MFastCode[i].MTrackItem4Name = (baseData.MTrackItem4.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MFastCode[i].MTrackItem4) ?? new GLTreeModel()).text;
					}
					if (baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail)
					{
						baseData.MFastCode[i].MTrackItem5Name = (baseData.MTrackItem5.MDataList.FirstOrDefault((GLTreeModel x) => x.id == baseData.MFastCode[i].MTrackItem5) ?? new GLTreeModel()).text;
					}
				}
			}
		}

		public void FillCodingListWithBaseData(MContext ctx, List<FPCodingModel> codings, int category, FPBaseDataModel baseData)
		{
			List<BDContactsTrackLinkModel> mTrackLink = baseData.MTrackLink;
			bool flag = category == 1;
			int i;
			for (i = 0; i < codings.Count; i++)
			{
				string text = string.Empty;
				string contactName = string.IsNullOrWhiteSpace(codings[i].MContactIDName) ? codings[i].MPSContactName : codings[i].MContactIDName;
				if (!string.IsNullOrWhiteSpace(codings[i].MContactID) && !baseData.MContact.Exists((BDContactsModel x) => x.MItemID == codings[i].MContactID))
				{
					text = codings[i].MContactID;
					codings[i].MContactID = null;
				}
				if (string.IsNullOrWhiteSpace(codings[i].MContactID))
				{
					BDContactsModel bDContactsModel = baseData.MContact.FirstOrDefault((BDContactsModel x) => x.MName == contactName);
					if (bDContactsModel == null)
					{
						bDContactsModel = new BDContactsModel
						{
							MItemID = (string.IsNullOrWhiteSpace(text) ? UUIDHelper.GetGuid() : text),
							MName = contactName,
							MIsNew = true
						};
						baseData.MContact.Add(bDContactsModel);
					}
					codings[i].MContactID = bDContactsModel.MItemID;
					codings[i].MContactIDName = bDContactsModel.MName;
				}
				if (!string.IsNullOrWhiteSpace(codings[i].MContactID) && baseData.MContact != null && baseData.MContact.Count > 0)
				{
					BDContactsModel contact = baseData.MContact.FirstOrDefault((BDContactsModel x) => x.MItemID == codings[i].MContactID);
					if (contact != null)
					{
						codings[i].MContactIDName = contact.MName;
						if (string.IsNullOrWhiteSpace(codings[i].MTrackItem1) && baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail && mTrackLink != null)
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel = mTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == contact.MItemID && x.MTrackID == baseData.MTrackItem1.MCheckTypeGroupID) ?? new BDContactsTrackLinkModel();
							codings[i].MTrackItem1 = (flag ? bDContactsTrackLinkModel.MPurTrackId : bDContactsTrackLinkModel.MSalTrackId);
						}
						if (string.IsNullOrWhiteSpace(codings[i].MTrackItem2) && baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail && mTrackLink != null)
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel2 = mTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == contact.MItemID && x.MTrackID == baseData.MTrackItem2.MCheckTypeGroupID) ?? new BDContactsTrackLinkModel();
							codings[i].MTrackItem2 = (flag ? bDContactsTrackLinkModel2.MPurTrackId : bDContactsTrackLinkModel2.MSalTrackId);
						}
						if (string.IsNullOrWhiteSpace(codings[i].MTrackItem3) && baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail && mTrackLink != null)
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel3 = mTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == contact.MItemID && x.MTrackID == baseData.MTrackItem3.MCheckTypeGroupID) ?? new BDContactsTrackLinkModel();
							codings[i].MTrackItem3 = (flag ? bDContactsTrackLinkModel3.MPurTrackId : bDContactsTrackLinkModel3.MSalTrackId);
						}
						if (string.IsNullOrWhiteSpace(codings[i].MTrackItem4) && baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail && mTrackLink != null)
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel4 = mTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == contact.MItemID && x.MTrackID == baseData.MTrackItem4.MCheckTypeGroupID) ?? new BDContactsTrackLinkModel();
							codings[i].MTrackItem4 = (flag ? bDContactsTrackLinkModel4.MPurTrackId : bDContactsTrackLinkModel4.MSalTrackId);
						}
						if (string.IsNullOrWhiteSpace(codings[i].MTrackItem5) && baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail && mTrackLink != null)
						{
							BDContactsTrackLinkModel bDContactsTrackLinkModel5 = mTrackLink.FirstOrDefault((BDContactsTrackLinkModel x) => x.MContactID == contact.MItemID && x.MTrackID == baseData.MTrackItem5.MCheckTypeGroupID) ?? new BDContactsTrackLinkModel();
							codings[i].MTrackItem5 = (flag ? bDContactsTrackLinkModel5.MPurTrackId : bDContactsTrackLinkModel5.MSalTrackId);
						}
						if (!string.IsNullOrWhiteSpace(contact.MCCurrentAccountCode))
						{
							BDAccountModel bDAccountModel = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MCode == contact.MCCurrentAccountCode);
							if (bDAccountModel != null)
							{
								if (!flag)
								{
									codings[i].MDebitAccount = (string.IsNullOrWhiteSpace(codings[i].MDebitAccount) ? bDAccountModel.MItemID : codings[i].MDebitAccount);
								}
								else
								{
									codings[i].MCreditAccount = (string.IsNullOrWhiteSpace(codings[i].MCreditAccount) ? bDAccountModel.MItemID : codings[i].MCreditAccount);
								}
							}
						}
					}
				}
				string text2 = string.Empty;
				string itemName = string.IsNullOrWhiteSpace(codings[i].MMerItemIDName) ? codings[i].MInventoryName : codings[i].MMerItemIDName;
				if (!string.IsNullOrWhiteSpace(codings[i].MMerItemID) && !baseData.MMerItem.Exists((BDItemModel x) => x.MItemID == codings[i].MMerItemID))
				{
					text2 = codings[i].MMerItemID;
					codings[i].MMerItemID = null;
				}
				if (string.IsNullOrWhiteSpace(codings[i].MMerItemID))
				{
					BDItemModel bDItemModel = baseData.MMerItem.FirstOrDefault((BDItemModel x) => x.MDesc == itemName);
					if (bDItemModel == null)
					{
						bDItemModel = new BDItemModel
						{
							MItemID = (string.IsNullOrWhiteSpace(text2) ? UUIDHelper.GetGuid() : text2),
							MDesc = itemName,
							MIsNew = true
						};
						baseData.MMerItem.Add(bDItemModel);
					}
					codings[i].MMerItemID = bDItemModel.MItemID;
					codings[i].MMerItemIDName = bDItemModel.MDesc;
				}
				if (!string.IsNullOrWhiteSpace(codings[i].MMerItemID) && baseData.MMerItem != null && baseData.MMerItem.Count > 0)
				{
					BDItemModel bDItemModel2 = baseData.MMerItem.FirstOrDefault((BDItemModel x) => x.MItemID == codings[i].MMerItemID) ?? new BDItemModel();
					codings[i].MMerItemIDName = (bDItemModel2.IsNew ? bDItemModel2.MDesc : bDItemModel2.MText);
					if (!bDItemModel2.MIsExpenseItem)
					{
						string code = (!flag) ? bDItemModel2.MIncomeAccountCode : bDItemModel2.MInventoryAccountCode;
						BDAccountModel bDAccountModel2 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MCode == code) ?? new BDAccountModel();
						if (!flag)
						{
							codings[i].MCreditAccount = (string.IsNullOrWhiteSpace(codings[i].MCreditAccount) ? bDAccountModel2.MItemID : codings[i].MCreditAccount);
							codings[i].MCreditAccountName = bDAccountModel2.MFullName;
						}
						else
						{
							codings[i].MDebitAccount = (string.IsNullOrWhiteSpace(codings[i].MDebitAccount) ? bDAccountModel2.MItemID : codings[i].MDebitAccount);
							codings[i].MDebitAccountName = bDAccountModel2.MFullName;
						}
					}
					else
					{
						string code2 = (!flag) ? bDItemModel2.MIncomeAccountCode : bDItemModel2.MCostAccountCode;
						BDAccountModel bDAccountModel3 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MCode == code2) ?? new BDAccountModel();
						if (!flag)
						{
							codings[i].MCreditAccount = (string.IsNullOrWhiteSpace(codings[i].MCreditAccount) ? bDAccountModel3.MItemID : codings[i].MCreditAccount);
							codings[i].MCreditAccountName = bDAccountModel3.MFullName;
						}
						else
						{
							codings[i].MDebitAccount = (string.IsNullOrWhiteSpace(codings[i].MDebitAccount) ? bDAccountModel3.MItemID : codings[i].MDebitAccount);
							codings[i].MDebitAccountName = bDAccountModel3.MFullName;
						}
					}
				}
				if (string.IsNullOrWhiteSpace(codings[i].MTaxAccount))
				{
					REGTaxRateModel rEGTaxRateModel = baseData.MTaxRate.FirstOrDefault((REGTaxRateModel x) => x.MEffectiveTaxRate == codings[i].MTaxPercent) ?? new REGTaxRateModel();
					string code3 = flag ? IfNull(rEGTaxRateModel.MPurchaseAccountCode, "22210101") : IfNull(rEGTaxRateModel.MSaleTaxAccountCode, "22210105");
					BDAccountModel bDAccountModel4 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MCode == code3) ?? new BDAccountModel();
					codings[i].MTaxAccount = bDAccountModel4.MItemID;
					codings[i].MTaxAccountName = bDAccountModel4.MFullName;
				}
				else
				{
					BDAccountModel bDAccountModel5 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == codings[i].MTaxAccount) ?? new BDAccountModel();
					codings[i].MTaxAccount = bDAccountModel5.MItemID;
					codings[i].MTaxAccountName = bDAccountModel5.MFullName;
				}
				if (!string.IsNullOrWhiteSpace(codings[i].MDebitAccount) && string.IsNullOrWhiteSpace(codings[i].MDebitAccountName))
				{
					BDAccountModel bDAccountModel6 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == codings[i].MDebitAccount) ?? new BDAccountModel();
					codings[i].MDebitAccount = bDAccountModel6.MItemID;
					codings[i].MDebitAccountName = bDAccountModel6.MFullName;
				}
				if (!string.IsNullOrWhiteSpace(codings[i].MCreditAccount) && string.IsNullOrWhiteSpace(codings[i].MCreditAccountName))
				{
					BDAccountModel bDAccountModel7 = baseData.MAccount.FirstOrDefault((BDAccountModel x) => x.MItemID == codings[i].MCreditAccount) ?? new BDAccountModel();
					codings[i].MCreditAccount = bDAccountModel7.MItemID;
					codings[i].MCreditAccountName = bDAccountModel7.MFullName;
				}
				SetBaseDataTrack(codings[i], baseData);
			}
		}

		private void SetBaseDataTrack(FPCodingModel coding, FPBaseDataModel baseData)
		{
			if (baseData.MTrackItem1 != null && baseData.MTrackItem1.MHasDetail)
			{
				string text = string.Empty;
				string trackItem1Name = coding.MTrackItem1Name;
				GLCheckTypeDataModel mTrackItem = baseData.MTrackItem1;
				if (!string.IsNullOrWhiteSpace(coding.MTrackItem1) && !mTrackItem.MDataList.Exists((GLTreeModel x) => x.id == coding.MTrackItem1))
				{
					text = coding.MTrackItem1;
					coding.MTrackItem1 = null;
				}
				if (string.IsNullOrWhiteSpace(coding.MTrackItem1))
				{
					GLTreeModel gLTreeModel = mTrackItem.MDataList.FirstOrDefault((GLTreeModel x) => x.text == trackItem1Name);
					if (gLTreeModel == null)
					{
						if (!string.IsNullOrWhiteSpace(trackItem1Name))
						{
							gLTreeModel = new GLTreeModel
							{
								id = (string.IsNullOrWhiteSpace(text) ? UUIDHelper.GetGuid() : text),
								text = trackItem1Name,
								MIsNew = true,
								parentId = mTrackItem.MCheckTypeGroupID
							};
							mTrackItem.MDataList.Add(gLTreeModel);
						}
						else
						{
							gLTreeModel = new GLTreeModel();
						}
					}
					coding.MTrackItem1 = gLTreeModel.id;
					coding.MTrackItem1Name = gLTreeModel.text;
				}
				else
				{
					GLTreeModel gLTreeModel2 = mTrackItem.MDataList.FirstOrDefault((GLTreeModel x) => x.id == coding.MTrackItem1) ?? new GLTreeModel();
					coding.MTrackItem1 = gLTreeModel2.id;
					coding.MTrackItem1Name = gLTreeModel2.text;
				}
			}
			if (baseData.MTrackItem2 != null && baseData.MTrackItem2.MHasDetail)
			{
				string text2 = string.Empty;
				string trackItem2Name = coding.MTrackItem2Name;
				GLCheckTypeDataModel mTrackItem2 = baseData.MTrackItem2;
				if (!string.IsNullOrWhiteSpace(coding.MTrackItem2) && !mTrackItem2.MDataList.Exists((GLTreeModel x) => x.id == coding.MTrackItem2))
				{
					text2 = coding.MTrackItem2;
					coding.MTrackItem2 = null;
				}
				if (string.IsNullOrWhiteSpace(coding.MTrackItem2))
				{
					GLTreeModel gLTreeModel3 = mTrackItem2.MDataList.FirstOrDefault((GLTreeModel x) => x.text == trackItem2Name);
					if (gLTreeModel3 == null)
					{
						if (!string.IsNullOrWhiteSpace(trackItem2Name))
						{
							gLTreeModel3 = new GLTreeModel
							{
								id = (string.IsNullOrWhiteSpace(text2) ? UUIDHelper.GetGuid() : text2),
								text = trackItem2Name,
								MIsNew = true,
								parentId = mTrackItem2.MCheckTypeGroupID
							};
							mTrackItem2.MDataList.Add(gLTreeModel3);
						}
						else
						{
							gLTreeModel3 = new GLTreeModel();
						}
					}
					coding.MTrackItem2 = gLTreeModel3.id;
					coding.MTrackItem2Name = gLTreeModel3.text;
				}
				else
				{
					GLTreeModel gLTreeModel4 = mTrackItem2.MDataList.FirstOrDefault((GLTreeModel x) => x.id == coding.MTrackItem2) ?? new GLTreeModel();
					coding.MTrackItem2 = gLTreeModel4.id;
					coding.MTrackItem2Name = gLTreeModel4.text;
				}
			}
			if (baseData.MTrackItem3 != null && baseData.MTrackItem3.MHasDetail)
			{
				string text3 = string.Empty;
				string trackItem3Name = coding.MTrackItem3Name;
				GLCheckTypeDataModel mTrackItem3 = baseData.MTrackItem3;
				if (!string.IsNullOrWhiteSpace(coding.MTrackItem3) && !mTrackItem3.MDataList.Exists((GLTreeModel x) => x.id == coding.MTrackItem3))
				{
					text3 = coding.MTrackItem3;
					coding.MTrackItem3 = null;
				}
				if (string.IsNullOrWhiteSpace(coding.MTrackItem3))
				{
					GLTreeModel gLTreeModel5 = mTrackItem3.MDataList.FirstOrDefault((GLTreeModel x) => x.text == trackItem3Name);
					if (gLTreeModel5 == null)
					{
						if (!string.IsNullOrWhiteSpace(trackItem3Name))
						{
							gLTreeModel5 = new GLTreeModel
							{
								id = (string.IsNullOrWhiteSpace(text3) ? UUIDHelper.GetGuid() : text3),
								text = trackItem3Name,
								MIsNew = true,
								parentId = mTrackItem3.MCheckTypeGroupID
							};
							mTrackItem3.MDataList.Add(gLTreeModel5);
						}
						else
						{
							gLTreeModel5 = new GLTreeModel();
						}
					}
					coding.MTrackItem3 = gLTreeModel5.id;
					coding.MTrackItem3Name = gLTreeModel5.text;
				}
				else
				{
					GLTreeModel gLTreeModel6 = mTrackItem3.MDataList.FirstOrDefault((GLTreeModel x) => x.id == coding.MTrackItem3) ?? new GLTreeModel();
					coding.MTrackItem3 = gLTreeModel6.id;
					coding.MTrackItem3Name = gLTreeModel6.text;
				}
			}
			if (baseData.MTrackItem4 != null && baseData.MTrackItem4.MHasDetail)
			{
				string text4 = string.Empty;
				string trackItem4Name = coding.MTrackItem4Name;
				GLCheckTypeDataModel mTrackItem4 = baseData.MTrackItem4;
				if (!string.IsNullOrWhiteSpace(coding.MTrackItem4) && !mTrackItem4.MDataList.Exists((GLTreeModel x) => x.id == coding.MTrackItem4))
				{
					text4 = coding.MTrackItem4;
					coding.MTrackItem4 = null;
				}
				if (string.IsNullOrWhiteSpace(coding.MTrackItem4))
				{
					GLTreeModel gLTreeModel7 = mTrackItem4.MDataList.FirstOrDefault((GLTreeModel x) => x.text == trackItem4Name);
					if (gLTreeModel7 == null)
					{
						if (!string.IsNullOrWhiteSpace(trackItem4Name))
						{
							gLTreeModel7 = new GLTreeModel
							{
								id = (string.IsNullOrWhiteSpace(text4) ? UUIDHelper.GetGuid() : text4),
								text = trackItem4Name,
								MIsNew = true,
								parentId = mTrackItem4.MCheckTypeGroupID
							};
							mTrackItem4.MDataList.Add(gLTreeModel7);
						}
						else
						{
							gLTreeModel7 = new GLTreeModel();
						}
					}
					coding.MTrackItem4 = gLTreeModel7.id;
					coding.MTrackItem4Name = gLTreeModel7.text;
				}
				else
				{
					GLTreeModel gLTreeModel8 = mTrackItem4.MDataList.FirstOrDefault((GLTreeModel x) => x.id == coding.MTrackItem4) ?? new GLTreeModel();
					coding.MTrackItem4 = gLTreeModel8.id;
					coding.MTrackItem4Name = gLTreeModel8.text;
				}
			}
			if (baseData.MTrackItem5 != null && baseData.MTrackItem5.MHasDetail)
			{
				string text5 = string.Empty;
				string trackItem5Name = coding.MTrackItem5Name;
				GLCheckTypeDataModel mTrackItem5 = baseData.MTrackItem5;
				if (!string.IsNullOrWhiteSpace(coding.MTrackItem5) && !mTrackItem5.MDataList.Exists((GLTreeModel x) => x.id == coding.MTrackItem5))
				{
					text5 = coding.MTrackItem5;
					coding.MTrackItem5 = null;
				}
				if (string.IsNullOrWhiteSpace(coding.MTrackItem5))
				{
					GLTreeModel gLTreeModel9 = mTrackItem5.MDataList.FirstOrDefault((GLTreeModel x) => x.text == trackItem5Name);
					if (gLTreeModel9 == null)
					{
						if (!string.IsNullOrWhiteSpace(trackItem5Name))
						{
							gLTreeModel9 = new GLTreeModel
							{
								id = (string.IsNullOrWhiteSpace(text5) ? UUIDHelper.GetGuid() : text5),
								text = trackItem5Name,
								MIsNew = true,
								parentId = mTrackItem5.MCheckTypeGroupID
							};
							mTrackItem5.MDataList.Add(gLTreeModel9);
						}
						else
						{
							gLTreeModel9 = new GLTreeModel();
						}
					}
					coding.MTrackItem5 = gLTreeModel9.id;
					coding.MTrackItem5Name = gLTreeModel9.text;
				}
				else
				{
					GLTreeModel gLTreeModel10 = mTrackItem5.MDataList.FirstOrDefault((GLTreeModel x) => x.id == coding.MTrackItem5) ?? new GLTreeModel();
					coding.MTrackItem5 = gLTreeModel10.id;
					coding.MTrackItem5Name = gLTreeModel10.text;
				}
			}
		}

		private string IfNull(string a, string b)
		{
			return string.IsNullOrWhiteSpace(a) ? b : a;
		}

		public List<FPCodingModel> FillCodingListWithCodingRows(MContext ctx, List<FPCodingModel> codings)
		{
			List<FPCodingModel> list = new List<FPCodingModel>();
			List<FPCodingModel> codingRowList = GetCodingRowList(ctx, (from x in codings
			select x.MID).Distinct().ToList());
			string text = null;
			for (int i = 0; i < codings.Count; i++)
			{
				string mid = codings[i].MID;
				string entryID = codings[i].MEntryID;
				if (text == null || text != codings[i].MID)
				{
					codings[i].MIsTop = true;
				}
				text = mid;
				list.Add(codings[i]);
				List<FPCodingModel> list2 = (from x in codingRowList
				where x.MID == mid
				select x).ToList();
				if (list2.Count != 0)
				{
					codings[i].MFixedTaxAmount = codings[i].MTaxAmount;
					codings[i].MFixedAmount = codings[i].MAmount;
					codings[i].MFixedTotalAmount = codings[i].MTotalAmount;
					FPCodingModel fPCodingModel = list2.FirstOrDefault((FPCodingModel x) => (string.IsNullOrWhiteSpace(entryID) ? string.IsNullOrWhiteSpace(x.MEntryID) : (x.MEntryID == entryID)) && x.MIndex == 0);
					if (fPCodingModel != null)
					{
						FillCodingWithRow(codings[i], fPCodingModel);
					}
					List<FPCodingModel> list3 = (from x in list2
					where x.MIndex != 0 && (string.IsNullOrWhiteSpace(entryID) ? string.IsNullOrWhiteSpace(x.MEntryID) : (x.MEntryID == entryID))
					select x).ToList();
					if (list3 != null && list3.Count > 0)
					{
						list3 = (from x in list3
						orderby x.MIndex
						select x).ToList();
						for (int j = 0; j < list3.Count; j++)
						{
							list.Add(list3[j]);
							list3[j].MInventoryName = codings[i].MInventoryName;
							list3[j].MFapiaoNumber = codings[i].MFapiaoNumber;
							list3[j].MPSContactName = codings[i].MPSContactName;
							list3[j].MBizDate = codings[i].MBizDate;
							list3[j].MTaxPercent = codings[i].MTaxPercent;
							list3[j].MStatus = codings[i].MStatus;
							list3[j].MFixedTaxAmount = list3[j].MTaxAmount;
							list3[j].MFixedAmount = list3[j].MAmount;
							list3[j].MFixedTotalAmount = list3[j].MTotalAmount;
						}
					}
				}
			}
			return list;
		}

		private void FillCodingWithRow(FPCodingModel coding, FPCodingModel row)
		{
			coding.MID = row.MID;
			coding.MItemID = row.MItemID;
			coding.MMerItemID = row.MMerItemID;
			coding.MContactID = row.MContactID;
			coding.MTaxAmount = row.MTaxAmount;
			coding.MAmount = row.MAmount;
			coding.MTaxAmount = row.MTaxAmount;
			coding.MTotalAmount = row.MTotalAmount;
			coding.MTrackItem1 = row.MTrackItem1;
			coding.MTrackItem2 = row.MTrackItem2;
			coding.MTrackItem3 = row.MTrackItem3;
			coding.MTrackItem4 = row.MTrackItem4;
			coding.MTrackItem5 = row.MTrackItem5;
			coding.MIndex = row.MIndex;
			coding.MTaxAccount = row.MTaxAccount;
			coding.MDebitAccount = row.MDebitAccount;
			coding.MCreditAccount = row.MCreditAccount;
			coding.MExplanation = row.MExplanation;
			coding.MTaxRate = row.MTaxRate;
			coding.MTrackItem1Name = row.MTrackItem1Name;
			coding.MTrackItem2Name = row.MTrackItem2Name;
			coding.MTrackItem3Name = row.MTrackItem3Name;
			coding.MTrackItem4Name = row.MTrackItem4Name;
			coding.MTrackItem5Name = row.MTrackItem5Name;
			coding.MMerItemIDName = row.MMerItemIDName;
			coding.MContactIDName = row.MContactIDName;
		}

		public int GetCodingPageCount(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = GetCodingQueryParameters(ctx, filter).ToList();
			string codingSubQuerySql = GetCodingSubQuerySql(ctx, filter, ref list, true, false);
			return int.Parse(new DynamicDbHelperMySQL(ctx).GetSingle(codingSubQuerySql, list.ToArray()).ToString());
		}

		public List<FPCodingModel> GetCodingList(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = GetCodingQueryParameters(ctx, filter).ToList();
			string codingQuerySql = GetCodingQuerySql(ctx, filter, ref list);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(codingQuerySql, list.ToArray());
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return new List<FPCodingModel>();
			}
			return ModelInfoManager.DataTableToList<FPCodingModel>(dataSet);
		}

		public string GetCodingQuerySql(MContext ctx, FPFapiaoFilterModel filter, ref List<MySqlParameter> parameters)
		{
			string format = "SELECT\r\n                                t1.MNumber AS MFapiaoNumber,\n                                t1.MID,\n                                t1.MBizDate,\n                                t1.MStatus,\n                                CASE\n                                    WHEN @MFapiaoCategory = 0 THEN t1.MPContactName\n                                    ELSE t1.MSContactName\n                                END AS MPSContactName,\n                                t1.MContactID,\n                                t1.MInvoiceType,\n                                t1.MType,\n                                t2.MEntryID,\n                                t2.MItemID AS MMerItemID,\n                                t2.MItemName AS MInventoryName,\n                                t2.MTaxPercent,\n                                t3.MItemID AS MTaxRate,\n                                (case when t2.MEntryID is null then t1.MAmount else t2.MAmount end) as MAmount,\n                                (case when t2.MEntryID is null then t1.MTaxAmount else t2.MTaxAmount end) as MTaxAmount,\n                                (case when t2.MEntryID is null then t1.MTotalAmount else t2.MTotalAmount end) as MTotalAmount,\n                                (case when t2.MEntryID is null then t1.MAmount else t2.MAmount end) as MFixedAmount,\n                                (case when t2.MEntryID is null then t1.MTaxAmount else t2.MTaxAmount end) as MFixedTaxAmount,\n                                (case when t2.MEntryID is null then t1.MTotalAmount else t2.MTotalAmount end) as MFixedTotalAmount\n                            FROM\n                                ({0}) t1\n                                    LEFT JOIN\n                                t_fp_fapiaoentry t2 ON t1.MID = t2.MID\n                                    AND t1.MOrgID = t2.MOrgID\n                                    AND t1.MIsDelete = t2.MIsDelete\n                                    LEFT JOIN\r\n                                    (\r\n                                SELECT a.* FROM t_Reg_taxrate a WHERE a.MOrgID = @MOrgID AND a.MIsDelete = 0 AND\r\n                                NOT EXISTS(SELECT 1 FROM t_Reg_taxrate b WHERE b.morgid = @MOrgID AND b.MIsDelete = 0\r\n                                AND a.MOrgID = b.MOrgID AND a.MEffectiveTaxRate = b.MEffectiveTaxRate AND a.mcreatedate > b.mcreatedate))\r\n                                 t3 ON t3.MEffectiveTaxRate = t2.MTaxPercent\n                                 AND t3.MOrgID = t1.MOrgID\n                                 AND t3.MIsDelete = t1.MIsDelete\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 ";
			string codingSubQuerySql = GetCodingSubQuerySql(ctx, filter, ref parameters, false, false);
			return string.Format(format, codingSubQuerySql);
		}

		public string GetCodingSubQuerySql(MContext ctx, FPFapiaoFilterModel filter, ref List<MySqlParameter> parameters, bool count, bool getID = false)
		{
			string str = "SELECT\r\n                               " + (count ? "count(t1.MID) AS MCount " : (getID ? " t1.MID " : " t1.* ")) + "\r\n                            FROM\n                                t_fp_fapiao t1\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                    AND t1.MInvoiceType = @MFapiaoCategory\n                                    AND t1.MCodingStatus = 0\n                                    AND t1.MStatus IN (1 , 2, 3, 4)\n                ";
			if (filter.MStartDate > new DateTime(1900, 1, 1))
			{
				str += " and t1.MBizDate >= @MStartDate ";
			}
			if (filter.MEndDate > new DateTime(1900, 1, 1))
			{
				str += " and t1.MBizDate <= @MEndDate ";
			}
			if (!string.IsNullOrWhiteSpace(filter.MKeyword))
			{
				str += " and (";
				str += "  t1.M{0}ContactName like concat('%',@MKeyword,'%') or t1.MNumber like concat('%',@MKeyword,'%') ";
				if (DateTime.TryParse(filter.MKeyword, out DateTime dateTime))
				{
					str += " OR date(t1.MBizDate) = @KeywordDate ";
					parameters.Add(new MySqlParameter("@KeywordDate", dateTime));
				}
				if (filter.MTotalAmount.HasValue)
				{
					str += " or t1.MTotalAmount = @MTotalAmount or t1.MTaxAmount = @MTotalAmount or t1.MAmount = @MTotalAmount\r\n                                OR EXISTS(SELECT\r\n                                        1\n                                    FROM\n                                        t_fp_fapiaoentry t2\n                                    WHERE\n                                        t2.MOrgID = t1.MOrgID\n                                            AND t2.MID = t1.MID\n                                            and t1.MIsDelete = t2.MIsDelete\n                                            AND (t2.MTotalAmount = @MTotalAmount\n                                            OR t2.MTaxAmount = @MTotalAmount\n                                            OR t2.MAmount = @MTotalAmount))";
				}
				str += " ) ";
			}
			string text = string.IsNullOrWhiteSpace(filter.Sort) ? "MBizDate" : filter.Sort;
			string text2 = string.IsNullOrWhiteSpace(filter.Order) ? " desc" : filter.Order;
			str += GetOrderString(filter.Sort, filter.Order, (filter.MFapiaoCategory == 1) ? "S" : "P", "");
			str += ((filter.rows > 0 && !count) ? (" limit " + (filter.page - 1) * filter.rows + "," + filter.rows) : "");
			return string.Format(str, (filter.MFapiaoCategory == 1) ? "S" : "P");
		}

		private string GetOrderString(string field, string orderBy, string contactType, string prefix = "")
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string text = "MBizDate";
			string text2 = "MNumber";
			string text3 = "M" + contactType + "ContactName";
			if (field == text)
			{
				empty = prefix + text;
				empty2 = prefix + text2;
				empty3 = "convert(" + prefix + text3 + " using gbk)";
				goto IL_00e6;
			}
			if (field == text2)
			{
				empty = prefix + text2;
				empty2 = prefix + text;
				empty3 = "convert(" + prefix + text3 + " using gbk)";
				goto IL_00e6;
			}
			if (field == text3)
			{
				empty = "convert(" + prefix + text3 + " using gbk)";
				empty2 = prefix + text2;
				empty3 = prefix + text;
				goto IL_00e6;
			}
			return "";
			IL_00e6:
			return " order by " + empty + " " + orderBy + ", " + empty2 + " " + orderBy + ", " + empty3 + " " + orderBy + " ";
		}

		public List<FPCodingModel> GetCodingRowList(MContext ctx, List<string> fapiaoIDs)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "select * from t_fp_coding where MOrgID = @MOrgID and MIsDelete = 0 and  MID " + GLUtility.GetInFilterQuery(fapiaoIDs, ref list, "M_ID");
			return ModelInfoManager.GetDataModelBySql<FPCodingModel>(ctx, sql, list.ToArray());
		}

		public MySqlParameter[] GetCodingQueryParameters(MContext ctx, FPFapiaoFilterModel filter)
		{
			List<MySqlParameter> list = new MySqlParameter[5]
			{
				new MySqlParameter("@MTotalAmount", filter.MTotalAmount.HasValue ? filter.MTotalAmount.Value : decimal.Zero),
				new MySqlParameter("@MKeyword", filter.MKeyword),
				new MySqlParameter("@MStartDate", filter.MStartDate),
				new MySqlParameter("@MEndDate", filter.MEndDate),
				new MySqlParameter("@MFapiaoCategory", filter.MFapiaoCategory)
			}.ToList();
			list.AddRange(ctx.GetParameters((MySqlParameter)null));
			return list.ToArray();
		}

		public List<CommandInfo> GetDeleteFapiaoTableReconcileCmds(MContext ctx, List<string> fapiaoIDs)
		{
			if (fapiaoIDs == null || fapiaoIDs.Count == 0)
			{
				return new List<CommandInfo>();
			}
			if (ctx.MOrgVersionID == 1)
			{
				return new List<CommandInfo>();
			}
			List<FPTableModel> tables = new FPTableRepository().GetTableListByFapiaoIds(ctx, fapiaoIDs);
			if (tables == null || tables.Count == 0)
			{
				return new List<CommandInfo>();
			}
			List<FPFapiaoModel> fapiaoListByTableIds = GetFapiaoListByTableIds(ctx, (from x in tables
			select x.MItemID).Distinct().ToList());
			if (fapiaoListByTableIds == null || fapiaoListByTableIds.Count == 0)
			{
				return new List<CommandInfo>();
			}
			FPTableRepository fPTableRepository = new FPTableRepository();
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			int i;
			for (i = 0; i < tables.Count; i++)
			{
				List<FPFapiaoModel> fapiaos = (from x in fapiaoListByTableIds
				where x.MTableID == tables[i].MItemID && !fapiaoIDs.Contains(x.MID)
				select x).ToList();
				fPTableRepository.UpdateTableIssueStatus(tables[i], fapiaos);
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPTableModel>(ctx, tables[i], new List<string>
				{
					"MIssueStatus",
					"MRAmount",
					"MRTaxAmount",
					"MRTotalAmount"
				}, true));
				list2.AddRange(FPTableLogHelper.GetRemoveReoncileLogCmd(ctx, new FPFapiaoReconcileModel
				{
					MTable = new FPTableViewModel
					{
						MItemID = tables[i].MItemID
					},
					MFapiaoList = (from x in fapiaoListByTableIds
					where x.MTableID == tables[i].MItemID && fapiaoIDs.Contains(x.MID)
					select x).ToList()
				}));
			}
			List<CommandInfo> deleteReconcileCmds = fPTableRepository.GetDeleteReconcileCmds(ctx, fapiaoIDs, null);
			list.AddRange(deleteReconcileCmds);
			list.AddRange(list2);
			return list;
		}

		internal void OperateVerifyType(FPFapiaoModel model)
		{
			model.MVerifyDate = model.MDeductionDate;
			if (!model.MVerifyDate.IsEmpty())
			{
				model.MVerifyType = 2;
			}
			else
			{
				model.MVerifyType = 0;
			}
		}
	}
}
