using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.AutoManager;
using JieNor.Megi.Common.ImportAndExport.Utility.Import;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.DataModel.IO.Import.PA;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.FA;
using JieNor.Megi.ServiceContract.GL;
using JieNor.Megi.ServiceContract.IV;
using JieNor.Megi.ServiceContract.PA;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class ImportController : GoControllerBase
	{
		private IIVInvoice _iInvoice = null;

		private IIVReceive _iReceive = null;

		private IIVPayment _iPayment = null;

		private IBDAccount _iAccount = null;

		private IBDBankAccount _bankAccount = null;

		private IGLVoucher _iVoucher = null;

		private IBDItem _item = null;

		private IBDExpenseItem _expItem = null;

		private IBDContacts _contact = null;

		private IBDEmployees _employee = null;

		private IPASalaryPayment _salaryPayment = null;

		private IGLInitBalance _glInitBalance = null;

		private IFAFixAssets _faFixAsset = null;

		private Dictionary<string, string> DicObjName
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Invoice_Sale", LangHelper.GetText(LangModule.IV, "SalesInvoice", "Sales Invoice"));
				dictionary.Add("Invoice_Sale_Red", LangHelper.GetText(LangModule.IV, "SalesCreditNote", "Sales Credit Note"));
				dictionary.Add("Invoice_Purchase", LangHelper.GetText(LangModule.IV, "Bills", "Bills"));
				dictionary.Add("Invoice_Purchase_Red", LangHelper.GetText(LangModule.IV, "PurchaseCreditNote", "Purchase Credit Note"));
				dictionary.Add("Receive_Sale", LangHelper.GetText(LangModule.Bank, "ReceiveMoney", "Receive Money"));
				dictionary.Add("Pay_Purchase", LangHelper.GetText(LangModule.Bank, "SpendMoney", "Spend Money"));
				dictionary.Add("Item", LangHelper.GetText(LangModule.BD, "InventoryItems", "Inventory Items"));
				dictionary.Add("Contact", LangHelper.GetText(LangModule.Contact, "Contacts", "Contacts"));
				dictionary.Add("Employees", LangHelper.GetText(LangModule.BD, "Employees", "Employees"));
				dictionary.Add("Account", LangHelper.GetText(LangModule.BD, "AccountItem", "Account"));
				dictionary.Add("AccountCover", LangHelper.GetText(LangModule.Common, "AccountCover", "Account(cover the original account)"));
				dictionary.Add("Voucher", LangHelper.GetText(LangModule.GL, "Voucher", "Voucher"));
				dictionary.Add("PayRun", LangHelper.GetText(LangModule.PA, "SalaryList", "Salary List"));
				dictionary.Add("OpeningBalance", LangHelper.GetText(LangModule.BD, "AccountBalancesFinancial", "Account Balances"));
				dictionary.Add("ExpenseItem", LangHelper.GetText(LangModule.BD, "ExpenseItems", "Expense Items"));
				dictionary.Add("Fixed_Assets", LangHelper.GetText(LangModule.FA, "FixedAsset", "固定资产卡片"));
				return dictionary;
			}
		}

		public ImportController(IIVInvoice iInvoice, IIVReceive iReceive, IIVPayment iPayment, IBDItem item, IBDContacts contact, IBDEmployees employee, IBDAccount iAccount, IGLVoucher iVoucher, IBDBankAccount bankAccount, IPASalaryPayment salaryPayment, IGLInitBalance glInitBalance, IBDExpenseItem expItem, IFAFixAssets faFixAsset)
		{
			_iInvoice = iInvoice;
			_iReceive = iReceive;
			_iPayment = iPayment;
			_item = item;
			_contact = contact;
			_employee = employee;
			_iAccount = iAccount;
			_iVoucher = iVoucher;
			_bankAccount = bankAccount;
			_glInitBalance = glInitBalance;
			_salaryPayment = salaryPayment;
			_expItem = expItem;
			_faFixAsset = faFixAsset;
		}

		public ActionResult Import(string id, string accountId, string contactType, bool isCover = false)
		{
			SetImportInfo(id);
			ViewBag.AccountId = accountId;
			ViewBag.ContactType = contactType;

			string empty = string.Empty;
			int num;
			switch (id)
			{
			default:
				num = ((id == "ExpenseItem") ? 1 : 0);
				break;
			case "Item":
			case "Contact":
			case "Account":
			case "OpeningBalance":
				num = 1;
				break;
			}
			empty = ((num == 0) ? HtmlLang.GetText(LangModule.IV, "ImportInvoiceStep2Warning1", "IMPORTANT:Do not change the column headings provided in the Hypercurrent template. These need to be unchanged for the import to work in the next step.<br/>Dates are assumed to be in Invariant Language (Invariant Country) format. For example, 12/25/2015 or 25 Dec 2015.") : HtmlLang.GetText(LangModule.Contact, "ImportContactsStep2Warning1", "IMPORTANT: Do not change the column headings in the template file. These need to be unchanged for the import to work in the next step."));
			ViewBag.ImportStep2Warning = empty;
			ViewBag.IsCover = isCover;

			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			string text = HtmlLang.GetText(LangModule.PA, "SalaryTmplVersion", "{0}版");
			PAPITThresholdFilterModel filter = new PAPITThresholdFilterModel
			{
				IsDefault = true
			};
			List<PAPITThresholdModel> resultData = _salaryPayment.GetPITThresholdList(filter, null).ResultData;
			foreach (PAPITThresholdModel item in resultData)
			{
				List<KeyValuePair<string, string>> list2 = list;
				DateTime mEffectiveDate = item.MEffectiveDate;
				string key = mEffectiveDate.ToString("yyyy-MM");
				string format = text;
				mEffectiveDate = item.MEffectiveDate;
				list2.Add(new KeyValuePair<string, string>(key, string.Format(format, mEffectiveDate.ToString("yyyy"))));
			}
			ViewBag.SalaryTmplList = list;

			return base.View();
		}

		public JsonResult GetImportAccountMatchResult(string type, string fileName)
		{
			string fullPath = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID, fileName);
			Stream fileStream = FileHelper.GetFileStream(fullPath);
			NPOIHelper nPOIHelperInstance = GetNPOIHelperInstance(type, false);
			List<BDAccountEditModel> acctList = nPOIHelperInstance.ImportData<BDAccountEditModel>(fileStream, type);
			IOAccountMatchModel resultData = _iAccount.GetImportAccountMatchResult(acctList, null).ResultData;
			return base.Json(resultData.ManualMatchList);
		}

		public ActionResult ConfirmImport(string id, string fileName, string importCnt, string accountId, string contactType, bool isCover = false)
		{
			SetImportInfo(id);
			ViewBag.IsCover = isCover;

			string empty = string.Empty;
			string empty2 = string.Empty;
			string arg = string.Empty;
			bool flag = id == "Item" || id == "Contact" || id == "Employees";
			if ((id == "Receive_Sale" || id == "Pay_Purchase" || id == "Account") | flag)
			{
				var text = HtmlLang.GetText(LangModule.IV, "ConfirmImportCount", "<ul><li><strong>{0}</strong> new {1} will be imported</li></ul>");
				empty = ReplaceLabelUnit(string.Format(text, importCnt, ViewBag.TypeName), id);
				var text2 = HtmlLang.GetText(LangModule.IV, "confirmImportState", "{0} will be imported can be edited or deleted afterwards");

				empty2 = string.Format(text2, ViewBag.TypeNames);
				
			}
			else if (id == "Voucher")
			{
				var text3 = HtmlLang.GetText(LangModule.IV, "ConfirmImportSavedCount", "<ul><li><strong>{0}</strong> 张 {1} 单据将按已保存状态导入</li></ul>");
				empty = string.Format(text3, importCnt, ViewBag.TypeName);
				var text4 = HtmlLang.GetText(LangModule.IV, "ConfirmImportAsSavedState", "{0}将按已保存状态导入，可在完成导入后对其进行编辑或删除。");

				empty2 = string.Format(text4, ViewBag.TypeNames);
			}
			else
			{
				var text5 = HtmlLang.GetText(LangModule.IV, "ConfirmImportDraftCount", "<ul><li><strong>{0}</strong> new draft {1} will be imported</li></ul>");
				empty = string.Format(text5, importCnt, ViewBag.TypeName);
				var text6 = HtmlLang.GetText(LangModule.IV, "ConfirmImportAsDraftState", "{0} will be imported as drafts and can be edited or deleted afterwards");
				empty2 = string.Format(text6, ViewBag.TypeNames);
			}
			if (id == "Contact")
			{
				arg = HtmlLang.GetText(LangModule.BD, "ConfirmImportNotice", "重要提示：除了联系人名称和类型外，其它导入模版上的所有字段无论是否为空，都会被全部覆盖。");
			}

			ViewBag.IsContact = id == "Contact";
			ViewBag.AccountId = accountId;
			ViewBag.ContactType = contactType;
			ViewBag.ImportingCount = importCnt;
			ViewBag.OriFileName = FileHelper.GetOriginalFileName(fileName);
			ViewBag.NewFileName = fileName;
			ViewBag.ConfirmImpCnt = empty;
			ViewBag.ConfirmImpState = empty2;
			ViewBag.ConfirmImportNotice = arg;

			var text7 = HtmlLang.GetText(LangModule.IV, "confirmImportNotify", "<p>The imported <strong>{0}</strong> contained {1} {2}, the following will be imported into Hypercurrent:</p>");
			var text8 = ReplaceLabelUnit(text7, id);
			ViewBag.ConfirmImportNotify = string.Format(text8, ViewBag.OriFileName, ViewBag.ImportingCount, ViewBag.TypeName);

			return View();
		}

		public JsonResult CompleteImport(string type, string fileName, string accountId, string contactType, bool isCover = false)
		{
			string fullPath = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID, fileName);
			Stream fileStream = FileHelper.GetFileStream(fullPath);
			NPOIHelper nPOIHelperInstance = GetNPOIHelperInstance(type, isCover);
			dynamic obj = null;
			dynamic obj2 = null;
			string importTemplateType = GetImportTemplateType(type);
			switch (type)
			{
				case "Invoice_Sale":
				case "Invoice_Sale_Red":
				case "Invoice_Purchase":
				case "Invoice_Purchase_Red":
					obj2 = nPOIHelperInstance.ImportData<IVInvoiceModel, IVInvoiceEntryModel>(fileStream, importTemplateType);
					foreach (var item in obj2)
					{
						item.MOrgID = MContext.MOrgID;
						item.MType = type;
					}
					obj = _iInvoice.ImportInvoiceList(obj2);
					break;
				case "Receive_Sale":
					obj2 = nPOIHelperInstance.ImportData<IVReceiveModel, IVReceiveEntryModel>(fileStream, importTemplateType);
					foreach (var item2 in obj2)
					{
						item2.MOrgID = MContext.MOrgID;
						item2.MBankID = accountId;
					}
					obj = _iReceive.ImportReceiveList(obj2);
					break;
				case "Pay_Purchase":
					obj2 = nPOIHelperInstance.ImportData<IVPaymentModel, IVPaymentEntryModel>(fileStream, importTemplateType);
					foreach (var item3 in obj2)
					{
						item3.MOrgID = MContext.MOrgID;
						item3.MBankID = accountId;
					}
					obj = _iPayment.ImportPaymentList(obj2);
					break;
				case "Item":
					obj2 = nPOIHelperInstance.ImportData<BDItemModel>(fileStream, importTemplateType);
					foreach (var item4 in obj2)
					{
						item4.MOrgID = MContext.MOrgID;
					}
					obj = _item.ImportItemList(obj2);
					break;
				case "ExpenseItem":
					obj2 = nPOIHelperInstance.ImportData<BDExpenseItemModel>(fileStream, importTemplateType);
					foreach (var item5 in obj2)
					{
						item5.MOrgID = MContext.MOrgID;
					}
					obj = _expItem.ImportExpenseItemsList(obj2);
					break;
				case "Contact":
					obj2 = nPOIHelperInstance.ImportData<BDContactsInfoModel>(fileStream, importTemplateType);
					foreach (var item6 in obj2)
					{
						item6.MOrgID = MContext.MOrgID;
					}
					obj = _contact.ImportContactList(obj2);
					break;
				case "Employees":
					obj2 = nPOIHelperInstance.ImportData<BDEmployeesModel>(fileStream, importTemplateType);
					foreach (var item7 in obj2)
					{
						item7.MOrgID = MContext.MOrgID;
					}
					obj = _employee.ImportEmployeeList(obj2);
					break;
				case "Account":
					{
						string mAccountTableID = ContextHelper.MContext.MAccountTableID;
						if (mAccountTableID == "3" & isCover)
						{
							obj2 = nPOIHelperInstance.ImportDataReadOnce<BDAccountEditModel>(fileStream, isCover);
							obj = _iAccount.ImportCustomAccountList(obj2);
						}
						else
						{
							obj2 = nPOIHelperInstance.ImportData<BDAccountEditModel>(fileStream, importTemplateType, out fileStream);
							obj = _iAccount.ImportAccountList(obj2);
						}
						break;
					}
				case "OpeningBalance":
					{
						List<GLInitBalanceModel> list = nPOIHelperInstance.ImportData<GLInitBalanceModel>(fileStream, importTemplateType, out fileStream);
						obj = _glInitBalance.ImportInitBalanceList(list, null);
						break;
					}
				case "Voucher":
					obj2 = nPOIHelperInstance.ImportData<GLVoucherModel, GLVoucherEntryModel>(fileStream, importTemplateType);
					obj = _iVoucher.ImportVoucherList(obj2);
					break;
				case "PayRun":
					{
						DateTime period = ConvertPeriodToDate(string.Empty);
						ImportSalaryModel salaryInfo = _salaryPayment.GetImportTemplateModel(period, null).ResultData.SalaryInfo;
						salaryInfo.ImportStep = 2;
						ImportSalaryModel model = new ImportSalaryHelper().ReadImportData(fileStream, salaryInfo);
						obj = _salaryPayment.ImportSalaryList(model, null);
						break;
					}
				case "Fixed_Assets":
					{
						List<FAFixAssetsModel> assetCardList = nPOIHelperInstance.ImportData<FAFixAssetsModel>(fileStream, importTemplateType, out fileStream);
						obj = _faFixAsset.ImportAssetCardList(assetCardList, null);
						break;
					}
				default:
					obj = new OperationResult();
					break;
			}

			return Json(obj);
		}

		public FileStreamResult DownloadImportTemplate(string id, string accountId, string contactType, string period)
		{
			ImportTemplateModel importTemplateModel = null;
			List<ImportTemplateModel> modelList = null;
			IVImportTransactionFilterModel param = new IVImportTransactionFilterModel
			{
				ContactType = contactType,
				AccountId = accountId,
				BizType = id
			};
			switch (id)
			{
			case "Invoice_Sale":
			case "Invoice_Sale_Red":
			case "Invoice_Purchase":
			case "Invoice_Purchase_Red":
				importTemplateModel = _iInvoice.GetImportTemplateModel(id, null).ResultData;
				break;
			case "Receive_Sale":
				importTemplateModel = _iReceive.GetImportTemplateModel(param, null).ResultData;
				break;
			case "Pay_Purchase":
				importTemplateModel = _iReceive.GetImportTemplateModel(param, null).ResultData;
				break;
			case "Item":
				importTemplateModel = _item.GetImportTemplateModel(null).ResultData;
				break;
			case "ExpenseItem":
				importTemplateModel = _expItem.GetImportTemplateModel(null).ResultData;
				break;
			case "Contact":
				importTemplateModel = _contact.GetImportTemplateModel(null).ResultData;
				break;
			case "Employees":
				importTemplateModel = _employee.GetImportTemplateModel(null).ResultData;
				break;
			case "Account":
			{
				IOImportAccountFilterModel iOImportAccountFilterModel2 = new IOImportAccountFilterModel();
				if (base.HttpContext.Request.QueryString.AllKeys.Contains("isCover"))
				{
					iOImportAccountFilterModel2.IsCover = Convert.ToBoolean(base.HttpContext.Request.QueryString["isCover"]);
					if (iOImportAccountFilterModel2.IsCover)
					{
						id = "AccountCover";
					}
				}
				importTemplateModel = _iAccount.GetImportTemplateModel(iOImportAccountFilterModel2, null).ResultData;
				break;
			}
			case "OpeningBalance":
			{
				IOImportAccountFilterModel iOImportAccountFilterModel = new IOImportAccountFilterModel();
				iOImportAccountFilterModel.Type = 1;
				modelList = _glInitBalance.GetImportTemplateModel(null).ResultData;
				break;
			}
			case "Voucher":
				importTemplateModel = _iVoucher.GetImportTemplateModel(false, null).ResultData;
				break;
			case "PayRun":
			{
				DateTime period2 = ConvertPeriodToDate(period);
				importTemplateModel = _salaryPayment.GetImportTemplateModel(period2, null).ResultData;
				string arg = string.Format(HtmlLang.GetText(LangModule.PA, "SalaryTmplVersion", "{0}版"), period2.ToString("yyyy"));
				importTemplateModel.TemplateName = string.Format(HtmlLang.GetText(LangModule.PA, "SalaryImportTemplate", "Salary Import Template"), arg) + ".xls";
				string fullPath = string.Join("\\", base.Server.MapPath("~/Template/PA"), importTemplateModel.TemplateName);
				importTemplateModel.TemplateStream = FileHelper.GetFileStream(fullPath);
				break;
			}
			case "Fixed_Assets":
				importTemplateModel = _faFixAsset.GetImportTemplateModel(null).ResultData;
				break;
			}
			if (id == "OpeningBalance")
			{
				List<GLInitBalanceModel> completeOpeningBalanceList = AccountManager.GetCompleteOpeningBalanceList(false);
				return base.GetImportTemplateFile(modelList, GetObjName(id), completeOpeningBalanceList);
			}
			return base.GetImportTemplateFile(importTemplateModel, GetObjName(id));
		}

		[HttpPost]
		public JsonResult ReadStatementData(string bankId)
		{
			dynamic obj = new OperationResult();
			BDBankAccountEditModel resultData = _bankAccount.GetBDBankAccountEditModel(bankId, null).ResultData;
			if (string.IsNullOrEmpty(resultData.MItemID) || !resultData.MIsNeedReconcile)
			{
				obj.Success = false;
				obj.Message = HtmlLang.GetText(LangModule.Bank, "NoReconcileNoImport", "The account can't be reconcile that cant import a statement.");

				UploadResult = JsonConvert.SerializeObject(obj);
				return Json(obj);
			}
			string fileName = string.Empty;
			HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
			if (httpPostedFile != null && httpPostedFile.ContentLength != 0)
			{
				try
				{
					FileHelper.ValidateFile(httpPostedFile, FileType.ExcelIncludeCSV, FileValidateType.FileType);
					fileName = FileHelper.UploadFile(httpPostedFile, string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID));
				}
				catch (Exception ex)
				{
					string fileExceptionMessage = base.GetFileExceptionMessage(fileName, ex);
					obj = new
					{
						Success = false,
						Message = fileExceptionMessage
					};
					UploadResult = JsonConvert.SerializeObject(obj);
					return GetJsonResult(obj);
				}
			}

			obj = new
			{
				Success = true,
				FileName = fileName
			};
			UploadResult = JsonConvert.SerializeObject(obj);
			return GetJsonResult(obj);
		}

		private string GetImportTemplateType(string type)
		{
			string result = string.Empty;
			switch (type)
			{
			case "Invoice_Sale":
			case "Invoice_Sale_Red":
			case "Invoice_Purchase":
			case "Invoice_Purchase_Red":
			case "OpeningBalance":
			case "Account":
			case "Receive_Sale":
			case "Pay_Purchase":
			case "Item":
			case "Contact":
			case "Voucher":
				result = type;
				break;
			case "Employees":
				result = "Employee";
				break;
			}
			return result;
		}

		[HttpPost]
		public JsonResult VerifyImportData(string type, bool isCover = false)
		{
			HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
			dynamic arg = new OperationResult();
			int num = 0;
			if (httpPostedFile != null && httpPostedFile.ContentLength != 0)
			{
				string fileName = Path.GetFileName(httpPostedFile.FileName);
				string text = string.Empty;
				try
				{
					FileHelper.ValidateFile(httpPostedFile, FileType.ExcelExcludeCSV, FileValidateType.FileType);
					Stream stream = null;
					NPOIHelper nPOIHelperInstance = GetNPOIHelperInstance(type, isCover);
					text = FileHelper.UploadFile(httpPostedFile, string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID));
					string importTemplateType = GetImportTemplateType(type);
					switch (type)
					{
					case "Invoice_Sale":
					case "Invoice_Sale_Red":
					case "Invoice_Purchase":
					case "Invoice_Purchase_Red":
					{
						List<IVInvoiceModel> source5 = nPOIHelperInstance.ImportData<IVInvoiceModel, IVInvoiceEntryModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source5.Count();
						break;
					}
					case "Receive_Sale":
					{
						List<IVReceiveModel> source4 = nPOIHelperInstance.ImportData<IVReceiveModel, IVReceiveEntryModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source4.Count();
						break;
					}
					case "Pay_Purchase":
					{
						List<IVPaymentModel> source7 = nPOIHelperInstance.ImportData<IVPaymentModel, IVPaymentEntryModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source7.Count();
						break;
					}
					case "Item":
							{
								List<BDItemModel> list3 = nPOIHelperInstance.ImportData<BDItemModel>(httpPostedFile.InputStream, importTemplateType, out stream);
								if (list3.Any())
								{
									arg = _item.IsImportItemsCodeExist(list3, null).ResultData;

									if (!arg.Success)
									{
										UploadResult = JsonConvert.SerializeObject(arg);

										return GetJsonResult(arg);
									}
								}
								num = list3.Count();
								break;
							}
					case "ExpenseItem":
					{
						List<BDExpenseItemModel> source6 = nPOIHelperInstance.ImportData<BDExpenseItemModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source6.Count();
						break;
					}
					case "Contact":
							{
								List<BDContactsInfoModel> list2 = nPOIHelperInstance.ImportData<BDContactsInfoModel>(httpPostedFile.InputStream, importTemplateType, out stream);
								num = list2.Count();
								if (!list2.Any())
								{
									break;
								}
								arg = _contact.IsImportContactHaveSameName(list2, null).ResultData;
								// TODO:待确认,是否if(arg.Success)
								if (!arg.Success)
								//if (!target(_003C_003Ep__, target2(_003C_003Ep__2, _003C_003Eo__21._003C_003Ep__7.Target(_003C_003Eo__21._003C_003Ep__7, arg))))
								{
									break;
								}

								object success = arg.Success;
								var anon = new
								{
									Success = success,
									Message = arg.Message,
									ImportCnt = num,
									FileName = text
								};
								UploadResult = JsonConvert.SerializeObject(anon);
								return GetJsonResult(anon);
							}
					case "Employees":
					{
						List<BDEmployeesModel> list4 = nPOIHelperInstance.ImportData<BDEmployeesModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						if (list4.Any())
						{
							arg = _employee.IsImportEmployeeNamesExist(list4, null).ResultData;
									if (!arg.Success)
									{
										if (!string.IsNullOrWhiteSpace(arg.Message))
										{
											arg.Message = MText.Encode(arg.Message);
										}

										UploadResult = JsonConvert.SerializeObject(arg);
										return GetJsonResult(arg);
									}
						}
						num = list4.Count();
						break;
					}
					case "Account":
					{
						string mAccountTableID = ContextHelper.MContext.MAccountTableID;
						List<BDAccountEditModel> list = new List<BDAccountEditModel>();
						if (mAccountTableID == "3" && isCover)
						{
							if (ContextHelper.MContext.MInitBalanceOver)
							{
								throw new Exception(HtmlLang.GetText(LangModule.Common, "NotAllowImportBecauseInitBalanceOver", "科目期初余额已完成了初始化，不允许在进行科目覆盖导入！"));
							}
							List<GLInitBalanceModel> resultData = _glInitBalance.GetModelList(new SqlWhere(), false, null).ResultData;
							if (resultData != null && resultData.Count() > 0)
							{
								throw new Exception(HtmlLang.GetText(LangModule.Common, "NotAllowImportBecauseHaveInitBalance", "已存在科目期初余额，不允许在进行科目覆盖导入！"));
							}
							list = nPOIHelperInstance.ImportDataReadOnce<BDAccountEditModel>(httpPostedFile.InputStream, true);
						}
						else
						{
							list = nPOIHelperInstance.ImportData<BDAccountEditModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						}
						num = list.Count();
						break;
					}
					case "OpeningBalance":
					{
						List<GLInitBalanceModel> source3 = nPOIHelperInstance.ImportData<GLInitBalanceModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source3.Count();
						break;
					}
					case "Voucher":
					{
						List<GLVoucherModel> source2 = nPOIHelperInstance.ImportData<GLVoucherModel, GLVoucherEntryModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source2.Count();
						break;
					}
					case "PayRun":
					{
						DateTime period = ConvertPeriodToDate(string.Empty);
						ImportSalaryModel salaryInfo = _salaryPayment.GetImportTemplateModel(period, null).ResultData.SalaryInfo;
						salaryInfo.ImportStep = 1;
						List<ImportSalaryListModel> salaryList = new ImportSalaryHelper().ReadImportData(httpPostedFile.InputStream, salaryInfo).SalaryList;
						num = salaryList.Count();
						break;
					}
					case "Fixed_Assets":
					{
						List<FAFixAssetsModel> source = nPOIHelperInstance.ImportData<FAFixAssetsModel>(httpPostedFile.InputStream, importTemplateType, out stream);
						num = source.Count();
						break;
					}
					}
					if (stream != null)
					{
						string uploadPath = FileHelper.GetUploadPath("~/App_Data/Temp/ImportFailed");
						string oldValue = uploadPath;
						FileHelper.UploadFileStream(stream, ref uploadPath, text, true);
						string text2 = HtmlLang.GetText(LangModule.Common, "ImportFialTips", "导入数据失败，是否下载文件查看详细错误信息？");
						string arg3 = uploadPath.Replace(oldValue, "");
						string fileUrl = $"/BD/Import/DownloadImportFailedData?fileName={arg3}\\{HttpUtility.UrlEncode(text)}";
						arg = new
						{
							Success = false,
							Message = text2,
							FileUrl = fileUrl
						};

						UploadResult = JsonConvert.SerializeObject(arg);
						return GetJsonResult(arg);
					}
					if (num == 0)
					{
						throw new Exception(HtmlLang.GetText(LangModule.Common, "NoDataFoundInImportingFile", "No data found in your importing file!"));
					}
					arg = new
					{
						Success = true,
						FileName = text,
						ImportCnt = num
					};

					UploadResult = JsonConvert.SerializeObject(arg);
					return GetJsonResult(arg);
				}
				catch (Exception ex)
				{
					string fileExceptionMessage = base.GetFileExceptionMessage(fileName, ex);
					arg = new
					{
						Success = false,
						Message = HttpUtility.HtmlEncode(fileExceptionMessage),
						FileName = text
					};

					UploadResult = JsonConvert.SerializeObject(arg);
					return GetJsonResult(arg);
				}
			}

			UploadResult = JsonConvert.SerializeObject(arg);
			return GetJsonResult(arg);
		}

		private NPOIHelper GetNPOIHelperInstance(string type, bool isCover = false)
		{
			int num = 2;
			bool haveCommentRow = false;
			int entryDataStartColumnIndex = NPOIHelper.GetEntryDataStartColumnIndex(type, false);
			if (entryDataStartColumnIndex > -1)
			{
				num++;
			}
			if (type == "OpeningBalance" || (type == "Account" && !isCover) || type == "ExpenseItem" || type == "Fixed_Assets" || type == "Voucher")
			{
				haveCommentRow = true;
				num++;
			}
			if (!(type == "Account"))
			{
				if (type == "OpeningBalance")
				{
					return new OpenBalanceNPOIHelper(num, haveCommentRow, entryDataStartColumnIndex);
				}
				return new NPOIHelper(num, haveCommentRow, entryDataStartColumnIndex);
			}
			return new AccountNPOIHelper(num, haveCommentRow, entryDataStartColumnIndex);
		}

		private void SetImportInfo(string id)
		{
			ViewBag.Type = id;

			string objName = GetObjName(id);
			ViewBag.TypeNames = objName;
			ViewBag.TypeName = objName.TrimEnd('s').ToLower();
		}

		private string GetObjName(string type)
		{
			return DicObjName.ContainsKey(type) ? DicObjName[type] : string.Empty;
		}

		private string ReplaceLabelUnit(string label, string type)
		{
			if (type == "Item" || type == "Contact" || type == "Employees" || type == "Account")
			{
				label = Regex.Replace(label, "[张|張]", HtmlLang.GetText(LangModule.Common, "UnitGe", "") ?? string.Empty);
				label = Regex.Replace(label, "[单据|單據]", "");
			}
			else if (type == "OpeningBalance")
			{
				label = Regex.Replace(label, "[张|張]", HtmlLang.GetText(LangModule.Common, "UnitStrip", "条") ?? string.Empty);
				label = Regex.Replace(label, "[单据|單據]", "");
			}
			return label;
		}

		private DateTime ConvertPeriodToDate(string period)
		{
			if (string.IsNullOrWhiteSpace(period))
			{
				return DateTime.MinValue;
			}
			char c = Regex.Match(period, "\\D").Value.ToCharArray()[0];
			string[] array = period.Split(c);
			return new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), 1);
		}
	}
}
