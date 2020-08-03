using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.PA;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.PA
{
	public class PASalaryPaymentBusiness : BusinessServiceBase, IPASalaryPaymentBusiness
	{
		private readonly BDEmployeesRepository dalEmp = new BDEmployeesRepository();

		public List<PAPITThresholdModel> GetPITThresholdList(MContext ctx, PAPITThresholdFilterModel filter)
		{
			List<PAPITThresholdModel> pITThresholdList = PAPITRepository.GetPITThresholdList(ctx, filter);
			return (from f in pITThresholdList.Take(2)
			orderby f.MEffectiveDate
			select f).ToList();
		}

		public List<PAPayRunListModel> GetPayRunList(MContext ctx, PAPayRunListFilterModel filter)
		{
			new PAPayItemBussiness().InsertCSocialSecurityOther(ctx);
			return PASalaryPaymentRepository.GetPayRunList(ctx, filter);
		}

		public DataGridJson<PAPayRunListModel> GetPayRunListPage(MContext ctx, PAPayRunListFilterModel filter)
		{
			new PAPayItemBussiness().InsertCSocialSecurityOther(ctx);
			return PASalaryPaymentRepository.GetPayRunListPage(ctx, filter);
		}

		public string GetChartStackedDictionary(MContext ctx, string payRunListData)
		{
			return PASalaryPaymentRepository.GetChartStackedDictionary(ctx, payRunListData);
		}

		public List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, DateTime startDate, DateTime endDate)
		{
			return PASalaryPaymentRepository.GetChartPieDictionary(ctx, startDate, endDate);
		}

		public PASalaryPaymentModel GetSalaryPaymentEditModel(MContext ctx, string mid)
		{
			PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, mid);
			PAPITThresholdModel pITThresholdModel = PAPITRepository.GetPITThresholdModel(ctx, salaryPaymentEditModel.MDate, salaryPaymentEditModel.MEmployeeID);
			salaryPaymentEditModel.MPITThresholdAmount = pITThresholdModel.MAmount;
			salaryPaymentEditModel.MPITTaxRateList = PAPITRepository.GetPITTaxRateList(ctx, salaryPaymentEditModel.MDate);
			PASalaryToIVPayEntryModel salaryToIvPayEntryModel = PASalaryPaymentRepository.GetSalaryToIvPayEntryModel(ctx, mid);
			if (salaryPaymentEditModel != null && !string.IsNullOrEmpty(salaryPaymentEditModel.MID))
			{
				IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
				iVVerificationListFilterModel.MBillID = salaryPaymentEditModel.MID;
				iVVerificationListFilterModel.MBizBillType = "PayRun";
				iVVerificationListFilterModel.MBizType = "Pay_Salary";
				iVVerificationListFilterModel.MContactID = salaryPaymentEditModel.MEmployeeID;
				iVVerificationListFilterModel.MCurrencyID = ctx.MBasCurrencyID;
				iVVerificationListFilterModel.MViewVerif = true;
				if (salaryToIvPayEntryModel != null && !string.IsNullOrEmpty(salaryToIvPayEntryModel.MEntryID))
				{
					IVVerificationModel mergeSalaryPaymentVerifData = IVVerificationRepository.GetMergeSalaryPaymentVerifData(ctx, iVVerificationListFilterModel);
					List<IVVerificationListModel> list = new List<IVVerificationListModel>();
					IVVerificationListModel iVVerificationListModel = new IVVerificationListModel();
					iVVerificationListModel.MBillID = mergeSalaryPaymentVerifData.MSourceBillID;
					iVVerificationListModel.VerificationID = mergeSalaryPaymentVerifData.MID;
					if (!string.IsNullOrWhiteSpace(mergeSalaryPaymentVerifData.MID))
					{
						iVVerificationListModel.MHaveVerificationAmtFor = salaryToIvPayEntryModel.MAmount;
						iVVerificationListModel.MBizBillType = "Payment";
						iVVerificationListModel.MBizType = "Pay_Other";
						iVVerificationListModel.MCurrencyID = ctx.MBasCurrencyID;
						iVVerificationListModel.IsMergePay = true;
						iVVerificationListModel.MBizDate = mergeSalaryPaymentVerifData.MBizDate;
						list.Add(iVVerificationListModel);
						salaryPaymentEditModel.Verification = list;
					}
				}
				salaryPaymentEditModel.Verification = (salaryPaymentEditModel.Verification ?? new List<IVVerificationListModel>());
				salaryPaymentEditModel.Verification.AddRange(IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel));
				salaryPaymentEditModel.MActionPermission = GetActionPermissionModel(ctx, salaryPaymentEditModel);
			}
			return salaryPaymentEditModel;
		}

		public DataGridJson<PASalaryPaymentListModel> GetSalaryPaymentList(MContext ctx, PASalaryPaymentListFilterModel filter)
		{
			return PASalaryPaymentRepository.GetSalaryPaymentList(ctx, filter);
		}

		public List<NameValueModel> GetSalaryHeader(MContext ctx)
		{
			return PAPayItemGroupRepository.GetMultLangGroupNameList(ctx);
		}

		public PAPayRunModel GetPayRunModel(MContext ctx, string id)
		{
			return ModelInfoManager.GetDataEditModel<PAPayRunModel>(ctx, id, false, true);
		}

		public string GetSalaryPaymentListByVerificationId(MContext ctx, string id)
		{
			return PASalaryPaymentRepository.GetSalaryPaymentListByBillId(ctx, id);
		}

		public List<PASalaryPaymentTreeModel> GetSalaryPaymentPersonDetails(MContext ctx, string salaryPayId)
		{
			return PASalaryPaymentRepository.GetSalaryPaymentPersonDetails(ctx, salaryPayId);
		}

		public OperationResult SalaryPaymentUpdate(MContext ctx, PASalaryPaymentModel spModel)
		{
			return PASalaryPaymentRepository.SalaryPaymentUpdate(ctx, spModel);
		}

		public OperationResult ValidatePayRunAction(MContext ctx, string yearMonth, PayRunSourceEnum source)
		{
			return PASalaryPaymentRepository.ValidatePayRunAction(ctx, yearMonth, source);
		}

		public OperationResult PayRunUpdate(MContext ctx, PAPayRunModel model, List<string> updateFields = null)
		{
			return PASalaryPaymentRepository.PayRunUpdate(ctx, model, updateFields);
		}

		public OperationResult PayRunNew(MContext ctx, string yearMonth)
		{
			return PASalaryPaymentRepository.PayRunNew(ctx, yearMonth);
		}

		public OperationResult PayRunCopy(MContext ctx, string yearMonth)
		{
			return PASalaryPaymentRepository.PayRunCopy(ctx, yearMonth);
		}

		public OperationResult SalaryPaymentListUpdate(MContext ctx, string runId, string employeeIds)
		{
			return PASalaryPaymentRepository.SalaryPaymentListUpdate(ctx, runId, employeeIds);
		}

		public List<PAEmployeesListModel> GetUnPayEmployeeList(MContext ctx, string runId)
		{
			return PASalaryPaymentRepository.GetUnPayEmployeeList(ctx, runId);
		}

		public OperationResult SalaryPaymentDelete(MContext ctx, string salaryPaymentIds)
		{
			return PASalaryPaymentRepository.SalaryPaymentDelete(ctx, salaryPaymentIds);
		}

		public List<PASalaryListModel> GetSalaryListForPrint(MContext ctx, PASalaryListFilterModel filter)
		{
			if (string.IsNullOrWhiteSpace(filter.MPrintSettingID) || string.IsNullOrWhiteSpace(filter.ObjectIds))
			{
				return new List<PASalaryListModel>();
			}
			filter.PrintSettingModel = new PTSalaryListBusiness().GetPrintSetting(ctx, filter.MPrintSettingID, false);
			return PASalaryPaymentRepository.GetSalaryListForPrint(ctx, filter);
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, DateTime period)
		{
			ImportTemplateModel importTemplateModel = new ImportTemplateModel();
			importTemplateModel.TemplateType = "SalaryList";
			List<PAPayItemModel> disableItemList = PASalaryPaymentRepository.GetDisableItemList(ctx);
			importTemplateModel.SalaryInfo = PASalaryPaymentRepository.GetEmployeeSalaryList(ctx, disableItemList, period);
			importTemplateModel.SalaryInfo.UserPayItemList = PAPayItemGroupRepository.GetUserPayItemList(ctx);
			importTemplateModel.SalaryInfo.FieldMappingList = new List<FieldMappingModel>();
			FieldMappingModel fieldMappingModel = new FieldMappingModel();
			fieldMappingModel.DataRowStartIndex = 8;
			fieldMappingModel.DataColumnStartIndex = 3;
			fieldMappingModel.AllFieldList = new string[32]
			{
				"MDate",
				"MLastName",
				"MFirstName",
				"MBaseSalary",
				"MAllowance",
				"MBonus",
				"MCommission",
				"MOverTime",
				"MTaxAdjustment",
				"MAttendance",
				"MOther",
				"MIncomeTaxThreshold",
				"MSocialSecurityBase",
				"MHosingProvidentFundBase",
				"MRetirementSecurityAmount",
				"MMedicalInsuranceAmount",
				"MUmemploymentAmount",
				"MProvidentAmount",
				"MProvidentAdditionalAmount",
				"MSalaryBeforeTax",
				"MTaxSalary",
				"MNetSalary",
				"MRetirementSecurityAmountC",
				"MMedicalInsuranceAmountC",
				"MUmemploymentInsuranceAmountC",
				"MIndustrialInjuryAmountC",
				"MMaternityInsuranceAmountC",
				"MSeriousIiinessInjuryAmountC",
				"MSocialSecurityOtherAmountC",
				"MProvidentFundAmountC",
				"MAddProvidentFundAmountC",
				"MTotalSalary"
			};
			fieldMappingModel.RequiredFieldList = new string[4]
			{
				"MDate",
				"MLastName",
				"MFirstName",
				"MBaseSalary"
			};
			FieldMappingModel item = fieldMappingModel;
			importTemplateModel.SalaryInfo.FieldMappingList.Add(item);
			InsertUserPayItemFields(importTemplateModel);
			fieldMappingModel = new FieldMappingModel();
			fieldMappingModel.DataRowStartIndex = 5;
			fieldMappingModel.DataColumnStartIndex = 1;
			fieldMappingModel.AllFieldList = new string[18]
			{
				"MEmpRetirementSecurityPer",
				"MRetirementSecurityPer",
				"MEmpMedicalInsurancePer",
				"MMedicalInsurancePer",
				"MEmpUmemploymentInsurancePer",
				"MUmemploymentInsurancePer",
				"",
				"MIndustrialInjuryPer",
				"",
				"MMaternityInsurancePer",
				"",
				"MSeriousIiinessInjuryPer",
				"",
				"MOtherPer",
				"MEmpProvidentFundPer",
				"MProvidentFundPer",
				"MEmpAddProvidentFundPer",
				"MAddProvidentFundPer"
			};
			FieldMappingModel item2 = fieldMappingModel;
			importTemplateModel.SalaryInfo.FieldMappingList.Add(item2);
			SetDisabledItems(importTemplateModel, disableItemList);
			SetEditableGroupList(ctx, importTemplateModel);
			importTemplateModel.SalaryInfo.MultiLangGroupNameList = PAPayItemGroupRepository.GetMultLangGroupNameList(ctx);
			importTemplateModel.SalaryInfo.ActiveEmployeeList = PASalaryPaymentRepository.GetActiveEmployeeList(ctx);
			return importTemplateModel;
		}

		private void SetDisabledItems(ImportTemplateModel model, List<PAPayItemModel> disabledItems)
		{
			Dictionary<PayrollItemEnum, string> allTypeFieldMapping = GetAllTypeFieldMapping();
			List<string> list = new List<string>();
			foreach (PAPayItemModel disabledItem in disabledItems)
			{
				KeyValuePair<PayrollItemEnum, string> keyValuePair = allTypeFieldMapping.FirstOrDefault((KeyValuePair<PayrollItemEnum, string> f) => f.Key == (PayrollItemEnum)disabledItem.MItemType);
				if (keyValuePair.Key != 0)
				{
					list.Add(keyValuePair.Value);
				}
				else if (disabledItem.MItemType == 3010)
				{
					list.Add("MTaxSalary");
				}
				if (disabledItem.MItemType == 1023 || disabledItem.MItemType == 1067)
				{
					list.Add(disabledItem.MItemTypeName);
				}
			}
			model.SalaryInfo.DisabledFieldList = list;
		}

		private void SetEditableGroupList(MContext ctx, ImportTemplateModel model)
		{
			List<PAPayItemGroupModel> salaryItemGroupList = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.All);
			List<PAPayItemGroupModel> list = new List<PAPayItemGroupModel>();
			Dictionary<PayrollItemEnum, string> editableTypeFieldMapping = GetEditableTypeFieldMapping();
			foreach (PayrollItemEnum key in editableTypeFieldMapping.Keys)
			{
				PAPayItemGroupModel pAPayItemGroupModel = salaryItemGroupList.FirstOrDefault((PAPayItemGroupModel f) => f.MItemType == (int)key);
				if (pAPayItemGroupModel == null)
				{
					list.Add(new PAPayItemGroupModel
					{
						MIsDelete = true,
						MItemType = (int)key,
						MItemTypeName = key.ToString()
					});
				}
				else
				{
					list.Add(pAPayItemGroupModel);
				}
			}
			model.SalaryInfo.EditablePresetGroupList = list;
		}

		private void InsertUserPayItemFields(ImportTemplateModel model)
		{
			List<PAPayItemGroupAmtModel> userPayItemList = model.SalaryInfo.UserPayItemList;
			if (userPayItemList.Any())
			{
				List<string> list = model.SalaryInfo.FieldMappingList[0].AllFieldList.ToList();
				int index = list.IndexOf("MTaxAdjustment");
				IEnumerable<string> enumerable = from f in userPayItemList
				where f.MCoefficient == 1
				select f.ItemTypeName;
				if (enumerable.Any())
				{
					list.InsertRange(index, enumerable);
				}
				int index2 = list.IndexOf("MOther");
				IEnumerable<string> enumerable2 = from f in userPayItemList
				where f.MCoefficient == -1
				select f.ItemTypeName;
				if (enumerable2.Any())
				{
					list.InsertRange(index2, enumerable2);
				}
				model.SalaryInfo.FieldMappingList[0].AllFieldList = list.ToArray();
			}
		}

		public OperationResult ImportSalaryList(MContext ctx, ImportSalaryModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			try
			{
				PAPaySettingModel paySettingModel = PASalaryPaymentRepository.GetPaySettingModel(ctx);
				if (string.IsNullOrWhiteSpace(paySettingModel.MItemID))
				{
					ModelHelper.CopyModelValue(model.PaySetting, paySettingModel);
					if (!COMModelValidateHelper.ValidateModel(ctx, paySettingModel, operationResult))
					{
						return operationResult;
					}
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPaySettingModel>(ctx, paySettingModel, null, true));
				}
				List<BDEmployeesModel> list2 = new List<BDEmployeesModel>();
				List<NameValueModel> employeeNameInfoList = new BDEmployeesRepository().GetEmployeeNameInfoList(ctx, true);
				IEnumerable<IGrouping<string, ImportSalaryListModel>> enumerable = from f in model.SalaryList
				group f by f.MDate;
				Dictionary<PayrollItemEnum, string> allTypeFieldMapping = GetAllTypeFieldMapping();
				List<PANewSalaryPaymentEntryModel> payItemList = PASalaryPaymentRepository.GetPayItemList(ctx, false);
				List<BDPayrollDetailModel> dataModelList = ModelInfoManager.GetDataModelList<BDPayrollDetailModel>(ctx, new SqlWhere(), false, false);
				List<BDPayrollDetailModel> list3 = new List<BDPayrollDetailModel>();
				foreach (IGrouping<string, ImportSalaryListModel> item in enumerable)
				{
					string empty = string.Empty;
					int num = 1;
					list.AddRange(PASalaryPaymentRepository.GetImportSalaryCommandList(ctx, item.Key, ref empty));
					List<ImportSalaryListModel> list4 = item.ToList();
					foreach (ImportSalaryListModel item2 in list4)
					{
						NameValueModel nameValueModel = employeeNameInfoList.FirstOrDefault((NameValueModel f) => f.MName == item2.MFirstName && f.MTag == item2.MLastName);
						BDPayrollDetailModel bDPayrollDetailModel = null;
						string empId = string.Empty;
						bool flag = false;
						if (nameValueModel == null)
						{
							BDEmployeesModel bDEmployeesModel = list2.FirstOrDefault((BDEmployeesModel f) => f.MFirstName == item2.MFirstName && f.MLastName == item2.MLastName);
							if (bDEmployeesModel == null)
							{
								BDEmployeesModel newEmpModel = GetNewEmpModel(ctx, item2, ref list);
								list2.Add(newEmpModel);
								empId = newEmpModel.MItemID;
							}
							else
							{
								empId = bDEmployeesModel.MItemID;
							}
						}
						else
						{
							string[] array = nameValueModel.MValue2.Split(',');
							string userName = GlobalFormat.GetUserName(item2.MFirstName, item2.MLastName, null);
							if ("Leave".Equals(array[0], StringComparison.CurrentCultureIgnoreCase))
							{
								operationResult.VerificationInfor.Add(new BizVerificationInfor
								{
									Level = AlertEnum.Error,
									Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "ImportEmployeeHasLeft", "{0}已离职，不能进行工资计算！"), userName)
								});
							}
							if ("0".Equals(array[1], StringComparison.CurrentCultureIgnoreCase))
							{
								operationResult.VerificationInfor.Add(new BizVerificationInfor
								{
									Level = AlertEnum.Error,
									Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "ImportEmployeeHasDisabled", "{0}已禁用，不能进行工资计算！"), userName)
								});
							}
							empId = nameValueModel.MValue;
							bDPayrollDetailModel = dataModelList.FirstOrDefault((BDPayrollDetailModel f) => f.MEmployeeID == empId);
						}
						if (bDPayrollDetailModel == null)
						{
							bDPayrollDetailModel = list3.FirstOrDefault((BDPayrollDetailModel f) => f.MEmployeeID == empId);
						}
						if (bDPayrollDetailModel == null)
						{
							bDPayrollDetailModel = new BDPayrollDetailModel();
							bDPayrollDetailModel.MOrgID = ctx.MOrgID;
							bDPayrollDetailModel.MEmployeeID = empId;
							flag = true;
						}
						item2.MEmployeeID = bDPayrollDetailModel.MEmployeeID;
						if (string.IsNullOrWhiteSpace(bDPayrollDetailModel.MItemID))
						{
							ModelHelper.CopyModelValue(item2, bDPayrollDetailModel);
							if (!AddEmpPaySettingCmd(ctx, model, bDPayrollDetailModel, ref list, operationResult))
							{
								return operationResult;
							}
						}
						if (flag)
						{
							list3.Add(bDPayrollDetailModel);
						}
						PASalaryPaymentModel salaryPaymentModel = GetSalaryPaymentModel(ctx, empty, item2, bDPayrollDetailModel);
						salaryPaymentModel.MSeq = num;
						AddSalaryPaymentEntry(item2, salaryPaymentModel, payItemList, allTypeFieldMapping);
						if (COMModelValidateHelper.ValidateModel(ctx, salaryPaymentModel, operationResult))
						{
							list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PASalaryPaymentModel>(ctx, salaryPaymentModel, null, true));
						}
						num++;
					}
				}
				if (!operationResult.Success)
				{
					return operationResult;
				}
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			}
			catch (Exception ex)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ex.Message
				});
				return operationResult;
			}
			return operationResult;
		}

		private void AddSalaryPaymentEntry(ImportSalaryListModel empSalary, PASalaryPaymentModel spModel, List<PANewSalaryPaymentEntryModel> activePayItemList, Dictionary<PayrollItemEnum, string> dicTypeFieldMapping)
		{
			spModel.SalaryPaymentEntry = new List<PASalaryPaymentEntryModel>();
			foreach (PANewSalaryPaymentEntryModel activePayItem in activePayItemList)
			{
				PASalaryPaymentEntryModel item;
				if ((activePayItem.MItemType == PayrollItemEnum.UserAddItem || activePayItem.MItemType == PayrollItemEnum.UserSubtractItem) && empSalary.UserPayItemList != null)
				{
					PAPayItemGroupAmtModel pAPayItemGroupAmtModel = empSalary.UserPayItemList.FirstOrDefault((PAPayItemGroupAmtModel f) => f.MPayItemID == activePayItem.MPayItemID);
					if (pAPayItemGroupAmtModel != null)
					{
						item = GetNewSalaryPaymentEntry(spModel, activePayItem, pAPayItemGroupAmtModel.Amount);
						goto IL_00bc;
					}
					continue;
				}
				item = GetPayEntryModel(dicTypeFieldMapping, empSalary, spModel, activePayItem);
				goto IL_00bc;
				IL_00bc:
				spModel.SalaryPaymentEntry.Add(item);
			}
		}

		public PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModelByStatus(MContext ctx)
		{
			return PASalaryPaymentRepository.GetSalaryPaymentSummaryModelByStatus(ctx);
		}

		public PASalaryPaymentSummaryModel GetSalaryPaymentSummaryModel(MContext ctx, string runId)
		{
			return PASalaryPaymentRepository.GetSalaryPaymentSummaryModel(ctx, runId);
		}

		public OperationResult UnApproveSalaryPayment(MContext ctx, string ids)
		{
			PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, ids);
			if (salaryPaymentEditModel == null || string.IsNullOrEmpty(salaryPaymentEditModel.MID))
			{
				return new OperationResult
				{
					Success = false
				};
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, ids);
			if (salaryPaymentEditModel.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && !operationResult.Success)
			{
				return operationResult;
			}
			return PASalaryPaymentRepository.UnApproveSalaryPayment(ctx, salaryPaymentEditModel);
		}

		public OperationResult PaySalary(MContext ctx, IVMakePaymentModel model)
		{
			model.MObjectType = "Invoice";
			IVSalaryMakePaymentRepository iVSalaryMakePaymentRepository = new IVSalaryMakePaymentRepository();
			return iVSalaryMakePaymentRepository.ToPay(ctx, model);
		}

		private static PASalaryPaymentModel GetSalaryPaymentModel(MContext ctx, string payRunId, ImportSalaryListModel empSalary, BDPayrollDetailModel empPaySetting)
		{
			PASalaryPaymentModel pASalaryPaymentModel = new PASalaryPaymentModel();
			pASalaryPaymentModel.MOrgID = ctx.MOrgID;
			pASalaryPaymentModel.MID = UUIDHelper.GetGuid();
			pASalaryPaymentModel.IsNew = true;
			pASalaryPaymentModel.MStatus = 1;
			pASalaryPaymentModel.MRunID = payRunId;
			pASalaryPaymentModel.MEmployeeID = empPaySetting.MEmployeeID;
			pASalaryPaymentModel.MTaxSalary = Math.Round(empSalary.MTaxSalary, 2, MidpointRounding.AwayFromZero);
			pASalaryPaymentModel.MNetSalary = Math.Round(empSalary.MNetSalary, 2, MidpointRounding.AwayFromZero);
			return pASalaryPaymentModel;
		}

		private static bool AddEmpPaySettingCmd(MContext ctx, ImportSalaryModel model, BDPayrollDetailModel empPaySetting, ref List<CommandInfo> cmdList, OperationResult result)
		{
			empPaySetting.MRetirementSecurityPercentage = model.PaySetting.MEmpRetirementSecurityPer;
			empPaySetting.MUmemploymentPercentage = model.PaySetting.MEmpUmemploymentInsurancePer;
			empPaySetting.MMedicalInsurancePercentage = model.PaySetting.MEmpMedicalInsurancePer;
			empPaySetting.MProvidentPercentage = model.PaySetting.MEmpProvidentFundPer;
			empPaySetting.MProvidentAdditionalPercentage = model.PaySetting.MEmpAddProvidentFundPer;
			bool flag = COMModelValidateHelper.ValidateModel(ctx, empPaySetting, result);
			if (flag)
			{
				cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDPayrollDetailModel>(ctx, empPaySetting, null, true));
			}
			return flag;
		}

		private static PASalaryPaymentEntryModel GetPayEntryModel(Dictionary<PayrollItemEnum, string> dicTypeFieldMapping, ImportSalaryListModel empSalary, PASalaryPaymentModel spModel, PANewSalaryPaymentEntryModel payItem)
		{
			decimal amount = default(decimal);
			if (dicTypeFieldMapping.ContainsKey(payItem.MItemType))
			{
				string propName = dicTypeFieldMapping[payItem.MItemType];
				amount = Math.Round(Convert.ToDecimal(ModelHelper.GetModelValue(empSalary, propName)), 2, MidpointRounding.AwayFromZero);
			}
			else
			{
				switch (payItem.MItemType)
				{
				case PayrollItemEnum.EmployeeSocialSecurity:
					amount = empSalary.MUmemploymentAmount + empSalary.MMedicalInsuranceAmount + empSalary.MRetirementSecurityAmount;
					break;
				case PayrollItemEnum.EmployeeHousingProvidentFund:
					amount = empSalary.MProvidentAmount + empSalary.MProvidentAdditionalAmount;
					break;
				case PayrollItemEnum.EmployerSocialSecurity:
					amount = empSalary.MUmemploymentInsuranceAmountC + empSalary.MRetirementSecurityAmountC + empSalary.MMedicalInsuranceAmountC + empSalary.MMaternityInsuranceAmountC + empSalary.MIndustrialInjuryAmountC + empSalary.MSeriousIiinessInjuryAmountC + empSalary.MSocialSecurityOtherAmountC;
					break;
				case PayrollItemEnum.EmployerHousingProvidentFund:
					amount = empSalary.MProvidentFundAmountC + empSalary.MAddProvidentFundAmountC;
					break;
				}
			}
			return GetNewSalaryPaymentEntry(spModel, payItem, amount);
		}

		private static PASalaryPaymentEntryModel GetNewSalaryPaymentEntry(PASalaryPaymentModel spModel, PANewSalaryPaymentEntryModel payItem, decimal amount)
		{
			PASalaryPaymentEntryModel pASalaryPaymentEntryModel = new PASalaryPaymentEntryModel();
			pASalaryPaymentEntryModel.MID = spModel.MID;
			pASalaryPaymentEntryModel.MPayItemID = payItem.MPayItemID;
			pASalaryPaymentEntryModel.MParentPayItemID = payItem.MParentPayItemID;
			pASalaryPaymentEntryModel.MAmount = amount;
			return pASalaryPaymentEntryModel;
		}

		private BDEmployeesModel GetNewEmpModel(MContext ctx, ImportSalaryListModel empSalary, ref List<CommandInfo> cmdList)
		{
			BDEmployeesModel bDEmployeesModel = new BDEmployeesModel
			{
				MOrgID = ctx.MOrgID,
				MFirstName = empSalary.MFirstName,
				MLastName = empSalary.MLastName,
				MStatus = "Regular",
				MultiLanguage = new List<MultiLanguageFieldList>
				{
					GetMultiLanguageFieldList("MFirstName", empSalary.MFirstName),
					GetMultiLanguageFieldList("MLastName", empSalary.MLastName)
				}
			};
			dalEmp.MultiLanguageAdd(bDEmployeesModel);
			cmdList.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDEmployeesModel>(ctx, bDEmployeesModel, null, true));
			return bDEmployeesModel;
		}

		private MultiLanguageFieldList GetMultiLanguageFieldList(string fieldName, string langName)
		{
			return new MultiLanguageFieldList
			{
				MFieldName = fieldName,
				MMultiLanguageField = new List<MultiLanguageField>
				{
					new MultiLanguageField
					{
						MLocaleID = "0x0009",
						MValue = langName
					},
					new MultiLanguageField
					{
						MLocaleID = "0x7804",
						MValue = langName
					},
					new MultiLanguageField
					{
						MLocaleID = "0x7C04",
						MValue = langName
					}
				}
			};
		}

		private Dictionary<PayrollItemEnum, string> GetEditableTypeFieldMapping()
		{
			Dictionary<PayrollItemEnum, string> dictionary = new Dictionary<PayrollItemEnum, string>();
			dictionary.Add(PayrollItemEnum.Allowance, "MAllowance");
			dictionary.Add(PayrollItemEnum.Commission, "MCommission");
			dictionary.Add(PayrollItemEnum.Bonus, "MBonus");
			dictionary.Add(PayrollItemEnum.OverTime, "MOverTime");
			dictionary.Add(PayrollItemEnum.TaxAdjustment, "MTaxAdjustment");
			dictionary.Add(PayrollItemEnum.Attendance, "MAttendance");
			dictionary.Add(PayrollItemEnum.Other, "MOther");
			return dictionary;
		}

		private Dictionary<PayrollItemEnum, string> GetEmpTypeFieldMapping()
		{
			Dictionary<PayrollItemEnum, string> dictionary = new Dictionary<PayrollItemEnum, string>();
			dictionary.Add(PayrollItemEnum.BaseSalary, "MBaseSalary");
			dictionary.Add(PayrollItemEnum.Allowance, "MAllowance");
			dictionary.Add(PayrollItemEnum.Commission, "MCommission");
			dictionary.Add(PayrollItemEnum.Bonus, "MBonus");
			dictionary.Add(PayrollItemEnum.OverTime, "MOverTime");
			dictionary.Add(PayrollItemEnum.TaxAdjustment, "MTaxAdjustment");
			dictionary.Add(PayrollItemEnum.Attendance, "MAttendance");
			dictionary.Add(PayrollItemEnum.Other, "MOther");
			dictionary.Add(PayrollItemEnum.PBasicRetirementSecurity, "MRetirementSecurityAmount");
			dictionary.Add(PayrollItemEnum.PBasicMedicalInsurance, "MMedicalInsuranceAmount");
			dictionary.Add(PayrollItemEnum.PBasicUnemploymentInsurance, "MUmemploymentAmount");
			dictionary.Add(PayrollItemEnum.PHousingProvidentFund, "MProvidentAmount");
			dictionary.Add(PayrollItemEnum.PAdditionHousingProvidentFund, "MProvidentAdditionalAmount");
			return dictionary;
		}

		private Dictionary<PayrollItemEnum, string> GetAllTypeFieldMapping()
		{
			Dictionary<PayrollItemEnum, string> empTypeFieldMapping = GetEmpTypeFieldMapping();
			empTypeFieldMapping.Add(PayrollItemEnum.CBasicRetirementSecurity, "MRetirementSecurityAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CBasicMedicalInsurance, "MMedicalInsuranceAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CBasicUnemploymentInsurance, "MUmemploymentInsuranceAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CMaternityInsurance, "MMaternityInsuranceAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CIndustrialInjury, "MIndustrialInjuryAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CSeriousIllnessMedicalTreatment, "MSeriousIiinessInjuryAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CSocialSecurityOther, "MSocialSecurityOtherAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CHousingProvidentFund, "MProvidentFundAmountC");
			empTypeFieldMapping.Add(PayrollItemEnum.CAdditionHousingProvidentFund, "MAddProvidentFundAmountC");
			return empTypeFieldMapping;
		}

		private PASalaryPaymentPermissionModel GetActionPermissionModel(MContext ctx, PASalaryPaymentModel model)
		{
			PASalaryPaymentPermissionModel pASalaryPaymentPermissionModel = new PASalaryPaymentPermissionModel();
			bool mIsCanPay = true;
			if (model.MStatus != Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment) || (Math.Abs(model.MNetSalary) == Math.Abs(model.MVerificationAmt) && model.MStatus == Convert.ToInt32(PASalaryPaymentStatusEnum.Paid)) || !base.HavePermission(ctx, "PayRun", "Approve") || !base.HavePermission(ctx, "BankAccount", "Change"))
			{
				mIsCanPay = false;
			}
			if (!base.HavePermission(ctx, "PayRun", "Change"))
			{
				pASalaryPaymentPermissionModel.MHaveAction = false;
				pASalaryPaymentPermissionModel.MIsCanPay = mIsCanPay;
				return pASalaryPaymentPermissionModel;
			}
			if (string.IsNullOrEmpty(model.MID))
			{
				pASalaryPaymentPermissionModel.MHaveAction = false;
				pASalaryPaymentPermissionModel.MIsCanEdit = true;
				return pASalaryPaymentPermissionModel;
			}
			pASalaryPaymentPermissionModel.MHaveAction = true;
			pASalaryPaymentPermissionModel.MIsCanPay = mIsCanPay;
			IVVerificationBusiness iVVerificationBusiness = new IVVerificationBusiness();
			List<IVVerificationInforModel> customerWaitForVerificationInfor = iVVerificationBusiness.GetCustomerWaitForVerificationInfor(ctx, model.MID, "PayRun");
			if (model.MStatus != Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment) || (Math.Abs(model.MNetSalary) == Math.Abs(model.MVerificationAmt) && model.MStatus == Convert.ToInt32(PASalaryPaymentStatusEnum.Paid)) || !base.HavePermission(ctx, "PayRun", "Approve") || customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0)
			{
				pASalaryPaymentPermissionModel.MIsCanVerification = false;
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (model.MStatus != Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero || !base.HavePermission(ctx, "PayRun", "Approve") || !operationResult.Success)
			{
				pASalaryPaymentPermissionModel.MIsCanUnApprove = false;
			}
			if (!base.HavePermission(ctx, "General_Ledger", "View") || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID))
			{
				pASalaryPaymentPermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MStatus >= 3)
			{
				pASalaryPaymentPermissionModel.MIsCanVoid = false;
			}
			if (model.MStatus >= Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero)
			{
				pASalaryPaymentPermissionModel.MIsCanDelete = false;
			}
			if (model.MStatus >= Convert.ToInt32(PASalaryPaymentStatusEnum.WaitingPayment))
			{
				pASalaryPaymentPermissionModel.MIsCanEdit = false;
			}
			return pASalaryPaymentPermissionModel;
		}
	}
}
