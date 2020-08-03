using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.BAS
{
	public class BASOrgInitSettingRepository : DataServiceT<BASOrgInitSettingModel>
	{
		private Dictionary<string, string> _presetAcctCheckGroupIDList = null;

		public Dictionary<string, string> PresetAcctCheckGroupIDList
		{
			get
			{
				if (_presetAcctCheckGroupIDList == null)
				{
					_presetAcctCheckGroupIDList = new Dictionary<string, string>();
					_presetAcctCheckGroupIDList.Add("1122", "1");
					_presetAcctCheckGroupIDList.Add("2203", "1");
					_presetAcctCheckGroupIDList.Add("2202", "1");
					_presetAcctCheckGroupIDList.Add("1123", "1");
					_presetAcctCheckGroupIDList.Add("1221", "2");
					_presetAcctCheckGroupIDList.Add("2241", "4");
				}
				return _presetAcctCheckGroupIDList;
			}
		}

		public OperationResult UpdateInitSettingModel(MContext ctx, BASOrgInitSettingModel model, string fields = null)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			try
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BASOrgInitSettingModel>(ctx, model, null, true));
				MySqlParameter[] array = new MySqlParameter[1]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				};
				array[0].Value = ctx.MOrgID;
				List<CommandInfo> list3 = list2;
				CommandInfo obj = new CommandInfo
				{
					CommandText = "update t_bd_account_l set MIsDelete = 1 where MParentID in (select MItemID from t_bd_account where MOrgID=@MOrgID and MIsDelete = 0 and MIsActive = 1) and MIsDelete = 0 "
				};
				DbParameter[] array2 = obj.Parameters = array;
				list3.Add(obj);
				List<CommandInfo> list4 = list2;
				CommandInfo obj2 = new CommandInfo
				{
					CommandText = "update t_bd_account set MIsDelete = 1 where MOrgID=@MOrgID and MIsDelete = 0  "
				};
				array2 = (obj2.Parameters = array);
				list4.Add(obj2);
				CommandInfo commandInfo = new CommandInfo();
				commandInfo.CommandText = "update t_iv_invoiceentry set MIsDelete=1 where MID in \r\n                                                               (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo.Parameters = array);
				list2.Add(commandInfo);
				CommandInfo commandInfo2 = new CommandInfo();
				commandInfo2.CommandText = "update t_iv_invoice set MIsDelete=1 where MID in \r\n                                                          (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo2.Parameters = array);
				list2.Add(commandInfo2);
				CommandInfo commandInfo3 = new CommandInfo();
				commandInfo3.CommandText = "update t_iv_receiveentry set MIsDelete=1 where MID in \r\n                                                               (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo3.Parameters = array);
				list2.Add(commandInfo3);
				CommandInfo commandInfo4 = new CommandInfo();
				commandInfo4.CommandText = "update t_iv_receive set MIsDelete=1 where MID in \r\n                                                               (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo4.Parameters = array);
				list2.Add(commandInfo4);
				CommandInfo commandInfo5 = new CommandInfo();
				commandInfo5.CommandText = "update t_iv_paymententry set MIsDelete=1 where MID in (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo5.Parameters = array);
				list2.Add(commandInfo5);
				CommandInfo commandInfo6 = new CommandInfo();
				commandInfo6.CommandText = "update t_iv_payment set MIsDelete=1 where MID in (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo6.Parameters = array);
				list2.Add(commandInfo6);
				CommandInfo commandInfo7 = new CommandInfo();
				commandInfo7.CommandText = "update t_iv_expenseentry set MIsDelete=1 where MID in (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo7.Parameters = array);
				list2.Add(commandInfo7);
				CommandInfo commandInfo8 = new CommandInfo();
				commandInfo8.CommandText = "update t_iv_expense set MIsDelete=1 where MID in (select MBillID from t_gl_initbalance where MOrgID=@MOrgID and MIsDelete=0 ) and MIsDelete=0";
				array2 = (commandInfo8.Parameters = array);
				list2.Add(commandInfo8);
				list2.AddRange(GetDeleteBaiscSettingAccountInfo(ctx));
				List<CommandInfo> list5 = list2;
				CommandInfo obj3 = new CommandInfo
				{
					CommandText = "update t_gl_initbalance set MIsDelete = 1 where MOrgID=@MOrgID and MIsDelete = 0 "
				};
				array2 = (obj3.Parameters = array);
				list5.Add(obj3);
				List<CommandInfo> list6 = list2;
				CommandInfo obj4 = new CommandInfo
				{
					CommandText = "update t_bd_bankaccount set MAccountID=null where MOrgID=@MOrgID  and MIsDelete = 0 "
				};
				array2 = (obj4.Parameters = array);
				list6.Add(obj4);
				List<CommandInfo> list7 = list2;
				CommandInfo obj5 = new CommandInfo
				{
					CommandText = "update t_bd_accountmatchlog set MIsDelete = 1 where MOrgID=@MOrgID and MIsDelete = 0 "
				};
				array2 = (obj5.Parameters = array);
				list7.Add(obj5);
				if (model.MAccountingStandard != "3")
				{
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.Equal("MOrgID", "0").Equal("MAccountTableID", model.MAccountingStandard);
					List<BDAccountModel> dataModelList = ModelInfoManager.GetDataModelList<BDAccountModel>(ctx, sqlWhere, false, false);
					List<NameValueModel> accountFullNameMapping = GetAccountFullNameMapping(ctx);
					UpdateAccountList(ctx, dataModelList, accountFullNameMapping, null, 7000);
					list2.AddRange(GetAcctInsertCmds(ctx, dataModelList, accountFullNameMapping));
				}
				MultiDBCommand[] cmdArray = new MultiDBCommand[2]
				{
					new MultiDBCommand(ctx)
					{
						CommandList = list,
						DBType = SysOrBas.Sys
					},
					new MultiDBCommand(ctx)
					{
						CommandList = list2,
						DBType = SysOrBas.Bas
					}
				};
				operationResult.Success = DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray);
				ctx.MAccountTableID = model.MAccountingStandard;
				ctx.MGLBeginDate = model.MConversionDate;
				ContextHelper.MContext = ctx;
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
			}
			return operationResult;
		}

		private List<CommandInfo> GetDeleteBaiscSettingAccountInfo(MContext ctx)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] array = new MySqlParameter[1]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = " update t_bd_contacts set MCCurrentAccountCode='' where MOrgID=@MOrgID";
			DbParameter[] array2 = commandInfo.Parameters = array;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = " update t_bd_employees set MCurrentAccountCode='' , MExpenseAccountCode='' where MOrgID=@MOrgID ";
			array2 = (commandInfo2.Parameters = array);
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update t_bd_item set MInventoryAccountCode='' , MIncomeAccountCode='' , MCostAccountCode='' where MOrgID=@MOrgID ";
			array2 = (commandInfo3.Parameters = array);
			list.Add(commandInfo3);
			CommandInfo commandInfo4 = new CommandInfo();
			commandInfo4.CommandText = "update t_bd_expenseitem set MAccountCode='' where MOrgID=@MOrgID ";
			array2 = (commandInfo4.Parameters = array);
			list.Add(commandInfo4);
			CommandInfo commandInfo5 = new CommandInfo();
			commandInfo5.CommandText = "update t_pa_payitem set MAccountCode='' where MOrgID=@MOrgID ";
			array2 = (commandInfo5.Parameters = array);
			list.Add(commandInfo5);
			CommandInfo commandInfo6 = new CommandInfo();
			commandInfo6.CommandText = "update t_pa_payitemgroup set MAccountCode='' where MOrgID=@MOrgID";
			array2 = (commandInfo6.Parameters = array);
			list.Add(commandInfo6);
			return list;
		}

		private List<NameValueModel> GetAccountFullNameMapping(MContext ctx)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			string sql = "select MPKID as MName, MFullName as MValue from T_BD_Account_L where MOrgID='0' and MIsDelete=0 and MParentID in (select MItemID from t_bd_account where MOrgID='0' and MIsDelete=0 and MAccountTableId in ('1','2'))";
			return ModelInfoManager.GetDataModelBySql<NameValueModel>(ctx, sql, (MySqlParameter[])null);
		}

		public List<CommandInfo> GetAcctInsertCmds(MContext ctx, List<BDAccountModel> acctList, List<NameValueModel> fullNameList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_bd_account(MItemID,MOrgID,MCode,MNumber,MParentID,MAccountTypeID,MAccountGroupID,MAccountTableID,MDC,MIsCheckForCurrency,MIsSys,MIsActive,MIsDelete,IsCanRelateContact,MCheckGroupID,MCreateDate,MCreatorID,MModifyDate,MModifierID)";
			string format = " SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},{11},{12},{13},'{16}','{14}','{15}','{14}','{15}'";
			string value2 = "INSERT INTO t_bd_account_l(MPKID,MParentID,MLocaleID,MName,MOrgID,MFullName)";
			string format2 = "SELECT '{0}','{1}','{2}','{3}','{4}','{5}'";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			foreach (BDAccountModel acct in acctList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				stringBuilder.AppendFormat(format, acct.MItemID, acct.MOrgID, acct.MCode, acct.MNumber, acct.MParentID, acct.MAccountTypeID, acct.MAccountGroupID, acct.MAccountTableID, acct.MDC, acct.MIsCheckForCurrency, acct.MIsSys, acct.MIsActive, acct.MIsDelete, acct.IsCanRelateContact, ctx.DateNowString, ctx.MUserID, acct.MCheckGroupID);
				MultiLanguageFieldList multiLanguageFieldList = acct.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					string mValue = fullNameList.SingleOrDefault((NameValueModel f) => f.MTag == item.MPKID).MValue;
					stringBuilder2.AppendFormat(format2, item.MPKID, multiLanguageFieldList.MParentID, item.MLocaleID, item.MValue, ctx.MOrgID, mValue);
					num2++;
				}
				num++;
			}
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			});
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			});
			return list;
		}

		public OperationResult GLSetupSuccess(MContext ctx)
		{
			bool success = false;
			string message = string.Empty;
			try
			{
				List<CommandInfo> list = new List<CommandInfo>();
				List<CommandInfo> list2 = new List<CommandInfo>();
				string roleModelId = GetRoleModelId(ctx);
				SqlWhere sqlWhere = new SqlWhere().In("MItemID", new string[2]
				{
					"General_Ledger",
					"Other_Reports"
				});
				List<CommandInfo> collection = SECPermissionRepository.AddAllPermissionToRole(ctx, string.Empty, roleModelId, sqlWhere, ModuleEnum.GL);
				list.AddRange(collection);
				list2.AddRange(collection);
				List<string> list3 = new List<string>();
				List<CommandInfo> sysAdminAddGLPermissionCmds = GetSysAdminAddGLPermissionCmds(ctx, out list3, roleModelId, "General_Ledger");
				list.AddRange(sysAdminAddGLPermissionCmds);
				list2.AddRange(sysAdminAddGLPermissionCmds);
				MultiDBCommand[] cmdArray = new MultiDBCommand[2]
				{
					new MultiDBCommand(ctx)
					{
						CommandList = list,
						DBType = SysOrBas.Sys
					},
					new MultiDBCommand(ctx)
					{
						CommandList = list2,
						DBType = SysOrBas.Bas
					}
				};
				success = DbHelperMySQL.ExecuteSqlTran(ctx, cmdArray);
				SECPermissionRepository.GetUserPermission(ctx, "");
				foreach (string item in list3)
				{
					SECPermissionRepository.GetUserPermission(ctx, item);
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}
			return new OperationResult
			{
				Success = success,
				Message = message
			};
		}

		public List<CommandInfo> GetSysAdminAddGLPermissionCmds(MContext ctx, out List<string> useList, string excludeRoleId = null, string module = "General_Ledger")
		{
			useList = new List<string>();
			List<CommandInfo> list = new List<CommandInfo>();
			SECUserPermissionListFilterModel sECUserPermissionListFilterModel = new SECUserPermissionListFilterModel();
			sECUserPermissionListFilterModel.page = 1;
			sECUserPermissionListFilterModel.PageSize = 2147483647;
			List<SECUserPermissionListModel> rows = SECPermissionRepository.GetUserPermissionPageList(ctx, sECUserPermissionListFilterModel).rows;
			if (rows != null && rows.Count() > 0)
			{
				SECRoleUserRepository sECRoleUserRepository = new SECRoleUserRepository();
				List<SECUserPermissionListModel> list2 = (from x in rows
				where x.MRole == "Admin"
				select x).ToList();
				if (list2 != null || list2.Count() > 0)
				{
					foreach (SECUserPermissionListModel item in list2)
					{
						SqlWhere sqlWhere = new SqlWhere();
						sqlWhere.Equal("MUserID", item.MItemID);
						sqlWhere.Equal("MOrgID", ctx.MOrgID);
						sqlWhere.Equal("MIsDelete", 0);
						sqlWhere.Equal("MIsActive", 1);
						SECRoleUserModel dataModelByFilter = sECRoleUserRepository.GetDataModelByFilter(ctx, sqlWhere);
						if (dataModelByFilter != null && (string.IsNullOrWhiteSpace(excludeRoleId) || !(dataModelByFilter.MRoleID == excludeRoleId)))
						{
							SqlWhere sqlWhere2 = new SqlWhere();
							if (module == "General_Ledger")
							{
								sqlWhere2.In("MItemID", new string[2]
								{
									"General_Ledger",
									"Other_Reports"
								});
							}
							else if (module == "Fixed_Assets")
							{
								sqlWhere2.In("MItemID", new string[2]
								{
									"Fixed_Assets",
									"Fixed_Assets_Reports"
								});
							}
							List<CommandInfo> list3 = SECPermissionRepository.AddAllPermissionToRole(ctx, string.Empty, dataModelByFilter.MRoleID, sqlWhere2, ModuleEnum.GL);
							if (list3 != null)
							{
								useList.Add(item.MItemID);
								list.AddRange(list3);
							}
						}
					}
				}
			}
			return list;
		}

		public string GetRoleModelId(MContext ctx)
		{
			string sql = "select ru.MRoleID from t_sec_roleuser ru \r\n                            inner join t_sec_role r on ru.MRoleID=r.MitemID and r.MIsDelete = 0 and r.MIsActive = 1\r\n                            where r.MOrgID=@OrgID and ru.MUserID=@UserID and ru.MIsDelete = 0 and ru.MIsActive = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@OrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@UserID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MUserID
				}
			};
			List<string> list = (from ru in ModelInfoManager.GetDataModelBySql<SECRoleUserModel>(ctx, sql, cmdParms)
			select ru.MRoleID).ToList();
			if (list.Count > 0)
			{
				return list[0];
			}
			return string.Empty;
		}

		private void SetDefaultCheckGroupID(BDAccountModel model)
		{
			if (PresetAcctCheckGroupIDList.ContainsKey(model.MNumber))
			{
				model.MCheckGroupID = PresetAcctCheckGroupIDList[model.MNumber];
			}
			else
			{
				model.MCheckGroupID = "0";
			}
		}

		public void UpdateAccountList(MContext ctx, IEnumerable<BDAccountModel> allAcctList, List<NameValueModel> fullNameList, IEnumerable<BDAccountModel> acctList = null, int startCode = 7000)
		{
			string tmpId = string.Empty;
			IEnumerable<BDAccountModel> enumerable = acctList ?? allAcctList;
			if (fullNameList == null || !fullNameList.Any())
			{
				fullNameList = GetAccountFullNameMapping(ctx);
			}
			foreach (BDAccountModel item in enumerable)
			{
				startCode++;
				if (!item.IsNew)
				{
					tmpId = item.MItemID;
					item.MItemID = UUIDHelper.GetGuid();
					item.IsNew = true;
					item.MOrgID = ctx.MOrgID;
					SetDefaultCheckGroupID(item);
					if (string.IsNullOrWhiteSpace(item.MCode) && ctx.MAccountTableID == "3")
					{
						item.MCode = Convert.ToString(startCode);
					}
					MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
					multiLanguageFieldList.MParentID = item.MItemID;
					foreach (MultiLanguageField item2 in multiLanguageFieldList.MMultiLanguageField)
					{
						string guid = UUIDHelper.GetGuid();
						fullNameList.SingleOrDefault((NameValueModel f) => f.MName == item2.MPKID).MTag = guid;
						item2.MPKID = guid;
					}
					IEnumerable<BDAccountModel> enumerable2 = from f in allAcctList
					where f.MParentID == tmpId
					select f;
					if (enumerable2.Any())
					{
						foreach (BDAccountModel item3 in enumerable2)
						{
							item3.MParentID = item.MItemID;
						}
						int startCode2 = startCode * 100;
						UpdateAccountList(ctx, allAcctList, fullNameList, from f in allAcctList
						where f.MParentID == item.MItemID
						select f, startCode2);
					}
				}
			}
		}
	}
}
