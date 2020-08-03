using Fasterflect;
using JieNor.Megi.Common;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.DataRepository.REG;
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

namespace JieNor.Megi.DataRepository.GL
{
	public class GLDocVoucherRepository : DataServiceT<GLDocVoucherModel>
	{
		public delegate bool Generate(MContext ctx, object model, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList);

		private class MIDEntryID
		{
			public string MID
			{
				get;
				set;
			}

			public string MEntryID
			{
				get;
				set;
			}
		}

		private readonly GLVoucherRepository _voucher = new GLVoucherRepository();

		private readonly GLVoucherEntryRepository _voucherEntry = new GLVoucherEntryRepository();

		private readonly BDAccountRepository accountRep = new BDAccountRepository();

		private readonly GLVoucherRepository voucherRep = new GLVoucherRepository();

		private readonly BDItemRepository itemRep = new BDItemRepository();

		private readonly BDContactsRepository contactsRep = new BDContactsRepository();

		public readonly REGTaxRateRepository taxRep = new REGTaxRateRepository();

		public readonly BDExpenseItemRepository expenseItemRep = new BDExpenseItemRepository();

		public readonly BDEmployeesRepository employeeRep = new BDEmployeesRepository();

		public readonly BDBankAccountRepository bankAccountRep = new BDBankAccountRepository();

		public readonly COMMultiLangRepository lang = new COMMultiLangRepository();

		public readonly GLSettlementRepository settlementRep = new GLSettlementRepository();

		public readonly GLUtility utility = new GLUtility();

		public OperationResult GenerateVouchersByBills<T>(MContext ctx, List<T> bills, List<T> dbBills = null) where T : BizDataModel
		{
			return GenerateVouchersByBills(ctx, bills, false, dbBills);
		}

		public OperationResult GenerateVouchersByBills<T>(MContext ctx, List<T> bills, List<T> dbBills = null, bool forceToGenerate = true) where T : BizDataModel
		{
			return GenerateVouchersByBills(ctx, bills, forceToGenerate, dbBills);
		}

		public OperationResult GenerateVouchersByBills<T>(MContext ctx, List<T> bills, bool forceToGenerate, List<T> dbBills = null) where T : BizDataModel
		{
			OperationResult result = new OperationResult
			{
				OperationCommands = new List<CommandInfo>(),
				Success = true
			};
			if (bills == null || !bills.Any())
			{
				return result;
			}
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			List<GLDocVoucherModel> list3 = new List<GLDocVoucherModel>();
			dbBills = (dbBills ?? new List<T>());
			GLDocTypeEnum docTypeByBill = GetDocTypeByBill(bills[0]);
			Generate generateMethod = GetGenerateMethod(docTypeByBill);
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			GLUtility gLUtility = new GLUtility();
			int billModuleByType = GetBillModuleByType(docTypeByBill);
			List<int> billPeriods = GetBillPeriods(bills);
			List<GLSimpleVoucherModel> simpleVouchersByPeriods = instance.GetSimpleVouchersByPeriods(billPeriods);
			List<T> list4 = new List<T>();
			int i;
			for (i = 0; i < bills.Count; i++)
			{
				bool flag = false;
				int num = 0;
				bool flag2 = false;
				T orginalModel = dbBills.FirstOrDefault((T x) => x?.MID == bills[i].MID);
				if (!forceToGenerate && !bills[i].IsNew)
				{
					HasBillChangedKeyValue(ctx, bills[i], bills[i].MID, ref flag, ref num, ref flag2, orginalModel);
					if (ValidatBillStatus(ctx, num, bills[i], simpleVouchersByPeriods))
					{
						if (flag2 && !flag && num == 1)
						{
							list4.Add(bills[i]);
						}
						else if (flag)
						{
							goto IL_0211;
						}
					}
					continue;
				}
				goto IL_0211;
				IL_0211:
				int billPeriod = GetBillPeriod(bills[i]);
				int num2 = billPeriod;
				DateTime mBeginDate = ctx.MBeginDate;
				int num3 = mBeginDate.Year * 100;
				mBeginDate = ctx.MBeginDate;
				if (num2 >= num3 + mBeginDate.Month)
				{
					List<GLVoucherModel> list5 = new List<GLVoucherModel>();
					List<GLDocVoucherModel> list6 = new List<GLDocVoucherModel>();
					bool flag3 = generateMethod(ctx, bills[i], ref list5, ref list6);
					bills[i].MGeneratedNewVoucher = true;
					list5.ForEach((Action<GLVoucherModel>)delegate(GLVoucherModel x)
					{
						x.MVoucherEntrys.ForEach((Action<GLVoucherEntryModel>)delegate(GLVoucherEntryModel y)
						{
							y.MModified = true;
						});
					});
					list.AddRange((IEnumerable<GLVoucherModel>)list5);
					list3.AddRange((IEnumerable<GLDocVoucherModel>)list6);
					if (!CanVoucherTransferToSaved(list5) || !flag3)
					{
						ValidateDraftVouchers(ctx, bills[i], ref list5, simpleVouchersByPeriods);
					}
					else
					{
						GLVoucherModel gLVoucherModel = TryCreateVoucher(ctx, bills[i], simpleVouchersByPeriods, billModuleByType, ref list5, ref list6, null, flag3);
						if (gLVoucherModel != null)
						{
							if (!list.Contains(gLVoucherModel))
							{
								list.Add(gLVoucherModel);
							}
							for (int j = 0; j < list6.Count; j++)
							{
								if (!list3.Contains(list6[j]))
								{
									list3.Add(list6[j]);
								}
							}
							list2.Add(gLVoucherModel);
						}
					}
				}
			}
			if (list4?.Any() ?? false)
			{
				TransferVouchersDraft2SavedByBills(ctx, list4, ref list, ref list2, ref list3, ref simpleVouchersByPeriods, null, true);
			}
			return AfterTransferVocuhers2Saved(ctx, bills, ref list, ref list2, ref list3, simpleVouchersByPeriods, instance);
		}

		public OperationResult GenerateVouchersByBill<T>(MContext ctx, T bill, T dbBill = null) where T : BizDataModel
		{
			return GenerateVouchersByBills(ctx, new List<T>
			{
				bill
			}, (dbBill == null) ? null : new List<T>
			{
				dbBill
			});
		}

		public OperationResult TransferVouchersDraft2SavedByBillIds(MContext ctx, List<string> billIds, List<GLDocEntryVoucherModel> docEntryVouchers = null, bool create = true)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLDocVoucherModel> list2 = new List<GLDocVoucherModel>();
			List<GLVoucherModel> list3 = new List<GLVoucherModel>();
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			List<GLSimpleVoucherModel> existsVouchers = null;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BizDataModel> bills = new List<BizDataModel>();
			billIds.ForEach(delegate(string x)
			{
				bills.Add(new BizDataModel("")
				{
					MID = x
				});
			});
			TransferVouchersDraft2SavedByBills(ctx, bills, ref list, ref list3, ref list2, ref existsVouchers, docEntryVouchers, create);
			operationResult = AfterTransferVocuhers2Saved(ctx, bills, ref list, ref list3, ref list2, existsVouchers, instance);
			List<BizDataModel> list4 = (from x in bills
			where x.ValidationErrors != null && x.ValidationErrors.Any()
			select x).ToList();
			if (list4?.Any() ?? false)
			{
				operationResult.MessageList = (from x in new GLUtility().Union((from x in list4
				select x.ValidationErrors).ToList())
				select x.Message.Trim()).Distinct().ToList();
				operationResult.Message = string.Join(";", operationResult.MessageList);
				operationResult.Success = false;
			}
			return operationResult;
		}

		public OperationResult TransferVouchersDraft2SavedByBillId(MContext ctx, string billId, List<GLDocEntryVoucherModel> docEntryVouchers = null, bool create = true)
		{
			return TransferVouchersDraft2SavedByBillIds(ctx, new List<string>
			{
				billId
			}, docEntryVouchers, create);
		}

