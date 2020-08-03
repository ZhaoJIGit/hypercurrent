/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var FileBase = {
    defaultRegex: /\.(TXT|CSV|DOC|DOCX|XLS|XLSX|PPT|PPTX|PDF|GIF|JPG|JPEG|PNG|BMP|RAR|ZIP|7Z)$/i,
    imgRegex: /\.(GIF|JPG|JPEG|PNG|BMP)$/i,
    excelIncludeCsvRegex: /\.(XLS|XLSX|CSV)$/i,
    excelExcludeCsvRegex: /\.(XLS|XLSX)$/i,
    maxUploadSize: parseInt($("#hdnMaxUploadSize").val()),
    validateFile: function (fileName, fileSize, regex, callBack) {
        fileName = fileName.substring(fileName.lastIndexOf('\\') + 1);
        var result;
        var curRegex = regex || FileBase.defaultRegex;
        var strRegex = curRegex.toString().replace('/\\.(', '').replace(')$/i', '').replace(/\|/g, '、');
        if (!(curRegex).test(fileName)) {
            result = HtmlLang.Write(LangModule.Docs, "AcceptUploadFileTypes", "The file:{0} you try to upload is not supported. Only support below file types: {1}").replace("{0}", fileName).replace("{1}", strRegex);
        }
        else if (fileSize > FileBase.maxUploadSize) {
            result = HtmlLang.Write(LangModule.Docs, "MaxUploadSize", "The size of file:{0} exceeds the limit({1}), please upload a smaller file.").replace("{0}", fileName).replace("{1}", FileBase.formatFileSize(FileBase.maxUploadSize));
        }
        if (callBack != undefined) {
            callBack(result);
        }
        return result;
    },
    formatFileSize: function(bytes) {
        if (typeof bytes !== 'number') {
            return '';
        }
        if (bytes >= 1024 * 1024) {
            return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
        }
        return (bytes / 1024).toFixed(2) + ' KB';
    }
}