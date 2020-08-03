using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.BusinessService.SEC;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.API;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JieNor.Megi.BusinessService
{
	public class BusinessServiceBase
	{
		public const string TEMPLATE_DATE_FORMAT = "yyyy-MM-dd";

		private readonly SECPermissionBusiness _svcPermission = new SECPermissionBusiness();

		private readonly GLSettlementBusiness _settle = new GLSettlementBusiness();

		public bool HavePermission(MContext ctx, string bizObjectKey, string permissionItem)
		{
			return _svcPermission.HavePermission(ctx, bizObjectKey, permissionItem, "");
		}

		public OperationResult ValidateImportCurrency(MContext ctx, string bankId, List<string> importCurrencyIDList)
		{
			OperationResult operationResult = new OperationResult();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			BDBankAccountEditModel bDBankAccountEditModel = bDAccountRepository.GetBDBankAccountEditModel(ctx, bankId);
			if (importCurrencyIDList.Distinct().Count() > 1 || !importCurrencyIDList.Contains(bDBankAccountEditModel.MCyID))
			{
				string arg = string.Empty;
				if (bDBankAccountEditModel.MultiLanguage != null)
				{
					arg = bDBankAccountEditModel.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				}
				string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "ImportTransactionError4Currency", "不能导入与{0}账户币别({1})不相同的交易!"), arg, bDBankAccountEditModel.MCyID);
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = message
				});
				return operationResult;
			}
			return operationResult;
		}

		public void CheckCurrencyExchangeRateExist<T>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult, List<REGCurrencyViewModel> forCurrencyList, string currencyField = "MCyID", string dateField = "MBizDate")
		{
			foreach (T item in list)
			{
				string modelValue = ModelHelper.GetModelValue(item, currencyField);
				DateTime dateTime = Convert.ToDateTime(ModelHelper.GetModelValue(item, dateField));
				string text = CheckCurrencyExchangeRate(ctx, forCurrencyList, modelValue, dateTime, false);
				if (!string.IsNullOrWhiteSpace(text))
				{
					int rowIndex = 0;
					int.TryParse(ModelHelper.TryGetModelValue(item, "MRowIndex"), out rowIndex);
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Currency,
						RowIndex = rowIndex,
						Message = text,
						FieldValue = ((dateTime == DateTime.MinValue) ? "" : dateTime.ToOrgZoneDateString(null))
					});
				}
			}
		}

		public string CheckCurrencyExchangeRate(MContext ctx, List<REGCurrencyViewModel> forCurrencyList, string currencyId, DateTime bizDate, bool isFromApi = false)
		{
			string result = string.Empty;
			if (currencyId != ctx.MBasCurrencyID)
			{
				string text = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExchangeRateNotFound", "{0}：无法找到{{1}}之前的汇率！"), currencyId);
				IEnumerable<REGCurrencyViewModel> source = from f in forCurrencyList
				where f.MCurrencyID == currencyId && f.MRateDate <= bizDate && !string.IsNullOrWhiteSpace(f.MRate)
				select f;
				if (!source.Any())
				{
					if (isFromApi)
					{
						text = string.Format(text, (bizDate == DateTime.MinValue) ? "" : bizDate.ToOrgZoneDateString(null));
					}
					result = text;
				}
			}
			return result;
		}

		public void CheckTaxRateSelect<T1, T2>(MContext ctx, List<T1> list, List<IOValidationResultModel> validationResult, bool validateEmpty = true)
		{
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "TaxRateUnSelect", "请选择税率！");
			foreach (T1 item in list)
			{
				PropertyInfo propertyInfo = item.GetType().GetProperties().SingleOrDefault((PropertyInfo f) => f.PropertyType == typeof(List<T2>) && f.Name != "MEntryList");
				List<T2> list2 = propertyInfo.GetValue(item, null) as List<T2>;
				foreach (T2 item2 in list2)
				{
					string modelValue = ModelHelper.GetModelValue(item, "MTaxID");
					if (validateEmpty && string.IsNullOrWhiteSpace(ModelHelper.GetModelValue(item2, "MTaxID")) && (modelValue == "Tax_Exclusive" || modelValue == "Tax_Inclusive"))
					{
						int rowIndex = 0;
						int.TryParse(ModelHelper.TryGetModelValue(item, "MRowIndex"), out rowIndex);
						validationResult.Add(new IOValidationResultModel
						{
							RowIndex = rowIndex,
							FieldType = IOValidationTypeEnum.TaxRate,
							Message = text
						});
					}
					else if (modelValue == "No_Tax")
					{
						ModelHelper.SetModelValue(item2, "MTaxID", string.Empty, null);
					}
				}
			}
		}

		public void ValidateDiscount<T, TEntry>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult) where T : IVBaseModel<TEntry> where TEntry : IVEntryBaseModel
		{
			if (list.Any((T f) => Enumerable.Any<TEntry>((IEnumerable<TEntry>)((IVBaseModel<TEntry>)f).MEntryList, (Func<TEntry, bool>)((TEntry e) => e.MDiscount < decimal.Zero || e.MDiscount > 100m))))
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.Discount,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DiscountInvoid", "折扣率必须大于等于零且小于等于100。")
				});
			}
		}

		public void ValidateTotalAmount<T, TEntry>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult) where T : IVBaseModel<TEntry> where TEntry : IVEntryBaseModel
		{
			if (list.Any((T f) => ((IVBaseModel<TEntry>)f).MTaxTotalAmtFor <= decimal.Zero))
			{
				validationResult.Add(new IOValidationResultModel
				{
					FieldType = IOValidationTypeEnum.TotalAmount,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TotalAmountNotToZero", "总金额必须大于零。")
				});
			}
		}

		public void ValidateBizDate<T>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult, bool filterInitData = false, string dateField = "MBizDate")
		{
			List<T> list2 = (from f in list
			where Convert.ToDateTime(ModelHelper.GetModelValue<T>(f, dateField)) < ctx.MBeginDate
			select f).ToList();
			DateTime dateTime;
			if (list2.Any())
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "datemustlargerthan", "The date must be larger than or equals to {0}.");
				foreach (T item in list2)
				{
					int rowIndex = 0;
					int.TryParse(ModelHelper.TryGetModelValue(item, "MRowIndex"), out rowIndex);
					IOValidationResultModel obj = new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.BizDate,
						RowIndex = rowIndex
					};
					string format = text;
					dateTime = ctx.MBeginDate;
					obj.Message = string.Format(format, dateTime.ToString("yyyy-MM-dd"));
					obj.FieldValue = ctx.MBeginDate.ToOrgZoneDateString(null);
					validationResult.Add(obj);
				}
			}
			else
			{
				List<T> list3 = (from f in list
				where Convert.ToDateTime(ModelHelper.GetModelValue<T>(f, dateField)) < ctx.MGLBeginDate
				select f).ToList();
				if (list3.Any())
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Documentsdateislessthantheopeningdate", "单据的日期小于总账启用日期:{0}不能导入");
					foreach (T item2 in list3)
					{
						int rowIndex2 = 0;
						int.TryParse(ModelHelper.TryGetModelValue(item2, "MRowIndex"), out rowIndex2);
						IOValidationResultModel obj2 = new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.BizDate,
							RowIndex = rowIndex2
						};
						string format2 = text2;
						dateTime = ctx.MGLBeginDate;
						obj2.Message = string.Format(format2, dateTime.ToString("yyyy-MM-dd"));
						dateTime = ctx.MGLBeginDate;
						obj2.FieldValue = dateTime.ToString("yyyy-MM");
						validationResult.Add(obj2);
					}
				}
			}
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<string> settledPeriodList = _settle.GetSettledPeriod(ctx);
				List<T> list4 = (from f in list
				where settledPeriodList.Contains(Convert.ToDateTime(ModelHelper.GetModelValue<T>(f, dateField)).ToString("yyyy-MM"))
				select f).ToList();
				if (list4.Any())
				{
					string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PeriodHasSettled", "已经结帐的期间：{0}不能导入！");
					foreach (T item3 in list4)
					{
						int rowIndex3 = 0;
						int.TryParse(ModelHelper.TryGetModelValue(item3, "MRowIndex"), out rowIndex3);
						IOValidationResultModel obj3 = new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.BizDate,
							RowIndex = rowIndex3
						};
						string format3 = text3;
						dateTime = Convert.ToDateTime(ModelHelper.GetModelValue(item3, dateField));
						obj3.Message = string.Format(format3, dateTime.ToString("yyyy-MM"));
						dateTime = Convert.ToDateTime(ModelHelper.GetModelValue(item3, dateField));
						obj3.FieldValue = dateTime.ToString("yyyy-MM");
						validationResult.Add(obj3);
					}
				}
			}
		}

		public MultiLanguageFieldList GetMultiLanguageList(string fieldName, string langName)
		{
			return new MultiLanguageFieldList
			{
				MFieldName = HttpUtility.HtmlDecode(fieldName),
				MMultiLanguageField = new List<MultiLanguageField>
				{
					new MultiLanguageField
					{
						MLocaleID = "0x0009",
						MValue = langName
					},
					new MultiLanguageField
					{
						MLocaleID = "0x7804",
						MValue = langName
					},
					new MultiLanguageField
					{
						MLocaleID = "0x7C04",
						MValue = langName
					}
				}
			};
		}

		public static OperationResult UpdateAccountInitBalanceByBill<T>(MContext ctx, List<T> list, bool isNeedUpdatePlus = false)
		{
			OperationResult result = new OperationResult();
			if (ctx.MRegProgress >= 12 && !ctx.MInitBalanceOver)
			{
				bool flag = false;
				if (typeof(T) == typeof(IVPaymentModel))
				{
					List<IVPaymentModel> source = list as List<IVPaymentModel>;
					flag = source.Any((IVPaymentModel f) => ((IVBaseModel<IVPaymentEntryModel>)f).MBizDate < ctx.MBeginDate);
				}
				else if (typeof(T) == typeof(IVExpenseModel))
				{
					List<IVExpenseModel> source2 = list as List<IVExpenseModel>;
					flag = source2.Any((IVExpenseModel f) => ((IVBaseModel<IVExpenseEntryModel>)f).MBizDate < ctx.MBeginDate && f.MStatus > 3);
				}
			}
			return result;
		}

		public void SetValidationResult<T>(MContext ctx, T result, List<IOValidationResultModel> validationResult, bool isFromApi = false)
		{
			ImportResult importResult = result as ImportResult;
			if (importResult != null)
			{
				importResult.Success = true;
			}
			if (validationResult.Any())
			{
				if (importResult != null)
				{
					importResult.Success = false;
					string format = (validationResult.Count((IOValidationResultModel f) => f.RowIndex == 0) == validationResult.Count()) ? string.Empty : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ErrorRowNo", "Row {0}: ");
					validationResult = (from f in validationResult
					orderby f.RowIndex
					orderby f.FieldType
					select f).ToList();
					foreach (IOValidationResultModel item2 in validationResult)
					{
						importResult.MessageList.Add(string.Format(format, item2.RowIndex) + string.Format(item2.Message, item2.FieldValue));
					}
				}
				else
				{
					OperationResult operationResult = result as OperationResult;
					if (operationResult != null)
					{
						int rowIndex;
						if (isFromApi)
						{
							foreach (IOValidationResultModel item3 in validationResult)
							{
								List<BizVerificationInfor> verificationInfor = operationResult.VerificationInfor;
								BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
								rowIndex = item3.RowIndex;
								bizVerificationInfor.Id = rowIndex.ToString();
								bizVerificationInfor.Level = AlertEnum.Error;
								bizVerificationInfor.Message = item3.Message;
								bizVerificationInfor.RowIndex = item3.RowIndex;
								verificationInfor.Add(bizVerificationInfor);
							}
						}
						else
						{
							IEnumerable<IGrouping<string, IOValidationResultModel>> enumerable = from f in validationResult
							group f by f.Message;
							foreach (IGrouping<string, IOValidationResultModel> item4 in enumerable)
							{
								List<IOValidationResultModel> list = item4.ToList();
								List<string> list2 = new List<string>();
								foreach (IOValidationResultModel item5 in list)
								{
									List<string> list3 = list2;
									object item;
									if (string.IsNullOrWhiteSpace(item5.Id))
									{
										rowIndex = item5.RowIndex;
										item = rowIndex.ToString();
									}
									else
									{
										item = item5.Id;
									}
									list3.Add((string)item);
								}
								string message = item4.Key.Replace("{0}", string.Join("、", (from f in list
								select f.FieldValue).Distinct()));
								operationResult.VerificationInfor.Add(new BizVerificationInfor
								{
									Id = string.Join("、", list2),
									Level = AlertEnum.Error,
									Message = message
								});
							}
						}
						operationResult.Success = (operationResult.VerificationInfor.Count == 0);
					}
				}
			}
		}

		public void SetValidationError<T>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult) where T : BaseModel
		{
			foreach (IOValidationResultModel item in validationResult)
			{
				string message = item.Message;
				int rowIndex = item.RowIndex;
				if (rowIndex < list.Count)
				{
					T val = list[rowIndex];
					List<ValidationError> list2 = val.ValidationErrors ?? new List<ValidationError>();
					list2.Add(new ValidationError(message));
					val.ValidationErrors = list2;
				}
			}
		}

		public static List<T> DealApiErrorMessageList<T>(List<T> originalList, OperationResult result, out List<T> unSuccessList) where T : BaseModel
		{
			unSuccessList = new List<T>();
			List<T> list = new List<T>();
			if (!result.Success)
			{
				List<BizVerificationInfor> verificationInfor = result.VerificationInfor;
				foreach (BizVerificationInfor item in verificationInfor)
				{
					if (item.CheckItem == null || item.CheckItem == "RowIndex")
					{
						int rowIndex = item.RowIndex;
						if (originalList.Count > rowIndex)
						{
							T model = originalList[rowIndex];
							ErrorsAddToModel(model, unSuccessList, item.Message);
						}
						else
						{
							foreach (T original in originalList)
							{
								ErrorsAddToModel(original, unSuccessList, item.Message);
							}
						}
					}
					else if (item.CheckItem == "Account")
					{
						string[] array = item.Id.Split('、');
						PropertyInfo p = typeof(T).GetProperty("MAccountCode");
						if (p != (PropertyInfo)null)
						{
							string[] array2 = array;
							foreach (string id in array2)
							{
								List<T> list2 = (from m in originalList
								where p.GetValue(m).ToString().ToStandardAcctNumber(true, 2)
									.Trim() == id.Trim()
								select m).ToList();
								if (list2.Any())
								{
									foreach (T item2 in list2)
									{
										ErrorsAddToModel(item2, unSuccessList, item.Message);
									}
								}
								else
								{
									foreach (T original2 in originalList)
									{
										ErrorsAddToModel(original2, unSuccessList, item.Message);
									}
								}
							}
						}
						else
						{
							foreach (T original3 in originalList)
							{
								ErrorsAddToModel(original3, unSuccessList, item.Message);
							}
						}
					}
					else
					{
						foreach (T original4 in originalList)
						{
							ErrorsAddToModel(original4, unSuccessList, item.Message);
						}
					}
				}
			}
			if (unSuccessList.Any())
			{
				foreach (T original5 in originalList)
				{
					List<ValidationError> validationErrors = original5.ValidationErrors;
					if (!validationErrors.Any())
					{
						list.Add(original5);
					}
				}
			}
			else
			{
				list = originalList;
			}
			return list;
		}

		private static void ErrorsAddToModel<T>(T model, List<T> unSuccessList, string message) where T : BaseModel
		{
			if (!unSuccessList.Contains(model))
			{
				unSuccessList.Add(model);
			}
			List<ValidationError> list = model.ValidationErrors ?? new List<ValidationError>();
			list.Add(new ValidationError
			{
				Message = message
			});
			model.ValidationErrors = list;
		}

		protected static void ErrorsAddToModel<T>(T model, string message) where T : BaseModel
		{
			List<ValidationError> list = model.ValidationErrors ?? new List<ValidationError>();
			list.Add(new ValidationError
			{
				Message = message
			});
			model.ValidationErrors = list;
		}

		protected void SetWhereString(GetParam param, string filedName, List<string> valueList, bool isAnd = true)
		{
			if (!string.IsNullOrEmpty(param.WhereString))
			{
				param.WhereString = string.Format("{0} {1} ", param.WhereString, isAnd ? "&&" : "||");
			}
			if (valueList.Count > 1)
			{
				param.WhereString = $"{param.WhereString} ( ";
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string value in valueList)
			{
				if (num == 0)
				{
					stringBuilder.AppendFormat("{0}==\"{1}\"", filedName, value);
				}
				else
				{
					stringBuilder.AppendFormat(" || {0}==\"{1}\"", filedName, value);
				}
				num++;
			}
			param.WhereString = $"{param.WhereString} {stringBuilder.ToString()}";
			if (valueList.Count > 1)
			{
				param.WhereString = $"{param.WhereString} ) ";
			}
		}

		protected void SetWhereString(GetParam param, string filedName, string queryName)
		{
			if (param.QueryString != null)
			{
				string text = param.QueryString[queryName];
				if (!string.IsNullOrEmpty(text))
				{
					string[] source = text.Split(',');
					SetWhereString(param, filedName, source.ToList(), true);
				}
			}
		}

		public bool IsMatchMultRecord<T>(MContext ctx, List<T> dbDataList, List<MultiLanguageFieldList> multLangFieldList, string dbFieldName, string mathFieldName) where T : BaseModel
		{
			if (multLangFieldList == null || multLangFieldList.Count == 0)
			{
				return false;
			}
			List<MultiLanguageField> multiLanguageField = multLangFieldList.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == mathFieldName)?.MMultiLanguageField;
			if (multiLanguageField == null || multiLanguageField.Count == 0)
			{
				return false;
			}
			int num = 0;
			foreach (T dbData in dbDataList)
			{
				List<MultiLanguageField> list = dbData.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == dbFieldName)?.MMultiLanguageField;

				if ((from langModel in list
				select multiLanguageField.Exists((Predicate<MultiLanguageField>)((MultiLanguageField a) => a.MLocaleID == langModel.MLocaleID && a.MValue.EqualsIgnoreCase(langModel.MValue)))).Any((bool isExists) => isExists))
				{
					num++;
				}
				if (num > 1)
				{
					return true;
				}
			}
			return false;
		}

		public T GetMatchItem<T>(MContext ctx, List<T> dbDataList, string postName, string fieldName = "MName")
			where T: new()
		{
			T result = new T();
			if (dbDataList == null || string.IsNullOrWhiteSpace(postName))
			{
				return result;
			}
			List<APILangModel> source = new List<APILangModel>();
			postName = postName.Trim().ToUpper();
			foreach (T dbData in dbDataList)
			{
				string text = ModelHelper.GetModelValue(dbData, fieldName).Trim().ToUpper();
				if (Regex.IsMatch(text, "^[{.+[LCID]+.+}]$"))
				{
					source = JsonConvert.DeserializeObject<List<APILangModel>>(text);
				}
				if (text == postName || source.Any((APILangModel f) => f.LCID == ctx.MLCID.ToUpper() && f.Value == postName))
				{
					result = dbData;
					break;
				}
			}
			return result;
		}

		public T MultLanguageMatchRecord<T>(MContext ctx, List<T> dbDataList, List<MultiLanguageFieldList> multLangFieldList, string dbFieldName, string mathFieldName) where T : BaseModel,new()
		{
			T result = new T();
			if (multLangFieldList == null || multLangFieldList.Count == 0)
			{
				return result;
			}
			List<MultiLanguageField> multiLanguageField = multLangFieldList.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == mathFieldName)?.MMultiLanguageField;
			if (multiLanguageField == null || multiLanguageField.Count == 0)
			{
				return result;
			}
			foreach (T dbData in dbDataList)
			{
				List<MultiLanguageField> list = dbData.MultiLanguage.FirstOrDefault((MultiLanguageFieldList a) => a.MFieldName == dbFieldName)?.MMultiLanguageField;
				if ((from langModel in list
				select multiLanguageField.Exists((Predicate<MultiLanguageField>)((MultiLanguageField a) => a.MLocaleID == langModel.MLocaleID && a.MValue.EqualsIgnoreCase(langModel.MValue)))).Any((bool isExists) => isExists))
				{
					return dbData;
				}
			}
			return result;
		}

		public void AddValidationError<T>(MContext ctx, T model, string fieldName, string value) where T : BaseModel
		{
			string textFormat = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.Common, "MatchedMultiRecord", "多语言字段“{0}”的传值“{1}”匹配到了两个系统记录，请检查。", fieldName, value);
			model.ValidationErrors.Add(new ValidationError
			{
				Message = textFormat
			});
		}
	}
}
