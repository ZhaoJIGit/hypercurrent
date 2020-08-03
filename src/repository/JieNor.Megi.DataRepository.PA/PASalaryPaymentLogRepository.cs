using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.PA
{
	public class PASalaryPaymentLogRepository
	{
		public static void AddSalaryPaymentEditLog(MContext ctx, PASalaryPaymentModel model)
		{
			OptLogTemplate optLogTemplate = OptLogTemplate.None;
			if (model.IsNew)
			{
				optLogTemplate = GetSalaryPaymentLogAddTemplate();
				AddSalaryPaymentEditLog(ctx, optLogTemplate, model);
				if (model.MStatus == Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment))
				{
					optLogTemplate = GetSalaryPaymentLogApproveTemplate();
					AddSalaryPaymentEditLog(ctx, optLogTemplate, model);
				}
			}
			else
			{
				optLogTemplate = ((model.MStatus != Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment)) ? GetSalaryPaymentLogEditTemplate() : GetSalaryPaymentLogApproveTemplate());
				AddSalaryPaymentEditLog(ctx, optLogTemplate, model);
			}
		}

		public static void AddSalaryPaymentDeleteLog(MContext ctx, ParamBase param)
		{
			string[] values = param.KeyIDs.Split(',');
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MID", SqlOperators.In, values);
			List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, sqlWhere, false, false);
			if (dataModelList.Count != 0)
			{
				foreach (PASalaryPaymentModel item in dataModelList)
				{
					OptLogTemplate salaryPaymentLogDeleteTemplate = GetSalaryPaymentLogDeleteTemplate();
					OptLog.AddLog(salaryPaymentLogDeleteTemplate, ctx, item.MID, item.MReference);
				}
			}
		}

		public static void AddSalaryPaymentApprovalLog(MContext ctx, ParamBase param)
		{
			if (param.MOperationID.ToMInt32() == Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment))
			{
				string[] values = param.KeyIDs.Split(',');
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MID", SqlOperators.In, values);
				List<PASalaryPaymentModel> dataModelList = ModelInfoManager.GetDataModelList<PASalaryPaymentModel>(ctx, sqlWhere, false, false);
				if (dataModelList.Count != 0)
				{
					foreach (PASalaryPaymentModel item in dataModelList)
					{
						OptLogTemplate salaryPaymentLogApproveTemplate = GetSalaryPaymentLogApproveTemplate();
						AddSalaryPaymentEditLog(ctx, salaryPaymentLogApproveTemplate, item);
					}
				}
			}
		}

		public static void AddSalaryPaymentExpectedInfoLog(MContext ctx, IVInvoiceModel model)
		{
			OptLogTemplate template = OptLogTemplate.SalaryPayment_NoteExpectedDate;
			OptLog.AddLog(template, ctx, model.MID, model.MDesc, model.MExpectedDate);
		}

		public static void AddSalaryPaymentPaidLog(MContext ctx, string id, decimal veriAccount)
		{
			PASalaryPaymentModel dataEditModel = ModelInfoManager.GetDataEditModel<PASalaryPaymentModel>(ctx, id, false, true);
			if (dataEditModel != null)
			{
				decimal num = dataEditModel.MNetSalary - dataEditModel.MVerificationAmt;
				OptLog.AddLog(OptLogTemplate.SalaryPayment_Paid, ctx, dataEditModel.MID, dataEditModel.MEmployeeID, ctx.DateNow, Math.Abs(veriAccount).To2Decimal());
			}
		}

		public static CommandInfo GetAddSalaryPaymentPaidLogCmd(MContext ctx, string id, DateTime paidDate, decimal veriAccount)
		{
			PASalaryPaymentModel dataEditModel = ModelInfoManager.GetDataEditModel<PASalaryPaymentModel>(ctx, id, false, true);
			if (dataEditModel == null)
			{
				return null;
			}
			return OptLog.GetAddLogCommand(OptLogTemplate.SalaryPayment_Paid, ctx, dataEditModel.MID, dataEditModel.MEmployeeID, paidDate, Math.Abs(veriAccount).To2Decimal());
		}

		public static void AddSalaryPaymentNoteLog(MContext ctx, PASalaryPaymentModel model)
		{
			OptLogTemplate salaryPaymentLogNoteTemplate = GetSalaryPaymentLogNoteTemplate();
			OptLog.AddLog(salaryPaymentLogNoteTemplate, ctx, model.MID, model.MReference);
		}

		private static void AddSalaryPaymentEditLog(MContext ctx, OptLogTemplate logTemplate, PASalaryPaymentModel model)
		{
			OptLog.AddLog(logTemplate, ctx, model.MID, model.MReference, model.MEmployeeID, Math.Abs(model.MNetSalary).To2Decimal());
		}

		public static void AddSalaryPaymentUnApproveLog(MContext ctx, PASalaryPaymentModel model)
		{
			OptLogTemplate template = OptLogTemplate.SalaryPayment_Reverse_Approved;
			OptLog.AddLog(template, ctx, model.MID, model.MDateFormat, model.MEmployeeID, Math.Abs(model.MNetSalary));
		}

		private static OptLogTemplate GetSalaryPaymentLogAddTemplate()
		{
			return OptLogTemplate.SalaryPayment_Created;
		}

		private static OptLogTemplate GetSalaryPaymentLogApproveTemplate()
		{
			return OptLogTemplate.SalaryPayment_Approved;
		}

		private static OptLogTemplate GetSalaryPaymentLogEditTemplate()
		{
			return OptLogTemplate.SalaryPayment_Edited;
		}

		private static OptLogTemplate GetSalaryPaymentLogDeleteTemplate()
		{
			return OptLogTemplate.SalaryPayment_Deleted;
		}

		private static OptLogTemplate GetSalaryPaymentLogNoteTemplate()
		{
			return OptLogTemplate.SalaryPayment_Note;
		}
	}
}
