using JieNor.Megi.Core;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.DataRepository.FP
{
	public class ApiFPFapiaoRepository
	{
		private string CommonSelect = "SELECT \n                                            t1.MID,\n                                            t1.MID as MFapiaoID,\n                                            t1.MCode,\n                                            t1.MNumber,\n                                            t1.MNumber as MFaPiaoNumber,\n                                            t1.MBizDate,\n                                            t1.MInvoiceType,\n                                            t1.MIsCredit,\n                                            t1.MType,\n                                            t1.MByAgency,\n                                            t1.MValidateCode,\n                                            t1.MPassword,\n                                            t1.MExplanation,\n                                            t1.MPContactName,\n                                            t1.MPContactTaxCode,\n                                            t1.MPContactAddressPhone,\n                                            t1.MPContactBankInfo,\n                                            t1.MSContactName,\n                                            t1.MSContactTaxCode,\n                                            t1.MSContactAddressPhone,\n                                            t1.MSContactBankInfo,\n                                            t1.MCodingStatus,\n                                            t1.MReconcileStatus,\n                                            t1.MReceiver,\n                                            t1.MReaduitor,\n                                            t1.MDrawer,\n                                            t1.MAmount,\n                                            t1.MSource,\n                                            t1.MTaxAmount,\n                                            t1.MTotalAmount,\n                                            t1.MVerifyType,\n                                            t1.MVerifyDate,\n                                            t1.MStatus,   \n                                            t1.MCreateBy,\n                                            t1.MModifyDate,\n                                            t2.MEntryID AS MFapiaoEntrys_MFapiaoLineID,\n                                            t2.MItemName AS MFapiaoEntrys_MItemName,\n                                            t2.MItemCategoryCode AS MFapiaoEntrys_MItemCategoryCode,\n                                            t2.MItemType AS MFapiaoEntrys_MItemType,\n                                            t2.MUnit AS MFapiaoEntrys_MUnit,\n                                            t2.MQuantity AS MFapiaoEntrys_MQuantity,\n                                            t2.MPrice AS MFapiaoEntrys_MPrice,\n                                            t2.MAmount AS MFapiaoEntrys_MAmount,\n                                            t2.MTaxPercent AS MFapiaoEntrys_MTaxPercent,\n                                            t2.MTaxAmount AS MFapiaoEntrys_MTaxAmount,\n                                            t2.MSeq AS MFapiaoEntrys_MSeq\n\n                                        FROM\n                                            t_fp_fapiao t1\n                                                INNER JOIN\n                                            t_fp_fapiaoentry t2 ON t1.MID = t2.MID\n                                                AND t1.MOrgID = t2.MOrgID\n                                                AND t1.MIsDelete = t2.MIsDelete       \n                                        WHERE\n                                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0";

		public DataGridJson<FPFapiaoModel> Get(MContext ctx, GetParam param)
		{
			return new APIDataRepository().Get<FPFapiaoModel>(ctx, param, CommonSelect, false, true, null);
		}
	}
}
