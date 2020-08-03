using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.REG
{
	public class REGFinancialBusiness : IREGFinancialBusiness, IDataContract<REGFinancialModel>
	{
		private REGFinancialRepository _repFinancial = new REGFinancialRepository();

		private BDAttachmentRepository _repAttachment = new BDAttachmentRepository();

		private REGTaxRateRepository taxRateDal = new REGTaxRateRepository();

		private BASOrganisationRepository _org = new BASOrganisationRepository();

		private BASOrgInitSettingBusiness _orgInitBiz = new BASOrgInitSettingBusiness();

		private readonly REGFinancialRepository dal = new REGFinancialRepository();

		public OperationResult UpdateByOrgID(MContext ctx, REGFinancialModel model)
		{
			REGFinancialModel byOrgID = _repFinancial.GetByOrgID(ctx, model);
			if (byOrgID != null && byOrgID.MTaxPayer != model.MTaxPayer && ctx.MRegProgress < 15)
			{
				taxRateDal.DeleteUnUsedTaxRate(ctx);
			}
			OperationResult operationResult = _repFinancial.UpdateByOrgID(ctx, model);
			DateTime date = Convert.ToDateTime(model.MConversionDate).Date;
			if (operationResult.Success && ctx.MRegProgress < 15 && (byOrgID == null || model.MCurrencyID != ctx.MBasCurrencyID || date != ctx.MGLBeginDate || model.MAccountingStandard != ctx.MAccountTableID))
			{
				ResetAccount(ctx, model, operationResult);
			}
			return operationResult;
		}

		private void ResetAccount(MContext ctx, REGFinancialModel model, OperationResult result)
		{
			DateTime date = Convert.ToDateTime(model.MConversionDate).Date;
			ctx.IsSys = true;
			BASOrganisationModel dataEditModel = ModelInfoManager.GetDataEditModel<BASOrganisationModel>(ctx, ctx.MOrgID, false, true);
			List<string> list = new List<string>();
			if (dataEditModel.MOriRegProgress < 5)
			{
				dataEditModel.MConversionDate = date;
				list.Add("MConversionDate");
				ctx.MBeginDate = dataEditModel.MConversionDate;
			}
			dataEditModel.MInitBalanceOver = false;
			list.Add("MInitBalanceOver");
			result = BASOrganisationRepository.UpdateOrgInfo(ctx, dataEditModel, list);
			ctx.IsSys = false;
			if (model.MCurrencyID != ctx.MBasCurrencyID)
			{
				ctx.MBasCurrencyID = model.MCurrencyID;
			}
			ctx.MInitBalanceOver = false;
			ContextHelper.MContext = ctx;
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			bDBankAccountRepository.InsertDefaultCashAccount(ctx);
			BASOrgInitSettingModel bASOrgInitSettingModel = _orgInitBiz.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID));
			if (bASOrgInitSettingModel == null)
			{
				bASOrgInitSettingModel = new BASOrgInitSettingModel();
			}
			ctx.IsSys = false;
			bASOrgInitSettingModel.MOrgID = ctx.MOrgID;
			bASOrgInitSettingModel.MConversionDate = date;
			bASOrgInitSettingModel.MAccountingStandard = model.MAccountingStandard;
			result = _orgInitBiz.InsertOrUpdate(ctx, bASOrgInitSettingModel, null);
		}

		public REGFinancialModel GetByOrgID(MContext ctx, REGFinancialModel model)
		{
			REGFinancialModel byOrgID = _repFinancial.GetByOrgID(ctx, model);
			if (byOrgID == null)
			{
				return new REGFinancialModel();
			}
			if (!string.IsNullOrEmpty(byOrgID.MTaxRegCertCopyAttachId))
			{
				byOrgID.MTaxRegCertCopyAttachModel = _repAttachment.GetAttachmentModel(byOrgID.MTaxRegCertCopyAttachId, ctx, false);
			}
			if (!string.IsNullOrEmpty(byOrgID.MLocalTaxRegCertCopyAttachId))
			{
				byOrgID.MLocalTaxRegCertCopyAttachModel = _repAttachment.GetAttachmentModel(byOrgID.MLocalTaxRegCertCopyAttachId, ctx, false);
			}
			if (!string.IsNullOrEmpty(model.TobeDelOriginalAttachIds))
			{
				ParamBase paramBase = new ParamBase();
				paramBase.KeyIDs = model.TobeDelOriginalAttachIds;
				_repAttachment.DeleteAttachmentList(ctx, paramBase);
			}
			return byOrgID;
		}

		public TaxPayerEnum GetTaxPayer(MContext ctx)
		{
			REGFinancialModel dataModelByFilter = GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID));
			return (TaxPayerEnum)Convert.ToInt32(dataModelByFilter.MTaxPayer);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, REGFinancialModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<REGFinancialModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public REGFinancialModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public REGFinancialModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<REGFinancialModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<REGFinancialModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
