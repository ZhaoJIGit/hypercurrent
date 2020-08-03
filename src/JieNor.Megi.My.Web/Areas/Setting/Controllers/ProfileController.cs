using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.My.Web.Controllers;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.SEC;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Setting.Controllers
{
	public class ProfileController : MyControllerBase
	{
		private IBDAttachment AttachmentService;

		private ISECUser UserService;

		private MContext ctx;

		public ProfileController(IBDAttachment attachment, ISECUser user)
		{
			AttachmentService = attachment;
			UserService = user;
			ctx = ContextHelper.MContext;
		}

		public ActionResult ProfileEdit()
		{
			SECUserModel resultData = UserService.GetModelByEmail(ctx.MEmail, null).ResultData;
			SECUserlModel sECUserLModel = resultData.SECUserLModel;

			ViewBag.MFirstName = sECUserLModel.MFristName;
			ViewBag.MLastName = sECUserLModel.MLastName;
			ViewBag.MPKID = sECUserLModel.MPKID;
			ViewBag.MParentID = sECUserLModel.MParentID;
			ViewBag.MImageID = resultData.MProfileImage;
			ViewBag.MJobTitle = sECUserLModel.MJobTitle;
			ViewBag.MBriefBio = sECUserLModel.MBriefBio;
			ViewBag.MMobilePhone = resultData.MMobilePhone;
			ViewBag.MLCD = ctx.MLCID;

			return base.View();
		}

		public ActionResult UploadHeaderImage(SECUserlModel lModel)
		{
			OperationResult operationResult = new OperationResult();
			string text = "0";
			int count = System.Web.HttpContext.Current.Request.Files.Count;
			string text2 = string.Empty;
			if (count > 0)
			{
				HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
				if (httpPostedFile != null && httpPostedFile.ContentLength != 0)
				{
					try
					{
						string fileName = Path.GetFileName(httpPostedFile.FileName);
						int contentLength = httpPostedFile.ContentLength;
						if (!CheckFileType(fileName))
						{
							string text3 = LangHelper.GetText(LangModule.My, "HeadImageFormatError", "Only supports the following format：jpg ,png,gif,jpeg");
							operationResult.Success = false;
							operationResult.Message = text3;
							return base.Json(operationResult);
						}
						DateTime now = DateTime.Now;
						string text4 = string.Format("{0}{1}{2}", now.ToString("yyyyMMddHHmm"), UUIDHelper.GetGuid().Substring(0, 8), Path.GetExtension(httpPostedFile.FileName));
						FtpHelper.UploadFile(text, httpPostedFile.InputStream, text4, contentLength);
						BDAttachmentModel bDAttachmentModel = new BDAttachmentModel();
						bDAttachmentModel.MName = fileName;
						bDAttachmentModel.MUploadName = text4;
						bDAttachmentModel.MSize = contentLength;
						BDAttachmentModel bDAttachmentModel2 = bDAttachmentModel;
						string arg = text;
						now = DateTime.Now;
						object arg2 = now.Year;
						now = DateTime.Now;
						bDAttachmentModel2.MPath = $"/{arg}/{arg2}/{now.Month}/";
						bDAttachmentModel.MOrgID = text;
						text2 = AttachmentService.UpdateAttachmentModel(bDAttachmentModel, null).ResultData;
					}
					catch (Exception ex)
					{
						string message = LangHelper.GetText(LangModule.Docs, "FileUploadFailed", "File upload failed") + ": " + ex.Message;
						operationResult.Success = false;
						operationResult.Message = message;
						return base.Json(operationResult);
					}
				}
			}
			SECUserModel sECUserModel = new SECUserModel();
			sECUserModel.MProfileImage = ((!string.IsNullOrEmpty(text2)) ? text2 : lModel.MProfileImage);
			sECUserModel.MItemID = lModel.ItemId;
			sECUserModel.SECUserLModel = lModel;
			sECUserModel.MMobilePhone = lModel.MMobilePhone;
			UserService.UpdateUserMulitLangModel(sECUserModel, null);
			return base.Json(operationResult);
		}

		private bool CheckFileType(string fileName)
		{
			bool result = false;
			string[] array = fileName.Split('.');
			if (array.Length != 0)
			{
				string text = array.Last();
				if (text.ToLower() == "jpg" || text.ToLower() == "png" || text.ToLower() == "jpeg" || text.ToLower() == "gif")
				{
					result = true;
				}
			}
			return result;
		}

		public ActionResult ShowHeaderIamge(string imageId)
		{
			if (string.IsNullOrEmpty(imageId))
			{
				imageId = UserService.GetModelByEmail(ctx.MEmail, null).ResultData.MProfileImage;
			}
			if (!string.IsNullOrEmpty(imageId))
			{
				BDAttachmentModel resultData = AttachmentService.GetAttachmentModelById(imageId, null).ResultData;
				if (resultData != null)
				{
					Stream downloadStream = FtpHelper.GetDownloadStream(resultData.MPath, resultData.MUploadName);
					List<byte> list = new List<byte>();
					for (int num = downloadStream.ReadByte(); num != -1; num = downloadStream.ReadByte())
					{
						list.Add((byte)num);
					}
					downloadStream.Close();
					return base.File(list.ToArray(), "image/jpeg");
				}
			}
			return base.File(GetDefaultImage(), "image/jpeg");
		}

		private byte[] GetDefaultImage()
		{
			try
			{
				FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Images/default-header.jpg", FileMode.Open);
				int num = (int)fileStream.Length;
				byte[] array = new byte[num];
				fileStream.Read(array, 0, num);
				fileStream.Close();
				return array;
			}
			catch
			{
			}
			return new byte[0];
		}
	}
}
