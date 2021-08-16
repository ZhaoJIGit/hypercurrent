using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Go.Web.VIewModels
{
    public class BaseModel
    {
        public string method { get; set; }
        public string requestId { get; set; }
        public ErrorResponse errorResponse { get; set; }
        public bool success { get; set; }
    }
    public class TokenModel : BaseModel
    {
        public Response response { get; set; }
    }
    public class ErrorResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string subCode { get; set; }
        public string subMessage { get; set; }
    }
    public class Response
    {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }
    public class BWInvoiceRequestModel
    {
        public bool isSplit { get; set; } = false;
        public BWInvoiceRequestData data { get; set; } = new BWInvoiceRequestData();
        public string orgCode { get; set; } = "";
        public string invoiceTerminalCode { get; set; } = "zpj";
        public string taxNo { get; set; } = "36996300000000011";
        public string taxDiskNo { get; set; } = "";
        public bool formatGenerate { get; set; } = false;
    }

    public class BWInvoiceRequestData
    { 
        //系统中均为含税金额 0 不含税，1含税
        public string priceTaxMark { get; set; } = "0";
        public string buyerBankAccount { get; set; } = "";
        public string invoiceTotalTax { get; set; } = "";
        public string checker { get; set; } = "";
        public string sellerAddressPhone { get; set; } = "";
        /// <summary>
        /// 征税方式， 0：普通征税；2：差额征税（默认是0普通征税）
        /// </summary>
        public string taxationMethod { get; set; } = "0";
        public string redInfoNo { get; set; } = "";
        public string payee { get; set; } = "";
        public string systemName { get; set; } = "";
        /// <summary>
        /// 开票类型 0:正数发票（蓝票） 1：负数发票（红票）默认0
        /// </summary>
        public string invoiceType { get; set; } = "0";
        public string buyerAddressPhone { get; set; } = "";
        public object ext { get; set; } = new { };

        public string systemId { get; set; } = "";
        public string buyerTaxNo { get; set; } = "";//91510106MA6CCY6B5Y
        /// <summary>
        /// 扣除额， taxationMode=2，差额征税时必传。数值必须小于价税合计。
        /// </summary>
        public string deductibleAmount { get; set; } = "";
        public string drawer { get; set; } = "";
        public string invoiceSpecialMark { get; set; } = "";
        public string buyerName { get; set; } = "";//成都途锦扬帆文化传播有限公司
        public string originalInvoiceNo { get; set; } = "";
        /// <summary>
        /// 0：无清单；1：带清单（专普票发票明细大于等于8行必须带清单）：大于8行必须为清单票(电子票只能为非请单票)（默认是0无清单）
        /// </summary>
        public string invoiceListMark { get; set; } = "0";
        public string invoiceTotalPriceTax { get; set; } = "";
        public string serialNo { get; set; } = Guid.NewGuid().ToString();
        public string invoiceTotalPrice { get; set; } = "";
        public string sellerBankAccount { get; set; } = "";
        public List<InvoiceDetail> invoiceDetailsList { get; set; } = new List<InvoiceDetail> { new InvoiceDetail() };
        /// <summary>
        /// 发票种类编码, 004：增值税专用发票；007：增值税普通发票；026：增值税电子发票；025：增值税卷式发票；028:增值税电子专用发票
        /// </summary>
        public string invoiceTypeCode { get; set; } = "004";
        public string remarks { get; set; } = "";
        public string originalInvoiceCode { get; set; } = "";
    }
    public class InvoiceDetail
    {
        public decimal goodsTaxRate { get; set; } = 0.03M;
        /// <summary>
        /// 发票行性质， 0：正常行；1：折扣行；2：被折扣行，默认为0
        /// </summary>
        public string invoiceLineNature { get; set; } = "0";
        public string goodsTotalPrice { get; set; } = "2.0";
        public string preferentialMarkFlag { get; set; } = "0";
        public string goodsPersonalCode { get; set; } = "";
        public string goodsSpecification { get; set; } = "";
        public string goodsPrice { get; set; } = "";
        /// <summary>
        /// 零税率标识， 空代表无； 1：出口免税和其他免税优惠政策； 2：不征增值税；3：普通零税率
        /// </summary>
        public string freeTaxMark { get; set; } = "";
        public string goodsQuantity { get; set; } = "";
        public string goodsUnit { get; set; } = "";
        public string goodsTotalTax { get; set; } = "";
        /// <summary>
        /// 为空则需要  手动百望云维护商品
        /// </summary>
        public string goodsCode { get; set; } = "";//"1010101070000000000"; //这个字段传19位税收分类编码的话不需要百望云上维护商品
        public string goodsName { get; set; } = "四日面板";
        public int goodsLineNo { get; set; } = 1;

        public string vatSpecialManagement { get; set; } = "";
    }
    public class BWInvoiceResponseModel : BaseModel
    {
        public InvoiceResponse response { get; set; }
    }
    public class InvoiceResponse
    {
        public List<InvoiceResponsFail> fail { get; set; }

        public List<InvoiceResponsSuccess> success { get; set; }
    }
    public class InvoiceResponsSuccess
    {
        public string eInvoiceUrl { get; set; }
        public string invoiceTotalTax { get; set; }
        public string invoiceDate { get; set; }//":"20190612110126",
        public string invoiceCode { get; set; }//":"015123456789",
        public string invoiceTotalPriceTax { get; set; }//":0,
        public string serialNo { get; set; }//{ get; set; },
        public string taxControlCode { get; set; }//":"037-+9<56<>-0570/<+583/*9920545/4887/996/*96*384*/<--*12>>400*7<0/28>873+*68296*74>3723>681167010<0706367>1<612-",
        public string invoiceCheckCode { get; set; }//":"08566901911295514678",
        public string invoiceQrCode { get; set; }//":"iVBORw0KGgoAAAANSUhEUgAAAJQAAACUCAIAAAD6XpeDAAADCUlEQVR42u3cy24CMQwF0Pn/n6bbbiqVsa+dgZMlAprkBI0fUa8rMF6/xl+vV97z7t/qmn9lLddTBjx48OB9Cd6rMCqT7oKsrHFyH7q+Hx48ePDg9eB1bXrXYioY6Tmkgyl48ODBg/cMvApAV3A0eSAmAzp48ODBg/eMgKUSXCSCncnkGh48ePDgnYXXNbl0AzYxTihKHNFVgAcPHrwPxpu8gPTNrx89IMGDB++D8V6Do2s+k+s9eW/hwYMHD979uaUTw66GajrhTRSdEwVrePDgwYP33p6ni8WnJfVdB7QrECt9Fh48ePDg3cabXPwJAdQkWGS98ODBgwevfb2J5DRxESi9rsQFrXhTFx48ePDgRRPbrWZs4kQnDlZbQxgePHjw4N3O87oewl2bsrYRg8X0tkIBPHjw4MFrx0ssLB2wJA5oIvEvFSXgwYMHD95Rhel0I3Tr8J1QTIAHDx48eP25XbqYm7jkc9pFpjZ4ePDgwYPXnrCnX09/duuyU2IO8ODBgwfvPl7X5BLJbCLoSBcZEgX0K32y4MGDB+8L8bowRpuNgeLAVuL/9nzgwYMHD15HPzLyz60Tm5UOXiqF+8SPAR48ePDgPSNgOblJmz7QiUY3PHjw4MG7j5cu1CYe/omAKF3IPu6iETx48ODBWw8o0k3gBHDX5ajIRsCDBw8evHaA9HcmAqutYKQECQ8ePHjwog3YtYfzUsG6st5EEfxfjVl48ODBg3f7WT4ZFFQamOn3JA5Wej7w4MGDB28uz0t/dvI9iYJ4248EHjx48ODdxmuLdgrJ71YCnihGxwNDePDgwYP3VjE6MRLY6abu5GEt7QM8ePDgweveh7YHb+Ihv3U4ug7KddqABw8evC/B6wpMuh7IicJ3ImBJFKZLzVh48ODBg9fSjE0kwluBQ6KRmw76/lWkhgcPHjx463iVzZoMrNIXpSIHGh48ePDgreMl3rO1QVuFAnjw4MGD94zC9GRwccKGds3niGYsPHjw4H0w3tYFpMngJZ2kJ34Yo6jw4MGD97l4PxCtu/dLsRmSAAAAAElFTkSuQmCC",
        public string invoiceTotalPrice { get; set; }//":3.0,
        public List<ResponsInvoiceDetail> invoiceDetailsList { get; set; }//
        public string invoiceTypeCode { get; set; }//":"007",
        public string invoiceNo { get; set; }//":"00900554"
    }
    public class InvoiceResponsFail
    {
        public string invoiceTotalPrice { get; set; }
        public string invoiceTotalTax { get; set; }
        public List<ResponsInvoiceDetail> invoiceDetailsList { get; set; }
        public string invoiceTotalPriceTax { get; set; }
        public string serialNo { get; set; }
    }
    public class ResponsInvoiceDetail
    {

        public string priceTaxMark { get; set; }
        public string goodsTaxRate { get; set; }
        public string invoiceLineNature { get; set; }
        public string goodsTotalPrice { get; set; }
        public string goodsSpecification { get; set; }
        public string goodsPrice { get; set; }
        public string freeTaxMark { get; set; }
        public string goodsQuantity { get; set; }
        public string goodsUnit { get; set; }
        public string goodsTotalTax { get; set; }
        public string goodsCode { get; set; }
        public string preferentialMark { get; set; }
        public string goodsName { get; set; }
        public string goodsLineNo { get; set; }
        public string vatSpecialManagement { get; set; }
    }


    public class GoodResponseModel : BaseModel
    {
        public GoodResponse response { get; set; }

    }
    public class GoodResponse
    {

        public string isAvailable { get; set; }
        public string priceTaxMark { get; set; }
        public string illustrateStr { get; set; }
        public string isValid { get; set; }
        public decimal vatRate { get; set; }
        public string freeType { get; set; }
        public string summarized { get; set; }
        public string updateTime { get; set; }
        public string version { get; set; }
        public string taxProv { get; set; }
        public string goodsSimpleName { get; set; }
        public string useDiscount { get; set; }
        public string parentCode { get; set; }
        public string rate { get; set; }
        public string goodsSpecification { get; set; }
        public decimal goodsPrice { get; set; }
        public string keyWorld { get; set; }
        public string customGoodsCode { get; set; }
        public string goodsUnit { get; set; }
        public string discountType { get; set; }
        public string goodsCode { get; set; }
        public string simpleCode { get; set; }
        public string goodsName { get; set; }
    }
}