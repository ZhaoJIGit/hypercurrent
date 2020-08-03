using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.BD.AccountItem;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDAccountHelper
	{
		public static List<BDAccountModel> GetChildrenAccountByRecursion(BDAccountModel parentAccount, List<BDAccountModel> accoutList, bool isSelf)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (isSelf)
			{
				list.Add(parentAccount);
			}
			List<BDAccountModel> list2 = (from x in accoutList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				return list;
			}
			foreach (BDAccountModel item in list2)
			{
				list.AddRange(GetChildrenAccountByRecursion(item, accoutList, true));
			}
			return list;
		}

		public static List<string> GetLeafAccountIDList(string parentId, List<BDAccountModel> accoutList)
		{
			List<string> list = new List<string>();
			List<BDAccountModel> list2 = (from x in accoutList
			where x.MParentID == parentId
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				list.Add(parentId);
				return list;
			}
			foreach (BDAccountModel item in list2)
			{
				list.AddRange(GetLeafAccountIDList(item.MItemID, accoutList));
			}
			return list;
		}

		public static List<string> GetLeafAccountIDList(List<string> parentIdList, List<BDAccountModel> accountList)
		{
			List<string> list = new List<string>();
			foreach (string parentId in parentIdList)
			{
				List<string> leafAccountIDList = GetLeafAccountIDList(parentId, accountList);
				if (leafAccountIDList != null && leafAccountIDList.Count != 0)
				{
					list.AddRange(leafAccountIDList);
				}
			}
			return list;
		}

		public static List<BDAccountListModel> GetChildrenAccountRecursion(BDAccountListModel parentAccount, List<BDAccountListModel> accoutList)
		{
			List<BDAccountListModel> list = new List<BDAccountListModel>();
			list.Add(parentAccount);
			List<BDAccountListModel> list2 = (from x in accoutList
			where x.MParentID == parentAccount.MItemID
			select x).ToList();
			if (list2 == null || list2.Count() == 0)
			{
				return list;
			}
			foreach (BDAccountListModel item in list2)
			{
				list.AddRange(GetChildrenAccountRecursion(item, accoutList));
			}
			return list;
		}

		public static List<BDAccountModel> GetParentAccountByRecursion(BDAccountModel childAccount, List<BDAccountModel> accoutList)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			if (childAccount.MParentID == "0")
			{
				return list;
			}
			BDAccountModel bDAccountModel = (from x in accoutList
			where x.MItemID == childAccount.MParentID
			select x).FirstOrDefault();
			if (bDAccountModel == null)
			{
				throw new Exception("can find parent account");
			}
			list.Add(bDAccountModel);
			if (bDAccountModel.MParentID != "0")
			{
				list.AddRange(GetParentAccountByRecursion(bDAccountModel, accoutList));
			}
			return list;
		}

		public static bool IsChangeCheckTypeStatus(int status, int oldStatus)
		{
			return status != oldStatus && (status == CheckTypeStatusEnum.DisabledRequired || status == CheckTypeStatusEnum.DisabledOptional) && (oldStatus == CheckTypeStatusEnum.Optional || oldStatus == CheckTypeStatusEnum.Required);
		}

		public static List<string> GetCurrentAccoutMCode()
		{
			List<string> list = new List<string>();
			list.Add("1122");
			list.Add("1123");
			list.Add("2202");
			list.Add("2203");
			list.Add("1221");
			list.Add("2241");
			return list;
		}

		public static List<string> GetBankAccountMCode()
		{
			List<string> list = new List<string>();
			list.Add("1001");
			list.Add("1002");
			return list;
		}

		public static bool IsCurrentAccount(string accountCode)
		{
			bool result = false;
			if (string.IsNullOrWhiteSpace(accountCode))
			{
				return false;
			}
			List<string> currentAccoutMCode = GetCurrentAccoutMCode();
			foreach (string item in currentAccoutMCode)
			{
				if (accountCode.IndexOf(item) == 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static bool IsBankAccount(string accountCode)
		{
			bool result = false;
			if (string.IsNullOrWhiteSpace(accountCode))
			{
				return false;
			}
			List<string> bankAccountMCode = GetBankAccountMCode();
			foreach (string item in bankAccountMCode)
			{
				if (accountCode.IndexOf(item) == 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static List<string> GetAccountIdListByFilter(MContext ctx, string startAccountId, string endAccountId, int accountLevel = 0, List<AccountItemTreeModel> accountTreeList = null)
		{
			List<string> result = new List<string>();
			if (string.IsNullOrWhiteSpace(startAccountId) && string.IsNullOrWhiteSpace(endAccountId) && accountLevel == 0)
			{
				return result;
			}
			if (accountTreeList == null)
			{
				BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
				BDAccountListFilterModel bDAccountListFilterModel = new BDAccountListFilterModel();
				bDAccountListFilterModel.IsAll = true;
				accountTreeList = bDAccountBusiness.GetAccountItemTreeList(ctx, bDAccountListFilterModel);
			}
			if (accountTreeList == null || accountTreeList.Count() == 0)
			{
				return result;
			}
			return GetAccountIdListByAccountSpan(startAccountId, endAccountId, accountTreeList, accountLevel);
		}

		private static AccountItemTreeModel GetTreeNodeById(string accountId, AccountItemTreeModel treeNode)
		{
			if (treeNode.id == accountId)
			{
				return treeNode;
			}
			if (treeNode.children == null || treeNode.children.Count() == 0)
			{
				return null;
			}
			AccountItemTreeModel accountItemTreeModel = null;
			foreach (AccountItemTreeModel child in treeNode.children)
			{
				accountItemTreeModel = GetTreeNodeById(accountId, child);
				if (accountItemTreeModel != null)
				{
					break;
				}
			}
			return accountItemTreeModel;
		}

		public static AccountItemTreeModel GetLastTreeNode(AccountItemTreeModel parentNode, List<AccountItemTreeModel> tree)
		{
			if (parentNode.children == null || parentNode.children.Count() == 0)
			{
				return parentNode;
			}
			AccountItemTreeModel parentNode2 = parentNode.children.Last();
			return GetLastTreeNode(parentNode2, tree);
		}

		public static List<string> GetAccountIdListByAccountSpan(string startId, string endId, List<AccountItemTreeModel> treeList, int accountLevel)
		{
			List<string> list = new List<string>();
			int num = -1;
			int num2 = -1;
			if (!string.IsNullOrWhiteSpace(startId) || !string.IsNullOrWhiteSpace(endId))
			{
				int num3 = treeList.Count();
				for (int i = 0; i < num3; i++)
				{
					AccountItemTreeModel node = treeList[i];
					List<string> accountIdListByTreeNode = GetAccountIdListByTreeNode(node, null, 0);
					if (!string.IsNullOrWhiteSpace(startId) && string.IsNullOrWhiteSpace(endId))
					{
						if (accountIdListByTreeNode.Contains(startId))
						{
							num = i;
							break;
						}
					}
					else if (string.IsNullOrWhiteSpace(startId) && !string.IsNullOrWhiteSpace(endId) && accountIdListByTreeNode.Contains(endId))
					{
						num2 = i;
						break;
					}
					if (!string.IsNullOrWhiteSpace(startId) && !string.IsNullOrWhiteSpace(endId))
					{
						if (accountIdListByTreeNode.Contains(startId))
						{
							num = i;
						}
						if (accountIdListByTreeNode.Contains(endId))
						{
							num2 = i;
						}
						if (num >= 0 && num2 >= 0)
						{
							break;
						}
					}
				}
			}
			num = ((num != -1) ? num : 0);
			num2 = ((num2 == -1) ? (treeList.Count() - 1) : num2);
			if (num > num2)
			{
				int num4 = num2;
				num2 = num;
				num = num4;
			}
			List<AccountItemTreeModel> range = treeList.GetRange(num, num2 - num + 1);
			accountLevel = ((accountLevel == 0) ? 10 : accountLevel);
			for (int j = 0; j < range.Count(); j++)
			{
				AccountItemTreeModel node2 = range[j];
				if ((j == 0 && !string.IsNullOrWhiteSpace(startId)) || (j == range.Count() - 1 && !string.IsNullOrWhiteSpace(endId)))
				{
					if (j == 0 && !string.IsNullOrWhiteSpace(startId))
					{
						List<string> accountIdListByTreeNode2 = GetAccountIdListByTreeNode(node2, startId, null, accountLevel);
						int num5 = accountIdListByTreeNode2.FindIndex((string x) => x == startId);
						accountIdListByTreeNode2 = ((num5 < 0) ? null : accountIdListByTreeNode2.GetRange(num5, accountIdListByTreeNode2.Count()));
						if (accountIdListByTreeNode2 != null)
						{
							list.AddRange(accountIdListByTreeNode2);
						}
					}
					if (j == range.Count() - 1 && !string.IsNullOrWhiteSpace(endId))
					{
						List<string> accountIdListByTreeNode3 = GetAccountIdListByTreeNode(node2, null, endId, accountLevel);
						int num6 = accountIdListByTreeNode3.FindIndex((string x) => x == endId);
						accountIdListByTreeNode3 = ((num6 < 0) ? null : accountIdListByTreeNode3.GetRange(0, accountIdListByTreeNode3.Count()));
						if (accountIdListByTreeNode3 != null)
						{
							if (num == num2)
							{
								list.Clear();
								int num7 = accountIdListByTreeNode3.FindIndex((string x) => x == startId);
								accountIdListByTreeNode3 = accountIdListByTreeNode3.GetRange(num7, accountIdListByTreeNode3.Count() - num7);
							}
							list.AddRange(accountIdListByTreeNode3);
						}
					}
				}
				else
				{
					List<string> accountIdListByTreeNode4 = GetAccountIdListByTreeNode(node2, null, null, accountLevel);
					if (accountIdListByTreeNode4 != null)
					{
						list.AddRange(accountIdListByTreeNode4);
					}
				}
			}
			return list;
		}

		public static List<AccountItemTreeModel> FilterAccountTreeList(MContext ctx, BDAccountListFilterModel filter, List<AccountItemTreeModel> accountTreeList = null, bool ignoreEndIndex = false)
		{
			if (accountTreeList == null || accountTreeList.Count() == 0)
			{
				BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
				BDAccountListFilterModel bDAccountListFilterModel = new BDAccountListFilterModel();
				bDAccountListFilterModel.IsAll = true;
				accountTreeList = bDAccountBusiness.GetAccountItemTreeList(ctx, bDAccountListFilterModel);
				if (accountTreeList == null)
				{
					return accountTreeList;
				}
			}
			List<AccountItemTreeModel> treeList = GetAllAccountItemTree(accountTreeList);
			if (filter.AccountStartIndex > 0 || filter.AccountEndIndex > 0)
			{
				int accountStartIndex = filter.AccountStartIndex;
				if (filter.AccountStartIndex > filter.AccountEndIndex && filter.AccountStartIndex != 0 && filter.AccountEndIndex != 0)
				{
					filter.AccountStartIndex = filter.AccountEndIndex;
					filter.AccountEndIndex = accountStartIndex;
				}
				if (filter.AccountStartIndex > 0 && treeList != null)
				{
					treeList = (from x in treeList
					where x.Index >= filter.AccountStartIndex
					select x).ToList();
				}
				if (filter.AccountEndIndex > 0 && treeList != null)
				{
					treeList = (from x in treeList
					where x.Index <= filter.AccountEndIndex
					select x).ToList();
				}
			}
			if (filter.AccountLevel > 0 && treeList != null)
			{
				treeList = (from x in treeList
				where x.AccountLevel <= filter.AccountLevel
				select x).ToList();
				if (treeList != null && treeList.Count() > 0 && filter.AccountLevel > 1)
				{
					List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
					foreach (AccountItemTreeModel item in treeList)
					{
						List<AccountItemTreeModel> childrenTreeNode = GetChildrenTreeNode(item, filter.AccountLevel);
						if (childrenTreeNode != null)
						{
							list.AddRange(childrenTreeNode);
						}
					}
					if (list.Count() > 0)
					{
						list.ForEach(delegate(AccountItemTreeModel x)
						{
							if (!treeList.Exists((AccountItemTreeModel y) => y.id == x.id))
							{
								treeList.Add(x);
							}
						});
					}
				}
			}
			List<AccountItemTreeModel> tempTreeList = new List<AccountItemTreeModel>();
			if (filter.IsLeafAccount)
			{
				treeList.ForEach(delegate(AccountItemTreeModel x)
				{
					List<AccountItemTreeModel> leafAccountTreeList = GetLeafAccountTreeList(x, filter, ignoreEndIndex);
					if (leafAccountTreeList != null)
					{
						tempTreeList.AddRange(leafAccountTreeList);
					}
				});
			}
			else
			{
				tempTreeList = treeList;
			}
			return tempTreeList;
		}

		public static List<AccountItemTreeModel> FilterAccountTreeList(MContext ctx, BDAccountListFilterModel filter, bool isRemoveDuplicate, List<AccountItemTreeModel> accountTreeList = null)
		{
			List<AccountItemTreeModel> treeList = FilterAccountTreeList(ctx, filter, accountTreeList, false);
			if (!isRemoveDuplicate)
			{
				return treeList;
			}
			List<AccountItemTreeModel> removingItemList = new List<AccountItemTreeModel>();
			treeList.ForEach(delegate(AccountItemTreeModel x)
			{
				if (x.AccountLevel != 1)
				{
					foreach (AccountItemTreeModel item in treeList)
					{
						if (!(item.id == x.id))
						{
							List<AccountItemTreeModel> childrenTreeNode = GetChildrenTreeNode(item, x.AccountLevel);
							if (childrenTreeNode?.Exists((AccountItemTreeModel y) => y.id == x.id) ?? false)
							{
								removingItemList.Add(x);
								break;
							}
						}
					}
				}
			});
			if (removingItemList.Count() > 0)
			{
				removingItemList.ForEach(delegate(AccountItemTreeModel x)
				{
					treeList.Remove(x);
				});
			}
			treeList.ForEach(delegate(AccountItemTreeModel x)
			{
				ReomveChildrenNode(x, filter.AccountLevel);
			});
			treeList = treeList.Distinct().ToList();
			return treeList;
		}

		private static void ReomveChildrenNode(AccountItemTreeModel tree, int accountLevel)
		{
			if (accountLevel > 0)
			{
				if (tree.AccountLevel == accountLevel)
				{
					tree.children = new List<AccountItemTreeModel>();
				}
				else
				{
					List<AccountItemTreeModel> children = tree.children;
					if (children != null && children.Count() != 0)
					{
						foreach (AccountItemTreeModel item in children)
						{
							ReomveChildrenNode(item, accountLevel);
						}
					}
				}
			}
		}

		public static List<AccountItemTreeModel> GetAllAccountItemTree(List<AccountItemTreeModel> accountTreeList)
		{
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			foreach (AccountItemTreeModel accountTree in accountTreeList)
			{
				List<AccountItemTreeModel> childrenTreeNode = GetChildrenTreeNode(accountTree, 0);
				if (childrenTreeNode != null)
				{
					list.AddRange(childrenTreeNode);
				}
			}
			return list;
		}

		public static List<string> GetAccountIdListByTreeNode(AccountItemTreeModel node, string endAccountId, int accountLevel)
		{
			List<string> list = new List<string>();
			list.Add(node.id);
			int num;
			if ((string.IsNullOrWhiteSpace(endAccountId) || !(node.id == endAccountId)) && node.children != null && node.children.Count() != 0)
			{
				num = ((node.AccountLevel == accountLevel) ? 1 : 0);
				goto IL_004b;
			}
			num = 1;
			goto IL_004b;
			IL_004b:
			if (num != 0)
			{
				return list;
			}
			foreach (AccountItemTreeModel child in node.children)
			{
				list.AddRange(GetAccountIdListByTreeNode(child, endAccountId, accountLevel));
				if (!string.IsNullOrWhiteSpace(endAccountId) && list.Contains(endAccountId))
				{
					break;
				}
			}
			return list;
		}

		public static List<string> GetAccountIdListByTreeNode(AccountItemTreeModel node, string startAccountId, string endAccountId, int accountLevel)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrWhiteSpace(startAccountId))
			{
				if (node.id == startAccountId)
				{
					if (node.AccountLevel < accountLevel)
					{
						list.AddRange(GetAccountIdListByTreeNode(node, null, accountLevel));
					}
					else
					{
						list.Add(node.id);
					}
					return list;
				}
				if (node.children != null && node.children.Count() > 0)
				{
					List<AccountItemTreeModel> children = node.children;
					foreach (AccountItemTreeModel item in children)
					{
						list.AddRange(GetAccountIdListByTreeNode(item, startAccountId, endAccountId, accountLevel));
					}
					return list;
				}
			}
			else if (!string.IsNullOrWhiteSpace(endAccountId))
			{
				if (node.id == endAccountId)
				{
					if (node.AccountLevel < accountLevel)
					{
						list.AddRange(GetAccountIdListByTreeNode(node, null, accountLevel));
					}
					else
					{
						list.Add(node.id);
					}
					return list;
				}
				list.Add(node.id);
				if (node.children != null && node.children.Count() > 0)
				{
					List<AccountItemTreeModel> children2 = node.children;
					foreach (AccountItemTreeModel item2 in children2)
					{
						list.AddRange(GetAccountIdListByTreeNode(item2, startAccountId, endAccountId, accountLevel));
						if (list.Contains(endAccountId))
						{
							break;
						}
					}
					return list;
				}
			}
			else
			{
				list.AddRange(GetAccountIdListByTreeNode(node, null, accountLevel));
			}
			return list;
		}

		public static List<AccountItemTreeModel> GetChildrenTreeNode(AccountItemTreeModel tree, int accountLevel)
		{
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			if (tree.children == null || tree.children.Count() == 0 || (accountLevel > 0 && tree.AccountLevel >= accountLevel))
			{
				list.Add(tree);
				return list;
			}
			list.Add(tree);
			foreach (AccountItemTreeModel child in tree.children)
			{
				list.AddRange(GetChildrenTreeNode(child, accountLevel));
			}
			return list;
		}

		public static List<AccountItemTreeModel> GetAccountTreeByLevel(AccountItemTreeModel tree, string startId, string endId, int accountLevel)
		{
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			if (tree.children == null || tree.children.Count() == 0 || (accountLevel > 0 && tree.AccountLevel >= accountLevel))
			{
				list.Add(tree);
				return list;
			}
			AccountItemTreeModel[] array = new AccountItemTreeModel[tree.children.Count()];
			tree.children.CopyTo(array);
			if (array == null || array.Length == 0 || (accountLevel > 0 && tree.AccountLevel >= accountLevel))
			{
				list.Add(tree);
				return list;
			}
			tree.children.Clear();
			AccountItemTreeModel[] array2 = array;
			foreach (AccountItemTreeModel tree2 in array2)
			{
				List<AccountItemTreeModel> accountTreeByLevel = GetAccountTreeByLevel(tree2, startId, endId, accountLevel);
				tree.children.AddRange(accountTreeByLevel);
			}
			if (!list.Contains(tree))
			{
				list.Add(tree);
			}
			return list;
		}

		public static List<BDAccountModel> GetChildrenList(List<BDAccountModel> accountList)
		{
			List<BDAccountModel> list = new List<BDAccountModel>();
			foreach (BDAccountModel account in accountList)
			{
				if (!accountList.Exists((BDAccountModel x) => x.MParentID == account.MItemID))
				{
					list.Add(account);
				}
			}
			return list;
		}

		public static bool IsProfitLossAccount(MContext ctx, BDAccountModel account)
		{
			bool result = false;
			string mAccountTypeID = account.MAccountTypeID;
			AccountTypeEnum accountTypeEnum = new AccountTypeEnum(ctx.MAccountTableID);
			if (mAccountTypeID == accountTypeEnum.OperatingCostsAndTaxes || mAccountTypeID == accountTypeEnum.OtherLoss || mAccountTypeID == accountTypeEnum.PeriodCharge || mAccountTypeID == accountTypeEnum.IncomeTax || (account.MAccountGroupID == "4" && account.MCode.StartsWith("6")) || account.MAccountGroupID == "5")
			{
				result = true;
			}
			return result;
		}

		public static List<AccountItemTreeModel> GetAccountTreeList(string startId, string endId, List<AccountItemTreeModel> tree, int accountLevel)
		{
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			foreach (AccountItemTreeModel item in tree)
			{
				List<string> accountIdListByTreeNode = GetAccountIdListByTreeNode(item, null, 0);
				if (string.IsNullOrWhiteSpace(startId) || accountIdListByTreeNode.Contains(startId) || list.Count() != 0)
				{
					if (!string.IsNullOrWhiteSpace(endId) && accountIdListByTreeNode.Contains(endId))
					{
						List<AccountItemTreeModel> childrenTreeNode = GetChildrenTreeNode(item, accountLevel);
						list.AddRange(childrenTreeNode);
						break;
					}
					List<AccountItemTreeModel> childrenTreeNode2 = GetChildrenTreeNode(item, accountLevel);
					list.AddRange(childrenTreeNode2);
				}
			}
			return list;
		}

		public static List<AccountItemTreeModel> GetLeafAccountTreeList(AccountItemTreeModel parentNode, BDAccountListFilterModel filter, bool ignoreEndIndex = false)
		{
			List<AccountItemTreeModel> list = new List<AccountItemTreeModel>();
			if (parentNode.children == null || parentNode.children.Count() == 0)
			{
				list.Add(parentNode);
			}
			List<AccountItemTreeModel> children = parentNode.children;
			foreach (AccountItemTreeModel item in children)
			{
				List<AccountItemTreeModel> leafAccountTreeList = GetLeafAccountTreeList(item, filter, false);
				if (!ignoreEndIndex && filter.AccountEndIndex > 0 && leafAccountTreeList.Exists((AccountItemTreeModel y) => y.Index >= filter.AccountEndIndex))
				{
					leafAccountTreeList = (from y in leafAccountTreeList
					where y.Index <= filter.AccountEndIndex
					select y).ToList();
					list.AddRange(leafAccountTreeList);
					return list;
				}
				if (leafAccountTreeList != null)
				{
					list.AddRange(leafAccountTreeList);
				}
			}
			return list;
		}

		public static List<BDAccountModel> OrderBy(List<BDAccountModel> accountList)
		{
			if (accountList == null || accountList.Count() == 0)
			{
				return accountList;
			}
			foreach (BDAccountModel account in accountList)
			{
				account.MOrderFiled = GetAccountOrderFieldValue(account.MNumber);
			}
			accountList = (from x in accountList
			orderby x.MOrderFiled
			select x).ToList();
			return accountList;
		}

		public static List<GLInitBalanceModel> OrderBy(List<GLInitBalanceModel> initBalanceList)
		{
			if (initBalanceList == null || initBalanceList.Count() == 0)
			{
				return initBalanceList;
			}
			foreach (GLInitBalanceModel initBalance in initBalanceList)
			{
				string mNumber = initBalance.MAccountModel.MNumber;
				initBalance.MOrderField = GetAccountOrderFieldValue(mNumber);
			}
			initBalanceList = (from x in initBalanceList
			orderby x.MOrderField
			select x).ToList();
			return initBalanceList;
		}

		public static List<GLBalanceModel> OrderBy(List<GLBalanceModel> balanceList)
		{
			if (balanceList == null || balanceList.Count() == 0)
			{
				return balanceList;
			}
			foreach (GLBalanceModel balance in balanceList)
			{
				string mNumber = balance.MNumber;
				balance.MOrderField = GetAccountOrderFieldValue(mNumber);
			}
			balanceList = (from x in balanceList
			orderby x.MOrderField
			select x).ToList();
			return balanceList;
		}

		public static List<RPTAccountBalanceModel> OrderBy(List<RPTAccountBalanceModel> list)
		{
			if (list == null || list.Count() == 0)
			{
				return list;
			}
			foreach (RPTAccountBalanceModel item in list)
			{
				string mNumberID = item.MNumberID;
				item.MOrderField = GetAccountOrderFieldValue(mNumberID);
			}
			list = (from x in list
			orderby x.MOrderField
			select x).ToList();
			return list;
		}

		private static string GetAccountOrderFieldValue(string number)
		{
			if (string.IsNullOrWhiteSpace(number))
			{
				return number;
			}
			string[] array = number.Split('.');
			string text = array[0];
			if (array.Length == 1)
			{
				return text;
			}
			string text2 = "0000";
			for (int i = 1; i < array.Length; i++)
			{
				string text3 = array[i];
				text3 = ((text3.Length < 4) ? (text2.Substring(0, text2.Length - text3.Length) + text3) : text3);
				text += text3;
			}
			return text;
		}
	}
}
