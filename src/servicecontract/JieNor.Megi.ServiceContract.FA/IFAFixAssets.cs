using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FA
{
	[ServiceContract]
	public interface IFAFixAssets
	{
		[OperationContract]
		MActionResult<GLCheckGroupValueModel> GetMergeCheckGroupValueModel(List<string> checkGroupValueIDs, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FAFixAssetsModel>> GetFixAssetsPageList(FAFixAssetsFilterModel filter = null, string accessToken = null);

		[OperationContract]
		MActionResult<FAFixAssetsModel> GetFixAssetsModel(string itemID = null, bool isCopy = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFixAssets(List<string> itemIDs, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveFixAssets(FAFixAssetsModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetFixAssetsTabInfo(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> HandleFixAssets(List<string> itemIDs, int type, string accessToke = null);

		[OperationContract]
		MActionResult<OperationResult> SetExpenseAccountDefault(bool check, string accountCode, string accessToken = null);

		[OperationContract]
		MActionResult<List<DateTime>> GetFAPeriodFromBeginDate(bool includeCurrentPeriod = false, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(string accessToken = null);

		[OperationContract]
		MActionResult<List<FAFixAssetsModel>> GetFixAssetsList(FAFixAssetsFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportAssetCardList(List<FAFixAssetsModel> assetCardList, string accessToken = null);
	}
}
