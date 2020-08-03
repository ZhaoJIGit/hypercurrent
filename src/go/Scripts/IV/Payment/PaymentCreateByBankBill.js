
var PaymentEdit = {
    init: function () {
        PaymentEditBase.initPaymentModel = PaymentEdit.getModel();

        $("#aSavePayment").click(function () {
            PaymentEditBase.savePayment(function (msg) {
                parent.BDBankReconcileEdit.afterNewPR(msg.ObjectID);
                Megi.closeDialog();
            },false);
        });
    },
    getModel: function () {
        var msg = mText.getObject("#hidPaymentModel");
        if (msg.MType != null) {
            $('#selType').combobox('setValue', msg.MType);
        }
        $("#txtRef").val(msg.MReference);
        var cyId = $("#hidAccountCurrencyID").val();
        if (PaymentEditBase.PaymentID.length == 0) {
            var bizDate = String($.mDate.format(Megi.request("date")));
            //$('#txtDate').datebox('setValue', bizDate);
            $('#selContact').combobox('setValue', Megi.request("cttId"));

            IVEditBase.init({
                IVType: IVEditBase.IVType.Purchase, //类型，主要是判断是销售还是采购
                id: PaymentEditBase.PaymentID, //单据ID
                oToLRate: msg.MOToLRate,  //原币到本位币的汇率
                lToORate: msg.MLToORate, //本位币到原币的汇率
                verificationList: msg.Verification, //核销列表
                entryList: PaymentEdit.getEntryList(), //分录列表
                cyId: cyId, //币别
                taxId: msg.MTaxID, //税率类型
                disabled: false, //是否禁止编辑
                currencyDisabled: true, //币别是否允许编辑
                type: msg.MType, //单据类型
                status: null //单据状态
            });

        } else {
            $('#txtDate').datebox('setValue', $.mDate.format(msg.MBizDate));
            $('#selContact').combobox('setValue', msg.MContactID);

            IVEditBase.init({
                IVType: IVEditBase.IVType.Purchase, //类型，主要是判断是销售还是采购
                id: PaymentEditBase.PaymentID, //单据ID
                oToLRate: msg.MOToLRate,  //原币到本位币的汇率
                lToORate: msg.MLToORate, //本位币到原币的汇率
                verificationList: msg.Verification, //核销列表
                entryList: msg.PaymentEntry, //分录列表
                cyId: cyId, //币别
                taxId: msg.MTaxID, //税率类型
                disabled: false, //是否禁止编辑
                currencyDisabled: true, //币别是否允许编辑
                type: msg.MType, //单据类型
                status: null, //单据状态
                bizDate: $('#txtDate').datebox('getValue')  //单据日期
            });

        }
        if ($("#hidEditType").val() == "1" || $("#hidEditType").val() == "2") {
            Megi.displaySuccess("#divMessage", "Save successfully!");
        }
    },
    getEntryList: function () {
        var arr = new Array();
        var amt = Megi.request("amt");
        var desc = Megi.request("desc");
        if (desc == "null") {
            desc = "";
        }
        amt = Megi.Math.toDecimal(amt, 2);
        var obj = {};
        obj.MEntryID = "";
        obj.MItemID = "";
        obj.MIsNew = true;
        obj.MDesc = decodeURI(desc);
        obj.MQty = "1";
        obj.MPrice = amt;
        obj.MDiscount = "";
        obj.MAcctID = "";
        obj.MTaxID = "";
        obj.MAmountFor = amt;
        obj.MAmount = "";
        obj.MTaxAmountFor = amt;
        obj.MTaxAmount = "";
        obj.MTaxAmtFor = 0;
        obj.MTaxAmt = 0;
        obj.MTrackItem1 = "";
        obj.MTrackItem2 = "";
        obj.MTrackItem3 = "";
        obj.MTrackItem4 = "";
        obj.MTrackItem5 = "";
        arr.push(obj);
        return arr;
    }
}
//初始化页面
$(document).ready(function () {
    PaymentEdit.init();
});

