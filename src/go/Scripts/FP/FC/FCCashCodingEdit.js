var FCCashCodingEdit = {
    //跟踪项数据源
    TrackDataSource: null,
    //联系人数据源
    ContactDataSource: null,
    //税率数据源
    TaxRateDataSource: null,
    //科目数据源
    AccountDataSource: [],
    init: function () {
        //获取跟踪项
        FCCashCodingEdit.TrackDataSource = mData.getNameValueTrackDataList();
        //获取联系人
        var contact = mText.getObject("#hidContacts");
        FCCashCodingEdit.ContactDataSource = eval(contact);
        //获取税率
        FCCashCodingEdit.TaxRateDataSource = mData.getTaxRateList();

        FCCashCodingEdit.initAction();
        FCCashCodingEdit.initForm();
    },
    initAction: function () {
        //保存
        $("#aSave").click(function () {
            FCCashCodingEdit.saveCashCodingEdit();
        });
    },
    initForm: function () {
        //初始化跟踪项
        FCCashCodingEdit.initTrack();
        //数据绑定到联系人combox
        FCCashCodingEdit.loadContactData($("#seleContact"));
        //数据绑定到税率combox
        FCCashCodingEdit.loadTaxRateData($("#seleTaxRate"));
        //数据绑定到科目combox
        FCCashCodingEdit.loadAccountData($("#seleAccount"));
        //获取mid的值
        var mId = $("#hidMID").val();
        //如果ID不为空，则去取数据。
        if (mId) {
            $("body").mFormGet({
                url: "/FC/FCCashCodingModule/GetFCCashCodingModel?mid=" + mId + ""
            });
        }
        if (!$("#haveGLPermission").val()) {
            $(".m-track1").css("padding-left", "0");
        }
    },
    initTrack: function () {
        var ds = FCCashCodingEdit.TrackDataSource;
        var trackCount = ds.length;
        //跟踪项有数据
        if (ds != null && trackCount > 0) {
            for (var i = 0; i < trackCount; i++) {
                //界面显示跟踪项
                $(".m-track" + (i + 1)).css('display', 'inline-block');
                //显示跟踪项标题
                var titleName = mText.encode(ds[i].MName);
                $(".m-track-title" + (i + 1)).html(titleName);
                //获取子项
                var mChildrens = ds[i].MChildren;
                //跟踪项数据源
                var cbDs = new Array();
                for (var j = 0; j < mChildrens.length; j++) {
                    //设置启用状态的数值
                    if (mChildrens[j].MValue1 === "1") {
                        cbDs.push(mChildrens[j]);
                    }
                }
                //绑定跟踪项数据源
                $("#seleTrack" + (i + 1)).combobox("loadData", cbDs);
            }
        }
    },
    getContactDataSource: function () {
        var arr = new Array();
        var contactList = FCCashCodingEdit.ContactDataSource;
        if (contactList == null || contactList.length == 0) {
            return arr;
        }
        return contactList;
    },
    loadContactData: function (target) {
        var ds = FCCashCodingEdit.getContactDataSource();
        $(target).combobox("loadData", ds);
    },
    getTaxRateDataSource: function () {
        var arr = new Array();
        var taxRateList = FCCashCodingEdit.TaxRateDataSource;
        if (taxRateList == null || taxRateList.length == 0) {
            return arr;
        }
        return taxRateList;
    },
    loadTaxRateData: function (target) {
        var ds = FCCashCodingEdit.getTaxRateDataSource();
        $(target).combobox("loadData", ds);
    },
    getAccountData: function (callback) {
        mAjax.post("/BD/BDAccount/GetAccountListIncludeBalance", {
            isIncludeParent: false,
            needFullName: true
        },
        function (data) {
            FCCashCodingEdit.AccountDataSource = data || [];
            callback && $.isFunction(callback) && callback(FCCashCodingEdit.AccountDataSource);
        });
    },
    loadAccountData: function (target) {
        FCCashCodingEdit.getAccountData(function () {

            var ds = FCCashCodingEdit.AccountDataSource;
            $(target).combobox("loadData", ds);
        });
    },
    saveCashCodingEdit: function () {
        $("body").mFormSubmit({
            url: "/FC/FCCashCodingModule/UpdateCashCodingModule", callback: function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "Save Successful！"));
                    $.mDialog.close(0, msg.Data.ObjectID);

                } else {
                    $.mDialog.alert(msg.Data.ErrorMessageDetail);
                }
            }
        });
    }
}
$(document).ready(function () {
    FCCashCodingEdit.init();
});