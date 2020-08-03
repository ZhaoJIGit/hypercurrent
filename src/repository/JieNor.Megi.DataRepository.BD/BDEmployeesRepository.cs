using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PA;
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

namespace JieNor.Megi.DataRepository.BD
{
	public class BDEmployeesRepository : DataServiceT<BDEmployeesModel>
	{
		private string multLangFieldSql = "\r\n            ,t2.MFirstName{0}\n            ,t2.MLastName{0}\n\t        ,t2.MBankAccName{0}\n\t        ,t2.MBankName{0} ";

		private readonly string CommonSelect = string.Format("SELECT \n                t1.MItemID,\n                t1.MItemID as MEmployeeID,\n                t1.MEmail,\n                t1.MSex,\n                t4.MNumber as MCurrentAccountCode,\n                t1.MDefaultCyID,\n\t            t1.MPurDueDate as MPaymentTerms_MExpense_MDay,\n\t            t1.MPurDueCondition as MPaymentTerms_MExpense_MDayType,\n\t            t1.MStatus,\n\t            t1.MIsActive,\n                t1.MModifyDate,\n\t            CONVERT(AES_DECRYPT(t1.MBankAcctNo, '{0}') USING UTF8) AS MBankAcctNo,\n                t2.MFirstName,\n                t2.MLastName,\n\t            t2.MBankAccName,\n\t            t2.MBankName,\n                t3.MEmailAddress AS MUserEmailAddress\n                #_#lang_field0#_#   \n            FROM\n                T_BD_Employees t1\n                    LEFT JOIN\n                @_@t_bd_employees_l@_@ t2 ON t1.MOrgID = t2.MOrgID\n                    and t2.MParentID = t1.MItemID\n                    and t2.MIsDelete = 0\n                    LEFT JOIN\n                T_Sec_User t3 ON t3.MItemID = t1.MUserID\n                    and t3.MIsDelete = 0\r\n                    LEFT JOIN\r\n                t_bd_account t4 ON t4.MOrgID = t1.MOrgID\r\n                    AND t4.MCode = t1.MCurrentAccountCode\r\n                    AND t4.MIsDelete = 0\n            WHERE\n                t1.MIsDelete = 0\n                    and t1.MOrgID = @MOrgID ", "JieNor-001");

		public DataGridJson<BDEmployeesModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<BDEmployeesModel>(ctx, param, CommonSelect, multLangFieldSql, false, true, null);
		}

