using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.PA
{
	public class PAPITCalculateUtility
	{
		private DateTime _salaryDate;

		private List<PAPITTaxRateModel> _pitTaxRateList;

		private List<PAPITThresholdModel> _pitThresholdList;

		private bool _isPITDisabled = false;

		private decimal? _pitAmount;

		private bool _isNewPayslip;

		public PAPITCalculateUtility(MContext ctx, DateTime salaryDate, bool isNewPayslip, decimal? pitAmount = default(decimal?))
		{
			_isNewPayslip = isNewPayslip;
			_salaryDate = salaryDate;
			_pitAmount = pitAmount;
			PAPITRepository.CheckSalaryPeriod(ref _salaryDate);
			_pitThresholdList = PAPITRepository.GetPITThresholdList(ctx, new PAPITThresholdFilterModel());
			_pitTaxRateList = PAPITRepository.GetPITTaxRateList(ctx, salaryDate);
			List<PAPayItemGroupModel> salaryItemGroupList = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.All);
			PAPayItemGroupModel pAPayItemGroupModel = salaryItemGroupList.FirstOrDefault((PAPayItemGroupModel f) => f.MItemType == 3010);
			_isPITDisabled = (pAPayItemGroupModel != null && !pAPayItemGroupModel.MIsActive);
		}

		public decimal CalculateSalaryPIT(string employeeId, decimal netSalary, out decimal pitAmount)
		{
			if (_isPITDisabled && _isNewPayslip)
			{
				pitAmount = default(decimal);
				return decimal.Zero;
			}
			PAPITThresholdModel pAPITThresholdModel = _pitThresholdList.FirstOrDefault((PAPITThresholdModel f) => f.MEmployeeID == employeeId && f.MEffectiveDate <= _salaryDate);
			pitAmount = (_pitAmount.HasValue ? _pitAmount.Value : pAPITThresholdModel.MAmount);
			decimal taxSalary = netSalary - pitAmount;
			PAPITTaxRateModel pAPITTaxRateModel = _pitTaxRateList.FirstOrDefault((PAPITTaxRateModel f) => f.MBeginAmount < taxSalary && f.MEndAmount >= taxSalary);
			if (pAPITTaxRateModel == null)
			{
				return decimal.Zero;
			}
			decimal d = taxSalary * pAPITTaxRateModel.MTaxRate / 100m - pAPITTaxRateModel.MDeductionAmount;
			return Math.Round(d, 2, MidpointRounding.AwayFromZero);
		}

		public decimal CalculateSalaryPIT(string employeeId, decimal netSalary)
		{
			decimal num = default(decimal);
			return CalculateSalaryPIT(employeeId, netSalary, out num);
		}
	}
}
