using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Go.Web.VIewModels;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.FP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.FP.Controllers
{
    public class FPHomeController : GoControllerBase
    {
        private IFPTable _table;

        private IFPFapiao _fapiao;
        public static string username = ConfigurationManager.AppSettings["BW_Username"];// "admin_3000000222561";
        public static string client_secret = ConfigurationManager.AppSettings["BW_Secret"];//"091e2a55-b0a3-4939-97ce-d9901cebc862";
        public static string client_id = ConfigurationManager.AppSettings["BW_Client_Id"];//"1001431";
        public static string password = ConfigurationManager.AppSettings["BW_Password"];//"Aa12345678";
                                                                                        //用户盐值
        public static string key = ConfigurationManager.AppSettings["BW_Key"];//"46aa1f29ff154ac08b628a7b742f76a2";
        public static string terminalCode = ConfigurationManager.AppSettings["BW_TerminalCode"];//"46aa1f29ff154ac08b628a7b742f76a2";



        public FPHomeController(IFPTable table, IFPFapiao fapiao)
        {
            _table = table;
            _fapiao = fapiao;
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult FPHome(int index = 1, int invoiceType = 0)
        {
            base.ViewData["index"] = index;
            base.ViewData["invoiceType"] = invoiceType;
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult FPEditTable(string tableId, string invoiceIds, int invoiceType = 0)
        {
            base.ViewData["invoiceIds"] = invoiceIds;
            base.ViewData["tableId"] = tableId;
            base.ViewData["invoiceType"] = invoiceType;
            base.ViewData["MCurrencyID"] = base.MContext.MBasCurrencyID;
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetInvoiceTable(string tableId, string invoiceIds)
        {
            MActionResult<FPTableViewModel> tableViewModel = _table.GetTableViewModel(tableId, invoiceIds, 0, null);
            return base.Json(tableViewModel);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult FPSaveTable(FPTableViewModel table)
        {
            MActionResult<OperationResult> data = _table.SaveTable(table, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult FPSaveFapiao(FPFapiaoModel fapiao)
        {
            MActionResult<FPFapiaoModel> data = _fapiao.SaveFapiao(fapiao, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetTableBaseInfo(string tableId, string invoiceIds, int invoiceType)
        {
            MActionResult<FPTableViewModel> tableViewModel = _table.GetTableViewModel(tableId, invoiceIds, invoiceType, null);
            return base.Json(tableViewModel);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetTableHomeData(int invoiceType, DateTime? date)
        {
            if (!date.HasValue)
            {
                date = new DateTime(1900, 1, 1);
            }
            MActionResult<List<NameValueModel>> tableHomeData = _table.GetTableHomeData(invoiceType, date.Value, null);
            return base.Json(tableHomeData);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetFapiaoList(string tableId, string invoiceIds)
        {
            MActionResult<List<FPFapiaoModel>> fapiaoListByTableInvoice = _table.GetFapiaoListByTableInvoice(tableId, invoiceIds, null);
            return base.Json(fapiaoListByTableInvoice);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetTableViewModelPageList(FPTableViewFilterModel filter)
        {
            MActionResult<DataGridJson<FPTableViewModel>> tableViewModelPageList = _table.GetTableViewModelPageList(filter, null);
            return base.Json(tableViewModelPageList);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult DeleteFapiaoByFapiaoIds(FPFapiaoFilterModel model)
        {
            MActionResult<OperationResult> data = _fapiao.DeleteFapiaoByFapiaoIds(model, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult DeleteFPImportByImportIds(FPFapiaoFilterModel model)
        {
            MActionResult<OperationResult> data = _fapiao.DeleteFPImportByIds(model, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult BatchUpdateFPStatusByIds(FPFapiaoFilterModel model)
        {
            MActionResult<OperationResult> data = _fapiao.BatchUpdateFPStatusByIds(model, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult BatchUpdateFPVerifyType(List<FPFapiaoModel> model)
        {
            MActionResult<OperationResult> data = _fapiao.BatchUpdateFPVerifyType(model, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult FPEditFapiao(DateTime? date, string mid = null, string contactId = null, string tableId = null, string invoiceType = null, string explanation = null, string number = null, string tableNumber = null, decimal maxAmount = default(decimal), decimal maxTaxAmount = default(decimal))
        {
            mid = (mid ?? string.Empty);
            contactId = (contactId ?? string.Empty);
            tableId = (tableId ?? string.Empty);
            number = (number ?? string.Empty);
            explanation = (explanation ?? string.Empty);
            tableNumber = (tableNumber ?? string.Empty);
            invoiceType = (invoiceType ?? "0");
            base.ViewData["mid"] = mid;
            base.ViewData["contactId"] = contactId;
            base.ViewData["tableId"] = tableId;
            base.ViewData["explanation"] = explanation;
            base.ViewData["date"] = (date.HasValue ? date.Value : ContextHelper.MContext.DateNow);
            base.ViewData["number"] = number;
            base.ViewData["tableNumber"] = tableNumber;
            base.ViewData["maxAmount"] = maxAmount;
            base.ViewData["maxTaxAmount"] = maxTaxAmount;
            base.ViewData["invoiceType"] = invoiceType;
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetFapiao(FPFapiaoModel fapiao)
        {
            MActionResult<List<FPFapiaoModel>> fapiaoByIds = _fapiao.GetFapiaoByIds(new List<string>
            {
                fapiao.MID
            }, true, fapiao.MContactID, null);
            FPFapiaoModel fPFapiaoModel = fapiaoByIds.ResultData[0];
            if (string.IsNullOrWhiteSpace(fPFapiaoModel.MID))
            {
                fPFapiaoModel.MNumber = fapiao.MNumber;
                fPFapiaoModel.MExplanation = fapiao.MExplanation;
                fPFapiaoModel.MBizDate = fapiao.MBizDate;
                fPFapiaoModel.MTableID = fapiao.MTableID;
            }
            return base.Json(fPFapiaoModel);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult FPAddLog(FPFapiaoModel model)
        {
            _table.FPAddLog(model, null);
            return base.Json(new
            {
                Success = true
            }, JsonRequestBehavior.DenyGet);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult DeleteTableByInvoiceIds(string invoiceIds)
        {
            MActionResult<OperationResult> data = _table.DeleteTableByInvoiceIds(invoiceIds, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult DeleteTableByTableIds(string tableIds)
        {
            MActionResult<OperationResult> data = _table.DeleteTableByTableIds(tableIds, null);
            return base.Json(data);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public JsonResult GetTableViewModelByInvoiceID(string invoiceId)
        {
            MActionResult<FPTableViewModel> tableViewModelByInvoiceID = _table.GetTableViewModelByInvoiceID(invoiceId, null);
            return base.Json(tableViewModelByInvoiceID);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult FPViewFapiao(string fapiaoId)
        {
            base.ViewData["FapiaoId"] = fapiaoId;
            FPFapiaoModel resultData = _fapiao.GetFapiaoModel(new FPFapiaoFilterModel
            {
                MFapiaoIDs = new List<string>
                {
                    fapiaoId
                }
            }, null).ResultData;
            base.ViewData["FapiaoModel"] = resultData;
            base.ViewData["FapiaoType"] = resultData.MType;
            return base.View();
        }



        #region 百望云对接
        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        [HttpPost]
        public ActionResult FPSendBWFapiao(string fapiaoId)
        {
            base.ViewData["FapiaoId"] = fapiaoId;
            FPFapiaoModel resultData = _fapiao.GetFapiaoModel(new FPFapiaoFilterModel
            {
                MFapiaoIDs = new List<string>
                {
                    fapiaoId
                }
            }, null).ResultData;

            var enstr = password + key;

            var p = GetPassword(enstr);

            ///获取token
            var result = GetToken(p);

            if (result.errorResponse != null)
            {
                return View();
            }
            var model = new BWInvoiceRequestModel();
            model.invoiceTerminalCode = terminalCode;

            model.taxNo = resultData.MSContactTaxCode;
            var invoice = new BWInvoiceRequestData();

            invoice.buyerAddressPhone = resultData.MPContactAddressPhone;
            invoice.buyerBankAccount = resultData.MPContactBankInfo;
            invoice.buyerName = resultData.MPContactName;
            invoice.buyerTaxNo = resultData.MPContactTaxCode;
            invoice.buyerAddressPhone = resultData.MPContactAddressPhone;
            invoice.serialNo = resultData.MFapiaoID;
            invoice.sellerBankAccount = resultData.MSContactBankInfo;
            invoice.sellerAddressPhone = resultData.MSContactAddressPhone;
            invoice.invoiceTypeCode=resultData.MType==0?"007":"004";
            invoice.invoiceListMark = resultData.MFapiaoEntrys.Count >= 8 ? "1" : "0";
            var detail = new List<InvoiceDetail>();
            int i = 1;
            foreach (var item in resultData.MFapiaoEntrys)
            {
                InvoiceDetail invoiceDetail = new InvoiceDetail();
                invoiceDetail.goodsLineNo = i;
                invoiceDetail.goodsName = item.MItemName;
                invoiceDetail.goodsPrice = item.MPrice == 0 ? "" : item.MPrice.ToString();
                invoiceDetail.goodsTotalPrice= item.MAmount.ToString();
                invoiceDetail.goodsTaxRate = item.MTaxPercent / 100;
                invoiceDetail.goodsQuantity = item.MQuantity==0?"": item.MQuantity.ToString();
                invoiceDetail.goodsTotalTax = "";// item.MTaxAmount.ToString();
                invoiceDetail.goodsUnit = item.MUnit==null?"" : item.MUnit;
                invoiceDetail.goodsSpecification = item.MItemType;
                detail.Add(invoiceDetail);

                i++;
            }


            invoice.invoiceDetailsList = detail;

            model.data = invoice;

            //根据流水号查询发票是否已开



            //作废发票，重新开票


            //开票具
            var reInvoice = AddInvoice(result.response.access_token, model);
            if (reInvoice.success)
            {
                return Json(new { code = 200, result = true, msg = "Successful" }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new { code = 201, result = false, data = JsonConvert.SerializeObject(reInvoice)+":::" + JsonConvert.SerializeObject(model), msg= reInvoice .errorResponse.message+";"+ reInvoice .errorResponse.subMessage}, JsonRequestBehavior.DenyGet);

            }

        }



        private string GetPassword(string text)
        {
            byte[] by = Encoding.UTF8.GetBytes(text);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(by);
            var sign2 = byteToHexStrToUpper(output);

            byte[] by2 = Encoding.UTF8.GetBytes(sign2);

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(by2);//得到哈希值
            var enText = new StringBuilder();
            //Convert.ToBase64String(result);
            //foreach (byte iByte in result)
            //{
            //    enText.AppendFormat("{0:x2}", iByte);
            //}
            var sign = byteToHexStrToUpper(result);
            return sign;
        }
        private TokenModel GetToken(string p)
        {
            string serviceAddress = "https://sandbox-openapi.baiwang.com/router/rest" +
                 "?timestamp=" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000 +
                 "&username=" + username +
                 "&client_secret=" + client_secret +
                 "&grant_type=password" +
                 "&method=baiwang.oauth.token" +
                 "&client_id=" + client_id +
                 "&password=" + p +
                 "&version=6.0";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "POST";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            //Response.Write(retString);

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(retString);

            return result;
        }
        private BWInvoiceResponseModel AddInvoice(string token, BWInvoiceRequestModel model)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();

            // 添加协议级请求参数
            var requsetId = Guid.NewGuid().ToString();
            var tp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            pairs.Add("method", "baiwang.output.invoice.issue");
            pairs.Add("version", "6.0");
            pairs.Add("appKey", client_id);
            pairs.Add("format", "json");
            pairs.Add("timestamp", tp.ToString());
            pairs.Add("token", token);
            pairs.Add("type", "sync");
            pairs.Add("requestId", requsetId);


            string requestBody = JsonConvert.SerializeObject(model);

            string signString = signTopRequest(pairs, client_secret, requestBody);//签名参数


            HttpClient webClient = new HttpClient();

            StringContent stringContent = new StringContent(requestBody);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");



            var result2 = webClient.PostAsync("https://sandbox-openapi.baiwang.com/router/rest" +
                "?method=baiwang.output.invoice.issue" +
                "&version=6.0" +
                "&appKey=" + client_id +
                "&format=json" +
                "&timestamp=" + tp +
                "&token=" + token +
                "&type=sync" +
                "&requestId=" + requsetId +
                "&sign=" + signString

                , stringContent).Result;
            var msgbody = result2.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<BWInvoiceResponseModel>(msgbody);
            return obj;
        }
        private GoodResponseModel GetGood(string token, InvoiceDetail model)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();

            // 添加协议级请求参数
            var requsetId = Guid.NewGuid().ToString();
            var tp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            pairs.Add("method", "baiwang.output.productcode.query");
            pairs.Add("version", "6.0");
            pairs.Add("appKey", client_id);
            pairs.Add("format", "json");
            pairs.Add("timestamp", tp.ToString());
            pairs.Add("token", token);
            pairs.Add("type", "sync");
            pairs.Add("requestId", requsetId);


            //            {
            //                "data":{
            //                    "pageNo":"",
            //		"customGoodsCode":"",
            //		"pageSize":"",
            //		"goodsCode":"",
            //		"goodsName":""

            //    },
            //	"taxNo":"512345678900000040"
            //}

            var data = new
            {
                data = new
                {
                    pageNo = "",
                    customGoodsCode = "",
                    pageSize = "",
                    goodsCode = model.goodsCode,
                    goodsName = model.goodsName
                },
                taxNo = "36996300000000011"
            };


            string requestBody = JsonConvert.SerializeObject(data);

            string signString = signTopRequest(pairs, client_secret, requestBody);//签名参数


            HttpClient webClient = new HttpClient();

            StringContent stringContent = new StringContent(requestBody);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var result2 = webClient.PostAsync("https://sandbox-openapi.baiwang.com/router/rest" +
                "?method=baiwang.output.productcode.query" +
                "&version=6.0" +
                "&appKey=" + client_id +
                "&format=json" +
                "&timestamp=" + tp +
                "&token=" + token +
                "&type=sync" +
                "&requestId=" + requsetId +
                "&sign=" + signString

                , stringContent).Result;
            var msgbody = result2.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<GoodResponseModel>(msgbody);
            return obj;
        }
        private GoodResponseModel AddGood(string token, InvoiceDetail model, bool IsAdd = true)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();

            // 添加协议级请求参数
            var requsetId = Guid.NewGuid().ToString();
            var tp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            pairs.Add("method", IsAdd ? "baiwang.output.productcode.add" : "baiwang.output.productcode.update");
            pairs.Add("version", "6.0");
            pairs.Add("appKey", client_id);
            pairs.Add("format", "json");
            pairs.Add("timestamp", tp.ToString());
            pairs.Add("token", token);
            pairs.Add("type", "sync");
            pairs.Add("requestId", requsetId);

            var data = new
            {
                data = new
                {
                    priceTaxMark = "Y",
                    parentCode = "",
                    goodsName = "",
                    freeType = 0,
                    rate = model.goodsTaxRate,
                    useDiscount = "N",
                    goodsUnit = model.goodsUnit,
                    discountType = "",
                    illustrate = "",
                    specificationModel = model.goodsSpecification,
                    goodsPrice = model.goodsPrice,
                    definedCode = model.goodsCode,
                    simpleCode = model.goodsCode,
                    customGoodsCode = model.goodsCode
                },
                taxNo = "36996300000000011"
            };


            string requestBody = JsonConvert.SerializeObject(data);

            string signString = signTopRequest(pairs, client_secret, requestBody);//签名参数


            HttpClient webClient = new HttpClient();

            StringContent stringContent = new StringContent(requestBody);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var result2 = webClient.PostAsync("https://sandbox-openapi.baiwang.com/router/rest" +
                "?method=" + (IsAdd ? "baiwang.output.productcode.add" : "baiwang.output.productcode.update") +
                "&version=6.0" +
                "&appKey=" + client_id +
                "&format=json" +
                "&timestamp=" + tp +
                "&token=" + token +
                "&type=sync" +
                "&requestId=" + requsetId +
                "&sign=" + signString

                , stringContent).Result;
            var msgbody = result2.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<GoodResponseModel>(msgbody);
            return obj;
        }

        private string signTopRequest(Dictionary<string, string> pairs, string secret, string body)
        {

            // 第一步：检查参数是否已经排序
            var arr = pairs.Select(i => i.Key).ToArray();

            Array.Sort(arr, string.CompareOrdinal); //ASCII排序
            StringBuilder query = new StringBuilder();
            query.Append(secret);
            // 第二步：把所有参数名和参数值串在一起
            for (int i = 0; i < arr.Length; i++)
            {
                var value = pairs.First(j => j.Key == arr[i]);
                if (!string.IsNullOrWhiteSpace(value.Value) && !string.IsNullOrWhiteSpace(value.Key))
                {
                    query.Append(value.Key).Append(value.Value);
                }
            }
            string tempStr = body.Replace("\n", "").Replace("\r", "");

            query.Append(tempStr);
            query.Append(secret);
            byte[] result = Encoding.UTF8.GetBytes(query.ToString());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            var sign = byteToHexStr(output);
            return sign;
        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static string byteToHexStrToUpper(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("x2");
                }
            }
            return returnStr;
        }

        #endregion

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ViewResult FPReconcileHome(int type = 0, int index = 0)
        {
            base.ViewData["Type"] = type;
            base.ViewData["Index"] = index;
            return base.View();
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ViewResult FPEditReconcile(FPFapiaoFilterModel filter)
        {
            base.ViewData["TableID"] = filter.MTableID;
            base.ViewData["MFapiaoCategory"] = filter.MFapiaoCategory;
            return base.View();
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ViewResult FPEditVerifyStatus(string selectIds)
        {
            base.ViewData["selectIds"] = selectIds;
            base.ViewData["lang"] = ((ContextHelper.MContext.MLCID == LangCodeEnum.EN_US) ? "en" : ((ContextHelper.MContext.MLCID == LangCodeEnum.ZH_CN) ? "zh-cn" : "zh-tw"));
            return base.View();
        }

        public ViewResult EditFastCode()
        {
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ViewResult FPTransactionDetail(FPFapiaoFilterModel filter)
        {
            base.ViewData["ImportID"] = filter.MImportID;
            base.ViewData["MFapiaoCategory"] = filter.MFapiaoCategory;
            base.ViewData["OrgVersion"] = filter.OrgVersion;
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetReconcileList(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetReconcileList(filter, null));
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetStatementList(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetStatementList(filter, null));
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetTransactionList(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetTransactionList(filter, null));
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetFaPiaoVerifyList(FPFapiaoFilterModel filter)
        {
            List<FPFapiaoModel> resultData = _fapiao.GetFapiaoListByFilter(filter, null).ResultData;
            resultData.Insert(0, new FPFapiaoModel
            {
                MID = ""
            });
            return base.Json(resultData);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetCodingList(FPFapiaoFilterModel filter)
        {
            return base.Json(null);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetFapiaoLogList(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetFapiaoLogList(filter, null));
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetFapiaoImportDetail(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetTransactionList(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SaveReconcile(FPFapiaoReconcileModel model)
        {
            return base.Json(_fapiao.SaveReconcile(model, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult RemoveReconcile(FPFapiaoReconcileModel model)
        {
            return base.Json(_fapiao.RemoveReconcile(model, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SetReconcileStatus(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.SetReconcileStatus(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult GetCodingPageList(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.GetCodingPageList(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SaveCodingStatus(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.SaveCodingStatus(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SaveCoding(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.SaveCoding(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult ResetCodingData(FPFapiaoFilterModel filter)
        {
            return base.Json(_fapiao.ResetCodingData(filter, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SaveCodingRow(List<FPCodingModel> data)
        {
            return base.Json(_fapiao.SaveCodingRow(data, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult DeleteCodingRow(FPCodingModel row)
        {
            return base.Json(_fapiao.DeleteCodingRow(row, null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult GetCodingSetting()
        {
            return base.Json(_fapiao.GetCodingSetting(null));
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public ActionResult SaveCodingSetting(FPCodingSettingModel model)
        {
            return base.Json(_fapiao.SaveCodingSetting(model, null));
        }

        public ActionResult GetBaseData()
        {
            return base.Json(_fapiao.GetBaseData(null));
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ViewResult FPDashboard()
        {
            return base.View();
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetDashboardData(int type)
        {
            return base.Json(null);
        }

        [Permission("Sales_Fapiao", "View", OperateTypeEnum.OR, "Purchases_Fapiao", "View", "")]
        public ActionResult GetDashboardTableData()
        {
            MActionResult<string> dashboardTableData = _table.GetDashboardTableData(null);
            return base.Json(dashboardTableData);
        }

        [Permission("Sales_Fapiao", "Change", OperateTypeEnum.OR, "Purchases_Fapiao", "Change", "")]
        public FileResult Export(string jsonParam)
        {
            FPFapiaoFilterModel fPFapiaoFilterModel = ReportParameterHelper.DeserializeObject<FPFapiaoFilterModel>(jsonParam);
            BizReportType reportType = (fPFapiaoFilterModel.MFapiaoCategory == 0) ? BizReportType.OutputInvoice : BizReportType.InputInvoice;
            string arg = (fPFapiaoFilterModel.MFapiaoCategory == 0) ? HtmlLang.GetText(LangModule.FP, "SalesFapiao", "销项发票") : HtmlLang.GetText(LangModule.FP, "IncomesFaPiao", "进项发票");
            ReportModel reportModel = ReportStorageHelper.CreateReportModel(reportType, jsonParam, CreateReportModelSource.Export, null, 0.ToString(), null);
            Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
            string exportName = $"{reportModel.OrgName}-{arg}.xls";
            return base.ExportReport(stream, exportName);
        }
    }
}
