using JieNor.Megi.BusinessService.FP;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FP;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JieNor.Megi.BusinessService.IO.Biz
{
	public class FaPiaoImport : ImportBase, IImport
	{
		private List<CommandInfo> cmdList = new List<CommandInfo>();

		private FPFapiaoRepository fpRepository = new FPFapiaoRepository();

		private BDContactsRepository contactsRepository = new BDContactsRepository();

		private FPImportRepository fpImportRepository = new FPImportRepository();

		private FPFapiaoBusiness fpBusiness = new FPFapiaoBusiness();

		private IOSolutionBusiness solution = new IOSolutionBusiness();

		public int GeneralTitleBegin;

		public ImportResult ImportData(MContext ctx, IOImportDataModel data)
		{
			ImportResult importResult = new ImportResult();
			string solutionID = data.SolutionID;
			int num = 999999;
			if (solutionID != num.ToString() && !data.IsSaveData)
			{
				OperationResult operationResult = solution.SaveSolution(ctx, data, false);
				if (!operationResult.Success)
				{
					importResult.Message = operationResult.Message;
					return importResult;
				}
			}
			IOSolutionModel solutionModel = solution.GetSolutionModel(ctx, data.SolutionID);
			if (solutionModel == null)
			{
				importResult.Success = false;
				return importResult;
			}
			data.MDataRowIndex = solutionModel.MDataRowIndex;
			data.MHeaderRowIndex = solutionModel.MHeaderRowIndex;
			List<IOSolutionConfigModel> solutionConfigList = solution.GetSolutionConfigList(ctx, data.TemplateType, data.SolutionID);
			if (solutionConfigList == null || solutionConfigList.Count == 0)
			{
				importResult.Success = false;
				return importResult;
			}
			data.EffectiveData = GetEffiectiveData(ctx, data, solutionConfigList, data.SourceData, out List<string> list);
			if (list.Count > 0)
			{
				importResult.MessageList = list;
				importResult.Success = false;
				return importResult;
			}
			if (data.EffectiveData == null || data.EffectiveData.Rows.Count == 0)
			{
				importResult.Success = false;
				importResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NoDataToImport", "That's no data to import!");
				return importResult;
			}
			importResult.Success = true;
			try
			{
				TranslateData(data.EffectiveData, data.TemplateType);
				List<FPFapiaoModel> list2 = MergeFaPiaoList(ctx, data.EffectiveData, data.TemplateType, importResult);
				CheckPaPiaoIsRepeat(ctx, list2, list);
				if (list.Count > 0)
				{
					importResult.MessageList = list;
					importResult.Success = false;
					return importResult;
				}
				foreach (FPFapiaoModel item2 in list2)
				{
					List<FPFapiaoEntryModel> mFapiaoEntrys = item2.MFapiaoEntrys;
					item2.MAmount = mFapiaoEntrys.Sum((FPFapiaoEntryModel m) => m.MAmount);
					item2.MTaxAmount = mFapiaoEntrys.Sum((FPFapiaoEntryModel m) => m.MTaxAmount);
					item2.MTotalAmount = item2.MAmount + item2.MTaxAmount;
					if (item2.MStatus == 1 && item2.MTotalAmount < decimal.Zero)
					{
						importResult.Success = false;
						importResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "RedCannotBeNormal", "负数发票不能为正常状态！");
						return importResult;
					}
					if (item2.MStatus == 4 && item2.MTotalAmount >= decimal.Zero)
					{
						importResult.Success = false;
						importResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "NonNegativeCannotBeCredit", "非负数发票不能为红冲状态！");
						return importResult;
					}
					string solutionID2 = data.SolutionID;
					num = 999999;
					if (solutionID2 == num.ToString() || item2.MStatus == -1)
					{
						item2.MStatus = ((!(item2.MTotalAmount < decimal.Zero)) ? 1 : 4);
					}
				}
				foreach (FPFapiaoModel item3 in list2)
				{
					List<string> list3 = COMModelValidateHelper.ValidateModel(ctx, item3);
					if (list3.Any())
					{
						importResult.Success = false;
						importResult.MessageList.AddRange(list3);
						return importResult;
					}
					if (list2.Count((FPFapiaoModel m) => m.MCode == item3.MCode && m.MNumber == item3.MNumber) > 1)
					{
						importResult.Success = false;
						importResult.MessageList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "ImportFapiaoNumberDuplicated", "导入的文件中含有重复发票编号：{0}"), item3.MNumber));
						return importResult;
					}
				}
				if (data.IsSaveData)
				{
					FpImportData(ctx, data, importResult, list2);
				}
				else
				{
					DateTime glBeginDate = ctx.MGLBeginDate;
					importResult.HasGLStartData = (from t in list2
					where t.MBizDate.CompareTo(glBeginDate) < 0
					select t).Any();
					importResult.NormalFPCount = (from t in list2
					where t.MType == 0
					select t).Count();
					importResult.SpecialFPCount = (from t in list2
					where t.MType == 1
					select t).Count();
				}
			}
			catch (Exception ex)
			{
				importResult.Success = false;
				importResult.Message = ex.Message;
				importResult.Tag = "1";
			}
			return importResult;
		}

		public virtual DataTable GetEffiectiveData(MContext ctx, IOImportDataModel model, List<IOSolutionConfigModel> soluConfig, DataTable sourceData, out List<string> errorMsgList)
		{
			errorMsgList = new List<string>();
			if (!CheckSourceDataIsEmpty(sourceData, errorMsgList))
			{
				return null;
			}
			int mHeaderRowIndex = model.MHeaderRowIndex;
			int mDataRowIndex = model.MDataRowIndex;
			return GetNewFaPiaoDataTable(ctx, soluConfig, sourceData, mHeaderRowIndex - 1, mDataRowIndex - 1, out errorMsgList);
		}

		public DataTable GetNewFaPiaoDataTable(MContext ctx, List<IOSolutionConfigModel> soluConfig, DataTable sourceData, int titleRowIndex, int dataRowIndex, out List<string> errorMsgList)
		{
			errorMsgList = new List<string>();
			DataTable dataTable = new DataTable();
			dataTable.TableName = "datatable1";
			int count = sourceData.Rows.Count;
			DataRow headerDr = sourceData.Rows[titleRowIndex];
			List<KeyValuePair<int, IOSolutionConfigModel>> columnMath = base.GetColumnMath(headerDr, soluConfig);
			if (!CheckDataRowIndexIsEffective(ctx, sourceData, headerDr, dataRowIndex, soluConfig, errorMsgList))
			{
				return dataTable;
			}
			foreach (IOSolutionConfigModel item in soluConfig)
			{
				dataTable.Columns.Add(item.MConfigStandardName);
			}
			dataTable.Columns.Add("CodeNumber");
			string text = "";
			List<string> list = new List<string>();
			for (int i = dataRowIndex; i < count; i++)
			{
				DataRow dataRow = sourceData.Rows[i];
				DataRow dataRow2 = dataTable.NewRow();
				if (!CheckIsEmptyRows(columnMath, dataRow))
				{
					bool isMainFaPiao = false;
					dataRow2["CodeNumber"] = dataRow[0] + dataRow[1].ToString();
					if (CheckIsMainFaPiao(columnMath, dataRow))
					{
						text = dataRow2["CodeNumber"].ToString();
						isMainFaPiao = true;
					}
					else
					{
						dataRow2["CodeNumber"] = text;
					}
					if (CheckDataRowData(ctx, columnMath, dataRow, dataRow2, errorMsgList, i, isMainFaPiao, out bool flag))
					{
						dataTable.Rows.Add(dataRow2);
						if (CheckIsMainFaPiao(columnMath, dataRow))
						{
							SetInvoiceType(i, GeneralTitleBegin, dataRow2);
						}
					}
					if (!flag)
					{
						list.Add(text);
					}
				}
			}
			return RemoveErrorData(ctx, dataTable, list, errorMsgList);
		}

		public virtual bool CheckDataRowData(MContext ctx, List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr, DataRow newDr, List<string> errorMsgList, int rowIndex, bool isMainFaPiao, out bool isRightDataRow)
		{
			bool flag = true;
			isRightDataRow = true;
			foreach (KeyValuePair<int, IOSolutionConfigModel> item in columnMath)
			{
				object obj = dr[item.Key];
				string text = obj?.ToString() ?? string.Empty;
				if (item.Value.MConfigStandardName == "MCode" && !CheckFaPiaoCodeColumnIsEffective(text))
				{
					flag = false;
					break;
				}
				if (item.Value.MConfigStandardName == "MItemName" && text.Trim() == "小计")
				{
					flag = false;
					break;
				}
				if (item.Value.MConfigStandardName == "MTaxAmount1" && string.IsNullOrEmpty(text))
				{
					flag = false;
					isRightDataRow = false;
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxAmountIsRequired", "第{0}行税额不能为空！"), rowIndex + 1));
					break;
				}
				if (item.Value.MDataType == "decimal" && !string.IsNullOrEmpty(text))
				{
					text = text.Replace(",", "").Replace("，", "");
					decimal num = default(decimal);
					if (!decimal.TryParse(text.Trim('%'), out num))
					{
						if (!RegExp.IsScientificCountFormat(text))
						{
							flag = false;
							isRightDataRow = false;
							string mConfigStandardName = item.Value.MConfigStandardName;
							switch (mConfigStandardName)
							{
							default:
								if (mConfigStandardName == "MEntryAmount")
								{
									errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "EntryAmountFormatError", "第{0}行金额格式错误！"), rowIndex + 1));
								}
								break;
							case "MQuantity":
								errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "QuantityFormatError", "第{0}行商品数量格式错误！"), rowIndex + 1));
								break;
							case "MPrice":
								errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "PriceFormatError", "第{0}行单价格式错误！"), rowIndex + 1));
								break;
							case "MTaxRate":
								errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxRateFormatError", "第{0}行税率格式错误！"), rowIndex + 1));
								break;
							case "MTaxAmount1":
								errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxAmountFormatError", "第{0}行税额格式错误！"), rowIndex + 1));
								break;
							}
							break;
						}
						num = decimal.Parse(text, NumberStyles.Float);
					}
				}
				if (item.Value.MDataType == "datetime" && !string.IsNullOrEmpty(text) && !DateTime.TryParse(text, out DateTime _))
				{
					flag = false;
					isRightDataRow = false;
					string mConfigStandardName2 = item.Value.MConfigStandardName;
					if (!(mConfigStandardName2 == "MBizDate"))
					{
						if (mConfigStandardName2 == "MVerifyDate")
						{
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "VerifyDateFormatError", "第{0}行认证日期格式错误！"), rowIndex + 1));
						}
					}
					else
					{
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "BizDateFormatError", "第{0}行开票日期格式错误！"), rowIndex + 1));
					}
					break;
				}
				if (isMainFaPiao)
				{
					if (item.Value.MIsDataRequired && string.IsNullOrEmpty(text))
					{
						flag = false;
						isRightDataRow = false;
						switch (item.Value.MConfigStandardName)
						{
						case "MCode":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "CodeIsRequired", "第{0}行发票代码不能为空！"), rowIndex + 1));
							break;
						case "MNumber":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "NumberIsRequired", "第{0}行发票号码不能为空！"), rowIndex + 1));
							break;
						case "MBizDate":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "BizDateIsRequired", "第{0}行开票日期不能为空！"), rowIndex + 1));
							break;
						case "MType":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TypeIsRequired", "第{0}行发票类型不能为空！"), rowIndex + 1));
							break;
						case "MTaxAmount1":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxAmountIsRequired", "第{0}行税额不能为空！"), rowIndex + 1));
							break;
						case "MEntryAmount":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "EntryAmountIsRequired", "第{0}行金额不能为空！"), rowIndex + 1));
							break;
						case "MTaxRate":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxRateIsRequired", "第{0}行税率不能为空！"), rowIndex + 1));
							break;
						case "MItemName":
							errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "ItemNameIsRequired", "第{0}行商品名称不能为空！"), rowIndex + 1));
							break;
						}
						if (errorMsgList.Count > 0)
						{
							break;
						}
					}
					if (!(item.Value.MDataType == "datetime"))
					{
						goto IL_074f;
					}
					goto IL_074f;
				}
				goto IL_0865;
				IL_074f:
				if (item.Value.MConfigStandardName == "MType")
				{
					if (!IsFapiaoType(text))
					{
						flag = false;
						isRightDataRow = false;
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FaPiaoTypeIsError", "第{0}行发票类型信息只能填写增值税普通发票、增值税普票、增值税专用发票、增值税专票,请重新填写后进行再次导入!"), rowIndex + 1));
						break;
					}
					if (IsCommonFapiao(text) && !IsSetLocalUpload(ctx, 0))
					{
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "CommonFaPiaoSettingIsError", "第{0}行普通发票设置为非本地上传，需要在发票设置中将导入方式修改为本地上传，再进行导入"), rowIndex + 1));
						break;
					}
					if (IsSpecilaFapiao(text) && !IsSetLocalUpload(ctx, 1))
					{
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "SpecialFaPiaoSettingIsError", "第{0}行专用发票设置为非本地上传，需要在发票设置中将导入方式修改为本地上传，再进行导入"), rowIndex + 1));
						break;
					}
				}
				goto IL_0865;
				IL_0865:
				if (item.Value.MConfigStandardName == "MStatus" && !string.IsNullOrEmpty(text) && !(text == "正常") && !(text == "作废") && !(text == "红冲") && !(text == "失控") && !(text == "异常"))
				{
					flag = false;
					isRightDataRow = false;
					errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "FaPiaoStatusError", "第{0}行发票状态信息只能填写正常、作废、红冲、失控和异常,请重写填写后再次进行导入！"), rowIndex + 1));
				}
				CheckColumnDataIsEffective(ctx, columnMath, dr, item, text, errorMsgList, rowIndex, isMainFaPiao, out isRightDataRow, out flag);
				if (flag && isRightDataRow)
				{
					newDr[item.Value.MConfigStandardName] = text;
					continue;
				}
				break;
			}
			return flag;
		}

		private List<FPFapiaoModel> MergeFaPiaoList(MContext ctx, DataTable effectiveData, ImportTypeEnum importType, ImportResult result)
		{
			List<FPFapiaoModel> list = new List<FPFapiaoModel>();
			int count = effectiveData.Rows.Count;
			FPFapiaoModel fPFapiaoModel = new FPFapiaoModel();
			string value = "";
			for (int i = 0; i < count; i++)
			{
				DataRow dataRow = effectiveData.Rows[i];
				if (!string.IsNullOrEmpty(dataRow[0].ToString().Trim()))
				{
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(fPFapiaoModel);
						fPFapiaoModel = GetFaPiaoModel(dataRow);
						value = dataRow[0].ToString().Trim();
					}
					else
					{
						fPFapiaoModel = GetFaPiaoModel(dataRow);
						value = dataRow[0].ToString().Trim();
					}
				}
				else
				{
					List<FPFapiaoEntryModel> mFapiaoEntrys = fPFapiaoModel.MFapiaoEntrys;
					FPFapiaoEntryModel faPiaoEntryModel = GetFaPiaoEntryModel(dataRow);
					faPiaoEntryModel.MSeq = mFapiaoEntrys.Count;
					mFapiaoEntrys.Add(faPiaoEntryModel);
				}
				if (i == count - 1)
				{
					list.Add(fPFapiaoModel);
				}
			}
			return list;
		}

		private FPFapiaoModel GetFaPiaoModel(DataRow dr)
		{
			FPFapiaoModel fPFapiaoModel = new FPFapiaoModel();
			fPFapiaoModel.MCode = dr["MCode"].ToString().Trim();
			fPFapiaoModel.MNumber = dr["MNumber"].ToString().Trim();
			fPFapiaoModel.MBizDate = Convert.ToDateTime(dr["MBizDate"]);
			fPFapiaoModel.MType = Convert.ToInt32(dr["MType"]);
			if (!string.IsNullOrEmpty(dr["MRemark"]?.ToString()))
			{
				fPFapiaoModel.MExplanation = dr["MRemark"].ToString();
			}
			if (!string.IsNullOrEmpty(dr["MReceiver"]?.ToString()))
			{
				fPFapiaoModel.MReceiver = dr["MReceiver"].ToString();
			}
			if (!string.IsNullOrEmpty(dr["MReaduitor"]?.ToString()))
			{
				fPFapiaoModel.MReaduitor = dr["MReaduitor"].ToString();
			}
			if (!string.IsNullOrEmpty(dr["MDrawer"]?.ToString()))
			{
				fPFapiaoModel.MDrawer = dr["MDrawer"].ToString();
			}
			if (!string.IsNullOrEmpty(dr["MStatus"]?.ToString()))
			{
				fPFapiaoModel.MStatus = Convert.ToInt32(dr["MStatus"]);
			}
			else
			{
				fPFapiaoModel.MStatus = -1;
			}
			SetFaPiaoModel(dr, fPFapiaoModel);
			List<FPFapiaoEntryModel> obj = new List<FPFapiaoEntryModel>
			{
				GetFaPiaoEntryModel(dr)
			};
			List<FPFapiaoEntryModel> list2 = fPFapiaoModel.MFapiaoEntrys = obj;
			list2[0].MSeq = 0;
			return fPFapiaoModel;
		}

		public virtual void SetFaPiaoModel(DataRow dr, FPFapiaoModel model)
		{
		}

		private FPFapiaoEntryModel GetFaPiaoEntryModel(DataRow dr)
		{
			FPFapiaoEntryModel fPFapiaoEntryModel = new FPFapiaoEntryModel();
			string text = dr["MItemName"].ToString().Trim();
			GroupCollection itemNameGroup = GetItemNameGroup(text);
			bool flag = itemNameGroup.Count > 2;
			fPFapiaoEntryModel.MItemCategoryCode = (flag ? itemNameGroup[1].ToString() : string.Empty);
			fPFapiaoEntryModel.MItemName = (flag ? itemNameGroup[2].ToString() : text);
			fPFapiaoEntryModel.MItemType = dr["MItemType"].ToString().Trim();
			fPFapiaoEntryModel.MUnit = dr["MUnit"].ToString().Trim();
			fPFapiaoEntryModel.MQuantity = dr["MQuantity"].ToString().ToDecimal();
			fPFapiaoEntryModel.MPrice = dr["MPrice"].ToString().ToDecimal();
			fPFapiaoEntryModel.MTaxPercent = dr["MTaxRate"].ToString().Trim('%').ToDecimal();
			fPFapiaoEntryModel.MTaxAmount = dr["MTaxAmount1"].ToString().ToDecimal(2);
			fPFapiaoEntryModel.MAmount = dr["MEntryAmount"].ToString().ToDecimal(2);
			fPFapiaoEntryModel.MTotalAmount = fPFapiaoEntryModel.MAmount + fPFapiaoEntryModel.MTaxAmount;
			return fPFapiaoEntryModel;
		}

		private void FpImportData(MContext ctx, IOImportDataModel data, ImportResult result, List<FPFapiaoModel> list)
		{
			string guid = UUIDHelper.GetGuid();
			List<FPImpportModel> list2 = new List<FPImpportModel>();
			FPImpportModel fPImpportModel = new FPImpportModel();
			fPImpportModel.IsNew = true;
			fPImpportModel.MID = guid;
			fPImpportModel.MDate = DateTime.Now;
			fPImpportModel.MCount = list.Count;
			fPImpportModel.MStartDate = list.Min((FPFapiaoModel m) => m.MBizDate);
			fPImpportModel.MEndDate = list.Max((FPFapiaoModel m) => m.MBizDate);
			fPImpportModel.MAmount = list.Sum((FPFapiaoModel m) => m.MAmount);
			fPImpportModel.MTaxAmount = list.Sum((FPFapiaoModel m) => m.MTaxAmount);
			fPImpportModel.MTotalAmount = fPImpportModel.MAmount + fPImpportModel.MTaxAmount;
			fPImpportModel.MSource = 2;
			fPImpportModel.MFileName = data.FileName;
			fPImpportModel.MFapiaoCategory = data.FaPiaoType;
			list2.Add(fPImpportModel);
			cmdList.AddRange(fpImportRepository.GetSaveFPImportCmds(ctx, list2));
			FaPiaoListDetail(ctx, list, guid, data);
			if (result.Success)
			{
				List<CommandInfo> saveFapiaoListCmds = fpRepository.GetSaveFapiaoListCmds(ctx, list);
				fpBusiness.SetFapiaoBasicDataID(ctx, list, saveFapiaoListCmds);
				List<string> fapiaoIDs = (from x in list
				where !string.IsNullOrWhiteSpace(x.MID) && x.MChangeToObsolete
				select x.MID).ToList();
				saveFapiaoListCmds.AddRange(fpRepository.GetDeleteFapiaoTableReconcileCmds(ctx, fapiaoIDs));
				if (saveFapiaoListCmds.Any())
				{
					cmdList.AddRange(saveFapiaoListCmds);
					DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
					result.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(cmdList) > 0);
				}
				else
				{
					result.Success = false;
					List<string> list3 = new List<string>();
					foreach (FPFapiaoModel item in list)
					{
						List<ValidationError> validationErrors = item.ValidationErrors;
						if (validationErrors.Any())
						{
							list3.AddRange((from m in validationErrors
							select m.Message).ToList());
						}
					}
					result.MessageList = list3;
				}
			}
		}

		private void FaPiaoListDetail(MContext ctx, List<FPFapiaoModel> faPiaoList, string importId, IOImportDataModel data)
		{
			foreach (FPFapiaoModel faPiao in faPiaoList)
			{
				SetFapiaoListDetail(ctx, faPiao);
				faPiao.MSource = 2;
				faPiao.MTotalAmount = faPiao.MAmount + faPiao.MTaxAmount;
				faPiao.MImportID = importId;
				if (!string.IsNullOrEmpty(faPiao.MSContactTaxCode))
				{
					BDContactsModel contactModelByTaxNo = contactsRepository.GetContactModelByTaxNo(ctx, faPiao.MSContactTaxCode);
					if (contactModelByTaxNo != null)
					{
						faPiao.MContactID = contactModelByTaxNo.MItemID;
						if (string.IsNullOrEmpty(faPiao.MContactName))
						{
							faPiao.MContactName = contactModelByTaxNo.MName;
						}
					}
				}
			}
		}

		public virtual void SetFapiaoListDetail(MContext ctx, FPFapiaoModel model)
		{
		}

		private DataTable TranslateData(DataTable dt, ImportTypeEnum importType)
		{
			DataColumnCollection columns = dt.Columns;
			List<string> list = new List<string>();
			if (columns.Contains("MVerifyType"))
			{
				list.Add("MVerifyType");
			}
			if (columns.Contains("MStatus"))
			{
				list.Add("MStatus");
			}
			if (columns.Contains("MType"))
			{
				list.Add("MType");
			}
			if (!list.Any())
			{
				return dt;
			}
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				foreach (string item in list)
				{
					int fapiaoType = TransferFaPiaoType(dataRow["MType"].ToString());
					switch (item)
					{
					case "MVerifyType":
					{
						string verifyType = dataRow["MVerifyType"].ToString();
						dataRow["MVerifyType"] = TransferVerifyType(verifyType, (FapiaoTypeEnum)fapiaoType, importType);
						break;
					}
					case "MType":
						dataRow["MType"] = fapiaoType.ToString();
						break;
					case "MStatus":
					{
						string status = dataRow["MStatus"].ToString();
						dataRow["MStatus"] = TransferStatus(status);
						break;
					}
					}
				}
			}
			return dt;
		}

		private static string TransferVerifyType(string verifyType, FapiaoTypeEnum fapiaoType, ImportTypeEnum importType)
		{
			int num;
			if ((importType == ImportTypeEnum.InFaPiao && fapiaoType == FapiaoTypeEnum.Common) || importType == ImportTypeEnum.OutFaPiao)
			{
				num = 3;
				return num.ToString();
			}
			string result;
			if (!(verifyType == "扫描认证"))
			{
				if (verifyType == "勾选认证")
				{
					num = 2;
					result = num.ToString();
				}
				else
				{
					num = 0;
					result = num.ToString();
				}
			}
			else
			{
				num = 1;
				result = num.ToString();
			}
			return result;
		}

		private static int TransferFaPiaoType(string fapiaoType)
		{
			int result = 0;
			switch (fapiaoType)
			{
			case "增值税普通发票":
			case "增值税普票":
				result = 0;
				break;
			case "增值税专用发票":
			case "增值税专票":
				result = 1;
				break;
			}
			return result;
		}

		private static string TransferStatus(string status)
		{
			int num = -1;
			string result = num.ToString();
			switch (status)
			{
			case "正常":
				num = 1;
				result = num.ToString();
				break;
			case "作废":
				num = 0;
				result = num.ToString();
				break;
			case "失控":
				num = 2;
				result = num.ToString();
				break;
			case "异常":
				num = 3;
				result = num.ToString();
				break;
			case "红冲":
				num = 4;
				result = num.ToString();
				break;
			}
			return result;
		}

		private static GroupCollection GetItemNameGroup(string itemName)
		{
			Match match = Regex.Match(itemName, "^\\*(?<category>.+?)(?=\\*)\\*(?<name>.*)");
			return match.Groups;
		}

		public static bool CheckSourceDataIsEmpty(DataTable sourceData, List<string> errorMsgList)
		{
			if (sourceData == null || sourceData.Rows.Count <= 0)
			{
				errorMsgList.Add(COMMultiLangRepository.GetText(LangModule.FP, "ImportFaPiaoDataIsNull", "导入的发票为空"));
				return false;
			}
			return true;
		}

		public bool CheckDataRowIndexIsEffective(MContext ctx, DataTable sourceData, DataRow headerDr, int dataRowIndex, List<IOSolutionConfigModel> soluConfig, List<string> errorMsgList)
		{
			int count = sourceData.Rows.Count;
			if (dataRowIndex > count - 1)
			{
				errorMsgList.Add(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FaPiaoNotIncludeImportFail", "不含发票行，导入失败！"));
				return false;
			}
			DataRow dr = sourceData.Rows[dataRowIndex];
			List<KeyValuePair<int, IOSolutionConfigModel>> columnMath = base.GetColumnMath(headerDr, soluConfig);
			if (!CheckIsMainFaPiao(columnMath, dr))
			{
				errorMsgList.Add(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "NotIncludeFaPiaoCodeOrNumberImportFail", "标题行的下一行不含发票代码或者发票号码导入失败！"));
				return false;
			}
			return true;
		}

		public static bool CheckIsEmptyRows(List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr)
		{
			return columnMath.All((KeyValuePair<int, IOSolutionConfigModel> kv) => dr[kv.Key] == null || string.IsNullOrWhiteSpace(dr[kv.Key].ToString()));
		}

		public static bool CheckIsMainFaPiao(List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr)
		{
			return (from kv in columnMath
			where kv.Value.MIsKey && kv.Value.MIsDataRequired
			select kv).Any((KeyValuePair<int, IOSolutionConfigModel> kv) => !string.IsNullOrEmpty(dr[kv.Key].ToString()));
		}

		public static bool CheckFaPiaoCodeColumnIsEffective(string fapiaoCode)
		{
			bool result = true;
			fapiaoCode = fapiaoCode.Trim();
			bool flag = fapiaoCode == "发票类别：专用发票" || fapiaoCode == "发票类别:专用发票";
			bool flag2 = fapiaoCode == "发票类别：普通发票" || fapiaoCode == "发票类别:普通发票";
			bool flag3 = fapiaoCode.StartsWith("份数");
			bool flag4 = fapiaoCode == "发票代码";
			if (flag | flag2 | flag3 | flag4)
			{
				result = false;
			}
			return result;
		}

		public virtual void CheckColumnDataIsEffective(MContext ctx, List<KeyValuePair<int, IOSolutionConfigModel>> columnMath, DataRow dr, KeyValuePair<int, IOSolutionConfigModel> kv, string str, List<string> errorMsgList, int rowIndex, bool isMainFaPiao, out bool isRightDataRow, out bool isRightRow)
		{
			isRightRow = true;
			isRightDataRow = true;
		}

		public static DataTable RemoveErrorData(MContext ctx, DataTable dt, List<string> errorCodeNumber, List<string> errorMsgList)
		{
			DataTable dataTable = dt.Clone();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dr = dt.Rows[i];
				if (errorCodeNumber.All((string m) => m.Trim() != dr["CodeNumber"].ToString()))
				{
					dataTable.Rows.Add(dr.ItemArray);
				}
				if (!string.IsNullOrEmpty(dr["MTaxRate"].ToString()))
				{
					decimal d = Convert.ToDecimal(dr["MTaxRate"].ToString().Trim('%'));
					decimal d2 = Convert.ToDecimal(string.IsNullOrEmpty(dr["MTaxAmount1"].ToString()) ? "0" : dr["MTaxAmount1"]);
					if (d == decimal.Zero && d2 != decimal.Zero)
					{
						errorMsgList.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FP, "TaxAmountIsError", "第{0}行税率为0，税额不为0"), i + 1));
					}
				}
			}
			return dataTable;
		}

		public string GetConfigColumnList(MContext ctx, IOConfigModel item, DataRow dr)
		{
			if (dr == null)
			{
				return string.Empty;
			}
			MultiLanguageFieldList multiLanguageFieldList = item.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
			if (multiLanguageFieldList?.MMultiLanguageField == null || multiLanguageFieldList.MMultiLanguageField.Count == 0)
			{
				return string.Empty;
			}
			foreach (string item2 in from col in dr.ItemArray
			where col != null && col != DBNull.Value
			select col.ToString() into str
			where !string.IsNullOrEmpty(str)
			select str)
			{
				if (!string.IsNullOrEmpty(item.MSimilarName))
				{
					string[] source = item.MSimilarName.Split(';');
					if (source.Count((string t) => t.ToLower() == item2.ToLower()) > 0)
					{
						return item2;
					}
				}
				int num = multiLanguageFieldList.MMultiLanguageField.Count((MultiLanguageField t) => !string.IsNullOrEmpty(t.MValue) && t.MValue.ToLower() == item2.ToLower());
				if (num > 0)
				{
					return item2;
				}
			}
			return string.Empty;
		}

		public void CheckPaPiaoIsRepeat(MContext ctx, List<FPFapiaoModel> importFapiaoList, List<string> errorMsgList)
		{
			List<string> list = new List<string>();
			list.Add("MID");
			List<FPFapiaoModel> fapiaoWithFields = new FPFapiaoRepository().GetFapiaoWithFields(ctx, (from x in importFapiaoList
			select x.MCode + x.MNumber).ToList(), list, "");
			if (fapiaoWithFields != null && fapiaoWithFields.Count > 0)
			{
				List<FPFapiaoModel> sysFapiaoList = SetFapiaoEntry(ctx, fapiaoWithFields);
				CheckFapiaoModelListIsEqual(ctx, importFapiaoList, sysFapiaoList, errorMsgList);
			}
		}

		public List<FPFapiaoModel> SetFapiaoEntry(MContext ctx, List<FPFapiaoModel> fapiaoList)
		{
			List<string> mFapiaoIDs = (from item in fapiaoList
			select item.MID).ToList();
			FPFapiaoFilterModel filter = new FPFapiaoFilterModel
			{
				MFapiaoIDs = mFapiaoIDs
			};
			return new FPFapiaoRepository().GetFapiaoListIncludeEntry(ctx, filter);
		}

		public virtual void CheckFapiaoModelListIsEqual(MContext ctx, List<FPFapiaoModel> importFapiaoList, List<FPFapiaoModel> sysFapiaoList, List<string> errorMsgList)
		{
		}

		public bool CheckFapiaoModelIsEqual(FPFapiaoModel importFapiao, FPFapiaoModel sysFapiao)
		{
			if (!CheckFapiaoBodyIsEqual(importFapiao, sysFapiao))
			{
				return false;
			}
			if (!CheckFapiaoEntrysIsEqual(importFapiao, sysFapiao))
			{
				return false;
			}
			return true;
		}

		public virtual bool CheckFapiaoBodyIsEqual(FPFapiaoModel importFapiao, FPFapiaoModel sysFapiao)
		{
			return true;
		}

		public bool CheckFapiaoEntrysIsEqual(FPFapiaoModel importFapiao, FPFapiaoModel sysFapiao)
		{
			if (importFapiao.MFapiaoEntrys.Count != sysFapiao.MFapiaoEntrys.Count)
			{
				return false;
			}
			foreach (FPFapiaoEntryModel mFapiaoEntry in sysFapiao.MFapiaoEntrys)
			{
				int num = 1;
				foreach (FPFapiaoEntryModel mFapiaoEntry2 in importFapiao.MFapiaoEntrys)
				{
					if (CheckFapiaoEntryModelIsEqual(mFapiaoEntry, mFapiaoEntry2))
					{
						break;
					}
					if (!CheckFapiaoEntryModelIsEqual(mFapiaoEntry, mFapiaoEntry2) && num == importFapiao.MFapiaoEntrys.Count)
					{
						return false;
					}
					num++;
				}
			}
			return true;
		}

		public bool CompareStringField(string importField, string sysField)
		{
			bool flag = string.IsNullOrEmpty(importField);
			bool flag2 = string.IsNullOrEmpty(sysField);
			if (flag & flag2)
			{
				return true;
			}
			if (flag | flag2)
			{
				return false;
			}
			if (importField.Equals(sysField))
			{
				return true;
			}
			return false;
		}

		public bool IsFapiaoType(string fapiaoType)
		{
			return fapiaoType == "增值税普通发票" || fapiaoType == "增值税专用发票" || fapiaoType == "增值税专票" || fapiaoType == "增值税普票";
		}

		public bool IsCommonFapiao(string fapiaoType)
		{
			return fapiaoType == "增值税普通发票" || fapiaoType == "增值税普票";
		}

		public bool IsSpecilaFapiao(string fapiaoType)
		{
			return fapiaoType == "增值税专用发票" || fapiaoType == "增值税专票";
		}

		private bool CheckFapiaoEntryModelIsEqual(FPFapiaoEntryModel sysEntry, FPFapiaoEntryModel importEntry)
		{
			decimal num = sysEntry.MAmount;
			if (!num.Equals(Math.Round(importEntry.MAmount, 10)))
			{
				return false;
			}
			num = sysEntry.MPrice;
			if (!num.Equals(Math.Round(importEntry.MPrice, 10)))
			{
				return false;
			}
			num = sysEntry.MQuantity;
			if (!num.Equals(Math.Round(importEntry.MQuantity, 10)))
			{
				return false;
			}
			num = sysEntry.MTaxAmount;
			if (!num.Equals(Math.Round(importEntry.MTaxAmount, 10)))
			{
				return false;
			}
			num = sysEntry.MTaxPercent;
			if (!num.Equals(Math.Round(importEntry.MTaxPercent, 2)))
			{
				return false;
			}
			if (!CompareStringField(sysEntry.MItemName, importEntry.MItemName))
			{
				return false;
			}
			if (!CompareStringField(sysEntry.MItemType, importEntry.MItemType))
			{
				return false;
			}
			if (!CompareStringField(sysEntry.MUnit, importEntry.MUnit))
			{
				return false;
			}
			return true;
		}

		public virtual bool IsSetLocalUpload(MContext ctx, int type)
		{
			return true;
		}

		public virtual void SetInvoiceType(int currentDataIndex, int commonBeginIndex, DataRow dr)
		{
		}
	}
}
