using JieNor.Megi.BusinessContract.PA;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.BusinessService.PA
{
	public class PAPayItemBussiness : APIBusinessBase<PAPayItemModel>, IPAPayItemBussiness, IDataContract<PAPayItemModel>
	{
		private PAPayItemRepository dal = new PAPayItemRepository();

		private PAPayItemGroupRepository GroupDal = new PAPayItemGroupRepository();

		private BDAccountBusiness _accountBiz = new BDAccountBusiness();

		private string[] _userPayItemTypes;

		public string[] UserPayItemTypes
		{
			get
			{
				if (_userPayItemTypes == null)
				{
					string[] obj = new string[2];
					int num = 1023;
					obj[0] = num.ToString();
					num = 1067;
					obj[1] = num.ToString();
					_userPayItemTypes = obj;
				}
				return _userPayItemTypes;
			}
		}

		public override void CheckEndpointAvailable(MContext ctx, int version = -1, string endPointName = null)
		{
			base.CheckEndpointAvailable(ctx, 1, "SalaryItems");
		}

		protected override DataGridJson<PAPayItemModel> OnGet(MContext ctx, GetParam param)
		{
			return dal.Get(ctx, param);
		}

		public List<PAPayItemModel> GetSalaryItemList(MContext ctx)
		{
			InsertDefaultDataIfNotExist(ctx);
			return dal.GetSalaryItemList(ctx, null, null);
		}

		public void InsertCSocialSecurityOther(MContext ctx)
		{
			List<PAPayItemModel> modelList = dal.GetModelList(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MItemType", new string[2]
			{
				"2045",
				"2050"
			}), false);
			if (modelList != null && modelList.Count != 0 && !modelList.Any((PAPayItemModel f) => f.MItemType == 2050))
			{
				PAPayItemModel pAPayItemModel = modelList.FirstOrDefault((PAPayItemModel f) => f.MItemType == 2045);
				if (pAPayItemModel != null)
				{
					PAPayItemModel dataModelByFilter = dal.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MItemType", 2050));
					if (dataModelByFilter != null)
					{
						dataModelByFilter.MItemID = UUIDHelper.GetGuid();
						dataModelByFilter.MOrgID = ctx.MOrgID;
						dataModelByFilter.IsNew = true;
						dataModelByFilter.MIsSys = false;
						dataModelByFilter.MGroupID = pAPayItemModel.MGroupID;
						foreach (MultiLanguageFieldList item in dataModelByFilter.MultiLanguage)
						{
							item.MParentID = dataModelByFilter.MItemID;
							foreach (MultiLanguageField item2 in item.MMultiLanguageField)
							{
								item2.MPKID = UUIDHelper.GetGuid();
							}
						}
						dal.InsertOrUpdate(ctx, dataModelByFilter, null);
					}
				}
			}
		}

		public OperationResult UpdateModel(MContext ctx, PAPayItemModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<string> codeList = new List<string>
			{
				model.MAccountCode
			};
			if (!_accountBiz.CheckAccountExist(ctx, codeList, operationResult))
			{
				return operationResult;
			}
			model.MOrgID = ctx.MOrgID;
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				List<CommandInfo> list = new List<CommandInfo>();
				PAPayItemModel salaryItemById = GetSalaryItemById(ctx, model.MItemID);
				salaryItemById.MultiLanguage = model.MultiLanguage;
				salaryItemById.MAccountCode = model.MAccountCode;
				if (ctx.MRegProgress >= 13)
				{
					List<PAPayItemModel> salaryItemList = dal.GetSalaryItemList(ctx, model.MItemID, null);
					if (salaryItemList != null && salaryItemList.Count() > 0)
					{
						List<string> list2 = new List<string>();
						list2.Add("MAccountCode");
						foreach (PAPayItemModel item in salaryItemList)
						{
							if (string.IsNullOrWhiteSpace(item.MAccountCode))
							{
								item.MAccountCode = salaryItemById.MAccountCode;
								list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayItemModel>(ctx, item, list2, true));
							}
						}
					}
				}
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PAPayItemModel>(ctx, salaryItemById, null, true));
				int num = BDRepository.ExecuteSqlTran(ctx, list);
				operationResult.Success = (num > 0);
				return operationResult;
			}
			return dal.InsertOrUpdate(ctx, model, null);
		}

		public PAPayItemModel GetSalaryItemById(MContext ctx, string id)
		{
			PAPayItemModel dataModel = dal.GetDataModel(ctx, id, false);
			if (dataModel != null && ctx.MRegProgress == 15)
			{
				string value = (dataModel.MAccountCode == null) ? "2211" : dataModel.MAccountCode;
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

		public List<SalaryItemTreeModel> GetSalaryItemTreeList(MContext ctx, bool removeInActive)
		{
			List<SalaryItemTreeModel> list = new List<SalaryItemTreeModel>();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MOrgID ", ctx.MOrgID);
			List<PAPayItemGroupModel> list2 = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.All);
			if (!list2.Any((PAPayItemGroupModel f) => !UserPayItemTypes.Contains(f.MItemType.ToString())))
			{
				InsertDefaultData(ctx);
				list2 = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.All);
			}
			if (removeInActive)
			{
				list2 = (from m in list2
				where m.MIsActive
				select m).ToList();
			}
			if (list2 != null && list2.Count > 0)
			{
				foreach (PAPayItemGroupModel item in list2)
				{
					SalaryItemTreeModel salaryItemTreeModel = new SalaryItemTreeModel();
					salaryItemTreeModel.id = item.MItemID;
					salaryItemTreeModel.text = item.MName;
					salaryItemTreeModel._parentId = "0";
					salaryItemTreeModel.IsActive = item.MIsActive;
					salaryItemTreeModel.ItemType = item.MItemType;
					salaryItemTreeModel.MCreateDate = item.MCreateDate;
					salaryItemTreeModel.children = new List<SalaryItemTreeChildModel>();
					List<PAPayItemModel> list3 = dal.GetSalaryItemList(ctx, item.MItemID, null);
					if (removeInActive)
					{
						list3 = (from m in list3
						where m.MIsActive
						select m).ToList();
					}
					if (list3 != null && list3.Count > 0)
					{
						foreach (PAPayItemModel item2 in list3)
						{
							SalaryItemTreeChildModel salaryItemTreeChildModel = new SalaryItemTreeChildModel();
							salaryItemTreeChildModel.id = item2.MItemID;
							salaryItemTreeChildModel.text = item2.MName;
							salaryItemTreeChildModel._parentId = item.MItemID;
							salaryItemTreeChildModel.IsActive = item2.MIsActive;
							salaryItemTreeChildModel.ItemType = item2.MItemType;
							salaryItemTreeModel.children.Add(salaryItemTreeChildModel);
						}
					}
					salaryItemTreeModel.children = (from f in salaryItemTreeModel.children
					orderby f.ItemType
					select f).ToList();
					list.Add(salaryItemTreeModel);
				}
			}
			return (from item in list
			orderby item.ItemType, item.MCreateDate
			select item).ToList();
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdate(MContext ctx, PAPayItemModel modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<PAPayItemModel> modelData, string fields = null)
		{
			throw new NotImplementedException();
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			throw new NotImplementedException();
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			throw new NotImplementedException();
		}

		public PAPayItemModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			throw new NotImplementedException();
		}

		public PAPayItemModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public List<PAPayItemModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<PAPayItemModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			throw new NotImplementedException();
		}

		public OperationResult ExistDefaultPayItem(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			operationResult.Success = GroupDal.ExistsByFilter(ctx, new SqlWhere().NotIn("MItemType", UserPayItemTypes));
			return operationResult;
		}

		public OperationResult InsertDefaultItem(MContext ctx)
		{
			List<PAPayItemGroupModel> list = new List<PAPayItemGroupModel>();
			PAPayItemGroupModel pAPayItemGroupModel = new PAPayItemGroupModel();
			pAPayItemGroupModel.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel.MItemType = 1000;
			pAPayItemGroupModel.MCoefficient = 1;
			pAPayItemGroupModel.MultiLanguage = GetMulitLang(PayrollItemEnum.BaseSalary);
			list.Add(pAPayItemGroupModel);
			PAPayItemGroupModel pAPayItemGroupModel2 = new PAPayItemGroupModel();
			pAPayItemGroupModel2.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel2.MCoefficient = 1;
			pAPayItemGroupModel2.MItemType = 1005;
			pAPayItemGroupModel2.MultiLanguage = GetMulitLang(PayrollItemEnum.Allowance);
			list.Add(pAPayItemGroupModel2);
			PAPayItemGroupModel pAPayItemGroupModel3 = new PAPayItemGroupModel();
			pAPayItemGroupModel3.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel3.MCoefficient = 1;
			pAPayItemGroupModel3.MItemType = 1010;
			pAPayItemGroupModel3.MultiLanguage = GetMulitLang(PayrollItemEnum.Bonus);
			list.Add(pAPayItemGroupModel3);
			PAPayItemGroupModel pAPayItemGroupModel4 = new PAPayItemGroupModel();
			pAPayItemGroupModel4.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel4.MCoefficient = 1;
			pAPayItemGroupModel4.MItemType = 1015;
			pAPayItemGroupModel4.MultiLanguage = GetMulitLang(PayrollItemEnum.Commission);
			list.Add(pAPayItemGroupModel4);
			PAPayItemGroupModel pAPayItemGroupModel5 = new PAPayItemGroupModel();
			pAPayItemGroupModel5.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel5.MCoefficient = 1;
			pAPayItemGroupModel5.MItemType = 1020;
			pAPayItemGroupModel5.MultiLanguage = GetMulitLang(PayrollItemEnum.OverTime);
			list.Add(pAPayItemGroupModel5);
			PAPayItemGroupModel pAPayItemGroupModel6 = new PAPayItemGroupModel();
			pAPayItemGroupModel6.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel6.MCoefficient = 1;
			pAPayItemGroupModel6.MItemType = 1025;
			pAPayItemGroupModel6.MultiLanguage = GetMulitLang(PayrollItemEnum.TaxAdjustment);
			list.Add(pAPayItemGroupModel6);
			PAPayItemGroupModel pAPayItemGroupModel7 = new PAPayItemGroupModel();
			pAPayItemGroupModel7.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel7.MCoefficient = 1;
			pAPayItemGroupModel7.MItemType = 1030;
			pAPayItemGroupModel7.MultiLanguage = GetMulitLang(PayrollItemEnum.Attendance);
			list.Add(pAPayItemGroupModel7);
			PAPayItemGroupModel pAPayItemGroupModel8 = new PAPayItemGroupModel();
			pAPayItemGroupModel8.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel8.MItemType = 3000;
			pAPayItemGroupModel8.MultiLanguage = GetMulitLang(PayrollItemEnum.SalaryBeforePIT);
			list.Add(pAPayItemGroupModel8);
			PAPayItemGroupModel pAPayItemGroupModel9 = new PAPayItemGroupModel();
			pAPayItemGroupModel9.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel9.MItemType = 3005;
			pAPayItemGroupModel9.MultiLanguage = GetMulitLang(PayrollItemEnum.SalaryAfterPIT);
			list.Add(pAPayItemGroupModel9);
			PAPayItemGroupModel pAPayItemGroupModel10 = new PAPayItemGroupModel();
			pAPayItemGroupModel10.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel10.MCoefficient = -1;
			pAPayItemGroupModel10.MItemType = 3010;
			pAPayItemGroupModel10.MultiLanguage = GetMulitLang(PayrollItemEnum.PIT);
			list.Add(pAPayItemGroupModel10);
			PAPayItemGroupModel pAPayItemGroupModel11 = new PAPayItemGroupModel();
			pAPayItemGroupModel11.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel11.MCoefficient = -1;
			pAPayItemGroupModel11.MItemType = 1035;
			pAPayItemGroupModel11.MultiLanguage = GetMulitLang(PayrollItemEnum.EmployeeSocialSecurity);
			list.Add(pAPayItemGroupModel11);
			PAPayItemGroupModel pAPayItemGroupModel12 = new PAPayItemGroupModel();
			pAPayItemGroupModel12.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel12.MCoefficient = -1;
			pAPayItemGroupModel12.MItemType = 1055;
			pAPayItemGroupModel12.MultiLanguage = GetMulitLang(PayrollItemEnum.EmployeeHousingProvidentFund);
			list.Add(pAPayItemGroupModel12);
			PAPayItemGroupModel pAPayItemGroupModel13 = new PAPayItemGroupModel();
			pAPayItemGroupModel13.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel13.MCoefficient = -1;
			pAPayItemGroupModel13.MItemType = 2015;
			pAPayItemGroupModel13.MultiLanguage = GetMulitLang(PayrollItemEnum.EmployerSocialSecurity);
			list.Add(pAPayItemGroupModel13);
			PAPayItemGroupModel pAPayItemGroupModel14 = new PAPayItemGroupModel();
			pAPayItemGroupModel14.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel14.MCoefficient = -1;
			pAPayItemGroupModel14.MItemType = 2000;
			pAPayItemGroupModel14.MultiLanguage = GetMulitLang(PayrollItemEnum.EmployerHousingProvidentFund);
			list.Add(pAPayItemGroupModel14);
			PAPayItemGroupModel pAPayItemGroupModel15 = new PAPayItemGroupModel();
			pAPayItemGroupModel15.MOrgID = ctx.MOrgID;
			pAPayItemGroupModel15.MCoefficient = -1;
			pAPayItemGroupModel15.MItemType = 3015;
			pAPayItemGroupModel15.MultiLanguage = GetMulitLang(PayrollItemEnum.TotalSalary);
			list.Add(pAPayItemGroupModel15);
			OperationResult operationResult = GroupDal.InsertOrUpdateModels(ctx, list, "");
			if (operationResult.Success)
			{
				List<PAPayItemModel> list2 = new List<PAPayItemModel>();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal(" MOrgID ", ctx.MOrgID);
				sqlWhere.Equal(" MItemType ", 1035);
				PAPayItemGroupModel dataModelByFilter = GroupDal.GetDataModelByFilter(ctx, sqlWhere);
				if (dataModelByFilter != null)
				{
					PAPayItemModel pAPayItemModel = new PAPayItemModel();
					pAPayItemModel.MGroupID = dataModelByFilter.MItemID;
					pAPayItemModel.MOrgID = ctx.MOrgID;
					pAPayItemModel.MCoefficient = -1;
					pAPayItemModel.MItemType = 1040;
					pAPayItemModel.MultiLanguage = GetMulitLang(PayrollItemEnum.PBasicRetirementSecurity);
					list2.Add(pAPayItemModel);
					PAPayItemModel pAPayItemModel2 = new PAPayItemModel();
					pAPayItemModel2.MOrgID = ctx.MOrgID;
					pAPayItemModel2.MCoefficient = -1;
					pAPayItemModel2.MGroupID = dataModelByFilter.MItemID;
					pAPayItemModel2.MItemType = 1045;
					pAPayItemModel2.MultiLanguage = GetMulitLang(PayrollItemEnum.PBasicMedicalInsurance);
					list2.Add(pAPayItemModel2);
					PAPayItemModel pAPayItemModel3 = new PAPayItemModel();
					pAPayItemModel3.MOrgID = ctx.MOrgID;
					pAPayItemModel3.MCoefficient = -1;
					pAPayItemModel3.MGroupID = dataModelByFilter.MItemID;
					pAPayItemModel3.MItemType = 1050;
					pAPayItemModel3.MultiLanguage = GetMulitLang(PayrollItemEnum.PBasicUnemploymentInsurance);
					list2.Add(pAPayItemModel3);
				}
				SqlWhere sqlWhere2 = new SqlWhere();
				sqlWhere2.Equal(" MOrgID ", ctx.MOrgID);
				sqlWhere2.Equal(" MItemType ", 1055);
				PAPayItemGroupModel dataModelByFilter2 = GroupDal.GetDataModelByFilter(ctx, sqlWhere2);
				if (dataModelByFilter2 != null)
				{
					PAPayItemModel pAPayItemModel4 = new PAPayItemModel();
					pAPayItemModel4.MOrgID = ctx.MOrgID;
					pAPayItemModel4.MCoefficient = -1;
					pAPayItemModel4.MGroupID = dataModelByFilter2.MItemID;
					pAPayItemModel4.MItemType = 1060;
					pAPayItemModel4.MultiLanguage = GetMulitLang(PayrollItemEnum.PHousingProvidentFund);
					list2.Add(pAPayItemModel4);
					PAPayItemModel pAPayItemModel5 = new PAPayItemModel();
					pAPayItemModel5.MOrgID = ctx.MOrgID;
					pAPayItemModel5.MCoefficient = -1;
					pAPayItemModel5.MGroupID = dataModelByFilter2.MItemID;
					pAPayItemModel5.MItemType = 1065;
					pAPayItemModel5.MultiLanguage = GetMulitLang(PayrollItemEnum.PAdditionHousingProvidentFund);
					list2.Add(pAPayItemModel5);
				}
				SqlWhere sqlWhere3 = new SqlWhere();
				sqlWhere3.Equal(" MOrgID ", ctx.MOrgID);
				sqlWhere3.Equal(" MItemType ", 2015);
				PAPayItemGroupModel dataModelByFilter3 = GroupDal.GetDataModelByFilter(ctx, sqlWhere3);
				if (dataModelByFilter3 != null)
				{
					PAPayItemModel pAPayItemModel6 = new PAPayItemModel();
					pAPayItemModel6.MOrgID = ctx.MOrgID;
					pAPayItemModel6.MCoefficient = -1;
					pAPayItemModel6.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel6.MItemType = 2020;
					pAPayItemModel6.MultiLanguage = GetMulitLang(PayrollItemEnum.CBasicRetirementSecurity);
					list2.Add(pAPayItemModel6);
					PAPayItemModel pAPayItemModel7 = new PAPayItemModel();
					pAPayItemModel7.MOrgID = ctx.MOrgID;
					pAPayItemModel7.MCoefficient = -1;
					pAPayItemModel7.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel7.MItemType = 2025;
					pAPayItemModel7.MultiLanguage = GetMulitLang(PayrollItemEnum.CBasicMedicalInsurance);
					list2.Add(pAPayItemModel7);
					PAPayItemModel pAPayItemModel8 = new PAPayItemModel();
					pAPayItemModel8.MOrgID = ctx.MOrgID;
					pAPayItemModel8.MCoefficient = -1;
					pAPayItemModel8.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel8.MItemType = 2035;
					pAPayItemModel8.MultiLanguage = GetMulitLang(PayrollItemEnum.CBasicUnemploymentInsurance);
					list2.Add(pAPayItemModel8);
					PAPayItemModel pAPayItemModel9 = new PAPayItemModel();
					pAPayItemModel9.MOrgID = ctx.MOrgID;
					pAPayItemModel9.MCoefficient = -1;
					pAPayItemModel9.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel9.MItemType = 2030;
					pAPayItemModel9.MultiLanguage = GetMulitLang(PayrollItemEnum.CMaternityInsurance);
					list2.Add(pAPayItemModel9);
					PAPayItemModel pAPayItemModel10 = new PAPayItemModel();
					pAPayItemModel10.MOrgID = ctx.MOrgID;
					pAPayItemModel10.MCoefficient = -1;
					pAPayItemModel10.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel10.MItemType = 2040;
					pAPayItemModel10.MultiLanguage = GetMulitLang(PayrollItemEnum.CIndustrialInjury);
					list2.Add(pAPayItemModel10);
					PAPayItemModel pAPayItemModel11 = new PAPayItemModel();
					pAPayItemModel11.MOrgID = ctx.MOrgID;
					pAPayItemModel11.MCoefficient = -1;
					pAPayItemModel11.MGroupID = dataModelByFilter3.MItemID;
					pAPayItemModel11.MItemType = 2045;
					list2.Add(pAPayItemModel11);
				}
				SqlWhere sqlWhere4 = new SqlWhere();
				sqlWhere4.Equal(" MOrgID ", ctx.MOrgID);
				sqlWhere4.Equal(" MItemType ", 2000);
				PAPayItemGroupModel dataModelByFilter4 = GroupDal.GetDataModelByFilter(ctx, sqlWhere4);
				if (dataModelByFilter4 != null)
				{
					PAPayItemModel pAPayItemModel12 = new PAPayItemModel();
					pAPayItemModel12.MOrgID = ctx.MOrgID;
					pAPayItemModel12.MGroupID = dataModelByFilter4.MItemID;
					pAPayItemModel12.MCoefficient = -1;
					pAPayItemModel12.MItemType = 2005;
					pAPayItemModel12.MultiLanguage = GetMulitLang(PayrollItemEnum.CHousingProvidentFund);
					list2.Add(pAPayItemModel12);
					PAPayItemModel pAPayItemModel13 = new PAPayItemModel();
					pAPayItemModel13.MOrgID = ctx.MOrgID;
					pAPayItemModel13.MGroupID = dataModelByFilter4.MItemID;
					pAPayItemModel13.MCoefficient = -1;
					pAPayItemModel13.MItemType = 2010;
					pAPayItemModel13.MultiLanguage = GetMulitLang(PayrollItemEnum.CAdditionHousingProvidentFund);
					list2.Add(pAPayItemModel13);
				}
				dal.InsertOrUpdateModels(ctx, list2, "");
			}
			return operationResult;
		}

		public OperationResult InsertDefaultDataIfNotExist(MContext ctx)
		{
			OperationResult operationResult = ExistDefaultPayItem(ctx);
			if (operationResult.Success)
			{
				return operationResult;
			}
			return InsertDefaultData(ctx);
		}

		private OperationResult InsertDefaultData(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<PAPayItemGroupModel> list2 = new List<PAPayItemGroupModel>();
			List<PAPayItemModel> list3 = new List<PAPayItemModel>();
			DateTime dateNow = ctx.DateNow;
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MOrgID", "0");
			List<PAPayItemGroupModel> dataModelList = ModelInfoManager.GetDataModelList<PAPayItemGroupModel>(ctx, new SqlWhere().Equal("MOrgID", "0"), false, true);
			if (dataModelList != null)
			{
				foreach (PAPayItemGroupModel item2 in dataModelList)
				{
					PAPayItemGroupModel pAPayItemGroupModel = new PAPayItemGroupModel();
					pAPayItemGroupModel.MItemID = UUIDHelper.GetGuid();
					pAPayItemGroupModel.IsNew = true;
					pAPayItemGroupModel.MOrgID = ctx.MOrgID;
					pAPayItemGroupModel.MTemplateID = item2.MItemID;
					pAPayItemGroupModel.MIsSys = item2.MIsSys;
					pAPayItemGroupModel.MItemType = item2.MItemType;
					pAPayItemGroupModel.MIsActive = item2.MIsActive;
					pAPayItemGroupModel.MIsDelete = item2.MIsDelete;
					pAPayItemGroupModel.MCreateDate = dateNow;
					pAPayItemGroupModel.MModifyDate = dateNow;
					pAPayItemGroupModel.MCoefficient = item2.MCoefficient;
					pAPayItemGroupModel.MultiLanguage = item2.MultiLanguage;
					if (pAPayItemGroupModel.MultiLanguage != null)
					{
						pAPayItemGroupModel.MultiLanguage[0].MParentID = pAPayItemGroupModel.MItemID;
						foreach (MultiLanguageField item3 in pAPayItemGroupModel.MultiLanguage[0].MMultiLanguageField)
						{
							item3.MPKID = UUIDHelper.GetGuid();
						}
					}
					list2.Add(pAPayItemGroupModel);
				}
				list.AddRange(GetPayItemGroupInsertCmds(ctx, list2));
			}
			SqlWhere sqlWhere2 = new SqlWhere();
			sqlWhere2.Equal("MOrgID", "0");
			List<PAPayItemModel> dataModelList2 = ModelInfoManager.GetDataModelList<PAPayItemModel>(ctx, new SqlWhere().Equal("MOrgID", "0"), false, true);
			if (dataModelList2 != null)
			{
				foreach (PAPayItemModel item4 in dataModelList2)
				{
					PAPayItemModel pAPayItemModel = new PAPayItemModel();
					pAPayItemModel.MItemID = UUIDHelper.GetGuid();
					pAPayItemModel.IsNew = true;
					pAPayItemModel.MOrgID = ctx.MOrgID;
					PAPayItemGroupModel pAPayItemGroupModel2 = (from x in list2
					where x.MTemplateID == item4.MGroupID
					select x).FirstOrDefault();
					pAPayItemModel.MGroupID = pAPayItemGroupModel2.MItemID;
					pAPayItemModel.MItemType = item4.MItemType;
					pAPayItemModel.MIsSys = item4.MIsSys;
					pAPayItemModel.MIsActive = item4.MIsActive;
					pAPayItemModel.MIsDelete = item4.MIsDelete;
					pAPayItemModel.MCreateDate = dateNow;
					pAPayItemModel.MModifyDate = dateNow;
					pAPayItemModel.MCoefficient = item4.MCoefficient;
					pAPayItemModel.MultiLanguage = item4.MultiLanguage;
					if (pAPayItemModel.MultiLanguage != null)
					{
						pAPayItemModel.MultiLanguage[0].MParentID = pAPayItemModel.MItemID;
						foreach (MultiLanguageField item5 in pAPayItemModel.MultiLanguage[0].MMultiLanguageField)
						{
							item5.MPKID = UUIDHelper.GetGuid();
						}
					}
					list3.Add(pAPayItemModel);
				}
				list.AddRange(GetPayItemInsertCmds(ctx, list3));
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			return operationResult;
		}

		public List<CommandInfo> GetPayItemGroupInsertCmds(MContext ctx, List<PAPayItemGroupModel> groupList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_pa_payitemgroup(MItemID,MOrgID,MIsSys,MItemType,MIsActive,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate,MCoefficient,MTemplateID,MAccountCode)";
			string text = " SELECT '{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}',{10},'{11}','{12}'";
			string value2 = "INSERT INTO t_pa_payitemgroup_l(MPKID,MParentID,MLocaleID,MName,MOrgID)";
			string format = "SELECT '{0}','{1}','{2}','{3}','{4}'";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			foreach (PAPayItemGroupModel group in groupList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				StringBuilder stringBuilder3 = stringBuilder;
				string format2 = text;
				object[] obj = new object[13]
				{
					group.MItemID,
					group.MOrgID,
					group.MIsSys,
					group.MItemType,
					group.MIsActive,
					group.MIsDelete,
					group.MCreatorID,
					null,
					null,
					null,
					null,
					null,
					null
				};
				DateTime dateTime = group.MCreateDate;
				obj[7] = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
				obj[8] = group.MModifierID;
				dateTime = group.MModifyDate;
				obj[9] = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
				obj[10] = group.MCoefficient;
				obj[11] = group.MTemplateID;
				obj[12] = group.MAccountCode;
				stringBuilder3.AppendFormat(format2, obj);
				MultiLanguageFieldList multiLanguageFieldList = group.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					stringBuilder2.AppendFormat(format, item.MPKID, multiLanguageFieldList.MParentID, item.MLocaleID, item.MValue, ctx.MOrgID);
					num2++;
				}
				num++;
			}
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			});
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			});
			return list;
		}

		public List<CommandInfo> GetPayItemInsertCmds(MContext ctx, List<PAPayItemModel> itemList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_pa_payitem(MItemID,MOrgID,MGroupID,MItemType,MIsSys,MIsActive,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate,MCoefficient,MAccountCode)";
			string text = " SELECT '{0}','{1}','{2}',{3},{4},{5},{6},'{7}','{8}','{9}','{10}',{11},'{12}'";
			string value2 = "INSERT INTO t_pa_payitem_l(MPKID,MParentID,MLocaleID,MName,MOrgID)";
			string format = "SELECT '{0}','{1}','{2}','{3}','{4}'";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			foreach (PAPayItemModel item in itemList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				StringBuilder stringBuilder3 = stringBuilder;
				string format2 = text;
				object[] obj = new object[13]
				{
					item.MItemID,
					item.MOrgID,
					item.MGroupID,
					item.MItemType,
					item.MIsSys,
					item.MIsActive,
					item.MIsDelete,
					item.MCreatorID,
					null,
					null,
					null,
					null,
					null
				};
				DateTime dateTime = item.MCreateDate;
				obj[8] = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
				obj[9] = item.MModifierID;
				dateTime = item.MModifyDate;
				obj[10] = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
				obj[11] = item.MCoefficient;
				obj[12] = item.MAccountCode;
				stringBuilder3.AppendFormat(format2, obj);
				MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				foreach (MultiLanguageField item2 in multiLanguageFieldList.MMultiLanguageField)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					stringBuilder2.AppendFormat(format, item2.MPKID, multiLanguageFieldList.MParentID, item2.MLocaleID, item2.MValue, ctx.MOrgID);
					num2++;
				}
				num++;
			}
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			});
			list.Add(new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			});
			return list;
		}

		private List<MultiLanguageFieldList> GetGroupItemMulitLang(MContext ctx, string id)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			DataSet multilanguage = GroupDal.GetMultilanguage(ctx, id);
			if (multilanguage != null && multilanguage.Tables != null)
			{
				MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
				foreach (DataRow row in multilanguage.Tables[0].Rows)
				{
					string localeID = (row["MLocaleID"] != null) ? row["MLocaleID"].ToString() : null;
					string value = (row["MName"] != null) ? row["MName"].ToString() : null;
					multiLanguageFieldList.CreateMultiLanguageFieldValue(localeID, value, null);
				}
				multiLanguageFieldList.MFieldName = "MName";
				list.Add(multiLanguageFieldList);
			}
			return list;
		}

		private List<MultiLanguageFieldList> GetItemMulitLang(MContext ctx, string id)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			DataSet multilanguage = dal.GetMultilanguage(ctx, id);
			if (multilanguage != null && multilanguage.Tables != null)
			{
				MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
				foreach (DataRow row in multilanguage.Tables[0].Rows)
				{
					string localeID = (row["MLocaleID"] != null) ? row["MLocaleID"].ToString() : null;
					string value = (row["MName"] != null) ? row["MName"].ToString() : null;
					multiLanguageFieldList.CreateMultiLanguageFieldValue(localeID, value, null);
				}
				multiLanguageFieldList.MFieldName = "MName";
				list.Add(multiLanguageFieldList);
			}
			return list;
		}

		private List<MultiLanguageFieldList> GetMulitLang(PayrollItemEnum type)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			if (type == PayrollItemEnum.BaseSalary)
			{
				MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
				multiLanguageFieldList.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Base Salary", null);
				multiLanguageFieldList.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "基本工资", null);
				multiLanguageFieldList.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "基本工資", null);
				multiLanguageFieldList.MFieldName = "MName";
				list.Add(multiLanguageFieldList);
			}
			if (type == PayrollItemEnum.Allowance)
			{
				MultiLanguageFieldList multiLanguageFieldList2 = new MultiLanguageFieldList();
				multiLanguageFieldList2.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Allowance", null);
				multiLanguageFieldList2.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "补贴", null);
				multiLanguageFieldList2.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "補貼", null);
				multiLanguageFieldList2.MFieldName = "MName";
				list.Add(multiLanguageFieldList2);
			}
			if (type == PayrollItemEnum.Bonus)
			{
				MultiLanguageFieldList multiLanguageFieldList3 = new MultiLanguageFieldList();
				multiLanguageFieldList3.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Bonus", null);
				multiLanguageFieldList3.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "年终奖", null);
				multiLanguageFieldList3.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "年終獎", null);
				multiLanguageFieldList3.MFieldName = "MName";
				list.Add(multiLanguageFieldList3);
			}
			if (type == PayrollItemEnum.Commission)
			{
				MultiLanguageFieldList multiLanguageFieldList4 = new MultiLanguageFieldList();
				multiLanguageFieldList4.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Commission", null);
				multiLanguageFieldList4.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "奖金", null);
				multiLanguageFieldList4.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "獎金", null);
				multiLanguageFieldList4.MFieldName = "MName";
				list.Add(multiLanguageFieldList4);
			}
			if (type == PayrollItemEnum.OverTime)
			{
				MultiLanguageFieldList multiLanguageFieldList5 = new MultiLanguageFieldList();
				multiLanguageFieldList5.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Over-Time", null);
				multiLanguageFieldList5.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "加班费", null);
				multiLanguageFieldList5.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "加班費", null);
				multiLanguageFieldList5.MFieldName = "MName";
				list.Add(multiLanguageFieldList5);
			}
			if (type == PayrollItemEnum.TaxAdjustment)
			{
				MultiLanguageFieldList multiLanguageFieldList6 = new MultiLanguageFieldList();
				multiLanguageFieldList6.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Tax Adjustment", null);
				multiLanguageFieldList6.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "税前调整", null);
				multiLanguageFieldList6.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "稅前調整", null);
				multiLanguageFieldList6.MFieldName = "MName";
				list.Add(multiLanguageFieldList6);
			}
			if (type == PayrollItemEnum.Attendance)
			{
				MultiLanguageFieldList multiLanguageFieldList7 = new MultiLanguageFieldList();
				multiLanguageFieldList7.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Attendance", null);
				multiLanguageFieldList7.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "出勤项", null);
				multiLanguageFieldList7.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "出勤項", null);
				multiLanguageFieldList7.MFieldName = "MName";
				list.Add(multiLanguageFieldList7);
			}
			if (type == PayrollItemEnum.EmployeeSocialSecurity)
			{
				MultiLanguageFieldList multiLanguageFieldList8 = new MultiLanguageFieldList();
				multiLanguageFieldList8.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Social Security (Employee)", null);
				multiLanguageFieldList8.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "社保（个人）", null);
				multiLanguageFieldList8.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "社保（個人）", null);
				multiLanguageFieldList8.MFieldName = "MName";
				list.Add(multiLanguageFieldList8);
			}
			if (type == PayrollItemEnum.EmployeeHousingProvidentFund)
			{
				MultiLanguageFieldList multiLanguageFieldList9 = new MultiLanguageFieldList();
				multiLanguageFieldList9.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Housing Provident Fund （Employee）", null);
				multiLanguageFieldList9.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "公积金(个人)", null);
				multiLanguageFieldList9.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "公積金(個人)", null);
				multiLanguageFieldList9.MFieldName = "MName";
				list.Add(multiLanguageFieldList9);
			}
			if (type == PayrollItemEnum.SalaryBeforePIT)
			{
				MultiLanguageFieldList multiLanguageFieldList10 = new MultiLanguageFieldList();
				multiLanguageFieldList10.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Total salary before P-I-T", null);
				multiLanguageFieldList10.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "税前工资", null);
				multiLanguageFieldList10.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "稅前工資", null);
				multiLanguageFieldList10.MFieldName = "MName";
				list.Add(multiLanguageFieldList10);
			}
			if (type == PayrollItemEnum.PIT)
			{
				MultiLanguageFieldList multiLanguageFieldList11 = new MultiLanguageFieldList();
				multiLanguageFieldList11.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "P-I-T", null);
				multiLanguageFieldList11.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "个人所得税", null);
				multiLanguageFieldList11.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "個人所得稅", null);
				multiLanguageFieldList11.MFieldName = "MName";
				list.Add(multiLanguageFieldList11);
			}
			if (type == PayrollItemEnum.SalaryAfterPIT)
			{
				MultiLanguageFieldList multiLanguageFieldList12 = new MultiLanguageFieldList();
				multiLanguageFieldList12.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Total salary after P-I-T", null);
				multiLanguageFieldList12.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "净工资", null);
				multiLanguageFieldList12.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "凈工資", null);
				multiLanguageFieldList12.MFieldName = "MName";
				list.Add(multiLanguageFieldList12);
			}
			if (type == PayrollItemEnum.EmployerSocialSecurity)
			{
				MultiLanguageFieldList multiLanguageFieldList13 = new MultiLanguageFieldList();
				multiLanguageFieldList13.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Social Security (Employer)", null);
				multiLanguageFieldList13.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "社保(公司)", null);
				multiLanguageFieldList13.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "社保(公司)", null);
				multiLanguageFieldList13.MFieldName = "MName";
				list.Add(multiLanguageFieldList13);
			}
			if (type == PayrollItemEnum.EmployerHousingProvidentFund)
			{
				MultiLanguageFieldList multiLanguageFieldList14 = new MultiLanguageFieldList();
				multiLanguageFieldList14.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Housing Provident Fund （Employer）", null);
				multiLanguageFieldList14.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "住房公积金(公司)", null);
				multiLanguageFieldList14.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "住房公積金(公司)", null);
				multiLanguageFieldList14.MFieldName = "MName";
				list.Add(multiLanguageFieldList14);
			}
			if (type == PayrollItemEnum.PBasicRetirementSecurity || type == PayrollItemEnum.CBasicRetirementSecurity)
			{
				MultiLanguageFieldList multiLanguageFieldList15 = new MultiLanguageFieldList();
				multiLanguageFieldList15.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Basic Retirement Security", null);
				multiLanguageFieldList15.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "养老保险", null);
				multiLanguageFieldList15.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "養老保險", null);
				multiLanguageFieldList15.MFieldName = "MName";
				list.Add(multiLanguageFieldList15);
			}
			if (type == PayrollItemEnum.PBasicMedicalInsurance || type == PayrollItemEnum.CBasicMedicalInsurance)
			{
				MultiLanguageFieldList multiLanguageFieldList16 = new MultiLanguageFieldList();
				multiLanguageFieldList16.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Basic Medical Insurance", null);
				multiLanguageFieldList16.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "医疗保险", null);
				multiLanguageFieldList16.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "醫療保險", null);
				multiLanguageFieldList16.MFieldName = "MName";
				list.Add(multiLanguageFieldList16);
			}
			if (type == PayrollItemEnum.PBasicUnemploymentInsurance || type == PayrollItemEnum.CBasicUnemploymentInsurance)
			{
				MultiLanguageFieldList multiLanguageFieldList17 = new MultiLanguageFieldList();
				multiLanguageFieldList17.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Basic Unemployment Insurance", null);
				multiLanguageFieldList17.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "失业保险", null);
				multiLanguageFieldList17.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "失業保險", null);
				multiLanguageFieldList17.MFieldName = "MName";
				list.Add(multiLanguageFieldList17);
			}
			if (type == PayrollItemEnum.PHousingProvidentFund || type == PayrollItemEnum.CHousingProvidentFund)
			{
				MultiLanguageFieldList multiLanguageFieldList18 = new MultiLanguageFieldList();
				multiLanguageFieldList18.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Housing Provident Fund", null);
				multiLanguageFieldList18.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "住房公积金", null);
				multiLanguageFieldList18.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "住房公積金(公司)", null);
				multiLanguageFieldList18.MFieldName = "MName";
				list.Add(multiLanguageFieldList18);
			}
			if (type == PayrollItemEnum.PAdditionHousingProvidentFund || type == PayrollItemEnum.CAdditionHousingProvidentFund)
			{
				MultiLanguageFieldList multiLanguageFieldList19 = new MultiLanguageFieldList();
				multiLanguageFieldList19.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Housing Provident Fund (Addition)", null);
				multiLanguageFieldList19.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "住房公积金(补充)", null);
				multiLanguageFieldList19.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "住房公積金(補充)", null);
				multiLanguageFieldList19.MFieldName = "MName";
				list.Add(multiLanguageFieldList19);
			}
			if (type == PayrollItemEnum.CIndustrialInjury)
			{
				MultiLanguageFieldList multiLanguageFieldList20 = new MultiLanguageFieldList();
				multiLanguageFieldList20.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Industrial Injury", null);
				multiLanguageFieldList20.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "工伤保险", null);
				multiLanguageFieldList20.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "工傷保險", null);
				multiLanguageFieldList20.MFieldName = "MName";
				list.Add(multiLanguageFieldList20);
			}
			if (type == PayrollItemEnum.CSeriousIllnessMedicalTreatment)
			{
				MultiLanguageFieldList multiLanguageFieldList21 = new MultiLanguageFieldList();
				multiLanguageFieldList21.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Serious Illness Medical Treatment", null);
				multiLanguageFieldList21.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "大病医疗", null);
				multiLanguageFieldList21.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "大病醫療", null);
				multiLanguageFieldList21.MFieldName = "MName";
				list.Add(multiLanguageFieldList21);
			}
			if (type == PayrollItemEnum.CMaternityInsurance)
			{
				MultiLanguageFieldList multiLanguageFieldList22 = new MultiLanguageFieldList();
				multiLanguageFieldList22.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Maternity Insurance", null);
				multiLanguageFieldList22.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "生育保险", null);
				multiLanguageFieldList22.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "生育保險", null);
				multiLanguageFieldList22.MFieldName = "MName";
				list.Add(multiLanguageFieldList22);
			}
			if (type == PayrollItemEnum.TotalSalary)
			{
				MultiLanguageFieldList multiLanguageFieldList23 = new MultiLanguageFieldList();
				multiLanguageFieldList23.CreateMultiLanguageFieldValue(LangCodeEnum.EN_US, "Total Salary", null);
				multiLanguageFieldList23.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_CN, "总工资", null);
				multiLanguageFieldList23.CreateMultiLanguageFieldValue(LangCodeEnum.ZH_TW, "總工資", null);
				multiLanguageFieldList23.MFieldName = "MName";
				list.Add(multiLanguageFieldList23);
			}
			return list;
		}

		public OperationResult ForbiddenSalaryItem(MContext ctx, string ids)
		{
			OperationResult operationResult = new OperationResult();
			if (!string.IsNullOrEmpty(ids))
			{
				string[] array = ids.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					SqlWhere sqlWhere = new SqlWhere();
					sqlWhere.Equal(" MItemID ", text);
					PAPayItemGroupModel dataModelByFilter = GroupDal.GetDataModelByFilter(ctx, sqlWhere);
					if (dataModelByFilter != null)
					{
						if (dataModelByFilter.MIsActive)
						{
							List<PAPayItemModel> salaryItemList = dal.GetSalaryItemList(ctx, text, null);
							if (salaryItemList != null && salaryItemList.Count > 0)
							{
								foreach (PAPayItemModel item in salaryItemList)
								{
									item.MIsActive = (!dataModelByFilter.MIsActive && true);
									dal.ForbiddenItem(ctx, item.MItemID, item.MIsActive);
								}
							}
						}
						dataModelByFilter.MIsActive = (!dataModelByFilter.MIsActive && true);
						GroupDal.ForbiddenItem(ctx, text, dataModelByFilter.MIsActive);
					}
					else
					{
						PAPayItemModel dataModelByFilter2 = dal.GetDataModelByFilter(ctx, sqlWhere);
						dataModelByFilter2.MIsActive = (!dataModelByFilter2.MIsActive && true);
						dal.ForbiddenItem(ctx, dataModelByFilter2.MItemID, dataModelByFilter2.MIsActive);
					}
					operationResult.Success = true;
				}
			}
			else
			{
				operationResult.Success = false;
			}
			return operationResult;
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "PayRun", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public List<PAPayItemModel> GetDisableItemList(MContext ctx)
		{
			List<PAPayItemGroupModel> dataModelList = ModelInfoManager.GetDataModelList<PAPayItemGroupModel>(ctx, new SqlWhere(), false, false);
			List<PAPayItemModel> list = PAPayItemRepository.GetPayItemList(ctx, false);
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			dictionary.Add(1035, new List<int>
			{
				1045,
				1040,
				1050
			});
			dictionary.Add(1055, new List<int>
			{
				1060,
				1065
			});
			dictionary.Add(2015, new List<int>
			{
				2025,
				2020,
				2035,
				2040,
				2030,
				2045,
				2050
			});
			dictionary.Add(2000, new List<int>
			{
				2005,
				2010
			});
			foreach (int key in dictionary.Keys)
			{
				List<int> childTypeList = dictionary[key];
				if (list.Any((PAPayItemModel f) => f.MItemType == key) && list.Count((PAPayItemModel f) => childTypeList.Contains(f.MItemType)) == childTypeList.Count)
				{
					list = (from f in list
					where !childTypeList.Contains(f.MItemType)
					select f).ToList();
				}
				else
				{
					PAPayItemGroupModel pAPayItemGroupModel = dataModelList.FirstOrDefault((PAPayItemGroupModel f) => f.MItemType == key);
					if (pAPayItemGroupModel != null)
					{
						foreach (int item in childTypeList)
						{
							PAPayItemModel pAPayItemModel = list.FirstOrDefault((PAPayItemModel f) => f.MItemType == item);
							if (pAPayItemModel != null)
							{
								pAPayItemModel.MName = $"{pAPayItemGroupModel.MName}-{pAPayItemModel.MName}";
							}
						}
					}
				}
			}
			return list;
		}

		public OperationResult Delete(MContext ctx, ParamBase param)
		{
			throw new NotImplementedException();
		}
	}
}
