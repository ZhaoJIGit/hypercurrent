(function () {

    mPublish = (function () {

        var notictTime = new Date(2019, 3, 18, 08, 0, 0);
        var startTime = new Date(2019, 3, 18, 21, 0, 0);
        var endTime = new Date(2019, 3, 18, 23, 59, 0);
        var publish = true;

        mPublish = function () {

            var that = this;

            this.show = function (message) {
                $(".m-notice").remove();

                $("body").append("<div class='m-notice'>" + message + "</div>");
                var bodyW = $("body").width();
                var msgW = $(".m-notice").width();
                $(".m-notice").css("left", (bodyW - msgW) / 2 + "px");
            }

            this.init = function () {

                window.setInterval(function () {

                    var now = new Date();

                    var orgName = top.window.$("#aOrgList").text();
                    var distinctOrgNames = [
                        "发版测试0416",
                        "基础资料优化-大数据测试组织01",
                        "斯迪迈（苏州）医疗科技有限公司",
                        "合得软件（上海）有限公司",
                        "上海仁园农业科技有限公司",
                        "上海开垣贸易有限公司",
                        "凌敏可（上海）工业科技有限公司",
                        "麦斯马汀（上海）贸易有限公司 Master Martini (Shanghai) Trade Co.",
                        "Beta组织_标准版_企业会计准则",
                        "Beta组织_标准版_小企业会计准则",
                        "Beta组织_总账版_企业会计准则",
                        "Beta组织_总账版_小企业会计准则",
                        "祁辉商务咨询服务（上海）有限公司",
                        "翱奢（上海）贸易有限公司",
                        "上海开功市场营销策划中心",
                        "上海中垚科技发展有限公司",
                        "佑卓（上海）化妆品有限公司",
                        "傲卓（上海）化妆品有限公司",
                        "上海释壹投资管理合伙企业（有限合伙）",
                        "很特（上海）商贸有限公司",
                        "韩思维（上海）投资管理有限公司"];

                    if (!publish || (distinctOrgNames && distinctOrgNames.length > 0 && !distinctOrgNames.contains(orgName))) return;

                    if (now < notictTime) return;

                    var message = "";

                    //发版之前
                    if (now >= notictTime && now < startTime) {

                        //如果是当天
                        if (now.getDate() === startTime.getDate()) {
                            message = HtmlLang.Write(LangModule.Common, "SystemUpdatePromt", "System will be update between {0} and {1} ").format(startTime.toTimeString().substring(0, 5), endTime.toTimeString().substring(0, 5));
                        }
                        else {
                            message = HtmlLang.Write(LangModule.Common, "SystemUpdatePromtDay", "Notice：the system will be upgrade between {1} to {2} on {0}, please do not use it during this period. ").format(mDate.format(endTime, "yyyy-MM-dd"), startTime.toTimeString().substring(0, 5), endTime.toTimeString().substring(0, 5));
                        }

                    }
                        //发版之中
                    else if (now >= startTime && now <= endTime) {

                        message = HtmlLang.Write(LangModule.Common, "SystemUpdating", "System is updating...");


                    }
                        //发版之后,30分钟提醒升级完成
                    else if (now > endTime && now < endTime.addMinutes(30)) {

                        message = HtmlLang.Write(LangModule.Common, "SystemUpgradeCompleted", "System upgrade completed!");
                    }

                    !!message && that.show(message);
                }, 10000);
            }
        }

        return mPublish;
    })()

    window.mPublish = mPublish;
})()

$(function () {
    new mPublish().init();
})