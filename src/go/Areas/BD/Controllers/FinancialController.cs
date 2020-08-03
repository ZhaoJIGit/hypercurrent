using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.REG;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class FinancialController : GoControllerBase
	{
		private IBASOrganisation _org = null;

		private IREGFinancial _financial = null;

		private IBDAttachment _attachment = null;

		public FinancialController(IBASOrganisation org, IREGFinancial financial, IBDAttachment attachment)
		{
			_org = org;
			_financial = financial;
			_attachment = attachment;
		}

		[Permission("Setting", "View", "")]
		public ActionResult FinancialEdit()
		{
			base.SetTitleAndCrumb(LangHelper.GetText(LangModule.Bank, "Financial", "Financial"), "<a href='/Setting/'>" + LangHelper.GetText(LangModule.Bank, "GeneralSettings", "General Settings") + " > </a>");
			string mOrgID = base.MContext.MOrgID;
			ViewBag.OrgCode = mOrgID;


			ViewData["IsCurrencyEnabled"] = false;
			SetModule("setting");
			string mLCID = MContext.MLCID;
			ViewBag.Lang = MContext.MLCID;

			return base.View();
		}

		public JsonResult GetFinancial(REGFinancialModel model)
		{
			REGFinancialModel rEGFinancialModel = _financial.GetByOrgID(model, null).ResultData;
			if (rEGFinancialModel == null)
			{
				rEGFinancialModel = new REGFinancialModel();
			}
			return base.Json(rEGFinancialModel);
		}

		public JsonResult UpdateFinancial(REGFinancialModel model)
		{
			MActionResult<OperationResult> data = _financial.UpdateByOrgID(model, null);
			return base.Json(data);
		}

		[HttpPost]
		public JsonResult UploadFinancialCert(REGFinancialModel model)
		{
			int count = System.Web.HttpContext.Current.Request.Files.Count;
			string[] array = new string[count];
			string text = string.Empty;
			for (int i = 0; i < count; i++)
			{
				HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[i];
				if (httpPostedFile != null && httpPostedFile.ContentLength != 0)
				{
					try
					{
						FileHelper.ValidateFile(httpPostedFile, FileType.Img, FileValidateType.All);
						text = Path.GetFileName(httpPostedFile.FileName);
						int contentLength = httpPostedFile.ContentLength;
						DateTime dateNow = base.MContext.DateNow;
						string text2 = string.Format("{0}{1}{2}", dateNow.ToString("yyyyMMddHHmm"), UUIDHelper.GetGuid().Substring(0, 8), Path.GetExtension(httpPostedFile.FileName));
						FtpHelper.UploadFile(base.MContext.MOrgID, httpPostedFile.InputStream, text2, contentLength);
						BDAttachmentModel bDAttachmentModel = new BDAttachmentModel();
						bDAttachmentModel.MName = text;
						bDAttachmentModel.MUploadName = text2;
						bDAttachmentModel.MSize = contentLength;
						bDAttachmentModel.MPath = $"/{base.MContext.MOrgID}/{dateNow.Year}/{dateNow.Month}/";
						bDAttachmentModel.MOrgID = base.MContext.MOrgID;
						array[i] = _attachment.UpdateAttachmentModel(bDAttachmentModel, null).ResultData;
					}
					catch (Exception ex)
					{
						string fileExceptionMessage = base.GetFileExceptionMessage(text, ex);
						string message = LangHelper.GetText(LangModule.Docs, "FileUploadFailed", "File upload failed") + ": " + fileExceptionMessage;
						return base.GetJsonResult(new
						{
							isSuccess = true,
							Message = message
						});
					}
				}
			}
			PreHandleAttachment(model, count, array);
			MActionResult<OperationResult> obj = _financial.UpdateByOrgID(model, null);
			return base.GetJsonResult(obj);
		}

		private void PreHandleAttachment(REGFinancialModel model, int fileCount, string[] arrAttachId)
		{
			string mTaxRegCertCopyAttachId = model.MTaxRegCertCopyAttachId;
			string mLocalTaxRegCertCopyAttachId = model.MLocalTaxRegCertCopyAttachId;
			switch (fileCount)
			{
			case 1:
				if (model.IsUpdateTaxRegCert)
				{
					model.MTaxRegCertCopyAttachId = arrAttachId[0];
				}
				else if (model.IsUpdateLocalTaxRegCert)
				{
					model.MLocalTaxRegCertCopyAttachId = arrAttachId[0];
				}
				break;
			case 2:
				if (!string.IsNullOrWhiteSpace(arrAttachId[0]))
				{
					model.MTaxRegCertCopyAttachId = arrAttachId[0];
				}
				if (!string.IsNullOrWhiteSpace(arrAttachId[1]))
				{
					model.MLocalTaxRegCertCopyAttachId = arrAttachId[1];
				}
				break;
			}
			if (!string.IsNullOrEmpty(mTaxRegCertCopyAttachId) && mTaxRegCertCopyAttachId != model.MTaxRegCertCopyAttachId)
			{
				model.TobeDelOriginalAttachIds = mTaxRegCertCopyAttachId;
			}
			if (!string.IsNullOrEmpty(mLocalTaxRegCertCopyAttachId) && mLocalTaxRegCertCopyAttachId != model.MLocalTaxRegCertCopyAttachId)
			{
				model.TobeDelOriginalAttachIds = model.TobeDelOriginalAttachIds + "," + mLocalTaxRegCertCopyAttachId;
				model.TobeDelOriginalAttachIds = model.TobeDelOriginalAttachIds.Trim(',');
			}
		}
	}
}
