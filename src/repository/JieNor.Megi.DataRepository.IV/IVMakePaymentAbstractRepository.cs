using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.IV
{
	public abstract class IVMakePaymentAbstractRepository
	{
		protected BillSourceType _sourceType = BillSourceType.Normal;

		protected CreateFromType _createFrom = CreateFromType.Normal;

		public abstract MakePaymentResultModel GetToPayResult(MContext ctx, IVMakePaymentModel makePaymentModel, BDBankAccountEditModel bankModel);

		public abstract MakePaymentResultModel GetToPayResults(MContext ctx, List<IVMakePaymentModel> makePaymentModel, BDBankAccountEditModel bankModel);

		public static IVMakePaymentAbstractRepository CreateInstance(string selectObj)
		{
			if (!(selectObj == "PayRun"))
			{
				if (selectObj == "Expense")
				{
					return new IVExpenseMakePaymentRepository();
				}
				return new IVInvoiceMakePaymentRepository();
			}
			return new IVSalaryMakePaymentRepository();
		}

		public OperationResult ToPay(MContext ctx, IVMakePaymentModel makePayment)
		{
			return ToPay(ctx, new List<IVMakePaymentModel>
			{
				makePayment
			});
		}

		public OperationResult ToPay(MContext ctx, List<IVMakePaymentModel> makePaymentList)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			operationResult.Success = true;
			if (makePaymentList == null || makePaymentList.Count == 0)
			{
				operationResult.Success = false;
				return operationResult;
			}
			string empty = string.Empty;
			list.AddRange(GetToPayCmdList(ctx, makePaymentList, ref empty));
			if (!string.IsNullOrWhiteSpace(empty))
			{
				operationResult.Success = false;
				operationResult.Message = empty;
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			operationResult.Success = (num > 0);
			return operationResult;
		}

		public List<CommandInfo> GetToPayCmdList(MContext ctx, List<IVMakePaymentModel> makePaymentList, ref string errorMsg)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			if (makePaymentList == null || makePaymentList.Count == 0)
			{
				return list;
			}
			BDBankAccountEditModel bankModel = GetBankModel(ctx, makePaymentList[0].MBankID);
			MakePaymentResultModel toPayResults = GetToPayResults(ctx, makePaymentList, bankModel);
			if (!toPayResults.MSuccess || toPayResults.MCommand == null)
			{
				errorMsg = $"{errorMsg}{toPayResults.MMessage}\n";
			}
			if (toPayResults.MCommand != null)
			{
				list.AddRange(toPayResults.MCommand);
			}
			if (!string.IsNullOrEmpty(errorMsg))
			{
				errorMsg = string.Join("\n", errorMsg.Split('\n').Distinct().ToList());
			}
			if (!string.IsNullOrWhiteSpace(errorMsg))
			{
				return new List<CommandInfo>();
			}
			return list;
		}

		protected BDBankAccountEditModel GetBankModel(MContext ctx, string bankId)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			return bDBankAccountRepository.GetBDBankAccountEditModel(ctx, bankId);
		}

		protected void CheckBank(MContext ctx, BDBankAccountEditModel bankModel, out bool success, out string message)
		{
			success = true;
			message = "";
			if (bankModel == null || string.IsNullOrEmpty(bankModel.MItemID))
			{
				success = false;
				message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "TheBankIsNotExists!", "The Bank is not exists!");
			}
		}

		protected static void CheckCurrency(MContext ctx, string bankCurrency, string billCurrency, out bool success, out string message, int rowIndex = 0)
		{
			success = true;
			message = "";
			if (bankCurrency != billCurrency)
			{
				success = false;
				if (rowIndex > 0)
				{
					message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ErrorRowNo", "Row {0}: "), rowIndex);
					message += COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExpenseCurrencyNotMathWithBank", "The currency of the expense claim is inconsistent with the bank account which you selected!");
				}
				else
				{
					message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "CurrencyNotMath", "Currency not math!");
				}
			}
		}

		protected void CheckPaidAccount(MContext ctx, decimal totalAmtFor, decimal verificationAmtFor, decimal toPaidAmtFor, out bool success, out string message, bool includeZero = false)
		{
			success = true;
			message = "";
			decimal num = Math.Abs(totalAmtFor) - Math.Abs(verificationAmtFor);
			if ((num == decimal.Zero && !includeZero) || toPaidAmtFor > num)
			{
				success = false;
				message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PaidToAmountError", "Paid Amount Error!");
			}
		}

		protected string GetPaymentBizType(string bizType)
		{
			switch (bizType)
			{
			case "Invoice_Sale":
				return "Receive_Sale";
			case "Invoice_Sale_Red":
				return "Pay_PurReturn";
			case "Invoice_Purchase":
				return "Pay_Purchase";
			case "Invoice_Purchase_Red":
				return "Receive_SaleReturn";
			case "Expense_Claims":
			case "Pay_Salary":
				return "Pay_Other";
			default:
				return "";
			}
		}

		protected IVVerificationModel GetVerificationModel(MContext ctx, string srcBillId, string srcbillType, string tgtBillId, string tgtBizObject, decimal paidAmtFor, decimal paidAmt)
		{
			IVVerificationModel iVVerificationModel = new IVVerificationModel();
			iVVerificationModel.MSourceBillID = srcBillId;
			iVVerificationModel.MSourceBillType = srcbillType;
			iVVerificationModel.MSourceBillEntryID = "";
			iVVerificationModel.MTargetBillID = tgtBillId;
			iVVerificationModel.MTargetBillType = tgtBizObject;
			iVVerificationModel.MTargetBillEntryID = "";
			iVVerificationModel.MAmount = paidAmtFor;
			iVVerificationModel.MAmtFor = paidAmtFor;
			iVVerificationModel.MAmt = paidAmt;
			iVVerificationModel.MDirection = 1;
			iVVerificationModel.MIsDelete = false;
			iVVerificationModel.MCreatorID = ctx.MUserID;
			iVVerificationModel.MCreateDate = ctx.DateNow;
			iVVerificationModel.MSource = Convert.ToInt32(_sourceType);
			return iVVerificationModel;
		}

		protected MultiDBCommand[] GetCmd(MContext ctx, List<CommandInfo> cmdList, MultiDBCommand[] multiCmdList, CommandInfo logCmd, CommandInfo unlockCmd)
		{
			MultiDBCommand[] array = new MultiDBCommand[3 + multiCmdList.Length];
			array[0] = new MultiDBCommand(ctx)
			{
				DBType = SysOrBas.Bas,
				CommandList = cmdList
			};
			for (int i = 1; i <= multiCmdList.Length; i++)
			{
				array[i] = multiCmdList[i - 1];
			}
			array[array.Length - 2] = new MultiDBCommand(ctx)
			{
				DBType = SysOrBas.Bas,
				CommandList = new List<CommandInfo>
				{
					logCmd
				}
			};
			array[array.Length - 1] = new MultiDBCommand(ctx)
			{
				DBType = SysOrBas.Bas,
				CommandList = new List<CommandInfo>
				{
					unlockCmd
				}
			};
			return array;
		}
	}
}
