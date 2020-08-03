using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FA
{
	public interface IFAFixAssetsBusiness : IDataContract<FAFixAssetsModel>
	{
		GLCheckGroupValueModel GetMergeCheckGroupValueModel(MContext ctx, List<string> checkGroupValueIDs);

		DataGridJson<FAFixAssetsModel> GetFixAssetsPageList(MContext ctx, FAFixAssetsFilterModel filter = null);

		FAFixAssetsModel GetFixAssetsModel(MContext ctx, string itemID = null, bool isCopy = false);

		OperationResult DeleteFixAssets(MContext ctx, List<string> itemIDs);

		OperationResult SaveFixAssets(MContext ctx, FAFixAssetsModel model);

		List<NameValueModel> GetFixAssetsTabInfo(MContext ctx);

		OperationResult HandleFixAssets(MContext ctx, List<string> itemIDs, int type);

		OperationResult SetExpenseAccountDefault(MContext ctx, bool check, string accountCode);

		List<DateTime> GetFAPeriodFromBeginDate(MContext ctx, bool includeCurrentPeriod = false);

		ImportTemplateModel GetImportTemplateModel(MContext ctx);

		List<FAFixAssetsModel> GetFixAssetsList(MContext ctx, FAFixAssetsFilterModel filter);

		OperationResult ImportAssetCardList(MContext ctx, List<FAFixAssetsModel> assetCardList);
	}
}
