using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.BusinessService.FP;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FP;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FP
{
	public class FPSettingService : ServiceT<FPImportTypeConfigModel>, IFPSetting
	{
		private readonly IFPSettingBusiness biz = new FPSettingBusiness();

		public MActionResult<OperationResult> SaveImportTypeConfig(FPImportTypeConfigModel model, string accessToken = null)
		{
			IFPSettingBusiness iFPSettingBusiness = biz;
			return base.RunFunc(iFPSettingBusiness.SaveImportTypeConfig, model, accessToken);
		}

		public MActionResult<OperationResult> SaveFaPiaoSetting(FPConfigSettingSaveModel model, string accessToken = null)
		{
			IFPSettingBusiness iFPSettingBusiness = biz;
			return base.RunFunc(iFPSettingBusiness.SaveFaPiaoSetting, model, accessToken);
		}

		public MActionResult<FPImportTypeConfigModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IFPSettingBusiness iFPSettingBusiness = biz;
			return base.GetDataModel(iFPSettingBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<List<FPImportTypeConfigModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IFPSettingBusiness iFPSettingBusiness = biz;
			return base.GetModelList(iFPSettingBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
