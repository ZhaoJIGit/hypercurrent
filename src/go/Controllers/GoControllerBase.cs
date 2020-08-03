using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.Controllers;
using JieNor.Megi.Identity.Go.AutoManager;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.IV;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Controllers
{
	public class GoControllerBase : FrameworkController
	{
		private const string _cacheKey = "UploadResult";

		public const string EXPORT_EXCEL_NAME_FORMAT = "{0} - {1}.xls";

		public const string EXPORT_NAME_FORMAT = "{0} - {1}.{2}";

		private static ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

		private IIVInvoice _invoice;

		protected string UploadResult
		{
			get
			{
				return _cache["UploadResult"];
			}
			set
			{
				_cache["UploadResult"] = value;
			}
		}

		public GoControllerBase()
		{
			base.ViewData["LoginUserName"] = base.LoginUserName;
		}

		public GoControllerBase(IIVInvoice invoice)
		{
			_invoice = invoice;
		}

		public FileResult ExportReport(Stream stream, string exportName)
		{
			AddHeaderForDownload(exportName);
			string contentType = "application/octet-stream";
			if (stream is MemoryStream)
			{
				return File(((MemoryStream)stream).ToArray(), contentType, exportName.FilterInvalidFileName());
			}
			return File(stream, contentType, exportName.FilterInvalidFileName());
		}

		public FileStreamResult GetImportTemplateFile(ImportTemplateModel model, string type)
		{
			Stream importTemplateStream = NPOIHelper.GetImportTemplateStream(model, null);
			if (string.IsNullOrWhiteSpace(model.TemplateName))
			{
				model.TemplateName = string.Format("{0} {1}({2}).xls", type, HtmlLang.GetText(LangModule.Common, "Template", "template"), base.MContext.MOrgName);
			}
			else
			{
				model.TemplateName = $"{Path.GetFileNameWithoutExtension(model.TemplateName)} ({base.MContext.MOrgName}){Path.GetExtension(model.TemplateName)}";
			}
			AddHeaderForDownload(model.TemplateName);
			return File(importTemplateStream, "application/octet-stream", model.TemplateName.FilterInvalidFileName());
		}

		public FileStreamResult GetImportTemplateFile(List<ImportTemplateModel> modelList, string type, object data = null)
		{
			Stream importTemplateStream = NPOIHelper.GetImportTemplateStream(modelList, data);
			ImportTemplateModel importTemplateModel = modelList.First();
			importTemplateModel.TemplateName = string.Format("{0} {1}({2}).xls", type, HtmlLang.GetText(LangModule.Common, "Template", "template"), base.MContext.MOrgName);
			AddHeaderForDownload(importTemplateModel.TemplateName);
			return File(importTemplateStream, "application/octet-stream", importTemplateModel.TemplateName.FilterInvalidFileName());
		}

		public FileStreamResult DownloadImportFailedData(string fileName)
		{
			Stream fileStream = FileHelper.GetFileStream(string.Join("\\", base.Server.MapPath("~/App_Data/Temp/ImportFailed"), base.MContext.MUserID, fileName.TrimStart('\\')));
			if (fileStream != null)
			{
				return File(fileStream, "application/vnd.ms-excel", FileHelper.GetOriginalFileName(fileName));
			}
			return null;
		}

		public ReportModel CreateReportModel(BizReportType reportType, string jsonParam, CreateReportModelSource source = CreateReportModelSource.Export, string printSettingID = null, string exportFormat = null)
		{
			return ReportStorageHelper.CreateReportModel(reportType, jsonParam, source, printSettingID, exportFormat, null);
		}

		public JsonResult GetJsonResult(object obj)
		{
			JsonResult jsonResult = base.Json(obj);
			if (!base.Request.AcceptTypes.Contains("application/json"))
			{
				jsonResult.ContentType = "text/html";
				jsonResult.ContentEncoding = Encoding.UTF8;
			}
			return jsonResult;
		}

		[HttpGet]
		public string GetUploadResult()
		{
			return UploadResult;
		}

		public MvcHtmlString ShowImage(string id, string attr = null)
		{
			Stream stream = null;
			string empty = string.Empty;
			string format = "data:image/jpg;base64,{0}";
			BDAttachmentModel attachmentModel = BDAttachmentManager.GetAttachmentModel(id);
			try
			{
				stream = FtpHelper.GetDownloadStream(attachmentModel.MPath, attachmentModel.MUploadName);
			}
			catch (Exception)
			{
			}
			if (stream != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				empty = string.Format(format, Convert.ToBase64String(memoryStream.ToArray()));
			}
			else
			{
				empty = string.Format(format, string.Empty);
			}
			return new MvcHtmlString($"<img {attr} src='{empty}' />");
		}

		public string GetFileExceptionMessage(string fileName, Exception ex)
		{
			FileException ex2 = ex as FileException;
			if (ex2 == null)
			{
				return ex.Message;
			}
			string result = string.Empty;
			switch (ex2.ExceptionType)
			{
			case FileExceptionType.ExcelFormatError:
				result = string.Format(HtmlLang.GetText(LangModule.Docs, "FakeFileType", "The format of the file:{0} you try to upload is incorrect, please save as Excel format and then re-upload!"), fileName);
				break;
			case FileExceptionType.FormatError:
				result = HtmlLang.GetText(LangModule.Common, "FileFormatError", "File format read error!");
				break;
			case FileExceptionType.FormatUnSupport:
			{
				string arg2 = FileHelper.GetSupportExtension(FileType.Default).Replace("|", "、");
				result = string.Format(HtmlLang.GetText(LangModule.Docs, "AcceptUploadFileTypes", "The file:{0} you try to upload is not supported. Only support below file types: {1}"), fileName, arg2);
				break;
			}
			case FileExceptionType.SizeExceedLimit:
			{
				string arg = FileHelper.FormatFileSize(FtpHelper.MaxUploadSize);
				result = string.Format(HtmlLang.GetText(LangModule.Docs, "MaxUploadSize", "The size of file:{0} exceeds the limit({1}), please upload a smaller file."), fileName, arg);
				break;
			}
			}
			return result;
		}

		private void AddHeaderForDownload(string exportName)
		{
			if (base.Request.Browser.Browser == "Safari")
			{
				Encoding uTF = Encoding.UTF8;
				base.Response.Charset = uTF.WebName;
				base.Response.HeaderEncoding = uTF;
				base.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{exportName}\"");
			}
		}

		protected string GetNextOpenDate(List<string> settledPeriods)
		{
			DateTime dateTime;
			if (settledPeriods == null || settledPeriods.Count == 0)
			{
				dateTime = ContextHelper.MContext.MGLBeginDate;
				return dateTime.ToString("yyyy-MM-dd");
			}
			List<DateTime> list = new List<DateTime>();
			foreach (string settledPeriod in settledPeriods)
			{
				string[] array = settledPeriod.Split('-');
				list.Add(new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), 1));
			}
			dateTime = (from t in list
			orderby t descending
			select t).First().AddMonths(1);
			return dateTime.ToString("yyyy-MM-dd");
		}

		protected string GetOpenDate(List<string> settledPeriods)
		{
			if (settledPeriods == null || settledPeriods.Count == 0)
			{
				MContext mContext = ContextHelper.MContext;
				return mContext.MGLBeginDate.ToString("yyyy-MM-dd");
			}
			List<DateTime> list = new List<DateTime>();
			foreach (string settledPeriod in settledPeriods)
			{
				string[] array = settledPeriod.Split('-');
				list.Add(new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), 1));
			}
			return (from t in list
			orderby t descending
			select t).First().AddMonths(1).ToString("yyyy-MM-dd");
		}
	}
}
