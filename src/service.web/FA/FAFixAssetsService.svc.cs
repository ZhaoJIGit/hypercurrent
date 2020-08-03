using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.BusinessService.FA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FA;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FA
{
	public class FAFixAssetsService : ServiceT<FAFixAssetsModel>, IFAFixAssets
	{
		private readonly IFAFixAssetsBusiness biz = new FAFixAssetsBusiness();

		public MActionResult<GLCheckGroupValueModel> GetMergeCheckGroupValueModel(List<string> checkGroupValueIDs, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetMergeCheckGroupValueModel, checkGroupValueIDs, accessToken);
		}

		public MActionResult<DataGridJson<FAFixAssetsModel>> GetFixAssetsPageList(FAFixAssetsFilterModel filter = null, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetFixAssetsPageList, filter, accessToken);
		}

		public MActionResult<List<FAFixAssetsModel>> GetFixAssetsList(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetFixAssetsList, filter, accessToken);
		}

		public MActionResult<OperationResult> DeleteFixAssets(List<string> itemIDs, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.DeleteFixAssets, itemIDs, accessToken);
		}

		public MActionResult<OperationResult> SaveFixAssets(FAFixAssetsModel model, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.SaveFixAssets, model, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetFixAssetsTabInfo(string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetFixAssetsTabInfo, accessToken);
		}

		public MActionResult<OperationResult> HandleFixAssets(List<string> itemIDs, int type, string accessToke = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.HandleFixAssets, itemIDs, type, null);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetImportTemplateModel, accessToken);
		}

		public MActionResult<OperationResult> ImportAssetCardList(List<FAFixAssetsModel> assetCardList, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.ImportAssetCardList, assetCardList, accessToken);
		}

		public MActionResult<FAFixAssetsModel> GetFixAssetsModel(string itemID = null, bool isCopy = false, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetFixAssetsModel, itemID, isCopy, accessToken);
		}

		public MActionResult<OperationResult> SetExpenseAccountDefault(bool check, string accountCode, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.SetExpenseAccountDefault, check, accountCode, accessToken);
		}

		public MActionResult<List<DateTime>> GetFAPeriodFromBeginDate(bool includeCurrentPeriod = false, string accessToken = null)
		{
			IFAFixAssetsBusiness iFAFixAssetsBusiness = biz;
			return base.RunFunc(iFAFixAssetsBusiness.GetFAPeriodFromBeginDate, includeCurrentPeriod, accessToken);
		}
	}
}
