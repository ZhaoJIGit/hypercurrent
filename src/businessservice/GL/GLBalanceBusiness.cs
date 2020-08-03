using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataMode;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLBalanceBusiness : IGLBalanceBusiness, IDataContract<GLBalanceModel>
	{
		private readonly GLBalanceRepository dal = new GLBalanceRepository();

		private static Dictionary<int, string> TrackCheckTypeName = new Dictionary<int, string>();

		public GLBalanceModel GetBalanceByAccountID(MContext ctx, string itemID, int? year, int? period)
		{
			List<GLBalanceModel> balanceByAccountID = dal.GetBalanceByAccountID(ctx, itemID, year, period, true);
			return new GLBalanceModel
			{
				MOrgID = ctx.MOrgID,
				MCredit = balanceByAccountID.Sum((GLBalanceModel x) => x.MCredit),
				MDebit = balanceByAccountID.Sum((GLBalanceModel x) => x.MDebit),
				MDebitFor = balanceByAccountID.Sum((GLBalanceModel x) => x.MDebitFor),
				MCreditFor = balanceByAccountID.Sum((GLBalanceModel x) => x.MCreditFor),
				MBeginBalance = balanceByAccountID.Sum((GLBalanceModel x) => x.MBeginBalance),
				MEndBalanceFor = balanceByAccountID.Sum((GLBalanceModel x) => x.MEndBalanceFor),
				MYtdCredit = balanceByAccountID.Sum((GLBalanceModel x) => x.MYtdCredit),
				MYtdCreditFor = balanceByAccountID.Sum((GLBalanceModel x) => x.MYtdCreditFor),
				MYtdDebit = balanceByAccountID.Sum((GLBalanceModel x) => x.MYtdDebit),
				MYtdDebitFor = balanceByAccountID.Sum((GLBalanceModel x) => x.MYtdDebitFor)
			};
		}

		public List<GLBalanceModel> GetCheckGroupBalanceByAccountID(MContext ctx, string itemID, int? year, int? period)
		{
			List<GLBalanceModel> balanceByAccountID = dal.GetBalanceByAccountID(ctx, itemID, year, period, false);
			return GetGroupBalanceList(ctx, balanceByAccountID, false);
		}

		public List<GLBalanceModel> GetGroupBalanceList(MContext ctx, List<GLBalanceModel> balanceList, bool isFinalTransfer = false)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			if (balanceList.Count() > 1)
			{
				balanceList = (from f in balanceList
				where f.MCheckGroupValueID != "0"
				select f).ToList();
			}
			string mAccountID = balanceList.FirstOrDefault().MAccountID;
			if (!isFinalTransfer)
			{
				balanceList = (from f in balanceList
				where Math.Abs(f.MCredit) + Math.Abs(f.MCreditFor) + Math.Abs(f.MDebit) + Math.Abs(f.MDebitFor) != decimal.Zero
				select f).ToList();
			}
			if (balanceList.Count == 0)
			{
				list.Add(new GLBalanceModel
				{
					MAccountID = mAccountID
				});
				return list;
			}
			var enumerable = from f in balanceList
			group f by new
			{
				f.MAccountID,
				f.MCurrencyID,
				f.MCheckGroupValueID
			};
			foreach (var item in enumerable)
			{
				List<GLBalanceModel> source = item.ToList();
				list.Add(new GLBalanceModel
				{
					MAccountID = item.Key.MAccountID,
					MCurrencyID = item.Key.MCurrencyID,
					MCheckGroupValueID = item.Key.MCheckGroupValueID,
					MOrgID = ctx.MOrgID,
					MCredit = source.Sum((GLBalanceModel x) => x.MCredit),
					MDebit = source.Sum((GLBalanceModel x) => x.MDebit),
					MDebitFor = source.Sum((GLBalanceModel x) => x.MDebitFor),
					MCreditFor = source.Sum((GLBalanceModel x) => x.MCreditFor),
					MBeginBalance = source.Sum((GLBalanceModel x) => x.MBeginBalance),
					MBeginBalanceFor = source.Sum((GLBalanceModel x) => x.MBeginBalanceFor),
					MEndBalance = source.Sum((GLBalanceModel x) => x.MEndBalance),
					MEndBalanceFor = source.Sum((GLBalanceModel x) => x.MEndBalanceFor),
					MYtdCredit = source.Sum((GLBalanceModel x) => x.MYtdCredit),
					MYtdCreditFor = source.Sum((GLBalanceModel x) => x.MYtdCreditFor),
					MYtdDebit = source.Sum((GLBalanceModel x) => x.MYtdDebit),
					MYtdDebitFor = source.Sum((GLBalanceModel x) => x.MYtdDebitFor),
					MDC = source.FirstOrDefault().MDC
				});
			}
			return list;
		}

		public bool CheckAssetLiablityEqual(MContext ctx, int year, int period)
		{
			BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
			AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
			List<BDAccountListModel> accountListIncludeBalance = bDAccountBusiness.GetAccountListIncludeBalance(ctx, new SqlWhere(), true);
			List<string> itemListByType = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
			{
				accountTypeEnum.CurrentAsset,
				accountTypeEnum.NoCurrentAsset
			}, true, false);
			List<GLBalanceModel> balanceListByAccountIDs = GetBalanceListByAccountIDs(ctx, itemListByType, year, period, true);
			List<string> itemListByType2 = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
			{
				accountTypeEnum.CurrentLiability,
				accountTypeEnum.NoCurrentLiability
			}, true, false);
			List<GLBalanceModel> balanceListByAccountIDs2 = GetBalanceListByAccountIDs(ctx, itemListByType2, year, period, true);
			List<string> itemListByType3 = BDAccountRepository.GetItemListByType(accountListIncludeBalance, new List<string>
			{
				accountTypeEnum.OwnerEquity
			}, true, false);
			List<GLBalanceModel> balanceListByAccountIDs3 = GetBalanceListByAccountIDs(ctx, itemListByType3, year, period, true);
			return balanceListByAccountIDs.Sum((GLBalanceModel x) => x.MDebit - x.MCredit) == balanceListByAccountIDs2.Sum((GLBalanceModel x) => x.MCredit - x.MDebit) + balanceListByAccountIDs3.Sum((GLBalanceModel x) => x.MCredit - x.MDebit);
		}

		public List<GLBalanceModel> GetBalanceListByAccountIDs(MContext ctx, List<string> accountIDs, int? year, int? period, bool isSummary = true)
		{
			if (accountIDs == null || accountIDs.Count == 0)
			{
				return new List<GLBalanceModel>();
			}
			List<GLBalanceModel> list = dal.GetBalanceListByAccountIDs(ctx, accountIDs, year, period, isSummary);
			if (list == null || list.Count == 0)
			{
				list.Add(new GLBalanceModel
				{
					MAccountID = accountIDs[0]
				});
				return list;
			}
			if (!isSummary)
			{
				list = GetGroupBalanceList(ctx, list, false);
			}
			return list;
		}

		public List<GLBalanceModel> GetPeriodBalanceListByAccountIDs(MContext ctx, List<string> accountIDs, int startYearPeriod, int endYearPeriod)
		{
			return dal.GetPeriodBalanceListByAccountIDs(ctx, accountIDs, startYearPeriod, endYearPeriod);
		}

		public Dictionary<string, string> GetAccountingPeriod(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			DataSet accountingPeriod = dal.GetAccountingPeriod(ctx);
			if (accountingPeriod != null && accountingPeriod.Tables.Count > 0 && accountingPeriod.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = accountingPeriod.Tables[0];
				foreach (DataRow row in dataTable.Rows)
				{
					int num = Convert.ToInt32(row[0]);
					int num2 = Convert.ToInt32(row[1]);
					string key = Convert.ToString(num * 100 + num2);
					string value = Convert.ToString(num) + "-" + Convert.ToString(num2);
					dictionary.Add(key, value);
				}
			}
			else
			{
				DateTime mGLBeginDate = ctx.MGLBeginDate;
				string key2 = mGLBeginDate.ToString("yyyyMM");
				mGLBeginDate = ctx.MGLBeginDate;
				string value2 = mGLBeginDate.ToString("yyyy-MM");
				dictionary.Add(key2, value2);
			}
			return dictionary;
		}

		public List<GLBalanceModel> GetBalanceByPeriods(MContext ctx, List<DateTime> periods, List<string> accountIDS)
		{
			return dal.GetBalanceByPeriods(ctx, periods, accountIDS);
		}

		public List<GLBalanceModel> GetCheckGroupInitBalanceModelList(MContext ctx, BDAccountEditModel accountModel)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MAccountID", accountModel.MItemID);
			sqlWhere.NotEqual("MCheckGroupValueID", "0");
			return dal.GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, false);
		}

		public OperationResult IsCanUpdateAccountCheckGroup(MContext ctx, BDAccountEditModel accountModel, BDAccountEditModel oldAccountModel)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			GLCheckGroupModel mCheckGroupModel = accountModel.MCheckGroupModel;
			if (mCheckGroupModel == null)
			{
				return operationResult;
			}
			List<GLBalanceModel> checkGroupInitBalanceModelList = GetCheckGroupInitBalanceModelList(ctx, accountModel);
			if (checkGroupInitBalanceModelList == null || checkGroupInitBalanceModelList.Count() == 0)
			{
				return operationResult;
			}
			if (oldAccountModel != null)
			{
				GLCheckGroupModel mCheckGroupModel2 = oldAccountModel.MCheckGroupModel;
				string empty = string.Empty;
				bool flag = IsCanChangeCheckTypeStatus(ctx, mCheckGroupModel, mCheckGroupModel2, checkGroupInitBalanceModelList, out empty);
				if (!flag)
				{
					operationResult.Success = flag;
					operationResult.Message = empty;
					return operationResult;
				}
			}
			PropertyInfo[] properties = mCheckGroupModel.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				if (!(name == "PKFieldValue") && !(name == "MItemID"))
				{
					int num = 0;
					string s = Convert.ToString(propertyInfo.GetValue(mCheckGroupModel));
					if (int.TryParse(s, out num) && (num == CheckTypeStatusEnum.Required || num == CheckTypeStatusEnum.DisabledRequired))
					{
						foreach (GLBalanceModel item in checkGroupInitBalanceModelList)
						{
							GLCheckGroupValueModel mCheckGroupValueModel = item.MCheckGroupValueModel;
							if (mCheckGroupValueModel != null)
							{
								Type type = mCheckGroupValueModel.GetType();
								PropertyInfo property = type.GetProperty(name);
								if (!(property == (PropertyInfo)null))
								{
									string value = Convert.ToString(property.GetValue(mCheckGroupValueModel));
									if (string.IsNullOrWhiteSpace(value))
									{
										operationResult.Success = (!operationResult.Success && false);
										break;
									}
									continue;
								}
								break;
							}
						}
					}
				}
			}
			return operationResult;
		}

		public bool IsCanChangeCheckTypeStatus(MContext ctx, GLCheckGroupModel checkGroupModel, GLCheckGroupModel oldCheckGroupModel, List<GLBalanceModel> balanceList, out string tips)
		{
			bool flag = true;
			tips = string.Empty;
			if (oldCheckGroupModel == null)
			{
				return flag;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MContactID, oldCheckGroupModel.MContactID))
			{
				bool flag2 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MContactID));
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ContactCheckTypeRefInInitBalance", "联系人核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MEmployeeID, oldCheckGroupModel.MEmployeeID))
			{
				bool flag3 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MEmployeeID));
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "EmployeeCheckTypeRefInInitBalance", "员工核算维度已经在期初余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text2);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MMerItemID, oldCheckGroupModel.MMerItemID))
			{
				bool flag4 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MMerItemID));
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ItemCheckTypeRefInBalance", "商品项目核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text3);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MExpItemID, oldCheckGroupModel.MExpItemID))
			{
				bool flag5 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MExpItemID));
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "ExpenseCheckTypeRefInBalance", "费用项目核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text4);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MPaItemID, oldCheckGroupModel.MPaItemID))
			{
				bool flag6 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MPaItemID));
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "PACheckTypeRefInBalance", "工资项目核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text5);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem1, oldCheckGroupModel.MTrackItem1))
			{
				bool flag7 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem1));
				string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInBalance", "跟踪项核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text6);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem2, oldCheckGroupModel.MTrackItem2))
			{
				bool flag8 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem2));
				string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInBalance", "跟踪项核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text7);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem3, oldCheckGroupModel.MTrackItem3))
			{
				bool flag9 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem3));
				string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInBalance", "跟踪项核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text8);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem4, oldCheckGroupModel.MTrackItem4))
			{
				bool flag10 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem4));
				string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInBalance", "跟踪项核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text9);
				flag = (!flag && false);
			}
			if (BDAccountHelper.IsChangeCheckTypeStatus(checkGroupModel.MTrackItem5, oldCheckGroupModel.MTrackItem5))
			{
				bool flag11 = balanceList.Exists((GLBalanceModel x) => x.MCheckGroupValueModel != null && !string.IsNullOrWhiteSpace(x.MCheckGroupValueModel.MTrackItem5));
				string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "MTrackItemCheckTypeRefInBalance", "跟踪项核算维度已经在余额中使用，不可更改为禁用状态");
				stringBuilder.AppendLine(text10);
				flag = (!flag && false);
			}
			tips = stringBuilder.ToString();
			return flag;
		}

		public GLBalanceAccountTreeModel FindAccountBalanceChildrenItem(MContext ctx, AccountItemTreeModel parentModel, List<GLBalanceModel> balanceList, GLReportBaseFilterModel filter)
		{
			GLBalanceAccountTreeModel model = new GLBalanceAccountTreeModel();
			model.id = parentModel.id;
			model.text = $"{parentModel.MNumber} {parentModel.text}";
			model.MNumber = parentModel.MNumber;
			if (parentModel.children == null || parentModel.children.Count() == 0)
			{
				if (parentModel.CheckGroupNameList == null || parentModel.CheckGroupNameList.Count() == 0)
				{
					return model;
				}
				List<GLBalanceModel> list = (from x in balanceList
				where x.MAccountID == model.id
				select x).ToList();
				if (list != null && list.Count() > 0)
				{
					foreach (GLBalanceModel item in list)
					{
						GLBalanceAccountTreeModel gLBalanceAccountTreeModel = new GLBalanceAccountTreeModel();
						gLBalanceAccountTreeModel.id = UUIDHelper.GetGuid();
						gLBalanceAccountTreeModel.AccountID = model.id;
						string checkTypeValueName = GetCheckTypeValueName(ctx, item);
						if (!string.IsNullOrWhiteSpace(checkTypeValueName) && (model.children == null || !model.children.Exists((GLBalanceAccountTreeModel x) => x.CheckGroupValueId == item.MCheckGroupValueID && x.AccountID == item.MAccountID)))
						{
							gLBalanceAccountTreeModel.text = $"{model.text}_{checkTypeValueName}";
							gLBalanceAccountTreeModel.IsCheckTypeAccount = true;
							gLBalanceAccountTreeModel.CheckTypeValueList = GetCheckTypeValueList(item);
							gLBalanceAccountTreeModel.CheckGroupValueId = item.MCheckGroupValueID;
							model.children = ((model.children == null) ? new List<GLBalanceAccountTreeModel>() : model.children);
							model.children.Add(gLBalanceAccountTreeModel);
						}
					}
				}
				return model;
			}
			model.children = new List<GLBalanceAccountTreeModel>();
			if (parentModel.children != null && parentModel.children.Count() > 0)
			{
				foreach (AccountItemTreeModel child in parentModel.children)
				{
					List<string> accountIdListByTreeNode = BDAccountHelper.GetAccountIdListByTreeNode(child, null, filter.AccountLevel);
					if (filter.AccountIdList == null || filter.AccountIdList.Count() <= 0 || filter.AccountIdList.Intersect(accountIdListByTreeNode).Count() != 0)
					{
						if (filter.AccountLevel > 0 && child.AccountLevel > filter.AccountLevel)
						{
							return model;
						}
						model.children.Add(FindAccountBalanceChildrenItem(ctx, child, balanceList, filter));
					}
				}
			}
			return model;
		}

		public List<GLBalanceModel> GetBankAccountGLBalance(MContext ctx, int startYearPeriod, int endYearPeriod, List<string> bankIdList)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.GreaterOrEqual("MYearPeriod", startYearPeriod);
			sqlWhere.LessOrEqual("MYearPeriod", endYearPeriod);
			sqlWhere.In("MAccountID", bankIdList);
			return dal.GetGLBalancesList(ctx, sqlWhere);
		}

		public OperationResult ValidateBalanceList(MContext ctx, List<GLBalanceModel> balanceList)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = true;
			BDCheckValidteListModel bDCheckValidteListModel = new BDCheckValidteListModel();
			List<GLCheckGroupValueModel> list = (from x in balanceList
			where x.MCheckGroupValueModel != null
			select x.MCheckGroupValueModel).ToList();
			if (list == null)
			{
				return operationResult;
			}
			List<string> list2 = (from x in list
			select x.MContactID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list2 != null)
			{
				bDCheckValidteListModel.ContactIdList.AddRange(list2);
			}
			List<string> list3 = (from x in list
			select x.MEmployeeID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list3 != null)
			{
				bDCheckValidteListModel.EmployeeIdList.AddRange(list3);
			}
			List<string> list4 = (from x in list
			select x.MMerItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list4 != null)
			{
				bDCheckValidteListModel.MerchandiseIdList.AddRange(list4);
			}
			List<string> list5 = (from x in list
			select x.MExpItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list5 != null)
			{
				bDCheckValidteListModel.ExpenseIdList.AddRange(list5);
			}
			List<string> list6 = (from x in list
			select x.MPaItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list6 != null)
			{
				bDCheckValidteListModel.PaIdList.AddRange(list6);
			}
			List<string> list7 = (from x in list
			select x.MPaItemGroupID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list7 != null)
			{
				bDCheckValidteListModel.PaGroupIdList.AddRange(list7);
			}
			List<string> list8 = new List<string>();
			List<string> list9 = (from x in list
			select x.MTrackItem1 into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list9 != null)
			{
				list8.AddRange(list9);
			}
			List<string> list10 = (from x in list
			select x.MTrackItem2 into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list10 != null)
			{
				list8.AddRange(list10);
			}
			List<string> list11 = (from x in list
			select x.MTrackItem3 into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list11 != null)
			{
				list8.AddRange(list11);
			}
			List<string> list12 = (from x in list
			select x.MTrackItem4 into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list12 != null)
			{
				list8.AddRange(list12);
			}
			List<string> list13 = (from x in list
			select x.MTrackItem5 into x
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list13 != null)
			{
				list8.AddRange(list13);
			}
			bDCheckValidteListModel.TrackEntryIdList.AddRange(list8);
			GLUtility gLUtility = new GLUtility();
			List<ValidateQueryModel> validateQueryModel = gLUtility.GetValidateQueryModel(ctx, bDCheckValidteListModel);
			List<MActionResultCodeEnum> list14 = gLUtility.QueryValidateSql(ctx, true, validateQueryModel.ToArray());
			return operationResult;
		}

		public void SetBalanceCheckTypeGroupValueId(MContext ctx, List<GLBalanceModel> balanceList)
		{
			if (balanceList == null)
			{
				throw new NullReferenceException("balanceList is null");
			}
			foreach (GLBalanceModel balance in balanceList)
			{
				string mCheckGroupValueID = balance.MCheckGroupValueID;
				GLCheckGroupValueModel mCheckGroupValueModel = balance.MCheckGroupValueModel;
				if ((string.IsNullOrWhiteSpace(mCheckGroupValueID) && mCheckGroupValueModel == null) || mCheckGroupValueID == "0")
				{
					balance.MCheckGroupValueID = "0";
				}
				else
				{
					GLCheckGroupValueModel gLCheckGroupValueModel = GLCheckGroupValueBusiness.FindExistCheckGroupValueModel(ctx, mCheckGroupValueModel);
					balance.MCheckGroupValueID = gLCheckGroupValueModel.MItemID;
				}
			}
		}

		public List<GLBalanceModel> CorrectBalanceList(MContext ctx, List<GLBalanceModel> balanceList)
		{
			List<GLBalanceModel> list = new List<GLBalanceModel>();
			List<BDAccountModel> baseBDAccountList = new BDAccountRepository().GetBaseBDAccountList(ctx, null, false, null);
			List<BDAccountModel> list2 = (from x in baseBDAccountList
			where x.MParentID == "0"
			select x).ToList();
			foreach (BDAccountModel item in list2)
			{
				List<BDAccountModel> list3 = (from x in baseBDAccountList
				where x.MParentID == item.MItemID
				select x).ToList();
				if (list3 != null && list3.Count() != 0)
				{
					List<string> childAccountIdList = (from x in list3
					select x.MItemID).ToList();
					List<GLBalanceModel> source = (from x in balanceList
					where childAccountIdList.Contains(x.MAccountID)
					select x).ToList();
					List<GLBalanceModel> source2 = (from x in source
					where x.MCheckGroupValueID == "0"
					select x).ToList();
					IEnumerable<IGrouping<int, GLBalanceModel>> enumerable = from x in source2
					group x by x.MYearPeriod;
					foreach (IGrouping<int, GLBalanceModel> item2 in enumerable)
					{
						int yearPeriod = item2.Key;
						List<GLBalanceModel> list4 = item2.ToList();
						if (list4 != null && list4.Count() != 0)
						{
							IEnumerable<IGrouping<string, GLBalanceModel>> enumerable2 = from x in list4
							group x by x.MCurrencyID;
							foreach (IGrouping<string, GLBalanceModel> item3 in enumerable2)
							{
								string currcnyId = item3.Key;
								List<GLBalanceModel> list5 = item3.ToList();
								if (list5.Count() != 0)
								{
									IEnumerable<IGrouping<string, GLBalanceModel>> enumerable3 = from x in list5
									group x by x.MCheckGroupValueID;
									foreach (IGrouping<string, GLBalanceModel> item4 in enumerable3)
									{
										string checkGroupValueId = item4.Key;
										List<GLBalanceModel> list6 = item4.ToList();
										GLBalanceModel gLBalanceModel = (from x in balanceList
										where x.MAccountID == item.MItemID && x.MCheckGroupValueID == "0" && x.MCurrencyID == currcnyId && x.MYearPeriod == yearPeriod && x.MCheckGroupValueID == checkGroupValueId
										select x).FirstOrDefault();
										bool flag = list5 != null && list5.Count() > 0;
										if (flag || gLBalanceModel != null)
										{
											if (flag && gLBalanceModel == null)
											{
												GLBalanceModel gLBalanceModel2 = list5.First();
												gLBalanceModel = new GLBalanceModel();
												gLBalanceModel.MYear = gLBalanceModel2.MYear;
												gLBalanceModel.MPeriod = gLBalanceModel2.MPeriod;
												gLBalanceModel.MYearPeriod = gLBalanceModel2.MYearPeriod;
												item.MCurrencyID = gLBalanceModel2.MCurrencyID;
												gLBalanceModel.MAccountID = item.MItemID;
												gLBalanceModel.MCheckGroupValueID = "0";
												gLBalanceModel.MBeginBalance = list5.Sum((GLBalanceModel x) => x.MBeginBalance);
												gLBalanceModel.MBeginBalanceFor = list5.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
												gLBalanceModel.MDebit = list5.Sum((GLBalanceModel x) => x.MDebit);
												gLBalanceModel.MDebitFor = list5.Sum((GLBalanceModel x) => x.MDebitFor);
												gLBalanceModel.MCredit = list5.Sum((GLBalanceModel x) => x.MCredit);
												gLBalanceModel.MCreditFor = list5.Sum((GLBalanceModel x) => x.MCreditFor);
												gLBalanceModel.MYtdCredit = list5.Sum((GLBalanceModel x) => x.MYtdCredit);
												gLBalanceModel.MYtdCreditFor = list5.Sum((GLBalanceModel x) => x.MYtdCreditFor);
												gLBalanceModel.MYtdDebit = list5.Sum((GLBalanceModel x) => x.MYtdDebit);
												gLBalanceModel.MYtdDebitFor = list5.Sum((GLBalanceModel x) => x.MYtdDebitFor);
												gLBalanceModel.MEndBalance = list5.Sum((GLBalanceModel x) => x.MEndBalance);
												gLBalanceModel.MEndBalanceFor = list5.Sum((GLBalanceModel x) => x.MEndBalanceFor);
												list.Add(gLBalanceModel);
											}
											else if (flag && gLBalanceModel != null)
											{
												decimal num = list5.Sum((GLBalanceModel x) => x.MBeginBalance);
												gLBalanceModel.MBeginBalance = ((gLBalanceModel.MBeginBalance != num) ? num : gLBalanceModel.MBeginBalance);
												decimal num2 = list5.Sum((GLBalanceModel x) => x.MBeginBalanceFor);
												gLBalanceModel.MBeginBalanceFor = ((gLBalanceModel.MBeginBalanceFor != num2) ? num2 : gLBalanceModel.MBeginBalanceFor);
												decimal num3 = list5.Sum((GLBalanceModel x) => x.MDebit);
												gLBalanceModel.MDebit = ((gLBalanceModel.MDebit != num3) ? num3 : gLBalanceModel.MDebit);
												decimal num4 = list5.Sum((GLBalanceModel x) => x.MDebitFor);
												gLBalanceModel.MDebitFor = ((gLBalanceModel.MDebitFor != num4) ? num4 : gLBalanceModel.MDebitFor);
												decimal num5 = list5.Sum((GLBalanceModel x) => x.MCredit);
												gLBalanceModel.MCredit = ((gLBalanceModel.MCredit != num5) ? num5 : gLBalanceModel.MCredit);
												decimal num6 = list5.Sum((GLBalanceModel x) => x.MCreditFor);
												gLBalanceModel.MCreditFor = ((gLBalanceModel.MCreditFor != num6) ? num6 : gLBalanceModel.MCreditFor);
												decimal num7 = list5.Sum((GLBalanceModel x) => x.MYtdDebit);
												gLBalanceModel.MYtdDebit = ((gLBalanceModel.MYtdDebit != num7) ? num7 : gLBalanceModel.MYtdDebit);
												decimal num8 = list5.Sum((GLBalanceModel x) => x.MYtdDebitFor);
												gLBalanceModel.MYtdDebitFor = ((gLBalanceModel.MYtdDebitFor != num8) ? num8 : gLBalanceModel.MYtdDebitFor);
												decimal num9 = list5.Sum((GLBalanceModel x) => x.MYtdCredit);
												gLBalanceModel.MYtdCredit = ((gLBalanceModel.MYtdCredit != num9) ? num9 : gLBalanceModel.MYtdCredit);
												decimal num10 = list5.Sum((GLBalanceModel x) => x.MYtdCreditFor);
												gLBalanceModel.MYtdCreditFor = ((gLBalanceModel.MYtdCreditFor != num10) ? num10 : gLBalanceModel.MYtdCreditFor);
												decimal num11 = list5.Sum((GLBalanceModel x) => x.MEndBalance);
												gLBalanceModel.MEndBalance = ((gLBalanceModel.MEndBalance != num11) ? num11 : gLBalanceModel.MEndBalance);
												decimal num12 = list5.Sum((GLBalanceModel x) => x.MEndBalanceFor);
												gLBalanceModel.MEndBalanceFor = ((gLBalanceModel.MEndBalanceFor != num12) ? num12 : gLBalanceModel.MEndBalanceFor);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			list.AddRange(balanceList);
			return list;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLBalanceModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLBalanceModel> modelData, string fields = null)
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

		public GLBalanceModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLBalanceModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLBalanceModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLBalanceModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public static string GetCheckTypeValueName(MContext ctx, GLBalanceModel balanceModel)
		{
			List<string> list = new List<string>();
			GLCheckGroupValueModel mCheckGroupValueModel = balanceModel.MCheckGroupValueModel;
			if (mCheckGroupValueModel == null || balanceModel.MCheckGroupValueID == "0")
			{
				return "";
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactName))
			{
				list.Add(mCheckGroupValueModel.MContactName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeName))
			{
				list.Add(mCheckGroupValueModel.MEmployeeName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemName))
			{
				list.Add(mCheckGroupValueModel.MMerItemName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemName))
			{
				list.Add(mCheckGroupValueModel.MExpItemName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemGroupName))
			{
				list.Add(mCheckGroupValueModel.MPaItemGroupName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemName))
			{
				list.Add(mCheckGroupValueModel.MPaItemName);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1Name))
			{
				list.Add(mCheckGroupValueModel.MTrackItem1Name);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2Name))
			{
				list.Add(mCheckGroupValueModel.MTrackItem2Name);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3Name))
			{
				list.Add(mCheckGroupValueModel.MTrackItem3Name);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4Name))
			{
				list.Add(mCheckGroupValueModel.MTrackItem4Name);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5Name))
			{
				list.Add(mCheckGroupValueModel.MTrackItem5Name);
			}
			//string text = "";
			return (list.Count() <= 0) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "UnKownCheckTypeValue", "未指定") : string.Join("_", list);
		}

		private static string GetCheckTypeValueString(MContext ctx, CheckTypeEnum checkType, string checkTypeValueName)
		{
			GLUtility gLUtility = new GLUtility();
			string checkTypeName = gLUtility.GetCheckTypeName(ctx, (int)checkType);
			if (!string.IsNullOrWhiteSpace(checkTypeValueName))
			{
				return checkTypeName + ":" + checkTypeValueName;
			}
			return checkTypeName + ":" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Acct, "UnKownCheckTypeValue", "未指定");
		}

		public static List<NameValueModel> GetCheckTypeValueList(GLBalanceModel balanceModel)
		{
			List<NameValueModel> list = new List<NameValueModel>();
			GLCheckGroupValueModel mCheckGroupValueModel = balanceModel.MCheckGroupValueModel;
			if (mCheckGroupValueModel == null)
			{
				return list;
			}
			int num;
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MContactName))
			{
				num = 0;
				string mName = num.ToString();
				string mContactID = mCheckGroupValueModel.MContactID;
				NameValueModel nameValueModel = new NameValueModel();
				nameValueModel.MName = mName;
				nameValueModel.MValue = mContactID;
				list.Add(nameValueModel);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MEmployeeName))
			{
				num = 1;
				string mName2 = num.ToString();
				string mEmployeeID = mCheckGroupValueModel.MEmployeeID;
				NameValueModel nameValueModel2 = new NameValueModel();
				nameValueModel2.MName = mName2;
				nameValueModel2.MValue = mEmployeeID;
				list.Add(nameValueModel2);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MMerItemName))
			{
				num = 2;
				string mName3 = num.ToString();
				string mMerItemID = mCheckGroupValueModel.MMerItemID;
				NameValueModel nameValueModel3 = new NameValueModel();
				nameValueModel3.MName = mName3;
				nameValueModel3.MValue = mMerItemID;
				list.Add(nameValueModel3);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MExpItemName))
			{
				num = 3;
				string mName4 = num.ToString();
				string mExpItemID = mCheckGroupValueModel.MExpItemID;
				NameValueModel nameValueModel4 = new NameValueModel();
				nameValueModel4.MName = mName4;
				nameValueModel4.MValue = mExpItemID;
				list.Add(nameValueModel4);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemGroupName) || !string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemName))
			{
				num = 4;
				string mName5 = num.ToString();
				string mValue = string.IsNullOrWhiteSpace(mCheckGroupValueModel.MPaItemID) ? mCheckGroupValueModel.MPaItemGroupID : mCheckGroupValueModel.MPaItemID;
				NameValueModel nameValueModel5 = new NameValueModel();
				nameValueModel5.MName = mName5;
				nameValueModel5.MValue = mValue;
				list.Add(nameValueModel5);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem1Name))
			{
				num = 5;
				string mName6 = num.ToString();
				string mTrackItem = mCheckGroupValueModel.MTrackItem1;
				NameValueModel nameValueModel6 = new NameValueModel();
				nameValueModel6.MName = mName6;
				nameValueModel6.MValue = mTrackItem;
				list.Add(nameValueModel6);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem2Name))
			{
				num = 6;
				string mName7 = num.ToString();
				string mTrackItem2 = mCheckGroupValueModel.MTrackItem2;
				NameValueModel nameValueModel7 = new NameValueModel();
				nameValueModel7.MName = mName7;
				nameValueModel7.MValue = mTrackItem2;
				list.Add(nameValueModel7);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem3Name))
			{
				num = 7;
				string mName8 = num.ToString();
				string mTrackItem3 = mCheckGroupValueModel.MTrackItem3;
				NameValueModel nameValueModel8 = new NameValueModel();
				nameValueModel8.MName = mName8;
				nameValueModel8.MValue = mTrackItem3;
				list.Add(nameValueModel8);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem4Name))
			{
				num = 8;
				string mName9 = num.ToString();
				string mTrackItem4 = mCheckGroupValueModel.MTrackItem4;
				NameValueModel nameValueModel9 = new NameValueModel();
				nameValueModel9.MName = mName9;
				nameValueModel9.MValue = mTrackItem4;
				list.Add(nameValueModel9);
			}
			if (!string.IsNullOrWhiteSpace(mCheckGroupValueModel.MTrackItem5Name))
			{
				num = 9;
				string mName10 = num.ToString();
				string mTrackItem5 = mCheckGroupValueModel.MTrackItem5;
				NameValueModel nameValueModel10 = new NameValueModel();
				nameValueModel10.MName = mName10;
				nameValueModel10.MValue = mTrackItem5;
				list.Add(nameValueModel10);
			}
			return list;
		}
	}
}