		private OperationResult HandleBillErrorByFrom<T>(MContext ctx, ref List<T> bills, ref List<GLVoucherModel> voucherList, ref List<GLVoucherModel> createdVoucherList, ref List<GLDocVoucherModel> docVoucherList) where T : BizDataModel
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (ctx.AppSource == "System" || ctx.AppSource == "Excel")
			{
				if (bills.Exists((Predicate<T>)((T x) => x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors))))
				{
					operationResult.Success = false;
				}
			}
			else if (ctx.AppSource == "Api")
			{
				List<T> list = new List<T>();
				List<string> errorBillsId = new List<string>();
				for (int i = 0; i < bills.Count; i++)
				{
					if (bills[i].ValidationErrors != null && bills[i].ValidationErrors.Any())
					{
						errorBillsId.Add(bills[i].MID);
					}
				}
				voucherList = (from x in voucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				createdVoucherList = (from x in createdVoucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				docVoucherList = (from x in docVoucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				bills = (from x in bills
				where !errorBillsId.Contains(x.MID)
				select x).ToList();
			}
			return operationResult;
		}

		private OperationResult HandleVoucherErrorByFrom<T>(MContext ctx, ref List<T> bills, ref List<GLVoucherModel> voucherList, ref List<GLVoucherModel> createdVoucherList, ref List<GLDocVoucherModel> docVoucherList) where T : BizDataModel
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (ctx.AppSource == "System" || ctx.AppSource == "Excel")
			{
				if (voucherList.Exists((Predicate<GLVoucherModel>)((GLVoucherModel x) => x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors))))
				{
					operationResult.Success = false;
				}
			}
			else if (ctx.AppSource == "Api")
			{
				List<string> errorBillsId = new List<string>();
				for (int i = 0; i < voucherList.Count; i++)
				{
					if (voucherList[i].ValidationErrors != null && voucherList[i].ValidationErrors.Any())
					{
						errorBillsId.Add(voucherList[i].MDocID);
					}
				}
				voucherList = (from x in voucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				createdVoucherList = (from x in createdVoucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				docVoucherList = (from x in docVoucherList
				where !errorBillsId.Contains(x.MDocID)
				select x).ToList();
				bills = (from x in bills
				where !errorBillsId.Contains(x.MID)
				select x).ToList();
			}
			return operationResult;
		}

		private OperationResult AfterTransferVocuhers2Saved<T>(MContext ctx, List<T> bills, ref List<GLVoucherModel> voucherList, ref List<GLVoucherModel> createdVoucherList, ref List<GLDocVoucherModel> docVoucherList, List<GLSimpleVoucherModel> existsVouchers, GLDataPool pool) where T : BizDataModel
		{
			OperationResult result = new OperationResult
			{
				Success = true,
				OperationCommands = new List<CommandInfo>()
			};
			if (bills.Exists((Predicate<T>)((T x) => x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors))))
			{
				result.Success = false;
				return result;
			}
			string text = bills?.First()?.TableName;
			createdVoucherList = (from x in createdVoucherList
			where !x.IsDelete
			select x).ToList();
			List<GLVoucherModel> list = (from x in createdVoucherList
			where string.IsNullOrWhiteSpace(x.MNumber)
			select x).ToList();
			SetVoucherListNumbers(ctx, ref list);
			GLVoucherRepository.ValidateVouchers(ctx, createdVoucherList, existsVouchers, false);
			voucherList.ForEach((Action<GLVoucherModel>)delegate(GLVoucherModel x)
			{
				x.IsDelete = false;
			});
			docVoucherList.ForEach((Action<GLDocVoucherModel>)delegate(GLDocVoucherModel x)
			{
				x.IsDelete = false;
			});
			List<GLVoucherModel> list2 = (from x in createdVoucherList
			where x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors)
			select x).ToList() ?? new List<GLVoucherModel>();
			list2.ForEach((Action<GLVoucherModel>)delegate(GLVoucherModel x)
			{
				T val = Enumerable.FirstOrDefault<T>((IEnumerable<T>)bills, (Func<T, bool>)((T y) => y.MID == x.MDocID));
				val.ValidationErrors = (val.ValidationErrors ?? new List<ValidationError>());
				val.ValidationErrors.AddRange((IEnumerable<ValidationError>)x.ValidationErrors);
				result.Success = false;
			});
			if (voucherList == null || !voucherList.Any() || !result.Success)
			{
				return result;
			}
			List<CommandInfo> collection = GetFullyDeleteDocsCreatedVoucherCmds(ctx, (from x in bills
			where x.MGeneratedNewVoucher
			select x.MID).ToList(), existsVouchers) ?? new List<CommandInfo>();
			List<CommandInfo> insertOrUpdateVoucherCommandList = GetInsertOrUpdateVoucherCommandList(ctx, voucherList, text);
			List<CommandInfo> collection2 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, docVoucherList, null, "docVoucher" + text) ?? new List<CommandInfo>();
			List<CommandInfo> collection3 = GetCheckGroupValueCommandList(ctx, pool) ?? new List<CommandInfo>();
			List<CommandInfo> createLogCmds = GlVoucherLogHelper.GetCreateLogCmds(ctx, list);
			result.OperationCommands.AddRange((IEnumerable<CommandInfo>)collection);
			result.OperationCommands.AddRange((IEnumerable<CommandInfo>)insertOrUpdateVoucherCommandList);
			result.OperationCommands.AddRange((IEnumerable<CommandInfo>)collection2);
			result.OperationCommands.AddRange((IEnumerable<CommandInfo>)collection3);
			result.OperationCommands.AddRange((IEnumerable<CommandInfo>)createLogCmds);
			return result;
		}

		public void TransferVouchersDraft2SavedByBills<T>(MContext ctx, List<T> bills, ref List<GLVoucherModel> voucherList, ref List<GLVoucherModel> createdVoucherList, ref List<GLDocVoucherModel> docVoucherList, ref List<GLSimpleVoucherModel> existsVouchers, List<GLDocEntryVoucherModel> docEntryVouchers = null, bool create = false) where T : BizDataModel
		{
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			voucherList = (voucherList ?? new List<GLVoucherModel>());
			docVoucherList = (docVoucherList ?? new List<GLDocVoucherModel>());
			GLDocTypeEnum type = (GLDocTypeEnum)((docEntryVouchers != null && docEntryVouchers.Any()) ? docEntryVouchers[0].MDocType : ((int)GetDocTypeByBill(bills[0])));
			int billModuleByType = GetBillModuleByType(type);
			existsVouchers = (existsVouchers ?? instance.GetSimpleVouchersByDocIDs((from x in bills
			select x.MID).ToList()));
			List<GLDocVoucherModel> source = (from x in GetDocCreatedDocVouchers(ctx, (from x in bills
			select x.MID).ToList())
			orderby x.MDocID
			select x).ToList();
			List<GLVoucherModel> docCreatedVouchers = GetDocCreatedVouchers(ctx, (from x in bills
			select x.MID).ToList(), (from x in source
			select x.MVoucherID).ToList());
			int i;
			for (i = 0; i < bills.Count; i++)
			{
				List<GLDocVoucherModel> billCreatedDocVouchers = (from x in source
				where x.MDocID == bills[i].MID && x.MergeStatus != 1
				select x).ToList();
				List<GLDocVoucherModel> billCreatedMergedDocVouchers = (from x in source
				where x.MDocID == bills[i].MID && x.MergeStatus == 1
				select x).ToList();
				List<GLVoucherModel> list = (from x in docCreatedVouchers
				where Enumerable.Contains<string>(Enumerable.Select<GLDocVoucherModel, string>((IEnumerable<GLDocVoucherModel>)billCreatedDocVouchers, (Func<GLDocVoucherModel, string>)((GLDocVoucherModel y) => y.MVoucherID)), x.MItemID)
				select x).ToList();
				GLVoucherModel mergedVoucher = (billCreatedMergedDocVouchers.Count == 0) ? null : docCreatedVouchers.FirstOrDefault((GLVoucherModel x) => Enumerable.Contains<string>(Enumerable.Select<GLDocVoucherModel, string>((IEnumerable<GLDocVoucherModel>)billCreatedMergedDocVouchers, (Func<GLDocVoucherModel, string>)((GLDocVoucherModel y) => y.MVoucherID)), x.MItemID));
				int j;
				for (j = 0; j < billCreatedDocVouchers.Count; j++)
				{
					GLVoucherModel gLVoucherModel = list.FirstOrDefault((GLVoucherModel x) => x.MItemID == billCreatedDocVouchers[j].MVoucherID);
					if (docEntryVouchers != null)
					{
						GLDocEntryVoucherModel docEntryVoucher = (from x in docEntryVouchers
						where x.MDocVoucherID == billCreatedDocVouchers[j].MItemID
						select x).FirstOrDefault();
						GLVoucherEntryModel gLVoucherEntryModel = gLVoucherModel.MVoucherEntrys.Find((Predicate<GLVoucherEntryModel>)((GLVoucherEntryModel x) => x.MEntryID.Equals(docEntryVoucher.MDebitEntryID)));
						GLVoucherEntryModel gLVoucherEntryModel2 = gLVoucherModel.MVoucherEntrys.Find((Predicate<GLVoucherEntryModel>)((GLVoucherEntryModel x) => x.MEntryID.Equals(docEntryVoucher.MTaxEntryID)));
						GLVoucherEntryModel gLVoucherEntryModel3 = gLVoucherModel.MVoucherEntrys.Find((Predicate<GLVoucherEntryModel>)((GLVoucherEntryModel x) => x.MEntryID.Equals(docEntryVoucher.MCreditEntryID)));
						gLVoucherEntryModel.MAccountID = docEntryVoucher.MDebitAccountID;
						gLVoucherEntryModel.MModified = true;
						gLVoucherEntryModel3.MAccountID = docEntryVoucher.MCreditAccountID;
						gLVoucherEntryModel3.MModified = true;
						if (gLVoucherEntryModel2 != null)
						{
							gLVoucherEntryModel2.MAccountID = docEntryVoucher.MTaxAccountID;
							gLVoucherEntryModel2.MModified = true;
						}
					}
				}
				voucherList.AddRange((IEnumerable<GLVoucherModel>)list);
				docVoucherList.AddRange((IEnumerable<GLDocVoucherModel>)billCreatedDocVouchers);
				if (!CanVoucherTransferToSaved(list))
				{
					ValidateDraftVouchers(ctx, bills[i], ref list, existsVouchers);
				}
				else
				{
					GLVoucherModel gLVoucherModel2 = TryCreateVoucher(ctx, bills[i], existsVouchers, billModuleByType, ref list, ref billCreatedDocVouchers, mergedVoucher, true);
					if (gLVoucherModel2 != null)
					{
						if (!voucherList.Contains(gLVoucherModel2))
						{
							voucherList.Add(gLVoucherModel2);
						}
						for (int k = 0; k < billCreatedDocVouchers.Count; k++)
						{
							if (!docVoucherList.Contains(billCreatedDocVouchers[k]))
							{
								docVoucherList.Add(billCreatedDocVouchers[k]);
							}
						}
						createdVoucherList.Add(gLVoucherModel2);
					}
				}
			}
		}

		public GLVoucherModel TryCreateVoucher<T>(MContext ctx, T bill, List<GLSimpleVoucherModel> existsVouchers, int module, ref List<GLVoucherModel> billVouchers, ref List<GLDocVoucherModel> billDocVouchers, GLVoucherModel mergedVoucher, bool create) where T : BizDataModel
		{
			GLVoucherRepository.ValidateVouchers(ctx, billVouchers, existsVouchers, true);
			if (billVouchers.Exists((Predicate<GLVoucherModel>)((GLVoucherModel x) => x.ValidationErrors != null && Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)x.ValidationErrors))))
			{
				bill.ValidationErrors = (bill.ValidationErrors ?? new List<ValidationError>());
				bill.ValidationErrors.AddRange((IEnumerable<ValidationError>)utility.Union((from x in billVouchers
				select x.ValidationErrors).ToList()));
				return null;
			}
			if (!create || !CanVoucherTransferToSaved(billVouchers))
			{
				return null;
			}
			GLVoucherModel voucher = null;
			if (billVouchers.Count == 1)
			{
				voucher = billVouchers[0];
				(from x in billDocVouchers
				where x.MVoucherID == voucher.MItemID
				select x).ToList().ForEach((Action<GLDocVoucherModel>)delegate(GLDocVoucherModel x)
				{
					x.MStatus = 0;
				});
			}
			else
			{
				voucher = Merge(ctx, ref billVouchers, ref billDocVouchers, module);
			}
			GLSimpleVoucherModel gLSimpleVoucherModel = existsVouchers.FirstOrDefault((GLSimpleVoucherModel x) => (mergedVoucher != null) ? (x.MItemID == mergedVoucher.MItemID) : (x.MDocID == bill.MID));
			int num3;
			if (gLSimpleVoucherModel != null)
			{
				int num = gLSimpleVoucherModel.MYear * 100 + gLSimpleVoucherModel.MPeriod;
				DateTime mDate = voucher.MDate;
				int num2 = mDate.Year * 100;
				mDate = voucher.MDate;
				if (num == num2 + mDate.Month)
				{
					num3 = ((!string.IsNullOrWhiteSpace(gLSimpleVoucherModel.MNumber)) ? 1 : 0);
					goto IL_01d5;
				}
			}
			num3 = 0;
			goto IL_01d5;
			IL_01d5:
			if (num3 != 0)
			{
				voucher.MNumber = gLSimpleVoucherModel.MNumber;
				gLSimpleVoucherModel.MNumber = null;
			}
			CalculateVoucherTotal(voucher);
			voucher.MStatus = 0;
			voucher.MSourceBillKey = 2.ToString();
			voucher.MDocID = bill.MID;
			if (string.IsNullOrWhiteSpace(voucher.MItemID))
			{
				voucher.MItemID = JieNor.Megi.Common.UUIDHelper.GetGuid();
				voucher.IsNew = true;
				voucher.MVoucherEntrys.ForEach((Action<GLVoucherEntryModel>)delegate(GLVoucherEntryModel x)
				{
					x.IsNew = true;
					x.MID = voucher.MID;
					x.MModified = true;
				});
			}
			else if (!voucher.IsNew)
			{
				(from x in billDocVouchers
				where x.MVoucherID == voucher.MItemID
				select x).ToList().ForEach((Action<GLDocVoucherModel>)delegate(GLDocVoucherModel x)
				{
					x.IsNew = false;
				});
				voucher.MVoucherEntrys.ForEach((Action<GLVoucherEntryModel>)delegate(GLVoucherEntryModel x)
				{
					x.IsNew = false;
					x.MID = voucher.MID;
					x.MModified = true;
				});
			}
			bill.MVoucherID = voucher.MItemID;
			return voucher;
		}

		public OperationResult ResetDocVoucher(MContext ctx, List<string> docIDs, int docType)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (docIDs == null || docIDs.Count == 0)
			{
				return operationResult;
			}
			docIDs = docIDs.Distinct().ToList();
			OperationResult operationResult2 = CanDocBeOperate(ctx, docIDs);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			List<CommandInfo> partlyDeleteDocsCreatedVoucherCmds = GetPartlyDeleteDocsCreatedVoucherCmds(ctx, docIDs, null);
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(partlyDeleteDocsCreatedVoucherCmds) > 0);
			if (!operationResult.Success)
			{
				operationResult.Message = COMMultiLangRepository.GetText(LangModule.Common, "Fail", "");
				return operationResult;
			}
			GLDataPool.RemovePool(ctx, false);
			switch (docType)
			{
			case 0:
			case 1:
			{
				List<IVInvoiceModel> dataModelList5 = ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, new SqlWhere().In("MID", docIDs.ToArray()), false, true);
				operationResult = GenerateVouchersByBills(ctx, dataModelList5, true, dataModelList5);
				break;
			}
			case 2:
			{
				List<IVExpenseModel> dataModelList4 = ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, new SqlWhere().In("MID", docIDs.ToArray()), false, true);
				operationResult = GenerateVouchersByBills(ctx, dataModelList4, true, dataModelList4);
				break;
			}
			case 3:
			{
				List<IVReceiveModel> dataModelList3 = ModelInfoManager.GetDataModelList<IVReceiveModel>(ctx, new SqlWhere().In("MID", docIDs.ToArray()), false, true);
				operationResult = GenerateVouchersByBills(ctx, dataModelList3, true, dataModelList3);
				break;
			}
			case 4:
			{
				List<IVPaymentModel> dataModelList2 = ModelInfoManager.GetDataModelList<IVPaymentModel>(ctx, new SqlWhere().In("MID", docIDs.ToArray()), false, true);
				operationResult = GenerateVouchersByBills(ctx, dataModelList2, true, dataModelList2);
				break;
			}
			case 5:
			{
				List<IVTransferModel> dataModelList = ModelInfoManager.GetDataModelList<IVTransferModel>(ctx, docIDs);
				operationResult = GenerateVouchersByBills(ctx, dataModelList, true, dataModelList);
				break;
			}
			}
			if (operationResult.Success && operationResult.OperationCommands != null && operationResult.OperationCommands.Count > 0)
			{
				operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(operationResult.OperationCommands) > 0);
			}
			return operationResult;
		}

		public static bool IsBillCreatedVoucher(MContext ctx, string billId)
		{
			List<GLDocVoucherModel> docVoucherListByDocID = GetDocVoucherListByDocID(ctx, billId);
			return docVoucherListByDocID?.Any() ?? false;
		}

		public static List<GLDocVoucherModel> GetDocVoucherListByDocID(MContext ctx, string docID)
		{
			string sql = "select distinct * from t_gl_doc_voucher where MDocID = @MDocID and MIsDelete = 0 and MIsActive = 1 and MOrgID = @MOrgID";
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, sql, new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MDocID",
					Value = docID
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			});
		}

		public List<CommandInfo> GetTransferVouchersSaved2DraftByBillIdsCmds(MContext ctx, List<string> docIds)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(GetPartlyDeleteDocsCreatedVoucherCmds(ctx, docIds, null));
			return list;
		}

		public List<CommandInfo> GetTransferVouchersSaved2DraftByBillIdCmds(MContext ctx, string billId)
		{
			return GetTransferVouchersSaved2DraftByBillIdsCmds(ctx, new List<string>
			{
				billId
			});
		}

		public OperationResult TransferVouchersSaved2DraftByBillIds(MContext ctx, List<string> billIds)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			OperationResult operationResult2 = CanDocBeOperate(ctx, billIds);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			operationResult.OperationCommands = GetTransferVouchersSaved2DraftByBillIdsCmds(ctx, billIds);
			return operationResult;
		}

		public OperationResult TransferVouchersSaved2DraftByBillId(MContext ctx, string billId)
		{
			return TransferVouchersSaved2DraftByBillIds(ctx, new List<string>
			{
				billId
			});
		}

		public OperationResult CanDocBeOperate(MContext ctx, List<string> billIds)
		{
			OperationResult operationResult = new OperationResult();
			if (billIds == null || billIds.Count == 0)
			{
				operationResult.Success = true;
				return operationResult;
			}
			operationResult.Success = CanDeleteOrUnapproveDoc(ctx, billIds);
			operationResult.Message = (operationResult.Success ? "" : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PeriodClosedOrExisitsApprovedVouchers", "当前期间已经结账或者单据对应的凭证已经审核,请先反结账或反审核凭证"));
			return operationResult;
		}

		public List<CommandInfo> GetFullyDeleteDocsCreatedVoucherCmds(MContext ctx, List<string> docIDs, List<GLSimpleVoucherModel> vouchers)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (docIDs == null || !docIDs.Any())
			{
				return list;
			}
			vouchers = (vouchers ?? GLDataPool.GetInstance(ctx, false, 0, 0, 0).GetSimpleVouchersByDocIDs(docIDs));
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(docIDs, ref list2, "M_ID");
			List<string> list3 = (from x in (from x in vouchers
			where docIDs.Contains(x.MDocID)
			select x)
			select x.MItemID)?.ToList();
			new GLVoucherRepository().ValidateDeleteVouchers(ctx, list3);
			CommandInfo obj = new CommandInfo
			{
				CommandText = "update t_gl_voucher t1, t_gl_doc_voucher t2 set t1.MIsDelete = 1 where t1.MOrgID = t2.MOrgID and t1.MOrgID = @MOrgID and t1.MItemId = t2.MVoucherID and t1.MIsDelete = 0 and t2.MIsDelete = 0  and t2.MDocID " + inFilterQuery
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			CommandInfo item = obj;
			list.Add(item);
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "update t_gl_voucherentry t1, t_gl_doc_voucher t2 set t1.MIsDelete = 1 where t1.MOrgID = t2.MOrgID and t1.MOrgID = @MOrgID and t1.MID = t2.MVoucherID and t1.MIsDelete = 0 and t2.MIsDelete = 0  and t2.MDocID " + inFilterQuery
			};
			array = (obj2.Parameters = list2.ToArray());
			CommandInfo item2 = obj2;
			list.Add(item2);
			CommandInfo obj3 = new CommandInfo
			{
				CommandText = "update t_gl_doc_voucher set MIsDelete = 1 where MOrgID = @MOrgID and MIsDelete = 0 and MDocID " + inFilterQuery
			};
			array = (obj3.Parameters = list2.ToArray());
			CommandInfo item3 = obj3;
			list.Add(item3);
			List<CommandInfo> collection = (list3 == null || !list3.Any()) ? new List<CommandInfo>() : new GLVoucherRepository().GetDeleteVoucherRelatedCmds(ctx, list3);
			list.AddRange(collection);
			return list;
		}

		public List<CommandInfo> GetPartlyDeleteDocsCreatedVoucherCmds(MContext ctx, List<string> docIDs, List<GLSimpleVoucherModel> vouchers)
		{
			vouchers = (vouchers ?? GLDataPool.GetInstance(ctx, false, 0, 0, 0).GetSimpleVouchersByDocIDs(docIDs));
			List<CommandInfo> list = new List<CommandInfo>();
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			string inFilterQuery = GLUtility.GetInFilterQuery(docIDs, ref list2, "M_ID");
			List<string> list3 = (from x in (from x in vouchers
			where docIDs.Contains(x.MDocID)
			select x)
			select x.MItemID)?.ToList();
			new GLVoucherRepository().ValidateDeleteVouchers(ctx, list3);
			CommandInfo obj = new CommandInfo
			{
				CommandText = "update t_gl_voucher t1, t_gl_doc_voucher t2 set t1.MNumber = null, t1.MStatus = -1 where t1.MOrgID = t2.MOrgID and t1.MOrgID = @MOrgID and t1.MItemId = t2.MVoucherID and t1.MIsDelete = 0 and t2.MIsDelete = 0  and t2.MDocID " + inFilterQuery + " "
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			CommandInfo item = obj;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = "update t_gl_voucher t1, t_gl_doc_voucher t2 set t1.MIsDelete = 1 where t1.MOrgID = t2.MOrgID and t1.MOrgID = @MOrgID and t1.MItemId = t2.MVoucherID and t1.MIsDelete = 0 and t2.MIsDelete = 0  and t2.MDocID " + inFilterQuery + " and t2.MergeStatus = 1 "
			};
			array = (obj2.Parameters = list2.ToArray());
			CommandInfo item2 = obj2;
			CommandInfo obj3 = new CommandInfo
			{
				CommandText = "update t_gl_voucherentry t1, t_gl_doc_voucher t2 set t1.MIsDelete = 1 where t1.MOrgID = t2.MOrgID and t1.MOrgID = @MOrgID and t1.MID = t2.MVoucherID and t1.MIsDelete = 0 and t2.MIsDelete = 0  and t2.MDocID " + inFilterQuery + " and t2.MergeStatus = 1 "
			};
			array = (obj3.Parameters = list2.ToArray());
			CommandInfo item3 = obj3;
			CommandInfo obj4 = new CommandInfo
			{
				CommandText = "update t_gl_doc_voucher t1 set t1.MIsDelete = 1 where  t1.MOrgID = @MOrgID and t1.MIsDelete = 0  and t1.MDocID " + inFilterQuery + " and t1.MergeStatus = 1 "
			};
			array = (obj4.Parameters = list2.ToArray());
			CommandInfo item4 = obj4;
			CommandInfo obj5 = new CommandInfo
			{
				CommandText = "update t_gl_doc_voucher t1 set t1.MIsActive = 1, t1.MergeStatus = 0 where t1.MOrgID = @MOrgID and t1.MIsDelete = 0 and t1.MDocID " + inFilterQuery + " and t1.MergeStatus = -1 "
			};
			array = (obj5.Parameters = list2.ToArray());
			CommandInfo item5 = obj5;
			List<CommandInfo> collection = (list3 == null || !list3.Any()) ? new List<CommandInfo>() : new GLVoucherRepository().GetDeleteVoucherRelatedCmds(ctx, list3);
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			list.Add(item5);
			list.AddRange(collection);
			return list;
		}

		private List<GLDocVoucherModel> GetDocCreatedDocVouchers(MContext ctx, List<string> docIDs)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = "select * from t_gl_doc_voucher t where t.MOrgID = @MOrgID and MIsDelete = 0 and MDocID " + GLUtility.GetInFilterQuery(docIDs, ref list, "M_ID");
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, sql, list.ToArray());
		}

		private List<GLVoucherModel> GetDocCreatedVouchers(MContext ctx, List<string> docIDs, List<string> voucherIds = null)
		{
			voucherIds = (voucherIds ?? (from x in GetDocCreatedDocVouchers(ctx, docIDs)
			select x.MVoucherID).Distinct().ToList());
			return _voucher.GetVoucherModelList(ctx, voucherIds, true, 0, 0);
		}

		public void HasBillChangedKeyValue(MContext ctx, object model, string billID, ref bool valueChanged, ref int statusChanged, ref bool exists, object orginalModel = null)
		{
			if (!string.IsNullOrWhiteSpace(billID) || orginalModel != null)
			{
				DateTime mBizDate;
				if (model is IVInvoiceModel || model.GetType().IsAssignableFrom(typeof(IVInvoiceModel)))
				{
					IVInvoiceModel iVInvoiceModel = (orginalModel ?? IVInvoiceRepository.GetInvoiceEditModel(ctx, billID)) as IVInvoiceModel;
					if (iVInvoiceModel != null)
					{
						exists = true;
						IVInvoiceModel iVInvoiceModel2 = model as IVInvoiceModel;
						bool num = valueChanged;
						mBizDate = iVInvoiceModel2.MBizDate;
						int num2 = mBizDate.Year * 12;
						mBizDate = iVInvoiceModel2.MBizDate;
						int num3 = num2 + mBizDate.Month;
						mBizDate = iVInvoiceModel.MBizDate;
						int num4 = mBizDate.Year * 12;
						mBizDate = iVInvoiceModel.MBizDate;
						valueChanged = (num | num3 != num4 + mBizDate.Month);
						valueChanged |= CompareStringNotEqual(iVInvoiceModel2.MTaxID, iVInvoiceModel.MTaxID);
						valueChanged |= CompareStringNotEqual(iVInvoiceModel2.MContactID, iVInvoiceModel.MContactID);
						valueChanged |= (iVInvoiceModel2.MCyID != iVInvoiceModel.MCyID);
						valueChanged |= (iVInvoiceModel2.MOToLRate != iVInvoiceModel.MOToLRate);
						valueChanged |= (iVInvoiceModel2.InvoiceEntry.Count != iVInvoiceModel.InvoiceEntry.Count);
						statusChanged |= GetStatusChanged(iVInvoiceModel2.MStatus, iVInvoiceModel.MStatus);
						if (!valueChanged)
						{
							for (int i = 0; i < iVInvoiceModel2.InvoiceEntry.Count; i++)
							{
								IVInvoiceEntryModel entry = iVInvoiceModel2.InvoiceEntry[i];
								IVInvoiceEntryModel iVInvoiceEntryModel = (from x in iVInvoiceModel.InvoiceEntry
								where x.MEntryID == entry.MEntryID
								select x).FirstOrDefault();
								valueChanged |= (iVInvoiceEntryModel == null);
								if (!valueChanged)
								{
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTaxID, entry.MTaxID);
									valueChanged |= (iVInvoiceEntryModel.MItemID != entry.MItemID);
									valueChanged |= (iVInvoiceEntryModel.MDesc != entry.MDesc);
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTrackItem1, entry.MTrackItem1);
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTrackItem2, entry.MTrackItem2);
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTrackItem3, entry.MTrackItem3);
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTrackItem4, entry.MTrackItem4);
									valueChanged |= CompareStringNotEqual(iVInvoiceEntryModel.MTrackItem5, entry.MTrackItem5);
									valueChanged |= (iVInvoiceEntryModel.MAmount != entry.MAmount);
									valueChanged |= (iVInvoiceEntryModel.MAmountFor != entry.MAmountFor);
									valueChanged |= (iVInvoiceEntryModel.MTaxAmt != entry.MTaxAmt);
									valueChanged |= (iVInvoiceEntryModel.MTaxAmtFor != entry.MTaxAmtFor);
								}
							}
						}
					}
				}
				else if (model is IVExpenseModel || model.GetType().IsAssignableFrom(typeof(IVExpenseModel)))
				{
					IVExpenseModel iVExpenseModel = (orginalModel ?? IVExpenseRepository.GetExpenseModel(ctx, billID)) as IVExpenseModel;
					if (iVExpenseModel != null)
					{
						exists = true;
						IVExpenseModel iVExpenseModel2 = model as IVExpenseModel;
						bool num5 = valueChanged;
						mBizDate = iVExpenseModel2.MBizDate;
						int num6 = mBizDate.Year * 12;
						mBizDate = iVExpenseModel2.MBizDate;
						int num7 = num6 + mBizDate.Month;
						mBizDate = iVExpenseModel.MBizDate;
						int num8 = mBizDate.Year * 12;
						mBizDate = iVExpenseModel.MBizDate;
						valueChanged = (num5 | num7 != num8 + mBizDate.Month);
						valueChanged |= CompareStringNotEqual(iVExpenseModel2.MTaxID, iVExpenseModel.MTaxID);
						valueChanged |= (iVExpenseModel2.MType != iVExpenseModel.MType);
						valueChanged |= CompareStringNotEqual(iVExpenseModel2.MContactID, iVExpenseModel.MContactID);
						valueChanged |= CompareStringNotEqual(iVExpenseModel2.MEmployee, iVExpenseModel.MEmployee);
						valueChanged |= (iVExpenseModel2.MCyID != iVExpenseModel.MCyID);
						valueChanged |= (iVExpenseModel2.MOToLRate != iVExpenseModel.MOToLRate);
						valueChanged |= (iVExpenseModel2.ExpenseEntry.Count != iVExpenseModel.ExpenseEntry.Count);
						statusChanged = GetStatusChanged(iVExpenseModel2.MStatus, iVExpenseModel.MStatus);
						if (!valueChanged)
						{
							for (int j = 0; j < iVExpenseModel2.ExpenseEntry.Count; j++)
							{
								IVExpenseEntryModel entry2 = iVExpenseModel2.ExpenseEntry[j];
								IVExpenseEntryModel iVExpenseEntryModel = (from x in iVExpenseModel.ExpenseEntry
								where x.MEntryID == entry2.MEntryID
								select x).FirstOrDefault();
								valueChanged |= (iVExpenseEntryModel == null);
								if (!valueChanged)
								{
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTaxID, entry2.MTaxID);
									valueChanged |= (iVExpenseEntryModel.MItemID != entry2.MItemID);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MDesc, entry2.MDesc);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTrackItem1, entry2.MTrackItem1);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTrackItem2, entry2.MTrackItem2);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTrackItem3, entry2.MTrackItem3);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTrackItem4, entry2.MTrackItem4);
									valueChanged |= CompareStringNotEqual(iVExpenseEntryModel.MTrackItem5, entry2.MTrackItem5);
									valueChanged |= (iVExpenseEntryModel.MAmount != entry2.MAmount);
									valueChanged |= (iVExpenseEntryModel.MAmountFor != entry2.MAmountFor);
									valueChanged |= (iVExpenseEntryModel.MTaxAmt != entry2.MTaxAmt);
									valueChanged |= (iVExpenseEntryModel.MTaxAmtFor != entry2.MTaxAmtFor);
								}
							}
						}
					}
				}
				else if (model is IVPaymentModel || model.GetType().IsAssignableFrom(typeof(IVPaymentModel)))
				{
					IVPaymentModel iVPaymentModel = (orginalModel ?? IVPaymentRepository.GetPaymentEditModel(ctx, billID)) as IVPaymentModel;
					if (iVPaymentModel != null)
					{
						exists = true;
						IVPaymentModel iVPaymentModel2 = model as IVPaymentModel;
						bool num9 = valueChanged;
						mBizDate = iVPaymentModel2.MBizDate;
						int num10 = mBizDate.Year * 12;
						mBizDate = iVPaymentModel2.MBizDate;
						int num11 = num10 + mBizDate.Month;
						mBizDate = iVPaymentModel.MBizDate;
						int num12 = mBizDate.Year * 12;
						mBizDate = iVPaymentModel.MBizDate;
						valueChanged = (num9 | num11 != num12 + mBizDate.Month);
						valueChanged |= CompareStringNotEqual(iVPaymentModel2.MTaxID, iVPaymentModel.MTaxID);
						valueChanged |= (iVPaymentModel2.MType != iVPaymentModel.MType);
						valueChanged |= CompareStringNotEqual(iVPaymentModel2.MContactID, iVPaymentModel.MContactID);
						valueChanged |= (iVPaymentModel2.MCyID != iVPaymentModel.MCyID);
						valueChanged |= (iVPaymentModel2.MOToLRate != iVPaymentModel.MOToLRate);
						valueChanged |= CompareStringNotEqual(iVPaymentModel2.MReference, iVPaymentModel.MReference);
						valueChanged |= (iVPaymentModel2.PaymentEntry.Count != iVPaymentModel.PaymentEntry.Count);
						if (!valueChanged)
						{
							for (int k = 0; k < iVPaymentModel2.PaymentEntry.Count; k++)
							{
								IVPaymentEntryModel entry3 = iVPaymentModel2.PaymentEntry[k];
								IVPaymentEntryModel iVPaymentEntryModel = (from x in iVPaymentModel.PaymentEntry
								where x.MEntryID == entry3.MEntryID
								select x).FirstOrDefault();
								valueChanged |= (iVPaymentEntryModel == null);
								if (!valueChanged)
								{
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTaxID, entry3.MTaxID);
									valueChanged |= (iVPaymentEntryModel.MItemID != entry3.MItemID);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MDesc, entry3.MDesc);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTrackItem1, entry3.MTrackItem1);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTrackItem2, entry3.MTrackItem2);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTrackItem3, entry3.MTrackItem3);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTrackItem4, entry3.MTrackItem4);
									valueChanged |= CompareStringNotEqual(iVPaymentEntryModel.MTrackItem5, entry3.MTrackItem5);
									valueChanged |= (iVPaymentEntryModel.MAmount != entry3.MAmount);
									valueChanged |= (iVPaymentEntryModel.MAmountFor != entry3.MAmountFor);
									valueChanged |= (iVPaymentEntryModel.MTaxAmt != entry3.MTaxAmt);
									valueChanged |= (iVPaymentEntryModel.MTaxAmtFor != entry3.MTaxAmtFor);
								}
							}
						}
					}
				}
				else if (model is IVReceiveModel || model.GetType().IsAssignableFrom(typeof(IVReceiveModel)))
				{
					IVReceiveModel iVReceiveModel = (orginalModel ?? IVReceiveRepository.GetReceiveEditModel(ctx, billID)) as IVReceiveModel;
					if (iVReceiveModel != null)
					{
						exists = true;
						IVReceiveModel iVReceiveModel2 = model as IVReceiveModel;
						bool num13 = valueChanged;
						mBizDate = iVReceiveModel2.MBizDate;
						int num14 = mBizDate.Year * 12;
						mBizDate = iVReceiveModel2.MBizDate;
						int num15 = num14 + mBizDate.Month;
						mBizDate = iVReceiveModel.MBizDate;
						int num16 = mBizDate.Year * 12;
						mBizDate = iVReceiveModel.MBizDate;
						valueChanged = (num13 | num15 != num16 + mBizDate.Month);
						valueChanged |= CompareStringNotEqual(iVReceiveModel2.MTaxID, iVReceiveModel.MTaxID);
						valueChanged |= (iVReceiveModel2.MType != iVReceiveModel.MType);
						valueChanged |= CompareStringNotEqual(iVReceiveModel2.MContactID, iVReceiveModel.MContactID);
						valueChanged |= (iVReceiveModel2.MCyID != iVReceiveModel.MCyID);
						valueChanged |= (iVReceiveModel2.MOToLRate != iVReceiveModel.MOToLRate);
						valueChanged |= CompareStringNotEqual(iVReceiveModel2.MReference, iVReceiveModel.MReference);
						valueChanged |= (iVReceiveModel2.ReceiveEntry.Count != iVReceiveModel.ReceiveEntry.Count);
						if (!valueChanged)
						{
							for (int l = 0; l < iVReceiveModel2.ReceiveEntry.Count; l++)
							{
								IVReceiveEntryModel entry4 = iVReceiveModel2.ReceiveEntry[l];
								IVReceiveEntryModel iVReceiveEntryModel = (from x in iVReceiveModel.ReceiveEntry
								where x.MEntryID == entry4.MEntryID
								select x).FirstOrDefault();
								valueChanged |= (iVReceiveEntryModel == null);
								if (!valueChanged)
								{
									valueChanged |= ((iVReceiveEntryModel.MTaxID ?? "") != (entry4.MTaxID ?? ""));
									valueChanged |= (iVReceiveEntryModel.MItemID != entry4.MItemID);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MDesc, entry4.MDesc);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MTrackItem1, entry4.MTrackItem1);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MTrackItem2, entry4.MTrackItem2);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MTrackItem3, entry4.MTrackItem3);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MTrackItem4, entry4.MTrackItem4);
									valueChanged |= CompareStringNotEqual(iVReceiveEntryModel.MTrackItem5, entry4.MTrackItem5);
									valueChanged |= (iVReceiveEntryModel.MAmount != entry4.MAmount);
									valueChanged |= (iVReceiveEntryModel.MAmountFor != entry4.MAmountFor);
									valueChanged |= (iVReceiveEntryModel.MTaxAmt != entry4.MTaxAmt);
									valueChanged |= (iVReceiveEntryModel.MTaxAmtFor != entry4.MTaxAmtFor);
								}
							}
						}
					}
				}
				else if (model is IVTransferModel || model.GetType().IsAssignableFrom(typeof(IVTransferModel)))
				{
					IVTransferModel iVTransferModel = (orginalModel ?? IVTransferRepository.GetTransferEditModel(ctx, billID)) as IVTransferModel;
					if (iVTransferModel != null)
					{
						exists = true;
						IVTransferModel iVTransferModel2 = model as IVTransferModel;
						bool num17 = valueChanged;
						mBizDate = iVTransferModel2.MBizDate;
						int num18 = mBizDate.Year * 12;
						mBizDate = iVTransferModel2.MBizDate;
						int num19 = num18 + mBizDate.Month;
						mBizDate = iVTransferModel.MBizDate;
						int num20 = mBizDate.Year * 12;
						mBizDate = iVTransferModel.MBizDate;
						valueChanged = (num17 | num19 != num20 + mBizDate.Month);
						valueChanged |= (iVTransferModel2.MFromAcctID != iVTransferModel.MFromAcctID);
						valueChanged |= (iVTransferModel2.MToAcctID != iVTransferModel.MToAcctID);
						valueChanged |= (iVTransferModel2.MFromCyID != iVTransferModel.MFromCyID);
						valueChanged |= (iVTransferModel2.MToCyID != iVTransferModel.MToCyID);
						valueChanged |= CompareStringNotEqual(iVTransferModel2.MReference, iVTransferModel.MReference);
						valueChanged |= (iVTransferModel2.MFromTotalAmt != iVTransferModel.MFromTotalAmt);
						valueChanged |= (iVTransferModel2.MFromTotalAmtFor != iVTransferModel.MFromTotalAmtFor);
						valueChanged |= (iVTransferModel2.MToTotalAmt != iVTransferModel.MToTotalAmt);
						valueChanged |= (iVTransferModel2.MToTotalAmtFor != iVTransferModel.MToTotalAmtFor);
						valueChanged |= (iVTransferModel2.MExchangeRate != iVTransferModel.MExchangeRate);
						valueChanged |= (iVTransferModel2.MBeginExchangeRate != iVTransferModel.MBeginExchangeRate);
					}
				}
			}
		}

		private bool CanVoucherTransferToSaved(List<GLVoucherModel> voucherList)
		{
			for (int i = 0; i < voucherList.Count; i++)
			{
				if (voucherList[i].MVoucherEntrys.Exists((GLVoucherEntryModel x) => string.IsNullOrWhiteSpace(x.MAccountID)))
				{
					return false;
				}
			}
			return true;
		}

		private void ValidateDraftVouchers<T>(MContext ctx, T bill, ref List<GLVoucherModel> billVouchers, List<GLSimpleVoucherModel> existsVouchers) where T : BizDataModel
		{
			GLVoucherRepository.ValidateVouchers(ctx, billVouchers, existsVouchers, true);
			if (billVouchers.Exists((Predicate<GLVoucherModel>)delegate(GLVoucherModel x)
			{
				List<ValidationError> validationErrors2 = x.ValidationErrors;
				return validationErrors2 != null && validationErrors2.Count > 0;
			}))
			{
				bill.ValidationErrors = (bill.ValidationErrors ?? new List<ValidationError>());
				billVouchers.ForEach((Action<GLVoucherModel>)delegate(GLVoucherModel x)
				{
					List<ValidationError> validationErrors = x.ValidationErrors;
					if (validationErrors != null && validationErrors.Count > 0)
					{
						bill.ValidationErrors.AddRange((IEnumerable<ValidationError>)x.ValidationErrors);
					}
				});
			}
		}

		private bool ValidatBillStatus<T>(MContext ctx, int statusChanged, T bill, List<GLSimpleVoucherModel> existsVouchers) where T : BizDataModel
		{
			if (bill == null)
			{
				return true;
			}
			if (statusChanged != 0 && existsVouchers != null && existsVouchers.Exists((Predicate<GLSimpleVoucherModel>)((GLSimpleVoucherModel x) => x.MDocID == bill.MID && x.MStatus >= 0)))
			{
				bill.Validate(ctx, true, "VoucherOfBillHasCreated", "账单对应的凭证已经生成，不可修改其状态", LangModule.Common);
				return false;
			}
			return true;
		}

		private int GetStatusChanged(int current, int original)
		{
			if (current < Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && original >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
			{
				return -1;
			}
			if (current >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && original < Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
			{
				return 1;
			}
			return 0;
		}

		private static bool CompareStringNotEqual(string a, string b)
		{
			a = (a ?? "");
			b = (b ?? "");
			return a != b;
		}

		private Generate GetGenerateMethod(GLDocTypeEnum type)
		{
			switch (type)
			{
			case GLDocTypeEnum.Invoice:
				return this.GenerateVoucherByInvoice;
			case GLDocTypeEnum.Bill:
				return this.GenerateVoucherByPurchase;
			case GLDocTypeEnum.Expense:
				return this.GenerateVoucherByExpense;
			case GLDocTypeEnum.Payment:
				return this.GenerateVoucherByPayment;
			case GLDocTypeEnum.Receive:
				return this.GenerateVoucherByReceive;
			case GLDocTypeEnum.Transfer:
				return this.GenerateVoucherByTransfer;
			default:
				return null;
			}
		}

		private List<int> GetBillPeriods<T>(List<T> bills) where T : BizDataModel
		{
			List<int> list = new List<int>();
			foreach (T bill in bills)
			{
				DateTime dateTime = Convert.ToDateTime(bill.GetPropertyValue("MBizDate"));
				int item = dateTime.Year * 100 + dateTime.Month;
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private int GetBillPeriod<T>(T bill) where T : BizDataModel
		{
			DateTime dateTime = Convert.ToDateTime(bill.GetPropertyValue("MBizDate"));
			return dateTime.Year * 100 + dateTime.Month;
		}

		private List<CommandInfo> GetCheckGroupValueCommandList(MContext ctx, GLDataPool pool)
		{
			List<GLCheckGroupValueModel> list = (from x in pool.CheckGroupValueList
			where x.IsNew
			select x).ToList();
			if (list == null || !list.Any())
			{
				return new List<CommandInfo>();
			}
			List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true);
			foreach (GLCheckGroupValueModel item in list)
			{
				item.IsNew = false;
			}
			return insertOrUpdateCmds;
		}

		private GLDocTypeEnum GetDocTypeByBill<T>(T model)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSubclassOf(typeof(IVInvoiceModel)) || typeFromHandle == typeof(IVInvoiceModel))
			{
				IVInvoiceModel iVInvoiceModel = model as IVInvoiceModel;
				return (!(iVInvoiceModel.MType == "Invoice_Sale") && !(iVInvoiceModel.MType == "Invoice_Sale_Red")) ? GLDocTypeEnum.Bill : GLDocTypeEnum.Invoice;
			}
			if (typeFromHandle.IsSubclassOf(typeof(IVExpenseModel)) || typeFromHandle == typeof(IVExpenseModel))
			{
				return GLDocTypeEnum.Expense;
			}
			if (typeFromHandle.IsSubclassOf(typeof(IVReceiveModel)) || typeFromHandle == typeof(IVReceiveModel))
			{
				return GLDocTypeEnum.Receive;
			}
			if (typeFromHandle.IsSubclassOf(typeof(IVPaymentModel)) || typeFromHandle == typeof(IVPaymentModel))
			{
				return GLDocTypeEnum.Payment;
			}
			if (typeFromHandle.IsSubclassOf(typeof(IVTransferModel)) || typeFromHandle == typeof(IVTransferModel))
			{
				return GLDocTypeEnum.Transfer;
			}
			return GLDocTypeEnum.Invoice;
		}

		private GLVoucherModel Merge(MContext ctx, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList, int moduleID)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			List<GLDocVoucherModel> list2 = new List<GLDocVoucherModel>();
			GLVoucherModel newVoucher = new GLVoucherModel();
			List<GLDocVoucherModel> list3 = Clone(ctx, docVoucherList);
			newVoucher = Merge2OneVoucher(ctx, voucherList, list3, moduleID);
			list3.ForEach(delegate(GLDocVoucherModel x)
			{
				x.MergeStatus = 1;
				x.MVoucherID = newVoucher.MItemID;
				x.MItemID = ((!string.IsNullOrWhiteSpace(x.MItemID)) ? JieNor.Megi.Common.UUIDHelper.GetGuid() : x.MItemID);
				x.IsNew = true;
			});
			voucherList.ForEach(delegate(GLVoucherModel x)
			{
				x.IsDelete = true;
				x.MNumber = null;
				x.MStatus = -1;
				x.MTransferTypeID = -1;
			});
			docVoucherList?.ForEach(delegate(GLDocVoucherModel x)
			{
				x.MergeStatus = -1;
			});
			list.Add(newVoucher);
			list.AddRange(voucherList);
			voucherList = list;
			list2.AddRange(list3);
			list2.AddRange(docVoucherList);
			docVoucherList = list2;
			return newVoucher;
		}

		private GLVoucherModel Merge2OneVoucher(MContext ctx, List<GLVoucherModel> voucherList, List<GLDocVoucherModel> docVoucherList, int moduleID)
		{
			GLVoucherModel newVoucher = voucherList[0];
			for (int i = 1; i < voucherList.Count; i++)
			{
				newVoucher = MergeVoucher(ctx, newVoucher, voucherList[i], moduleID);
			}
			newVoucher.MItemID = JieNor.Megi.Common.UUIDHelper.GetGuid();
			newVoucher.IsNew = true;
			newVoucher.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
			{
				string newEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid();
				string[] oldEntryIDs = x.MEntryID.Split(',');
				docVoucherList?.ForEach(delegate(GLDocVoucherModel y)
				{
					if (oldEntryIDs.Contains(y.MCreditEntryID))
					{
						y.MCreditEntryID = newEntryID;
					}
					else if (oldEntryIDs.Contains(y.MDebitEntryID))
					{
						y.MDebitEntryID = newEntryID;
					}
					else if (oldEntryIDs.Contains(y.MTaxEntryID))
					{
						y.MTaxEntryID = newEntryID;
					}
				});
				x.MID = newVoucher.MItemID;
				x.MEntryID = newEntryID;
				x.IsNew = true;
			});
			return newVoucher;
		}

		private GLVoucherModel MergeVoucher(MContext ctx, GLVoucherModel dest, GLVoucherModel src, int moduleID)
		{
			GLVoucherModel gLVoucherModel = Clone(ctx, dest);
			gLVoucherModel.MVoucherEntrys = new List<GLVoucherEntryModel>();
			gLVoucherModel.IsNew = true;
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			list.AddRange(Clone(ctx, dest.MVoucherEntrys));
			list.AddRange(Clone(ctx, src.MVoucherEntrys));
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			BDVoucherSettingCategoryModel bDVoucherSettingCategoryModel = instance.VoucherSettingCategoryList.FirstOrDefault((BDVoucherSettingCategoryModel x) => x.MModuleID == moduleID);
			bool mStatus = bDVoucherSettingCategoryModel.MSettingList.FirstOrDefault((BDVoucherSettingModel x) => x.MColumnID == BDVoucherSettingColumnEnum.EntryMergeSetting && x.MTypeID == BDVoucherSettingEumn.AccountCheckTypeSame).MStatus;
			list = ((!bDVoucherSettingCategoryModel.MSettingList.FirstOrDefault((BDVoucherSettingModel x) => x.MColumnID == BDVoucherSettingColumnEnum.EntryMergeSetting && x.MTypeID == BDVoucherSettingEumn.AccountDescCheckTypeSame).MStatus) ? (from x in list
			orderby x.MAccountID
			orderby x.MCheckGroupValueID
			select x).ToList() : (from x in list
			orderby x.MAccountID
			orderby x.MExplanation
			orderby x.MCheckGroupValueID
			select x).ToList());
			GLVoucherEntryModel gLVoucherEntryModel = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				GLVoucherEntryModel gLVoucherEntryModel2 = NeedEntryMerge(ctx, gLVoucherEntryModel, list[i], moduleID);
				if (gLVoucherEntryModel2 != null)
				{
					list[i] = gLVoucherEntryModel2;
					gLVoucherEntryModel.IsDelete = true;
				}
				gLVoucherEntryModel = list[i];
			}
			gLVoucherModel.MVoucherEntrys = (from x in list
			where !x.IsDelete
			select x).ToList();
			gLVoucherModel.MVoucherEntrys = (from x in gLVoucherModel.MVoucherEntrys
			orderby x.MDebit descending
			select x).ToList();
			GLVoucherModel gLVoucherModel2 = gLVoucherModel;
			GLVoucherModel gLVoucherModel3 = gLVoucherModel;
			decimal num3 = gLVoucherModel2.MDebitTotal = (gLVoucherModel3.MCreditTotal = default(decimal));
			for (int j = 0; j < gLVoucherModel.MVoucherEntrys.Count; j++)
			{
				gLVoucherModel.MVoucherEntrys[j].MEntrySeq = j;
				GLVoucherModel gLVoucherModel4 = gLVoucherModel;
				gLVoucherModel4.MCreditTotal += gLVoucherModel.MVoucherEntrys[j].MCredit;
				GLVoucherModel gLVoucherModel5 = gLVoucherModel;
				gLVoucherModel5.MDebitTotal += gLVoucherModel.MVoucherEntrys[j].MDebit;
				gLVoucherModel.MVoucherEntrys[j].MExplanation = string.Join("·", gLVoucherModel.MVoucherEntrys[j].MExplanation.Split('·').Distinct());
			}
			return gLVoucherModel;
		}

		private GLVoucherEntryModel NeedEntryMerge(MContext ctx, GLVoucherEntryModel src, GLVoucherEntryModel dest, int moduleID)
		{
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			BDVoucherSettingCategoryModel bDVoucherSettingCategoryModel = instance.VoucherSettingCategoryList.FirstOrDefault((BDVoucherSettingCategoryModel x) => x.MModuleID == moduleID);
			bool mStatus = bDVoucherSettingCategoryModel.MSettingList.FirstOrDefault((BDVoucherSettingModel x) => x.MColumnID == BDVoucherSettingColumnEnum.EntryMergeSetting && x.MTypeID == BDVoucherSettingEumn.AccountCheckTypeSame).MStatus;
			bool mStatus2 = bDVoucherSettingCategoryModel.MSettingList.FirstOrDefault((BDVoucherSettingModel x) => x.MColumnID == BDVoucherSettingColumnEnum.EntryMergeSetting && x.MTypeID == BDVoucherSettingEumn.AccountDescCheckTypeSame).MStatus;
			if (((mStatus && src.MAccountID == dest.MAccountID && src.MCheckGroupValueID == dest.MCheckGroupValueID) || (mStatus2 && src.MAccountID == dest.MAccountID && src.MCheckGroupValueID == dest.MCheckGroupValueID && IsExplanationEqual(src.MExplanation, dest.MExplanation))) && src.MCurrencyID == dest.MCurrencyID && src.MDC == dest.MDC)
			{
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MExplanation = (mStatus2 ? dest.MExplanation : (((string.IsNullOrWhiteSpace(dest.MExplanation) || dest.MExplanation == src.MExplanation) ? "" : (dest.MExplanation + "·")) + src.MExplanation)),
					MAccountID = src.MAccountID,
					MCurrencyID = src.MCurrencyID,
					MExchangeRate = src.MExchangeRate,
					MCheckGroupValueID = src.MCheckGroupValueID,
					MCheckGroupValueModel = src.MCheckGroupValueModel,
					MDebit = src.MDebit + dest.MDebit,
					MCredit = src.MCredit + dest.MCredit,
					MEntryID = src.MEntryID + "," + dest.MEntryID,
					MAmount = src.MAmount + dest.MAmount,
					MAmountFor = src.MAmountFor + dest.MAmountFor,
					MDC = src.MDC,
					MIsDelete = false,
					IsNew = true
				};
				gLVoucherEntryModel.MExplanation = string.Join("·", gLVoucherEntryModel.MExplanation.Split('·').Distinct());
				return gLVoucherEntryModel;
			}
			return null;
		}

		private bool IsExplanationEqual(string src, string dest)
		{
			if (src == dest)
			{
				return true;
			}
			if (!string.IsNullOrWhiteSpace(src) || !string.IsNullOrWhiteSpace(dest))
			{
				string a = string.Join("", (from x in src.Split('·').Distinct()
				orderby x descending
				select x).ToList());
				string b = string.Join("", (from x in dest.Split('·').Distinct()
				orderby x descending
				select x).ToList());
				return a == b;
			}
			return true;
		}

		private List<GLVoucherEntryModel> GetVoucherEntryWithModified(List<GLVoucherModel> voucherList)
		{
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			for (int i = 0; i < voucherList.Count; i++)
			{
				list.AddRange((from x in voucherList[i].MVoucherEntrys
				where x.MModified
				select x).ToList());
			}
			return list;
		}

		private List<GLVoucherEntryModel> GetVoucherEntryList(List<GLVoucherModel> voucherList)
		{
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			for (int i = 0; i < voucherList.Count; i++)
			{
				list.AddRange(voucherList[i].MVoucherEntrys);
			}
			return list;
		}

		private List<CommandInfo> GetInsertOrUpdateVoucherCommandList(MContext ctx, List<GLVoucherModel> voucherList, string customizePrefix)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (voucherList == null || !voucherList.Any())
			{
				return list;
			}
			List<GLVoucherModel> list2 = (from x in voucherList
			where x.IsNew
			select x).ToList();
			if (list2.Any())
			{
				voucherList.ForEach(delegate(GLVoucherModel x)
				{
					GLVoucherRepository.ProcessVoucher(ctx, x);
				});
				List<CommandInfo> batchInsertOrUpdateCmds = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list2, null, "voucher_" + customizePrefix);
				list.AddRange(batchInsertOrUpdateCmds);
				List<GLVoucherEntryModel> voucherEntryList = GetVoucherEntryList(list2);
				List<CommandInfo> batchInsertOrUpdateCmds2 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, voucherEntryList, null, "voucherEntry_" + customizePrefix);
				list.AddRange(batchInsertOrUpdateCmds2);
			}
			List<GLVoucherModel> list3 = (from x in voucherList
			where !x.IsNew
			select x).ToList();
			if (list3.Any())
			{
				List<CommandInfo> batchInsertOrUpdateCmds3 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list3, null, "voucher_" + customizePrefix);
				List<GLVoucherEntryModel> voucherEntryWithModified = GetVoucherEntryWithModified(list3);
				List<CommandInfo> batchInsertOrUpdateCmds4 = ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, voucherEntryWithModified, null, "voucherEntry_" + customizePrefix);
				list.AddRange(batchInsertOrUpdateCmds3);
				list.AddRange(batchInsertOrUpdateCmds4);
			}
			return list;
		}

		public bool GenerateVoucherByPurchase(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVInvoiceModel model = obj as IVInvoiceModel;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			bool flag = model.MTaxID == "No_Tax";
			bool flag2 = model.MTaxID == "Tax_Inclusive";
			bool flag3 = model.MTaxID == "Tax_Exclusive";
			decimal mExchangeRate = model.MExchangeRate;
			BDContactsModel bDContactsModel = string.IsNullOrWhiteSpace(model.MContactID) ? null : instance.ContactList.FirstOrDefault((BDContactsModel x) => x.MItemID == model.MContactID);
			string contact = (bDContactsModel != null) ? bDContactsModel.MName : "";
			string contactCurrentAcctCode = (bDContactsModel != null) ? bDContactsModel.MCCurrentAccountCode : "";
			BDAccountModel bDAccountModel = string.IsNullOrWhiteSpace(contactCurrentAcctCode) ? new BDAccountModel() : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == contactCurrentAcctCode);
			string[] itemIDList = (from x in model.InvoiceEntry
			select x.MItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToArray();
			List<BDItemModel> source = (itemIDList.Count() == 0) ? new List<BDItemModel>() : (from x in instance.MerItemList
			where itemIDList.Contains(x.MItemID)
			select x).ToList();
			BDAccountModel bDAccountModel2 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.Contains("22210101")) ?? new BDAccountModel();
			for (int i = 0; i < model.InvoiceEntry.Count; i++)
			{
				IVInvoiceEntryModel entry = model.InvoiceEntry[i];
				GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, entry.MEntryID, model.MID, 1);
				DateTime mBizDate = model.MBizDate;
				int year = mBizDate.Year;
				mBizDate = model.MBizDate;
				GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, model.MBizDate);
				BDItemModel bDItemModel = source.FirstOrDefault((BDItemModel x) => x.MItemID == entry.MItemID) ?? new BDItemModel();
				decimal num = flag2 ? (entry.MAmount - entry.MTaxAmt) : entry.MAmount;
				decimal mAmountFor = flag2 ? (entry.MAmountFor - entry.MTaxAmtFor) : entry.MAmountFor;
				decimal mTaxAmt = entry.MTaxAmt;
				decimal mTaxAmtFor = entry.MTaxAmtFor;
				decimal num2 = flag3 ? (entry.MAmount + entry.MTaxAmt) : entry.MAmount;
				decimal mAmountFor2 = flag3 ? (entry.MAmountFor + entry.MTaxAmtFor) : entry.MAmountFor;
				BDAccountModel bDAccountModel3 = new BDAccountModel();
				string costAccountCode = bDItemModel.MIsExpenseItem ? bDItemModel.MCostAccountCode : bDItemModel.MInventoryAccountCode;
				if (!string.IsNullOrWhiteSpace(entry.MDebitAccount))
				{
					bDAccountModel3.MItemID = entry.MDebitAccount;
				}
				else if (!string.IsNullOrWhiteSpace(costAccountCode))
				{
					bDAccountModel3 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode == costAccountCode);
				}
				GLCheckGroupValueModel checkGroupValueModel = instance.GetCheckGroupValueModel(new GLCheckGroupValueModel
				{
					MContactID = model.MContactID,
					MEmployeeID = null,
					MMerItemID = entry.MItemID,
					MExpItemID = null,
					MPaItemID = null,
					MTrackItem1 = entry.MTrackItem1,
					MTrackItem2 = entry.MTrackItem2,
					MTrackItem3 = entry.MTrackItem3,
					MTrackItem4 = entry.MTrackItem4,
					MTrackItem5 = entry.MTrackItem5
				});
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MID = tempVoucherModel.MItemID,
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 1, 1, model.MNumber, contact, string.Empty, model.MReference, bDItemModel.MNumber, string.Empty, entry.MDesc),
					MAccountID = bDAccountModel3.MItemID,
					MAmount = num,
					MAmountFor = mAmountFor,
					MDebit = num,
					MCredit = decimal.Zero,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = 0,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
				if (entry.MTaxAmtFor != decimal.Zero)
				{
					REGTaxRateModel tax = (!string.IsNullOrWhiteSpace(entry.MTaxID)) ? instance.TaxRateList.FirstOrDefault((REGTaxRateModel x) => x.MItemID == entry.MTaxID) : new REGTaxRateModel();
					BDAccountModel bDAccountModel4 = new BDAccountModel();
					if (!string.IsNullOrWhiteSpace(entry.MTaxAccount))
					{
						bDAccountModel4.MItemID = entry.MTaxAccount;
					}
					else
					{
						bDAccountModel4 = ((tax == null || string.IsNullOrWhiteSpace(tax.MPurchaseAccountCode)) ? bDAccountModel2 : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == tax.MPurchaseAccountCode));
					}
					GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
					{
						MOrgID = ctx.MOrgID,
						MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
						MID = tempVoucherModel.MItemID,
						MExplanation = utility.GetVoucherEntryExplanation(ctx, 1, 1, model.MNumber, contact, string.Empty, model.MReference, bDItemModel.MNumber, string.Empty, entry.MDesc),
						MAccountID = bDAccountModel4.MItemID,
						MAmount = mTaxAmt,
						MAmountFor = mTaxAmtFor,
						MDebit = mTaxAmt,
						MCredit = decimal.Zero,
						MCurrencyID = model.MCyID,
						MExchangeRate = mExchangeRate,
						MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
						MCheckGroupValueID = checkGroupValueModel.MItemID,
						IsNew = true,
						MEntrySeq = 1,
						MDC = 1
					};
					tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
					tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
				}
				BDAccountModel bDAccountModel5 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MCreditAccount))
				{
					bDAccountModel5.MItemID = entry.MCreditAccount;
				}
				else
				{
					bDAccountModel5 = bDAccountModel;
				}
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MID = tempVoucherModel.MItemID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 1, -1, model.MNumber, contact, string.Empty, model.MReference, bDItemModel.MNumber, string.Empty, entry.MDesc),
					MAccountID = bDAccountModel5.MItemID,
					MAmount = num2,
					MAmountFor = mAmountFor2,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					MDebit = decimal.Zero,
					MCredit = num2,
					IsNew = true,
					MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
					MDC = -1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
				tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
				tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
				voucherList.Add(tempVoucherModel);
				docVoucherList.Add(tempDocVoucherModel);
			}
			return model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
		}

		public bool GenerateVoucherByPayment(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVPaymentModel model = obj as IVPaymentModel;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			decimal mExchangeRate = model.MExchangeRate;
			bool flag = model.MTaxID == "No_Tax";
			bool flag2 = model.MTaxID == "Tax_Inclusive";
			bool flag3 = model.MTaxID == "Tax_Exclusive";
			bool flag4 = model.MSource == 101 || (model.MSource == 103 && model.MCreateFrom == 204);
			bool flag5 = model.MContactType == "Employees";
			BDContactsModel contact = (flag5 && !string.IsNullOrEmpty(model.MContactID)) ? null : instance.ContactList.FirstOrDefault((BDContactsModel x) => x.MItemID == model.MContactID);
			BDAccountModel bDAccountModel = (flag5 || contact == null || string.IsNullOrWhiteSpace(contact.MCCurrentAccountCode)) ? new BDAccountModel() : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == contact.MCCurrentAccountCode);
			BDEmployeesModel employee = (flag5 && !string.IsNullOrEmpty(model.MContactID)) ? instance.EmployeeList.FirstOrDefault((BDEmployeesModel x) => x.MItemID == model.MContactID) : null;
			BDAccountModel bDAccountModel2 = (flag5 && employee != null && !string.IsNullOrWhiteSpace(employee.MCurrentAccountCode)) ? accountList.FirstOrDefault((BDAccountModel x) => x.MCode == employee.MCurrentAccountCode) : new BDAccountModel();
			string text = (employee != null) ? employee.MFullName : "";
			text = ((string.IsNullOrWhiteSpace(text) && contact != null) ? contact.MName : text);
			BDAccountModel bDAccountModel3 = (model.MType == "Pay_BankFee") ? accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf("660303") == 0) : new BDAccountModel();
			BDAccountModel bDAccountModel4 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.Contains("22210101"));
			for (int i = 0; i < model.PaymentEntry.Count; i++)
			{
				IVPaymentEntryModel entry = model.PaymentEntry[i];
				GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, entry.MEntryID, model.MID, 4);
				DateTime mBizDate = model.MBizDate;
				int year = mBizDate.Year;
				mBizDate = model.MBizDate;
				GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, model.MBizDate);
				decimal num = flag4 ? entry.MTaxAmount : (flag2 ? (entry.MAmount - entry.MTaxAmt) : entry.MAmount);
				decimal mAmountFor = flag4 ? entry.MTaxAmountFor : (flag2 ? (entry.MAmountFor - entry.MTaxAmtFor) : entry.MAmountFor);
				decimal num2 = flag4 ? decimal.Zero : entry.MTaxAmt;
				decimal num3 = flag4 ? decimal.Zero : entry.MTaxAmtFor;
				decimal num4 = (flag3 | flag4) ? entry.MTaxAmount : entry.MAmount;
				decimal mAmountFor2 = (flag3 | flag4) ? entry.MTaxAmountFor : entry.MAmountFor;
				BDItemModel bDItemModel = instance.MerItemNumberList.FirstOrDefault((BDItemModel x) => x.MItemID == entry.MItemID) ?? new BDItemModel();
				BDExpenseItemModel bDExpenseItemModel = instance.ExpenseItemNameList.FirstOrDefault((BDExpenseItemModel x) => x.MItemID == entry.MItemID) ?? new BDExpenseItemModel();
				BDAccountModel bDAccountModel5 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MDebitAccount))
				{
					bDAccountModel5.MItemID = entry.MDebitAccount;
				}
				else if (!string.IsNullOrWhiteSpace(entry.MAcctID))
				{
					bDAccountModel5.MItemID = entry.MAcctID;
				}
				if (string.IsNullOrWhiteSpace(bDAccountModel5.MItemID))
				{
					bDAccountModel5 = ((!(model.MType == "Pay_BankFee")) ? ((!(model.MType == "Pay_Adjustment")) ? (flag5 ? bDAccountModel2 : bDAccountModel) : new BDAccountModel()) : bDAccountModel3);
					bDAccountModel5 = (bDAccountModel5 ?? new BDAccountModel());
				}
				GLCheckGroupValueModel checkGroupValueModel = instance.GetCheckGroupValueModel(new GLCheckGroupValueModel
				{
					MContactID = ((model.MContactType != "Employees") ? model.MContactID : null),
					MEmployeeID = ((model.MContactType == "Employees") ? model.MContactID : null),
					MMerItemID = ((model.MContactType != "Employees") ? entry.MItemID : null),
					MExpItemID = ((model.MContactType == "Employees") ? entry.MItemID : null),
					MPaItemID = null,
					MTrackItem1 = entry.MTrackItem1,
					MTrackItem2 = entry.MTrackItem2,
					MTrackItem3 = entry.MTrackItem3,
					MTrackItem4 = entry.MTrackItem4,
					MTrackItem5 = entry.MTrackItem5
				});
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 4, 1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
					MAccountID = bDAccountModel5.MItemID,
					MAmount = num,
					MAmountFor = mAmountFor,
					MDebit = num,
					MCredit = decimal.Zero,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = 0,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
				if (num3 != decimal.Zero)
				{
					REGTaxRateModel tax = (!string.IsNullOrWhiteSpace(entry.MTaxID)) ? instance.TaxRateList.FirstOrDefault((REGTaxRateModel x) => x.MItemID == entry.MTaxID) : new REGTaxRateModel();
					BDAccountModel bDAccountModel6 = new BDAccountModel();
					bDAccountModel6 = ((tax == null || string.IsNullOrWhiteSpace(tax.MPurchaseAccountCode)) ? bDAccountModel4 : (accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf(tax.MPurchaseAccountCode) == 0) ?? bDAccountModel6));
					GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
					{
						MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
						MID = tempVoucherModel.MItemID,
						MOrgID = ctx.MOrgID,
						MExplanation = utility.GetVoucherEntryExplanation(ctx, 4, 1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
						MAccountID = bDAccountModel6.MItemID,
						MAmount = num2,
						MAmountFor = num3,
						MDebit = num2,
						MCredit = decimal.Zero,
						MCurrencyID = model.MCyID,
						MExchangeRate = mExchangeRate,
						MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
						MCheckGroupValueID = checkGroupValueModel.MItemID,
						IsNew = true,
						MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
						MDC = 1
					};
					tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
					tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
				}
				BDAccountModel bDAccountModel7 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(model.MBankID))
				{
					bDAccountModel7.MItemID = model.MBankID;
				}
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 4, -1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
					MAccountID = bDAccountModel7.MItemID,
					MAmount = num4,
					MAmountFor = mAmountFor2,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MDebit = decimal.Zero,
					MCredit = num4,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
					MDC = -1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
				tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
				tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
				voucherList.Add(tempVoucherModel);
				docVoucherList.Add(tempDocVoucherModel);
			}
			return true;
		}

		public bool GenerateVoucherByReceive(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVReceiveModel model = obj as IVReceiveModel;
			decimal mExchangeRate = model.MExchangeRate;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			bool flag = model.MTaxID == "No_Tax";
			bool flag2 = model.MTaxID == "Tax_Inclusive";
			bool flag3 = model.MTaxID == "Tax_Exclusive";
			bool flag4 = model.MSource == 101 || (model.MSource == 103 && model.MCreateFrom == 204);
			bool flag5 = model.MContactType == "Employees";
			BDContactsModel contact = (flag5 && !string.IsNullOrEmpty(model.MContactID)) ? null : instance.ContactList.FirstOrDefault((BDContactsModel x) => x.MItemID == model.MContactID);
			BDAccountModel bDAccountModel = (flag5 || contact == null || string.IsNullOrWhiteSpace(contact.MCCurrentAccountCode)) ? new BDAccountModel() : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == contact.MCCurrentAccountCode);
			BDEmployeesModel employee = (flag5 && !string.IsNullOrEmpty(model.MContactID)) ? instance.EmployeeList.FirstOrDefault((BDEmployeesModel x) => x.MItemID == model.MContactID) : null;
			BDAccountModel bDAccountModel2 = (flag5 && employee != null && !string.IsNullOrWhiteSpace(employee.MCurrentAccountCode)) ? accountList.FirstOrDefault((BDAccountModel x) => x.MCode == employee.MCurrentAccountCode) : new BDAccountModel();
			string text = (employee != null) ? employee.MFullName : "";
			text = ((string.IsNullOrWhiteSpace(text) && contact != null) ? contact.MName : text);
			BDAccountModel bDAccountModel3 = (model.MType == "Receive_BankInterest") ? accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf("660301") == 0) : new BDAccountModel();
			BDAccountModel bDAccountModel4 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf("22210105") == 0) ?? new BDAccountModel();
			for (int i = 0; i < model.ReceiveEntry.Count; i++)
			{
				IVReceiveEntryModel entry = model.ReceiveEntry[i];
				GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, entry.MEntryID, model.MID, 3);
				DateTime mBizDate = model.MBizDate;
				int year = mBizDate.Year;
				mBizDate = model.MBizDate;
				GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, model.MBizDate);
				BDAccountModel bDAccountModel5 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MCreditAccount))
				{
					bDAccountModel5.MItemID = entry.MCreditAccount;
				}
				else if (!string.IsNullOrWhiteSpace(entry.MAcctID))
				{
					bDAccountModel5.MItemID = entry.MAcctID;
				}
				else
				{
					bDAccountModel5 = (flag5 ? bDAccountModel2 : bDAccountModel);
				}
				BDItemModel bDItemModel = instance.MerItemNumberList.FirstOrDefault((BDItemModel x) => x.MItemID == entry.MItemID) ?? new BDItemModel();
				BDExpenseItemModel bDExpenseItemModel = instance.ExpenseItemNameList.FirstOrDefault((BDExpenseItemModel x) => x.MItemID == entry.MItemID) ?? new BDExpenseItemModel();
				BDAccountModel bDAccountModel6 = new BDAccountModel();
				BDAccountModel bDAccountModel7 = new BDAccountModel();
				int num = 1;
				if (model.MType == "Receive_BankInterest")
				{
					bDAccountModel6 = bDAccountModel3;
					num = -1;
					bDAccountModel7.MItemID = model.MBankID;
				}
				else if (model.MType == "Receive_Adjustment")
				{
					bDAccountModel6.MItemID = model.MBankID;
					num = 1;
				}
				else
				{
					bDAccountModel6.MItemID = model.MBankID;
					bDAccountModel7 = bDAccountModel5;
					num = 1;
				}
				decimal d = flag3 ? (entry.MAmount + entry.MTaxAmt) : entry.MAmount;
				decimal d2 = flag3 ? (entry.MAmountFor + entry.MTaxAmtFor) : entry.MAmountFor;
				decimal num2 = flag4 ? decimal.Zero : entry.MTaxAmt;
				decimal num3 = flag4 ? decimal.Zero : entry.MTaxAmtFor;
				decimal d3 = flag4 ? entry.MTaxAmount : (flag2 ? (entry.MAmount - entry.MTaxAmt) : entry.MAmount);
				decimal d4 = flag4 ? entry.MTaxAmountFor : (flag2 ? (entry.MAmountFor - entry.MTaxAmtFor) : entry.MAmountFor);
				GLCheckGroupValueModel checkGroupValueModel = instance.GetCheckGroupValueModel(new GLCheckGroupValueModel
				{
					MContactID = ((!flag5) ? model.MContactID : null),
					MEmployeeID = (flag5 ? model.MContactID : null),
					MMerItemID = ((!flag5) ? entry.MItemID : null),
					MExpItemID = (flag5 ? entry.MItemID : null),
					MPaItemID = null,
					MTrackItem1 = entry.MTrackItem1,
					MTrackItem2 = entry.MTrackItem2,
					MTrackItem3 = entry.MTrackItem3,
					MTrackItem4 = entry.MTrackItem4,
					MTrackItem5 = entry.MTrackItem5
				});
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 3, 1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
					MAccountID = bDAccountModel6.MItemID,
					MAmount = d * (decimal)num,
					MAmountFor = d2 * (decimal)num,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MDebit = d * (decimal)num,
					MCredit = decimal.Zero,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
				if (num3 != decimal.Zero)
				{
					REGTaxRateModel tax = (!string.IsNullOrWhiteSpace(entry.MTaxID)) ? instance.TaxRateTaxAccountList.FirstOrDefault((REGTaxRateModel x) => x.MItemID == entry.MTaxID) : new REGTaxRateModel();
					BDAccountModel bDAccountModel8 = new BDAccountModel();
					bDAccountModel8 = ((tax == null || string.IsNullOrWhiteSpace(tax.MSaleTaxAccountCode)) ? bDAccountModel4 : (accountList.FirstOrDefault((BDAccountModel x) => x.MCode == tax.MSaleTaxAccountCode) ?? bDAccountModel8));
					GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
					{
						MOrgID = ctx.MOrgID,
						MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
						MID = tempVoucherModel.MItemID,
						MExplanation = utility.GetVoucherEntryExplanation(ctx, 3, -1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
						MAccountID = bDAccountModel8.MItemID,
						MAmount = num2,
						MAmountFor = num3,
						MCurrencyID = model.MCyID,
						MExchangeRate = mExchangeRate,
						MDebit = decimal.Zero,
						MCredit = num2,
						MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
						MCheckGroupValueID = checkGroupValueModel.MItemID,
						IsNew = true,
						MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
						MDC = -1
					};
					tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
					tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
				}
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 3, -1, string.Empty, flag5 ? string.Empty : text, flag5 ? text : string.Empty, model.MReference, flag5 ? string.Empty : bDItemModel.MNumber, flag5 ? bDExpenseItemModel.MName : string.Empty, entry.MDesc),
					MAccountID = bDAccountModel7.MItemID,
					MAmount = d3 * (decimal)num,
					MAmountFor = d4 * (decimal)num,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					MDebit = decimal.Zero,
					MCredit = d3 * (decimal)num,
					IsNew = true,
					MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
					MDC = -1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
				tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
				tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
				voucherList.Add(tempVoucherModel);
				docVoucherList.Add(tempDocVoucherModel);
			}
			return true;
		}

		private string GetReceivePaymentQuerySql(MContext ctx, GLDocVoucherFilterModel filter, bool calCount = false)
		{
			string empty = string.Empty;
			if (calCount)
			{
				empty = "select count(*) total from (" + GetReceivePaymentBaseSql(filter, calCount, false) + ") as t1";
			}
			else
			{
				empty = "select distinct\r\n                    a.MNumber,\n                    a.MBizDate,\n                    a.MModifyDate,\n                    a.MTotalAmt,\n                    a.MTaxTotalAmt,\n                    a.MTaxAmt as MTax,\n                    a.MReference,\n                    a.MContactType,\n                    3 as MDocStatus,\n                    a.MOrgID,\n                    a.MType,\n                    CONVERT( AES_DECRYPT(c.MName, '{0}') USING UTF8) AS MContactName ,\n                    x.MFirstName,\n                    x.MLastName,\n                    b.MEntryID,\n                    b.MID as MDocID,\n                    b.MID,\n                    b.MItemID,\n                    b.MTaxID,\n                    b.MAmount,\n                    b.MTaxAmt,\n                    b.MDesc,\n                    b.MSeq,\n                    d.MItemID as MDocVoucherID,\n                    d.MDebitEntryID,\n                    d.MCreditEntryID,\n                    d.MTaxEntryID,\n                    d.MDOCTYPE,\n                    d.MergeStatus,\n                    k.MNumber as MVoucherNumber,\n                    k.MYear,\n                    k.MPeriod,\n                    k.MItemID as MVoucherID,\n                    k.MStatus as MVoucherStatus,\n                    f.MFullName as MDebitAccountName,\n                    h.MFullName as MTaxAccountName,\n                    j.MFullName as MCreditAccountName,\n                    ff.MItemID as MDebitAccountID,\n                    hh.MItemID as MTaxAccountID,\n                    jj.MItemID as MCreditAccountID,\n                    f.MFullName as MDebitAccountFullName,\n                    h.MFullName as MTaxAccountFullName,\n                    j.MFullName as MCreditAccountFullName,\n                    TRIM(TRAILING ':' FROM CONCAT(CONCAT(IFNULL(l.MNumber, ''),\n                                    IFNULL(z.MName, '')),\n                                ':',\n                            CONCAT(IFNULL(n.MDesc, ''), IFNULL(z.MDesc, '')))) AS MItemName " + GetReceivePaymentBaseSql(filter, false, false);
				empty = empty + " and b.MEntryID in(" + GetMEntryIDBySql(ctx, filter, GetReceivePaymentBaseSql(filter, false, true)) + ")";
				empty += " order by a.MBizDate desc, a.MID desc, b.MSeq asc ";
			}
			return string.Format(empty, "JieNor-001");
		}

		private string GetReceivePaymentBaseSql(GLDocVoucherFilterModel filter, bool calCount = false, bool getEntryID = false)
		{
			string text = "\n                from t_iv_{0} a\n                inner join t_iv_{0}entry b\n                on a.MID = b.MID\n                and b.MOrgID = a.MOrgID\n                and b.MIsDelete = 0 \n        \n                left join t_bd_contacts_l c\n                on a.MContactID = c.MParentID\n                and c.MLocaleID = @MLocaleID\n                and c.MOrgID = a.MOrgID\n                and c.MIsDelete = 0 \n                \n                left join t_bd_employees_l x\n                on a.MContactID = x.MParentID\n                and x.MLocaleID = @MLocaleID  \n                and x.MOrgID = a.MOrgID   \n                and x.MIsDelete = 0       \n\n                left join t_gl_doc_voucher d\n                on d.MDOCID = a.MID\n                and d.MOrgID = a.MOrgID\n                and d.MEntryID = b.MEntryID\n                and d.MDOCTYPE = {1}\n                and d.MergeStatus != -1\n                and d.MIsDelete = 0 \n\n                left join t_gl_voucher k\n                on k.MItemID = d.MVoucherID\n                and k.MOrgID = a.MOrgID\n                and k.MIsDelete = 0 \n\n                left join t_gl_voucherentry e\n                on d.MDebitEntryID = e.MEntryID\n                and e.MOrgID = a.MOrgID\n                and e.MIsDelete = 0 \n\n                left join t_bd_account ff\r\n                on ff.MItemID = e.MAccountID\n                and ff.MOrgID = a.MOrgID\n                and ff.MIsDelete = 0 \r\n\r\n                left join t_bd_account_l f\n                on f.MParentID = e.MAccountID\n                and f.MLocaleID = @MLocaleID\n                and f.MOrgID = a.MOrgID\n                and f.MIsDelete = 0 \n\n                left join t_gl_voucherentry g\n                on d.MTaxEntryID = g.MEntryID\n                and g.MOrgID = a.MOrgID\n                and g.MIsDelete = 0 \n\n                left join t_bd_account hh\r\n                on hh.MItemID = g.MAccountID\n                and hh.MOrgID = a.MOrgID\n                and hh.MIsDelete = 0 \r\n\r\n                left join t_bd_account_l h\n                on h.MParentID = g.MAccountID\n                and h.MLocaleID = @MLocaleID\n                and h.MOrgID = a.MOrgID\n                and h.MIsDelete = 0 \n\n                left join t_gl_voucherentry i\n                on d.MCreditEntryID = i.MEntryID\n                and i.MOrgID = a.MOrgID\n                and i.MIsDelete = 0 \n\n                left join t_bd_account jj\r\n                on jj.MItemID = i.MAccountID\n                and jj.MOrgID = a.MOrgID\n                and jj.MIsDelete = 0 \r\n\r\n                left join t_bd_account_l j\n                on j.MParentID = i.MAccountID\n                and j.MLocaleID = @MLocaleID\n                and j.MOrgID = a.MOrgID\n                and j.MIsDelete = 0 \n\n                left join t_bd_item l\n                on l.MItemID = b.MItemID\n                and l.MOrgID = a.MOrgID\n                and l.MIsDelete = 0 \n\n                left join t_bd_item_l n\n                on n.MParentID = l.MItemID\n                and n.MLocaleID = @MLocaleID\n                and n.MOrgID = a.MOrgID\n                and n.MIsDelete = 0 \n\n                left join t_bd_expenseitem y\n                on y.MItemID = b.MItemID\n                and y.MOrgID = a.MOrgID\n                and y.MIsDelete = 0 \n\n                left join t_bd_expenseitem_l z\n                on z.MParentID = y.MItemID\n                and z.MLocaleID = @MLocaleID\n                and z.MOrgID = a.MOrgID\n                and z.MIsDelete = 0 \n\n                where a.MOrgID = @MOrgID and a.MIsDelete = 0\n                ";
			if (filter.MDocIDs != null && filter.MDocIDs.Count > 0)
			{
				text = text + " and a.MID in ('" + string.Join("','", filter.MDocIDs) + "')";
			}
			else if (filter.MEntryIDs != null && filter.MEntryIDs.Count > 0)
			{
				text = text + " and b.MEntryID in('" + string.Join("','", filter.MEntryIDs.ToArray()) + "')";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(filter.Keyword))
				{
					text += "  AND (\r\n                    a.MReference like concat('%',@Keyword,'%')\r\n                    OR b.MDesc like concat('%',@Keyword,'%')\r\n                    OR CONVERT( AES_DECRYPT(c.MName, '{2}') USING UTF8) like concat('%',@Keyword,'%')\n                    OR a.MNumber like  concat('%',@Keyword,'%')\r\n                    OR k.MNumber like  concat('%',@Keyword,'%')\r\n                    OR f.MName like  concat('%',@Keyword,'%')\r\n                    OR f.MFullName like  concat('%',@Keyword,'%')\r\n                    OR h.MName like  concat('%',@Keyword,'%')\r\n                    OR h.MFullName like  concat('%',@Keyword,'%')\r\n                    OR j.MName like  concat('%',@Keyword,'%')\r\n                    OR j.MFullName like  concat('%',@Keyword,'%')\r\n                    OR l.MNumber like concat('%',@Keyword,'%')\r\n                    OR n.MDesc like concat ('%',@Keyword,'%')\r\n                    OR z.MName like concat('%',@Keyword,'%')\r\n                    OR z.MDesc like concat ('%',@Keyword,'%')\r\n                    OR x.MFirstName like concat('%',@Keyword,'%')\r\n                    OR x.MLastName like concat('%',@Keyword,'%') ";
					if (filter.DecimalKeyword.HasValue)
					{
						text += " OR a.MTotalAmt = @DecimalKeyword  \r\n                                  OR a.MTaxTotalAmt = @DecimalKeyword \r\n                                  OR a.MTaxAmt = @DecimalKeyword\r\n                                  OR b.MAmount = @DecimalKeyword \r\n                                  OR b.MTaxAmt = @DecimalKeyword ";
					}
					if (filter.DatetimeKeyword.HasValue)
					{
						text += " OR a.MBizDate = @DatetimeKeyword ";
					}
					text += ")";
				}
				if (filter.Year * filter.Period > 0)
				{
					text += "\n                            and a.MBizDate >= @MStartDate \n                            and a.MBizDate <= @MEndDate\r\n                            and a.MBizDate >= @OrgBeginDate\r\n                            and a.MBizDate >= @GLBeginDate";
				}
				if (!string.IsNullOrWhiteSpace(filter.Number))
				{
					text += " AND (a.MNumber  like concat('%', @Number ,'%') Or a.MTotalAmt = @Number) ";
				}
				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					text = ((!(filter.Status == "0")) ? (text + " AND (length(ifnull(k.MNumber,'')) = 0)") : (text + " AND length(ifnull(k.MNumber,'')) > 0  and (k.MStatus  = 0 or k.MStatus = 1)"));
				}
			}
			text = string.Format(text, (filter.Type == 3) ? "receive" : "payment", filter.Type, "JieNor-001");
			if (calCount)
			{
				text = " select distinct a.MID " + text;
			}
			else if (getEntryID)
			{
				text = " select distinct a.MID, b.MEntryID " + text + " order by a.MBizDate, a.MID desc, b.MSeq asc  ";
			}
			return text;
		}

		public bool GenerateVoucherByTransfer(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVTransferModel iVTransferModel = obj as IVTransferModel;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			IVTransferModel iVTransferModel2 = iVTransferModel;
			GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, iVTransferModel2.MID, iVTransferModel.MID, 5);
			DateTime mBizDate = iVTransferModel.MBizDate;
			int year = mBizDate.Year;
			mBizDate = iVTransferModel.MBizDate;
			GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, iVTransferModel.MBizDate);
			BDAccountModel bDAccountModel = new BDAccountModel
			{
				MItemID = iVTransferModel2.MToAcctID
			};
			BDAccountModel bDAccountModel2 = new BDAccountModel
			{
				MItemID = iVTransferModel2.MFromAcctID
			};
			GLCheckGroupValueModel checkGroupValueModel = instance.GetCheckGroupValueModel(new GLCheckGroupValueModel());
			GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
			{
				MOrgID = ctx.MOrgID,
				MID = tempVoucherModel.MItemID,
				MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
				MExplanation = iVTransferModel.MReference,
				MAccountID = bDAccountModel.MItemID,
				MAmount = iVTransferModel2.MToTotalAmt,
				MAmountFor = iVTransferModel2.MToTotalAmtFor,
				MCurrencyID = iVTransferModel2.MToCyID,
				MExchangeRate = ((iVTransferModel2.MToCyID == ctx.MBasCurrencyID) ? 1.0m : iVTransferModel2.MExchangeRate),
				MDebit = iVTransferModel2.MToTotalAmt,
				MCredit = decimal.Zero,
				MCheckGroupValueID = checkGroupValueModel.MItemID,
				MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
				IsNew = true,
				MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
				MDC = 1
			};
			tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
			tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
			if (iVTransferModel.MExchangeLoss != decimal.Zero)
			{
				BDAccountModel bDAccountModel3 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.IndexOf("660302") == 0) ?? new BDAccountModel();
				GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
				{
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MOrgID = ctx.MOrgID,
					MExplanation = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "ExchangeLoss", "汇兑损失"),
					MAccountID = bDAccountModel3.MItemID,
					MAmount = iVTransferModel2.MExchangeLoss,
					MAmountFor = iVTransferModel2.MExchangeLoss,
					MCurrencyID = ctx.MBasCurrencyID,
					MExchangeRate = 1.0m,
					MDebit = iVTransferModel2.MExchangeLoss,
					MCredit = decimal.Zero,
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					IsNew = true,
					MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
				tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
			}
			GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
			{
				MOrgID = ctx.MOrgID,
				MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
				MID = tempVoucherModel.MItemID,
				MExplanation = iVTransferModel.MReference,
				MAccountID = bDAccountModel2.MItemID,
				MAmount = iVTransferModel2.MFromTotalAmt,
				MAmountFor = iVTransferModel2.MFromTotalAmtFor,
				MCurrencyID = iVTransferModel2.MFromCyID,
				MExchangeRate = ((iVTransferModel2.MFromCyID == ctx.MBasCurrencyID) ? 1.0m : iVTransferModel.MBeginExchangeRate),
				MDebit = decimal.Zero,
				MCredit = iVTransferModel2.MFromTotalAmt,
				MCheckGroupValueID = checkGroupValueModel.MItemID,
				MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
				IsNew = true,
				MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
				MDC = -1
			};
			tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
			tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
			tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
			voucherList.Add(tempVoucherModel);
			docVoucherList.Add(tempDocVoucherModel);
			return true;
		}

		private string GetTransferQuerySql(GLDocVoucherFilterModel filter, bool calCount = false)
		{
			string str = $"select distinct\n\t                a.MBizDate,\n                    a.MModifyDate,\n\t                a.MID as MEntryID,\n\t                a.MID as MDocID,\n                    a.MID,\n\t                a.MFromTotalAmt as MAmount,\n                    0 as MTax,\n\t                a.MReference as MDesc,\n                    3 as MDocStatus,\n                    a.MOrgID,\n                    '' as MType,\n                    k1.MItemID as MFromAccountID,    \n                    k2.MItemID as MToAccountID,\n                    b.MName as MFromAccountName,\n                    c.MName as MToAccountName,\n\t                d.MItemID as MDocVoucherID,\n                    d.MDebitEntryID,\n                    d.MCreditEntryID,\n                    d.MTaxEntryID,\n\t                d.MDOCTYPE,\n                    d.MergeStatus,\n\t                k.MNumber as MVoucherNumber,\n                    k.MStatus as MVoucherStatus,\n                    k.MYear,\n                    k.MPeriod,\n\t                k.MItemID as MVoucherID,\n\t                f.MFullName as MDebitAccountName,\n\t                h.MFullName as MTaxAccountName,\n                    j.MFullName as MCreditAccountName,\n                    k1.MBankAccountType as MFromAccountType,\n                    k2.MBankAccountType as MToAccountType,\n                    ff.MItemID as MDebitAccountID,\n                    gg.MItemID as MTaxAccountID,\n                    hh.MItemID as MCreditAccountID,\n                    concat(ff.MNumber ,' ', f.MName) as MDebitAccountFullName,\n                    concat(gg.MNumber ,' ', h.MName) as MTaxAccountFullName,\n                    concat(hh.MNumber ,' ', j.MName) as MCreditAccountFullName\n\n\t                from t_iv_transfer a\n\n\t                inner join t_bd_account_l b\n\t                on a.MFromAcctID = b.MParentID\n                    AND b.MLocaleID = @MLocaleID\n                    and b.MOrgID = a.MOrgID\n                    and b.MIsDelete = 0 \n\n\t                inner join t_bd_account_l c\n\t                on a.MToAcctID = c.MParentID\n                    AND c.MLocaleID = @MLocaleID\n                    and c.MOrgID = a.MOrgID\n                    and c.MIsDelete = 0 \n\n                    inner join t_bd_bankaccount k1\n                    on k1.MItemID = a.MFromAcctID\n                    and k1.MOrgID = a.MOrgID\n                    and k1.MIsDelete = 0 \n\n                    inner join t_bd_bankaccount k2\n                    on k2.MItemID = a.MToAcctID\n                    and k2.MOrgID = a.MOrgID\n                    and k2.MIsDelete = 0 \n\n\t                left join t_gl_doc_voucher d\n\t                on d.MDOCID = a.MID\n                    and d.MOrgID = a.MOrgID\n                    and d.MEntryID = a.MID\n\t                and d.MDOCTYPE = {filter.Type}\n                    and d.MergeStatus != -1\n                    and d.MIsDelete = 0 \n\n\t                left join t_gl_voucher k\n\t                on k.MItemID = d.MVoucherID\n                    and k.MOrgID = a.MOrgID\n                    and k.MIsDelete = 0 \n\n\t                left join t_gl_voucherentry e\n\t                on d.MDebitEntryID = e.MEntryID\n                    and e.MOrgID = a.MOrgID\n                    and e.MIsDelete = 0 \n\n                    left join t_bd_account ff\n                    on ff.MItemID = e.MAccountID\n                    and ff.MOrgID = a.MOrgID\n                    and ff.MIsDelete = 0 \n\n\t                left join t_bd_account_l f\n\t                on f.MParentID = e.MAccountID\n                    and f.MLocaleID = @MLocaleID\n                    and f.MOrgID = a.MOrgID\n                    and f.MIsDelete = 0 \n\n                    left join t_gl_voucherentry g\n\t                on d.MTaxEntryID = g.MEntryID\n                    and g.MOrgID = a.MOrgID\n                    and g.MIsDelete = 0 \n\n                    left join t_bd_account gg\n                    on gg.MItemID = g.MAccountID\n                    and gg.MOrgID = a.MOrgID\n                    and gg.MIsDelete = 0 \n\n\t                left join t_bd_account_l h\n\t                on h.MParentID = g.MAccountID\n                    and h.MLocaleID = @MLocaleID\n                    and h.MOrgID = a.MOrgID\n                    and h.MIsDelete = 0 \n\n\t                left join t_gl_voucherentry i\n\t                on d.MCreditEntryID = i.MEntryID\n                    and i.MOrgID = a.MOrgID\n                    and i.MIsDelete = 0 \n\n                    left join t_bd_account hh\n                    on hh.MItemID = i.MAccountID\n                    and hh.MOrgID = a.MOrgID\n                    and hh.MIsDelete = 0 \n\n\t                left join t_bd_account_l j\n\t                on j.MParentID = i.MAccountID\n                    and j.MLocaleID = @MLocaleID\n                    and j.MOrgID = a.MOrgID\n                    and j.MIsDelete = 0 \n\n\t                where a.MOrgID = @MOrgID and a.MIsDelete = 0\n                ";
			if (filter.MDocIDs != null && filter.MDocIDs.Count > 0)
			{
				str = str + " and a.MID in ('" + string.Join("','", filter.MDocIDs) + "')";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(filter.Keyword))
				{
					str += " AND (a.MReference like concat('%',@Keyword,'%')\n\t\t                    OR b.MName like concat('%',@Keyword,'%')\n\t\t                    OR c.MName like concat('%',@Keyword,'%')\n\t\t                    OR f.MName like  concat('%',@Keyword,'%')\n\t\t                    OR h.MName like  concat('%',@Keyword,'%')\n                            OR j.MName like  concat('%',@Keyword,'%')\n                            OR f.MFullName like  concat('%',@Keyword,'%')\n\t\t                    OR h.MFullName like  concat('%',@Keyword,'%')\n                            OR j.MFullName like  concat('%',@Keyword,'%')";
					if (filter.DecimalKeyword.HasValue)
					{
						str += " OR a.MFromTotalAmt = @DecimalKeyword \r\n                                  OR a.MToTotalAmt = @DecimalKeyword  ";
					}
					if (filter.DatetimeKeyword.HasValue)
					{
						str += " OR a.MBizDate = @DatetimeKeyword ";
					}
					str += ")";
				}
				if (filter.Year * filter.Period > 0)
				{
					str += "\n                            and a.MBizDate >= @MStartDate \n                            and a.MBizDate <= @MEndDate\r\n                            and a.MBizDate >= @OrgBeginDate\r\n                            and a.MBizDate >= @GLBeginDate";
				}
				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					str = ((!(filter.Status == "0")) ? (str + " AND (length(ifnull(k.MNumber,'')) = 0)") : (str + " AND length(ifnull(k.MNumber,'')) > 0  and (k.MStatus  = 0 or k.MStatus = 1)"));
				}
				if (!string.IsNullOrWhiteSpace(filter.Number))
				{
					str += " AND ( a.MFromTotalAmt = @Number OR a.MToTotalAmt = @Number ) ";
				}
				str += "  order by a.MBizDate asc ";
				if (filter.rows > 0 && !calCount)
				{
					str = str + " limit " + (filter.page - 1) * filter.rows + "," + filter.rows;
				}
			}
			return "select " + (calCount ? "count(*) total " : "*") + " from (" + string.Format(str, filter.Type) + ") t1";
		}

		public bool GenerateVoucherByInvoice(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVInvoiceModel model = obj as IVInvoiceModel;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			decimal mExchangeRate = model.MExchangeRate;
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<BDAccountModel> accountList = instance.AccountList;
			bool flag = model.MTaxID == "No_Tax";
			bool flag2 = model.MTaxID == "Tax_Inclusive";
			bool flag3 = model.MTaxID == "Tax_Exclusive";
			string[] array = (from x in model.InvoiceEntry
			select x.MTaxID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToArray();
			BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.Contains("22210105")) ?? new BDAccountModel();
			BDContactsModel bDContactsModel = string.IsNullOrWhiteSpace(model.MContactID) ? null : instance.ContactList.FirstOrDefault((BDContactsModel x) => x.MItemID == model.MContactID);
			string contact = (bDContactsModel != null) ? bDContactsModel.MName : "";
			string contactCurrentAcctCode = (bDContactsModel != null) ? bDContactsModel.MCCurrentAccountCode : "";
			BDAccountModel bDAccountModel2 = string.IsNullOrWhiteSpace(contactCurrentAcctCode) ? new BDAccountModel() : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == contactCurrentAcctCode);
			List<string> itemIDList = (from x in model.InvoiceEntry
			select x.MItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToList();
			List<BDItemModel> source = (from x in instance.MerItemList
			where itemIDList.Contains(x.MItemID)
			select x).ToList();
			for (int i = 0; i < model.InvoiceEntry.Count; i++)
			{
				IVInvoiceEntryModel entry = model.InvoiceEntry[i];
				GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, entry.MEntryID, model.MID, 0);
				DateTime mBizDate = model.MBizDate;
				int year = mBizDate.Year;
				mBizDate = model.MBizDate;
				GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, model.MBizDate);
				BDItemModel item = source.FirstOrDefault((BDItemModel x) => x.MItemID == entry.MItemID) ?? new BDItemModel();
				decimal num = flag3 ? (entry.MAmount + entry.MTaxAmt) : entry.MAmount;
				decimal mAmountFor = flag3 ? (entry.MAmountFor + entry.MTaxAmtFor) : entry.MAmountFor;
				decimal mTaxAmt = entry.MTaxAmt;
				decimal mTaxAmtFor = entry.MTaxAmtFor;
				decimal num2 = flag2 ? (entry.MAmount - entry.MTaxAmt) : entry.MAmount;
				decimal mAmountFor2 = flag2 ? (entry.MAmountFor - entry.MTaxAmtFor) : entry.MAmountFor;
				BDAccountModel bDAccountModel3 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MDebitAccount))
				{
					bDAccountModel3.MItemID = entry.MDebitAccount;
				}
				else if (bDAccountModel2 != null)
				{
					bDAccountModel3.MItemID = bDAccountModel2.MItemID;
				}
				GLCheckGroupValueModel checkGroupValueModel = instance.GetCheckGroupValueModel(new GLCheckGroupValueModel
				{
					MContactID = model.MContactID,
					MEmployeeID = null,
					MMerItemID = entry.MItemID,
					MExpItemID = null,
					MPaItemID = null,
					MTrackItem1 = entry.MTrackItem1,
					MTrackItem2 = entry.MTrackItem2,
					MTrackItem3 = entry.MTrackItem3,
					MTrackItem4 = entry.MTrackItem4,
					MTrackItem5 = entry.MTrackItem5
				});
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MID = tempVoucherModel.MItemID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MOrgID = ctx.MOrgID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 0, 1, model.MNumber, contact, string.Empty, model.MReference, item.MNumber, string.Empty, entry.MDesc),
					MAccountID = bDAccountModel3.MItemID,
					MAmount = num,
					MAmountFor = mAmountFor,
					MDebit = num,
					MCredit = decimal.Zero,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = 0,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
				if (entry.MTaxAmtFor != decimal.Zero)
				{
					REGTaxRateModel tax = (!string.IsNullOrWhiteSpace(entry.MTaxID)) ? instance.TaxRateList.FirstOrDefault((REGTaxRateModel x) => x.MItemID == entry.MTaxID) : new REGTaxRateModel();
					BDAccountModel bDAccountModel4 = new BDAccountModel();
					if (!string.IsNullOrWhiteSpace(entry.MTaxAccount))
					{
						bDAccountModel4.MItemID = entry.MTaxAccount;
					}
					else
					{
						bDAccountModel4 = ((tax == null || string.IsNullOrWhiteSpace(tax.MSaleTaxAccountCode)) ? bDAccountModel : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == tax.MSaleTaxAccountCode));
					}
					GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
					{
						MOrgID = ctx.MOrgID,
						MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
						MID = tempVoucherModel.MItemID,
						MExplanation = utility.GetVoucherEntryExplanation(ctx, 0, -1, model.MNumber, contact, string.Empty, model.MReference, item.MNumber, string.Empty, entry.MDesc),
						MAccountID = bDAccountModel4.MItemID,
						MAmount = mTaxAmt,
						MAmountFor = mTaxAmtFor,
						MCurrencyID = model.MCyID,
						MExchangeRate = mExchangeRate,
						MDebit = decimal.Zero,
						MCredit = mTaxAmt,
						MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
						MCheckGroupValueID = checkGroupValueModel.MItemID,
						IsNew = true,
						MEntrySeq = 1,
						MDC = -1
					};
					tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
					tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
				}
				BDAccountModel bDAccountModel5 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MCreditAccount))
				{
					bDAccountModel5.MItemID = entry.MCreditAccount;
				}
				else if (!string.IsNullOrWhiteSpace(item.MIncomeAccountCode))
				{
					bDAccountModel5.MItemID = accountList.FirstOrDefault((BDAccountModel x) => x.MCode == item.MIncomeAccountCode)?.MItemID;
				}
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 0, -1, model.MNumber, contact, string.Empty, model.MReference, item.MNumber, string.Empty, entry.MDesc),
					MAccountID = bDAccountModel5.MItemID,
					MAmount = num2,
					MAmountFor = mAmountFor2,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MDebit = decimal.Zero,
					MCredit = num2,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					IsNew = true,
					MEntrySeq = (string.IsNullOrWhiteSpace(entry.MTaxID) ? 1 : 2),
					MDC = -1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
				tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
				tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
				voucherList.Add(tempVoucherModel);
				docVoucherList.Add(tempDocVoucherModel);
			}
			return model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
		}

		private string GetInvoiceQuerySql(MContext ctx, GLDocVoucherFilterModel filter, bool calCount = false)
		{
			string empty = string.Empty;
			if (calCount)
			{
				empty = "select count(*) total from (" + GetInvoiceBaseSql(filter, true, false) + ") as t1";
			}
			else
			{
				empty = "select distinct\n                        a.MNumber,\n                        a.MBizDate,\n                        a.MModifyDate,\n                        a.MReference,\n                        a.MTotalAmt,\n                        a.MTaxTotalAmt,\n                        a.MTaxAmt as MTax,\n                        a.MStatus as MDocStatus,\n                        a.MOrgID,\n                        a.MType,\n                        CONVERT( AES_DECRYPT(c.MName, '{0}') USING UTF8) AS MContactName ,\n                        b.MEntryID,\n                        b.MID as MDocID,\n                        b.MTaxID,\n                        b.MAmount,\n                        b.MTaxAmt,\n                        b.MDesc,\n                        b.MItemID,\n                        b.MSeq,\n                        d.MItemID as MDocVoucherID,\n                        d.MDOCTYPE,\n                        d.MDebitEntryID,\n                        d.MCreditEntryID,\n                        d.MTaxEntryID,\n                        d.MergeStatus,\n                        k.MNumber as MVoucherNumber,\n                        k.MYear,\n                        k.MPeriod,\n                        k.MStatus as MVoucherStatus,\n                        k.MItemID as MVoucherID,\n                        e.MEntryID as MDebitEntryID,\n                        g.MEntryID as MTaxEntryID,\n                        i.MEntryID as MCreditEntryID,\n                        f.MFullName as MDebitAccountName,\n                        h.MFullName as MTaxAccountName,\n                        j.MFullName as MCreditAccountName,\n                        ff.MItemID as MDebitAccountID,\n                        hh.MItemID as MTaxAccountID,\n                        jj.MItemID as MCreditAccountID,\n                        f.MFullName as MDebitAccountFullName,\n                        h.MFullName as MTaxAccountFullName,\n                        j.MFullName as MCreditAccountFullName,\n                        TRIM(TRAILING ':' FROM concat(l.MNumber , ':', IFNULL(n.MDesc, ''))) as MItemName " + GetInvoiceBaseSql(filter, false, false);
				empty = empty + " and b.MEntryID in(" + GetMEntryIDBySql(ctx, filter, GetInvoiceBaseSql(filter, false, true)) + ")";
				empty += " order by a.MBizDate desc, a.MID desc, b.MSeq asc ";
			}
			return string.Format(empty, "JieNor-001");
		}

		public string GetInvoiceBaseSql(GLDocVoucherFilterModel filter, bool calCount = false, bool getEntryID = false)
		{
			string str = " from t_iv_invoice a\n                        inner join t_iv_invoiceentry b\n                        on a.MID = b.MID\n                        and b.MOrgID = a.MOrgID\n                        and b.MIsDelete = 0 \n\n                        inner join t_bd_contacts_l c\n                        on a.MContactID = c.MParentID\n                        and c.MLocaleID = @MLocaleID\n                        and c.MOrgID = a.MOrgID\n                        and c.MIsDelete = 0 \n\n                        left join t_gl_doc_voucher d\n                        on d.MDOCID = b.MID\n                        and d.MOrgID = a.MOrgID\n                        and d.MEntryID = b.MEntryID\n                        and d.MDOCTYPE = {0}\n                        and d.MergeStatus != -1\n                        and d.MIsDelete = 0 \n\n                        left join t_gl_voucher k\n                        on k.MItemID = d.MVoucherID\n                        and k.MOrgID = a.MOrgID\n                        and k.MIsDelete = 0 \n\n                        left join t_gl_voucherentry e\n                        on d.MDebitEntryID = e.MEntryID\n                        and e.MOrgID = a.MOrgID\n                        and e.MIsDelete = 0 \n\n                        left join t_bd_account ff\n                        on ff.MItemID = e.MAccountID\n                        and ff.MOrgID = a.MOrgID\n                        and ff.MIsDelete = 0 \n\n                        left join t_bd_account_l f\n                        on f.MParentID = e.MAccountID\n                        and f.MOrgID = a.MOrgID\n                        and f.MLocaleID = @MLocaleID\n                        and f.MIsDelete = 0 \n\n                        left join t_gl_voucherentry g\n                        on d.MTaxEntryID = g.MEntryID\n                        and g.MOrgID = a.MOrgID\n                        and g.MIsDelete = 0 \n\n                        left join t_bd_account hh\n                        on hh.MItemID = g.MAccountID\n                        and hh.MOrgID = a.MOrgID\n                        and hh.MIsDelete = 0 \n\n                        left join t_bd_account_l h\n                        on h.MParentID = g.MAccountID\n                        and h.MOrgID = a.MOrgID\n                        and h.MLocaleID = @MLocaleID\n                        and h.MIsDelete = 0 \n\n                        left join t_gl_voucherentry i\n                        on d.MCreditEntryID = i.MEntryID\n                        and i.MOrgID = a.MOrgID\n                        and i.MIsDelete = 0 \n\n                        left join t_bd_account jj\n                        on jj.MItemID = i.MAccountID\n                        and jj.MOrgID = a.MOrgID\n                        and jj.MIsDelete = 0 \n\n                        left join t_bd_account_l j\n                        on j.MParentID = i.MAccountID\n                        and j.MLocaleID = @MLocaleID\n                        and j.MOrgID = a.MOrgID\n                        and j.MIsDelete = 0 \n                        \n                        left join t_bd_item l\n                        on l.MItemID = b.MItemID\n                        and l.MOrgID = a.MOrgID\n                        and l.MIsDelete = 0 \n\n                        left join t_bd_item_l n\n                        on n.MParentID = l.MItemID\n                        and n.MLocaleID = @MLocaleID\n                        and n.MOrgID = a.MOrgID\n                        and n.MIsDelete = 0 \n\n                        where a.MIsDelete = 0 and a.MOrgID = @MOrgID \n                ";
			if (filter.MDocIDs != null && filter.MDocIDs.Count > 0)
			{
				str = str + " and a.MID in ('" + string.Join("','", filter.MDocIDs) + "')";
			}
			else if (filter.MEntryIDs != null && filter.MEntryIDs.Count > 0)
			{
				str = str + " and b.MEntryID in('" + string.Join("','", filter.MEntryIDs.ToArray()) + "')";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(filter.Keyword))
				{
					str += " AND (\n                                a.MReference like concat('%',@Keyword,'%')\n                                OR b.MDesc like concat('%',@Keyword,'%')\n                                OR CONVERT( AES_DECRYPT(c.MName, '{1}') USING UTF8) like concat('%',@Keyword,'%')\n                                OR a.MNumber like  concat('%',@Keyword,'%')\r\n                                OR k.MNumber like  concat('%',@Keyword,'%')\r\n                                OR f.MName like  concat('%',@Keyword,'%')\r\n                                OR f.MFullName like  concat('%',@Keyword,'%')\r\n                                OR h.MName like  concat('%',@Keyword,'%')\r\n                                OR h.MFullName like  concat('%',@Keyword,'%')\r\n                                OR j.MName like  concat('%',@Keyword,'%')\r\n                                OR j.MFullName like  concat('%',@Keyword,'%')\r\n                                OR l.MNumber like concat ('%',@Keyword,'%')\r\n                                OR n.MDesc like concat ('%',@Keyword,'%')";
					if (filter.DecimalKeyword.HasValue)
					{
						str += " OR a.MTotalAmt = @DecimalKeyword  \r\n                                  OR a.MTaxTotalAmt = @DecimalKeyword \r\n                                  OR a.MTaxAmt = @DecimalKeyword\r\n                                  OR b.MAmount = @DecimalKeyword \r\n                                  OR b.MTaxAmt = @DecimalKeyword ";
					}
					if (filter.DatetimeKeyword.HasValue)
					{
						str += " OR a.MBizDate = @DatetimeKeyword";
					}
					str += ")";
				}
				if (filter.Year * filter.Period > 0)
				{
					str += "\n                            and a.MBizDate >= @MStartDate \n                            and a.MBizDate <= @MEndDate\r\n                            and a.MBizDate >= @OrgBeginDate\r\n                            and a.MBizDate >= @GLBeginDate";
				}
				str = ((filter.Type != 0) ? (str + " and (a.MType = 'Invoice_Purchase' or a.MType = 'Invoice_Purchase_Red')") : (str + " and (a.MType = 'Invoice_Sale' or a.MType = 'Invoice_Sale_Red')"));
				if (!string.IsNullOrWhiteSpace(filter.Number))
				{
					str += " AND (a.MNumber like concat('%', @Number ,'%') Or a.MTotalAmt = @Number) ";
				}
				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					str = ((!(filter.Status == "0")) ? (str + " AND (length(ifnull(k.MNumber,'')) = 0)") : (str + " AND length(ifnull(k.MNumber,'')) > 0 and (k.MStatus  = 0 or k.MStatus = 1)"));
				}
			}
			str = string.Format(str, filter.Type, "JieNor-001");
			if (calCount)
			{
				str = " select distinct a.MID " + str;
			}
			else if (getEntryID)
			{
				str = " select distinct a.MID, b.MEntryID " + str + " order by a.MBizDate desc, a.MID desc, b.MSeq asc  ";
			}
			return str;
		}

		public bool GenerateVoucherByExpense(MContext ctx, object obj, ref List<GLVoucherModel> voucherList, ref List<GLDocVoucherModel> docVoucherList)
		{
			IVExpenseModel model = obj as IVExpenseModel;
			decimal mExchangeRate = model.MExchangeRate;
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountList = instance.AccountList;
			BDEmployeesModel bDEmployeesModel = string.IsNullOrEmpty(model.MEmployee) ? null : instance.EmployeeList.FirstOrDefault((BDEmployeesModel x) => x.MItemID == model.MEmployee);
			string employee = (bDEmployeesModel != null) ? bDEmployeesModel.MFullName : "";
			string employeeAcctCode = (bDEmployeesModel != null) ? bDEmployeesModel.MCurrentAccountCode : "";
			BDAccountModel bDAccountModel = string.IsNullOrWhiteSpace(employeeAcctCode) ? new BDAccountModel() : accountList.FirstOrDefault((BDAccountModel x) => x.MCode == employeeAcctCode);
			string[] itemIDList = (from x in model.ExpenseEntry
			select x.MItemID into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToArray();
			List<BDExpenseItemModel> source = (itemIDList.Count() == 0) ? new List<BDExpenseItemModel>() : (from x in instance.ExpenseItemList
			where itemIDList.Contains(x.MItemID)
			select x).ToList();
			BDAccountModel bDAccountModel2 = accountList.FirstOrDefault((BDAccountModel x) => x.MCode.Contains("22210101")) ?? new BDAccountModel();
			for (int i = 0; i < model.ExpenseEntry.Count; i++)
			{
				IVExpenseEntryModel entry = model.ExpenseEntry[i];
				GLDocVoucherModel tempDocVoucherModel = GetTempDocVoucherModel(ctx, entry.MEntryID, model.MID, 2);
				DateTime mBizDate = model.MBizDate;
				int year = mBizDate.Year;
				mBizDate = model.MBizDate;
				GLVoucherModel tempVoucherModel = GetTempVoucherModel(ctx, year, mBizDate.Month, model.MBizDate);
				tempVoucherModel.MRowIndex = model.MRowIndex;
				BDExpenseItemModel item = source.FirstOrDefault((BDExpenseItemModel x) => x.MItemID == entry.MItemID) ?? new BDExpenseItemModel();
				BDAccountModel bDAccountModel3 = (!string.IsNullOrWhiteSpace(item.MItemID)) ? accountList.FirstOrDefault((BDAccountModel x) => x.MCode == item.MAccountCode) : new BDAccountModel();
				BDAccountModel bDAccountModel4 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MDebitAccount))
				{
					bDAccountModel4.MItemID = entry.MDebitAccount;
				}
				else if (bDAccountModel3 != null)
				{
					bDAccountModel4 = bDAccountModel3;
				}
				GLCheckGroupValueModel checkGroupValueModel = utility.GetCheckGroupValueModel(ctx, new GLCheckGroupValueModel
				{
					MContactID = null,
					MEmployeeID = model.MContactID,
					MMerItemID = null,
					MExpItemID = entry.MItemID,
					MPaItemID = null,
					MTrackItem1 = entry.MTrackItem1,
					MTrackItem2 = entry.MTrackItem2,
					MTrackItem3 = entry.MTrackItem3,
					MTrackItem4 = entry.MTrackItem4,
					MTrackItem5 = entry.MTrackItem5
				});
				GLVoucherEntryModel gLVoucherEntryModel = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 2, 1, model.MNumber, string.Empty, employee, model.MReference, string.Empty, item.MName, entry.MDesc),
					MAccountID = bDAccountModel4.MItemID,
					MAmount = entry.MAmount - entry.MTaxAmt,
					MAmountFor = entry.MAmountFor - entry.MTaxAmtFor,
					MDebit = entry.MAmount - entry.MTaxAmt,
					MCredit = decimal.Zero,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					IsNew = true,
					MEntrySeq = 0,
					MDC = 1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel);
				tempDocVoucherModel.MDebitEntryID = gLVoucherEntryModel.MEntryID;
				if (entry.MTaxAmtFor != decimal.Zero)
				{
					BDAccountModel bDAccountModel5 = new BDAccountModel();
					if (!string.IsNullOrWhiteSpace(entry.MTaxAccount))
					{
						bDAccountModel5.MItemID = entry.MTaxAccount;
					}
					else if (bDAccountModel2 != null)
					{
						bDAccountModel5 = bDAccountModel2;
					}
					GLVoucherEntryModel gLVoucherEntryModel2 = new GLVoucherEntryModel
					{
						MOrgID = ctx.MOrgID,
						MID = tempVoucherModel.MItemID,
						MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
						MExplanation = utility.GetVoucherEntryExplanation(ctx, 2, 1, model.MNumber, string.Empty, employee, model.MReference, string.Empty, item.MName, entry.MDesc),
						MAccountID = bDAccountModel5.MItemID,
						MAmount = entry.MTaxAmt,
						MAmountFor = entry.MTaxAmtFor,
						MDebit = entry.MTaxAmt,
						MCredit = decimal.Zero,
						MCurrencyID = model.MCyID,
						MExchangeRate = mExchangeRate,
						MCheckGroupValueID = checkGroupValueModel.MItemID,
						MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
						IsNew = true,
						MEntrySeq = tempVoucherModel.MVoucherEntrys.Count,
						MDC = 1
					};
					tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel2);
					tempDocVoucherModel.MTaxEntryID = gLVoucherEntryModel2.MEntryID;
				}
				BDAccountModel bDAccountModel6 = new BDAccountModel();
				if (!string.IsNullOrWhiteSpace(entry.MCreditAccount))
				{
					bDAccountModel6.MItemID = entry.MCreditAccount;
				}
				else if (bDAccountModel != null)
				{
					bDAccountModel6 = bDAccountModel;
				}
				GLVoucherEntryModel gLVoucherEntryModel3 = new GLVoucherEntryModel
				{
					MOrgID = ctx.MOrgID,
					MEntryID = JieNor.Megi.Common.UUIDHelper.GetGuid(),
					MID = tempVoucherModel.MItemID,
					MExplanation = utility.GetVoucherEntryExplanation(ctx, 2, -1, model.MNumber, string.Empty, employee, model.MReference, string.Empty, item.MName, entry.MDesc),
					MAccountID = bDAccountModel6.MItemID,
					MAmount = entry.MTaxAmount,
					MAmountFor = entry.MTaxAmountFor,
					MDebit = decimal.Zero,
					MCredit = entry.MTaxAmount,
					MCurrencyID = model.MCyID,
					MExchangeRate = mExchangeRate,
					MCheckGroupValueID = checkGroupValueModel.MItemID,
					MCheckGroupValueModel = utility.CopyCheckGroupValueModel(checkGroupValueModel),
					IsNew = true,
					MEntrySeq = 1,
					MDC = -1
				};
				tempVoucherModel.MVoucherEntrys.Add(gLVoucherEntryModel3);
				tempDocVoucherModel.MCreditEntryID = gLVoucherEntryModel3.MEntryID;
				tempDocVoucherModel.MVoucherID = tempVoucherModel.MItemID;
				voucherList.Add(tempVoucherModel);
				docVoucherList.Add(tempDocVoucherModel);
			}
			return model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment);
		}

		private string GetExpenseQuerySql(MContext ctx, GLDocVoucherFilterModel filter, bool calCount = false)
		{
			string empty = string.Empty;
			if (calCount)
			{
				empty = "select count(*) total from (" + GetExpenseBaseSql(filter, true, false) + ") as t1";
			}
			else
			{
				empty = "select distinct\n                    a.MBizDate,\n                    a.MModifyDate,\n                    a.MReference,\n                    a.MTotalAmt,\n                    a.MTaxTotalAmt,\n                    a.MStatus as MDocStatus,\n                    a.MOrgID,\n                    a.MType,\n                    a.MTaxAmt as MTax,\n                    c.MFirstName ,\n                    c.MLastName,\n                    b.MEntryID,\n                    b.MID,\n                    b.MID as MDocID,\n                    b.MItemID,\n                    b.MTaxAmount as MAmount,\n                    b.MTaxAmt,\n                    b.MDesc,\n                    b.MSeq,\n                    d.MItemID as MDocVoucherID,\n                    d.MDOCTYPE,\n                    d.MDebitEntryID,\n                    d.MCreditEntryID,\n                    d.MTaxEntryID,\n                    d.MergeStatus,\n                    k.MNumber as MVoucherNumber,\n                    k.MYear,\n                    k.MPeriod,\n                    k.MStatus as MVoucherStatus,\n                    k.MItemID as MVoucherID,\n                    e.MEntryID as MDebitEntryID,\n                    g.MEntryID as MCreditEntryID,\n                    m.MEntryID as MTaxEntryID,\n                    f.MFullName as MDebitAccountName,\n                    h.MFullName as MCreditAccountName,\n                    n.MFullName as MTaxAccountName,\n                    ff.MItemID as MDebitAccountID,\n                    nn.MItemID as MTaxAccountID,\n                    hh.MItemID as MCreditAccountID,\n                    f.MFullName as MDebitAccountFullName,\n                    h.MFullName as MCreditAccountFullName,\n                    n.MFullName as MTaxAccountFullName,\n                    TRIM(TRAILING ':' FROM CONCAT( ifnull(l.MName,''),':', ifnull(l.mdesc, ''))) as MItemName " + GetExpenseBaseSql(filter, false, false);
				empty = empty + " and b.MEntryID in(" + GetMEntryIDBySql(ctx, filter, GetExpenseBaseSql(filter, false, true)) + ")";
				empty += " order by a.MBizDate desc, a.MID desc, b.MSeq asc ";
			}
			return string.Format(empty, filter.Type, "JieNor-001");
		}

		private string GetExpenseBaseSql(GLDocVoucherFilterModel filter, bool calCount = false, bool getEntryID = false)
		{
			string text = "\n                from t_iv_expense a\n                inner join t_iv_expenseentry b\n                on a.MID = b.MID\n                and b.MOrgID = a.MOrgID\n                and b.MIsDelete = 0\n\n                left join t_bd_employees_l c\n                on a.MContactID = c.MParentID\n                AND c.MLocaleID = @MLocaleID\n                and c.MOrgID = a.MOrgID\n                and c.MIsDelete = 0 \n\n                left join t_gl_doc_voucher d\n                on d.MDOCID = b.MID\n                and d.MOrgID = a.MOrgID\n                and d.MEntryID = b.MEntryID\n                and d.MDOCTYPE = {0}\n                and d.MergeStatus != -1\n                and d.MIsDelete = 0 \n\n                left join t_gl_voucher k\n                on k.MItemID = d.MVoucherID\n                and k.MOrgID = a.MOrgID\n                and k.MIsDelete = 0 \n\n                left join t_gl_voucherentry e\n                on d.MDebitEntryID = e.MEntryID\n                and e.MOrgID = a.MOrgID\n                and e.MIsDelete = 0 \n\n                left join t_bd_account ff\n                on ff.MItemID = e.MAccountID\n                and ff.MOrgID = a.MOrgID\n                and ff.MIsDelete = 0 \n\n                left join t_bd_account_l f\n                on f.MParentID = e.MAccountID\n                and f.MLocaleID = @MLocaleID\n                and f.MOrgID = a.MOrgID\n                and f.MIsDelete = 0 \n\n                left join t_gl_voucherentry g\n                on d.MCreditEntryID = g.MEntryID\n                and g.MOrgID = a.MOrgID\n                and g.MIsDelete = 0 \n\n                left join t_bd_account hh\n                on hh.MItemID = g.MAccountID\n                and hh.MOrgID = a.MOrgID\n                and hh.MIsDelete = 0 \n\n                left join t_bd_account_l h\n                on h.MParentID = g.MAccountID\n                and h.MLocaleID = @MLocaleID\n                and h.MOrgID = a.MOrgID\n                and h.MIsDelete = 0 \n\n                left join t_bd_expenseitem_l l\n                on l.MParentID = b.MItemID\n                and l.MLocaleID = @MLocaleID\n                and l.MOrgID = a.MOrgID\n                and l.MIsDelete = 0 \n\n                \n                left join t_gl_voucherentry m\n                on d.MTaxEntryID = m.MEntryID\n                and e.MOrgID = m.MOrgID\n                and m.MIsDelete = 0 \n\n                left join t_bd_account nn\n                on nn.MItemID = m.MAccountID\n                and nn.MOrgID = a.MOrgID\n                and nn.MIsDelete = 0 \n\n                left join t_bd_account_l n\n                on n.MParentID = m.MAccountID\n                and n.MLocaleID = @MLocaleID\n                and n.MOrgID = a.MOrgID\n                and n.MIsDelete = 0 \n\n                where a.MOrgID = @MOrgID and a.MIsDelete = 0 \n                ";
			if (filter.MDocIDs != null && filter.MDocIDs.Count > 0)
			{
				text = text + " and a.MID in ('" + string.Join("','", filter.MDocIDs) + "')";
			}
			else if (filter.MEntryIDs != null && filter.MEntryIDs.Count > 0)
			{
				text = text + " and b.MEntryID in('" + string.Join("','", filter.MEntryIDs.ToArray()) + "')";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(filter.Keyword))
				{
					text += " AND (\n                    a.MReference like concat('%',@Keyword,'%')\n                    OR b.MDesc like concat('%',@Keyword,'%')\n                    OR MFirstName like concat('%',@Keyword,'%')\n                    OR MLastName like concat('%',@Keyword,'%')\r\n                    OR k.MNumber like  concat('%',@Keyword,'%')\r\n                    OR f.MName like  concat('%',@Keyword,'%')\r\n                    OR f.MFullName like  concat('%',@Keyword,'%')\r\n                    OR h.MName like  concat('%',@Keyword,'%')\r\n                    OR h.MFullName like  concat('%',@Keyword,'%')\r\n                    OR n.MName like concat ('%',@Keyword,'%')\r\n                    OR n.MFullName like concat ('%',@Keyword,'%')\r\n                    OR l.MName like concat ('%',@Keyword,'%')\r\n                    OR n.MDesc like concat ('%',@Keyword,'%') ";
					if (filter.DecimalKeyword.HasValue)
					{
						text += " OR a.MTotalAmt = @DecimalKeyword  \r\n                                  OR a.MTaxTotalAmt = @DecimalKeyword \r\n                                  OR a.MTaxAmt = @DecimalKeyword\r\n                                  OR b.MAmount = @DecimalKeyword \r\n                                  OR b.MTaxAmt = @DecimalKeyword ";
					}
					if (filter.DatetimeKeyword.HasValue)
					{
						text += " OR a.MBizDate = @DatetimeKeyword ";
					}
					text += ")";
				}
				if (filter.Year * filter.Period > 0)
				{
					text += "\n                            and a.MBizDate >= @MStartDate \n                            and a.MBizDate <= @MEndDate\r\n                            and a.MBizDate >= @OrgBeginDate\r\n                            and a.MBizDate >= @GLBeginDate";
				}
				if (!string.IsNullOrWhiteSpace(filter.Number))
				{
					text += " AND a.MTotalAmt = @Number ";
				}
				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					text = ((!(filter.Status == "0")) ? (text + " AND (length(ifnull(k.MNumber,'')) = 0)") : (text + " AND length(ifnull(k.MNumber,'')) > 0 and (k.MStatus  = 0 or k.MStatus = 1)"));
				}
			}
			text = string.Format(text, filter.Type, "JieNor-001");
			if (calCount)
			{
				text = " select distinct a.MID " + text;
			}
			else if (getEntryID)
			{
				text = " select distinct a.MID, b.MEntryID " + text + " order by a.MBizDate desc, a.MID desc, b.MSeq asc ";
			}
			return text;
		}

		public OperationResult CreateDocVoucher(MContext ctx, List<GLDocEntryVoucherModel> list, bool create)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<CommandInfo> list3 = new List<CommandInfo>();
			operationResult = CheckIsDeleted(ctx, (from t in list
			select t.MVoucherID).Distinct().ToList());
			if (!operationResult.Success)
			{
				return operationResult;
			}
			operationResult = CanDocBeOperate(ctx, (from x in list
			select x.MDocID).ToList());
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<GLDocEntryVoucherModel> mergedDocVoucherEntrys = (from x in list
			where x.MergeStatus == 1
			select x).ToList();
			List<GLDocVoucherModel> beMergedDocVouchers = GetBeMergedDocVouchers(ctx, (from x in mergedDocVoucherEntrys
			select x.MDocID).ToList());
			if (mergedDocVoucherEntrys != null && mergedDocVoucherEntrys.Any())
			{
				List<string> list4 = (from x in mergedDocVoucherEntrys
				select x.MDocID).Distinct().ToList();
				List<GLSimpleVoucherModel> simpleVouchersByDocIDs = GLDataPool.GetInstance(ctx, false, 0, 0, 0).GetSimpleVouchersByDocIDs(list4);
				List<CommandInfo> partlyDeleteDocsCreatedVoucherCmds = GetPartlyDeleteDocsCreatedVoucherCmds(ctx, list4, simpleVouchersByDocIDs);
				list2.AddRange(partlyDeleteDocsCreatedVoucherCmds);
				int i;
				for (i = 0; i < mergedDocVoucherEntrys.Count; i++)
				{
					GLDocVoucherModel gLDocVoucherModel = beMergedDocVouchers.FirstOrDefault((GLDocVoucherModel x) => x.MDocID == mergedDocVoucherEntrys[i].MDocID && x.MEntryID == mergedDocVoucherEntrys[i].MEntryID);
					mergedDocVoucherEntrys[i].MVoucherID = gLDocVoucherModel.MVoucherID;
					mergedDocVoucherEntrys[i].MDocVoucherID = gLDocVoucherModel.MItemID;
					mergedDocVoucherEntrys[i].MergeStatus = 0;
					mergedDocVoucherEntrys[i].MDebitEntryID = gLDocVoucherModel.MDebitEntryID;
					mergedDocVoucherEntrys[i].MCreditEntryID = gLDocVoucherModel.MCreditEntryID;
					mergedDocVoucherEntrys[i].MTaxEntryID = gLDocVoucherModel.MTaxEntryID;
					mergedDocVoucherEntrys[i].MVoucherNumber = string.Empty;
				}
			}
			List<GLDocEntryVoucherModel> list5 = (from x in list
			where !string.IsNullOrWhiteSpace(x.MVoucherNumber)
			select x).ToList();
			List<GLDocEntryVoucherModel> entryList = (from x in list
			where string.IsNullOrWhiteSpace(x.MVoucherNumber)
			select x).ToList();
			if (create)
			{
				list2.AddRange(GetCreateDocVoucherByDocsCmds(ctx, entryList, operationResult));
				list2.AddRange(GetSaveDocVoucherOneByOne(ctx, list5, true));
			}
			else
			{
				list2.AddRange(GetSaveDocVoucherOneByOne(ctx, list, false));
			}
			if (!operationResult.Success)
			{
				return operationResult;
			}
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2) > 0);
			return operationResult;
		}

		private List<CommandInfo> GetSaveDocVoucherOneByOne(MContext ctx, List<GLDocEntryVoucherModel> list, bool create)
		{
			List<GLVoucherModel> list2 = new List<GLVoucherModel>();
			List<GLDocVoucherModel> list3 = new List<GLDocVoucherModel>();
			List<CommandInfo> list4 = new List<CommandInfo>();
			for (int i = 0; i < list.Count; i++)
			{
				GLDocEntryVoucherModel entry = list[i];
				GLVoucherModel gLVoucherModel = _voucher.GetVoucherModelList(ctx, new List<string>
				{
					entry.MVoucherID
				}, true, 0, 0)[0];
				bool flag = !string.IsNullOrWhiteSpace(gLVoucherModel.MNumber);
				GLDocVoucherModel item = DocEntryModel2DocVoucherModel(list[i]);
				gLVoucherModel.MNumber = (string.IsNullOrWhiteSpace(entry.MVoucherNumber) ? null : entry.MVoucherNumber);
				GLVoucherEntryModel gLVoucherEntryModel = gLVoucherModel.MVoucherEntrys.Find((GLVoucherEntryModel x) => x.MEntryID.Equals(entry.MDebitEntryID));
				GLVoucherEntryModel gLVoucherEntryModel2 = gLVoucherModel.MVoucherEntrys.Find((GLVoucherEntryModel x) => x.MEntryID.Equals(entry.MTaxEntryID));
				GLVoucherEntryModel gLVoucherEntryModel3 = gLVoucherModel.MVoucherEntrys.Find((GLVoucherEntryModel x) => x.MEntryID.Equals(entry.MCreditEntryID));
				gLVoucherEntryModel.MAccountID = entry.MDebitAccountID;
				if (gLVoucherEntryModel2 != null)
				{
					gLVoucherEntryModel2.MAccountID = entry.MTaxAccountID;
				}
				gLVoucherEntryModel3.MAccountID = entry.MCreditAccountID;
				if (create | flag)
				{
					utility.CheckVoucherCheckGroupValueMatchCheckGroup(ctx, new List<GLVoucherModel>
					{
						gLVoucherModel
					}, true);
				}
				list2.Add(gLVoucherModel);
				list3.Add(item);
			}
			list4.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list2, null, null));
			list4.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, new GLUtility().Union((from x in list2
			select x.MVoucherEntrys).ToList()), null, "voucherEntry_docVoucher"));
			list4.AddRange(ModelInfoManager.GetBatchInsertOrUpdateCmds(ctx, list3, null, null));
			return list4;
		}

		public void SetVoucherListNumbers(MContext ctx, ref List<GLVoucherModel> voucherList)
		{
			if (voucherList != null && voucherList.Count != 0)
			{
				IEnumerable<IGrouping<int, GLVoucherModel>> enumerable = from x in voucherList
				group x by x.MYear * 100 + x.MPeriod;
				foreach (IGrouping<int, GLVoucherModel> item in enumerable)
				{
					List<GLVoucherModel> list = item.ToList();
					int mYear = list[0].MYear;
					int mPeriod = list[0].MPeriod;
					List<string> nextVoucherNumbers = COMResourceHelper.GetNextVoucherNumbers(ctx, mYear, mPeriod, list.Count, null, null);
					for (int i = 0; i < list.Count; i++)
					{
						list[i].MNumber = nextVoucherNumbers[i];
						list[i].MStatus = 0;
					}
				}
			}
		}

		public bool CanDeleteOrUnapproveDoc(MContext ctx, List<string> docIDs)
		{
			bool result = true;
			List<GLDocVoucherModel> docCreatedVoucher = GetDocCreatedVoucher(ctx, docIDs);
			if (docCreatedVoucher != null && docCreatedVoucher.Count > 0)
			{
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MItemID", SqlOperators.In, (from x in docCreatedVoucher
				select x.MVoucherID).Distinct().ToList());
				List<GLVoucherModel> modelList = new GLVoucherRepository().GetModelList(ctx, sqlWhere, false);
				if (modelList != null && modelList.Count > 0)
				{
					result = (!modelList.Exists((GLVoucherModel x) => x.MStatus == 1) && !new GLSettlementRepository().IsPeriodsExistsSettled(ctx, (from x in modelList
					select x.MDate).Distinct().ToList()));
				}
			}
			return result;
		}

		public List<GLDocVoucherModel> GetDocVoucherByVoucherID(MContext ctx, string voucherID)
		{
			string sql = "select * from t_gl_doc_voucher where MVOUCHERID = @MVoucherID and MOrgID = @MOrgID and MIsDelete = 0 and MIsActive = 1";
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, sql, new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MVoucherID",
					Value = voucherID
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			});
		}

		public List<string> GetUpdatedDocTable(MContext ctx, DateTime lastQueryTime)
		{
			List<string> list = new List<string>();
			string empty = string.Empty;
			if (ctx.MOrgVersionID == 0)
			{
				empty = empty + " select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "SaleInvoice", "Sale Invoice") + "' as MType, max(a.MModifyDate) as MDate from t_iv_invoice a where a.MOrgID = @MOrgID and (a.MType = 'Invoice_Sale' or a.MType = 'Invoice_Sale_Red') and a.MIsDelete=0 ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PurchaseInvoice", "Purchase Invoice") + "' as MType, max(b.MModifyDate) as MDate from t_iv_invoice b where b.MOrgID = @MOrgID and (b.MType = 'Invoice_Purchase' or b.MType = 'Invoice_Purchase_Red') and b.MIsDelete=0  ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Expense", "Expense") + "' as MType, max(c.MModifyDate) as MDate from t_iv_expense c where c.MOrgID = @MOrgID   and c.MIsDelete=0 ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Receive", "Receive") + "' as MType, max(d.MModifyDate) as MDate from t_iv_receive d where d.MOrgID = @MOrgID   and d.MIsDelete=0  ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Payment", "Payment") + "' as MType, max(e.MModifyDate) as MDate from t_iv_payment e where e.MOrgID = @MOrgID   and e.MIsDelete=0  ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Transfer", "Transfer") + "' as MType, max(f.MModifyDate) as MDate from t_iv_transfer f where f.MOrgID = @MOrgID   and f.MIsDelete=0  ";
				empty = empty + " union  select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Voucher", "凭证") + "' as MType, max(g.MModifyDate) as MDate from t_gl_voucher g where g.MOrgID = @MOrgID   and g.MIsDelete=0   ";
			}
			else
			{
				empty = empty + " select '" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Voucher", "凭证") + "' as MType, max(a.MModifyDate) as MDate from t_gl_voucher a where a.MOrgID = @MOrgID  and a.MIsDelete=0  ";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(empty, cmdParms);
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				DataRowCollection rows = dataSet.Tables[0].Rows;
				for (int i = 0; i < rows.Count; i++)
				{
					DateTime t = (rows[i]["MDate"] == null || string.IsNullOrWhiteSpace(rows[i]["MDate"].ToString())) ? DateTime.MinValue : DateTime.Parse(rows[i]["MDate"].ToString());
					if (t > lastQueryTime)
					{
						list.Add(rows[i]["MType"].ToString());
					}
				}
			}
			return list;
		}

		public string GetMEntryIDBySql(MContext ctx, GLDocVoucherFilterModel filter, string sql)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			MySqlParameter[] parameterByFilter = GetParameterByFilter(ctx, filter);
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, parameterByFilter);
			List<MIDEntryID> list3 = BindDataSet2Model(ds);
			int num = (filter.page - 1) * filter.rows;
			int num2 = (filter.rows == 0) ? 2147483647 : filter.rows;
			string text = string.Empty;
			for (int i = 0; i < list3.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(text) || text != list3[i].MID)
				{
					text = list3[i].MID;
					list2.Add(list3[i].MID);
				}
				if (list2.Count > num && list2.Count <= num + num2)
				{
					list.Add(list3[i].MEntryID);
				}
			}
			return "'" + string.Join("','", list.ToArray()) + "'";
		}

		public GLVoucherModel GetCreatedVoucherByDocID(MContext ctx, string docID)
		{
			string sql = "SELECT distinct\r\n                        t1.*\r\n                    FROM\r\n                        t_gl_voucher t1\r\n                            INNER JOIN\r\n                        t_gl_doc_voucher t2 \r\n                            ON t1.MItemID = t2.MVoucherID \r\n                            and t1.MOrgID = t2.MOrgID\r\n                            and t2.MIsDelete = 0\r\n                            and t2.MIsActive = 1\r\n                            where  t1.MOrgID = @MOrgID\r\n                            and t2.MDocID = @MDocID\r\n                            and (t1.MStatus  = 0 or t1.MStatus = 1) \r\n                            and length(ifnull(t1.MNumber,'')) > 0\r\n                            and t1.MIsDelete = 0\r\n                            ";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MDocID",
					Value = docID
				}
			};
			return ModelInfoManager.GetDataModel<GLVoucherModel>(ctx, sql, cmdParms);
		}

		public List<GLDocVoucherModel> GetDocVoucherList(MContext ctx, List<string> voucherIdList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			stringBuilder.Append("SELECT * FROM T_GL_DOC_Voucher WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MVoucherID IN ( ");
			int num = 1;
			foreach (string voucherId in voucherIdList)
			{
				string text = $"@IDParam{num}";
				stringBuilder.AppendFormat("{0},", text);
				list.Add(new MySqlParameter(text, voucherId));
				num++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(")");
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, stringBuilder.ToString(), list.ToArray());
		}

		public List<GLDocEntryVoucherModel> GetDocEntryVoucherModelListByFilter(MContext ctx, GLDocVoucherFilterModel filter)
		{
			string querySql = GetQuerySql(ctx, filter, false);
			MySqlParameter[] parameterByFilter = GetParameterByFilter(ctx, filter);
			DataTable dt = new DynamicDbHelperMySQL(ctx).Query(querySql.ToString(), parameterByFilter).Tables[0];
			List<GLDocEntryVoucherModel> list = ModelInfoManager.DataTableToList<GLDocEntryVoucherModel>(dt);
			if (filter.Type == 2)
			{
				list.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					if (ctx.MLCID == LangCodeEnum.EN_US)
					{
						x.MEmployeeName = x.MFirstName + " " + x.MLastName;
					}
					else
					{
						x.MEmployeeName = x.MLastName + " " + x.MFirstName;
					}
				});
			}
			list = ((filter.Type != 0) ? OtherDocOrderBy(list) : InvoiceOrderBy(list));
			if (filter.Type == 3 || filter.Type == 4)
			{
				list.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					if (x.MContactType == "Employees")
					{
						if (ctx.MLCID == LangCodeEnum.EN_US)
						{
							string text3 = x.MEmployeeName = (x.MContactName = x.MFirstName + " " + x.MLastName);
						}
						else
						{
							string text3 = x.MEmployeeName = (x.MContactName = x.MLastName + " " + x.MFirstName);
						}
					}
				});
			}
			if (filter.Type == 5)
			{
				list.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					if (string.IsNullOrWhiteSpace(x.MDebitAccountID))
					{
						x.MDebitAccountID = x.MToAccountID;
					}
					if (string.IsNullOrWhiteSpace(x.MCreditAccountID))
					{
						x.MCreditAccountID = x.MFromAccountID;
					}
				});
			}
			int year = filter.Year;
			int num = filter.Period;
			if (list != null && list.Count > 0)
			{
				DateTime mBizDate = list[0].MBizDate;
				year = mBizDate.Year;
				mBizDate = list[0].MBizDate;
				num = mBizDate.Month;
			}
			if (year * num > 0)
			{
				bool settled = new GLSettlementRepository().IsPeirodSettled(ctx, year, num);
				list.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					x.MSettleStatus = (settled ? 1 : 0);
				});
			}
			if (list.Count > 0)
			{
				list.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					x.MDocType = filter.Type;
				});
			}
			return list;
		}

		private List<GLDocEntryVoucherModel> InvoiceOrderBy(List<GLDocEntryVoucherModel> list)
		{
			List<GLDocEntryVoucherModel> newList = new List<GLDocEntryVoucherModel>();
			if (list != null && list.Count > 0)
			{
				List<string> list2 = (from x in (from x in list
				select x.MBizDate.ToString("yyyy-MM-dd") + "$" + x.MNumber + "$" + x.MDocID).Distinct()
				orderby x descending
				select x).ToList();
				list2.ForEach(delegate(string x)
				{
					newList.AddRange((from y in list
					where y.MBizDate.ToString("yyyy-MM-dd") + "$" + y.MNumber + "$" + y.MDocID == x
					select y into m
					orderby m.MSeq
					select m).ToList());
				});
			}
			return newList;
		}

		private List<GLDocEntryVoucherModel> OtherDocOrderBy(List<GLDocEntryVoucherModel> list)
		{
			list = (list ?? new List<GLDocEntryVoucherModel>());
			return (from x in list
			orderby x.MBizDate descending, x.MModifyDate descending
			select x).ToList();
		}

		public int GetDocEntryVoucherModelListCountByFilter(MContext ctx, GLDocVoucherFilterModel filter)
		{
			string querySql = GetQuerySql(ctx, filter, true);
			MySqlParameter[] parameterByFilter = GetParameterByFilter(ctx, filter);
			DataTable dataTable = new DynamicDbHelperMySQL(ctx).Query(querySql.ToString(), parameterByFilter).Tables[0];
			return int.Parse(dataTable.Rows[0]["total"].ToString());
		}

		private string GetQuerySql(MContext ctx, GLDocVoucherFilterModel filter, bool calCount = false)
		{
			switch (filter.Type)
			{
			case 0:
			case 1:
				return GetInvoiceQuerySql(ctx, filter, calCount);
			case 2:
				return GetExpenseQuerySql(ctx, filter, calCount);
			case 3:
			case 4:
				return GetReceivePaymentQuerySql(ctx, filter, calCount);
			case 5:
				return GetTransferQuerySql(filter, calCount);
			default:
				return string.Empty;
			}
		}

		private MySqlParameter[] GetParameterByFilter(MContext ctx, GLDocVoucherFilterModel filter)
		{
			DateTime dateTime = (filter.Year * filter.Period == 0) ? DateTime.Now : new DateTime(filter.Year, filter.Period, 1);
			DateTime dateTime3;
			if (filter.Year * filter.Period != 0)
			{
				DateTime dateTime2 = new DateTime(filter.Year, filter.Period, 1);
				dateTime2 = dateTime2.AddMonths(1);
				dateTime3 = dateTime2.AddDays(-1.0);
			}
			else
			{
				dateTime3 = DateTime.Now;
			}
			DateTime dateTime4 = dateTime3;
			return new MySqlParameter[11]
			{
				new MySqlParameter
				{
					ParameterName = "@MDocID",
					Value = filter.MDocID
				},
				new MySqlParameter
				{
					ParameterName = "@MStartDate",
					Value = (object)dateTime
				},
				new MySqlParameter
				{
					ParameterName = "@MEndDate",
					Value = (object)dateTime4
				},
				new MySqlParameter
				{
					ParameterName = "@OrgBeginDate",
					Value = (object)ctx.MBeginDate
				},
				new MySqlParameter
				{
					ParameterName = "@GLBeginDate",
					Value = (object)ctx.MGLBeginDate
				},
				new MySqlParameter
				{
					ParameterName = "@Keyword",
					Value = filter.Keyword
				},
				new MySqlParameter
				{
					ParameterName = "@DecimalKeyword",
					Value = (object)(filter.DecimalKeyword.HasValue ? filter.DecimalKeyword.Value : decimal.Zero)
				},
				new MySqlParameter
				{
					ParameterName = "@DatetimeKeyword",
					Value = (object)(filter.DatetimeKeyword.HasValue ? filter.DatetimeKeyword.Value : DateTime.MinValue)
				},
				new MySqlParameter
				{
					ParameterName = "@Number",
					Value = filter.Number
				},
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MLocaleID",
					Value = ctx.MLCID
				}
			};
		}

		public List<GLDocVoucherModel> GetDocCreatedVoucher(MContext ctx, string docID)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MDocID", SqlOperators.Equal, docID);
			return GetModelList(ctx, sqlWhere, false);
		}

		public List<GLDocVoucherModel> GetDocCreatedVoucher(MContext ctx, List<string> docIDs)
		{
			if (docIDs == null || (from x in docIDs
			where !string.IsNullOrWhiteSpace(x)
			select x).Count() == 0)
			{
				return new List<GLDocVoucherModel>();
			}
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MDocID", SqlOperators.In, docIDs);
			return new GLDocVoucherRepository().GetModelList(ctx, sqlWhere, false);
		}

		public List<GLDocVoucherModel> GetDocVoucherList(MContext ctx, int? year, int? period, int? type, int? status)
		{
			string text = "SELECT * FROM t_gl_doc_voucher t1\r\n                     inner join t_gl_voucher t2\r\n                     on t1.MVoucherID = t2.MItemID \r\n                     and t1.MOrgId = t2.MOrgID\r\n                     and t2.MIsDelete = 0 \r\n                     where \r\n                     and t1.MOrgID = @MOrgID   \r\n                     and t1.MIsDelete = 0   \r\n                     and t1.MIsActive = 0";
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				}
			};
			if (type.HasValue)
			{
				text += " and t1.MDocType = @MType ";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MType",
					Value = (object)type.Value
				});
			}
			if (year.HasValue)
			{
				text += " and t2.MYear = @MYear ";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MYear",
					Value = (object)year.Value
				});
			}
			if (period.HasValue)
			{
				text += " and t2.MPeriod = @MPeriod ";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MPeriod",
					Value = (object)period.Value
				});
			}
			if (status.HasValue)
			{
				text += " and t1.MStatus = @MStatus ";
				list.Add(new MySqlParameter
				{
					ParameterName = "@MStatus",
					Value = (object)status.Value
				});
			}
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, text, list.ToArray());
		}

		private int GetBillModuleByType(GLDocTypeEnum type)
		{
			switch (type)
			{
			case GLDocTypeEnum.Invoice:
				return 0;
			case GLDocTypeEnum.Bill:
				return 1;
			case GLDocTypeEnum.Expense:
				return 2;
			case GLDocTypeEnum.Receive:
				return 6;
			case GLDocTypeEnum.Payment:
				return 7;
			case GLDocTypeEnum.Transfer:
				return 8;
			default:
				return 4;
			}
		}

		public List<GLVoucherModel> GenerateVoucherByBill(MContext ctx, List<object> bills, GLDocTypeEnum type, ref List<BizVerificationInfor> result)
		{
			List<GLVoucherModel> list = new List<GLVoucherModel>();
			Generate generateMethod = GetGenerateMethod(type);
			for (int i = 0; i < bills.Count; i++)
			{
				List<GLVoucherModel> list2 = new List<GLVoucherModel>();
				List<GLDocVoucherModel> docVoucherList = new List<GLDocVoucherModel>();
				generateMethod(ctx, bills[i], ref list2, ref docVoucherList);
				GLUtility gLUtility = new GLUtility();
				for (int j = 0; j < list2.Count; j++)
				{
					List<BizVerificationInfor> list3 = gLUtility.CheckVoucherCheckGroupValueMatchCheckGroup(ctx, list2[j], true);
					if (list3?.Any() ?? false)
					{
						result.AddRange(list3);
					}
				}
				GLVoucherModel gLVoucherModel = Merge2OneVoucher(ctx, list2, docVoucherList, 2);
				CalculateVoucherTotal(gLVoucherModel);
				gLVoucherModel.MStatus = 0;
				gLVoucherModel.MSourceBillKey = 1.ToString();
				foreach (GLVoucherEntryModel mVoucherEntry in gLVoucherModel.MVoucherEntrys)
				{
					if (mVoucherEntry.MExplanation.Contains('·'))
					{
						mVoucherEntry.MExplanation = string.Join(";", mVoucherEntry.MExplanation.Split('·').Distinct());
					}
				}
				list.Add(gLVoucherModel);
			}
			SetVoucherListNumbers(ctx, ref list);
			return list;
		}

		private void CalculateVoucherTotal(GLVoucherModel voucher)
		{
			decimal debitTotal = default(decimal);
			decimal creditTotal = default(decimal);
			voucher.MVoucherEntrys.ForEach(delegate(GLVoucherEntryModel x)
			{
				debitTotal += x.MDebit;
				creditTotal += x.MCredit;
			});
			voucher.MDebitTotal = debitTotal;
			voucher.MCreditTotal = creditTotal;
		}

		private List<MIDEntryID> BindDataSet2Model(DataSet ds)
		{
			List<MIDEntryID> list = new List<MIDEntryID>();
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = ds.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new MIDEntryID
					{
						MID = dataTable.Rows[i]["MID"].ToString(),
						MEntryID = dataTable.Rows[i]["MEntryID"].ToString()
					});
				}
			}
			return list;
		}

		private GLDocVoucherModel Clone(MContext ctx, GLDocVoucherModel real)
		{
			return new GLDocVoucherModel
			{
				MItemID = real.MItemID,
				MDocID = real.MDocID,
				MVoucherID = real.MVoucherID,
				MEntryID = real.MEntryID,
				MDocType = real.MDocType,
				MDebitEntryID = real.MDebitEntryID,
				MCreditEntryID = real.MCreditEntryID,
				MTaxEntryID = real.MTaxEntryID,
				MStatus = real.MStatus,
				MergeStatus = real.MergeStatus,
				MIsDelete = real.MIsDelete,
				MOrgID = real.MOrgID
			};
		}

		private List<GLDocVoucherModel> Clone(MContext ctx, List<GLDocVoucherModel> realList)
		{
			List<GLDocVoucherModel> list = new List<GLDocVoucherModel>();
			if (realList == null || !realList.Any())
			{
				return list;
			}
			for (int i = 0; i < realList.Count; i++)
			{
				list.Add(Clone(ctx, realList[i]));
			}
			return list;
		}

		private GLVoucherEntryModel Clone(MContext ctx, GLVoucherEntryModel real)
		{
			return new GLVoucherEntryModel
			{
				MID = real.MID,
				MOrgID = real.MOrgID,
				MEntryID = real.MEntryID,
				MExplanation = real.MExplanation,
				MAccountID = real.MAccountID,
				MAmount = real.MAmount,
				MAmountFor = real.MAmountFor,
				MCheckGroupValueID = real.MCheckGroupValueID,
				MCheckGroupValueModel = real.MCheckGroupValueModel,
				MCurrencyID = real.MCurrencyID,
				MExchangeRate = real.MExchangeRate,
				MDC = real.MDC,
				MDebit = real.MDebit,
				MCredit = real.MCredit,
				MEntrySeq = real.MEntrySeq,
				MSideEntrySeq = real.MSideEntrySeq
			};
		}

		private List<GLVoucherEntryModel> Clone(MContext ctx, List<GLVoucherEntryModel> realList)
		{
			List<GLVoucherEntryModel> list = new List<GLVoucherEntryModel>();
			for (int i = 0; i < realList.Count; i++)
			{
				list.Add(Clone(ctx, realList[i]));
			}
			return list;
		}

		private GLVoucherModel Clone(MContext ctx, GLVoucherModel real)
		{
			return new GLVoucherModel
			{
				MItemID = real.MItemID,
				MOrgID = real.MOrgID,
				MDate = real.MDate,
				MYear = real.MYear,
				MPeriod = real.MPeriod,
				MNumber = real.MNumber,
				MTransferTypeID = real.MTransferTypeID,
				MVoucherGroupID = real.MVoucherGroupID,
				MVoucherGroupNo = real.MVoucherGroupNo,
				MSourceBillKey = real.MSourceBillKey,
				MRVoucherID = real.MRVoucherID,
				MAttachments = real.MAttachments,
				MReference = real.MReference,
				MInternalIND = real.MInternalIND,
				MDebitTotal = real.MDebitTotal,
				MCreditTotal = real.MCreditTotal,
				MStatus = real.MStatus,
				MAuditorID = real.MAuditorID,
				MAuditDate = real.MAuditDate,
				MVoucherEntrys = Clone(ctx, real.MVoucherEntrys),
				MRowIndex = real.MRowIndex
			};
		}

		private GLVoucherModel GetTempVoucherModel(MContext ctx, int year, int period, DateTime date)
		{
			return new GLVoucherModel
			{
				MItemID = JieNor.Megi.Core.UUIDHelper.GetGuid(),
				IsNew = true,
				MYear = year,
				MPeriod = period,
				MDate = date,
				MNumber = null,
				MStatus = -1,
				MOrgID = ctx.MOrgID,
				MSourceBillKey = 2.ToString(),
				MVoucherEntrys = new List<GLVoucherEntryModel>()
			};
		}

		private GLDocVoucherModel GetTempDocVoucherModel(MContext ctx, string entryID, string docID, int type)
		{
			return new GLDocVoucherModel
			{
				MDocID = docID,
				MEntryID = entryID,
				MDocType = type,
				MOrgID = ctx.MOrgID
			};
		}

		private List<GLDocVoucherModel> GetBeMergedDocVouchers(MContext ctx, List<string> docIds)
		{
			if (docIds == null || !docIds.Any())
			{
				return new List<GLDocVoucherModel>();
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string sql = " select * from t_gl_doc_voucher where MOrgID = @MOrgID and MIsDelete = 0 and MergeStatus != 1 and MDocID " + GLUtility.GetInFilterQuery(docIds, ref list, "M_ID");
			return ModelInfoManager.GetDataModelBySql<GLDocVoucherModel>(ctx, sql, list.ToArray());
		}

		private OperationResult CheckIsDeleted(MContext ctx, List<string> vouchids)
		{
			OperationResult operationResult = new OperationResult();
			if (vouchids == null || vouchids.Count == 0)
			{
				operationResult.Success = true;
				return operationResult;
			}
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			string strSql = "select case when count(1) = @VoucherCount then 0 else 1 end from t_gl_voucher \r\n                        where MOrgID = @MOrgID and MIsDelete = 0 and MItemID " + GLUtility.GetInFilterQuery(vouchids, ref list, "M_ID");
			list.Add(new MySqlParameter("@VoucherCount", vouchids.Count));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			if (dynamicDbHelperMySQL.Exists(strSql, list.ToArray()))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataChanged", "数据已在其它页面被修改,请刷新后重试!");
			}
			return operationResult;
		}

		private List<CommandInfo> GetCreateDocVoucherByDocsCmds(MContext ctx, List<GLDocEntryVoucherModel> entryList, OperationResult result)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<IGrouping<string, GLDocEntryVoucherModel>> list2 = (from x in entryList
			group x by x.MDocID).ToList();
			List<GLDocEntryVoucherModel> list3 = new List<GLDocEntryVoucherModel>();
			List<GLDocEntryVoucherModel> list4 = new List<GLDocEntryVoucherModel>();
			List<string> list5 = new List<string>();
			foreach (IGrouping<string, GLDocEntryVoucherModel> item in list2)
			{
				bool canCreate = true;
				string key = item.Key;
				List<GLDocEntryVoucherModel> list6 = item.ToList();
				list6.ForEach(delegate(GLDocEntryVoucherModel x)
				{
					canCreate = (canCreate && x.CanCreateVoucher);
				});
				if (canCreate)
				{
					list3.AddRange(list6);
					list5.Add(key);
				}
				else
				{
					list4.AddRange(list6);
				}
			}
			if (list3 != null && list3.Count > 0)
			{
				OperationResult operationResult = TransferVouchersDraft2SavedByBillIds(ctx, list5, list3, true);
				if (operationResult.Success)
				{
					list = operationResult.OperationCommands;
				}
				else
				{
					result.Success = false;
					result.Message = operationResult.Message;
				}
			}
			if (list4 != null && list4.Count > 0)
			{
				List<CommandInfo> saveDocVoucherOneByOne = GetSaveDocVoucherOneByOne(ctx, list4, true);
				list.AddRange(saveDocVoucherOneByOne);
			}
			return list;
		}

		private GLDocVoucherModel DocEntryModel2DocVoucherModel(GLDocEntryVoucherModel model)
		{
			return new GLDocVoucherModel
			{
				MItemID = model.MDocVoucherID,
				MEntryID = model.MEntryID,
				MDocType = model.MDocType,
				MVoucherID = model.MVoucherID,
				MDebitEntryID = model.MDebitEntryID,
				MCreditEntryID = model.MCreditEntryID,
				MergeStatus = model.MergeStatus,
				MTaxEntryID = model.MTaxEntryID,
				MStatus = model.MDocStatus,
				MDocID = model.MDocID,
				MOrgID = model.MOrgID
			};
		}
	}
}
