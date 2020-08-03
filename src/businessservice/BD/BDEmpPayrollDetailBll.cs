using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.PA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDEmpPayrollDetailBll : IBDEmpPayrollDetailBll
	{
		private BDEmpPayrollDetailRepository dal = new BDEmpPayrollDetailRepository();

		private PASalaryPaymentBusiness bizSalaryPayment = new PASalaryPaymentBusiness();

		private PASalaryPaymentRepository dalSalaryPayment = new PASalaryPaymentRepository();

		public List<CommandInfo> GetEmpPayrollDetailUpdateCmdList(MContext ctx, BDPayrollDetailModel model, OperationResult result)
		{
			model.MOrgID = ctx.MOrgID;
			if (string.IsNullOrWhiteSpace(model.MItemID))
			{
				List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere().Equal("MEmployeeID", model.MEmployeeID), false, false);
				if (dataModelList.Any())
				{
					model.MItemID = dataModelList.FirstOrDefault().MItemID;
				}
			}
			COMModelValidateHelper.ValidateModel(ctx, model, result);
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<BDPayrollDetailModel>(ctx, model, null, true);
			if (model.PITThresholdList != null && model.PITThresholdList.Any())
			{
				List<PAPITThresholdModel> pITThresholdList = PAPITRepository.GetPITThresholdList(ctx, new PAPITThresholdFilterModel
				{
					EmployeeID = model.MEmployeeID
				});
				List<PASalaryPaymentModel> employeeSalaryPaymentList = PASalaryPaymentRepository.GetEmployeeSalaryPaymentList(ctx, model.MEmployeeID, 1);
				foreach (PAPITThresholdModel pITThreshold in model.PITThresholdList)
				{
					if (pITThreshold.MAmount != pITThreshold.MDefaultAmount)
					{
						pITThreshold.MOrgID = ctx.MOrgID;
						COMModelValidateHelper.ValidateModel(ctx, pITThreshold, result);
						insertOrUpdateCmd.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPITThresholdModel>(ctx, pITThreshold, null, true));
					}
					else if (!string.IsNullOrWhiteSpace(pITThreshold.MItemID))
					{
						insertOrUpdateCmd.AddRange(ModelInfoManager.GetDeleteFlagCmd<PAPITThresholdModel>(ctx, pITThreshold.MItemID));
					}
					AddSalaryPaymentPITUpdateCmdList(ctx, model, insertOrUpdateCmd, pITThresholdList, employeeSalaryPaymentList, pITThreshold);
				}
			}
			return insertOrUpdateCmd;
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDPayrollDetailModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> empPayrollDetailUpdateCmdList = GetEmpPayrollDetailUpdateCmdList(ctx, model, operationResult);
			if (string.IsNullOrEmpty(model.MEmployeeID))
			{
				throw new NullReferenceException("employeeid can not null");
			}
			if (!operationResult.Success)
			{
				return operationResult;
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(empPayrollDetailUpdateCmdList) > 0);
			return operationResult;
		}

		private void AddSalaryPaymentPITUpdateCmdList(MContext ctx, BDPayrollDetailModel empPayrollDetail, List<CommandInfo> cmdList, List<PAPITThresholdModel> empPITList, List<PASalaryPaymentModel> empDraftSalaryList, PAPITThresholdModel pitModel)
		{
			if (empDraftSalaryList.Any())
			{
				List<PASalaryPaymentModel> list = new List<PASalaryPaymentModel>();
				PAPITThresholdModel pAPITThresholdModel = empPITList.FirstOrDefault((PAPITThresholdModel f) => f.MEffectiveDate == pitModel.MEffectiveDate);
				if (pitModel.MAmount != pAPITThresholdModel.MAmount)
				{
					PAPITThresholdModel nextPeriod = (from f in empPayrollDetail.PITThresholdList
					orderby f.MEffectiveDate
					select f).FirstOrDefault((PAPITThresholdModel f) => f.MEffectiveDate > pitModel.MEffectiveDate);
					DateTime startDate = (pitModel.MEffectiveDate == new DateTime(2011, 9, 1)) ? DateTime.MinValue : pitModel.MEffectiveDate;
					list = (from f in empDraftSalaryList
					where f.MDate >= startDate
					select f).ToList();
					if (nextPeriod != null)
					{
						list = (from f in empDraftSalaryList
						where f.MDate < nextPeriod.MEffectiveDate
						select f).ToList();
					}
					if (list.Any())
					{
						PAPITCalculateUtility pAPITCalculateUtility = new PAPITCalculateUtility(ctx, pitModel.MEffectiveDate, false, pitModel.MAmount);
						foreach (PASalaryPaymentModel item in list)
						{
							item.MTaxSalary = pAPITCalculateUtility.CalculateSalaryPIT(pitModel.MEmployeeID, item.MNetSalary + item.MTaxSalary);
						}
						cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, new List<string>
						{
							"MTaxSalary"
						}, false));
					}
				}
			}
		}

		public BDPayrollDetailModel GetModel(MContext ctx, string employeeID)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MOrgID ", ctx.MOrgID);
			sqlWhere.Equal(" MEmployeeID ", employeeID);
			return dal.GetDataModelByFilter(ctx, sqlWhere);
		}

		public List<BDPayrollDetailModel> GetList(MContext ctx, string employeeIds)
		{
			return dal.GetModelList(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MEmployeeID", employeeIds.Split(',')), true);
		}
	}
}
