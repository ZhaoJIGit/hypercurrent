using JieNor.Megi.Core;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.PT;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PT.Controllers
{
	public class PTBaseController : GoControllerBase
	{
		private IBDAttachment _attachment = null;

		private IPTBiz _ptBiz = null;

		private IPTVoucher _ptVoucher = null;

		private IPTSalaryList _ptSalaryList = null;

		public PTBaseController(IBDAttachment attachment)
		{
			_attachment = attachment;
		}

		public PTBaseController(IBDAttachment attachment, IPTBiz ptBiz, IPTVoucher ptVoucher, IPTSalaryList ptSalaryList)
		{
			_attachment = attachment;
			_ptBiz = ptBiz;
			_ptVoucher = ptVoucher;
			_ptSalaryList = ptSalaryList;
		}

		public PTBaseController()
		{
		}

		public ActionResult PTIndex()
		{
			return base.View(GetActionModel(PTTypeEnum.Biz));
		}

		public ActionResult PTBizListPartial()
		{
			return GetPartial(PTTypeEnum.Biz);
		}

		public ActionResult PTVoucherListPartial()
		{
			return GetPartial(PTTypeEnum.Voucher);
		}

		public ActionResult PTSalaryListPartial()
		{
			return GetPartial(PTTypeEnum.SalaryList);
		}

		private ActionResult GetPartial(PTTypeEnum type)
		{
			return base.PartialView(GetActionModel(type));
		}

		public PTListModel GetActionModel(PTTypeEnum type)
		{
			PTListModel pTListModel = new PTListModel();
			switch (type)
			{
			case PTTypeEnum.Biz:
				pTListModel.List = _ptBiz.GetList(null).ResultData;
				break;
			case PTTypeEnum.Voucher:
					pTListModel.List = _ptVoucher.GetList(null).ResultData;
				break;
			case PTTypeEnum.SalaryList:
					pTListModel.List = _ptSalaryList.GetList(null).ResultData;
				break;
			}
			return pTListModel;
		}

		public string UploadLogoFile(string id, string errMsg)
		{
			string result = string.Empty;
			HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
			string text = string.Empty;
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
					result = _attachment.UpdateAttachmentModel(bDAttachmentModel, null).ResultData;
				}
				catch (Exception ex)
				{
					string fileExceptionMessage = base.GetFileExceptionMessage(text, ex);
					errMsg = LangHelper.GetText(LangModule.Docs, "FileUploadFailed", "File upload failed") + ": " + fileExceptionMessage;
				}
			}
			return result;
		}
	}
}
