var GLReportBase = {
    reportType: null,
    accountList: null,
    currencyList: null,
    isClick:false,
    //选择的具体核算维度值
    checkTypeValueList:null,
    init: function () {
        GLReportBase.initAction();
        GLReportBase.initBaseData();
        GLReportBase.initUI();
        
    },
    initAction: function () {
        $("#btnMoreFilter").click(function () {
            if (!GLReportBase.isMouseover) {
                GLReportBase.showMoreFilter();
            }
            GLReportBase.isClick = true;

        }).mouseover(function (e) {
            setTimeout(function () {
                if (!GLReportBase.isClick) {
                    GLReportBase.showMoreFilter();
                }
            },700);
        });

        $(".gl-rpt-search").click(function (event) {
            event = event || window.event;
            event.stopPropagation();
        });

        $("body").click(function (e) {

            if ($(e.srcElement).attr("id") == "btnMoreFilter") {
                return;
            }

            $(".gl-rpt-search").hide();
            GLReportBase.isClick = false;
        });

        $("#btnUpdate").click(function () {
            var baseFilter = GLReportBase.getFilter();
            $("#aReportUpdate").trigger("click");
            $(".gl-rpt-search").hide();
        });

        $("#btnReset").click(function () {
            GLReportBase.resetSearchFilter();
        });

        $("#btnCheckTypeDetail").click(function () {
            $.mDialog.show({
                mTitle: HtmlLang.Write(LangModule.Report, "DisplayCheckTypeDetail", "显示核算明细"),
                mDrag: "mBoxTitle",
                mShowbg: true,
                mWidth: 800,
                mHeight: 500,
                mContent: "iframe:/Report/ReportGLBase/CheckTypeDetail",
                mCloseCallback: function (checkTypeParams) {
                    if (checkTypeParams) {
                        GLReportBase.setCheckTypeValueObject(checkTypeParams);
                    }
                   
                },
                mPostData: { "value": encodeURIComponent(JSON.stringify(GLReportBase.checkTypeValueList)) }
            });
        });

        $("#ckbIncludeChecktype").click(function () {
            if ($(this).is(":checked") && GLReportBase.reportType!="41") {
                $("#btnCheckTypeDetail").show();
            } else {
                $("#btnCheckTypeDetail").hide();
            }
        });

        $("#ckbIsLeafAccount").off("click").on("click", function () {
            if ($("#ckbIsLeafAccount").is(":checked")) {
                $("#nsAccountLevel").numberspinner("setValue", "");
                $("#nsAccountLevel").numberspinner("disable");
            } else {
                $("#nsAccountLevel").numberspinner("enable");
            }
            
        });
    },
    showMoreFilter:function(){
        var dom = $(".gl-rpt-search");
        var display = dom.css("display") == "block";
        if (display) {
            dom.hide();
        } else {
            dom.show();
        }
    },
    setCheckTypeValueObject: function (checkTypeParams) {
        if (!checkTypeParams || checkTypeParams.length == 0) {
            GLReportBase.checkTypeValueList = null;
        } else {
            GLReportBase.checkTypeValueList = new Array();
            checkTypeParams = JSON.parse(checkTypeParams);
            for (var i = 0 ; i < checkTypeParams.length; i++) {
                var nameValueObject = {};
                var checkTypeParam = checkTypeParams[i];
                nameValueObject.MName = checkTypeParam.key;
                nameValueObject.MValue = checkTypeParam.value;
                nameValueObject.MValue1 =mText.encode(checkTypeParam.name);
                GLReportBase.checkTypeValueList.push(nameValueObject);
            }

            
        }
    },
    initBaseData: function () {
        if (GLReportBase.accountList == null) {
            var url =  "/BD/BDAccount/GetAccountList";
            $.mAjax.post(url, { filter: { ShowNumber: true, IsAll: true } }, function (data) {
                if (data) {
                    GLReportBase.accountList = data;
                }
            }, false, true, false);
        }

        if (GLReportBase.currencyList == null) {

            var integratedStandardCurrency = HtmlLang.Write(LangModule.Report, "IntegratedStandardCurrency", "Integrated standard currency");

            var integratedStandardCurrencyOj = {
                MName:integratedStandardCurrency,
                MCurrencyID: "0"
            }

            //新增综合本位币
            GLReportBase.currencyList = new Array();
            GLReportBase.currencyList.push(integratedStandardCurrencyOj);


            var url = "/BD/Currency/GetBDCurrencyList";
            $.mAjax.post(url, { isIncludeBase: true }, function (data) {
                if (data) {
                    GLReportBase.currencyList = GLReportBase.currencyList.concat(data);
                }
            }, false, true, false);
        }
    },
    initUI: function () {
        $("#cbxStartAccount").combotree({
            data: $.extend({} , GLReportBase.accountList),
        });

        $("#cbxEndAccount").combotree({
            data: $.extend({}, GLReportBase.accountList)
        });

        $("#cbxMCurrencyID").combobox({
            textField: "MName",
            valueField: "MCurrencyID",
            data: GLReportBase.currencyList,
            onLoadSuccess: function () {

            }
        });

        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");

        if ($("#ckbIncludeChecktype").is(":checked") && GLReportBase.reportType != "41") {
            $("#btnCheckTypeDetail").show();
        } else {
            $("#btnCheckTypeDetail").hide();
        };

        if ($("#ckbIsLeafAccount").is(":checked")) {
            $("#nsAccountLevel").numberspinner("setValue", "");
            $("#nsAccountLevel").numberspinner("disable");
        } else {
            $("#nsAccountLevel").numberspinner("enable");
        }
    },
    initFilter:function(){
        $("#btnCheckTypeDetail").show();
        if ($("#ckbIncludeChecktype").is(":checked") && GLReportBase.reportType!="41") {
            $("#btnCheckTypeDetail").show();
        } else {
            $("#btnCheckTypeDetail").hide();
        };

        if ($("#ckbIsLeafAccount").is(":checked")) {
            $("#nsAccountLevel").numberspinner("setValue", "");
            $("#nsAccountLevel").numberspinner("disable");
        } else {
            $("#nsAccountLevel").numberspinner("enable");
        }
    },
    //获取过滤条件
    getFilter: function () {
        var filter = {};

        filter.IncludeCheckType = $("#ckbIncludeChecktype").is(':checked');
        filter.MStartPeroid = $("#cbxStartPeriod").combobox("getValue");
        filter.MEndPeroid = $("#cbxEndPeriod").combobox("getValue");

        if (filter.MEndPeroid && filter.MStartPeroid) {
            if (parseFloat(filter.MEndPeroid) < filter.MStartPeroid) {
                var switchTemp = filter.MStartPeroid;
                filter.MStartPeroid = filter.MEndPeroid;
                filter.MEndPeroid = switchTemp;
            }
        }
        filter.MCurrencyID = $("#cbxMCurrencyID").combobox("getValue");
        filter.AccountLevel = $("#nsAccountLevel").numberspinner("getValue");
        //bug 23663. 兼容网络错误
        if (!filter.AccountLevel) {
            filter.AccountLevel = 0;
        }
        filter.MDisplayNoAccurrenceAmount = $("#ckbMDisplayNoAccurrenceAmount").is(":checked");
        filter.MDisplayZeorEndBalance = $("#ckbMDisplayZeorEndBalance").is(":checked");
        filter.IncludeUnapprovedVoucher = $("#ckbIncludeUnapprovedVoucher").is(":checked");
        filter.IncludeDisabledAccount = $("#ckbIncludeDisabledAccount").is(":checked");
       
        filter.MStartAccountID = $("#cbxStartAccount").combotree("getValue");

        var selectedStartAccount = GLReportBase.getSelectedNode($("#cbxStartAccount"));
        filter.AccountStartIndex = selectedStartAccount ? selectedStartAccount.Index : 0;

        filter.MEndAccountID = $("#cbxEndAccount").combotree("getValue");
        var selectedEndAccount = GLReportBase.getSelectedNode($("#cbxEndAccount"));
        filter.AccountEndIndex = selectedEndAccount ? selectedEndAccount.Index : 0;

        filter.CheckTypeValueList = filter.IncludeCheckType ? GLReportBase.checkTypeValueList : null;
        filter.CheckGroupValueId = filter.IncludeCheckType ? GLReportBase.checkGroupValueId : null;
        return filter;
    },
    getSearchAccountList: function () {
        var startAccountNode = GLReportBase.getSelectedNode($("#cbxStartAccount"));
        var endAccountNode = GLReportBase.getSelectedNode($("#cbxEndAccount"));

        if (startAccountNode == null) {
            startAccountNode = GLReportBase.accountList[0];
        }

        if (endAccountNode == null) {
            endAccountNode = GLReportBase.accountList[GLReportBase.accountList.length - 1];

            endAccountNode = GLReportBase.getTreeLastNode($("#cbxEndAccount"));
        }

        var startAccountNumber = startAccountNode.MNumber;
        var endAccountNumber = endAccountNode.MNumber;

        //如果用户选择的顺序相反，需要对调开始科目和结束科目
        if (+startAccountNumber.replace(".", "") > +endAccountNumber.replace(".", "")) {
            var tempStartAccountNode = $.extend({}, startAccountNode);

            startAccountNode = endAccountNode;
            endAccountNode = tempStartAccountNode;
        }
    },
    getTree: function (selector) {
        var tree = selector.combotree("tree");
        return tree;
    },
    getSelectedNode: function (selector) {
        var tree = GLReportBase.getTree(selector);

        var node = !selector.combotree("getValue") ? null : tree.tree("getSelected");

        return node;
    },
    getTreeLastNode: function (selector, parentId) {
        var tree = GLReportBase.getTree(selector);

        var node = tree.tree("find", parentId);

        var childrenNodes = tree.tree("getChildren", node.trigger);
        if (!childrenNodes && childrenNodes.length == 0) {
            return node;
        }

        var lastChildNode = children[childrenNodes.length - 1];

        var lastNodes = GLReportBase.getTreeLastNode(selector, lastChildNode.id);

        return lastNodes;
    },
    getTreeChildrenNode: function (selector , parentNode) {
        var tree = GLReportBase.getTree(selector);
        var childNodeList = new Array();

        var children = tree.tree("getChildren", parentNode.target);

        if (!children || children.length == 0) {
            return childNodeList;
        }

        childNodeList = childNodeList.concat(children);

        for (var i = 0 ; i < children.length; i++) {
            var tempChildNode = children[i];
            var tempChildNodeList = GLReportBase.getTreeChildrenNode(selector, tempChildNode);

            childNodeList = childNodeList.concat(tempChildNodeList);
        }

        return childNodeList;
    },
    resetSearchFilter: function () {
        $("#ckbIncludeChecktype").removeAttr("checked");
        $("#cbxMCurrencyID").combobox("setValue","");
        $("#nsAccountLevel").numberspinner("setValue","");
        $("#ckbMDisplayNoAccurrenceAmount").attr("checked","checked");
        $("#ckbMDisplayZeorEndBalance").attr("checked", "checked");
        $("#ckbIncludeUnapprovedVoucher").removeAttr("checked");
        $("#ckbIncludeDisabledAccount").removeAttr("checked");
        $("#cbxStartAccount").combotree("setValue", "");
        $("#cbxEndAccount").combotree("setValue", "");
    }
}