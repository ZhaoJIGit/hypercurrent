var SystemUpdate = {
	newVersion: 24,
	updateDate: "2019/03/28 21:30:00",
	languageId: $("#hideLocaleId").val(),
	fileDownUrl: $("#aGoServer").val() + "/FileDownload/SystemUpdateInfo?fileName=systemupdateinfo.zip&fileType=octet-stream",
	init: function () {

	    var isShow = $.cookie('showupdateinfo');
	    var version = $.cookie('systemversion');
	    if (isShow && isShow == "0" && version && parseInt(version)== parseInt(SystemUpdate.newVersion)) {
	        return;
	    }
		return;

	    var userCreateDate = $("#hideUserCreateDate").val();


	    //如果账号创建日期大于更新日期，不提示
	    var updateDateTime = new Date(SystemUpdate.updateDate.replace(/^(\d{4})(\d{2})(\d{2})$/, "$1/$2/$3"));
	    var userCreateDateTime = new Date(userCreateDate.replace(/^(\d{4})(\d{2})(\d{2})$/, "$1/$2/$3"));
	    if (updateDateTime < userCreateDateTime) {
	        return;
	    }
	    var currentDate = new Date();  
	    if (currentDate < updateDateTime) {
	        return;
	    }

	    if (!SystemUpdate.languageId) {
	        SystemUpdate.languageId = "0x0009";
	    }

	    var tipsMessageList = SystemUpdate.getTipsMessageList();
	    //弹框
	    SystemUpdate.popup(tipsMessageList);

	},
    //这里是升级的提示信息，每次发版后直接修改
	getTipsMessageList: function () {
	    var tipsMessageList = new Array();
	    if (SystemUpdate.languageId == "0x0009") {
            tipsMessageList.push("1.Optimized bank reconciliation function. Increase display of bank transactions per screen. Searching and bulk reconciliation is available now.");
            tipsMessageList.push("2.Optimized explanation on journal entry generation setting. Tick box of default explanation item can be cancelled if need.");
            tipsMessageList.push("3.Added New accounts for China Enterprise Accounting Standards, including 1481 Holdings of Assets for Sale, 1482 Reserve for Holdings of Assets for Sale Impairment, 2245 Holdings of Liabilities for Sale, 2502.01Preferred Stock, 2502.02 Perpetual Debt, 4003 Perpetual Debt Other Consolidated Benefits, 4401 Other Equity Tools, 4401.01 Preferred Stock, 4401.02 Perpetual Debt, 6112 Other Benefits.");
            tipsMessageList.push("4.Updated Financial Statements of China Enterprise Accounting Standards, including Balance Sheet, Profit and Loss.");
	       
	        tipsMessageList.push("");
	        //tipsMessageList.push("Details of the upgrade, please click <a target='_blank' href='" + SystemUpdate.fileDownUrl + "' style='font-size:15px'>here</a> to download ");
	    } else {
            tipsMessageList.push("1. 优化银行自动勾对功能，增多界面显示的数据量，且支持搜索及批量勾对；");
            tipsMessageList.push("2. 优化总账凭证的摘要生成规则，可以取消默认摘要选项；");
            tipsMessageList.push("3. 新增企业会计准则下的会计科目：1481持有待售资产、1482持有待售资产减值准备、2245持有待售负债、2502.01优先股、2502.02永续债、4003其他综合收益、4401其他权益工具、4401.01优先股、4401.02永续债、6112其他收益；");
            tipsMessageList.push("4. 更新企业会计准则下的财务报表格式，包括资产负债表、利润表。");
	       
	        tipsMessageList.push("");
	        //tipsMessageList.push("详细升级信息，请点击 <a target='_blank' href='" + SystemUpdate.fileDownUrl + "' style='font-size:15px'>这里</a>下载");

	    }

	    return tipsMessageList;

	},
	popup: function (tipsList) {
	    if (!tipsList || tipsList.length == 0) {
	        return;
	    }
	    var popupHtml = "<div id='divSystemUpdate' class='system-update' style='position: fixed; z-index: 9999999; padding: 0px; margin: 0px; height:420px;width:550px;background: #fff;border: solid 1px #99b1c4;'>";
	    popupHtml += "<div class='popup_title' style=''></div>"
	    popupHtml += "<div style='padding-left:20px;font-size:24px;font-weight:normal'>" + HtmlLang.Write(LangModule.Common, "SystemUpdateFinded", "美记系统已完成升级") + "</div>"

	    var updateInfo = HtmlLang.Write(LangModule.Common, "SystemUpdateInfo", "{0}系统升级，我们对美记系统做了以下优化：").replace("{0}", $.mDate.format(SystemUpdate.updateDate, "YYYY-MM-DD"));
	    popupHtml += "<div style='font-size:15px;font-weight:normal;padding-left: 18px; padding-top: 10px;'>" + updateInfo + "</div>";
	    popupHtml += "<div style='padding-left:20px;font-size:15px;width: 95%;height:70%;overflow: auto;'>";
	  
	    popupHtml += "<div>";
	    for (var i = 0 ; i < tipsList.length - 1; i++) {
	        var tips = tipsList[i];
	        popupHtml += "<li style='font-size:15px;margin-top: 10px;font-weight:normal'>" + tips + "</li>";

	    }
	    popupHtml += "</div>";
	    popupHtml += "</div>";
	    popupHtml += "<div style='padding-left: 20px;font-size:15px;font-weight:normal;margin-top:10px'>" + tipsList[tipsList.length - 1] + "</div>";
	    popupHtml += "<div style='padding-right:15px'><a  id='btnSystemUpdateClose' href='###' class='easyui-linkbutton easyui-linkbutton-yellow' style='width: 65px;height: 24px;float:right;text-align: center'>" + HtmlLang.Write(LangModule.Common, "IKnow", "我知道了") + "</a></div>"
	    popupHtml += "</div>";


	    var maskDiv = '<div id="su_popup_overlay" style="position: absolute; z-index: 9999998; top: 0px; left: 0px; width: 100%; height: 100%; background: rgb(0, 0, 0); opacity: 0.5;"></div>';

	    $("body").append(maskDiv);

	    $("body").append(popupHtml);


	    //计算，使弹框居中

	    var bodyDom = $("body");
	    var divSystemUpdateDom = $("#divSystemUpdate");

	    var left = (bodyDom.width() - divSystemUpdateDom.width()) / 2;

	    var top = (bodyDom.height() - divSystemUpdateDom.height()) / 2;

	    divSystemUpdateDom.css("left", left);

	    divSystemUpdateDom.css("top", top);

	    $("#btnSystemUpdateClose").off("click").on("click", SystemUpdate.closeEvent);
	},
	closeEvent: function () {
	    var domain = $("#hidDomain").val();
	    $.cookie('showupdateinfo', '0', { domain: "." + domain, expires: 1000 });
	    $.cookie("systemversion", SystemUpdate.newVersion, { domain: domain, expires: 1000 });
	    $("#divSystemUpdate").remove();

	    $("#su_popup_overlay").remove();
	}
}