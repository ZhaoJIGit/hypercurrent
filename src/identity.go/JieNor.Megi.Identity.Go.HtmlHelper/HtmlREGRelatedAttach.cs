using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity.HtmlHelper;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlREGRelatedAttach
	{
		public static MvcHtmlString RelatedAttach(string bizObject, string bizObjectID)
		{
			if (!HtmlSECMenu.HavePermission("Attachment", "View", ""))
			{
				return new MvcHtmlString(string.Empty);
			}
			string text = string.Format("<div class='footer'>\r\n                    <div class='left'><a id='aAddFromFilesLib' class='easyui-linkbutton'>{0}</a></div>\r\n                    <div class='right'>\r\n                        <input type='file' id='fileInput' name='fileInput' filter='track;uploadFile' multiple='multiple' class='input-file'>\r\n                         <a id='aUploadFiles' class='easyui-linkbutton' style='margin-right:0px;'>{1}</a>\r\n                    </div>\r\n                </div>", HtmlLang.Write(LangModule.Docs, "AddFromFileLib", "+ Add from file library..."), HtmlLang.Write(LangModule.Docs, "UploadFiles", "+ Upload files..."));
			string format = string.Format("<a id='aRelatedAttach' href='javascript:void(0);' title='{2}' class='m-icon-attachment'><span id='spAttachCount' class='m-attach-count'></span></a>\r\n                <div id='divRelatedAttach' class='m-form-related-attach'>\r\n                    <b class='popup-arrow popup-arrow-notch'></b>\r\n                    <b class='popup-arrow'></b>\r\n                    <div class='header'>{0}</div>\r\n                    <div id='divFiles' class='body'><div id='divFileList'>{{0}}</div></div>{1}                                    \r\n                </div>", HtmlLang.Write(LangModule.Docs, "RelatedFiles", "Related Files"), HtmlSECMenu.HavePermission("Attachment", "Change", "") ? text : string.Empty, HtmlLang.Write(LangModule.Docs, "Attachment", "Attachment"));
			string str = string.Format("<input type='hidden' id='hdnMaxUploadSize' value='{0}' />{6}\r\n                <input type='hidden' id='hidBizObject' value='{3}' />\r\n                <script type='text/javascript' src='{1}/fw/scripts/jquery.ui.widget.js?{5}'></script>\r\n                <script type='text/javascript' src='{1}/fw/scripts/jquery.iframe-transport.js?{5}'></script>\r\n                <script type='text/javascript' src='{1}/fw/scripts/jquery.fileupload.js?{5}'></script>\r\n                <script type='text/javascript' src='{2}/Scripts/BD/Docs/FileBase.js?{5}'></script>\r\n                <script type='text/javascript' src='{2}/Scripts/BD/Import/ImportBase.js?{5}'></script>\r\n                <script type='text/javascript' src='{2}/Scripts/BD/Docs/AssociateFiles.js?{5}'></script>\r\n                <script type='text/javascript'>$(document).ready(function(){{AssociateFiles.initTarget('{3}', '{4}');AssociateFiles.loadFileList();}});</script>", FtpHelper.MaxUploadSize, ServerHelper.StaticServer, ServerHelper.GoServer, bizObject, bizObjectID, ServerHelper.JSVersion, HtmlLang.WriteScript(LangModule.Docs));
			return new MvcHtmlString(string.Format(format, string.Empty) + str);
		}
	}
}
