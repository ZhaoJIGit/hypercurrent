using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Go.HtmlHelper;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.IO;
using JieNor.Megi.ServiceContract.IV;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IO.Controllers
{
	public class ImportBySolutionController : GoControllerBase
	{
		private IIVTransactions _transaction;

		private IIOSolution _solution;

		private IBDBankAccount _bankAccount;

		private List<KeyValuePair<int, string>> ImportTypeList
		{
			get
			{
				List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
				if (base.MContext.MEnabledModules.Contains(ModuleEnum.GL) && HtmlSECMenu.HavePermission("General_Ledger", "Change", ""))
				{
					list.Add(new KeyValuePair<int, string>(1, LangHelper.GetText(LangModule.GL, "Voucher", "Voucher")));
				}
				return list;
			}
		}

		private Dictionary<int, string> DicObjName
		{
			get
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				dictionary.Add(7, LangHelper.GetText(LangModule.FP, "PurchaseFapiao", "进项发票"));
				dictionary.Add(8, LangHelper.GetText(LangModule.FP, "SaleFapiao", "销项发票"));
				return dictionary;
			}
		}

		public ImportBySolutionController(IIVTransactions transaction, IIOSolution solution, IBDBankAccount bankAccount)
		{
			_transaction = transaction;
			_solution = solution;
			_bankAccount = bankAccount;
		}

		public ActionResult ImportFPType(int type)
		{
			ViewBag.Type = type;
			return base.View();
		}

		public ActionResult ImportFaPiao(int inOrOuttype)
		{
			ViewBag.InOrOuttype = inOrOuttype;
			return base.View();
		}

		public ActionResult Import(int type)
		{
			ViewBag.ImportTypeList = JsonConvert.SerializeObject(ImportTypeList);

			return base.View();
		}

		public ActionResult ImportOptions(string id, int type, string fileName, int inOrOuttype = 0)
		{
			DataTable dataTable = null;
			string url = $"/IO/ImportBySolution/Import?id={id}&type={type}";
			if (type == 7 || type == 8)
			{
				url = $"/IO/ImportBySolution/ImportFaPiao?inOrOuttype={inOrOuttype}";
			}
			int num;
			try
			{
				dataTable = GetImportingData(fileName);
				int num2;
				if (dataTable != null && dataTable.Rows.Count != 0)
				{
					num = 999999;
					num2 = ((id == num.ToString() && dataTable.Rows.Count < 7) ? 1 : 0);
				}
				else
				{
					num2 = 1;
				}
				if (num2 != 0)
				{
					throw new Exception(HtmlLang.GetText(LangModule.Common, "NoDataFoundInImportingFile", "No data found in your importing file!"));
				}
			}
			catch (Exception ex)
			{
				if (type == 7 || type == 8)
				{
					return base.RedirectToAction("ImportFaPiao", new
					{
						inOrOuttype = inOrOuttype,
						message = ex.Message
					});
				}
				return base.RedirectToAction("Import", new
				{
					id = id,
					type = type,
					message = ex.Message
				});
			}
			if (string.IsNullOrWhiteSpace(id))
			{
				base.Response.Redirect(url);
			}
			ViewBag.FileName = fileName;


			IOSolutionModel resultData = _solution.GetSolutionModel(id, null).ResultData;
			ViewBag.JsonSolution = JsonConvert.SerializeObject(resultData);

			DataTable dataTable2 = new DataTable();
			DataTable dataTable3 = new DataTable();
			num = 999999;
			if (id == num.ToString())
			{
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (dataTable.Rows[i][0].ToString().Contains("发票类别") && dataTable.Rows[i][0].ToString().Contains("专用发票"))
					{
						num3 = i + 1;
					}
					else if (dataTable.Rows[i][0].ToString().Contains("发票类别") && dataTable.Rows[i][0].ToString().Contains("普通发票"))
					{
						num4 = i + 1;
					}
					else if (num3 > 0 || num4 > 0)
					{
						if (i >= num3 && dataTable2.Rows.Count == 0 && num3 != 0)
						{
							dataTable2 = dataTable.AsEnumerable().Skip(i).Take(20)
								.CopyToDataTable();
						}
						if (i >= num4 && dataTable3.Rows.Count == 0 && num4 != 0)
						{
							dataTable3 = dataTable.AsEnumerable().Skip(i).Take(20)
								.CopyToDataTable();
						}
					}
				}
				if (num3 != 0 && num4 != 0 && num4 <= num3)
				{
					try
					{
						throw new Exception(HtmlLang.GetText(LangModule.FP, "GeneralFaPiaoMustLargeSpecialFaPiao", "航天发票文件。专用发票的数据必须在普通发票之前!"));
					}
					catch (Exception ex2)
					{
						if (type != 7 && type != 8)
						{
							goto end_IL_03b9;
						}
						return base.RedirectToAction("ImportFaPiao", new
						{
							inOrOuttype = inOrOuttype,
							message = ex2.Message
						});
						end_IL_03b9:;
					}
				}
				if (num4 > num3 && num4 < num3 + 20 && num4 != 0 && num3 != 0)
				{
					dataTable2 = dataTable2.AsEnumerable().Take(num4 - num3 - 1).CopyToDataTable();
				}
			}
			int num5 = 20;
			if (resultData.MDataRowIndex > 0)
			{
				num5 += resultData.MDataRowIndex - 1;
			}
			ViewBag.PreviewTop = num5;

			DataTable dataTable4 = dataTable.AsEnumerable().Take(num5).CopyToDataTable();
			ViewBag.ExcelHeader = string.Join(",", ExcelHelper.GetExcelHeader(dataTable4.Columns.Count));
			ViewBag.ExcelData = dataTable4;
			ViewBag.JsonExcelData = JsonConvert.SerializeObject(dataTable4, new DataTableConverter()).Replace("'", "&apos;");
			ViewBag.inOrOuttype = inOrOuttype;

			//if (_003C_003Eo__9._003C_003Ep__7 == null)
			//{
			//	_003C_003Eo__9._003C_003Ep__7 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "isHangTian", typeof(ImportBySolutionController), new CSharpArgumentInfo[2]
			//	{
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
			//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			//	}));
			//}
			//Func<CallSite, object, bool, object> target = _003C_003Eo__9._003C_003Ep__7.Target;
			//CallSite<Func<CallSite, object, bool, object>> _003C_003Ep__ = _003C_003Eo__9._003C_003Ep__7;
			//object viewBag = base.get_ViewBag();
			
			//target(_003C_003Ep__, viewBag, id == num.ToString());
			num = 999999;
			ViewBag.isHangTian = (id == num.ToString());

			ViewBag.SpecialInvoiceData = dataTable2;
			ViewBag.GeneralInvoiceData = dataTable3;
			ViewBag.SpecialInvoiceJson = JsonConvert.SerializeObject(dataTable2, new DataTableConverter()).Replace("'", "&apos;");
			ViewBag.GeneralInvoiceJson = JsonConvert.SerializeObject(dataTable3, new DataTableConverter()).Replace("'", "&apos;");

			return base.View(resultData);
		}

		public ActionResult ConfirmImport(int type, string fileName, string importCnt, string solutionID, string normalFPCount, string specialFPCount, bool hasglstartData = false)
		{
			SetImportInfo(type);
			string arg = string.Empty;
			string arg2 = string.Empty;
			string empty = string.Empty;
			int num = RegExp.IsNumeric(normalFPCount) ? Convert.ToInt32(normalFPCount) : 0;
			int num2 = RegExp.IsNumeric(specialFPCount) ? Convert.ToInt32(specialFPCount) : 0;
			if (num2 > 0 || num > 0)
			{
				importCnt = (num + num2).ToString();
			}

			ViewBag.SolutionID = solutionID;
			ViewBag.OriFileName = FileHelper.GetOriginalFileName(fileName);
			ViewBag.NewFileName = fileName;
			ViewBag.ImportingCount = importCnt;
			ViewBag.Type = type;


			if (type == 7 || type == 8)
			{
				arg = string.Format(HtmlLang.GetText(LangModule.FP, "ConfirmImportFPCount", "<ul><li><strong>{0}</strong>张发票数据中包含{1}张增值税专用发票、{2}张增值税普通发票</li></ul>"), num + num2, specialFPCount, normalFPCount);
				if (hasglstartData)
				{
					arg2 = HtmlLang.GetText(LangModule.FP, "ConfirmImportFPStartDate", "此次导入的发票数据中包含启用日期之前的数据，点击”完成导入“会将启用前的发票数据一起导入系统！");
				}
				string text = HtmlLang.GetText(LangModule.FP, "ConfirmImportNotify", "<p>导入的 <strong>{0}</strong> 包含{1}条 {2}数据，下面的数据将会被导入美记：</p>");

				object arg4 = ViewBag.OriFileName;
				object arg5 = ViewBag.ImportingCount;

				ViewBag.ConfirmImportNotify = string.Format(text, arg4, arg5, ViewBag.TypeName);
			}
			ViewBag.ConfirmImpCnt = arg;
			ViewBag.ConfirmImpState = arg2;

			return base.View();
		}

		private void SetImportInfo(int id)
		{
			ViewBag.Type = id;

			string objName = GetObjName(id);
			ViewBag.TypeNames = objName;
			ViewBag.TypeName = objName.TrimEnd('s').ToLower();
		}

		private string GetObjName(int type)
		{
			return DicObjName.ContainsKey(type) ? DicObjName[type] : string.Empty;
		}

		public JsonResult GetSolutionList(int type)
		{
			List<IOSolutionModel> list = (from a in _solution.GetSolutionList((ImportTypeEnum)type, null).ResultData
			orderby a.MModifyDate descending
			select a).ToList();
			int num = list.Count;
			if (type == 8)
			{
				list.Insert(num, new IOSolutionModel
				{
					MItemID = 999999.ToString(),
					MName = "航天开票文件"
				});
				num++;
			}
			list.Insert(num, new IOSolutionModel
			{
				MItemID = "",
				MName = HtmlLang.GetText(LangModule.Common, "NewImportSolution", "新增导入方案")
			});
			return base.Json(list);
		}

		[ValidateInput(false)]
		[HttpPost]
		public JsonResult Upload(IOImportDataModel model)
		{
			HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
			IOUploadResultModel iOUploadResultModel = new IOUploadResultModel();
			if (httpPostedFile.ContentLength == 0)
			{
				iOUploadResultModel.Message = "";
				return base.GetJsonResult(iOUploadResultModel);
			}
			FileHelper.ValidateFile(httpPostedFile, FileType.ExcelIncludeCSV, FileValidateType.FileType);
			string uploadPath = FileHelper.GetUploadPath("~/App_Data/Temp/Common");
			string arg = FileHelper.UploadFile(httpPostedFile, uploadPath);
			arg = $"{uploadPath}\\{arg}";
			iOUploadResultModel.MFileName = Path.GetFileName(arg);
			iOUploadResultModel.MType = model.TemplateType;
			if (model.SolutionID == 999999.ToString())
			{
				iOUploadResultModel.ObjectID = model.SolutionID;
				iOUploadResultModel.Success = true;
			}
			else
			{
				model.SourceData = GetImportingData(iOUploadResultModel.MFileName);
				OperationResult resultData = _solution.SaveSolution(model, true, null).ResultData;
				if (!resultData.Success)
				{
					iOUploadResultModel.Success = false;
					iOUploadResultModel.Message = resultData.Message;
				}
				iOUploadResultModel.ObjectID = resultData.ObjectID;
				iOUploadResultModel.Success = resultData.Success;
			}
			base.UploadResult = JsonConvert.SerializeObject(iOUploadResultModel);
			return base.GetJsonResult(iOUploadResultModel);
		}

		public JsonResult Save(IOImportDataModel model)
		{
			model.SourceData = GetImportingData(model.FileName);
			model.EffectiveData = ((model.EffectiveData == null || model.EffectiveData.Rows.Count == 0) ? model.SourceData : model.EffectiveData);
			ImportResult resultData = _solution.ImportData(model.TemplateType, model, null).ResultData;
			return base.Json(resultData);
		}

		private DataTable GetImportingData(string fileName)
		{
			string fullPath = string.Join("\\", base.Server.MapPath("~/App_Data/Temp/Common"), base.MContext.MUserID, fileName);
			Stream fileStream = FileHelper.GetFileStream(fullPath);
			NPOIHelper nPOIHelper = new NPOIHelper();
			return nPOIHelper.ImportData(fileStream, fileName);
		}
	}
}
