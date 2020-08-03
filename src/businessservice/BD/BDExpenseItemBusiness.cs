using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDExpenseItemBusiness : APIBusinessBase<BDExpenseItemModel>, IBDExpenseItemBusiness
	{
		private List<BDExpenseItemModel> _expenseItemDataPool;

		private BDExpenseItemRepository expItem = new BDExpenseItemRepository();

		private IBASLangBusiness _lang = new BASLangBusiness();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		protected override DataGridJson<BDExpenseItemModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_expenseItemDataPool = instance.ExpenseItems;
			return expItem.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, BDExpenseItemModel model)
		{
			BDExpenseItemModel bDExpenseItemModel = _expenseItemDataPool.FirstOrDefault((BDExpenseItemModel f) => f.MItemID == model.MItemID);
			if (bDExpenseItemModel != null)
			{
				model.MName = bDExpenseItemModel.MName;
				model.MDesc = bDExpenseItemModel.MDesc;
			}
			if (model.MParentExpenseItemModel != null && !string.IsNullOrWhiteSpace(model.MParentExpenseItemModel.MExpenseItemID))
			{
				BDExpenseItemModel bDExpenseItemModel2 = _expenseItemDataPool.FirstOrDefault((BDExpenseItemModel f) => f.MItemID == model.MParentExpenseItemModel.MExpenseItemID);
				if (bDExpenseItemModel2 != null)
				{
					//model.MParentExpenseItemModel.set_MName((object)bDExpenseItemModel2.MName);
					model.MParentExpenseItemModel.MName = bDExpenseItemModel2.MName;
				}
			}
		}

		public DataGridJson<BDExpenseItemModel> GetList(MContext ctx, GetParam param)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (!param.IncludeDisabled)
			{
				sqlWhere.Equal("MIsActive", 1);
			}
			if (param.ModifiedSince > DateTime.MinValue)
			{
				sqlWhere.GreaterThen("MModifyDate", param.ModifiedSince);
			}
			if (!string.IsNullOrWhiteSpace(param.ElementID))
			{
				sqlWhere.Equal("MItemID", param.ElementID);
			}
			sqlWhere.PageIndex = param.PageIndex;
			sqlWhere.PageSize = param.PageSize;
			sqlWhere.WhereSqlString = param.Where;
			DataGridJson<BDExpenseItemModel> modelPageList = expItem.GetModelPageList(ctx, sqlWhere, false);
			SetMultiKeyValue(modelPageList.rows);
			return modelPageList;
		}

		public List<BDExpenseItemModel> GetList(MContext ctx, bool isActive = true)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (isActive)
			{
				sqlWhere.Equal("MIsActive", 1);
			}
			else
			{
				sqlWhere.Equal("MIsActive", 0);
			}
			List<BDExpenseItemModel> list = expItem.GetModelList(ctx, sqlWhere, false);
			SetMultiKeyValue(list);
			if (list != null && list.Count() > 0)
			{
				list = (from x in list
				orderby x.MName.Trim()
				select x).ToList();
			}
			return list;
		}

		public List<BDExpenseItemModel> GetListByTier(MContext ctx, bool includeDisable = false, bool isAddChildren = false)
		{
			List<BDExpenseItemModel> list = new List<BDExpenseItemModel>();
			List<BDExpenseItemModel> parentExpenseItemList = expItem.GetParentExpenseItemList(ctx, !includeDisable, false, true);
			if (parentExpenseItemList == null)
			{
				return list;
			}
			IOrderedEnumerable<BDExpenseItemModel> orderedEnumerable = from x in parentExpenseItemList
			where x.MParentItemID == "0"
			select x into t
			orderby t.MName
			select t;
			IOrderedEnumerable<BDExpenseItemModel> source = from x in parentExpenseItemList
			where x.MParentItemID != null && x.MParentItemID != "0"
			select x into t
			orderby t.MName
			select t;
			List<BDAccountModel> acctList = new List<BDAccountModel>();
			if (isAddChildren)
			{
				List<string> list2 = (from f in parentExpenseItemList
				select f.MAccountCode into v
				where !string.IsNullOrWhiteSpace(v)
				select v).ToList();
				if (list2.Any())
				{
					acctList = _accountBiz.GetBDAccountList(ctx, new SqlWhere().In("MCode", list2), true);
				}
			}
			foreach (BDExpenseItemModel item in orderedEnumerable)
			{
				List<BDExpenseItemModel> list3 = (from t in source
				where t.MParentItemID == item.MItemID
				select t).ToList();
				if (list3 == null || list3.Count == 0)
				{
					SetExpItemAccount(acctList, item);
					list.Add(item);
				}
				else
				{
					if (isAddChildren)
					{
						SetExpItemAccount(acctList, item);
						if (item.Chileren == null)
						{
							item.Chileren = new List<BDExpenseItemModel>();
						}
					}
					foreach (BDExpenseItemModel item2 in list3)
					{
						item2.MGroupName = item.MName;
						if (isAddChildren)
						{
							SetExpItemAccount(acctList, item2);
							item.Chileren.Add(item2);
						}
						else
						{
							list.Add(item2);
						}
					}
					if (isAddChildren)
					{
						list.Add(item);
					}
				}
			}
			return (from t in list
			orderby t.MGroupName
			select t).ToList();
		}

		private static void SetExpItemAccount(List<BDAccountModel> acctList, BDExpenseItemModel item)
		{
			if (!string.IsNullOrWhiteSpace(item.MAccountCode) && acctList.Any())
			{
				BDAccountModel bDAccountModel = acctList.FirstOrDefault((BDAccountModel f) => f.MCode == item.MAccountCode);
				if (bDAccountModel != null)
				{
					item.MAccountNumber = bDAccountModel.MFullName;
				}
			}
		}

		public List<BDExpenseItemModel> GetNoParentList(MContext ctx)
		{
			List<BDExpenseItemModel> modelList = expItem.GetModelList(ctx, new SqlWhere(), false);
			List<BDExpenseItemModel> result = null;
			SetMultiKeyValue(modelList);
			IEnumerable<BDExpenseItemModel> enumerable = from x in modelList
			where x.MParentItemID == "0"
			select x;
			if (enumerable != null && enumerable.Count() > 0)
			{
				result = (((from x in modelList
				where x.MParentItemID != "0"
				select x) != null) ? (from x in modelList
				where x.MParentItemID != "0"
				select x).ToList() : new List<BDExpenseItemModel>());
				foreach (BDExpenseItemModel item in enumerable)
				{
					if (!modelList.Any((BDExpenseItemModel x) => x.MParentItemID == item.MItemID))
					{
						result.Add(item);
					}
				}
				result = (from x in result
				orderby x.MParentItemID
				select x).ToList();
			}
			return result;
		}

		public List<BDExpenseItemModel> GetParentList(MContext ctx, string itemId = null)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MParentID", "0");
			List<BDExpenseItemModel> list = expItem.GetParentExpenseItemList(ctx, true, false, false);
			if (!string.IsNullOrEmpty(itemId))
			{
				IEnumerable<BDExpenseItemModel> enumerable = from x in list
				where x.MItemID != itemId
				select x;
				list = ((enumerable == null) ? new List<BDExpenseItemModel>() : enumerable.ToList());
			}
			return list;
		}

		public DataGridJson<BDExpenseItemModel> GetPageList(MContext ctx, BDExpenseItemListFilterModel filter)
		{
			DataGridJson<BDExpenseItemModel> modelPageList = expItem.GetModelPageList(ctx, filter, false);
			SetMultiKeyValue(modelPageList.rows);
			return modelPageList;
		}

		private void SetMultiKeyValue(List<BDExpenseItemModel> list)
		{
			foreach (BDExpenseItemModel item in list)
			{
				MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				if (multiLanguageFieldList != null)
				{
					item.MName = multiLanguageFieldList.MMultiLanguageValue;
				}
				multiLanguageFieldList = item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MDesc");
				if (multiLanguageFieldList != null)
				{
					item.MDesc = multiLanguageFieldList.MMultiLanguageValue;
				}
			}
		}

		public BDExpenseItemModel GetEditInfo(MContext ctx, BDExpenseItemModel model)
		{
			BDExpenseItemModel dataModel = expItem.GetDataModel(ctx, model.MItemID, false);
			if (dataModel != null && ctx.MRegProgress == 15)
			{
				string value = (dataModel.MAccountCode == null) ? "6602" : dataModel.MAccountCode;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("MCode", value);
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() > 0)
				{
					BDAccountModel bDAccountModel = baseBDAccountList.First();
					dataModel.MAccountId = bDAccountModel.MItemID;
				}
			}
			return dataModel;
		}

		public OperationResult Update(MContext ctx, BDExpenseItemModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> codeList = new List<string>
			{
				model.MAccountCode
			};
			if (!_accountBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				if (string.IsNullOrWhiteSpace(model.MItemID))
				{
					return operationResult;
				}
				model.MAccountCode = "";
				operationResult.Success = true;
			}
			model.MOrgID = ctx.MOrgID;
			if (string.IsNullOrEmpty(model.MParentItemID))
			{
				model.MParentItemID = "0";
			}
			else if (model.MParentItemID != "0")
			{
				OperationResult operationResult2 = BDRepository.IsCanDelete(ctx, "ExpenseItem", model.MParentItemID);
				List<BDExpenseItemModel> noParentList = GetNoParentList(ctx);
				if (!operationResult2.Success && (noParentList == null || !noParentList.Exists((BDExpenseItemModel x) => x.MParentItemID == model.MParentItemID)))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuotedError", "费用项目已经被应用，不允许新增下级项目");
					return operationResult;
				}
				if (!string.IsNullOrWhiteSpace(model.MItemID))
				{
					List<BDExpenseItemModel> dataModelList = ModelInfoManager.GetDataModelList<BDExpenseItemModel>(ctx, new SqlWhere().In("MParentItemID", new string[1]
					{
						model.MItemID
					}), false, false);
					if (dataModelList.Any())
					{
						operationResult.Success = false;
						operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemSetParentError", "费用项目存在下级项目，不允许设置上一级费用项目");
						return operationResult;
					}
				}
			}
			//bool flag = false;
			if ((!(model.MParentItemID == "0")) ? ModelInfoManager.IsLangColumnValueExists<BDExpenseItemModel>(ctx, "MName", model.MultiLanguage, model.MItemID, "MParentItemID", model.MParentItemID, false) : ModelInfoManager.IsLangColumnValueExists<BDExpenseItemModel>(ctx, "MName", model.MultiLanguage, model.MItemID, "MParentItemID", "0", false))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuotedBBB", "同一级中存在相同的名称或者子级名称与父级相同");
				return operationResult;
			}
			return ModelInfoManager.InsertOrUpdate<BDExpenseItemModel>(ctx, model, null);
		}

		public OperationResult IsExists(MContext ctx, List<BDExpenseItemModel> listExpense, BDExpenseItemModel model, OperationResult result)
		{
			foreach (BDExpenseItemModel item in listExpense)
			{
				if (!(item.MItemID == model.MItemID))
				{
					List<MultiLanguageFieldList> list = (from x in model.MultiLanguage
					where !string.IsNullOrWhiteSpace(x.MFieldName) && x.MFieldName == "MName"
					select x).ToList();
					foreach (MultiLanguageFieldList item2 in list)
					{
						List<MultiLanguageField> mMultiLanguageField = item2.MMultiLanguageField;
						foreach (MultiLanguageField item3 in mMultiLanguageField)
						{
							if (ctx.MLCID == item3.MLocaleID && item.MName == item3.MValue)
							{
								result.Success = false;
								result.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuotedBBB", "同一级中存在相同的名称或者子级名称与父级相同");
								return result;
							}
						}
					}
				}
			}
			return result;
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "ExpenseItem", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public OperationResult DeleteExpItem(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = new OperationResult();
			string keyIDs = param.KeyIDs;
			if (keyIDs.IndexOf(',') >= 0)
			{
				List<string> list = new List<string>();
				operationResult = BDRepository.IsCanDelete(ctx, "ExpenseItem", keyIDs, out list);
				if (list.Count != 0)
				{
					List<BDExpenseItemModel> noParentList = GetNoParentList(ctx);
					List<string> list2 = new List<string>();
					foreach (string item in list)
					{
						OperationResult operationResult2 = ChildrenItemIsQuote(ctx, item, noParentList);
						if (!operationResult2.Success)
						{
							list2.Add(item);
						}
					}
					if (list2.Count > 0)
					{
						expItem.DeleteModels(ctx, list2);
					}
				}
			}
			else
			{
				operationResult = BDRepository.IsCanDelete(ctx, "ExpenseItem", keyIDs);
				if (operationResult.Success)
				{
					List<BDExpenseItemModel> noParentList2 = GetNoParentList(ctx);
					OperationResult operationResult3 = ChildrenItemIsQuote(ctx, keyIDs, noParentList2);
					if (!operationResult3.Success)
					{
						expItem.DeleteModels(ctx, keyIDs.Split(',').ToList());
					}
					else
					{
						operationResult.Success = false;
						operationResult.Message = operationResult3.Message;
					}
				}
			}
			return operationResult;
		}

		private OperationResult ChildrenItemIsQuote(MContext ctx, string itemId, List<BDExpenseItemModel> childList)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = false;
			BDExpenseItemModel dataModel = expItem.GetDataModel(ctx, itemId, false);
			if (dataModel != null && dataModel.MParentItemID == "0")
			{
				IEnumerable<BDExpenseItemModel> enumerable = from x in childList
				where x.MParentItemID == itemId
				select x;
				if (enumerable != null && enumerable.Count() > 0)
				{
					List<string> list = (from x in enumerable
					select x.MItemID).ToList();
					foreach (string item in list)
					{
						OperationResult operationResult2 = BDRepository.IsCanDelete(ctx, "ExpenseItem", item);
						if (!operationResult2.Success)
						{
							operationResult.Success = true;
							operationResult.Message = operationResult2.Message;
							break;
						}
					}
				}
			}
			return operationResult;
		}

		public BDExpenseItemModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return expItem.GetDataModel(ctx, pkID, includeDelete);
		}

		public OperationResult ArchiveItem(MContext ctx, string itemIds, bool isRestore = false)
		{
			int num = expItem.ArchievExpenseItem(ctx, itemIds, isRestore);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public void CheckExpenseItemExist<T>(MContext ctx, List<T> modelList, List<IOValidationResultModel> validationResult, string fieldName = "MItemID")
		{
			List<BDExpenseItemModel> childrenExpenseItemList = expItem.GetChildrenExpenseItemList(ctx, true, true);
			foreach (T model in modelList)
			{
				CheckExpenseItemExist(ctx, model, ref childrenExpenseItemList, validationResult, fieldName, -1);
			}
		}

		public void CheckExpenseItemExist<T>(MContext ctx, T model, ref List<BDExpenseItemModel> expenseItemList, List<IOValidationResultModel> validationResult, string expenseItemField, int rowIndex = -1)
		{
			if (expenseItemList == null)
			{
				expenseItemList = expItem.GetChildrenExpenseItemList(ctx, true, true);
			}
			string text = string.Empty;
			string expenseItemName = ModelHelper.GetModelValue(model, expenseItemField);
			if (!string.IsNullOrWhiteSpace(expenseItemName))
			{
				BDExpenseItemModel bDExpenseItemModel = expenseItemList.FirstOrDefault((BDExpenseItemModel f) => f.MItemID == expenseItemName || (!string.IsNullOrWhiteSpace(f.MName) && HttpUtility.HtmlDecode(f.MName.Trim()) == expenseItemName));
				if (bDExpenseItemModel != null)
				{
					if (!bDExpenseItemModel.MIsActive)
					{
						text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemHasDisabled", "费用项目：{0}已禁用！");
					}
					else
					{
						ModelHelper.SetModelValue(model, expenseItemField, bDExpenseItemModel.MItemID, null);
					}
				}
				else
				{
					text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExpenseItemNotFound", "The Expense Item:{0} can't be found!");
				}
				if (!string.IsNullOrWhiteSpace(text))
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
						FieldType = IOValidationTypeEnum.ExpenseItem,
						FieldValue = expenseItemName,
						Message = text,
						RowIndex = rowIndex2
					});
				}
			}
		}

		public OperationResult ImportExpenseItemsList(MContext ctx, List<BDExpenseItemModel> models)
		{
			return ImportExpenseItemsList(ctx, models, InvokeSourceEnum.Import);
		}

		public OperationResult ImportExpenseItemsList(MContext ctx, List<BDExpenseItemModel> models, InvokeSourceEnum source = InvokeSourceEnum.Import)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.VerificationInfor = new List<BizVerificationInfor>();
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDExpenseItemModel> list2 = new List<BDExpenseItemModel>();
			foreach (BDExpenseItemModel model in models)
			{
				COMModelValidateHelper.ValidateModel(ctx, model, operationResult);
			}
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (source == InvokeSourceEnum.Import)
			{
				List<IOValidationResultModel> list3 = new List<IOValidationResultModel>();
				List<string> list4 = new List<string>();
				List<string> list5 = new List<string>();
				List<BDExpenseItemModel> parentExpenseItemList = expItem.GetParentExpenseItemList(ctx, false, true, true);
				IEnumerable<BDExpenseItemModel> source2 = from f in parentExpenseItemList
				where f.MParentItemID == "0"
				select f;
				IEnumerable<BDExpenseItemModel> source3 = from f in parentExpenseItemList
				where f.MParentItemID != "0"
				select f;
				List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel
				{
					IncludeDisable = true
				}, false, false);
				IEnumerable<BDExpenseItemModel> enumerable = from f in models
				where string.IsNullOrWhiteSpace(f.MParentItemID)
				select f;
				foreach (BDExpenseItemModel item in enumerable)
				{
					List<string> importNameList = new List<string>();
					if (item.MultiLanguage != null)
					{
						importNameList = (from f in item.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField
						select f.MValue).ToList();
					}
					BDExpenseItemModel bDExpenseItemModel = source2.FirstOrDefault((BDExpenseItemModel f) => !string.IsNullOrWhiteSpace(f.MName) && importNameList.Contains(f.MName.Trim()));
					if (bDExpenseItemModel != null)
					{
						list4.Add(item.MName);
					}
					else
					{
						item.MItemID = UUIDHelper.GetGuid();
						item.IsNew = true;
						IEnumerable<BDExpenseItemModel> enumerable2 = from f in models
						where f.MParentItemID == item.MName
						select f;
						foreach (BDExpenseItemModel item2 in enumerable2)
						{
							item2.MParentItemID = item.MItemID;
							item2.IsNew = true;
							item2.MParentItemName = item.MName;
						}
					}
				}
				IEnumerable<BDExpenseItemModel> enumerable3 = from c in models
				where !string.IsNullOrWhiteSpace(c.MParentItemID) && !models.Any((BDExpenseItemModel p) => (c.MRowIndex != p.MRowIndex && c.MParentItemID == p.MName) || c.MParentItemID == p.MItemID)
				select c;
				foreach (BDExpenseItemModel item3 in enumerable3)
				{
					BDExpenseItemModel bDExpenseItemModel2 = source2.FirstOrDefault((BDExpenseItemModel f) => (!string.IsNullOrWhiteSpace(f.MName) && f.MName.Trim() == item3.MParentItemID) || f.MItemID == item3.MParentItemID);
					if (bDExpenseItemModel2 == null)
					{
						bDExpenseItemModel2 = list2.FirstOrDefault((BDExpenseItemModel f) => f.MName == item3.MParentItemID);
						if (bDExpenseItemModel2 == null)
						{
							bDExpenseItemModel2 = GetNewExpenseItemModel(item3.MParentItemID);
							list2.Add(bDExpenseItemModel2);
						}
					}
					if (bDExpenseItemModel2.MIsActive)
					{
						item3.MParentItemID = bDExpenseItemModel2.MItemID;
						item3.MParentItemName = bDExpenseItemModel2.MName;
						item3.IsNew = true;
					}
					else
					{
						list5.Add(bDExpenseItemModel2.MName);
					}
				}
				List<IGrouping<string, BDExpenseItemModel>> list6 = (from f in models
				where !string.IsNullOrWhiteSpace(f.MParentItemID)
				select f into g
				group g by g.MParentItemID).ToList();
				foreach (IGrouping<string, BDExpenseItemModel> item4 in list6)
				{
					List<BDExpenseItemModel> list7 = (from f in source3.ToList()
					where f.MParentItemID == item4.Key
					select f).ToList();
					if (list7.Any())
					{
						List<BDExpenseItemModel> list8 = item4.ToList();
						foreach (BDExpenseItemModel item5 in list8)
						{
							List<MultiLanguageField> list9 = item5.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName")?.MMultiLanguageField.ToList();
							if (list9 != null)
							{
								foreach (MultiLanguageField item6 in list9)
								{
									foreach (BDExpenseItemModel item7 in list7)
									{
										List<MultiLanguageField> list10 = item7.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName")?.MMultiLanguageField.ToList();
										if (list10 != null && list10.Exists((MultiLanguageField a) => a.MLocaleID == item6.MLocaleID && a.MValue == item6.MValue))
										{
											list4.Add(item6.MValue);
											if (item5.MIsActive)
											{
												list5.Add(item6.MValue);
											}
										}
									}
								}
							}
						}
					}
				}
				if (list4.Any())
				{
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemNameExist", "该费用项目：{0}已经存在！");
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(text, string.Join("、", list4.Distinct()))
					});
				}
				if (list5.Any())
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemDisabled", "费用项目：{0}已被禁用！");
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(text2, string.Join("、", list5.Distinct()))
					});
				}
				if (operationResult.VerificationInfor.Any())
				{
					return operationResult;
				}
				if (list2.Any())
				{
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
				}
				List<string> list11 = new List<string>();
				IEnumerable<IGrouping<string, BDExpenseItemModel>> enumerable4 = from x in models
				group x by x.MParentItemID;
				foreach (IGrouping<string, BDExpenseItemModel> item8 in enumerable4)
				{
					List<BDExpenseItemModel> expItemList = item8.ToList();
					ValidateDuplicateImportName(list11, expItemList);
				}
				if (list11.Any())
				{
					string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemNameDuplicate", "同级费用项目不能重名（{0}）");
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(text3, string.Join("、", list11.Distinct()))
					});
					return operationResult;
				}
				if (operationResult.VerificationInfor.Any())
				{
					return operationResult;
				}
				foreach (BDExpenseItemModel model2 in models)
				{
					_accountBiz.CheckImportAccountExist(ctx, model2, bDAccountList, "MAccountCode", list3, "MCode", true);
				}
				if (list3.Any())
				{
					string message = string.Format(list3[0].Message, string.Join("、", (from f in list3
					select f.FieldValue).Distinct()));
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = message
					});
					return operationResult;
				}
			}
			List<string> codeList = (from m in models
			select m.MAccountCode).ToList();
			if (!_accountBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				operationResult.Success = false;
			}
			List<BDExpenseItemModel> noParentList = GetNoParentList(ctx);
			List<BDExpenseItemModel> parentExpenseItemList2 = expItem.GetParentExpenseItemList(ctx, false, false, false);
			int num = 0;
			List<string> list12 = new List<string>();
			foreach (BDExpenseItemModel model3 in models)
			{
				model3.MOrgID = ctx.MOrgID;
				if (string.IsNullOrEmpty(model3.MParentItemID))
				{
					model3.MParentItemID = "0";
					goto IL_0ad3;
				}
				if (!list12.Contains(model3.MParentItemID))
				{
					if (IsParentExpenseItemQuoted(ctx, model3, noParentList))
					{
						operationResult.Success = false;
						list12.Add(model3.MParentItemID);
						operationResult.VerificationInfor.Add(new BizVerificationInfor
						{
							Level = AlertEnum.Error,
							Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuoted", "费用项目:{0}已经被引用，不允许新增下级项目"), model3.MParentItemName),
							Id = num.ToString(),
							CheckItem = "RowIndex",
							RowIndex = num
						});
					}
					goto IL_0ad3;
				}
				continue;
				IL_0ad3:
				//bool flag = false;
				if ((!(model3.MParentItemID == "0")) ? ModelInfoManager.IsLangColumnValueExists<BDExpenseItemModel>(ctx, "MName", model3.MultiLanguage, model3.MItemID, "MParentItemID", model3.MParentItemID, false) : ModelInfoManager.IsLangColumnValueExists<BDExpenseItemModel>(ctx, "MName", model3.MultiLanguage, model3.MItemID, "MParentItemID", "0", false))
				{
					BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
					bizVerificationInfor.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuotedBBB", "同一级中存在相同的名称或者子级名称与父级相同");
					bizVerificationInfor.Id = num.ToString();
					bizVerificationInfor.RowIndex = Convert.ToInt32(num.ToString());
					bizVerificationInfor.CheckItem = "RowIndex";
					operationResult.VerificationInfor.Add(bizVerificationInfor);
					operationResult.Success = false;
				}
				num++;
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExpenseItemModel>(ctx, model3, model3.UpdateFieldList, true));
			}
			if (operationResult.Success)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				if (dynamicDbHelperMySQL.ExecuteSqlTran(list) <= 0)
				{
					operationResult.Success = false;
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "SaveFail", "保存失败！")
					});
				}
				else if (list2.Any())
				{
					operationResult.Tag = string.Join(",", from f in list2
					select f.MName);
				}
			}
			return operationResult;
		}

		public bool IsParentExpenseItemQuoted(MContext ctx, BDExpenseItemModel model, List<BDExpenseItemModel> childList)
		{
			bool result = false;
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "ExpenseItem", model.MParentItemID);
			if (!operationResult.Success && (childList == null || !childList.Exists((BDExpenseItemModel x) => x.MParentItemID == model.MParentItemID)))
			{
				result = true;
			}
			return result;
		}

		private BDExpenseItemModel GetNewExpenseItemModel(string expItemName)
		{
			BDExpenseItemModel bDExpenseItemModel = new BDExpenseItemModel();
			bDExpenseItemModel.MItemID = UUIDHelper.GetGuid();
			bDExpenseItemModel.MParentItemID = "0";
			bDExpenseItemModel.IsNew = true;
			bDExpenseItemModel.MIsActive = true;
			bDExpenseItemModel.MName = expItemName;
			bDExpenseItemModel.MultiLanguage = new List<MultiLanguageFieldList>
			{
				new MultiLanguageFieldList
				{
					MFieldName = HttpUtility.HtmlDecode("MName"),
					MMultiLanguageField = new List<MultiLanguageField>
					{
						new MultiLanguageField
						{
							MLocaleID = "0x0009",
							MValue = expItemName
						},
						new MultiLanguageField
						{
							MLocaleID = "0x7804",
							MValue = expItemName
						},
						new MultiLanguageField
						{
							MLocaleID = "0x7C04",
							MValue = expItemName
						}
					}
				}
			};
			return bDExpenseItemModel;
		}

		private static void ValidateDuplicateImportName(List<string> duplicateImportNameList, IEnumerable<BDExpenseItemModel> expItemList)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (BDExpenseItemModel expItem2 in expItemList)
			{
				MultiLanguageFieldList multiLanguageFieldList = expItem2.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				if (multiLanguageFieldList != null)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
					{
						if (!string.IsNullOrWhiteSpace(item.MValue))
						{
							if (dictionary.ContainsKey(item.MLocaleID))
							{
								dictionary[item.MLocaleID].Add(item.MValue);
							}
							else
							{
								dictionary.Add(item.MLocaleID, new List<string>
								{
									item.MValue
								});
							}
						}
					}
				}
			}
			foreach (string key in dictionary.Keys)
			{
				List<string> list = (from c in dictionary[key]
				group c by c into g
				where g.Count() > 1
				select g into f
				select f.Key).ToList();
				if (list.Any())
				{
					duplicateImportNameList.AddRange(list);
				}
			}
		}

		public BizVerificationInfor IsExistsItems(MContext ctx, List<BDExpenseItemModel> listExpense, BDExpenseItemModel model, string id)
		{
			BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
			foreach (BDExpenseItemModel item in listExpense)
			{
				if (!(item.MItemID == model.MItemID))
				{
					List<MultiLanguageFieldList> list = (from x in model.MultiLanguage
					where !string.IsNullOrWhiteSpace(x.MFieldName) && x.MFieldName == "MName"
					select x).ToList();
					foreach (MultiLanguageFieldList item2 in list)
					{
						List<MultiLanguageField> mMultiLanguageField = item2.MMultiLanguageField;
						foreach (MultiLanguageField item3 in mMultiLanguageField)
						{
							if (ctx.MLCID == item3.MLocaleID && item.MName == item3.MValue)
							{
								bizVerificationInfor.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItemQuotedBBB", "同一级中存在相同的名称或者子级名称与父级相同");
								bizVerificationInfor.Id = id;
								bizVerificationInfor.RowIndex = Convert.ToInt32(id);
								bizVerificationInfor.CheckItem = "RowIndex";
								return bizVerificationInfor;
							}
						}
					}
				}
			}
			return bizVerificationInfor;
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MParentItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ParentExpenseItem", "父级费用项目", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ParentItemComment", "下拉框选择父级费用项目；如此行为父级则无需填写；未创建父级费用项目则直接在此栏手动录入"), false),
				new IOTemplateConfigModel("MName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ExpenseItemName", "费用项目名称", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ManualInput", "手动录入"), true)
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MAccountCode", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "Account", "Account", null)
					}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DropDownSelect", "下拉框选择"), false)
				});
			}
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MDesc", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.BD, "ExpenseItemDesc", "费用项目描述", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ManualInput", "手动录入"), false)
			});
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			BDExpenseItemModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<BDExpenseItemModel>(ctx);
			List<string> fieldList = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			List<BASLangModel> orgLangList = _lang.GetOrgLangList(ctx);
			List<ImportDataSourceInfo> dicLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dicLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			ImportTemplateDataSource importTemplateDataSource = new ImportTemplateDataSource();
			importTemplateDataSource.FieldType = ImportTemplateColumnType.MultiLanguage;
			importTemplateDataSource.FieldList = fieldList;
			importTemplateDataSource.DataSourceList = dicLangList;
			list.Add(importTemplateDataSource);
			List<BDExpenseItemModel> parentExpenseItemList = expItem.GetParentExpenseItemList(ctx, true, false, false);
			List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
			foreach (BDExpenseItemModel item in parentExpenseItemList)
			{
				list2.Add(new ImportDataSourceInfo
				{
					Key = item.MItemID,
					Value = item.MName
				});
			}
			list.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.IsExpenseItem,
				FieldList = new List<string>
				{
					"MParentItemID"
				},
				DataSourceList = list2
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<BDAccountModel> bDAccountList = _accountBiz.GetBDAccountList(ctx, new BDAccountListFilterModel(), false, false);
				List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item2 in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item2.MCode))
					{
						list3.Add(new ImportDataSourceInfo
						{
							Key = item2.MCode,
							Value = item2.MFullName
						});
					}
				}
				list.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Account,
					FieldList = new List<string>
					{
						"MAccountCode"
					},
					DataSourceList = list3
				});
			}
			return list;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary);
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TravelExpense", "差旅费");
			dictionary2.Add("MParentItemID", new string[2]
			{
				" ",
				text
			});
			dictionary2.Add("MName", new string[2]
			{
				text,
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AiTicket", "机票")
			});
			dictionary2.Add("MAccountCode", new string[2]
			{
				" ",
				"6602.08.01"
			});
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			dictionary3.Add("MAccountCode", 7);
			return new ImportTemplateModel
			{
				TemplateType = "ExpenseItem",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2,
				ColumnWidthList = dictionary3
			};
		}

		public BDExpenseItemModel GetInsertExpenseIitemModel(MContext ctx, string expenseItemName, List<BASLangModel> languageList)
		{
			BDExpenseItemModel bDExpenseItemModel = new BDExpenseItemModel();
			bDExpenseItemModel.MItemID = UUIDHelper.GetGuid();
			bDExpenseItemModel.MParentItemID = "0";
			bDExpenseItemModel.IsNew = true;
			bDExpenseItemModel.MName = expenseItemName;
			if (languageList == null)
			{
				languageList = new BASLangBusiness().GetOrgLangList(ctx);
			}
			MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
			multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
			multiLanguageFieldList.MFieldName = "MName";
			foreach (BASLangModel language in languageList)
			{
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MLocaleID = language.LangID;
				multiLanguageField.MOrgID = ctx.MOrgID;
				multiLanguageField.MValue = expenseItemName;
				multiLanguageFieldList.MMultiLanguageField.Add(multiLanguageField);
			}
			bDExpenseItemModel.MultiLanguage = new List<MultiLanguageFieldList>();
			bDExpenseItemModel.MultiLanguage.Add(multiLanguageFieldList);
			return bDExpenseItemModel;
		}

		public bool IsExistsName<T>(List<T> sysList, T model, string field) where T : BaseModel
		{
			List<MultiLanguageFieldList> list = (from x in model.MultiLanguage
			where !string.IsNullOrWhiteSpace(x.MFieldName) && x.MFieldName == field
			select x).ToList();
			foreach (MultiLanguageFieldList item in list)
			{
				List<MultiLanguageField> mMultiLanguageField = item.MMultiLanguageField;
				foreach (MultiLanguageField item2 in mMultiLanguageField)
				{
					foreach (T sys in sysList)
					{
						List<MultiLanguageFieldList> list2 = (from x in sys.MultiLanguage
						where !string.IsNullOrWhiteSpace(x.MFieldName) && x.MFieldName == field
						select x).ToList();
						foreach (MultiLanguageFieldList item3 in list2)
						{
							if (item3.MMultiLanguageField.Exists((Predicate<MultiLanguageField>)((MultiLanguageField a) => a.MLocaleID == item2.MLocaleID && a.MValue == item2.MValue)))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}