		public OperationResult Post(MContext ctx, List<BDEmployeesModel> list)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list2 = new List<CommandInfo>();
				PAPaySettingModel paySettingModel = PASalaryPaymentRepository.GetPaySettingModel(ctx);
				foreach (BDEmployeesModel item in list)
				{
					SetSSHFAmountByCompanyPer(paySettingModel, item);
					MultiLanguageAdd(item);
					List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, item, item.UpdateFieldList, true);
					list2.AddRange(insertOrUpdateCmd);
					BDPayrollDetailModel bDPayrollDetailModel = new BDPayrollDetailModel();
					bDPayrollDetailModel.MEmployeeID = item.MItemID;
					ModelPropertyHelper.CopyModelValue(item, bDPayrollDetailModel);
					SetPayrollAmountByPercent(bDPayrollDetailModel);
					list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDPayrollDetailModel>(ctx, bDPayrollDetailModel, null, true));
				}
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(list2);
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

		public List<BDEmployeesListModel> GetBDEmployeesList(MContext ctx, string searchFilter, bool includeDisable = false)
		{
			StringBuilder listSqlBuilder = GetListSqlBuilder(searchFilter, false, includeDisable);
			if (ctx.MLCID == LangCodeEnum.EN_US)
			{
				listSqlBuilder.AppendLine(" order by MFirstName,MLastName");
			}
			else
			{
				listSqlBuilder.AppendLine(" order by MLastName,MFirstName");
			}
			MySqlParameter[] listParameters = GetListParameters(ctx, searchFilter);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<BDEmployeesListModel> list = ModelInfoManager.DataTableToList<BDEmployeesListModel>(dynamicDbHelperMySQL.Query(listSqlBuilder.ToString(), listParameters).Tables[0]);
			foreach (BDEmployeesListModel item in list)
			{
				item.MName = GlobalFormat.GetUserName(item.MFirstName, item.MLastName, ctx);
			}
			return list;
		}

		private string GetEmployeeOrderFields(MContext ctx, string aliasName)
		{
			return string.Format("convert(F_GetUserName({0}.MFirstName,{0}.MLastName) using gbk)", aliasName);
		}

		public DataGridJson<BDEmployeesListModel> GetBDEmployeesPageList(MContext ctx, BDEmployeesListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder empListSqlBulider = GetEmpListSqlBulider(filter);
			MySqlParameter[] listParameters = GetListParameters(ctx, filter);
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = empListSqlBulider.ToString();
			string employeeOrderFields = GetEmployeeOrderFields(ctx, "t2");
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				if (filter.Sort == "MName")
				{
					sqlQuery.AddOrderBy(employeeOrderFields, SqlOrderDir.Asc);
				}
				else
				{
					sqlQuery.AddOrderBy($" {filter.Sort} {filter.Order} ", SqlOrderDir.Asc);
				}
			}
			else
			{
				sqlQuery.AddOrderBy(employeeOrderFields, SqlOrderDir.Asc);
			}
			MySqlParameter[] array = listParameters;
			foreach (MySqlParameter para in array)
			{
				sqlQuery.AddParameter(para);
			}
			DataGridJson<BDEmployeesListModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<BDEmployeesListModel>(ctx, sqlQuery);
			List<PAPITThresholdModel> source = null;
			List<PAPITThresholdModel> list = null;
			if (filter.IsFromExport)
			{
				source = PAPITRepository.GetPITThresholdList(ctx, new PAPITThresholdFilterModel());
			}
			foreach (BDEmployeesListModel row in pageDataModelListBySql.rows)
			{
				row.MName = GlobalFormat.GetUserName(row.MFirstName, row.MLastName, ctx);
				if (filter.IsFromExport)
				{
					list = (from f in source
					where f.MEmployeeID == row.MItemID
					select f).ToList();
					row.MIncomeTaxThresholdNew = list[0].MAmount;
					row.MIncomeTaxThreshold = list[1].MAmount;
				}
			}
			return pageDataModelListBySql;
		}

		private StringBuilder GetListSqlBuilder(string searchFilter, bool isFromExport = false, bool includeDisable = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select distinct t2.MFirstName,t2.MLastName,t1.MSex,t1.MStatus,t1.MEmail,t1.MItemID,t1.MPhone,t1.MUserID, ");
			stringBuilder.AppendFormat(" convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo,t3.MEmailAddress as MLinkUser, t1.MCurrentAccountCode, t4.MItemID as MCurrentAccountID ", "JieNor-001");
			if (isFromExport)
			{
				stringBuilder.AppendLine(" ,t2.MPAttention, t2.MPStreet, t2.MPRegion,t2.MRealAttention,t2.MRealStreet,t2.MRealRegion,t2.MBankAccName,t2.MBankName, ");
				stringBuilder.AppendLine(" F_GetUserName(t2.MFirstName,t2.MLastName) as MName,");
				stringBuilder.AppendLine(" MAccountID, t18.MName as MPCountryID, MPCityID, MPPostalNo, t19.MName as MRealCountryID, t2.MRealCityID, MRealPostalNo,");
				stringBuilder.AppendLine(" MFax, MMobile, MDirectPhone, MQQNo, MWeChatNo, MSkypeName, MWebsite, MOurEmail, MDefaultCyID,");
				stringBuilder.AppendLine(" MTaxNo, MSalTaxTypeID, MPurTaxTypeID, MDiscount,MPurDueDate,MSalDueDate,MPurDueCondition,");
				stringBuilder.AppendLine(" MSalDueCondition, MRecAcctID, MPayAcctID, MBorrowAcctID, MExpenseAccountCode");
			}
			stringBuilder.AppendLine(" from T_BD_Employees t1");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Employees_l t2 ON t1.MOrgID = t2.MOrgID and  t2.MParentID=t1.MItemID AND t2.MLocaleID=@MLocaleID and t2.MIsDelete = 0 " + GetActiveFilter("t2.", includeDisable));
			stringBuilder.AppendLine(" left join T_Sec_User  t3 on t3.MItemID=t1.MUserID and t3.MIsDelete = 0 and t3.MIsActive = 1");
			stringBuilder.AppendLine(" left join T_BD_Account t4 on t4.MCode=t1.MCurrentAccountCode and t4.MOrgID=t1.MOrgID and t4.MIsDelete = 0  ");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t18 on  t1.MPCountryID=t18.MParentID and t18.MLocaleID=@MLocaleID and t18.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t19 on  t1.MRealCountryID=t19.MParentID and t19.MLocaleID=@MLocaleID and t19.MIsDelete = 0 ");
			stringBuilder.AppendLine(" where t1.MIsDelete=0 and t1.MOrgID=@MOrgID  " + GetActiveFilter("t1.", includeDisable));
			stringBuilder.AppendLine(GetFilterWhere(searchFilter));
			return stringBuilder;
		}

		private string GetActiveFilter(string aliasPre, bool includeDisable = false)
		{
			if (includeDisable)
			{
				return string.Empty;
			}
			return $" and {aliasPre}MIsActive=1";
		}

		private StringBuilder GetEmpListSqlBulider(BDEmployeesListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select distinct t2.MFirstName,t2.MLastName,t1.MSex,t1.MStatus,t1.MEmail,t1.MItemID,t1.MPhone,t1.MUserID, ");
			stringBuilder.AppendFormat(" convert(AES_DECRYPT(t1.MBankAcctNo,'{0}') using utf8) AS MBankAcctNo,t3.MEmailAddress as MLinkUser, t1.MCurrentAccountCode, t4.MItemID as MCurrentAccountID ", "JieNor-001");
			if (filter.IsFromExport)
			{
				stringBuilder.AppendLine(" ,t2.MPAttention, t2.MPStreet, t2.MPRegion,t2.MRealAttention,t2.MRealStreet,t2.MRealRegion,t2.MBankAccName,t2.MBankName, ");
				stringBuilder.AppendLine(" F_GetUserName(t2.MFirstName,t2.MLastName) as MName,");
				stringBuilder.AppendLine(" MAccountID, t8.MName as MPCountryID, t2.MPCityID, MPPostalNo, t9.MName as MRealCountryID, t2.MRealCityID, MRealPostalNo,");
				stringBuilder.AppendLine(" MFax, MMobile, MDirectPhone, MQQNo, MWeChatNo, MSkypeName, MWebsite, MOurEmail, MDefaultCyID,");
				stringBuilder.AppendLine(" MTaxNo, MSalTaxTypeID, MPurTaxTypeID, MDiscount,MPurDueDate,MSalDueDate,MPurDueCondition,");
				stringBuilder.AppendLine(" MSalDueCondition, MRecAcctID, MPayAcctID, MBorrowAcctID, MExpenseAccountCode");
			}
			stringBuilder.AppendLine(" from T_BD_Employees t1");
			stringBuilder.AppendLine(" INNER JOIN T_BD_Employees_l t2 ON t2.MParentID=t1.MItemID AND t2.MLocaleID=@MLocaleID and t2.MOrgID = t1.MOrgID and t2.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Sec_User  t3 on t3.MItemID=t1.MUserID ");
			stringBuilder.AppendLine(" left join T_BD_Account t4 on t4.MCode=t1.MCurrentAccountCode and t4.MOrgID = t1.MOrgID and t4.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t8 on t8.MParentID=t1.MPCountryID  AND t8.MLocaleID=@MLocaleID and t8.MIsDelete = 0 ");
			stringBuilder.AppendLine(" left join T_Bas_Country_L t9 on t9.MParentID=t1.MRealCountryID  AND t9.MLocaleID=@MLocaleID  and t9.MIsDelete = 0 ");
			stringBuilder.AppendLine(" where t1.MIsActive=@MIsActive and t1.MIsDelete=0 and t1.MOrgID=@MOrgID ");
			stringBuilder.AppendLine(GetFilterWhere(filter.searchFilter));
			return stringBuilder;
		}

		private MySqlParameter[] GetListParameters(MContext ctx, string searchFilter)
		{
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@searchFilter", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = searchFilter;
			return array;
		}

		private MySqlParameter[] GetListParameters(MContext ctx, BDEmployeesListFilterModel filter)
		{
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@searchFilter", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MIsActive", MySqlDbType.Bit, 1)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.searchFilter;
			array[3].Value = (string.IsNullOrWhiteSpace(filter.IsActive) ? 1 : int.Parse(filter.IsActive));
			return array;
		}

		public List<NameValueModel> GetEmployeeNameInfoList(MContext ctx, bool includeDisabled = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("SELECT  a.MItemID  AS MValue, b.MFirstName as MName, b.MLastName as MTag,c.MItemID AS MValue1, concat(ifnull(a.MStatus, ''), ',', case when a.MIsActive=1 then '1' else '0' end, '') AS MValue2 FROM T_BD_Employees a \r\n                            INNER JOIN T_BD_Employees_l b ON a.MItemID=b.MParentID  and a.MOrgID = b.MOrgID and a.MIsDelete = 0 and b.MIsDelete=0 \r\n                            LEFT JOIN (select MItemID,MCode from t_bd_account where morgid=@MOrgID and MIsDelete = 0 ) c ON a.MCurrentAccountCode=c.MCode\r\n                            WHERE a.MOrgID = @MOrgID AND a.MIsDelete=0  ", "JieNor-001"));
			if (!includeDisabled)
			{
				stringBuilder.AppendLine(" AND a.MIsActive=1");
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null));
			return ModelInfoManager.DataTableToList<NameValueModel>(ds);
		}

		private static string GetFilterWhere(string searchFilter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(searchFilter) && !searchFilter.Equals("0") && !searchFilter.Equals("ALL"))
			{
				if (searchFilter.Equals("123"))
				{
					stringBuilder.AppendLine(" AND (t2.MFirstName REGEXP '^[0-9]' or t2.MLastName REGEXP '^[0-9]') ");
				}
				else
				{
					stringBuilder.AppendLine(" AND (t2.MFirstName like concat(@searchFilter, '%') or t2.MLastName like concat(@searchFilter, '%')  or t2.MNameFirstLetter like concat(@searchFilter, '%')) ");
				}
			}
			return stringBuilder.ToString();
		}

		public List<CommandInfo> GetEmployeeUpdateCmdList(MContext ctx, BDEmployeesModel model, OperationResult result)
		{
			if (model.IsDelete)
			{
				model.MIsDelete = true;
			}
			else
			{
				OperationResult operationResult = IsImportEmployeeNamesExist(ctx, new List<BDEmployeesModel>
				{
					model
				}, model.MItemID, true);
				if (!operationResult.Success)
				{
					result.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "EmployeeExist", "The name already exists!")
					});
				}
			}
			if (!result.Success)
			{
				return new List<CommandInfo>();
			}
			MultiLanguageAdd(model);
			return ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, model, null, true);
		}

		public BDEmployeesModel MultiLanguageAdd(BDEmployeesModel model)
		{
			MultiLanguageFieldList multiLanguageFieldList = (from t in model.MultiLanguage
			where t.MFieldName == "MFirstName"
			select t).FirstOrDefault();
			MultiLanguageFieldList multiLanguageFieldList2 = (from t in model.MultiLanguage
			where t.MFieldName == "MLastName"
			select t).FirstOrDefault();
			if (multiLanguageFieldList == null || multiLanguageFieldList2 == null)
			{
				return model;
			}
			List<MultiLanguageField> mMultiLanguageField = multiLanguageFieldList.MMultiLanguageField;
			List<MultiLanguageField> mMultiLanguageField2 = multiLanguageFieldList2.MMultiLanguageField;
			for (int i = 0; i < multiLanguageFieldList.MMultiLanguageField.Count; i++)
			{
				string mLocaleID = mMultiLanguageField[i].MLocaleID;
				string mValue = mMultiLanguageField[i].MValue;
				string mValue2 = mMultiLanguageField2[i].MValue;
				string userName = GlobalFormat.GetUserName(mValue, mValue2, null);
				string chineseSpell = AchieveLetter.GetChineseSpell(userName);
				string filedValue = AchieveLetter.StrConvertToPinyin(userName);
				MultiLangHelper.UpdateValue(mLocaleID, "MNameLetter", filedValue, model.MultiLanguage);
				MultiLangHelper.UpdateValue(mLocaleID, "MNameFirstLetter", chineseSpell, model.MultiLanguage);
			}
			return model;
		}

		public BDEmployeesModel GetEmployeeInfo(MContext ctx, string employeeId)
		{
			BDEmployeesModel dataEditModel = ModelInfoManager.GetDataEditModel<BDEmployeesModel>(ctx, employeeId, false, true);
			if (dataEditModel == null)
			{
				return new BDEmployeesModel();
			}
			return GetMultielementData(ctx, dataEditModel);
		}

		private BDEmployeesModel GetMultielementData(MContext ctx, BDEmployeesModel model)
		{
			model.MFirstName = GetMultielementColumnValue(ctx, "MFirstName", model);
			model.MLastName = GetMultielementColumnValue(ctx, "MLastName", model);
			model.Name = GlobalFormat.GetUserName(model.MFirstName, model.MLastName, ctx);
			return model;
		}

		public OperationResult ArchiveEmployee(MContext ctx, ParamBase param)
		{
			List<string> pkID = param.KeyIDs.Split(',').ToList();
			return ModelInfoManager.ArchiveFlag<BDEmployeesModel>(ctx, pkID);
		}

		public OperationResult DeleteEmployee(MContext ctx, ParamBase param)
		{
			return ModelInfoManager.DeleteFlag<BDEmployeesModel>(ctx, param.KeyIDs);
		}

		public List<BDEmployeesModel> GetOrgUserList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(" select t1.MUserID,t2.MEmailAddress FROM T_Sec_OrgUser t1 ");
			stringBuilder.AppendLine(" left join T_Sec_User t2 on t1.MUserID=t2.MItemID and t2.MIsDelete = 0 and t2.MIsActive = 1 ");
			stringBuilder.AppendLine(" where t1.MOrgID=@MOrgID and t1.MIsDelete=0 and t1.MIsActive = 1  ");
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), ctx.GetParameters((MySqlParameter)null));
			if (dataSet == null || dataSet.Tables.Count == 0)
			{
				return null;
			}
			DataTable dataTable = dataSet.Tables[0];
			if (dataTable.Rows.Count == 0)
			{
				return null;
			}
			List<BDEmployeesModel> list = new List<BDEmployeesModel>();
			foreach (DataRow row in dataTable.Rows)
			{
				BDEmployeesModel bDEmployeesModel = new BDEmployeesModel();
				string mUserID = row["MUserID"].ToString();
				string mEmail = row["MEmailAddress"].ToString();
				bDEmployeesModel.MUserID = mUserID;
				bDEmployeesModel.MEmail = mEmail;
				list.Add(bDEmployeesModel);
			}
			return list;
		}

		public OperationResult IsImportEmployeeNamesExist(MContext ctx, List<BDEmployeesModel> list, string excludeId = null, bool includeDisable = true)
		{
			OperationResult operationResult = new OperationResult();
			List<string> list2 = new List<string>();
			foreach (BDEmployeesModel item2 in list)
			{
				List<MultiLanguageFieldList> source = (from f in item2.MultiLanguage
				where f.MFieldName == "MFirstName" || f.MFieldName == "MLastName"
				select f).ToList();
				MultiLanguageFieldList multiLanguageFieldList = source.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MFirstName");
				MultiLanguageFieldList multiLanguageFieldList2 = source.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MLastName");
				foreach (MultiLanguageField item3 in multiLanguageFieldList.MMultiLanguageField)
				{
					list2.Add($"{item3.MValue} {multiLanguageFieldList2.MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == item3.MLocaleID).MValue}");
				}
			}
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < list2.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(list2[i]))
				{
					stringBuilder.AppendFormat(",{0}", "@FullName" + i);
					list3.Add(new MySqlParameter("@FullName" + i, MySqlDbType.VarChar, 100));
					list3[num].Value = list2[i];
					num++;
				}
			}
			list3.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			list3.Add(new MySqlParameter("@MLocaleID", ctx.MLCID));
			string text = $"select F_GetUserName(b.MFirstName,b.MLastName) as EmployeeName from T_BD_Employees a \r\n                            inner join T_BD_Employees_l b on a.MItemID=b.MParentID and a.MOrgID = b.MOrgID  and b.MIsDelete = 0 \r\n                            where a.MOrgID=@MOrgID and a.MIsDelete = 0 and concat(MFirstName, ' ', MLastName) in ({stringBuilder.ToString().Trim(',')})";
			if (!includeDisable)
			{
				text += " and a.MIsActive = 1";
			}
			if (!string.IsNullOrWhiteSpace(excludeId))
			{
				list3.Add(new MySqlParameter("@MItemID", excludeId));
				text += " and MItemID!=@MItemID";
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(text, list3.ToArray());
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				List<string> list4 = new List<string>();
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					list4.Add(Convert.ToString(row[0]));
				}
				operationResult.VerificationInfor = new List<BizVerificationInfor>();
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ImportFailed4ExistingEmployeeName", "The Employee Name:{0} already exists. Please enter a different Employee Name!");
				string message = string.Format(text2, string.Join(",", list4.Distinct()));
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					CheckItem = "检查导入的员工是否已存在",
					Level = AlertEnum.Error,
					Message = message
				});
			}
			return operationResult;
		}

		public OperationResult ImportEmployeeList(MContext ctx, List<BDEmployeesModel> list)
		{
			OperationResult operationResult = new OperationResult();
			try
			{
				List<CommandInfo> list2 = new List<CommandInfo>();
				PAPaySettingModel paySettingModel = PASalaryPaymentRepository.GetPaySettingModel(ctx);
				List<PAPITThresholdModel> pITThresholdList = PAPITRepository.GetPITThresholdList(ctx, new PAPITThresholdFilterModel
				{
					IsDefault = true
				});
				List<PAPITThresholdModel> list3 = new List<PAPITThresholdModel>();
				foreach (BDEmployeesModel item in list)
				{
					item.MItemID = UUIDHelper.GetGuid();
					item.IsNew = true;
					if (string.IsNullOrWhiteSpace(item.MStatus))
					{
						item.MStatus = "None";
					}
					CorrectPercentField(item);
					SetSSHFAmountByCompanyPer(paySettingModel, item);
					MultiLanguageAdd(item);
					List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, item, null, true);
					list2.AddRange(insertOrUpdateCmd);
					BDPayrollDetailModel bDPayrollDetailModel = new BDPayrollDetailModel();
					bDPayrollDetailModel.MEmployeeID = item.MItemID;
					ModelPropertyHelper.CopyModelValue(item, bDPayrollDetailModel);
					bDPayrollDetailModel.MMedicalInsurancePercentage = Convert.ToDecimal(item.MMedicalInsurancePercentage);
					bDPayrollDetailModel.MUmemploymentPercentage = Convert.ToDecimal(item.MUmemploymentPercentage);
					bDPayrollDetailModel.MRetirementSecurityPercentage = Convert.ToDecimal(item.MRetirementSecurityPercentage);
					bDPayrollDetailModel.MProvidentPercentage = Convert.ToDecimal(item.MProvidentPercentage);
					bDPayrollDetailModel.MProvidentAdditionalPercentage = Convert.ToDecimal(item.MProvidentAdditionalPercentage);
					SetPayrollAmountByPercent(bDPayrollDetailModel);
					COMModelValidateHelper.ValidateModel(ctx, bDPayrollDetailModel, operationResult);
					list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDPayrollDetailModel>(ctx, bDPayrollDetailModel, null, true));
					AddEmployeePITCommand(ctx, list2, pITThresholdList, item, operationResult);
				}
				if (!operationResult.Success)
				{
					return operationResult;
				}
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(list2);
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

		private void AddEmployeePITCommand(MContext ctx, List<CommandInfo> commandList, List<PAPITThresholdModel> defaultPITList, BDEmployeesModel model, OperationResult result)
		{
			if (!model.MIncomeTaxThreshold.HasValue)
			{
				model.MIncomeTaxThreshold = defaultPITList[1].MAmount;
			}
			if (model.MIncomeTaxThreshold.Value != defaultPITList[1].MAmount)
			{
				commandList.AddRange(GetEmployeePITCommandList(ctx, model.MItemID, defaultPITList[1].MEffectiveDate, model.MIncomeTaxThreshold.Value, result));
			}
			if (!model.MIncomeTaxThresholdNew.HasValue)
			{
				model.MIncomeTaxThresholdNew = defaultPITList[0].MAmount;
			}
			if (model.MIncomeTaxThresholdNew.Value != defaultPITList[0].MAmount)
			{
				commandList.AddRange(GetEmployeePITCommandList(ctx, model.MItemID, defaultPITList[0].MEffectiveDate, model.MIncomeTaxThresholdNew.Value, result));
			}
		}

		private List<CommandInfo> GetEmployeePITCommandList(MContext ctx, string employeeId, DateTime effectiveDate, decimal pit, OperationResult result)
		{
			PAPITThresholdModel pAPITThresholdModel = new PAPITThresholdModel();
			pAPITThresholdModel.MOrgID = ctx.MOrgID;
			pAPITThresholdModel.MEmployeeID = employeeId;
			pAPITThresholdModel.MEffectiveDate = effectiveDate;
			pAPITThresholdModel.MAmount = pit;
			if (!COMModelValidateHelper.ValidateModel(ctx, pAPITThresholdModel, result))
			{
				return new List<CommandInfo>();
			}
			return ModelInfoManager.GetInsertOrUpdateCmd<PAPITThresholdModel>(ctx, pAPITThresholdModel, null, true);
		}

		private decimal? CorrectPercentData(decimal? per)
		{
			if (!per.HasValue)
			{
				return per;
			}
			decimal? nullable = per;
			if (nullable.GetValueOrDefault() < default(decimal) & nullable.HasValue)
			{
				return default(decimal);
			}
			nullable = per;
			decimal d = 100;
			if (nullable.GetValueOrDefault() > d & nullable.HasValue)
			{
				return 100;
			}
			return per;
		}

		private void CorrectPercentField(BDEmployeesModel model)
		{
			model.MMedicalInsurancePercentage = CorrectPercentData(model.MMedicalInsurancePercentage);
			model.MUmemploymentPercentage = CorrectPercentData(model.MUmemploymentPercentage);
			model.MRetirementSecurityPercentage = CorrectPercentData(model.MRetirementSecurityPercentage);
			model.MProvidentPercentage = CorrectPercentData(model.MProvidentPercentage);
			model.MProvidentAdditionalPercentage = CorrectPercentData(model.MProvidentAdditionalPercentage);
		}

		private void SetSSHFAmountByCompanyPer(PAPaySettingModel companyPaySetting, BDEmployeesModel model)
		{
			if (!model.MMedicalInsurancePercentage.HasValue)
			{
				model.MMedicalInsurancePercentage = companyPaySetting.MEmpMedicalInsurancePer;
				model.MMedicalInsuranceAmount = companyPaySetting.MEmpMedicalInsurancePer * model.MSocialSecurityBase;
			}
			if (!model.MUmemploymentPercentage.HasValue)
			{
				model.MUmemploymentPercentage = companyPaySetting.MEmpUmemploymentInsurancePer;
				model.MUmemploymentAmount = companyPaySetting.MEmpUmemploymentInsurancePer * model.MSocialSecurityBase;
			}
			if (!model.MRetirementSecurityPercentage.HasValue)
			{
				model.MRetirementSecurityPercentage = companyPaySetting.MEmpRetirementSecurityPer;
				model.MRetirementSecurityAmount = companyPaySetting.MEmpRetirementSecurityPer * model.MSocialSecurityBase;
			}
			if (!model.MProvidentPercentage.HasValue)
			{
				model.MProvidentPercentage = companyPaySetting.MProvidentFundPer;
				model.MProvidentAmount = companyPaySetting.MProvidentFundPer * model.MHosingProvidentFundBase;
			}
			if (!model.MProvidentAdditionalPercentage.HasValue)
			{
				model.MProvidentAdditionalPercentage = companyPaySetting.MAddProvidentFundPer;
				model.MProvidentAdditionalAmount = companyPaySetting.MAddProvidentFundPer * model.MHosingProvidentFundBase;
			}
		}

		private void SetPayrollAmountByPercent(BDPayrollDetailModel model)
		{
			if (model != null)
			{
				if (model.MSocialSecurityBase > decimal.Zero)
				{
					model.MRetirementSecurityAmount = model.MSocialSecurityBase * model.MRetirementSecurityPercentage / 100m;
					model.MMedicalInsuranceAmount = model.MSocialSecurityBase * model.MMedicalInsurancePercentage / 100m;
					model.MUmemploymentAmount = model.MSocialSecurityBase * model.MUmemploymentPercentage / 100m;
				}
				if (model.MHosingProvidentFundBase > decimal.Zero)
				{
					model.MProvidentAmount = model.MHosingProvidentFundBase * model.MProvidentPercentage / 100m;
					model.MProvidentAdditionalAmount = model.MHosingProvidentFundBase * model.MProvidentAdditionalPercentage / 100m;
				}
			}
		}

		private string GetMultielementColumnValue(MContext ctx, string columnName, BDEmployeesModel model)
		{
			string result = string.Empty;
			if (model.MultiLanguage != null)
			{
				result = model.MultiLanguage.FirstOrDefault((MultiLanguageFieldList w) => w.MFieldName.EqualsIgnoreCase(columnName)).MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
			}
			return result;
		}

		public List<CommandInfo> UpdateEmpMapAccount(MContext ctx, string oldCode, string newCode)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@oldCode", MySqlDbType.VarChar, 36)
				{
					Value = oldCode
				},
				new MySqlParameter("@newCode", MySqlDbType.VarChar, 36)
				{
					Value = newCode
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_bd_employees set MCurrentAccountCode=@newCode where MCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update t_bd_employees set MExpenseAccountCode=@newCode where MExpenseAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete = 0 ";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			return list;
		}

		public OperationResult RestoreEmployee(MContext ctx, List<string> ids)
		{
			OperationResult operationResult = new OperationResult();
			if (ids == null || ids.Count() == 0)
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			foreach (string id in ids)
			{
				MySqlParameter[] parameters = new MySqlParameter[2]
				{
					new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
					{
						Value = ctx.MOrgID
					},
					new MySqlParameter("@MItemID", MySqlDbType.VarChar, 36)
					{
						Value = id
					}
				};
				CommandInfo commandInfo = new CommandInfo();
				commandInfo.CommandText = "update t_bd_employees set MIsActive=1 where MOrgID=@MOrgID and MItemID=@MItemID and MIsActive = 0 and MIsDelete = 0";
				DbParameter[] array = commandInfo.Parameters = parameters;
				CommandInfo commandInfo2 = new CommandInfo();
				commandInfo2.CommandText = "update T_BD_Employees_l set MIsActive=1 where  MOrgID=@MOrgID and MparentID=@MItemID and MIsActive = 0 and MIsDelete = 0";
				array = (commandInfo2.Parameters = parameters);
				list.Add(commandInfo);
				list.Add(commandInfo2);
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public OperationResult CheckEmployeeIsExist(MContext ctx, SqlWhere filter)
		{
			OperationResult operationResult = new OperationResult();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT a.mitemid , b.MFirstName,b.MLastName , c.MIDNumber FROM t_bd_employees a\r\n                                 INNER JOIN t_bd_employees_l b on a.MItemID = b.MParentID and a.MIsDelete=0 and a.MOrgID=@morgid and b.MLocaleID=@MLocaleID and b.MIsDelete=0 \r\n                                 INNER JOIN t_bd_emppayrollbasicset c on c.MEmployeeID=a.MItemID and a.MIsDelete=0 ");
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@MOrgID", ctx.MOrgID));
			list.Add(new MySqlParameter("@MLocaleID", ctx.MLCID));
			if (!string.IsNullOrWhiteSpace(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			list.AddRange(filter.Parameters);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), list.ToArray());
			if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				operationResult.Success = true;
			}
			else
			{
				operationResult.Success = false;
			}
			return operationResult;
		}

		public List<BDEmployeesModel> GetEmployeeList(MContext ctx, bool includeDisable)
		{
			string text = "SELECT \n                            t1.MOrgID,\n                            t1.MItemID,\n                            t2.MFirstName,\n                            t2.MLastName,\n                            t1.MEmail,\n                            t1.MIsActive,\n                            F_GETUSERNAME(t2.MFirstName, t2.MLastName) AS MFullName\n                        FROM\n                            t_bd_employees t1\n                                INNER JOIN\n                            t_bd_employees_l t2 ON t1.MItemID = t2.MParentID\n                                AND t2.MIsDelete = 0\n                                AND t1.MOrgID = t2.MOrgID\n                                AND t2.MLocaleId = @MLocaleID\n                        WHERE\n                            t1.MIsDelete = 0\n                                AND IFNULL(t1.MStatus, '') <> 'Leave'\n                                AND t1.MOrgID = @MOrgID\n                                AND t1.MIsDelete = 0";
			if (!includeDisable)
			{
				text += " and t1.MIsActive=1 ";
			}
			return ModelInfoManager.GetDataModelBySql<BDEmployeesModel>(ctx, text, ctx.GetParameters((MySqlParameter)null).ToArray());
		}
	}
}
