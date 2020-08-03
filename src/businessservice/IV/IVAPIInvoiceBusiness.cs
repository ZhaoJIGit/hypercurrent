using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVAPIInvoiceBusiness : IVAPIInvoiceBaseBusiness, IBasicBusiness<IVAPIInvoiceModel>
	{
		private APIBusinessBase<IVInvoiceModel> bizApiBase = new APIBusinessBase<IVInvoiceModel>();

		public IVAPIInvoiceModel Delete(MContext ctx, DeleteParam param)
		{
			CheckEndPoint(ctx);
			GetParam getParam = new GetParam();
			getParam.ElementID = param.ElementID;
			IVAPIInvoiceModel iVAPIInvoiceModel = string.IsNullOrWhiteSpace(param.ElementID) ? null : Get(ctx, getParam).rows.FirstOrDefault();
			bizApiBase.CheckRequestResourceExist(ctx, iVAPIInvoiceModel == null);
			if (iVAPIInvoiceModel.ValidationErrors == null)
			{
				iVAPIInvoiceModel.ValidationErrors = new List<ValidationError>();
			}
			if (iVAPIInvoiceModel.MStatus >= 3)
			{
				string message = (iVAPIInvoiceModel.MType == "Invoice_Sale" || iVAPIInvoiceModel.MType == "Invoice_Purchase") ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteApprovedInvoiceFail", "只有“草稿”和“提交审核”状态的账单才可以被删除。") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteApprovedCreditNoteFail", "只有“草稿”和“提交审核”状态的红字账单才可以被删除。");
				iVAPIInvoiceModel.ValidationErrors.Add(new ValidationError
				{
					Message = message
				});
				return iVAPIInvoiceModel;
			}
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = param.ElementID;
			OperationResult operationResult = IVInvoiceRepository.DeleteInvoiceList(ctx, paramBase);
			if (!operationResult.Success)
			{
				iVAPIInvoiceModel.ValidationErrors.Add(new ValidationError
				{
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteInvoiceFail", " 删除单据失败！")
				});
			}
			return iVAPIInvoiceModel;
		}

		public DataGridJson<IVAPIInvoiceModel> Get(MContext ctx, GetParam param)
		{
			CheckEndPoint(ctx);
			if (!param.IncludeDetail.HasValue)
			{
				param.IncludeDetail = true;
			}
			DataGridJson<IVAPIInvoiceModel> invoice = base._repInvoice.GetInvoice(ctx, param);
			bizApiBase.CheckRequestResourceExist(ctx, !string.IsNullOrWhiteSpace(param.ElementID) && invoice.rows.Count == 0);
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			if (invoice != null && invoice.rows.Count > 0)
			{
				List<IVInvoiceModel> modelList = invoice.rows.Cast<IVInvoiceModel>().ToList();
				base.OnGetAfter(ctx, param, instance, modelList);
			}
			return invoice;
		}

		public List<IVAPIInvoiceModel> Post(MContext ctx, PostParam<IVAPIInvoiceModel> param)
		{
			CheckEndPoint(ctx);
			List<IVAPIInvoiceModel> dbDataList = new List<IVAPIInvoiceModel>();
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MID)
			select t.MID).ToList();
			List<string> list2 = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MNumber)
			select t.MNumber).ToList();
			if (list.Count > 0 || list2.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				if (list.Count > 0)
				{
					base.SetWhereString(param2, "InvoiceID", list, true);
				}
				if (list2.Count > 0)
				{
					base.SetWhereString(param2, "InvoiceNumber", list2, false);
				}
				DataGridJson<IVAPIInvoiceModel> invoice = base._repInvoice.GetInvoice(ctx, param2);
				dbDataList = invoice.rows;
			}
			base.Post(ctx, param.IsPut, param.DataList, dbDataList);
			list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MID) && t.ValidationErrors.Count == 0
			select t.MID).ToList();
			if (list.Count > 0)
			{
				GetParam param3 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true),
					FromPost = true
				};
				base.SetWhereString(param3, "InvoiceID", list, true);
				DataGridJson<IVAPIInvoiceModel> dataGridJson = Get(ctx, param3);
				List<IVAPIInvoiceModel> list3 = new List<IVAPIInvoiceModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					IVAPIInvoiceModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list3.Add(model);
					}
					else
					{
						IVAPIInvoiceModel item = dataGridJson.rows.FirstOrDefault((IVAPIInvoiceModel a) => a.MInvoiceID == model.MInvoiceID);
						list3.Add(item);
					}
				}
				param.DataList = list3;
			}
			return param.DataList;
		}

		private void CheckEndPoint(MContext ctx)
		{
			bizApiBase.CheckEndpointAvailable(ctx, 1, "Invoices");
		}
	}
}
