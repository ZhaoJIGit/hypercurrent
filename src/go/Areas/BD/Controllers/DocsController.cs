using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class DocsController : GoControllerBase
	{
		private IBDAttachment _iBDAttachment = null;

		public DocsController(IBDAttachment iBDAttachment)
		{
			_iBDAttachment = iBDAttachment;
		}

		[Permission("Attachment", "View", "")]
		public ActionResult Folders()
		{
			base.SetTitle(LangHelper.GetText(LangModule.Docs, "Files", "Files"));
			ViewBag.CurrentUserId = base.LoginUserName;
			ViewBag.MaxUploadSize = FtpHelper.MaxUploadSize;

			return base.View(new BDAttachmentFolderListModel
			{
				List = _iBDAttachment.GetCategoryList(null).ResultData
			});
		}

		public ActionResult FoldersPartial(string reload)
		{
			if (reload == "1")
			{
				return base.PartialView(new BDAttachmentFolderListModel
				{
					List = _iBDAttachment.GetCategoryList(null).ResultData
				});
			}
			return base.PartialView();
		}

		[Permission("Attachment", "View", "")]
		public ActionResult FileView(string curFileId, string fileIds, bool? isSetup)
		{
			List<BDAttachmentListModel> resultData = _iBDAttachment.GetAttachmentListByIds(string.IsNullOrWhiteSpace(fileIds) ? curFileId : fileIds, null).ResultData;
			ViewBag.AttachmentList = resultData;

			BDAttachmentListModel bDAttachmentListModel = resultData.FirstOrDefault((BDAttachmentListModel f) => f.MItemID == curFileId);
			if (bDAttachmentListModel == null)
			{
				bDAttachmentListModel = new BDAttachmentListModel();
			}
			ViewBag.PageIndex = resultData.IndexOf(bDAttachmentListModel) + 1;

			bDAttachmentListModel.MSizeFormated = FormatFileSize(bDAttachmentListModel.MSize);
			ViewBag.CurrentAttachment = bDAttachmentListModel;

			base.ViewData["IsSetup"] = (isSetup.HasValue ? isSetup.Value.ToString().ToLower() : "false");
			base.ViewData["EnablePaging"] = !string.IsNullOrWhiteSpace(fileIds);
			return base.View();
		}

		[Permission("Attachment", "Change", "")]
		public ActionResult SelectFiles()
		{
			List<BDAttachmentCategoryListModel> resultData = _iBDAttachment.GetCategoryList(null).ResultData;
			if (resultData != null)
			{
				ViewBag.CurrentFolderId = resultData[0].MItemID;
			}
			return base.View(new BDAttachmentFolderListModel
			{
				List = resultData
			});
		}

		public ActionResult ShowImage(string id)
		{
			Stream stream = null;
			BDAttachmentModel resultData = _iBDAttachment.GetAttachmentModel(id, null).ResultData;
			try
			{
				stream = FtpHelper.GetDownloadStream(resultData.MPath, resultData.MUploadName);
			}
			catch (Exception ex)
			{
				string message = string.Format(LangHelper.GetText(LangModule.Docs, "DownloadFileError", "File download failed:{0}"), ex.Message);
				throw new Exception(message);
			}
			string contentType = $"image/{resultData.MUploadName.Split('.')[1]}";
			return new ImageResult(stream, contentType);
		}

		public JsonResult GetAttachmentCategoryList()
		{
			MActionResult<List<BDAttachmentCategoryListModel>> categoryList = _iBDAttachment.GetCategoryList(null);
			return base.Json(categoryList, JsonRequestBehavior.AllowGet);
		}

		[Permission("Attachment", "View", "")]
		public FileStreamResult DownloadFile(string id)
		{
			BDAttachmentModel resultData = _iBDAttachment.GetAttachmentModel(id, null).ResultData;
			Stream downloadStream = FtpHelper.GetDownloadStream(resultData.MPath, resultData.MUploadName);
			return File(downloadStream, "application/octet-stream", resultData.MName);
		}

		public JsonResult GetFolderInfo(string folderId)
		{
			List<BDAttachmentCategoryListModel> resultData = _iBDAttachment.GetCategoryList(null).ResultData;
			BDAttachmentCategoryModel data = resultData.FirstOrDefault((BDAttachmentCategoryListModel f) => f.MItemID == folderId);
			return base.Json(data);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult FolderAdd()
		{
			BDAttachmentCategoryModel bDAttachmentCategoryModel = new BDAttachmentCategoryModel();
			bDAttachmentCategoryModel.MCategoryName = GenerateNewFolderName();
			bDAttachmentCategoryModel.MOrgID = base.MContext.MOrgID;
			MActionResult<OperationResult> data = _iBDAttachment.UpdateCategoryModel(bDAttachmentCategoryModel, null);
			return base.Json(data);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult FolderEdit(BDAttachmentCategoryModel model)
		{
			model.MOrgID = base.MContext.MOrgID;
			OperationResult resultData = _iBDAttachment.UpdateCategoryModel(model, null).ResultData;
			string message = (!resultData.Success) ? resultData.VerificationInfor[0].Message : string.Empty;
			return base.Json(new
			{
				Success = resultData.Success,
				Message = message,
				MCategoryNameEllipsis = EllipsisString(model.MCategoryName)
			});
		}

		public JsonResult GetFileList(BDAttachmentListFilterModel param)
		{
			MActionResult<DataGridJson<BDAttachmentListModel>> attachmentList = _iBDAttachment.GetAttachmentList(param, null);
			return base.Json(attachmentList);
		}

		public JsonResult GetRelatedFileList(string bizObject, string bizObjectID, string fileIds)
		{
			MActionResult<List<BDAttachmentListModel>> relatedAttachmentList = _iBDAttachment.GetRelatedAttachmentList(bizObject, bizObjectID, fileIds, null);
			return base.Json(relatedAttachmentList);
		}

		public JsonResult GetFileListByIds(string fileIds)
		{
			MActionResult<List<BDAttachmentListModel>> attachmentListByIds = _iBDAttachment.GetAttachmentListByIds(fileIds, null);
			return base.Json(attachmentListByIds);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult DeleteFiles(ParamBase param)
		{
			MActionResult<OperationResult> data = _iBDAttachment.DeleteAttachmentList(param, null);
			return base.Json(data);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult MoveFilesTo(ParamBase param)
		{
			bool flag = true;
			try
			{
				_iBDAttachment.MoveAttachmentListTo(param, null);
			}
			catch (Exception)
			{
				flag = false;
			}
			return base.Json(flag);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult CreateFilesAssociation(string bizObject, ParamBase param)
		{
			_iBDAttachment.CreateAttachmentAssociation(bizObject, param, null);
			return base.Json(true);
		}

		[Permission("Attachment", "Change", "")]
		public JsonResult RemoveFileRelation(string bizObject, ParamBase param)
		{
			return base.Json(_iBDAttachment.RemoveAttachmentAssociation(bizObject, param, null));
		}

		public JsonResult GetBizObjectCategoryId(string id)
		{
			MActionResult<string> associateTargetCategoryId = _iBDAttachment.GetAssociateTargetCategoryId(id, null);
			return base.Json(associateTargetCategoryId);
		}

		[Permission("Attachment", "Change", "")]
		[HttpPost]
		public JsonResult UploadFile(string id)
		{
			HttpPostedFile httpPostedFile = System.Web.HttpContext.Current.Request.Files[0];
			string mItemID = string.Empty;
			object obj = null;
			string text = string.Empty;
			if (httpPostedFile != null)
			{
				try
				{
					FileHelper.ValidateFile(httpPostedFile, FileType.Default, FileValidateType.All);
					text = Path.GetFileName(httpPostedFile.FileName);
					int contentLength = httpPostedFile.ContentLength;
					DateTime dateNow = base.MContext.DateNow;
					string text2 = string.Format("{0}{1}{2}", dateNow.ToString("yyyyMMddHHmm"), UUIDHelper.GetGuid().Substring(0, 8), Path.GetExtension(httpPostedFile.FileName));
					FtpHelper.UploadFile(base.MContext.MOrgID, httpPostedFile.InputStream, text2, contentLength);
					BDAttachmentModel bDAttachmentModel = new BDAttachmentModel();
					bDAttachmentModel.MCategoryID = id;
					bDAttachmentModel.MName = text;
					bDAttachmentModel.MUploadName = text2;
					bDAttachmentModel.MSize = contentLength;
					bDAttachmentModel.MPath = $"/{base.MContext.MOrgID}/{dateNow.Year}/{dateNow.Month}/";
					bDAttachmentModel.MOrgID = base.MContext.MOrgID;
					mItemID = _iBDAttachment.UpdateAttachmentModel(bDAttachmentModel, null).ResultData;
				}
				catch (Exception ex)
				{
					string fileExceptionMessage = base.GetFileExceptionMessage(text, ex);
					string message = LangHelper.GetText(LangModule.Docs, "FileUploadFailed", "File upload failed") + ": " + fileExceptionMessage;
					obj = new
					{
						isSuccess = false,
						Message = message
					};

					UploadResult = JsonConvert.SerializeObject(obj); 
					return GetJsonResult(obj);
				}
			}
			obj = new
			{
				isSuccess = true,
				MItemID = mItemID
			};

			UploadResult = JsonConvert.SerializeObject(obj);
			return GetJsonResult(obj);
		}

		private string GenerateNewFolderName()
		{
			string defaultName = LangHelper.GetText(LangModule.Docs, "NewFolderDefaultName", "New folder");
			Regex regex = new Regex(defaultName, RegexOptions.IgnoreCase);
			List<BDAttachmentCategoryListModel> resultData = _iBDAttachment.GetCategoryList(null).ResultData;
			List<int> list = new List<int>();
			if (resultData != null)
			{
				List<BDAttachmentCategoryListModel> list2 = (from f in resultData
				where f.MCategoryName.StartsWith(defaultName, StringComparison.InvariantCultureIgnoreCase)
				select f).ToList();
				int item = 0;
				foreach (BDAttachmentCategoryListModel item2 in list2)
				{
					string s = regex.Replace(item2.MCategoryName, "").Trim();
					int.TryParse(s, out item);
					list.Add(item);
				}
			}
			if (list != null && list.Count > 0)
			{
				return defaultName + " " + (list.Max() + 1);
			}
			return defaultName;
		}

		private string FormatFileSize(decimal bytes)
		{
			string empty = string.Empty;
			if (bytes >= 1048576m)
			{
				return Math.Round(bytes / 1048576m, 2) + "MB";
			}
			return Math.Round(bytes / 1024m) + "KB";
		}

		private string EllipsisString(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}
			int num = 15;
			int byteCount = Encoding.GetEncoding("gb2312").GetByteCount(str);
			if (byteCount > num)
			{
				int num2 = 0;
				for (int i = 0; i < str.Length; i++)
				{
					int byteCount2 = Encoding.GetEncoding("gb2312").GetByteCount(new char[1]
					{
						str[i]
					});
					num2 += byteCount2;
					if (num2 > num)
					{
						return str.Substring(0, i) + "...";
					}
				}
			}
			return str;
		}
	}
}
