using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVAPIInvoiceBaseBusiness : BusinessServiceBase
	{
		protected IVInvoiceRepository _repInvoice = new IVInvoiceRepository();

		private List<BDContactsInfoModel> _contactDataPool;

		private List<REGCurrencyViewModel> _currencyDataPool;

		private List<BDTrackModel> _trackDataPool;

		private List<BDTrackModel> _trackList;

		private List<REGTaxRateModel> _taxRateDataPool;

		private List<REGTaxRateModel> _taxRateList;

		private List<BDItemModel> _itemDataPool;

		protected void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, List<IVInvoiceModel> modelList)
		{
			if (modelList != null && modelList.Any())
			{
				if (!string.IsNullOrWhiteSpace(param.ElementID) || param.FromPost)
				{
					List<BDContactsInfoModel> source = new List<BDContactsInfoModel>();
					List<string> list = (from item in modelList
					where !string.IsNullOrWhiteSpace(item?.MContactInfo?.MContactID)
					select item.MContactInfo?.MContactID).ToList();
					if (list.Count > 0)
					{
						GetParam param2 = new GetParam
						{
							MOrgID = ctx.MOrgID,
							MUserID = ctx.MUserID,
							IncludeDisabled = true
						};
						base.SetWhereString(param2, "ContactID", list, true);
						source = (new BDContactsBusiness().Get(ctx, param2).rows ?? new List<BDContactsInfoModel>());
					}
					foreach (IVInvoiceModel model in modelList)
					{
						if (!string.IsNullOrWhiteSpace(model.MContactInfo?.MContactID))
						{
							model.MContactInfo = source.FirstOrDefault((BDContactsInfoModel a) => a.MContactID == model.MContactInfo.MContactID);
						}
						if (model.MContactInfo != null)
						{
							model.MContactInfo.MBalances = new BDContactsBalanceModel();
						}
						HandleInvoiceEntry(ctx, param, dataPool, new List<IVInvoiceModel>
						{
							model
						});
					}
				}
				else
				{
					HandleInvoiceEntry(ctx, param, dataPool, modelList);
				}
			}
		}

		protected void HandleInvoiceEntry(MContext ctx, GetParam param, APIDataPool dataPool, List<IVInvoiceModel> modelList)
		{
			_trackDataPool = dataPool.TrackingCategories;
			foreach (IVInvoiceModel model in modelList)
			{
				model.IncludeDetail = param.IncludeDetail;
				if (!param.IncludeDetail.HasValue || param.IncludeDetail.Value)
				{
					model.MEntryList = (from t in model.MEntryList
					orderby t.MSeq
					select t).ToList();
					foreach (IVInvoiceEntryModel item in model.InvoiceEntry)
					{
						item.MTracking = GetTrackSelectList(_trackDataPool, item);
						item.MTracking = (from a in item.MTracking
						orderby a.MCreateDate
						select a).ToList();
					}
				}
			}
		}

		protected List<IVInvoiceModel> ConvertData<T>(List<T> srcList) where T : IVInvoiceModel, new()
		{
			List<IVInvoiceModel> list = new List<IVInvoiceModel>();
			foreach (T src in srcList)
			{
				list.Add((IVInvoiceModel)src);
			}
			return list;
		}

		protected List<BDTrackSelectModel> GetTrackSelectList(List<BDTrackModel> trackList, IVInvoiceEntryModel entryModel)
		{
			List<BDTrackSelectModel> list = new List<BDTrackSelectModel>();
			if (!string.IsNullOrEmpty(entryModel.MTrackItem1))
			{
				BDTrackSelectModel trackSelectModel = GetTrackSelectModel(trackList, entryModel.MTrackItem1);
				if (trackSelectModel != null)
				{
					list.Add(trackSelectModel);
				}
				entryModel.MTrackItem1 = null;
			}
			if (!string.IsNullOrEmpty(entryModel.MTrackItem2))
			{
				BDTrackSelectModel trackSelectModel2 = GetTrackSelectModel(trackList, entryModel.MTrackItem2);
				if (trackSelectModel2 != null)
				{
					list.Add(trackSelectModel2);
				}
				entryModel.MTrackItem2 = null;
			}
			if (!string.IsNullOrEmpty(entryModel.MTrackItem3))
			{
				BDTrackSelectModel trackSelectModel3 = GetTrackSelectModel(trackList, entryModel.MTrackItem3);
				if (trackSelectModel3 != null)
				{
					list.Add(trackSelectModel3);
				}
				entryModel.MTrackItem3 = null;
			}
			if (!string.IsNullOrEmpty(entryModel.MTrackItem4))
			{
				BDTrackSelectModel trackSelectModel4 = GetTrackSelectModel(trackList, entryModel.MTrackItem4);
				if (trackSelectModel4 != null)
				{
					list.Add(trackSelectModel4);
				}
				entryModel.MTrackItem4 = null;
			}
			if (!string.IsNullOrEmpty(entryModel.MTrackItem5))
			{
				BDTrackSelectModel trackSelectModel5 = GetTrackSelectModel(trackList, entryModel.MTrackItem5);
				if (trackSelectModel5 != null)
				{
					list.Add(trackSelectModel5);
				}
				entryModel.MTrackItem5 = null;
			}
			entryModel.MTrackItem1 = null;
			entryModel.MTrackItem2 = null;
			entryModel.MTrackItem3 = null;
			entryModel.MTrackItem4 = null;
			entryModel.MTrackItem5 = null;
			return list;
		}

		protected BDTrackSelectModel GetTrackSelectModel(List<BDTrackModel> trackList, string trackOptionId)
		{
			foreach (BDTrackModel track in trackList)
			{
				if (track.MEntryList != null && track.MEntryList.Count != 0)
				{
					BDTrackEntryModel bDTrackEntryModel = track.MEntryList.FirstOrDefault((BDTrackEntryModel t) => t.MEntryID == trackOptionId);
					if (bDTrackEntryModel != null)
					{
						return new BDTrackSelectModel
						{
							TrackingCategoryID = track.MItemID,
							Name = track.MName,
							TrackingOptionName = bDTrackEntryModel.MName,
							TrackingOptionID = bDTrackEntryModel.MEntryID,
							MCreateDate = track.MCreateDate
						};
					}
				}
			}
			return null;
		}

		public void Post<T>(MContext ctx, bool isPut, List<T> list, List<T> dbDataList) where T : IVInvoiceModel, new()
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			APIDataRepository.FillBaseData(ctx, list, true, null);
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_contactDataPool = instance.Contacts;
			_currencyDataPool = instance.Currencies;
			_itemDataPool = instance.Items;
			_trackDataPool = instance.TrackingCategories;
			_taxRateDataPool = instance.TaxRates;
			_trackList = ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true);
			_taxRateList = ModelInfoManager.GetDataModelList<REGTaxRateModel>(ctx, new SqlWhere(), false, true);
			int num = 0;
			List<T> list3 = new List<T>();
			List<T> list4 = new List<T>();
			foreach (T item in list)
			{
				T val = null;
				if (!string.IsNullOrEmpty(item.MID))
				{
					val = dbDataList.FirstOrDefault((T t) => t.MID == item.MID);
				}
				if (!(item.MType == "Invoice_Purchase") && !(item.MType == "Invoice_Purchase_Red") && val == null && !string.IsNullOrEmpty(item.MNumber))
				{
					val = dbDataList.FirstOrDefault((T t) => t.MNumber == item.MNumber);
				}
				item.IsNew = (val == null);
				list4.Add(val);
			}
			PostContact(ctx, list, list4);
			List<T> list5 = new List<T>();
			IVAutoNumberModel maxAutoNumber = IVInvoiceRepository.GetMaxAutoNumber(ctx);
			foreach (T item2 in list)
			{
				T dbModel = list4[num];
				if (ProcessInvoiceModel(ctx, isPut, item2, dbModel))
				{
					list5.Add(item2);
					List<CommandInfo> collection = new List<CommandInfo>();
					bool flag = false;
					ProcessNumber(ctx, item2, dbModel, list, isPut, ref maxAutoNumber, ref collection, ref list5, ref flag);
					if (flag)
					{
						list5.Remove(item2);
					}
					foreach (IVInvoiceEntryModel mEntry in ((IVBaseModel<IVInvoiceEntryModel>)item2).MEntryList)
					{
						if (item2.MType == "Invoice_Sale_Red" || item2.MType == "Invoice_Purchase_Red")
						{
							mEntry.MQty = -mEntry.MQty;
							mEntry.MTaxAmtFor = -Math.Abs(mEntry.MTaxAmtFor);
						}
					}
					if (item2.ValidationErrors.Count <= 0)
					{
						if (item2.IsNew)
						{
							item2.MCreateBy = ctx.MConsumerKey;
							item2.UpdateFieldList.Add("MCreateBy");
						}
						item2.UpdateFieldList.Add("MModifyDate");
						list3.Add(item2);
						list2.AddRange((IEnumerable<CommandInfo>)collection);
						list2.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetInsertOrUpdateCmd<T>(ctx, item2, item2.UpdateFieldList, true));
						list2.AddRange((IEnumerable<CommandInfo>)IVInvoiceLogHelper.GetSaveLogCmd(ctx, item2));
						goto IL_03bf;
					}
					continue;
				}
				goto IL_03bf;
				IL_03bf:
				num++;
			}
			List<CommandInfo> list6 = new List<CommandInfo>();
			if (list5.Count > 0)
			{
				List<T> bills = (from a in list5
				where (a.MType == "Invoice_Sale_Red" || a.MType == "Invoice_Sale") && !a.ForceToGenerate
				select a).ToList();
				List<T> bills2 = (from a in list5
				where (a.MType == "Invoice_Purchase_Red" || a.MType == "Invoice_Purchase") && !a.ForceToGenerate
				select a).ToList();
				List<T> bills3 = (from a in list5
				where (a.MType == "Invoice_Sale_Red" || a.MType == "Invoice_Sale") && a.ForceToGenerate
				select a).ToList();
				List<T> bills4 = (from a in list5
				where a.MType == "Invoice_Purchase_Red" || (a.MType == "Invoice_Purchase" && a.ForceToGenerate)
				select a).ToList();
				OperationResult operationResult = new GLDocVoucherRepository().GenerateVouchersByBills(ctx, bills, list4, false);
				OperationResult operationResult2 = new GLDocVoucherRepository().GenerateVouchersByBills(ctx, bills2, list4, false);
				OperationResult operationResult3 = new GLDocVoucherRepository().GenerateVouchersByBills(ctx, bills3, list4, true);
				OperationResult operationResult4 = new GLDocVoucherRepository().GenerateVouchersByBills(ctx, bills4, list4, true);
				List<CommandInfo> operationCommands = operationResult.OperationCommands;
				List<CommandInfo> operationCommands2 = operationResult2.OperationCommands;
				List<CommandInfo> operationCommands3 = operationResult3.OperationCommands;
				List<CommandInfo> operationCommands4 = operationResult4.OperationCommands;
				list6.AddRange((IEnumerable<CommandInfo>)operationCommands);
				list6.AddRange((IEnumerable<CommandInfo>)operationCommands2);
				list6.AddRange((IEnumerable<CommandInfo>)operationCommands3);
				list6.AddRange((IEnumerable<CommandInfo>)operationCommands4);
			}
			bool flag2 = false;
			if (list2.Count > 0)
			{
				List<MultiDBCommand> list7 = new List<MultiDBCommand>();
				MultiDBCommand multiDBCommand = new MultiDBCommand(ctx);
				multiDBCommand.DBType = SysOrBas.Bas;
				multiDBCommand.CommandList = list2;
				list7.Add(multiDBCommand);
				if (list6.Count > 0)
				{
					MultiDBCommand multiDBCommand2 = new MultiDBCommand(ctx);
					multiDBCommand2.DBType = SysOrBas.Bas;
					multiDBCommand2.CommandList = list6;
					list7.Add(multiDBCommand2);
				}
				flag2 = DbHelperMySQL.ExecuteSqlTran(ctx, list7.ToArray());
			}
			RestoreData(list);
		}

		public void PostContact<T>(MContext ctx, List<T> list, List<T> dbModelList) where T : IVInvoiceModel, new()
		{
			List<BDContactsInfoModel> list2 = new List<BDContactsInfoModel>();
			for (int i = 0; i < list.Count; i++)
			{
				bool flag = list[i].UpdateFieldList.Contains("MContactInfo");
				if ((dbModelList[i] != null && !flag) || !TypeIsOk(list[i]))
				{
					list2.Add(new BDContactsInfoModel
					{
						MSourceBizObject = "Invoice",
						IsIgnore = true,
						UpdateFieldList = new List<string>()
					});
				}
				else
				{
					BDContactsInfoModel bDContactsInfoModel = list[i].MContactInfo ?? new BDContactsInfoModel
					{
						UpdateFieldList = list[i].UpdateFieldList
					};
					bDContactsInfoModel.MSourceBizObject = "Invoice";
					bDContactsInfoModel.ValidationErrors.AddRange((IEnumerable<ValidationError>)(from f in list[i].ValidationErrors
					where f.Type == 2
					select f).ToList());
					list[i].ValidationErrors.RemoveAll((Predicate<ValidationError>)((ValidationError a) => a.Type == 2));
					list2.Add(bDContactsInfoModel);
				}
			}
			IBasicBusiness<BDContactsInfoModel> basicBusiness = new BDContactsBusiness();
			List<BDContactsInfoModel> list3 = basicBusiness.Post(ctx, new PostParam<BDContactsInfoModel>
			{
				DataList = list2
			});
			if (list3.Any((BDContactsInfoModel f) => !Enumerable.Any<ValidationError>((IEnumerable<ValidationError>)f.ValidationErrors)))
			{
				_contactDataPool = APIDataPool.GetInstance(ctx).Contacts;
			}
			int num = 0;
			foreach (T item in list)
			{
				item.MContactInfo = list3[num];
				if (item.MContactInfo.ValidationErrors.Any())
				{
					foreach (ValidationError validationError in item.MContactInfo.ValidationErrors)
					{
						item.ValidationErrors.Add(validationError);
					}
					item.MContactInfo.ValidationErrors.Clear();
				}
				num++;
			}
		}

		private void RestoreData<T>(List<T> list) where T : IVInvoiceModel, new()
		{
			foreach (T item in list)
			{
				if (((IVBaseModel<IVInvoiceEntryModel>)item).MEntryList != null && ((IVBaseModel<IVInvoiceEntryModel>)item).MEntryList.Count != 0 && (item.MType == "Invoice_Sale_Red" || item.MType == "Invoice_Purchase_Red"))
				{
					foreach (IVInvoiceEntryModel mEntry in ((IVBaseModel<IVInvoiceEntryModel>)item).MEntryList)
					{
						if (mEntry.MQty < decimal.Zero)
						{
							mEntry.MQty = -mEntry.MQty;
							mEntry.MTaxAmtFor = Math.Abs(mEntry.MTaxAmtFor);
						}
					}
				}
			}
		}

		private bool ProcessInvoiceModel(MContext ctx, bool isPut, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			bool flag = true;
			bool flag2 = true;
			if (TypeIsOk(model))
			{
				flag = ProcessContact(ctx, model, dbModel);
				flag2 = ProcessIdAndNumber(ctx, isPut, model, dbModel);
			}
			bool flag3 = ProcessDate(ctx, model, dbModel);
			bool flag4 = ProcessCurrency(ctx, model, dbModel);
			ProcessLineAmountType(ctx, model);
			bool flag5 = ProcessUrl(ctx, model);
			bool flag6 = ProcessStatus(ctx, model, dbModel);
			if (!ProcessEntryCount(ctx, model, dbModel))
			{
				return false;
			}
			if (!ProcessEntryList(ctx, model, dbModel))
			{
				return false;
			}
			bool flag7 = ProcessTaxTotalAmtFor(ctx, model);
			return (model.ValidationErrors == null || model.ValidationErrors.Count == 0) & flag & flag2 & flag3 & flag4 & flag5 & flag6 & flag7;
		}

		private bool ProcessUrl(MContext ctx, IVInvoiceModel model)
		{
			if (!string.IsNullOrEmpty(model.MUrl) && !RegExp.IsUrl(model.MUrl))
			{
				model.Validate(ctx, true, "UrlFormatIsError", "来源必须为有效的url地址格式。", LangModule.IV);
				return false;
			}
			return true;
		}

		private bool ProcessTaxTotalAmtFor(MContext ctx, IVInvoiceModel model)
		{
			if (model.MTaxTotalAmtFor <= decimal.Zero)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TotalAmtMustGreaterThanZero", "The Total for this document must be greater than zero.")));
				return false;
			}
			if (DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MTaxTotalAmtFor)) || DecimalUtility.IsDecimalValueTooLong(Math.Abs(model.MTaxTotalAmt)))
			{
				string msg = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DecimalCanNotExceedMaxInteger", "“{0}”的整数位不能超过12位。"), "Total");
				if (model.ValidationErrors.All((ValidationError a) => a.Message != msg))
				{
					model.ValidationErrors.Add(new ValidationError(msg));
				}
				return false;
			}
			return true;
		}

		private bool ProcessIdAndNumber(MContext ctx, bool isPut, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			if (model.IsNew && !model.UpdateFieldList.Contains("MType"))
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Type");
				return false;
			}
			if (isPut && model.MType != "Invoice_Purchase" && model.MType != "Invoice_Purchase_Red" && dbModel != null)
			{
				model.Validate(ctx, true, "mustBeUnique", "'{字段名}' must be unique.", LangModule.Common, (model.MType == "Invoice_Sale") ? "InvoiceNumber" : "CreditNoteNumber");
				return false;
			}
			if (dbModel == null && !string.IsNullOrWhiteSpace(model.MID))
			{
				if (model.MType == "Invoice_Sale")
				{
					model.Validate(ctx, true, "InvoiceIDNumberNotExist", "“InvoiceID”对应的账单不存在。", LangModule.IV);
					return false;
				}
				if (model.MType == "Invoice_Sale_Red")
				{
					model.Validate(ctx, true, "CreditNoteIDNumberNotExist", "““CreditNoteID”或“CreditNoteNumber”对应的红字账单不存在。", LangModule.IV);
					return false;
				}
				if (model.MType == "Invoice_Purchase")
				{
					model.Validate(ctx, true, "InvoiceIDNotExist", "“InvoiceID”对应的账单不存在。", LangModule.IV);
					return false;
				}
				if (model.MType == "Invoice_Purchase_Red")
				{
					model.Validate(ctx, true, "CreditNoteIDNotExist", "“CreditNoteID”对应的红字账单不存在。", LangModule.IV);
					return false;
				}
			}
			if (dbModel != null)
			{
				model.MID = dbModel.MID;
				model.IsUpdate = true;
				if (!model.IsUpdateFieldExists("MReference"))
				{
					model.MReference = dbModel.MReference;
				}
				if (!model.IsUpdateFieldExists("MType"))
				{
					model.MType = dbModel.MType;
				}
				if (model.UpdateFieldList.Contains("MType") && dbModel.MType != model.MType)
				{
					if (dbModel.MType == "Invoice_Sale" || dbModel.MType == "Invoice_Purchase")
					{
						model.Validate(ctx, true, "InvoiceTypeCannotModify", "不能更改已存在账单的单据类型。", LangModule.IV);
						return false;
					}
					if (dbModel.MType == "Invoice_Sale_Red" || dbModel.MType == "Invoice_Purchase_Red")
					{
						model.Validate(ctx, true, "CreditNoteTypeCannotModify", "不能更改已存在红字账单的单据类型。", LangModule.IV);
						return false;
					}
				}
			}
			if (string.IsNullOrEmpty(model.MNumber) && model.UpdateFieldList.Contains("MNumber"))
			{
				if (model.MType == "Invoice_Sale")
				{
					model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "InvoiceNumber");
					return false;
				}
				if (model.MType == "Invoice_Sale_Red")
				{
					model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "CreditNoteNumber");
					return false;
				}
			}
			return true;
		}

		private string GetNumber<T>(IVInvoiceModel model, List<T> postList, ref IVAutoNumberModel autoNum) where T : IVInvoiceModel
		{
			if (!string.IsNullOrEmpty(model.MNumber))
			{
				return model.MNumber;
			}
			string arg = "";
			int num = 0;
			switch (model.MType)
			{
			case "Invoice_Sale":
				model.UpdateFieldList.Add("MNumber");
				autoNum.InvoiceSale++;
				num = autoNum.InvoiceSale;
				arg = "INV-";
				break;
			case "Invoice_Purchase":
				model.UpdateFieldList.Add("MNumber");
				autoNum.InvoicePurchase++;
				num = autoNum.InvoicePurchase;
				arg = "BINV-";
				break;
			case "Invoice_Sale_Red":
				model.UpdateFieldList.Add("MNumber");
				autoNum.InvoiceSaleRed++;
				num = autoNum.InvoiceSaleRed;
				arg = "CN-";
				break;
			case "Invoice_Purchase_Red":
				model.UpdateFieldList.Add("MNumber");
				autoNum.InvoicePurchaseRed++;
				num = autoNum.InvoicePurchaseRed;
				arg = "BCN-";
				break;
			}
			string number = $"{arg}{num.ToString().PadLeft(4, '0')}";
			if (postList.Any((T t) => !string.IsNullOrEmpty(t.MNumber) && t.MNumber.ToUpper() == number))
			{
				return GetNumber(model, postList, ref autoNum);
			}
			return number;
		}

		private void ProcessNumber<T>(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel, List<T> postList, bool isPut, ref IVAutoNumberModel autoNum, ref List<CommandInfo> extCmdList, ref List<T> genernateVoucherList, ref bool isHaveRepeat) where T : IVInvoiceModel
		{
			bool flag = model.IsUpdateFieldExists("MNumber");
			string number = GetNumber(model, postList, ref autoNum);
			if (!model.IsNew)
			{
				if (!flag)
				{
					model.MNumber = dbModel.MNumber;
					return;
				}
				if (!string.IsNullOrWhiteSpace(model.MNumber) && dbModel != null && dbModel.MNumber != number && model.MType != "Invoice_Purchase" && model.MType != "Invoice_Purchase_Red")
				{
					model.Validate(ctx, true, "mustBeUnique", "'{0}' must be unique.", LangModule.Common, (model.MType == "Invoice_Sale") ? "InvoiceNumber" : "CreditNoteNumber");
				}
			}
			T val = postList.FirstOrDefault((T t) => t.MNumber == number && t.ValidationErrors.Count == 0 && t.UniqueIndex < model.UniqueIndex);
			if (val != null)
			{
				if (isPut)
				{
					if (model.MType != "Invoice_Purchase" && model.MType != "Invoice_Purchase_Red")
					{
						isHaveRepeat = true;
						model.Validate(ctx, val.MNumber == number, "mustBeUnique", "'{0}' must be unique.", LangModule.Common, (model.MType == "Invoice_Sale") ? "InvoiceNumber" : "CreditNoteNumber");
					}
				}
				else
				{
					if (val.IsNew)
					{
						model.ForceToGenerate = true;
					}
					genernateVoucherList.Remove(val);
					model.MID = val.MID;
					model.IsNew = false;
					model.IsUpdate = true;
					foreach (IVInvoiceEntryModel mEntry in ((IVBaseModel<IVInvoiceEntryModel>)val).MEntryList)
					{
						extCmdList.AddRange((IEnumerable<CommandInfo>)ModelInfoManager.GetDeleteCmd<IVInvoiceEntryModel>(ctx, mEntry.MEntryID));
					}
				}
			}
			model.MNumber = number;
		}

		private void ProcessLineAmountType(MContext ctx, IVInvoiceModel model)
		{
			if (model.IsNew && !model.IsUpdateFieldExists("MTaxID"))
			{
				model.MTaxID = "Tax_Exclusive";
				model.UpdateFieldList.Add("MTaxID");
			}
		}

		private bool ProcessEntryCount(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			if (model.MEntryList == null || model.MEntryList.Count == 0)
			{
				if (model.IsUpdate && !model.IsUpdateFieldExists("InvoiceEntry"))
				{
					model.MEntryList = dbModel.MEntryList;
					return true;
				}
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "AtLeastOneLine", "One or more line items must be specified.")));
				return false;
			}
			return true;
		}

		private bool ProcessEntryList(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			if (!model.IsNew && !model.IsUpdateFieldExists("InvoiceEntry"))
			{
				return true;
			}
			if (!model.IsNew && model.IsUpdateFieldExists("InvoiceEntry"))
			{
				List<string> list = (from a in model.InvoiceEntry
				where !string.IsNullOrEmpty(a.MLineItemID)
				select a.MLineItemID).ToList();
				if (list.Count != list.Distinct().ToList().Count)
				{
					model.Validate(ctx, true, "CanNotSupportMultSameEntryID", "不允许多行分录有相同的“{0}”。", LangModule.Common, "LineItemID");
					return false;
				}
			}
			if (!model.IsUpdateFieldExists("MTotalAmtFor"))
			{
				model.UpdateFieldList.Add("MTotalAmtFor");
			}
			if (!model.IsUpdateFieldExists("MTotalAmt"))
			{
				model.UpdateFieldList.Add("MTotalAmt");
			}
			if (!model.IsUpdateFieldExists("MTaxTotalAmtFor"))
			{
				model.UpdateFieldList.Add("MTaxTotalAmtFor");
			}
			if (!model.IsUpdateFieldExists("MTaxTotalAmt"))
			{
				model.UpdateFieldList.Add("MTaxTotalAmt");
			}
			if (!model.IsUpdateFieldExists("MTaxAmtFor"))
			{
				model.UpdateFieldList.Add("MTaxAmtFor");
			}
			if (!model.IsUpdateFieldExists("MTaxAmt"))
			{
				model.UpdateFieldList.Add("MTaxAmt");
			}
			int num = 1;
			foreach (IVInvoiceEntryModel mEntry in model.MEntryList)
			{
				mEntry.MSeq = num;
				if (!ProcessEntryModel(ctx, model, mEntry) && mEntry.ValidationErrors.Count > 0)
				{
					model.ValidationErrors = model.ValidationErrors.Concat(mEntry.ValidationErrors).ToList();
					mEntry.ValidationErrors.Clear();
				}
				bool flag = false;
				if (dbModel != null)
				{
					flag = dbModel.MEntryList.Exists((IVInvoiceEntryModel a) => a.MEntryID == mEntry.MEntryID);
				}
				if (flag)
				{
					mEntry.IsUpdate = true;
				}
				else
				{
					mEntry.MEntryID = UUIDHelper.GetGuid();
					mEntry.IsNew = true;
				}
				num++;
			}
			return true;
		}

		private bool ProcessEntryModel(MContext ctx, IVInvoiceModel model, IVInvoiceEntryModel entryModel)
		{
			entryModel.MExchangeRate = model.MExchangeRate;
			bool flag = SetEntryDefaultValue(ctx, model.MType, entryModel);
			bool flag2 = ProcessEntryQty(ctx, model.MType, entryModel);
			bool flag3 = ProcessEntryPrice(ctx, entryModel);
			bool flag4 = ProcessEntryDiscount(ctx, model.MContactInfo, entryModel);
			bool flag5 = ProcessEntryTax(ctx, model.MType, model.MTaxID, model.MContactInfo, entryModel);
			bool flag6 = ProcessEntryLineAmount(ctx, entryModel);
			bool flag7 = ProcessEntryTracking(ctx, entryModel);
			return flag & flag2 & flag3 & flag4 & flag5 & flag6 & flag7;
		}

		private bool SetEntryDefaultValue(MContext ctx, string invoiceType, IVInvoiceEntryModel model)
		{
			bool flag = model.IsUpdateFieldExists("MItemCode");
			bool flag2 = model.IsUpdateFieldExists("MDesc");
			if (_itemDataPool == null)
			{
				_itemDataPool = new List<BDItemModel>();
			}
			if (!flag || string.IsNullOrEmpty(model.MItemCode))
			{
				if (flag2)
				{
					model.Validate(ctx, string.IsNullOrEmpty(model.MDesc), "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Description");
					return !string.IsNullOrEmpty(model.MDesc);
				}
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Description");
				return false;
			}
			if (!string.IsNullOrEmpty(model.MItemCode))
			{
				BDItemModel bDItemModel = _itemDataPool.FirstOrDefault((BDItemModel t) => t.MNumber == model.MItemCode);
				if (bDItemModel == null)
				{
					model.Validate(ctx, true, "EnumDataInvoid", "'{0}' is not a valid value for '{1}'.", LangModule.Common, model.MItemCode, "ItemCode");
					if (!flag2)
					{
						model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Description");
					}
					return false;
				}
				if (!bDItemModel.MIsActive)
				{
					model.Validate(ctx, true, "ItemIsDisabled", "The status of item with code '{0}' is disabled.", LangModule.IV, model.MItemCode);
					if (!flag2)
					{
						model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Description");
					}
					return false;
				}
				bool flag3;
				if (!flag2)
				{
					model.MDesc = bDItemModel.MDesc;
					model.UpdateFieldList.Add("MDesc");
					flag3 = string.IsNullOrWhiteSpace(bDItemModel.MDesc);
					model.Validate(ctx, flag3, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Description");
				}
				else
				{
					flag3 = string.IsNullOrWhiteSpace(model.MDesc);
					model.Validate(ctx, flag3, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Description");
				}
				if (flag3)
				{
					return false;
				}
				if (!model.IsUpdateFieldExists("MPrice"))
				{
					model.MPrice = decimal.Zero;
					model.UpdateFieldList.Add("MPrice");
				}
				model.MItemID = bDItemModel.MItemID;
				model.UpdateFieldList.Add("MItemID");
			}
			return true;
		}

		private bool ProcessEntryQty(MContext ctx, string invoiceType, IVInvoiceEntryModel model)
		{
			if (!model.IsUpdateFieldExists("MQty"))
			{
				model.Validate(ctx, true, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "Quantity");
				return false;
			}
			if (model.MQty <= decimal.Zero)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "QtyMustGreaterThanZero", "数量必须大于0!")));
				return false;
			}
			model.MQty = Math.Round(model.MQty, 4);
			return true;
		}

		private bool ProcessEntryPrice(MContext ctx, IVInvoiceEntryModel model)
		{
			if (model.MPrice < decimal.Zero)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PriceMustGreaterThanZero", "价格必须大于0!")));
				return false;
			}
			model.MPrice = Math.Round(model.MPrice, 8);
			return true;
		}

		private bool ProcessEntryDiscount(MContext ctx, BDContactsInfoModel contactModel, IVInvoiceEntryModel model)
		{
			if (!model.IsUpdateFieldExists("MDiscount"))
			{
				model.MDiscount = decimal.Zero;
			}
			if (model.MDiscount < decimal.Zero || model.MDiscount > 100m)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DiscountError", "Discount Rate must be less than or equal to one hundred and greater than or equal to zero.")));
				return false;
			}
			model.MDiscount = Math.Round(model.MDiscount, 6);
			return true;
		}

		private bool ProcessEntryTax(MContext ctx, string invoiceType, string taxType, BDContactsInfoModel contactModel, IVInvoiceEntryModel model)
		{
			model.MTaxTypeID = taxType;
			if (taxType == "No_Tax")
			{
				model.MTaxID = "";
				model.MTaxAmtFor = decimal.Zero;
				return true;
			}
			if (model.MTaxRate == null)
			{
				if (model.UpdateFieldList.Contains("MTaxRate"))
				{
					model.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
					return false;
				}
				model.MTaxRate = new REGTaxRateModel();
			}
			if (model.MTaxRate.UpdateFieldList != null && !model.MTaxRate.UpdateFieldList.Contains("MTaxRateID") && !model.MTaxRate.UpdateFieldList.Contains("MName"))
			{
				model.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
				return false;
			}
			if (string.IsNullOrEmpty(model.MTaxRate.MTaxRateID))
			{
				if (model.MTaxRate.UpdateFieldList != null && (model.MTaxRate.UpdateFieldList.Contains("MTaxRateID") || model.MTaxRate.UpdateFieldList.Contains("MName")))
				{
					model.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
					return false;
				}
				BDItemModel bDItemModel = _itemDataPool.FirstOrDefault((BDItemModel t) => t.MItemID == model.MItemID && t.MIsActive);
				model.MTaxRate.MTaxRateID = ((bDItemModel == null) ? "" : ((invoiceType == "Invoice_Sale" || invoiceType == "Invoice_Sale_Red") ? bDItemModel.MSalTaxTypeID : bDItemModel.MPurTaxTypeID));
				if (string.IsNullOrEmpty(model.MTaxRate.MTaxRateID))
				{
					model.MTaxRate.MTaxRateID = ((invoiceType == "Invoice_Sale" || invoiceType == "Invoice_Sale_Red") ? contactModel.MSalTaxTypeID : contactModel.MPurTaxTypeID);
				}
			}
			string mName = model.MTaxRate.MName;
			if (!string.IsNullOrEmpty(mName) && base.IsMatchMultRecord(ctx, _taxRateList, model.MTaxRate.MultiLanguage, "MName", "MName"))
			{
				base.AddValidationError(ctx, model, "Name", mName);
				return false;
			}
			if (string.IsNullOrEmpty(model.MTaxRate?.MTaxRateID) || _taxRateDataPool == null || _taxRateDataPool.Count == 0)
			{
				model.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
				return false;
			}
			REGTaxRateModel rEGTaxRateModel = _taxRateDataPool.FirstOrDefault((REGTaxRateModel t) => t.MTaxRateID == model.MTaxRate.MTaxRateID);
			if (rEGTaxRateModel == null)
			{
				model.Validate(ctx, true, "TaxRateNameNotExist", "无效的税率。请提供有效的TaxRateID或税率Name。", LangModule.Common);
				return false;
			}
			if (!rEGTaxRateModel.MIsActive)
			{
				model.Validate(ctx, true, "TaxRateDisabled", "提供的税率已禁用。", LangModule.Common);
				return false;
			}
			model.MTaxID = model.MTaxRate.MTaxRateID;
			if (!model.IsUpdateFieldExists("MTaxAmtFor"))
			{
				if (taxType == "Tax_Exclusive")
				{
					model.MTaxAmtFor = Math.Abs(model.MAmountFor) * (rEGTaxRateModel.MEffectiveTaxRate * 0.01m);
				}
				else if (taxType == "Tax_Inclusive")
				{
					model.MTaxAmtFor = Math.Abs(model.MAmountFor) - Math.Abs(model.MAmountFor) / (decimal.One + rEGTaxRateModel.MEffectiveTaxRate * 0.01m);
				}
				else if (taxType == "No_Tax")
				{
					model.MTaxAmtFor = decimal.Zero;
				}
			}
			else if (model.MTaxAmtFor < decimal.Zero || model.MTaxAmtFor > model.MAmountFor)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxAmountCannotLessThanZero", "“税额“必须大于等于0小于等于“金额”")));
				return false;
			}
			model.MTaxAmtFor = Math.Round(model.MTaxAmtFor, 2);
			return true;
		}

		private bool ProcessEntryLineAmount(MContext ctx, IVInvoiceEntryModel model)
		{
			if (!model.IsUpdateFieldExists("MAmountFor"))
			{
				return true;
			}
			if (model.MAmountFor != model.MLineAmountFor)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.IV, "LineAmountError", "LineAmount '{0}' does not match the expected LineAmount '{1}'.", model.MLineAmountFor.To2Decimal(), model.MAmountFor.To2Decimal())));
				return false;
			}
			return true;
		}

		private bool ProcessEntryTracking(MContext ctx, IVInvoiceEntryModel model)
		{
			if (_trackList == null || _trackList.Count == 0 || model.MTracking == null || model.MTracking.Count == 0)
			{
				return true;
			}
			bool result = true;
			int num = 1;
			string empty = string.Empty;
			foreach (BDTrackModel item in from a in _trackList
			orderby a.MCreateDate
			select a)
			{
				if (item.MEntryList != null && item.MEntryList.Count != 0)
				{
					bool flag = false;
					foreach (BDTrackSelectModel item2 in model.MTracking)
					{
						if (!item2.IsHaveError && (!string.IsNullOrEmpty(item2.TrackingCategoryID) || !string.IsNullOrEmpty(item2.Name)) && !string.IsNullOrEmpty(item2.TrackingOptionName))
						{
							if (item.MItemID == item2.TrackingCategoryID)
							{
								string value = SetTrackingItem(ctx, model, num, item.MEntryList, item2, ref result, ref flag, ref empty);
								if (!string.IsNullOrEmpty(value))
								{
									break;
								}
							}
							if (base.IsMatchMultRecord(ctx, _trackList, item2.MultiLanguage, "MName", "Name"))
							{
								base.AddValidationError(ctx, model, "TrackingCategoryName", item2.Name);
								item2.IsHaveError = true;
								result = false;
							}
							else
							{
								List<BDTrackModel> dbDataList = new List<BDTrackModel>
								{
									item
								};
								BDTrackModel bDTrackModel = base.MultLanguageMatchRecord(ctx, dbDataList, item2.MultiLanguage, "MName", "Name");
								if (!string.IsNullOrEmpty(bDTrackModel?.MItemID))
								{
									string value2 = SetTrackingItem(ctx, model, num, bDTrackModel.MEntryList, item2, ref result, ref flag, ref empty);
									if (string.IsNullOrEmpty(value2))
									{
										if (flag)
										{
											model.Validate(ctx, true, "TrackOptionDisabled", "提供的跟踪项选项“{0}”已禁用。", LangModule.Common, empty);
											result = false;
										}
										continue;
									}
									break;
								}
							}
						}
					}
					num++;
				}
			}
			return result;
		}

		private string SetTrackingItem(MContext ctx, IVInvoiceEntryModel model, int trackingIndex, List<BDTrackEntryModel> entryList, BDTrackSelectModel selectItem, ref bool isOk, ref bool isDisabled, ref string entryName)
		{
			if (base.IsMatchMultRecord(ctx, entryList, selectItem.MultiLanguage, "MName", "TrackingOptionName"))
			{
				base.AddValidationError(ctx, model, "TrackingOptionName", selectItem.TrackingOptionName);
				isOk = false;
				return "";
			}
			BDTrackEntryModel bDTrackEntryModel = base.MultLanguageMatchRecord(ctx, entryList, selectItem.MultiLanguage, "MName", "TrackingOptionName");
			if (!string.IsNullOrEmpty(bDTrackEntryModel?.MItemID))
			{
				entryName = bDTrackEntryModel.MName;
				if (!bDTrackEntryModel.MIsActive)
				{
					isDisabled = true;
					return "";
				}
				isDisabled = false;
				switch (trackingIndex)
				{
				case 1:
					model.MTrackItem1 = bDTrackEntryModel.MEntryID;
					model.UpdateFieldList.Add("MTrackItem1");
					return bDTrackEntryModel.MEntryID;
				case 2:
					model.MTrackItem2 = bDTrackEntryModel.MEntryID;
					model.UpdateFieldList.Add("MTrackItem2");
					return bDTrackEntryModel.MEntryID;
				case 3:
					model.MTrackItem3 = bDTrackEntryModel.MEntryID;
					model.UpdateFieldList.Add("MTrackItem3");
					return bDTrackEntryModel.MEntryID;
				case 4:
					model.MTrackItem4 = bDTrackEntryModel.MEntryID;
					model.UpdateFieldList.Add("MTrackItem4");
					return bDTrackEntryModel.MEntryID;
				case 5:
					model.MTrackItem5 = bDTrackEntryModel.MEntryID;
					model.UpdateFieldList.Add("MTrackItem5");
					return bDTrackEntryModel.MEntryID;
				}
			}
			return "";
		}

		private bool ProcessContact(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			if (dbModel != null && !model.UpdateFieldList.Contains("MContactInfo"))
			{
				model.MContactID = dbModel.MContactID;
				return true;
			}
			if (!string.IsNullOrEmpty(model.MContactInfo?.MContactID) && _contactDataPool != null && !model.MContactInfo.IsNew)
			{
				model.MContactInfo = _contactDataPool.FirstOrDefault((BDContactsInfoModel t) => t.MContactID == model.MContactInfo.MContactID);
			}
			model.UpdateFieldList.Add("MContactID");
			BDContactsInfoModel mContactInfo = model.MContactInfo;
			if (mContactInfo != null && !mContactInfo.IsHaveNameOrIdError)
			{
				model.MContactID = mContactInfo.MContactID;
				switch (model.MType)
				{
				case "Invoice_Sale":
				case "Invoice_Sale_Red":
					if (!mContactInfo.MIsCustomer)
					{
						string message2 = (model.MType == "Invoice_Sale") ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "SaleContactNotCustomer", "销售单的联系人不是“客户”类别。") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "RedSaleContactNotCustomer", "红字销售单的联系人不是“客户”类别。");
						model.ValidationErrors.Add(new ValidationError(message2));
						return false;
					}
					goto default;
				case "Invoice_Purchase":
				case "Invoice_Purchase_Red":
					if (!mContactInfo.MIsSupplier)
					{
						string message = (model.MType == "Invoice_Purchase") ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BillContactNotSupplier", "采购单的联系人不是“供应商”类别。") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "RedBillContactNotSupplier", "红字采购单的联系人不是“供应商”类别。");
						model.ValidationErrors.Add(new ValidationError(message));
						return false;
					}
					goto default;
				default:
					return !mContactInfo.ValidationErrors.Any();
				}
			}
			return true;
		}

		private bool ProcessStatus(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			bool flag = model.IsUpdateFieldExists("MStatus");
			if (flag && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.Draft) && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval) && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.Paid))
			{
				return false;
			}
			if (flag && model.IsNew && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.Draft) && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval))
			{
				if (model.MType == "Invoice_Purchase" || model.MType == "Invoice_Sale")
				{
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceIsNotDraft", "目前只支持创建“DRAFT”及“SUBMITTED”状态的账单。")));
					return false;
				}
				if (model.MType == "Invoice_Purchase_Red" || model.MType == "Invoice_Sale_Red")
				{
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CreditNoteIsNotDraft", "目前只支持创建“DRAFT”及“SUBMITTED”状态的红字账单。")));
					return false;
				}
			}
			if (model.IsNew && !flag)
			{
				model.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
				model.UpdateFieldList.Add("MStatus");
			}
			else if (dbModel != null && !flag)
			{
				model.MStatus = dbModel.MStatus;
			}
			if (dbModel != null)
			{
				if ((model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.Draft) && model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval)) & flag)
				{
					if (model.MType == "Invoice_Purchase" || model.MType == "Invoice_Sale")
					{
						model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceIsNotSupportPaidOrApproved", "目前不支持将账单的状态更新为“APPROVED”或“PAID”。")));
						return false;
					}
					if (model.MType == "Invoice_Purchase_Red" || model.MType == "Invoice_Sale_Red")
					{
						model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CreditNoteIsNotSupportPaidOrApproved", "目前不支持将红字账单的状态更新为“APPROVED”或“PAID”。")));
						return false;
					}
				}
				if (dbModel.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || dbModel.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.Paid))
				{
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, (dbModel.MType == "Invoice_Sale" || dbModel.MType == "Invoice_Purchase") ? "AprInvCanNotBeUpdated" : "AprInvRedCanNotBeUpdated", "Approved invoice can not be updated.")));
					return false;
				}
			}
			return true;
		}

		private void ProcessDueDateAndExpectedDate(IVInvoiceModel model)
		{
			bool flag = model.MType == "Invoice_Purchase_Red" || model.MType == "Invoice_Sale_Red";
			if (flag)
			{
				model.MDueDate = DateTime.MinValue;
			}
			if (model.MType == "Invoice_Purchase" | flag)
			{
				model.MExpectedDate = DateTime.MinValue;
			}
		}

		private bool ProcessDate(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			bool flag = model.IsUpdateFieldExists("MBizDate");
			bool flag2 = model.IsUpdateFieldExists("MDueDate");
			bool flag3 = model.IsUpdateFieldExists("MExpectedDate");
			ProcessDueDateAndExpectedDate(model);
			DateTime dateTime;
			if (model.IsNew)
			{
				if (!flag)
				{
					dateTime = ctx.DateNow;
					model.MBizDate = dateTime.Date;
					model.UpdateFieldList.Add("MBizDate");
				}
			}
			else
			{
				if (!flag)
				{
					model.MBizDate = dbModel.MBizDate;
				}
				if (!flag2)
				{
					model.MDueDate = dbModel.MDueDate;
				}
				if (!flag3)
				{
					model.MExpectedDate = dbModel.MExpectedDate;
				}
			}
			dateTime = model.MBizDate;
			model.MBizDate = dateTime.Date;
			dateTime = model.MDueDate;
			model.MDueDate = dateTime.Date;
			DateTime mExpectedDate;
			if (!(model.MExpectedDate < new DateTime(1900, 1, 1)))
			{
				dateTime = model.MExpectedDate;
				mExpectedDate = dateTime.Date;
			}
			else
			{
				mExpectedDate = DateTime.MinValue;
			}
			model.MExpectedDate = mExpectedDate;
			if (model.MBizDate == new DateTime(1, 1, 1))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "Date");
			}
			else if (model.MBizDate < ctx.MBeginDate)
			{
				object[] obj = new object[2];
				dateTime = model.MBizDate;
				obj[0] = dateTime.ToString("yyyy-MM-dd");
				dateTime = ctx.MBeginDate;
				obj[1] = dateTime.ToString("yyyy-MM");
				model.Validate(ctx, true, "DateGreaterThanConversionDate", "The period of Date '{0}' must be greater than organization conversion period '{1}.", LangModule.Common, obj);
			}
			else if (!GLInterfaceRepository.IsPeriodUnclosed(ctx, model.MBizDate).Success)
			{
				object[] obj2 = new object[1];
				dateTime = model.MBizDate;
				obj2[0] = dateTime.ToString("yyyy-MM-dd");
				model.Validate(ctx, true, "DateMustGLOpen", "The accounting period of Date'{0}' has been closed.", LangModule.Common, obj2);
			}
			if (model.MType == "Invoice_Sale" || model.MType == "Invoice_Purchase")
			{
				model.Validate(ctx, model.IsNew && !flag2, "FieldMustProvide", "必须提供“{0}”。", LangModule.Common, "DueDate");
				model.Validate(ctx, flag2 && model.MDueDate == new DateTime(1, 1, 1), "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "DueDate");
				if (model.MDueDate > new DateTime(1, 1, 1) && model.MDueDate < model.MBizDate)
				{
					List<ValidationError> validationErrors = model.ValidationErrors;
					string mLCID = ctx.MLCID;
					object[] obj3 = new object[2]
					{
						"DueDate",
						null
					};
					dateTime = model.MDueDate;
					obj3[1] = dateTime.ToString("yyyy-MM-dd");
					validationErrors.Add(new ValidationError(COMMultiLangRepository.GetTextFormat(mLCID, LangModule.IV, "DueDateSmallBizDate", "{0}“{1}”必须大于等于账单日期。", obj3)));
					return false;
				}
				if ((model.MType == "Invoice_Sale" & flag3) && model.MExpectedDate != DateTime.MinValue && model.MExpectedDate < model.MBizDate)
				{
					List<ValidationError> validationErrors2 = model.ValidationErrors;
					string mLCID2 = ctx.MLCID;
					object[] obj4 = new object[2]
					{
						"ExpectedPaymentDate",
						null
					};
					dateTime = model.MExpectedDate;
					obj4[1] = dateTime.ToString("yyyy-MM-dd");
					validationErrors2.Add(new ValidationError(COMMultiLangRepository.GetTextFormat(mLCID2, LangModule.IV, "DueDateSmallBizDate", "{0}“{1}”必须大于等于账单日期。", obj4)));
					return false;
				}
			}
			return true;
		}

		private DateTime GetDueDate(DateTime bizDate, string condition, int date)
		{
			DateTime result = DateTime.MinValue;
			if (date <= 0)
			{
				return result;
			}
			DateTime dateTime;
			switch (condition)
			{
			case "item0":
			{
				dateTime = DateTime.Now;
				int year2 = dateTime.Year;
				dateTime = DateTime.Now;
				dateTime = new DateTime(year2, dateTime.Month, date);
				result = dateTime.AddMonths(1);
				break;
			}
			case "item1":
				result = bizDate.AddDays((double)date);
				break;
			case "item2":
				result = new DateTime(bizDate.Year, bizDate.Month, date);
				break;
			case "item3":
			{
				dateTime = DateTime.Now;
				int year = dateTime.Year;
				dateTime = DateTime.Now;
				result = new DateTime(year, dateTime.Month, date);
				break;
			}
			}
			return result;
		}

		private bool ProcessCurrency(MContext ctx, IVInvoiceModel model, IVInvoiceModel dbModel)
		{
			bool flag = model.IsUpdateFieldExists("MCyID");
			bool flag2 = model.IsUpdateFieldExists("MExchangeRate");
			model.MCyID = (string.IsNullOrEmpty(model.MCyID) ? "" : model.MCyID.ToUpper());
			if (model.MCyID == ctx.MBasCurrencyID)
			{
				model.MExchangeRate = decimal.One;
				flag2 = true;
			}
			if (flag2)
			{
				model.UpdateFieldList.Add("MExchangeRate");
				model.UpdateFieldList.Add("MOToLRate");
				model.MLToORate = Math.Round(decimal.One / model.MExchangeRate, 6);
				model.UpdateFieldList.Add("MLToORate");
			}
			if (model.IsNew)
			{
				if (!flag)
				{
					model.MCyID = ctx.MBasCurrencyID;
					model.MExchangeRate = decimal.One;
					model.UpdateFieldList.Add("MCyID");
					model.UpdateFieldList.Add("MExchangeRate");
					model.UpdateFieldList.Add("MOToLRate");
					model.UpdateFieldList.Add("MLToORate");
					flag2 = true;
				}
				if (!flag2 && !GetExchangeRate(ctx, model, ref flag2))
				{
					return false;
				}
			}
			else
			{
				if (!flag && !flag2)
				{
					return true;
				}
				if (!flag)
				{
					model.MCyID = dbModel.MCyID;
				}
				else if (!flag2)
				{
					model.MExchangeRate = dbModel.MExchangeRate;
				}
				if (flag && !GetExchangeRate(ctx, model, ref flag2))
				{
					return false;
				}
			}
			if (string.IsNullOrWhiteSpace(model.MCyID))
			{
				model.Validate(ctx, true, "FieldEmpty", "'{0}' cannot be empty.", LangModule.Common, "CurrencyCode");
				return false;
			}
			if (model.MCyID.ToUpper() != ctx.MBasCurrencyID.ToUpper())
			{
				REGCurrencyViewModel rEGCurrencyViewModel = _currencyDataPool.FirstOrDefault((REGCurrencyViewModel t) => t.MCurrencyID == model.MCyID.Trim().ToUpper());
				if (rEGCurrencyViewModel == null)
				{
					model.Validate(ctx, true, "NotExists", "{0}'{1}' does not exist.", LangModule.Common, "CurrencyCode", model.MCyID);
					return false;
				}
				model.MCyID = rEGCurrencyViewModel.MCurrencyID;
			}
			if (model.MCyID == ctx.MBasCurrencyID)
			{
				model.MExchangeRate = decimal.One;
			}
			if (flag2 && model.MCurrencyRate <= decimal.Zero)
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExchangeRateGreaterThan0", "汇率必须大于0。")));
				return false;
			}
			return true;
		}

		private bool GetExchangeRate(MContext ctx, IVInvoiceModel model, ref bool isRateExists)
		{
			if (model.MCyID == ctx.MBasCurrencyID)
			{
				model.MExchangeRate = decimal.One;
				if (!isRateExists)
				{
					model.UpdateFieldList.Add("MExchangeRate");
					model.UpdateFieldList.Add("MOToLRate");
					model.UpdateFieldList.Add("MLToORate");
				}
				return true;
			}
			if (isRateExists)
			{
				return true;
			}
			isRateExists = false;
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			List<REGCurrencyViewModel> billViewList = rEGCurrencyRepository.GetBillViewList(ctx, model.MBizDate, null, false);
			if (billViewList == null || billViewList.Count == 0)
			{
				model.Validate(ctx, true, "NotRateForInvoice", "The Currency Rate for the invoice date is not maintained in system.", LangModule.IV, model.MCyID);
				return false;
			}
			REGCurrencyViewModel rEGCurrencyViewModel = billViewList.FirstOrDefault((REGCurrencyViewModel t) => t.MCurrencyID == model.MCyID && !string.IsNullOrEmpty(t.MExchangeRateID));
			if (rEGCurrencyViewModel == null)
			{
				model.Validate(ctx, true, "NotRateForInvoice", "The Currency Rate for the invoice date is not maintained in system.", LangModule.IV, model.MCyID);
				return false;
			}
			model.MExchangeRate = Convert.ToDecimal(rEGCurrencyViewModel.MRate);
			model.MLToORate = Math.Round(decimal.One / model.MExchangeRate, 6);
			model.UpdateFieldList.Add("MExchangeRate");
			model.UpdateFieldList.Add("MOToLRate");
			model.UpdateFieldList.Add("MLToORate");
			isRateExists = true;
			return true;
		}

		private bool ProcessReference(MContext ctx, IVInvoiceModel model)
		{
			if (model.IsNew)
			{
				if (string.IsNullOrEmpty(model.MReference))
				{
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ReferenceIsNull", "备注不能为空!")));
					return false;
				}
				return true;
			}
			if (!model.IsUpdateFieldExists("MReference"))
			{
				return true;
			}
			if (string.IsNullOrEmpty(model.MReference))
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ReferenceIsNull", "备注不能为空!")));
				return false;
			}
			return true;
		}

		private bool TypeIsOk(IVInvoiceModel model)
		{
			if (!model.IsUpdateFieldExists("MType"))
			{
				return true;
			}
			string mType = model.MType;
			int result;
			switch (mType)
			{
			default:
				result = ((mType == "Invoice_Sale") ? 1 : 0);
				break;
			case "Invoice_Sale_Red":
			case "Invoice_Purchase_Red":
			case "Invoice_Purchase":
				result = 1;
				break;
			}
			return (byte)result != 0;
		}
	}
}
