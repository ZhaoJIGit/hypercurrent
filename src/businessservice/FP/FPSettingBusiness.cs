using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.FP
{
	public class FPSettingBusiness : IFPSettingBusiness, IDataContract<FPImportTypeConfigModel>, IBasicBusiness<FPSettingForAutoUploadModel>
	{
		private BASOrganisationRepository dalOrg = new BASOrganisationRepository();

		private FPSettingRepository dalFPSetting = new FPSettingRepository();

		public List<FPSettingForAutoUploadModel> GetFapiaoSettingList(MContext ctx, string orgIds)
		{
			List<FPSettingForAutoUploadModel> list = new List<FPSettingForAutoUploadModel>();
			SYSStorageRepository sYSStorageRepository = new SYSStorageRepository();
			var enumerable = from f in sYSStorageRepository.GetOrgServerList(ctx, orgIds, true)
			group f by new
			{
				f.MDBServerName,
				f.MDBServerPort,
				f.MUserName,
				f.MPassWord,
				f.MStandardDBName
			};
			foreach (var item in enumerable)
			{
				Dictionary<string, DateTime> dicList = item.ToDictionary((SYSStorageServerModel f) => f.MOrgID, (SYSStorageServerModel f) => f.MConversionDate);
				SYSStorageServerModel serverModel = new SYSStorageServerModel
				{
					MDBServerName = item.Key.MDBServerName,
					MDBServerPort = item.Key.MDBServerPort,
					MUserName = item.Key.MUserName,
					MPassWord = item.Key.MPassWord,
					MStandardDBName = item.Key.MStandardDBName
				};
				string serverConnectionString = sYSStorageRepository.GetServerConnectionString(serverModel);
				List<FPSettingForAutoUploadModel> fapiaoSettingList = dalFPSetting.GetFapiaoSettingList(dicList, serverConnectionString);
				list.AddRange(fapiaoSettingList);
			}
			return list;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dalFPSetting.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dalFPSetting.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FPImportTypeConfigModel modelData, string fields = null)
		{
			return dalFPSetting.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FPImportTypeConfigModel> modelData, string fields = null)
		{
			return dalFPSetting.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dalFPSetting.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dalFPSetting.DeleteModels(ctx, pkID);
		}

		public FPImportTypeConfigModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dalFPSetting.GetDataModelByFilter(ctx, filter);
		}

		public FPImportTypeConfigModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dalFPSetting.GetDataModel(ctx, pkID, includeDelete);
		}

		public List<FPImportTypeConfigModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dalFPSetting.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FPImportTypeConfigModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dalFPSetting.GetModelPageList(ctx, filter, includeDelete);
		}

		public OperationResult SaveImportTypeConfig(MContext ctx, FPImportTypeConfigModel model)
		{
			return dalFPSetting.SaveImportTypeConfig(ctx, model);
		}

		public OperationResult SaveFaPiaoSetting(MContext ctx, FPConfigSettingSaveModel model)
		{
			return dalFPSetting.SaveFaPiaoSetting(ctx, model);
		}

		public FPSettingForAutoUploadModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}

		public DataGridJson<FPSettingForAutoUploadModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<FPSettingForAutoUploadModel> dataGridJson = new DataGridJson<FPSettingForAutoUploadModel>();
			List<BASOrganisationModel> rows = dalOrg.GetOrgListWithFPChangeAuth(ctx, param).rows;
			string text = string.Join(",", from f in rows
			select f.MItemID);
			List<FPSettingForAutoUploadModel> list = new List<FPSettingForAutoUploadModel>();
			if (text.Any())
			{
				list = GetFapiaoSettingList(ctx, text);
				foreach (FPSettingForAutoUploadModel item in list)
				{
					BASOrganisationModel bASOrganisationModel = rows.FirstOrDefault((BASOrganisationModel f) => f.MItemID == item.MOrgID);
					if (bASOrganisationModel != null)
					{
						item.MOrgName = bASOrganisationModel.MLegalTradingName;
						item.MViewBizObjectIDs = bASOrganisationModel.MViewBizObjectIDs;
						item.MChangeBizObjectIDs = bASOrganisationModel.MChangeBizObjectIDs;
						item.MIsOrgExpired = bASOrganisationModel.MIsExpired;
						item.MIsOrgDelete = bASOrganisationModel.MIsDelete;
					}
				}
			}
			dataGridJson.rows = list;
			dataGridJson.total = list.Count;
			return dataGridJson;
		}

		public List<CommandInfo> GetFpLastUploadTimeUpdateCmds(MContext ctx, int type, DateTime dt)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<FPImportTypeConfigModel> dataModelList = ModelInfoManager.GetDataModelList<FPImportTypeConfigModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MType", type), false, false);
			foreach (FPImportTypeConfigModel item in dataModelList)
			{
				item.MLastUploadDate = dt;
			}
			if (dataModelList.Any())
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, dataModelList, null, true));
			}
			return list;
		}

		public List<FPSettingForAutoUploadModel> Post(MContext ctx, PostParam<FPSettingForAutoUploadModel> param)
		{
			OperationResult operationResult = new OperationResult();
			List<FPSettingForAutoUploadModel> dataList = param.DataList;
			if (!dataList.Any())
			{
				return new List<FPSettingForAutoUploadModel>();
			}
			if (dataList.Count == 1 && dataList[0].MLastUploadDate != DateTime.MinValue)
			{
				List<CommandInfo> fpLastUploadTimeUpdateCmds = GetFpLastUploadTimeUpdateCmds(ctx, dataList[0].MType, DateTime.Now);
				if (fpLastUploadTimeUpdateCmds.Any())
				{
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(fpLastUploadTimeUpdateCmds) > 0);
					if (!operationResult.Success)
					{
						dataList[0].ValidationErrors = new List<ValidationError>();
						dataList[0].ValidationErrors.Add(new ValidationError(operationResult.Message));
					}
				}
			}
			else
			{
				operationResult = ModelInfoManager.InsertOrUpdate(ctx, dataList, null);
				if (!operationResult.Success)
				{
					throw new Exception(operationResult.Message);
				}
			}
			return dataList;
		}
	}
}
