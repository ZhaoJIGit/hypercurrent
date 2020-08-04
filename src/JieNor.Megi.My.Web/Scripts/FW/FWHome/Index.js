/// <reference path="../../../intellisense/jquery.megi.common.js" />

var MyIndex = {
    dataSource: null,
    result: null,  //返回数据的结果
    iframeUrl: $("#redirectOnLoad").val() === 'false' ? '' : $("#iframeUrl").val(),
    //是否有权限修改或者删除
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    orgListShowType: null,
    tabswitch: new BrowserTabSwitch(),
    init: function () {

        MyIndex.tabswitch.initSessionStorage();
        $("body").mask("");

        MyIndex.orgListShowType = $("#hideOrgListShowType").val();

        MyIndex.initOrgList();
        MyIndex.bindAction();
        MyIndex.initGrid();
        MyIndex.initSetup();

    },
    initPageWizard: function () {
        var steps = MyIndex.getPageWizardSteps();
        var pagewizard = new Pagewizard(steps);

        pagewizard.init(steps);
    },
    //获取提示向导
    getPageWizardSteps: function () {
        var steps = [{
            popup: {
                type: 'modal',
                width: 500,
                height: 280,
                titleimageclass: "my-firststep",
                titletext: "欢迎来到美记",
                detailtext: ["这里有些小提示，可以帮您更有效的使用美记，我们建议您看看"],
                textalign: "center"
            }
        }, {
            wrapper: '#aCreateDemoCompany',
            popup: {
                type: 'tooltip',
                position: 'top',
                offsetHorizontal: 60,
                offsetVertical: -40,
                width: 360,
                height: 160,
                titletext: "试用演示公司",
                detailtext: ["如果您对美记系统不太熟悉，可以使用演示公司试用美记的所有模块"],
                textalign: "left"
            }
        }, {
            wrapper: '#aAddOrganisationTop',
            popup: {
                type: 'tooltip',
                position: 'right',
                titletext: "增加新的组织",
                width: 360,
                height: 170,
                detailtext: ["从这里选择您需要新增的组织版本", "-标准版", "-总账版"],
                textalign: "left"
            },
            onEnter: function () {
                $('.m-btn-line:visible').trigger('mouseenter.menubutton');
                return true;
            }
        }, {
            wrapper: '.info',
            popup: {
                type: 'tooltip',
                position: 'right',
                titletext: "快捷访问最后一次登录的组织",
                width: 360,
                height: 150,
                detailtext: ["就算您有多个组织，也可以从这里快速的找到并切换到最后一次登录的组织"],
                textalign: "left"
            },
            onEnter: function () {
                $('.m-btn-line:visible').trigger('mouseenter.menubutton');
                return true;
            }
        }, {
            popup: {
                type: 'modal',
                position: 'right',
                width: 400,
                height: 260,
                titleimageclass: "my-laststep",
                titletext: "祝您使用美记愉快",
                detailtext: ["如果您有其他问题，或需要帮助，请随时联系我们。"],
                textalign: "center"
            },
            onEnter: function () {
                $('.m-btn-line:visible').trigger('mouseenter.menubutton');
                return true;
            }
        }];

        return steps;
    },

    initSetup: function () {
        if (MyIndex.iframeUrl.indexOf("paymentsuccess") != -1) {
            MyIndex.iframeUrl && $.mTab.addOrUpdate(HtmlLang.Write(LangModule.My, "RenewalResults", "Renewal Results"), MyIndex.iframeUrl, true);
        } else {
            MyIndex.iframeUrl && $.mTab.addOrUpdate(HtmlLang.Write(LangModule.My, "InitializeWizard", "Initialize Wizard"), MyIndex.iframeUrl, true);
        }
    },
    initGrid: function () {
        if (MyIndex.orgListShowType == "2") {
            $("#aDataList").trigger("click");
        } else {
            $("#aDataGrid").trigger("click");
        }
        //MyIndex.requestData();
    },
    requestData: function () {
        $.mAjax.Post("/FW/FWHome/GetOrgListByPage", {}, function (result) {
            MyIndex.result = result;
            MyIndex.dataSource = MyIndex.result ? MyIndex.result.rows : "";
            MyIndex.bindGridData();
        }, "", true);
    },
    bindAction: function () {
        $("#aDataList").click(function () {
            MyIndex.updateOrgListShowType(2, function () {
                $("#aDataList").removeClass().addClass("m-data-list-current");

                $("#aDataGrid").removeClass().addClass("m-data-grid");


                $("#divDataGridContainer").hide();
                $("#divDataListContainer").show();
                MyIndex.bindListData();
            })
        });
        $("#aDataGrid").click(function () {

            MyIndex.updateOrgListShowType(1, function () {
                //图标
                $("#aDataList").removeClass().addClass("m-data-list");
                $("#aDataGrid").removeClass().addClass("m-data-grid-current");

                $("#divDataListContainer").hide();
                $("#divDataGridContainer").show();
                MyIndex.requestData();
            });

        });

        //为删除键绑定方法
        $("#aDelete").live("click", function () {
            //先获取组织ID
            var orgId = $(this).attr("orgId");
            if (!orgId) {
                var errorMsg = HtmlLang.Write(LangModule.My, "noOrgId", "Do not find organization，Unable to execution delete");
                $.mDialog.alert(errorMsg);
                return;
            }
            //数据的行号
            var rowIndex = $(this).attr("rowindex");

            MyIndex.deleteOrg($(this), orgId, rowIndex);
        });

        //创建演示组织
        $("#aCreateDemoCompany").click(function () {
            var orgId = $(this).attr("orgId");
            if (!orgId) {
                var tip = HtmlLang.Write(LangModule.My, "createDemoTip", "试用演示公司（全球版） ，系统需要预设数据，这个过程大概需要两分钟。");
                $.mDialog.confirm(tip, function () {
                    MyIndex.createDemoOrg();
                });
            } else {
                MyIndex.createDemoOrg();
            }
        });
    },
    createDemoOrg: function () {
        mAjax.submit(
            "/Organisation/CreateDemoCompany",
            {},
            function (msg) {
                if (msg && msg.ObjectID) {
                    MyIndex.onSelectOrg(msg.ObjectID, '/', '15', '0', false, '');
                } else {
                    $.mDialog.alert(HtmlLang.Write(LangModule.My, "CreateDemoCompanyError", "Try the Demo Company error."));
                }
            });
    },
    //删除组织的方法
    deleteOrg: function (dom, orgId, rowIndex) {

        var msg = HtmlLang.Write(LangModule.My, "deleteOrg", "Are you sure delete the organisation?");
        $.mConfirm(msg, {
            callback: function () {

                var url = "/FWHome/DeleteOrgById";
                var data = { orgId: orgId };

                $.mAjax.submit(url, data, function (msg) {
                    if (msg == 1) {

                        //删除组织成功以后，如果刚好在走向导页面，重新把my站点刷新一次
                        var currentOrgId = $("#aOrgList").attr("orgid");

                        //如果当前的OrgID和删除OrgID相同
                        if (orgId == currentOrgId) {
                            //删除成功以后，清掉页面保存的组织id信息
                            $("#aOrgList").attr("orgid", "");

                            var url = $("#aMySite").val();

                            mWindow.reload(url);

                            return;
                        }
                        var item = $(dom).parents(".item");
                        //隐藏该组织
                        if (item && item.length > 0) {
                            //针对大视图
                            MyIndex.requestData(10, 1);

                        } else {
                            MyIndex.bindListData();

                            MyIndex.requestData(10, 1);
                            //针对表格
                            $.mMsg(HtmlLang.Write(LangModule.My, "deleteOrgSuccess", "Organisation delete success!"));
                        }
                        //更新顶部组织下拉框
                        MyIndex.initOrgList();
                        $.mMsg(HtmlLang.Write(LangModule.Org, "DeleteOrgSuccess", "Delete Organisation Successfully"));

                    } else if (msg == -2) {
                        HtmlLang.Write(LangModule.BD, "YouNoHavePermissions", "You don't have appropriate permissions to operate !");
                    } else {
                        $.mDialog.alert(HtmlLang.Write(LangModule.My, "deleteOrgFail", "Organisation delete Fail , please try again!"));
                    }
                }, "", true);
            }
        });
    },
    //绑定列表形式数据
    bindListData: function () {
        var goServer = $("#aGoServer").val();
        Megi.grid('#divDataList', {
            resizable: true,
            pagination: false,
            fitColumns: true,
            auto: false,
            width: $(".m-imain-content").width(),
            url: "/FW/FWHome/GetOrgListByPage",
            columns: [[
                {
                    field: 'MOrgID', hidden: true
                },
                {
                    title: LangKey.Name,
                    field: 'MOrgName',
                    width: 250,
                    align: 'left',
                    sortable: true,
                    formatter: function (value, row, index) {
                        //row.Url = mUtil.GetOrgUrl(row.MRegProgress);
                        row.MVersionType = MyIndex.getVersionType(row);

                        if (row.MVersionType == 1 || row.MVersionType == 5) {
                            return "<span style='padding-left:5px;'>" + mText.encode(value) + "</span>";
                        } else {

                            return "<a href='javascript:void(0);' style='color:#048fc2' onclick=\"MyIndex.onSelectOrg('" + row.MOrgID + "', '" + row.Url + "', '" +

                                row.MRegProgress + "', '" + row.MVersionID + "', '" + row.MIsBeta + "');\">" + mText.encode(value) + "</a>";
                        }
                    }
                },

                {
                    title: HtmlLang.Write(LangModule.My, "Versions", "版本"),
                    align: 'center',
                    field: 'MVersionID',
                    width: 100,
                    sortable: true
                    ,
                    formatter: function (value, row, index) {
                        //处理
                        if (value == "0") {
                            return HtmlLang.Write(LangModule.My, "StandardEdition", "标准版");
                        } else if (value == "1") {
                            return HtmlLang.Write(LangModule.My, "GeneralLedgerEdition", "总账专版");
                        }
                    }
                },

                {
                    title: HtmlLang.Write(LangModule.My, "CreateDate", "Create Date"),
                    align: 'center',
                    field: 'MCreateDate',
                    width: 160,
                    sortable: true
                    ,
                    formatter: function (value, row, index) {
                        return $.mDate.format(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.My, "Role", "Role"),
                    field: 'MRoleName',
                    align: 'center',
                    width: 160,
                    sortable: true,
                    formatter: function (value, row, index) {
                        return HtmlLang.Write(LangModule.My, value, value);
                    }
                }
                ,
                {
                    title: HtmlLang.Write(LangModule.My, "LastViewed", "Last Viewed"),
                    align: 'center',
                    field: 'MLastViewDate',
                    width: 160,
                    sortable: true
                    ,
                    formatter: function (value, row, index) {

                        return $.mDate.format(row.MLastViewDate);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.My, "ExpiredDate", "Expired Date"),
                    align: 'center',
                    field: 'MExpiredDate',
                    width: 160,
                    sortable: true
                    ,
                    formatter: function (value, row, index) {
                        return $.mDate.format(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.My, "SoftVerison", "Verison"),
                    align: 'center',
                    field: 'MVersionType',
                    width: 160,
                    sortable: true
                    ,
                    formatter: function (value, row, index) {

                        var status = MyIndex.getVersionType(row);
                        if (status == 1) {
                            return HtmlLang.Write(LangModule.My, "TrialExpired", "Trial Expired");
                        } else if (status == 2) {
                            return HtmlLang.Write(LangModule.My, "FreeTrailVersion", "Free trial version");
                        } else if (status == 3) {
                            return HtmlLang.Write(LangModule.My, "PaidVersion", "Paid version");
                        } else if (status == 4) {
                            return HtmlLang.Write(LangModule.My, "BetaTesterOneYearFreeUse", "Beta tester one year free use");
                        } else if (status == 5) {
                            return HtmlLang.Write(LangModule.My, "Expired", "Expired");
                        } else {
                            return HtmlLang.Write(LangModule.My, "FreeTrailVersion", "Free trial version");
                        }
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.My, "Operation", "Operation "),
                    align: 'center',
                    field: 'MaturityDate2',
                    width: 80,
                    sortable: false
                    ,
                    formatter: function (value, row, index) {
                        if ((row.MVersionType == 1 || row.MVersionType == 2 || row.MVersionType == 5) && row.HasChangePermission) {
                            return "<div class='list-item-action'><a href=\"#\" class='list-item-del' onclick='MyIndex.deleteOrg($(this) ,\"" + row.MOrgID + "\",\"" +

                                index + "\")'></a></div>";
                        } else {
                            return "";
                        }

                    }
                }
            ]]
        });
    },
    getVersionType: function (row) {
        var status = 0;
        if (mDate.parse(mDate.format(row.MExpiredDate, 'yyyy-MM-dd')) < mDate.parse(mDate.format(mDate.DateNow, 'yyyy-MM-dd'))) {
            if (row.MIsPaid) {
                status = 5;
            } else {
                status = 1;
            }
        } else if (row.MIsPaid) {
            status = 3;
        } else if (row.MIsBetaUser) {
            status = 4;
        } else {
            status = 2;
        }

        return status;
    },
    onSelectOrg: function (orgId, url, regProgress, versionId, isBeta, orgName) {
        //判断组织是否已被删除
        $.mAjax.Post("/FW/FWHome/ValidateOrgIsDelete", { orgId: orgId }, function (result) {
            if (result) {
                var message = HtmlLang.Write(LangModule.My, "OrgIsDelete", "组织{0}已被删除!");
                $.mMsg(message.replace("{0}", ""));
                MyIndex.initOrgList();
            }
        });

        var betaGoServer = $("#hideBetaGoServer").val();

        var goServer = isBeta == "true" && betaGoServer ? betaGoServer : $("#aGoServer").val();

        MyIndex.validateOrgCreateOrJumpAuth(versionId, function () {

            url = "/FW/FWHome/OrgSelect?MOrgID=" + orgId + "&RedirectUrl=" + escape(goServer + url) + "&IsBeta=" + isBeta;
            if (regProgress == 15) {
                $("body").mask("");
                mWindow.reload(url);
            } else {
                $.mTab.addOrUpdate(HtmlLang.Write(LangModule.My, "InitializeWizard", "Initial wizard"), url, true);
            }
        });
    },
    //绑定表格形式数据
    bindGridData: function () {
        var goServer = $("#aGoServer").val();
        $("#divDataGrid").html("");
        $("#divDataGridContainer").mask("");
        if (MyIndex.dataSource != null && MyIndex.dataSource.length > 0) {


            for (var i = 0; i < MyIndex.dataSource.length; i++) {
                var html = '';
                var obj = MyIndex.dataSource[i];
                var versionHtml = '';

                obj.MVersionType = MyIndex.getVersionType(obj);
                html += '<div class="item m-bank-try" status="' + obj.MVersionType + '" version="' + obj.MVersionID + '">';
                html += '<div class="title">';
                //obj.Url = mUtil.GetOrgUrl(obj.MRegProgress);
                if (obj.MVersionType == 1 || obj.MVersionType === 5) {
                    html += '<span class="expired-name">' + mText.encode(obj.MOrgName) + '</span>';
                } else {
                    html += '<a href="javascript:void(0);" class="name" style="color:#048fc2" title=' + HtmlLang.Write(LangModule.My, "GoToOrganization", "go to the 

organization") + ' onclick="MyIndex.onSelectOrg(\'' + obj.MOrgID + '\', \'' + obj.Url + '\',\'' + obj.MRegProgress + '\', \'' + obj.MVersionID + '\', \'' + obj.MIsBeta

                        + '\');">' + mText.encode(obj.MOrgName) + '</a>';
                }
                html += '</div>';
                html += '<div class="info">';

                if (obj.MVersionID == "0") {
                    html += '<span class="info1" style="color:#e23f2a;font-weight:bold"><label>' + HtmlLang.Write(LangModule.My, "younowuser", "You are using") +

                        '</label>' + HtmlLang.Write(LangModule.My, "StandardEdition", "标准版") + '</span>';
                } else if (obj.MVersionID == "1") {
                    html += '<span class="info1" style="color:#e23f2a;font-weight:bold"><label>' + HtmlLang.Write(LangModule.My, "younowuser", "You are using") +

                        '</label>' + HtmlLang.Write(LangModule.My, "GeneralLedgerEdition", "总账专版") + '</span>';
                }

                html += '<span class="info3"><label style="">' + HtmlLang.Write(LangModule.My, "YourRole", "Your role:") + '</label>' + HtmlLang.Write(LangModule.My,

                    obj.MRoleName, obj.MRoleName) + '</span>';
                html += '<span class="info3"><label>' + HtmlLang.Write(LangModule.My, "CreateDateColon", "创建日期:") + '</label>' + $.mDate.format(obj.MCreateDate) +

                    '</span>';
                html += '</div>';
                html += '<div class="info">';
                var expiredDate = "";
                if (obj.MVersionType == 1) {
                    html += '<span class="info1 expired-version">' + HtmlLang.Write(LangModule.My, "TrialExpired", "Trial Expired") + '</span>';
                } else if (obj.MVersionType == 2) {
                    html += '<span class="info1 version">' + HtmlLang.Write(LangModule.My, "FreeTrailVersion", "Free trial version") + '</span>';
                } else if (obj.MVersionType == 3) {
                    html += '<span class="info1 version paid">' + HtmlLang.Write(LangModule.My, "PaidVersion", "Paid version") + '</span>';
                } else if (obj.MVersionType == 4) {
                    html += '<span class="info1 version">' + HtmlLang.Write(LangModule.My, "BetaTesterOneYearFreeUse", "Beta tester one year free use") + '</span>';
                } else if (obj.MVersionType == 5) {
                    html += '<span class="info1 version">' + HtmlLang.Write(LangModule.My, "Expired", "Expired") + '</span>';
                } else {
                    html += '<span class="info1 version">' + HtmlLang.Write(LangModule.My, "FreeTrailVersion", "Free trial version") + '</span>';
                }
                html += '<span class="info3"><label>' + HtmlLang.Write(LangModule.My, "LastView", "Last View:") + '</label>' + $.mDate.format(obj.MLastViewDate) +

                    '</span>';

                obj.MIsExpiredAlert = mDate.parse(obj.MExpiredDate).getTime() < new Date().addDays(15).getTime() ? true : false;

                html += (obj.MIsExpiredAlert ? '<span class="info3" style="color:#ff0000;">' : '<span class="info3">') + '<label>' + HtmlLang.Write(LangModule.My,

                    "ExpiredDateB", "Expired Date:") + '</label>' + $.mDate.format(obj.MExpiredDate) + '</span>';
                if (obj.MVersionType == 1 || obj.MVersionType == 2 || obj.MVersionType == 5) {
                    html += ' <span class="info4">';
                    html += '<a class="easyui-linkbutton easyui-linkbutton-yellow l-btn l-btn-small" group="" id="" onclick="MyIndex.PayNow(this,\'' + obj.MOrgID

                        + '\');"><span class="l-btn-left"><span class="l-btn-text">' + HtmlLang.Write(LangModule.My, "PayNow", "Pay Now") + '</span></span></a>';
                    if (obj.HasChangePermission) {
                        html += '<a href="javascript:void(0)" class="easyui-linkbutton l-btn l-btn-small" id="aDelete" orgid="' + obj.MOrgID + '" rowindex="' + i +

                            '"><span class="l-btn-left"><span class="l-btn-text">' +
                            HtmlLang.Write(LangModule.My, "DeleteButton", "Delete") + '</span></span></a>';
                    }
                }

                html += '</span>';

                html += '</div>';
                html += '</div>';

                $("#divDataGrid").append(html);
            }
        }


        $("#divDataGridContainer").unmask();
    },
    //马上支付的弹框提醒
    PayNow: function (element, orgId) {
        var mySite = $("#aMySite").val();
        //这个是获取的链接的地址
        var goSite = $("#PayNow").val();
        var contactUs = LangKey.ContactUs;
        var paymentAlert = LangKey.paymentAlert.format(contactUs);
        var div = $(element).parents(".m-bank-try");
        var version = div.attr("version");
        var status = div.attr("status");

        window.mFilter.doFilter("track", ["PayNow", { version: version, status: status }]);
        //warning
        //$.mDialog.built(paymentAlert, function () {

        //});
        //warning

        console.log(orgId);
        $.mDialog.show({
            width: 650,
            height: 450,
            mContent: `iframe:${mySite}/FW/FWHome/Payment?orgId=` + orgId
        }, function (x) {
            console.log(x);
            var orgId = "xxx";//todo:get orgId
            var qty = 12;//todo: get qty
            MyIndex.Checkout(orgId, qty)
        });
    },
    Checkout: function (orgId, sku, qty) {
        $.mDialog.close();

        var mySite = $("#aMySite").val();
        $.mAjax.Post(mySite + "/FW/FWHome/CheckOut", {
            OrgId: orgId,
            Items: [
                {
                    SkuId: sku,
                    Qty: qty
                }
            ]
        }, function (msg) {
            if (msg && msg.RedirectUrl) {
                window.open(msg.RedirectUrl)
            } else {
                console.error(msg)
            }
        });
        MyIndex.PaymentPending(orgId);
    },
    PaymentPending: function (orgId) {
        var mySite = $("#aMySite").val();
        $.mDialog.show({
            mWidth: 650,
            mHeight: 450,
            mContent: `iframe:${mySite}/FW/FWHome/PaymentPending?orgId=` + orgId
        }, function (x) {
            console.log(x);
        });
    },

    //初始化高度
    initDomHeight: function () {
        //
        $(".m-imain").css({
            "overflow": "hidden"
        });
        var tryDemoDivH = $(".try-demo-div").height();
        if (tryDemoDivH == 0) {
            tryDemoDivH = 25;
        }
        $("#divDataGridContainer").css({
            "height": ($(".m-imain").height() - tryDemoDivH - 35) + "px",
            "overflow-y": "hidden"
        });
        $("#divDataListContainer").css({
            "height": ($(".m-imain").height() - tryDemoDivH - 35) + "px",
            "overflow-y": "auto",
            "overflow-x": "hidden",
        });
        $("#divDataGrid").css({
            "height": ($("#divDataGridContainer").height()) + "px",
            "overflow-y": "auto"
        });
    },
    //初始化组织下拉列表
    initOrgList: function () {
        var mySite = $("#aMySite").val();
        //
        var lastOrgInfo = $(".info");
        //
        var lastOrgInfoID = lastOrgInfo.attr("lastOrgID");
        //
        var lastOrgInfoHref = $("a", lastOrgInfo);
        lastOrgInfoHref.removeAttr("onclick");
        //
        $.mAjax.Post(mySite + "/FW/FWHome/GetOrgList", "", function (msg) {
            //数据
            var data = msg.Data;
            //上下文
            var context = msg.Context;
            var contextOrg = context.MOrgID;
            var goSite = $("#aGoServer").val();
            //高度
            var height = 0;
            var html = ""; //"<div class='m-org'><a href='###' id='aOrgList'>" + HtmlLang.Write(LangModule.My, "MyMegi", "My Megi") + "</a></div>";
            if (data && data.length > 0) {
                html += "<div id='divOrgList' class='m-pop-box m-pop-menu'><b class='popup-arrow'></b>";
                html += "<div class='item-list'>";
                //先清空
                $("#divOrgList").empty();
                for (var i = 0; i < data.length; i++) {

                    var temp = data[i];
                    var orgId = temp.MOrgID;

                    var url = mySite + "/FW/FWHome/OrgSelect?MOrgID=" + orgId + "&RedirectUrl=" + goSite + temp.Url + "&IsBeta=" + temp.MIsBeta;

                    url = MyIndex.tabswitch.intercept(url);

                    var clickEvent = 'MyIndex.onSelectOrg(\'' + temp.MOrgID + '\', \'' + temp.Url + '\',\'' + temp.MRegProgress + '\',\'' + temp.MVersionID + '\', \''

                        + temp.MIsBeta + '\');';


                    html += '<p><a href="###" onclick="' + clickEvent + '">' + mText.encode(temp.MOrgName) + '</a></p>';
                    //
                    if (orgId === lastOrgInfoID) {
                        lastOrgInfoHref.attr("onclick", clickEvent);
                    }
                }

                if (!lastOrgInfoHref.attr("onclick")) {
                    lastOrgInfoHref.attr("onclick", "alert('" + HtmlLang.Write(LangModule.My, "UnavailableOrg", "该组织不可用，可能已过期、被删除或被禁用！") + "');");
                }

                height = 38 * data.length;
                html += "</div>";
            }
            $("#divOrgCombox").append(html);
            //重新注册一下事件
            FW.initMenu();
            //设置高度
            $(".item-list", "#divOrgList").css("height", height + "px");
            MyIndex.initDomHeight()
        }, "", true);
    },
    updateOrgListShowType: function (type, callback) {
        callback();
    },
    addOrganisation: function (version) {
        if (version == 1) {

            //先校验是否有增加组织的权限
            MyIndex.validateOrgCreateOrJumpAuth(version, function (version) {
                var mySite = $("#aMySite").val();
                var message = HtmlLang.Write(LangModule.My, "AddSmartVersionOrganistionConfirm", "这个版本只提供总账功能，适用于代理记账公司，不提供任何管理需求。是否

确认要添加 ? ");
                $.mDialog.confirm(message, function () {
                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.My, "InitializeWizard", "Initialize Wizard"), $("#aGoServer").val() + "/BD/Setup/OrgSetting/0?

version = " + version, true);
                });
            });

        } else {
            $.mTab.addOrUpdate(HtmlLang.Write(LangModule.My, "InitializeWizard", "Initialize Wizard"), $("#aGoServer").val() + "/BD/Setup/OrgSetting/0?version=" +

                version, true);
        }
    },
    validateOrgCreateOrJumpAuth: function (version, callback) {
        var mySite = $("#aMySite").val();
        $.mAjax.Post(mySite + "/FW/FWHome/ValidateCreateOrgAuth", { type: version }, function (msg) {
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
        var mySite = $("#aMySite").val();
        $.mAjax.Post(mySite + "/FW/FWHome/ValidateBeta", { isBeta: isBeta }, function (msg) {
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
    }
}
$(document).ready(function () {

    SystemUpdate.init();

    MyIndex.init();

    MyIndex.tabswitch.bindSwitchEvent(function () {
        top.accessRequest(function () {

        }, false);
    });
});

//设置一个定时器，每隔1分钟去请求后台判断用户是否过期,目前只有my站点和login站点需要
$(function () {
    //打开一个定时器，轮训判断用户是否已经登陆,时间间隔为1s
    //var timeoutIntervalId = setInterval(function () {
    //    //如果正在登陆状态则不进行轮训了
    //    if (top.$(".m-login-box-home").is(":hidden")) {
    //        //异步请求是否超时
    //        top.accessRequest(null, false, "1");
    //    }
    //}, 60000);
})
//登录完成后的回调函数集合
window.loginCallback = [];