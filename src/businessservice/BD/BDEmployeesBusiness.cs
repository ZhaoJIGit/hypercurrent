using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDEmployeesBusiness : APIBusinessBase<BDEmployeesModel>, IBDEmployeesBusiness
	{
		private readonly BDEmployeesRepository apiDal = new BDEmployeesRepository();

		private readonly REGCurrencyRepository currencyDal = new REGCurrencyRepository();

		private string[] currentAccountCodes = new string[4]
		{
			"1123",
			"1221",
			"2203",
			"2241"
		};

		private List<BDEmployeesModel> _multEmployeeList;

		private List<BDEmployeesModel> _dbList = new List<BDEmployeesModel>();

		private List<BDEmployeesModel> _employeeDataPool;

		private List<BDAccountEditModel> _accountDataPool;

		private List<REGCurrencyViewModel> _currencyDataPool;

		private BDEmployeesRepository _bdEmpls = new BDEmployeesRepository();

		private BDAccountRepository _account = new BDAccountRepository();

		private REGTaxRateRepository _taxRate = new REGTaxRateRepository();

		private REGCurrencyRepository _bdCurrency = new REGCurrencyRepository();

		private BASLangBusiness _langBiz = new BASLangBusiness();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BDAccountRepository accountRepository = new BDAccountRepository();

		private BASCountryBusiness _country = new BASCountryBusiness();

		private BDEmpPayrollDetailBll _empPayrollDetailBiz = new BDEmpPayrollDetailBll();

		protected override DataGridJson<BDEmployeesModel> OnGet(MContext ctx, GetParam param)
		{
			return apiDal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDEmployeesModel model)
		{
			if (model.MPaymentTerms != null && (ctx.MOrgVersionID == 1 || (model.MPaymentTerms.MExpense != null && model.MPaymentTerms.MExpense.MDay == 0 && string.IsNullOrWhiteSpace(model.MPaymentTerms.MExpense.MDayType))))
			{
				model.MPaymentTerms.MExpense = null;
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<BDEmployeesModel> param, APIDataPool dataPool)
		{
			_multEmployeeList = ModelInfoManager.GetDataModelList<BDEmployeesModel>(ctx, new SqlWhere(), false, false);
			_employeeDataPool = dataPool.Employees;
			_accountDataPool = dataPool.Accounts;
			_currencyDataPool = currencyDal.GetCurrencyViewList(ctx, null, true, null);
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID)
			select t.MItemID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true),
					IncludeDisabled = true
				};
				if (list.Count > 0)
				{
					base.SetWhereString(param2, "EmployeeID", list, true);
				}
				DataGridJson<BDEmployeesModel> dataGridJson = apiDal.Get(ctx, param2);
				_dbList = dataGridJson.rows;
			}
		}

		protected override void OnPostValidate(MContext ctx, PostParam<BDEmployeesModel> param, APIDataPool dataPool, BDEmployeesModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			ProcessEmployeesModel(ctx, isPut, model);
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<BDEmployeesModel> param, APIDataPool dataPool, BDEmployeesModel model)
		{
			if (!string.IsNullOrEmpty(model.MCurrentAccountCode))
			{
				model.MCurrentAccountCode = model.MCurrentAccountCode.Replace(".", "");
			}
			apiDal.MultiLanguageAdd(model);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, model, model.UpdateFieldList, true));
			if (model.IsNew)
			{
				_multEmployeeList.Add(model);
			}
			return list;
		}

		protected override void OnPostAfter(MContext ctx, PostParam<BDEmployeesModel> param, APIDataPool dataPool)
		{
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MItemID) && t.ValidationErrors.Count == 0
			select t.MItemID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				base.SetWhereString(param2, "EmployeeID", list, true);
				DataGridJson<BDEmployeesModel> dataGridJson = base.Get(ctx, param2);
				List<BDEmployeesModel> list2 = new List<BDEmployeesModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					BDEmployeesModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list2.Add(model);
					}
					else
					{
						BDEmployeesModel bDEmployeesModel = dataGridJson.rows.FirstOrDefault((BDEmployeesModel a) => a.MItemID == model.MItemID);
						if (bDEmployeesModel != null)
						{
							bDEmployeesModel.IsNew = model.IsNew;
							list2.Add(bDEmployeesModel);
						}
					}
				}
				param.DataList = list2;
			}
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, BDEmployeesModel model)
		{
			if (!model.MIsActive)
			{
				model.Validate(ctx, true, "DisableEmployeeCanNotDelete", "禁用状态的员工不能被删除。", LangModule.BD);
			}
			List<string> itemIds = new List<string>
			{
				model.MItemID
			};
			BDIsCanDeleteModel bDIsCanDeleteModel = BDRepository.IsCanDeleteOrInactive(ctx, "Employees", itemIds, true);
			if (!bDIsCanDeleteModel.AllSuccess)
			{
				string userName = GlobalFormat.GetUserName(model.MFirstName, model.MLastName, ctx);
				model.Validate(ctx, true, "UsedEmployeeCanNotDelete", "员工“{0}”已被使用，不能被删除。", LangModule.BD, userName);
			}
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, BDEmployeesModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> pkIDS = new List<string>
			{
				model.MItemID
			};
			list.AddRange(ModelInfoManager.GetDeleteCmd<BDEmployeesModel>(ctx, pkIDS));
			return list;
		}

		private void ProcessEmployeesModel(MContext ctx, bool isPut, BDEmployeesModel model)
		{
			BDEmployeesModel bDEmployeesModel = null;
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				bDEmployeesModel = _dbList.FirstOrDefault((BDEmployeesModel t) => t.MItemID == model.MItemID);
			}
			BDEmployeesModel matchModel = null;
			ProcessIdAndName(ctx, model, bDEmployeesModel, isPut, ref matchModel);
			if (matchModel != null)
			{
				bDEmployeesModel = _multEmployeeList.FirstOrDefault((BDEmployeesModel t) => t.MItemID == matchModel.MItemID);
			}
			model.IsNew = (bDEmployeesModel == null);
			if (model.IsNew)
			{
				model.MCreateBy = ctx.MConsumerKey;
				model.UpdateFieldList.Add("MCreateBy");
				model.MItemID = UUIDHelper.GetGuid();
			}
			else
			{
				model.MItemID = bDEmployeesModel.MItemID;
			}
			ProcessJobStatus(ctx, model);
			ProcessEmail(ctx, model);
			ProcessGender(ctx, model);
			ProcessAccountCode(ctx, model);
			ProcessCurrencyCode(ctx, model);
			ProcessPaymentTerms(ctx, model);
			if (bDEmployeesModel != null && !isPut && !bDEmployeesModel.MIsActive)
			{
				model.Validate(ctx, true, "DisabledEmployeeCanNotUpdate", "禁用状态的员工不能被更新。", LangModule.BD);
			}
		}

		private void ProcessIdAndName(MContext ctx, BDEmployeesModel model, BDEmployeesModel dbModel, bool isPut, ref BDEmployeesModel matchModel)
		{
			bool flag = model.IsUpdateFieldExists("MFirstName");
			bool flag2 = flag && string.IsNullOrEmpty(model.MFirstName);
			bool flag3 = model.IsUpdateFieldExists("MLastName");
			bool flag4 = flag3 && string.IsNullOrEmpty(model.MLastName);
			bool flag5 = model.IsUpdateFieldExists("MEmployeeID") && string.IsNullOrEmpty(model.MEmployeeID);
			if (flag2 | flag4)
			{
				model.Validate(ctx, true, "FirstOrLastNameCanNotEmpty", "必须提供“FirstName + LastName”。", LangModule.BD);
			}
			else if (dbModel == null && (!flag || !flag3))
			{
				if (string.IsNullOrEmpty(model.MItemID) && !isPut)
				{
					model.Validate(ctx, true, "IDOrFirstOrLastNameCanNotEmpty", "必须提供“EmployeeID”或“FirstName + LastName”。", LangModule.BD);
				}
				else
				{
					model.Validate(ctx, true, "FirstOrLastNameCanNotEmpty", "必须提供“FirstName + LastName”。", LangModule.BD);
				}
			}
			else if (dbModel == null || (flag && flag3))
			{
				int num = 0;
				string mLCID = ctx.MLCID;
				matchModel = MatchMultEmployeeRecord(ctx, model, ref num, ref mLCID);
				if (num > 0 & isPut)
				{
					string fullUserName = GetFullUserName(model, mLCID);
					model.Validate(ctx, true, "EmployeeAlreadyExists", "员工名称“{0}”在系统中已经存在。", LangModule.BD, fullUserName);
				}
				else if (num > 1)
				{
					model.Validate(ctx, true, "EmployeeMatchMultRecord", "员工的多语言字段”FirstName + LastName”的组合，匹配到两个系统记录，请检查。", LangModule.BD);
				}
				else if (matchModel != null)
				{
					if (dbModel != null && matchModel.MItemID != dbModel.MItemID)
					{
						string fullUserName2 = GetFullUserName(model, mLCID);
						model.Validate(ctx, true, "EmployeeAlreadyExists", "员工名称“{0}”在系统中已经存在。", LangModule.BD, fullUserName2);
					}
					else
					{
						SetMultFieldInfo(model, matchModel);
					}
				}
			}
		}

		private BDEmployeesModel MatchMultEmployeeRecord(MContext ctx, BDEmployeesModel model, ref int matchCount, ref string matchLcid)
		{
			BDEmployeesModel result = null;
			List<MultiLanguageFieldList> multiLanguage = model.MultiLanguage;
			if (multiLanguage == null || multiLanguage.Count == 0)
			{
				matchCount = 0;
				return null;
			}
			List<MultiLanguageField> list = multiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MFirstName")?.MMultiLanguageField;
			List<MultiLanguageField> list2 = multiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MLastName")?.MMultiLanguageField;
			if (list == null || list2 == null)
			{
				matchCount = 0;
				return null;
			}
			bool flag = false;
			foreach (BDEmployeesModel multEmployee in _multEmployeeList)
			{
				List<MultiLanguageField> source = multEmployee.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MFirstName")?.MMultiLanguageField;
				List<MultiLanguageField> source2 = multEmployee.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MLastName")?.MMultiLanguageField;
				bool flag2 = false;
				foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
				{
					string a2 = source.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == mActiveLocaleID)?.MValue;
					string a3 = source2.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == mActiveLocaleID)?.MValue;
					string b = list.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == mActiveLocaleID)?.MValue;
					string b2 = list2.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == mActiveLocaleID)?.MValue;
					if (a2.EqualsIgnoreCase(b) && a3.EqualsIgnoreCase(b2))
					{
						flag2 = true;
						if (!flag)
						{
							matchLcid = mActiveLocaleID;
						}
						if (mActiveLocaleID == ctx.MLCID)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag2)
				{
					matchCount++;
					result = multEmployee;
				}
				if (matchCount > 1)
				{
					return null;
				}
			}
			if (matchCount == 1)
			{
				return result;
			}
			return null;
		}

		private void ProcessJobStatus(MContext ctx, BDEmployeesModel model)
		{
			if (!model.IsUpdateFieldExists("MStatus") && model.IsNew)
			{
				model.MStatus = "None";
				model.UpdateFieldList.Add("MStatus");
			}
		}

		private void ProcessEmail(MContext ctx, BDEmployeesModel model)
		{
			if (!string.IsNullOrEmpty(model.MEmail) && !RegExp.IsEmail(model.MEmail))
			{
				model.Validate(ctx, true, "EmailFormatIsError", "“{0}”不是有效的邮箱格式。", LangModule.Common, model.MEmail);
			}
		}

		private void ProcessGender(MContext ctx, BDEmployeesModel model)
		{
			if (!model.IsUpdateFieldExists("MSex") && model.IsNew)
			{
				model.MSex = "Male";
				model.UpdateFieldList.Add("MSex");
			}
		}

		private void ProcessAccountCode(MContext ctx, BDEmployeesModel model)
		{
			if (!string.IsNullOrEmpty(model.MCurrentAccountCode))
			{
				BDAccountEditModel acctModel = _accountDataPool.FirstOrDefault((BDAccountEditModel a) => a.MNumber == model.MCurrentAccountCode);
				if (acctModel == null)
				{
					model.Validate(ctx, true, "AccountCodeNotExist", "科目代码“{0}”不存在。", LangModule.BD, model.MCurrentAccountCode);
				}
				else
				{
					if (_accountDataPool.Exists((BDAccountEditModel a) => a.MParentID == acctModel.MItemID))
					{
						model.Validate(ctx, true, "EmployeeAccountExistSubAcct", "Account“{0}”不是末级科目。", LangModule.BD, model.MCurrentAccountCode);
					}
					if (!currentAccountCodes.Any((string c) => model.MCurrentAccountCode.StartsWith(c)))
					{
						model.Validate(ctx, true, "OnlySelectCurrentCode", "只能选择往来科目,即科目代码为“1123”,“1221”,“2203”,“2241”的本级或下级明细科目。", LangModule.BD, model.MCurrentAccountCode);
					}
					else if (!acctModel.MIsActive)
					{
						model.Validate(ctx, true, "AccountAlreadyDisabled", "Account“{0}”已禁用。", LangModule.BD, model.MCurrentAccountCode);
					}
				}
			}
		}

		private void ProcessCurrencyCode(MContext ctx, BDEmployeesModel model)
		{
			if (model.IsUpdateFieldExists("MDefaultCyID") && !string.IsNullOrEmpty(model.MDefaultCyID))
			{
				REGCurrencyViewModel rEGCurrencyViewModel = _currencyDataPool.FirstOrDefault((REGCurrencyViewModel x) => x.MCurrencyID.EqualsIgnoreCase(model.MDefaultCyID));
				if (rEGCurrencyViewModel == null)
				{
					model.Validate(ctx, true, "FieldNotExists", "{0}“{1}”不存在。", LangModule.Common, "DefaultCurrencyCode", model.MDefaultCyID);
				}
				else
				{
					model.MDefaultCyID = rEGCurrencyViewModel.MCurrencyID;
				}
			}
		}

		private void ProcessPaymentTerms(MContext ctx, BDEmployeesModel model)
		{
			if (ctx.MOrgVersionID == 1)
			{
				model.ValidationErrors = (from f in model.ValidationErrors
				where f.Type != 4
				select f).ToList();
			}
			else if (model.IsUpdateFieldExists("MPaymentTerms"))
			{
				if (model.MPaymentTerms == null)
				{
					model.UpdateFieldList.AddRange(new string[2]
					{
						"MPurDueDate",
						"MPurDueCondition"
					});
				}
				else
				{
					BDContactsBillsPaymentTermModel mExpense = model.MPaymentTerms.MExpense;
					if (model.MPaymentTerms.UpdateFieldList.Contains("MExpense"))
					{
						if (mExpense == null)
						{
							model.UpdateFieldList.AddRange(new string[2]
							{
								"MPurDueDate",
								"MPurDueCondition"
							});
						}
						else
						{
							bool flag = mExpense.UpdateFieldList.Contains("MDayType");
							bool flag2 = mExpense.UpdateFieldList.Contains("MPurDueDate");
							if (flag2 && model.MPaymentTerms.MExpense != null && model.MPaymentTerms.MExpense.MDay < 0)
							{
								model.Validate(ctx, true, "DayMustGreaterZero", "“{0}”必须大于等于零。", LangModule.BD, "Day");
							}
							if (flag || flag2)
							{
								if (flag & flag2)
								{
									if (model.MPaymentTerms.MExpense != null)
									{
										model.MPurDueDate = model.MPaymentTerms.MExpense.MDay;
										model.MPurDueCondition = model.MPaymentTerms.MExpense.MDayType;
										model.UpdateFieldList.AddRange(new string[2]
										{
											"MPurDueDate",
											"MPurDueCondition"
										});
									}
								}
								else
								{
									model.Validate(ctx, true, "DayTypeAndDayMustProvide", "当添加或更新员工默认付款方式信息时，“天数类型”和“天数”都必须指定。", LangModule.BD, "DefaultCurrencyCode");
								}
							}
						}
					}
				}
			}
		}

		private string GetFullUserName(BDEmployeesModel model, string lcid)
		{
			List<MultiLanguageFieldList> multiLanguage = model.MultiLanguage;
			List<MultiLanguageField> list = multiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MFirstName")?.MMultiLanguageField;
			List<MultiLanguageField> list2 = multiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == "MLastName")?.MMultiLanguageField;
			if (list == null || list2 == null)
			{
				return GlobalFormat.GetUserName(model.MFirstName, model.MLastName, null);
			}
			string firstName = list.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == lcid)?.MValue;
			string lastName = list2.FirstOrDefault((MultiLanguageField a) => a.MLocaleID == lcid)?.MValue;
			return GlobalFormat.GetUserName(firstName, lastName, null);
		}

		private void SetMultFieldInfo(BDEmployeesModel model, BDEmployeesModel matchItem)
		{
			if (matchItem != null && matchItem.MultiLanguage != null)
			{
				APIValidator.SetMatchMultiFieldInfo(model.MultiLanguage, matchItem.MItemID, matchItem.MultiLanguage);
			}
		}

		public List<BDEmployeesListModel> GetBDEmployeesList(MContext ctx, string filterString, bool includeDisable = false)
		{
			return _bdEmpls.GetBDEmployeesList(ctx, filterString, includeDisable);
		}

		public DataGridJson<BDEmployeesListModel> GetBDEmployeesPageList(MContext ctx, BDEmployeesListFilterModel filter)
		{
			return _bdEmpls.GetBDEmployeesPageList(ctx, filter);
		}

		public List<BDEmployeesListModel> GetEmployeeListForExport(MContext ctx, BDEmployeesListFilterModel filter)
		{
			filter.IsFromExport = true;
			return _bdEmpls.GetBDEmployeesPageList(ctx, filter).rows;
		}

		public BDEmployeesModel GetEmployeesEditInfo(MContext ctx, string employeeId)
		{
			BDEmployeesModel employeeInfo = _bdEmpls.GetEmployeeInfo(ctx, employeeId);
			if (employeeInfo != null && ctx.MRegProgress == 15)
			{
				string currencyMoneyCode = (employeeInfo.MCurrentAccountCode == null) ? "1221" : employeeInfo.MCurrentAccountCode;
				string expenseCode = (employeeInfo.MExpenseAccountCode == null) ? "6602" : employeeInfo.MExpenseAccountCode;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MAccountTableId", ctx.MAccountTableID);
				sqlWhere.In("MCode", new string[2]
				{
					currencyMoneyCode,
					expenseCode
				});
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() == 2)
				{
					BDAccountModel bDAccountModel = (from x in baseBDAccountList
					where x.MCode == currencyMoneyCode
					select x).FirstOrDefault();
					employeeInfo.MCurrentAccountId = ((bDAccountModel != null) ? bDAccountModel.MItemID : "");
					BDAccountModel bDAccountModel2 = (from x in baseBDAccountList
					where x.MCode == expenseCode
					select x).FirstOrDefault();
					employeeInfo.MExpenseAccountId = ((bDAccountModel2 != null) ? bDAccountModel2.MItemID : "");
				}
			}
			employeeInfo.PayrollDetail = _empPayrollDetailBiz.GetModel(ctx, employeeId);
			return employeeInfo;
		}

		public BDEmployeesModel GetEmployeeInfo(MContext ctx, string employeeId)
		{
			return _bdEmpls.GetEmployeeInfo(ctx, employeeId);
		}

		public OperationResult EmployeesUpdate(MContext ctx, BDEmployeesModel info)
		{
			OperationResult operationResult = new OperationResult();
			List<string> codeList = new List<string>
			{
				info.MCurrentAccountCode
			};
			if (!_accountBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				return operationResult;
			}
			bool flag = false;
			if (!string.IsNullOrWhiteSpace(info.MIDNumber) && info.MIDType == "1")
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MIDNumber", info.MIDNumber);
				sqlWhere.Equal("MIDType", info.MIDType);
				if (!string.IsNullOrWhiteSpace(info.MItemID))
				{
					sqlWhere.NotEqual("a.MItemID", info.MItemID);
				}
				flag = _bdEmpls.CheckEmployeeIsExist(ctx, sqlWhere).Success;
			}
			if (flag)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "EmployeeIDNumberIsExist", "员工的填写的证件信息已经存在，请使用其他证件号码");
				operationResult.Success = false;
				operationResult.Message = text;
			}
			List<CommandInfo> employeeUpdateCmdList = _bdEmpls.GetEmployeeUpdateCmdList(ctx, info, operationResult);
			info.PayrollDetail.MEmployeeID = info.MItemID;
			List<PAPITThresholdModel> pITThresholdList = info.PayrollDetail.PITThresholdList;
			if (pITThresholdList != null)
			{
				foreach (PAPITThresholdModel item in pITThresholdList)
				{
					item.MEmployeeID = info.MItemID;
				}
			}
			employeeUpdateCmdList.AddRange(_empPayrollDetailBiz.GetEmpPayrollDetailUpdateCmdList(ctx, info.PayrollDetail, operationResult));
			if (!operationResult.Success)
			{
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(employeeUpdateCmdList) > 0);
			return operationResult;
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "Employees", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public OperationResult DeleteEmployee(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			string keyIDs = param.KeyIDs;
			if (keyIDs.IndexOf(',') >= 0)
			{
				List<string> list2 = new List<string>();
				operationResult = BDRepository.IsCanDelete(ctx, "Employees", keyIDs, out list2);
				if (list2.Count > 0)
				{
					string message = operationResult.Message;
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDEmployeesModel>(ctx, list2));
					list.AddRange(GetDeleteEmployeePayrollCmdList(ctx, list2));
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
					operationResult.Message = message;
				}
			}
			else
			{
				operationResult = BDRepository.IsCanDelete(ctx, "Employees", keyIDs);
				if (operationResult.Success)
				{
					List<string> list3 = new List<string>
					{
						keyIDs
					};
					list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDEmployeesModel>(ctx, list3));
					list.AddRange(GetDeleteEmployeePayrollCmdList(ctx, list3));
					DynamicDbHelperMySQL dynamicDbHelperMySQL2 = new DynamicDbHelperMySQL(ctx);
					operationResult.Success = (dynamicDbHelperMySQL2.ExecuteSqlTran(list) > 0);
				}
			}
			return operationResult;
		}

		private List<CommandInfo> GetDeleteEmployeePayrollCmdList(MContext ctx, List<string> canDeleteList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MEmployeeID", canDeleteList), false, false);
			if (dataModelList.Any())
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDPayrollDetailModel>(ctx, (from f in dataModelList
				select f.MItemID).ToList()));
			}
			List<PAPITThresholdModel> dataModelList2 = ModelInfoManager.GetDataModelList<PAPITThresholdModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MEmployeeID", canDeleteList), false, false);
			if (dataModelList2.Any())
			{
				list.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPITThresholdModel>(ctx, (from f in dataModelList2
				select f.MItemID).ToList()));
			}
			return list;
		}

		public OperationResult ArchiveEmployee(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				operationResult.Success = false;
			}
			return _bdEmpls.ArchiveEmployee(ctx, param);
		}

		public List<BDEmployeesModel> GetOrgUserList(MContext ctx)
		{
			return _bdEmpls.GetOrgUserList(ctx);
		}

		public OperationResult IsImportEmployeeNamesExist(MContext ctx, List<BDEmployeesModel> list)
		{
			return _bdEmpls.IsImportEmployeeNamesExist(ctx, list, null, true);
		}

		public OperationResult ImportEmployeeList(MContext ctx, List<BDEmployeesModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<BASCountryModel> countryList = _country.GetCountryList(ctx);
			List<IOValidationResultModel> list2 = new List<IOValidationResultModel>();
			if (ctx.MOrgVersionID != 1 && !SECPermissionRepository.HavePermission(ctx, "PayRun", "Change", "") && CheckEmplyeeHasPayRunData(list))
			{
				operationResult.Success = false;
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "NoAuthImportPayRunData", "员工信息无法导入：没有导入工资信息的权限！");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
				return operationResult;
			}
			if (CheckNameDuplicate(list))
			{
				operationResult.Success = false;
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "ImportEmployeeExistSameName", "导入的文件中存在相同的员工");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text2
				});
				return operationResult;
			}
			List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
			{
				IncludeDisable = true,
				ParentCodes = "1221,2241,2203,1123"
			}, false, false);
			_currencyBiz.CheckCurrencyExist(ctx, list, "MDefaultCyID", list2);
			foreach (BDEmployeesModel item in list)
			{
				COMModelValidateHelper.ValidateModel(ctx, item, list2, IOValidationTypeEnum.Employee);
				_accountBiz.CheckImportAccountExist(ctx, item, bDAccountList, "MCurrentAccountCode", list2, "MCode", false);
				_country.CheckCountryExist(ctx, item, list2, countryList, "MPCountryID", -1);
				_country.CheckCountryExist(ctx, item, list2, countryList, "MRealCountryID", -1);
				item.MExpenseAccountCode = string.Empty;
			}
			if (list.Any(delegate(BDEmployeesModel f)
			{
				decimal? mIncomeTaxThreshold = f.MIncomeTaxThreshold;
				int result;
				if (!(mIncomeTaxThreshold.GetValueOrDefault() < default(decimal) & mIncomeTaxThreshold.HasValue))
				{
					mIncomeTaxThreshold = f.MIncomeTaxThresholdNew;
					result = ((mIncomeTaxThreshold.GetValueOrDefault() < default(decimal) & mIncomeTaxThreshold.HasValue) ? 1 : 0);
				}
				else
				{
					result = 1;
				}
				return (byte)result != 0;
			}))
			{
				list2.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.TaxThreshold,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "TaxThresholdValueError", "起征点需大于或等于0。")
				});
			}
			base.SetValidationResult(ctx, operationResult, list2, false);
			ValidateEmployeeIDNumber(ctx, list, operationResult);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			return _bdEmpls.ImportEmployeeList(ctx, list);
		}

		private void ValidateEmployeeIDNumber(MContext ctx, List<BDEmployeesModel> list, OperationResult result)
		{
			List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID), false, false);
			List<string> list2 = (from f in dataModelList
			where !string.IsNullOrWhiteSpace(f.MIDNumber) && f.MIDType == "1"
			select f.MIDNumber).ToList();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ImportEmployeeIDNumberIsExist", "{0}的证件信息已经存在，请使用其他证件号码");
			foreach (BDEmployeesModel item in list)
			{
				if (!(item.MIDType != "1"))
				{
					if (list2.Contains(item.MIDNumber))
					{
						string userName = GlobalFormat.GetUserName(item.MFirstName, item.MLastName, null);
						result.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = string.Format(text, userName)
						});
					}
					else
					{
						list2.Add(item.MIDNumber);
					}
				}
			}
			result.Success = !result.VerificationInfor.Any();
		}

		private bool CheckEmplyeeHasPayRunData(List<BDEmployeesModel> employeeList)
		{
			bool result = false;
			if (employeeList == null || employeeList.Count == 0)
			{
				return result;
			}
			foreach (BDEmployeesModel employee in employeeList)
			{
				int num;
				if (!(employee.MJoinTime > DateTime.MinValue) && !(employee.MBaseSalary != decimal.Zero))
				{
					decimal? mIncomeTaxThreshold = employee.MIncomeTaxThreshold;
					if ((mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue) && string.IsNullOrWhiteSpace(employee.MIDType) && string.IsNullOrWhiteSpace(employee.MIDNumber) && string.IsNullOrWhiteSpace(employee.MSocialSecurityAccount))
					{
						mIncomeTaxThreshold = employee.MRetirementSecurityPercentage;
						if ((mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue) && !(employee.MRetirementSecurityAmount != decimal.Zero))
						{
							mIncomeTaxThreshold = employee.MMedicalInsurancePercentage;
							if (mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue)
							{
								mIncomeTaxThreshold = employee.MUmemploymentPercentage;
								if ((mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue) && !(employee.MUmemploymentAmount != decimal.Zero) && string.IsNullOrWhiteSpace(employee.MProvidentAccount))
								{
									mIncomeTaxThreshold = employee.MProvidentPercentage;
									if ((mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue) && !(employee.MProvidentAmount != decimal.Zero))
									{
										mIncomeTaxThreshold = employee.MProvidentAdditionalPercentage;
										if ((mIncomeTaxThreshold.GetValueOrDefault() == default(decimal) & mIncomeTaxThreshold.HasValue) && !(employee.MProvidentAdditionalAmount != decimal.Zero) && !(employee.MSocialSecurityBase != decimal.Zero))
										{
											num = ((employee.MHosingProvidentFundBase != decimal.Zero) ? 1 : 0);
											goto IL_0220;
										}
									}
								}
							}
						}
					}
				}
				num = 1;
				goto IL_0220;
				IL_0220:
				if (num != 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private bool CheckNameDuplicate(List<BDEmployeesModel> list)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (BDEmployeesModel item in list)
			{
				MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MFirstName");
				MultiLanguageFieldList multiLanguageFieldList2 = item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MLastName");
				if (multiLanguageFieldList != null && multiLanguageFieldList2 != null)
				{
					foreach (MultiLanguageField item2 in multiLanguageFieldList.MMultiLanguageField)
					{
						MultiLanguageField multiLanguageField = multiLanguageFieldList2.MMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == item2.MLocaleID);
						string userName = GlobalFormat.GetUserName(item2.MValue, multiLanguageField.MValue, null);
						if (!string.IsNullOrWhiteSpace(userName))
						{
							if (!dictionary.ContainsKey(item2.MLocaleID))
							{
								dictionary.Add(item2.MLocaleID, new List<string>
								{
									userName
								});
							}
							else
							{
								dictionary[item2.MLocaleID].Add(userName);
							}
						}
					}
				}
			}
			bool result = false;
			foreach (string key in dictionary.Keys)
			{
				List<IGrouping<string, string>> source = (from v in dictionary[key]
				group v by v.ToLower() into f
				where f.Count() > 1
				select f).ToList();
				if (source.Any())
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, ref List<PAPITThresholdModel> pitList, bool isFromExcel = false)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Country", "Country");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Area", "Area");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Number", "Number");
			string comment = $"({text} {text2} {text3})";
			string comment2 = $"({text} {text3})";
			string commentFmt = "({0})";
			string format = "({0}%)";
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			PAPaySettingModel paySettingModel = PASalaryPaymentRepository.GetPaySettingModel(ctx);
			string comment3 = "-";
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MFirstName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "FirstName", "FirstName", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MLastName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "LastName", "LastName", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MEmail", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Email", "Email", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MUserID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "LinkUser", "Link User", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MSex", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Sex", "Sex", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MStatus", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Status", "Status", null)
				}), false, false, 2, null)
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MCurrentAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "CurrentMoneyAccount", "Current Money Account", null)
					}), false, false, 2, null)
				});
			}
			COMLangInfoModel cOMLangInfoModel = new COMLangInfoModel(LangModule.Contact, "PostalAddress", "Postal Address", comment3);
			COMLangInfoModel cOMLangInfoModel2 = new COMLangInfoModel(LangModule.Contact, "PhysicalAddress", "Physical Address", comment3);
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MPAttention", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Common, "Attention", "Attention", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPStreet", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "StreetAddressORPOBox", "Street Address or PO Box", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPCityID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "TownCity", "Town / City", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPRegion", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "StateRegion", "State / Region", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPPostalNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "PostalZipCode", "Postal / Zip Code", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MPCountryID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel,
					new COMLangInfoModel(LangModule.Contact, "Country", "Country", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealAttention", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Common, "Attention", "Attention", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealStreet", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "StreetAddressORPOBox", "Street Address or PO Box", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealCityID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "TownCity", "Town / City", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealRegion", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "StateRegion", "State / Region", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealPostalNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "PostalZipCode", "Postal / Zip Code", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MRealCountryID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
				{
					cOMLangInfoModel2,
					new COMLangInfoModel(LangModule.Contact, "Country", "Country", null)
				}), false, false, 2, null)
			});
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MPhone", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "PhoneNumber", "Phone Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MFax", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "FaxNumber", "Fax Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MMobile", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "MobileNumber", "Mobile Number", comment2)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MDirectPhone", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "DirectDialNumber", "Direct Dial Number", comment)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MSkypeName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "SkypeNameOrNumber", "Skype Name/Number", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MWebSite", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Website", "Website", null)
				}), false, false, 2, null)
			});
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MDiscount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Discount", "Discount %", null)
				}), IOTemplateFieldType.Decimal, false, false, 2, null),
				new IOTemplateConfigModel("MDefaultCyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "DefaultCurrency", "Default Currency", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankAcctNo", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "BankAccount_Number", "Bank Account Number", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankAccName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Account_Name", "Account Name", null)
				}), false, false, 2, null),
				new IOTemplateConfigModel("MBankName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Details", "Details", null)
				}), false, false, 2, null)
			});
			if (ctx.MOrgVersionID != 1)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MPurDueDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "ExpenseDueDay", "Expense Claims default due day", null)
					}), IOTemplateFieldType.Decimal, false, false, 0, null),
					new IOTemplateConfigModel("MPurDueCondition", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "ExpenseDueDayCondition", "Expense Claims default due day condition", null)
					}), false, false, 2, null)
				});
				pitList = PAPITRepository.GetPITThresholdList(ctx, new PAPITThresholdFilterModel
				{
					IsDefault = true
				});
				List<string> list2 = (from f in pitList
				select string.Format(commentFmt, f.MEffectiveDate.ToString("yyyy"))).ToList();
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MJoinTime", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "JoinTime", "Join Time", null)
					}), IOTemplateFieldType.Date, false, false, 2, null),
					new IOTemplateConfigModel("MBaseSalary", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "BaseSalary", "Base Salary", null)
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MIncomeTaxThreshold", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "Personalincometaxthreshold", "Personal income tax threshold", list2[1])
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MIncomeTaxThresholdNew", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "Personalincometaxthreshold", "Personal income tax threshold", list2[0])
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MIDType", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "IDType", "ID Type", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MIDNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "IDNumber", "ID Number", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MSocialSecurityBase", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "SocialSecurityBase", "Social Security Base", null)
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MSocialSecurityAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "PersonalAccountNumber", "Personal Account Number", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MRetirementSecurityPercentage", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Contact, "BaiscRetirementSecurity", "Baisc Retirement Security", " "),
						new COMLangInfoModel(LangModule.Report, "Percent", "Percent", string.Format(format, paySettingModel.MRetirementSecurityPer))
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MMedicalInsurancePercentage", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Contact, "BasicMedicalInsurance", "Basic Medical Insurance", " "),
						new COMLangInfoModel(LangModule.Report, "Percent", "Percent", string.Format(format, paySettingModel.MMedicalInsurancePer))
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MUmemploymentPercentage", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Contact, "UmemploymentInsurance", "Umemployment Insurance", " "),
						new COMLangInfoModel(LangModule.Report, "Percent", "Percent", string.Format(format, paySettingModel.MUmemploymentInsurancePer))
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MHosingProvidentFundBase", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "HosingProvidentFundBase", "Hosing Provident Fund Base", null)
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MProvidentAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Contact, "PersonalAccountNumber", "Personal Account Number", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MProvidentPercentage", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Contact, "HousingProvidentFund", "Housing Provident Fund", " "),
						new COMLangInfoModel(LangModule.Report, "Percent", "Percent", string.Format(format, paySettingModel.MProvidentFundPer))
					}), IOTemplateFieldType.Decimal, false, false, 2, null),
					new IOTemplateConfigModel("MProvidentAdditionalPercentage", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[2]
					{
						new COMLangInfoModel(LangModule.Contact, "HousingProvidentFund(Additional)", "Housing Provident Fund(Additional)", " "),
						new COMLangInfoModel(LangModule.Report, "Percent", "Percent", string.Format(format, paySettingModel.MAddProvidentFundPer))
					}), IOTemplateFieldType.Decimal, false, false, 2, null)
				});
			}
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, bool isFromExcel = false, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			BDEmployeesModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDEmployeesModel>(ctx);
			List<string> fieldList = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			List<BASLangModel> orgLangList = _langBiz.GetOrgLangList(ctx);
			BASCurrencyViewModel @base = _bdCurrency.GetBase(ctx, false, null, null);
			List<REGCurrencyViewModel> viewList = _bdCurrency.GetViewList(ctx, null, null, false, null);
			List<BDEmployeesModel> orgUserList = _bdEmpls.GetOrgUserList(ctx);
			List<BASCountryModel> countryList = _country.GetCountryList(ctx);
			List<ImportDataSourceInfo> dsLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dsLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			list.Add(new ImportTemplateDataSource(false)
			{
				FieldType = ImportTemplateColumnType.MultiLanguage,
				FieldList = fieldList,
				DataSourceList = dsLangList
			});
			List<ImportDataSourceInfo> dsCurrencyDatasource = new List<ImportDataSourceInfo>();
			if (@base != null)
			{
				dsCurrencyDatasource.Add(new ImportDataSourceInfo
				{
					Key = @base.MCurrencyID,
					Value = $"{@base.MCurrencyID} {@base.MLocalName}"
				});
			}
			viewList.ForEach(delegate(REGCurrencyViewModel f)
			{
				dsCurrencyDatasource.Add(new ImportDataSourceInfo
				{
					Key = f.MCurrencyID,
					Value = $"{f.MCurrencyID} {f.MName}"
				});
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Currency,
				FieldList = new List<string>
				{
					"MDefaultCyID"
				},
				DataSourceList = dsCurrencyDatasource
			});
			List<ImportDataSourceInfo> countryDatasource = new List<ImportDataSourceInfo>();
			countryList.ForEach(delegate(BASCountryModel f)
			{
				countryDatasource.Add(new ImportDataSourceInfo
				{
					Key = f.MItemID,
					Value = f.MCountryName
				});
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Country,
				FieldList = new List<string>
				{
					"MPCountryID",
					"MRealCountryID"
				},
				DataSourceList = countryDatasource
			});
			List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
			foreach (BDEmployeesModel item in orgUserList)
			{
				if (!string.IsNullOrWhiteSpace(item.MEmail))
				{
					list2.Add(new ImportDataSourceInfo
					{
						Key = item.MUserID,
						Value = item.MEmail
					});
				}
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.LinkUser,
				FieldList = new List<string>
				{
					"MUserID"
				},
				DataSourceList = list2
			});
			List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
			list3.Add(new ImportDataSourceInfo
			{
				Key = "Male",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Male", "Male")
			});
			list3.Add(new ImportDataSourceInfo
			{
				Key = "Female",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Female", "Female")
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Sex,
				FieldList = new List<string>
				{
					"MSex"
				},
				DataSourceList = list3
			});
			List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
			list4.Add(new ImportDataSourceInfo
			{
				Key = "None",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "None", "None")
			});
			list4.Add(new ImportDataSourceInfo
			{
				Key = "Probation",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Probation", "Probation")
			});
			list4.Add(new ImportDataSourceInfo
			{
				Key = "Regular",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Regular", "Regular")
			});
			list4.Add(new ImportDataSourceInfo
			{
				Key = "Leave",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Leave", "Leave")
			});
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.EmpStatus,
				FieldList = new List<string>
				{
					"MStatus"
				},
				DataSourceList = list4
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
				{
					IsActive = true,
					ParentCodes = "1221,2241,2203,1123"
				}, false, false);
				List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item2 in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item2.MCode))
					{
						list5.Add(new ImportDataSourceInfo
						{
							Key = item2.MCode,
							Value = item2.MFullName
						});
					}
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.EmpCurrentAccount,
					FieldList = new List<string>
					{
						"MCurrentAccountCode"
					},
					DataSourceList = list5
				});
			}
			if (ctx.MOrgVersionID != 1)
			{
				List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
				list6.Add(new ImportDataSourceInfo
				{
					Key = "1",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "IdentityCard", "Identity Card")
				});
				list6.Add(new ImportDataSourceInfo
				{
					Key = "2",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Passport", "Passport")
				});
				list6.Add(new ImportDataSourceInfo
				{
					Key = "0",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Other", "Other")
				});
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.IDType,
					FieldList = new List<string>
					{
						"MIDType"
					},
					DataSourceList = list6
				});
				List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
				list7.Add(new ImportDataSourceInfo
				{
					Key = "item0",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "OfTheFollowingMonth", "of the following month")
				});
				list7.Add(new ImportDataSourceInfo
				{
					Key = "item1",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "DayAfterTheExpenseClaimsDate", "day(s) after the expense claims date")
				});
				list7.Add(new ImportDataSourceInfo
				{
					Key = "item2",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "DayAfterEndExpenseClaimsMonth", "day(s) after the end of the expense claims month")
				});
				list7.Add(new ImportDataSourceInfo
				{
					Key = "item3",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "OfTheCurrentMonth", "of the current month")
				});
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.DueDate,
					FieldList = new List<string>
					{
						"MPurDueCondition"
					},
					DataSourceList = list7
				});
			}
			return list;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<PAPITThresholdModel> list = new List<PAPITThresholdModel>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, ref list, false);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary, false, dictionary2);
			dictionary2.Add("MFirstName", new string[1]
			{
				"Lizzie"
			});
			dictionary2.Add("MLastName", new string[1]
			{
				"Ma"
			});
			if (ctx.MOrgVersionID != 1)
			{
				dictionary2.Add("MJoinTime", new string[1]
				{
					"2014/10/31"
				});
				dictionary2.Add("MBaseSalary", new string[1]
				{
					"8000.00"
				});
				Dictionary<string, string[]> dictionary3 = dictionary2;
				string[] obj = new string[1];
				decimal mAmount = list[1].MAmount;
				obj[0] = mAmount.ToString();
				dictionary3.Add("MIncomeTaxThreshold", obj);
				Dictionary<string, string[]> dictionary4 = dictionary2;
				string[] obj2 = new string[1];
				mAmount = list[0].MAmount;
				obj2[0] = mAmount.ToString();
				dictionary4.Add("MIncomeTaxThresholdNew", obj2);
				dictionary2.Add("MIDNumber", new string[1]
				{
					"36043019890618051X"
				});
				dictionary2.Add("MSocialSecurityBase", new string[1]
				{
					"6000.00"
				});
				dictionary2.Add("MHosingProvidentFundBase", new string[1]
				{
					"6000.00"
				});
				dictionary2.Add("MSocialSecurityAccount", new string[1]
				{
					"6222084000062652233"
				});
				dictionary2.Add("MProvidentAccount", new string[1]
				{
					"625936123211000321"
				});
				dictionary2.Add("MRetirementSecurityPercentage", new string[1]
				{
					"2.00"
				});
				dictionary2.Add("MMedicalInsurancePercentage", new string[1]
				{
					"2.00"
				});
				dictionary2.Add("MUmemploymentPercentage", new string[1]
				{
					"2.00"
				});
				dictionary2.Add("MProvidentPercentage", new string[1]
				{
					"3.00"
				});
				dictionary2.Add("MProvidentAdditionalPercentage", new string[1]
				{
					"3.00"
				});
			}
			List<string> alignRightFieldList = "MPurDueDate,MSalDueDate".Split(',').ToList();
			List<string> requiredColumnList = new List<string>
			{
				"ContactType"
			}.Union((from f in templateConfig
			where f.MIsRequired
			select f.MFieldName).ToList()).ToList();
			return new ImportTemplateModel
			{
				TemplateType = "Employee",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = requiredColumnList,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				AlignRightFieldList = alignRightFieldList
			};
		}

		public OperationResult RestoreEmployee(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			if (string.IsNullOrWhiteSpace(param.KeyIDs))
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<string> ids = param.KeyIDs.Split(',').ToList();
			return _bdEmpls.RestoreEmployee(ctx, ids);
		}

		public List<CommandInfo> CheckEmployeeExist<T>(MContext ctx, List<T> modelList, string fieldName, List<IOValidationResultModel> validationResult, ref string newEmps, bool createNewEmp = false, bool isCheckId = false)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDEmployeesModel> employeeList = GetEmployeeList(ctx, true);
			List<BDEmployeesModel> newEmployeeList = new List<BDEmployeesModel>();
			foreach (T model in modelList)
			{
				list.AddRange((IEnumerable<CommandInfo>)CheckEmployeeExist(ctx, model, ref employeeList, validationResult, fieldName, ref newEmps, createNewEmp, "MItemID", isCheckId, -1, newEmployeeList));
			}
			return list;
		}

		public List<BDEmployeesModel> GetEmployeeList(MContext ctx, bool includeDisable)
		{
			return apiDal.GetEmployeeList(ctx, includeDisable);
		}

		public List<CommandInfo> CheckEmployeeExist<T>(MContext ctx, T model, ref List<BDEmployeesModel> empList, List<IOValidationResultModel> validationResult, string fieldName, string idFieldName = "MItemID", int rowIndex = -1)
		{
			string empty = string.Empty;
			return CheckEmployeeExist(ctx, model, ref empList, validationResult, fieldName, ref empty, false, idFieldName, false, rowIndex, null);
		}

		public List<CommandInfo> CheckEmployeeExist<T>(MContext ctx, T model, ref List<BDEmployeesModel> empList, List<IOValidationResultModel> validationResult, string fieldName, ref string newEmps, bool createNewEmp = false, string idFieldName = "MItemID", bool isCheckId = false, int rowIndex = -1, List<BDEmployeesModel> newEmployeeList = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (empList == null)
			{
				empList = _bdEmpls.GetEmployeeList(ctx, false);
			}
			string empName = ModelHelper.GetModelValue(model, fieldName);
			newEmployeeList = (newEmployeeList ?? new List<BDEmployeesModel>());
			if (!string.IsNullOrWhiteSpace(empName))
			{
				if (isCheckId)
				{
					BDEmployeesModel bDEmployeesModel = empList.FirstOrDefault((BDEmployeesModel m) => m.MItemID == empName);
					if (bDEmployeesModel == null)
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
							Id = ModelHelper.GetModelValue(model, idFieldName),
							FieldType = IOValidationTypeEnum.Employee,
							FieldValue = empName,
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EmployeeNotFound", "The employee:{0} can't be found!"),
							RowIndex = rowIndex2
						});
					}
				}
				else
				{
					string text = string.Empty;
					BDEmployeesModel bDEmployeesModel2 = empList.FirstOrDefault((BDEmployeesModel f) => (!string.IsNullOrWhiteSpace(f.MFullName) && HttpUtility.HtmlDecode(f.MFullName).ToUpper().Trim() == empName.ToUpper().Trim()) || f.MItemID == empName);
					if (bDEmployeesModel2 == null)
					{
						bDEmployeesModel2 = newEmployeeList.FirstOrDefault((BDEmployeesModel f) => !string.IsNullOrWhiteSpace(f.MFullName) && HttpUtility.HtmlDecode(f.MFullName).ToUpper().Trim() == empName.ToUpper().Trim());
					}
					if (bDEmployeesModel2 != null)
					{
						if (!bDEmployeesModel2.MIsActive)
						{
							text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "EmployeeHasDisabled", "员工：{0}已禁用！");
						}
						else
						{
							ModelHelper.SetModelValue(model, fieldName, bDEmployeesModel2.MItemID, null);
						}
					}
					else
					{
						newEmps = empName;
						if (createNewEmp)
						{
							BDEmployeesModel bDEmployeesModel3 = new BDEmployeesModel();
							if (string.IsNullOrWhiteSpace(bDEmployeesModel3.MStatus))
							{
								bDEmployeesModel3.MStatus = "None";
							}
							string value = bDEmployeesModel3.MItemID = UUIDHelper.GetGuid();
							bDEmployeesModel3.IsNew = true;
							string empty = string.Empty;
							string langName = string.Empty;
							bDEmployeesModel3.MFullName = empName;
							string pattern = "[一-龥]+";
							if (Regex.IsMatch(empName, pattern))
							{
								int num = empName.ToCharArray().Length;
								if (num == 1)
								{
									empty = empName;
								}
								else
								{
									langName = empName.Substring(0, 1);
									empty = empName.Substring(1);
								}
							}
							else
							{
								string[] array = empName.Split(' ');
								empty = array[0];
								if (array.Length > 1)
								{
									if (array.Length == 2)
									{
										langName = array[1];
									}
									else
									{
										List<string> list2 = new List<string>();
										string[] array2 = array;
										foreach (string item in array2)
										{
											list2.Add(item);
										}
										langName = string.Join(" ", list2);
									}
								}
							}
							bDEmployeesModel3.MOrgID = ctx.MOrgID;
							bDEmployeesModel3.MultiLanguage = new List<MultiLanguageFieldList>();
							bDEmployeesModel3.MultiLanguage.Add(base.GetMultiLanguageList("MFirstName", empty));
							bDEmployeesModel3.MultiLanguage.Add(base.GetMultiLanguageList("MLastName", langName));
							ModelHelper.SetModelValue(model, fieldName, value, null);
							list.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmds(ctx, new List<BDEmployeesModel>
							{
								bDEmployeesModel3
							}, null, true));
							newEmployeeList.Add(bDEmployeesModel3);
						}
						else
						{
							text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EmployeeNotFound", "The employee:{0} can't be found!");
						}
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						int rowIndex3 = 0;
						if (rowIndex != -1)
						{
							rowIndex3 = rowIndex;
						}
						else
						{
							int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex3);
						}
						validationResult.Add(new IOValidationResultModel
						{
							Id = ModelHelper.TryGetModelValue(model, idFieldName),
							FieldType = IOValidationTypeEnum.Employee,
							FieldValue = empName,
							Message = text,
							RowIndex = rowIndex3
						});
					}
				}
			}
			return list;
		}

		public BDEmployeesModel GetInsertEmployeeModel(MContext ctx, string firstName, string lastName, List<BASLangModel> languageList)
		{
			BDEmployeesModel bDEmployeesModel = new BDEmployeesModel();
			bDEmployeesModel.MItemID = UUIDHelper.GetGuid();
			bDEmployeesModel.MFirstName = firstName;
			bDEmployeesModel.MLastName = lastName;
			bDEmployeesModel.IsNew = true;
			bDEmployeesModel.MStatus = "None";
			if (languageList == null)
			{
				languageList = new BASLangBusiness().GetOrgLangList(ctx);
			}
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList.MFieldName = "MFirstName";
			foreach (BASLangModel language in languageList)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = language.LangID;
				multiLanguageField.MOrgID = ctx.MOrgID;
				multiLanguageField.MValue = firstName;
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			MultiLanguageFieldList multiLanguageFieldList2 = new MultiLanguageFieldList();
			multiLanguageFieldList2.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList2.MFieldName = "MLastName";
			foreach (BASLangModel language2 in languageList)
			{
				MultiLanguageField multiLanguageField2 = new MultiLanguageField();
				multiLanguageField2.MLocaleID = language2.LangID;
				multiLanguageField2.MOrgID = ctx.MOrgID;
				multiLanguageField2.MValue = lastName;
				multiLanguageFieldList2.MMultiLanguageField.Add(multiLanguageField2);
			}
			bDEmployeesModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDEmployeesModel.MultiLanguage.Add(multiLanguageFieldList);
			bDEmployeesModel.MultiLanguage.Add(multiLanguageFieldList2);
			return bDEmployeesModel;
		}

		public List<string> AnalysisEmployeeName(MContext ctx, string employeeName)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrWhiteSpace(employeeName))
			{
				return list;
			}
			string pattern = "[一-龥]+";
			if (Regex.IsMatch(employeeName, pattern))
			{
				int num = employeeName.ToCharArray().Length;
				if (num == 1)
				{
					list.Add(employeeName);
					list.Add(employeeName);
				}
				else
				{
					list.Add(employeeName.Substring(1));
					list.Add(employeeName.Substring(0, 1));
				}
			}
			else
			{
				string[] array = employeeName.Split(' ');
				list.Add(array[0]);
				if (array.Length > 1)
				{
					if (array.Length == 2)
					{
						list.Add(array[1]);
					}
					else
					{
						List<string> list2 = new List<string>();
						string[] array2 = array;
						foreach (string item in array2)
						{
							list2.Add(item);
						}
						list.Add(string.Join(" ", list2));
					}
				}
			}
			return list;
		}
	}
}
