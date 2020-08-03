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
	public class IVAPICreditNoteBusiness : IVAPIInvoiceBaseBusiness, IBasicBusiness<IVAPICreditNoteModel>
	{
		private APIBusinessBase<IVAPICreditNoteModel> bizApiBase = new APIBusinessBase<IVAPICreditNoteModel>();

		public IVAPICreditNoteModel Delete(MContext ctx, DeleteParam param)
		{
			CheckEndPoint(ctx);
			GetParam getParam = new GetParam();
			getParam.ElementID = param.ElementID;
			IVAPICreditNoteModel iVAPICreditNoteModel = string.IsNullOrWhiteSpace(param.ElementID) ? null : Get(ctx, getParam).rows.FirstOrDefault();
			bizApiBase.CheckRequestResourceExist(ctx, iVAPICreditNoteModel == null);
			if (iVAPICreditNoteModel.ValidationErrors == null)
			{
				iVAPICreditNoteModel.ValidationErrors = new List<ValidationError>();
			}
			if (iVAPICreditNoteModel.MStatus >= 3)
			{
				iVAPICreditNoteModel.ValidationErrors.Add(new ValidationError
				{
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteApprovedInvoiceFail", "已审核的单据不能删除！")
				});
				return iVAPICreditNoteModel;
			}
			ParamBase paramBase = new ParamBase();
			paramBase.KeyIDs = param.ElementID;
			OperationResult operationResult = IVInvoiceRepository.DeleteInvoiceList(ctx, paramBase);
			if (!operationResult.Success)
			{
				iVAPICreditNoteModel.ValidationErrors.Add(new ValidationError
				{
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteInvoiceFail", " 删除单据失败！")
				});
			}
			return iVAPICreditNoteModel;
		}

		public DataGridJson<IVAPICreditNoteModel> Get(MContext ctx, GetParam param)
		{
			CheckEndPoint(ctx);
			if (!param.IncludeDetail.HasValue)
			{
				param.IncludeDetail = true;
			}
			DataGridJson<IVAPICreditNoteModel> creditNote = base._repInvoice.GetCreditNote(ctx, param);
			bizApiBase.CheckRequestResourceExist(ctx, !string.IsNullOrWhiteSpace(param.ElementID) && creditNote.rows.Count == 0);
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			if (creditNote != null && creditNote.rows.Count > 0)
			{
				List<IVInvoiceModel> modelList = creditNote.rows.Cast<IVInvoiceModel>().ToList();
				base.OnGetAfter(ctx, param, instance, modelList);
			}
			return creditNote;
		}

		public List<IVAPICreditNoteModel> Post(MContext ctx, PostParam<IVAPICreditNoteModel> param)
		{
			CheckEndPoint(ctx);
			List<IVAPICreditNoteModel> dbDataList = new List<IVAPICreditNoteModel>();
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
					base.SetWhereString(param2, "CreditNoteID", list, true);
				}
				if (list2.Count > 0)
				{
					base.SetWhereString(param2, "CreditNoteNumber", list2, false);
				}
				DataGridJson<IVAPICreditNoteModel> creditNote = base._repInvoice.GetCreditNote(ctx, param2);
				dbDataList = creditNote.rows;
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
				base.SetWhereString(param3, "CreditNoteID", list, true);
				DataGridJson<IVAPICreditNoteModel> dataGridJson = Get(ctx, param3);
				List<IVAPICreditNoteModel> list3 = new List<IVAPICreditNoteModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					IVAPICreditNoteModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list3.Add(model);
					}
					else
					{
						IVAPICreditNoteModel item = dataGridJson.rows.FirstOrDefault((IVAPICreditNoteModel a) => a.MCreditNoteID == model.MCreditNoteID);
						list3.Add(item);
					}
				}
				param.DataList = list3;
			}
			return param.DataList;
		}

		private void CheckEndPoint(MContext ctx)
		{
			bizApiBase.CheckEndpointAvailable(ctx, 1, "CreditNotes");
		}
	}
}
