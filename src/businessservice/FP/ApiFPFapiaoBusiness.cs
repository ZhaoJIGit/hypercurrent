using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.BusinessService.FP
{
	public class ApiFPFapiaoBusiness : APIBusinessBase<FPFapiaoModel>
	{
		private readonly ApiFPFapiaoRepository apiFPFapiaoRepository = new ApiFPFapiaoRepository();

		private List<FPFapiaoModel> _dbList = new List<FPFapiaoModel>();

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
		}

		protected override DataGridJson<FPFapiaoModel> OnGet(MContext ctx, GetParam param)
		{
			if (!param.IncludeDetail.HasValue)
			{
				param.IncludeDetail = true;
			}
			return apiFPFapiaoRepository.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, FPFapiaoModel model)
		{
			if (param.IncludeDetail.HasValue && !param.IncludeDetail.Value)
			{
				model.MFapiaoEntrys = null;
			}
			else
			{
				model.MFapiaoEntrys = (from a in model.MFapiaoEntrys
				orderby a.MSeq
				select a).ToList();
			}
		}

		protected override void OnPostBefore(MContext ctx, PostParam<FPFapiaoModel> param, APIDataPool dataPool)
		{
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MID)
			select t.MID).ToList();
			GetParam param2 = new GetParam
			{
				MOrgID = ctx.MOrgID,
				MUserID = ctx.MUserID,
				IncludeDetail = new bool?(true)
			};
			if (list.Count > 0)
			{
				base.SetWhereString(param2, "MFapiaoID", list, true);
			}
			DataGridJson<FPFapiaoModel> dataGridJson = apiFPFapiaoRepository.Get(ctx, param2);
			_dbList = dataGridJson.rows;
		}

		protected override void OnPostValidate(MContext ctx, PostParam<FPFapiaoModel> param, APIDataPool dataPool, FPFapiaoModel model, bool isPut, ref Dictionary<string, List<string>> validNameList, ref Dictionary<string, string> updNameList)
		{
			ProcessFapiaoModel(ctx, model, isPut);
			SetFapiaoType(model, ctx);
			SetVerifyType(model, ctx);
			if (model.ValidationErrors.Count == 0)
			{
				_dbList.Add(model);
			}
		}

		private void ProcessFapiaoModel(MContext ctx, FPFapiaoModel model, bool isPut)
		{
			FPFapiaoModel fPFapiaoModel = null;
			if (!string.IsNullOrEmpty(model.MID))
			{
				fPFapiaoModel = _dbList.FirstOrDefault((FPFapiaoModel t) => t.MID == model.MID);
			}
			if (!string.IsNullOrEmpty(model.MCode) && !string.IsNullOrEmpty(model.MNumber) && fPFapiaoModel == null)
			{
				fPFapiaoModel = _dbList.FirstOrDefault((FPFapiaoModel t) => t.MCode == model.MCode && t.MNumber == model.MNumber);
			}
			if (fPFapiaoModel != null & isPut)
			{
				model.Validate(ctx, true, "CodeCombNumberMustUnique", "发票的“发票代码+发票号码”组合不能重复。", LangModule.FP);
			}
			model.IsNew = (fPFapiaoModel == null);
			if (fPFapiaoModel == null)
			{
				model.MCreateBy = ctx.MConsumerKey;
				model.UpdateFieldList.Add("MCreateBy");
				model.MSource = 3;
			}
			bool flag = model.IsUpdateFieldExists("MCode");
			if (!flag)
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Code");
			}
			if (flag && string.IsNullOrEmpty(model.MCode))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Code");
			}
			bool flag2 = model.IsUpdateFieldExists("MNumber");
			if (!flag2)
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Number");
			}
			if (flag2 && string.IsNullOrEmpty(model.MNumber))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Number");
			}
			if (!model.IsUpdateFieldExists("MBizDate"))
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Date");
			}
			else if (model.MBizDate == DateTime.MinValue)
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Date");
			}
			if (!model.IsUpdateFieldExists("MType"))
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Type");
			}
			bool flag3 = model.IsUpdateFieldExists("MSContactName");
			if (!flag3)
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "SellerName");
			}
			if (flag3 && string.IsNullOrEmpty(model.MSContactName))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "SellerName");
			}
			bool flag4 = model.IsUpdateFieldExists("MPContactName");
			if (!flag4)
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "BuyerName");
			}
			if (flag4 && string.IsNullOrEmpty(model.MPContactName))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "BuyerName");
			}
			bool flag5 = false;
			ProcessEntryCount(ctx, model, fPFapiaoModel, ref flag5);
			if (!flag5)
			{
				bool flag6 = false;
				ProcessEntryList(ctx, model, fPFapiaoModel, ref flag6);
				if (!flag6)
				{
					List<FPFapiaoEntryModel> mFapiaoEntrys = model.MFapiaoEntrys;
					model.MAmount = mFapiaoEntrys.Sum((FPFapiaoEntryModel m) => m.MAmount);
					model.MTaxAmount = mFapiaoEntrys.Sum((FPFapiaoEntryModel m) => m.MTaxAmount);
					model.MTotalAmount = model.MAmount + model.MTaxAmount;
					if (DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MAmount)))
					{
						string msg = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DecimalCanNotExceedMaxInteger", "“{0}”的整数位不能超过12位。"), "SubTotal");
						if (model.ValidationErrors.All((ValidationError a) => a.Message != msg))
						{
							model.ValidationErrors.Add(new ValidationError(msg));
						}
					}
					if (DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MTaxAmount)))
					{
						string msg2 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DecimalCanNotExceedMaxInteger", "“{0}”的整数位不能超过12位。"), "TotalTax");
						if (model.ValidationErrors.All((ValidationError a) => a.Message != msg2))
						{
							model.ValidationErrors.Add(new ValidationError(msg2));
						}
					}
					if (DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MTotalAmount)))
					{
						string msg3 = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DecimalCanNotExceedMaxInteger", "“{0}”的整数位不能超过12位。"), "Total");
						if (model.ValidationErrors.All((ValidationError a) => a.Message != msg3))
						{
							model.ValidationErrors.Add(new ValidationError(msg3));
						}
					}
					model.MStatus = ((!(model.MTotalAmount < decimal.Zero)) ? 1 : 4);
				}
			}
		}

		private void ProcessEntryList(MContext ctx, FPFapiaoModel model, FPFapiaoModel dbModel, ref bool entryListError)
		{
			int num = 0;
			foreach (FPFapiaoEntryModel mFapiaoEntry in model.MFapiaoEntrys)
			{
				mFapiaoEntry.MSeq = num;
				FPFapiaoEntryModel fPFapiaoEntryModel = null;
				if (dbModel != null)
				{
					fPFapiaoEntryModel = dbModel.MFapiaoEntrys.FirstOrDefault((FPFapiaoEntryModel a) => a.MEntryID == mFapiaoEntry.MEntryID);
				}
				else
				{
					mFapiaoEntry.MEntryID = UUIDHelper.GetGuid();
					mFapiaoEntry.IsNew = true;
				}
				if (!ProcessEntryModel(ctx, model, mFapiaoEntry) || mFapiaoEntry.ValidationErrors.Count > 0)
				{
					model.ValidationErrors = model.ValidationErrors.Concat(mFapiaoEntry.ValidationErrors).ToList();
					mFapiaoEntry.ValidationErrors.Clear();
					entryListError = true;
				}
				num++;
			}
		}

		private bool ProcessEntryModel(MContext ctx, FPFapiaoModel model, FPFapiaoEntryModel entryModel)
		{
			bool result = true;
			if (entryModel.MPrice < decimal.Zero)
			{
				entryModel.Validate(ctx, true, "PriceMustGreaterThanZero", "单价必须大于等于0。", LangModule.FP);
				result = false;
			}
			if (entryModel.MTaxPercent < decimal.Zero || entryModel.MTaxPercent > 100m)
			{
				entryModel.Validate(ctx, true, "TaxRateBetween0And100", "税率必须为0~100之间的数。", LangModule.FP);
				result = false;
			}
			if (entryModel.MTaxPercent == decimal.Zero && entryModel.MTaxAmount != decimal.Zero)
			{
				entryModel.Validate(ctx, true, "RateMustBe0WhenAmountBe0", "税率为0时，税额必须也为0。", LangModule.FP);
				result = false;
			}
			bool flag = entryModel.UpdateFieldList.Contains("MTaxAmount");
			bool flag2 = entryModel.UpdateFieldList.Contains("MAmount");
			if (!flag)
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "TaxAmount");
				result = false;
			}
			if (!flag2)
			{
				entryModel.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "LineAmount");
				result = false;
			}
			if ((flag & flag2) && Math.Abs(entryModel.MTaxAmount) > Math.Abs(entryModel.MAmount))
			{
				entryModel.Validate(ctx, true, "LineAmountMustMoreThanTaxAmount", "“税额“必须小于等于“金额”。", LangModule.FP);
				result = false;
			}
			bool flag3 = entryModel.UpdateFieldList.Contains("MPrice");
			bool flag4 = entryModel.UpdateFieldList.Contains("MQuantity");
			if (flag3 & flag4 & flag2)
			{
				decimal num = Math.Round(entryModel.MPrice * entryModel.MQuantity, 2);
				if (num != Math.Round(entryModel.MAmount, 2))
				{
					entryModel.Validate(ctx, true, "AmountMustEqualPriceMultiQuantity", "{0}不等于计算出的金额{1}。", LangModule.FP, Math.Round(entryModel.MAmount, 2), num);
					result = false;
				}
			}
			GroupCollection itemNameGroup = GetItemNameGroup(string.IsNullOrEmpty(entryModel.MItemName) ? "" : entryModel.MItemName);
			bool flag5 = itemNameGroup.Count > 2;
			entryModel.MItemCategoryCode = (flag5 ? itemNameGroup[1].ToString() : string.Empty);
			entryModel.MItemName = (flag5 ? itemNameGroup[2].ToString() : entryModel.MItemName);
			entryModel.MTotalAmount = entryModel.MAmount + entryModel.MTaxAmount;
			return result;
		}

		private void ProcessEntryCount(MContext ctx, FPFapiaoModel model, FPFapiaoModel dbModel, ref bool entryCountError)
		{
			if (model.MFapiaoEntrys == null || model.MFapiaoEntrys.Count < 1)
			{
				model.Validate(ctx, true, "LeastOneLines", "至少指定一条分录信息。", LangModule.FP);
				entryCountError = true;
			}
		}

		protected override List<CommandInfo> OnPostGetCmd(MContext ctx, PostParam<FPFapiaoModel> param, APIDataPool dataPool, FPFapiaoModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			FPFapiaoModel fPFapiaoModel = param.DataList.LastOrDefault((FPFapiaoModel t) => !string.IsNullOrEmpty(t.MID) && t.MID == model.MID && t.ValidationErrors.Count == 0 && t.UniqueIndex < model.UniqueIndex);
			if (fPFapiaoModel != null && !model.IsNew)
			{
				foreach (FPFapiaoEntryModel mFapiaoEntry in fPFapiaoModel.MFapiaoEntrys)
				{
					if (mFapiaoEntry != null)
					{
						list.AddRange(ModelInfoManager.GetDeleteCmd<FPFapiaoEntryModel>(ctx, mFapiaoEntry.MEntryID));
					}
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FPFapiaoModel>(ctx, model, model.UpdateFieldList, true));
			return list;
		}

		protected override void OnPostAfter(MContext ctx, PostParam<FPFapiaoModel> param, APIDataPool dataPool)
		{
			List<string> list = (from t in param.DataList
			where !string.IsNullOrEmpty(t.MID) && t.ValidationErrors.Count == 0
			select t.MID).ToList();
			if (list.Count > 0)
			{
				GetParam param2 = new GetParam
				{
					MOrgID = ctx.MOrgID,
					MUserID = ctx.MUserID,
					IncludeDetail = new bool?(true)
				};
				base.SetWhereString(param2, "MFapiaoID", list, true);
				DataGridJson<FPFapiaoModel> dataGridJson = base.Get(ctx, param2);
				List<FPFapiaoModel> list2 = new List<FPFapiaoModel>();
				for (int i = 0; i < param.DataList.Count; i++)
				{
					FPFapiaoModel model = param.DataList[i];
					if (model.ValidationErrors.Count > 0)
					{
						list2.Add(model);
					}
					else
					{
						FPFapiaoModel fPFapiaoModel = dataGridJson.rows.FirstOrDefault((FPFapiaoModel a) => a.MID == model.MID);
						if (fPFapiaoModel != null)
						{
							fPFapiaoModel.IsNew = model.IsNew;
							list2.Add(fPFapiaoModel);
						}
					}
				}
				param.DataList = list2;
			}
		}

		protected override void OnDeleteValidate(MContext ctx, DeleteParam param, APIDataPool dataPool, FPFapiaoModel model)
		{
			if (ctx.MOrgVersionID == 0 && model.MReconcileStatus == 1)
			{
				model.Validate(ctx, true, "ReconciledFaPiaoCanNotDelete", "已被勾兑的发票不能被删除。", LangModule.FP);
			}
			if (ctx.MOrgVersionID == 1 && model.MCodingStatus == 1)
			{
				model.Validate(ctx, true, "VoucheredFaPiaoCanNotDelete", "已生成凭证的发票不能被删除。", LangModule.FP);
			}
			if (model.MSource == 1)
			{
				model.Validate(ctx, true, "AutoFaPiaoCanNotDelete", "自动获取的发票不能被删除。", LangModule.FP);
			}
		}

		protected override List<CommandInfo> OnDeleteGetCmd(MContext ctx, DeleteParam param, APIDataPool dataPool, FPFapiaoModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<string> fapiaoIdList = new List<string>
			{
				model.MID
			};
			list.AddRange(new FPFapiaoRepository().GetDeleteFapiaoCmd(ctx, fapiaoIdList));
			return list;
		}

		private static GroupCollection GetItemNameGroup(string itemName)
		{
			Match match = Regex.Match(itemName, "^\\*(?<category>.+?)(?=\\*)\\*(?<name>.*)");
			return match.Groups;
		}

		private static void SetFapiaoType(FPFapiaoModel model, MContext ctx)
		{
			if (model.ValidationErrors.Count == 0)
			{
				string text = model.MPContactName;
				string text2 = model.MSContactName;
				string b = ctx.MLegalTradingName.Replace("（", "(").Replace("）", ")");
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("（", "(").Replace("）", ")");
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2.Replace("（", "(").Replace("）", ")");
				}
				if (text == b)
				{
					model.MInvoiceType = 1;
				}
				else if (text2 == b)
				{
					model.MInvoiceType = 0;
				}
				else
				{
					model.Validate(ctx, true, "FapiaoCategoryError", "发票购销方名称都匹配不到美记组织法定名称，无法创建。", LangModule.FP);
				}
			}
		}

		private static void SetVerifyType(FPFapiaoModel model, MContext ctx)
		{
			if (model.ValidationErrors.Count == 0)
			{
				if (model.MInvoiceType == 0)
				{
					model.MVerifyType = 3;
				}
				else if (model.MStatus == 1 && model.MType == 1)
				{
					model.MVerifyType = 0;
				}
				else
				{
					model.MVerifyType = 3;
				}
			}
		}
	}
}
