if ($("#hidDomain").val()) {
    document.domain = $("#hidDomain").val();
}

var Index = {
    tabswitch: new BrowserTabSwitch(),
    //新建联系人
    newContact: function () {
        $.mDialog.show({
            mTitle: HtmlLang.Write(LangModule.Contact, "AddContact", "Add Contact"),
            mContent: 'iframe:/BD/Contacts/ContactsEdit?Origin=Quick_Menu',
            mWidth: 1100,
            mHeight: 450,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle"
        });
    },
    //添加商品
    newInventoryItem: function () {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "NewInventoryItem", "New Inventory Item"),
            width: 620,
            height: 500,
            href: '/BD/Item/ItemEdit?Origin=Quick_Menu'
        });
    },
    newTransactionTab: function (type, bankId) {
        var isPayment = type == "Payment";//Receive
        var title = isPayment ? HtmlLang.Write(LangModule.Bank, "NewSpendMoney", "New Spend Money") : HtmlLang.Write(LangModule.Bank, "NewReceiveMoney", "New Receive Money");
        var url = isPayment ? "/IV/Payment/PaymentEdit?Origin=Quick_Menu" : "/IV/Receipt/ReceiptEdit?Origin=Quick_Menu";
        $.mTab.addOrUpdate(title, url + "&acctid=" + bankId, true);
    },
    newTransaction: function (type) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "SelectBank", "Select Bank Account"),
            width: 430,
            height: 270,
            href: '/BD/BDBank/SelectBank/' + type
        });
    },
    //下拉框高度自适应
    initOrgList: function () {
        var goSite = $("#aGoServer").val();
        //
        $.mAjax.Post(goSite + "/FW/FWHome/GetOrgList", "", function (msg) {
            //数据
            var data = msg.Data;

            var lastUserOrgId = msg.Context.MOrgID;

            //高度
            var height = 0;

            if (data && data.length > 0) {
                //获取当前登录组织
                var orgList = Enumerable.From(data);

                var currentOrg = orgList.Where("x=>x.MOrgID == '" + lastUserOrgId + "'").FirstOrDefault();
                var html = "<div class='m-org'><a href='###' id='aOrgList'></a></div>"
                if (currentOrg) {
                    html = "<div class='m-org'><a href='###' id='aOrgList' orgid='" + currentOrg.MOrgID + "' isPaid='" + (currentOrg.MIsPaid ? 1 : 0) + "'>" + mText.encode(currentOrg.MOrgName) + "</a></div>";
                }


                var unCurrentOrgList = orgList.Where("x=>x.MOrgID != '" + lastUserOrgId + "'").ToArray();

                html += "<div id='divOrgList' class='m-pop-box m-pop-menu'><b class='popup-arrow'></b>";
                html += "<div class='item-list'>";
                //先清空
                $(".item-list", "#divOrgList").empty();


                html += "<p><a href='###' onclick ='Index.navToMySite()'>" + HtmlLang.Write(LangModule.My, "MyMegi", "My Hypercurrent") + "</a></p>";

                for (var i = 0; i < unCurrentOrgList.length; i++) {

                    var temp = unCurrentOrgList[i];

                    var clickEvent = 'Index.onSelectOrg(\'' + temp.MOrgID + '\', \'' + temp.Url + '\',\'' + temp.MRegProgress + '\',\'' + temp.MVersionID + '\', \'' + temp.MIsBeta + '\');';


                    html += '<p><a href="###" onclick="' + clickEvent + '">' + mText.encode(temp.MOrgName) + '</a></p>';

                }
                height = 38 * data.length;
                html += "</div>";
            }
            $("#divOrgCombox").append(html);
            //重新注册一下事件
            FW.initMenu();
            //设置高度
            $(".item-list", "#divOrgList").css("height", height + "px");

            //包含组织的容器
            var itemListDiv = $(".item-list", "#divOrgList");
            //去所有的p
            var p = $(itemListDiv).find("p");

            if (p && p.length > 0) {
                var height = 38 * p.length;

                $(itemListDiv).css("height", height + "px");
            }

        }, "", true);
    },
    onSelectOrg: function (orgId, url, regProgress, versionId, isBeta) {
        var goServer = isBeta == "true" ? $("#hideBetaGoServer").val() : $("#hideFinalGoServer").val();

        Index.validateOrgCreateOrJumpAuth(versionId, function () {
            var mySite = $("#aMySite").val();

            url = mySite + "/FW/FWHome/OrgSelect?MOrgID=" + orgId + "&RedirectUrl=" + goServer + url + "&IsBeta=" + isBeta;

            if (regProgress == 15) {
                //location.href = url;
                mWindow.reload(url);
            } else {
                url = mySite + "?RedirectUrl=" + encodeURIComponent(url) + "&redirectOnload=true";
                mWindow.reload(url);
            }
        });
    },
    navToMySite: function () {
        var mySite = $("#aMySite").val();

        //跳转到my站点的连接
        var mySiteItemUrl = mySite + "/FW/FWHome/OrgSelect?MOrgID=&RedirectUrl=" + mySite;

        mWindow.reload(mySiteItemUrl);
    },
    validateOrgCreateOrJumpAuth: function (version, callback) {

        $.mAjax.Post("/FW/FWHome/ValidateCreateOrgAuth", { type: version }, function (msg) {
            if (!msg || !msg.Success) {
                var message = msg.Message;
                $.mDialog.alert(message);
            } else {
                if (callback && $.isFunction(callback)) {
                    callback(version);
                }
            }
        }, "", true);
    },
    validateBeta: function (isBeta, version, callback) {

        $.mAjax.Post("/FW/FWHome/ValidateBeta", { isBeta: isBeta }, function (msg) {
            if (msg) {
                var message = isBeta == "true" ? HtmlLang.Write(LangModule.My, "JumpBetaLoginSite", "该组织为Beta测试组织，即将跳转到Beta登录站点")
                    : HtmlLang.Write(LangModule.My, "JumpLoginSite", "该组织不是Beta组织，即将跳转到正式登录站点");
                $.mDialog.confirm(message, function () {
                    if (callback && $.isFunction(callback)) {
                        callback(version);
                    }
                });
            } else {
                if (callback && $.isFunction(callback)) {
                    callback(version);
                }
            }
        });
    },
    //弹出高级搜索
    initSearch: function () {
        $(".m-search").mTip({
            target: $(".m-search-div"),
            width: 1080,
            height: 502,
            top: 25,
            callback: function () {
                if (!$("#dataGrid").attr("inited")) {
                    new mSearch().init();
                }
            },
            parent: $(".m-search-div").parent()
        });
    },
    open: function (type) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.FA, "EnableFixAssets", "启用固定资产"),
            width: 600,
            height: 320,
            href: '/FA/FAInit/Index?type=' + type + ''
        });
    },
    //一键导出
    exportAll: function () {
        $.mDialog.confirm(HtmlLang.Write(LangModule.Common, "OneKeyExportConfirm", "所有的基础资料和历史业务单据都会被导出，是否继续？"),
            {
                callback: function () {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
                    function hideWrapper(delay) {
                        setTimeout(function () {
                            divWrapper.animate({ height: hideH }, 800);
                        }, delay);
                    };
                    var showH = 53;
                    var hideH = 11;
                    var divWrapper = $("#divExportAll");
                    if (divWrapper.is(":hidden")) {
                        divWrapper.remove();
                    }
                    if (divWrapper.is(":visible")) {
                        if (divWrapper.height() == hideH) {
                            divWrapper.animate({ height: showH }, 500);
                            hideWrapper(3000);
                        }
                        return;
                    }
                    $("<div id='divExportAll' class='export-all-wrapper' style='background-color:#F5FFE6;z-index:99990;'><iframe src='/BD/Export/Index' width='100%'></iframe></div>").appendTo("body");
                    Index.setExportAllWrapperWidth();
                    divWrapper = $("#divExportAll");

                    divWrapper.show("slow");
                    hideWrapper(5000);

                    divWrapper.hover(function () {
                        divWrapper.height(showH);
                    }, function () {
                        divWrapper.height(hideH);
                    });
                }
            });
    },
    //设置一键导出容器宽度
    setExportAllWrapperWidth: function () {
        var navW = $(".m-nav").width();
        var w = $(".m-main").width();
        $("#divExportAll").css({ "left": navW, "width": w });

        var divWrapper = $("#divExportAll");
        divWrapper.mask(HtmlLang.Write(LangModule.BD, "PreparingExportPlsWait", "Preparing to export, please wait..."));
        var maskMsg = divWrapper.find(".loadmask-msg");
        setTimeout(function () {
            if (maskMsg.width() > 0) {
                var left = (w - maskMsg.width()) / 2;
                maskMsg.css({ "top": 15, "left": left });
            }
        }, 380);
    },
    initRedirectUrl: function () {
        var url = Megi.request("redirectUrl");
        if (url != undefined && url != "") {
            var tabTitle = HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice");
            $.mTab.addOrUpdate(tabTitle, url, true);
        }
    },

    //点击马上支付弹出提示框
    PayNow: function () {
        //问题1.怎么使用这种图片的弹框
        //    2.弹框的内容肯定是多语言
        //    3.联系我们的地址是多少   页面的为空
        //
        $.mDialog.warning(HtmlLang.Write(LangModule.Common, "AreYouSureToUnAuditToDraft", "Are you sure to UnAudit To Draft?"));
    },

    initDashboardURL: function () {
        var url = $("div[data-index='0']").attr("data-url");

        url = Index.tabswitch.intercept(url);

        $("div[data-index='0']").attr("data-url", url);

        setTimeout(function () {
            $("#dashboard").addClass("m-page-loading");
            var dashboardUrl = $("#hideDashboardUrl").val();

            dashboardUrl = Index.tabswitch.intercept(url);

            $("#dashboard")[0].src = dashboardUrl;
        }, 100);
    },
    bindAction: function () {
        $("#aLogout").off("click").on("click", function () {

            var url = $(this).attr("href");
            mWindow.reload(url);
            window.mFilter.doFilter('track', ['Logout', {}]);
        });
    }
}

$(document).ready(function () {

    Index.tabswitch.initSessionStorage();

    Index.tabswitch.bindSwitchEvent(function () {
        top.accessRequest(function () {

        }, false);
    });
});

//设置一个定时器，每隔1分钟去请求后台判断用户是否过期,目前只有my站点和login站点需要
$(function () {


    Index.initOrgList();

    Index.initSearch();

    Index.initDashboardURL();

    SystemUpdate.init();

    $(window).resize(function () {
        Index.setExportAllWrapperWidth();
    });

    Index.initRedirectUrl();
})
//登录完成后的回调函数集合
window.loginCallback = [];