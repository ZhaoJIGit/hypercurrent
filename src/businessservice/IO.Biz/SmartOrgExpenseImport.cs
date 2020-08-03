using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log.GlLog;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public class SmartOrgExpenseImport : ExpenseImport
	{
		public override ImportResult ImportData(MContext ctx, IOImportDataModel data)
		{
			ImportResult importResult = new ImportResult();
			importResult.Success = true;
			List<BizVerificationInfor> list = ValidateDataTable(ctx, data.EffectiveData);
			if (list != null)
			{
				importResult.FeedbackInfo.AddRange(list);
			}
			base.SetCacheData(ctx);
			List<IVExpenseModel> list2 = new List<IVExpenseModel>();
			list2 = ConvertToExpenseList(ctx, data.EffectiveData);
			if (list2 == null || list2.Count == 0)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoDataFoundInImportingFile", "导入的文件里面没有数据");
				importResult.MessageList = new List<string>
				{
					text
				};
				importResult.Success = false;
				return importResult;
			}
			list2.First().TrackItemNameList = data.TrackItemNameList;
			List<BizVerificationInfor> list3 = ValidateExpense(ctx, list2);
			if (list3 != null && list3.Count > 0)
			{
				importResult.FeedbackInfo.AddRange(list3);
			}
			if (importResult.FeedbackInfo != null && importResult.FeedbackInfo.Count > 0)
			{
				importResult.Success = false;
				return importResult;
			}
			List<CommandInfo> list4 = new List<CommandInfo>();
			List<string> list5 = new List<string>();
			List<object> list6 = new List<object>();
			list6.AddRange(list2);
			List<BizVerificationInfor> list7 = new List<BizVerificationInfor>();
			List<GLVoucherModel> list8 = new GLDocVoucherRepository().GenerateVoucherByBill(ctx, list6, GLDocTypeEnum.Expense, ref list7);
			if (list7?.Any() ?? false)
			{
				importResult.Success = false;
				importResult.FeedbackInfo.AddRange(list7);
			}
			List<BizVerificationInfor> list9 = new GLVoucherBusiness().ValidateVoucher(ctx, list8);
			if (list9 != null && list9.Count > 0)
			{
				importResult.Success = false;
				importResult.FeedbackInfo.AddRange(list9);
			}
			if (!importResult.Success)
			{
				return importResult;
			}
			foreach (GLVoucherModel item in list8)
			{
				list4.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<GLVoucherModel>(ctx, item, null, true));
				list4.AddRange(GlVoucherLogHelper.GetSaveLog(ctx, item));
				IVExpenseModel expense = list2.First((IVExpenseModel x) => x.MRowIndex == item.MRowIndex);
				List<BizVerificationInfor> voucherSuccessInfo = GetVoucherSuccessInfo(ctx, expense, item);
				importResult.FeedbackInfo.AddRange(voucherSuccessInfo);
				list5.AddRange(GetSuccessRowIndex(expense));
			}
			importResult.SuccessRowIndexes = string.Join(",", list5);
			int num = 0;
			if (list4 != null && list4.Count > 0)
			{
				num = BDRepository.ExecuteSqlTran(ctx, list4);
			}
			importResult.Success = (num > 0);
			importResult.FeedbackInfo = ((!importResult.Success && importResult.FeedbackInfo != null) ? null : importResult.FeedbackInfo);
			return importResult;
		}

		private List<BizVerificationInfor> ValidateExpense(MContext ctx, List<IVExpenseModel> expenseList)
		{
			List<BizVerificationInfor> result = new List<BizVerificationInfor>();
			foreach (IVExpenseModel expense in expenseList)
			{
				int mRowIndex;
				if (string.IsNullOrWhiteSpace(expense.MEmployee))
				{
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EmployeeIsRequire", "员工为必填项！");
					List<BizVerificationInfor> list = result;
					BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
					mRowIndex = expense.MRowIndex;
					bizVerificationInfor.Id = mRowIndex.ToString();
					bizVerificationInfor.Message = text;
					bizVerificationInfor.CheckItem = "RowIndex";
					bizVerificationInfor.RowIndex = expense.MRowIndex;
					list.Add(bizVerificationInfor);
				}
				if (!string.IsNullOrWhiteSpace(expense.MReference) && expense.MReference.Length > 200)
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "RefrerenceTooLength", "备注长度不能大于200字符！");
					List<BizVerificationInfor> list2 = result;
					BizVerificationInfor bizVerificationInfor2 = new BizVerificationInfor();
					mRowIndex = expense.MRowIndex;
					bizVerificationInfor2.Id = mRowIndex.ToString();
					bizVerificationInfor2.Message = text2;
					bizVerificationInfor2.CheckItem = "RowIndex";
					bizVerificationInfor2.RowIndex = expense.MRowIndex;
					list2.Add(bizVerificationInfor2);
				}
				if (expense.MEntryList != null)
				{
					expense.MEntryList.ForEach(delegate(IVExpenseEntryModel x)
					{
						int mRowIndex2;
						if (string.IsNullOrWhiteSpace(x.MItemID))
						{
							string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExpenseItemIsRequire", "费用项目为必填项！");
							List<BizVerificationInfor> list4 = result;
							BizVerificationInfor bizVerificationInfor3 = new BizVerificationInfor();
							mRowIndex2 = x.MRowIndex;
							bizVerificationInfor3.Id = mRowIndex2.ToString();
							bizVerificationInfor3.Message = text3;
							bizVerificationInfor3.CheckItem = "RowIndex";
							bizVerificationInfor3.RowIndex = x.MRowIndex;
							list4.Add(bizVerificationInfor3);
						}
						if (!string.IsNullOrWhiteSpace(x.MDesc) && x.MDesc.Length > 200)
						{
							string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DescTooLength", "描述长度不能大于200字符！");
							List<BizVerificationInfor> list5 = result;
							BizVerificationInfor bizVerificationInfor4 = new BizVerificationInfor();
							mRowIndex2 = x.MRowIndex;
							bizVerificationInfor4.Id = mRowIndex2.ToString();
							bizVerificationInfor4.Message = text4;
							bizVerificationInfor4.CheckItem = "RowIndex";
							bizVerificationInfor4.RowIndex = x.MRowIndex;
							list5.Add(bizVerificationInfor4);
						}
					});
				}
			}
			IVExpenseBusiness iVExpenseBusiness = new IVExpenseBusiness();
			List<BizVerificationInfor> list3 = iVExpenseBusiness.ValidateExpense(ctx, expenseList);
			if (list3 != null)
			{
				result.AddRange(list3);
			}
			expenseList.ForEach(delegate(IVExpenseModel x)
			{
				x.MContactID = x.MEmployee;
			});
			return result;
		}

		private List<BizVerificationInfor> ValidateDataTable(MContext ctx, DataTable dt)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			foreach (DataRow row in dt.Rows)
			{
				string text = row.Field<string>("MBizDate");
				string text2 = row.Field<string>("MRowIndex");
				if (!string.IsNullOrWhiteSpace(text))
				{
					string[] formats = new string[3]
					{
						"yyyy-MM-dd",
						"dd-MM-yyyy",
						"dd/MM/yyyy"
					};
					if (!DateTime.TryParse(text, out DateTime dateTime) && !DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
					{
						string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DateTimeFormatterError", "日期格式错误，请确保格式为yyyy-MM-dd dd-MM-yyyy dd/MM/yyyy！");
						list.Add(new BizVerificationInfor
						{
							Id = text2,
							Message = text3,
							CheckItem = "RowIndex",
							RowIndex = int.Parse(text2)
						});
					}
				}
				string text4 = row.Field<string>("MPrice");
				if (!string.IsNullOrWhiteSpace(text4) && !decimal.TryParse(text4, out decimal _))
				{
					string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PriceFormatterError", "单价只能为数字");
					list.Add(new BizVerificationInfor
					{
						Id = text2,
						Message = text5,
						CheckItem = "RowIndex",
						RowIndex = int.Parse(text2)
					});
					row["MPrice"] = "2";
				}
				string text6 = row.Field<string>("MQty");
				if (!string.IsNullOrWhiteSpace(text6) && !decimal.TryParse(text6, out decimal _))
				{
					string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "QuantityFormatterError", "数量只能为数字");
					list.Add(new BizVerificationInfor
					{
						Id = text2,
						Message = text7,
						CheckItem = "RowIndex",
						RowIndex = int.Parse(text2)
					});
					row["MQty"] = "1";
				}
				string text8 = row.Field<string>("MTaxAmtFor");
				if (!string.IsNullOrWhiteSpace(text8) && !decimal.TryParse(text8, out decimal _))
				{
					string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "taxAmountFormatterError", "税额只能为数字");
					list.Add(new BizVerificationInfor
					{
						Id = text2,
						Message = text9,
						CheckItem = "RowIndex",
						RowIndex = int.Parse(text2)
					});
					row["MTaxAmtFor"] = "1";
				}
			}
			return list;
		}

		private OperationResult ConvertMActionException(MContext ctx, OperationResult sourceResult)
		{
			if (sourceResult == null || sourceResult.VerificationInfor == null || sourceResult.VerificationInfor.Count == 0)
			{
				return sourceResult;
			}
			OperationResult operationResult = new OperationResult();
			foreach (BizVerificationInfor item in sourceResult.VerificationInfor)
			{
				if (item.Exception == null || item.Exception == null)
				{
					operationResult.VerificationInfor.Add(item);
				}
				else
				{
					GLInterfaceRepository.HandleActionException(ctx, operationResult, item.Exception, true);
				}
			}
			return operationResult;
		}

		private List<BizVerificationInfor> GetVoucherSuccessInfo(MContext ctx, IVExpenseModel expense, GLVoucherModel voucher)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			string mNumber = voucher.MNumber;
			foreach (int mRowIndex in expense.MRowIndexList)
			{
				BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
				bizVerificationInfor.ExtendField = "MNumber";
				bizVerificationInfor.RowIndex = mRowIndex;
				bizVerificationInfor.Message = "GL-" + mNumber;
				bizVerificationInfor.DisplayType = 2;
				list.Add(bizVerificationInfor);
			}
			return list;
		}

		private List<string> GetSuccessRowIndex(IVExpenseModel expense)
		{
			List<string> list = new List<string>();
			if (expense.MRowIndexList != null)
			{
				list.AddRange(from x in expense.MRowIndexList
				select x.ToString());
			}
			return list;
		}

		private List<IVExpenseModel> ConvertToExpenseList(MContext ctx, DataTable dt)
		{
			List<IVExpenseModel> list = new List<IVExpenseModel>();
			List<IOExpenseImportModel> list2 = base.ConvertToList<IOExpenseImportModel>(dt);
			List<IOExpenseImportModel> mainExpenseList = base.GetMainExpenseList(list2);
			foreach (IOExpenseImportModel item in mainExpenseList)
			{
				IVExpenseModel expModel = base.GetExpenseModel(ctx, item, list2);
				if (expModel != null && expModel.MEntryList != null)
				{
					expModel.MEntryList.ForEach(delegate(IVExpenseEntryModel x)
					{
						expModel.MRowIndexList = ((expModel.MRowIndexList == null) ? new List<int>() : expModel.MRowIndexList);
						expModel.MRowIndexList.Add(x.MRowIndex);
					});
					expModel.MEntryList.RemoveAll((IVExpenseEntryModel x) => x.MQty * x.MPrice <= decimal.Zero);
					expModel.MDueDate = expModel.MBizDate;
					expModel.MExpectedDate = expModel.MBizDate;
					list.Add(expModel);
				}
			}
			list.RemoveAll((IVExpenseModel x) => x.MEntryList.Count == 0);
			return list;
		}

		protected override IVExpenseEntryModel ConvertToExpenseEntryModel(MContext ctx, IVExpenseModel expModel, IOExpenseImportModel middleExpenseEntry)
		{
			IVExpenseEntryModel iVExpenseEntryModel = new IVExpenseEntryModel();
			iVExpenseEntryModel.MEntryID = UUIDHelper.GetGuid();
			iVExpenseEntryModel.IsNew = true;
			iVExpenseEntryModel.MItemID = middleExpenseEntry.MItemID;
			iVExpenseEntryModel.MDesc = middleExpenseEntry.MDesc;
			iVExpenseEntryModel.MQty = Math.Round(middleExpenseEntry.MQty, 4);
			iVExpenseEntryModel.MPrice = Math.Round(middleExpenseEntry.MPrice, 4);
			iVExpenseEntryModel.MDebitAccount = middleExpenseEntry.MDebitAccount;
			iVExpenseEntryModel.MCreditAccount = middleExpenseEntry.MCreditAccount;
			iVExpenseEntryModel.MTaxAccount = middleExpenseEntry.MTaxAccount;
			iVExpenseEntryModel.MTrackItem1 = middleExpenseEntry.MTrackItem1;
			iVExpenseEntryModel.MTrackItem2 = middleExpenseEntry.MTrackItem2;
			iVExpenseEntryModel.MTrackItem3 = middleExpenseEntry.MTrackItem3;
			iVExpenseEntryModel.MTrackItem4 = middleExpenseEntry.MTrackItem4;
			iVExpenseEntryModel.MTrackItem5 = middleExpenseEntry.MTrackItem5;
			iVExpenseEntryModel.MRowIndex = middleExpenseEntry.MRowIndex;
			iVExpenseEntryModel.MTaxTypeID = "Tax_Inclusive";
			iVExpenseEntryModel.MTaxAmtFor = Math.Round(middleExpenseEntry.MTaxAmtFor, 2);
			base.SetExpenseEntryInfo(expModel, iVExpenseEntryModel);
			return iVExpenseEntryModel;
		}
	}
}
