/// <reference path="../RptUC/UCReport.js" />
var SubsidiaryLedger = {
    baseParams: null,
    currencyList: null,
    accountCheckTypeTree: null,
    checkTypeValueList: null,
    isFirstLoad: true,
    treeData: null,
    init: function () {
        SubsidiaryLedger.initAction();
        SubsidiaryLedger.changeCombo();
        SubsidiaryLedger.initUI();
        var filter = UCReport.getFilter();

        SubsidiaryLedger.loadReportData();
    },
    initUI:function(){
        $(".showleafaccount").show();
    },
    changeCombo: function () {

        $("#cbxStartPeriod").combobox({
            onChange: function (n, o) {
                Megi.dateCompare(n, $("#cbxEndPeriod"), 1);
            }
        });
        $("#cbxEndPeriod").combobox({
            onChange: function (n, o) {
                Megi.dateCompare(n, $("#cbxStartPeriod"), 2);
            }
        });

    },
    initAction: function () {
        $("#aReportUpdate").off("click").on("click", function () {
            if (!$("#cbxStartPeriod").combobox("getValue") ||
                !$("#cbxEndPeriod").combobox("getValue")) {
                return;
            }

            SubsidiaryLedger.getFastSwitchAccountList();
        });
       

        $("#btnUpdate").off("click").on("click", function () {
            if (!$("#cbxStartPeriod").combobox("getValue") ||
                !$("#cbxEndPeriod").combobox("getValue")) {
                return;
            }

            SubsidiaryLedger.getFastSwitchAccountList();
        });

        //前一个科目按钮
        $(".rpt-sub-preaccount").click(function () {
            var selector = $("#cbxFastSwitch");
            var tree = selector.combotree("tree");
            var selectedId = selector.combotree("getValue");
            
            if (!selectedId) {
                SubsidiaryLedger.selectTreeNodeByIndex("#cbxFastSwitch", 0);
                return;
            }
            var selectedNode = tree.tree("find", selectedId);
            var preNodeTarget = $(selectedNode.target).parent().prev();

            //表示没有同级了，这时候需重新选取一个父级
            if (!preNodeTarget || preNodeTarget.length == 0) {
                preNodeTarget = $(selectedNode.target).parent().parent().parent().prev();
            }

            preNodeTarget = preNodeTarget.find(".tree-node")[0];

            var preNode = tree.tree("getNode", preNodeTarget);

            if (preNode) {
                selector.combotree("setValue", preNode.id);
            }
        });

        $(".rpt-sub-nextaccount").click(function () {
            var selector = $("#cbxFastSwitch");
            var tree = selector.combotree("tree");
            var selectedId = selector.combotree("getValue");
            if (!selectedId) {
                SubsidiaryLedger.selectTreeNodeByIndex("#cbxFastSwitch", 0);
                return;
            }
            var selectedNode = tree.tree("find", selectedId);

            var nextNodeTarget = null;

            var parentDom = $(selectedNode.target).parent();
            if (parentDom.find("ul").length > 0) {
                //存在子科目
                nextNodeTarget = $(parentDom).find("ul")[0];
            } else {
                nextNodeTarget = $(parentDom).next();
            }

            //表示没有同级了，这时候需重新选取一个父级
            if (!nextNodeTarget || nextNodeTarget.length == 0) {
                nextNodeTarget = $(selectedNode.target).parent().parent().parent().next();
            }

            nextNodeTarget = $(nextNodeTarget).find(".tree-node")[0];

            var nextNode = tree.tree("getNode", nextNodeTarget);

            if (nextNode) {
                selector.combotree("setValue", nextNode.id);
            }
        });
    },
    loadReportData: function () {
        var opts = {};
        opts.type = "GL";
        opts.url = "/Report/RptSubsidiaryLedger/GetReportData";
        opts.getFilter = function () {
            return SubsidiaryLedger.getFilter();
        };

        opts.initFilter = function (filter) {
            SubsidiaryLedger.initFilter(filter);
        };

        opts.callback = function (msg) {
            
            GLReportBase.initFilter();

            //第一次加载，加载默认开始科目和结束科目
            if (!UCReport.options.IsReload) {
                var filter = msg.Filter;
                if (filter) {
                    SubsidiaryLedger.initFilter(filter);
                    $("#hidReportFilter").val("");
                }

                SubsidiaryLedger.getFastSwitchAccountList();
            }

            $("table", "#divReportDetail").mTableResize({
                forceFit: false
            });
        };

        UCReport.init(opts);


    },
    //获取开始科目到结束科目的所有id
    getAccountIdList: function () {
        var result = {};
        //var filter = SubsidiaryLedger.getFilter();filter.MStartAccountID && filter.MEndAccountID && 
        if (SubsidiaryLedger.treeData) {
            $.each(SubsidiaryLedger.treeData, function (idx, item) {
                result[item.id] = item.text;
                if (item.children.length > 0) {
                    SubsidiaryLedger.addChildAcctId(item.children, result);
                }
            });
        }
        return result;
    },
    addChildAcctId: function (childItem, result) {
        $.each(childItem, function (idx, item) {
            result[item.id] = item.text;
            if (item.children.length > 0) {
                SubsidiaryLedger.addChildAcctId(item.children, result);
            }
        });
    },
    getFastSwitchAccountList: function (loadSuccessEvent) {
        var filter = SubsidiaryLedger.getFilter();

        SubsidiaryLedger.checkTypeValueList = filter.CheckTypeValueList;
        GLReportBase.checkTypeValueList = filter.CheckTypeValueList;
        
        //var t1 = $('#cbxStartAccount').combotree('tree');
        //var n1 = t1.tree('getSelected');

        //if (n1) {
        //    filter.AccountStartIndex = n1.Index;
        //}

        //var t2 = $('#cbxEndAccount').combotree('tree');
        //var n2 = t2.tree('getSelected');

        //if (n2) {
        //    filter.EndStartIndex = n2.Index;
        //}

        filter.ShowNumber = true;
        filter.IsActive = true;
        filter.IsAll = true;
        var url = "/BD/BDAccount/GetAccountList";

        $.mAjax.post(url, { filter: filter }, function (data) {
            SubsidiaryLedger.treeData = data;
            $("#cbxFastSwitch").combotree({
                data: data,
                onSelect: function (node) {
                    //核算维度查询
                    filter.AccountID = node.id;
                    $("#hideAccountID").val(node.id);
                    UCReport.reload();
                },
                onBeforeSelect: function (node) {

                    if (SubsidiaryLedger.isFirstLoad) {
                        SubsidiaryLedger.isFirstLoad = false;
                        return false;
                    }

                    return true;
                },
                onLoadSuccess: function () {
                    if (loadSuccessEvent != undefined) {
                        loadSuccessEvent();
                    }

                    //科目为空或者combotree中找不到这个节点，选中第一个科目
                    if (!filter.MAccountID || !$("#cbxFastSwitch").combotree("tree").tree("find" , filter.MAccountID)) {
                        filter.MAccountID = $("#cbxFastSwitch").combotree("tree").tree("getRoots")[0].id;
                    }

                    

                    $("#cbxFastSwitch").combotree("setValue", filter.MAccountID);

                    GLReportBase.checkTypeValueList = filter.CheckTypeValueList;
                }
            });
        }, false, true, true);
    },
    getFilter: function () {
        var param = {};
        param.MReportID = UCReport.ReportID;
        param.IsReload = UCReport.options.IsReload;

        param = $.extend(param, GLReportBase.getFilter());
        param.MAccountID = $("#hideAccountID").val() ? $("#hideAccountID").val() : param.MStartAccountID;
        param.IsLeafAccount = $("#ckbIsLeafAccount").is(":checked");

        param.MStartAccountIndex = $("#startAccountIndex").val();
        param.MEndAccountIndex = $("#endAccountIndex").val();

        param = $.extend(param, UCReport.getFilter());


        return param;
    },
    initFilter: function (filter) {
        if (filter.MStartAccountID) {
            $("#cbxStartAccount").combotree("setValue", filter.MStartAccountID);
        }

        if (filter.MEndAccountID) {
            $("#cbxEndAccount").combotree("setValue", filter.MEndAccountID);
        }


        $("#cbxStartPeriod").combobox("setValue", filter.MStartPeroid);
        $("#cbxEndPeriod").combobox("setValue", filter.MEndPeroid);


        if (filter.MCurrencyID) {
            $("#cbxMCurrencyID").combobox("setValue", filter.MCurrencyID);
        } else {
            $("#cbxMCurrencyID").combobox("setValue", "");
        }
        if (filter.IncludeCheckType) {
            $("#btnCheckTypeDetail").attr("checked", "checked");
            $("#btnCheckTypeDetail").show();
        }

        if (filter.CheckTypeValueList) {
            SubsidiaryLedger.checkTypeValueList = filter.CheckTypeValueList;
            GLReportBase.checkTypeValueList = filter.CheckTypeValueList;
        }

        if (filter.AccountLevel) {
            $("#nsAccountLevel").numberspinner("setValue", filter.AccountLevel);
        }

        if (filter.IsLeafAccount) {
            $("#ckbIsLeafAccount").attr("checked", "checked");
        }

        if (filter.MAccountID) {
            $("#hideAccountID").val(filter.MAccountID);
        }

        $("#startAccountIndex").val(filter.MStartAccountIndex);
        $("#endAccountIndex").val(filter.MEndAccountIndex);

    },
    faseAccountTreeLoadSuccess: function () {
        var filter = GLReportBase.getFilter();
        var startAccountId = filter.MStartAccountID;
        var endAccountId = filter.MEndAccountID;

        $(selector).combotree("setValue", startAccountId);
    },
    isStartOrEndAccountNode: function (tree, rootNode, matchAccountId) {
        if (rootNode.id == matchAccountId) {
            return true;
        }

        var childrenNode = rootNode.children;
        if (!childrenNode || childrenNode.length == 0) {
            return false;
        }

        for (var i = 0; i < childrenNode.length; i++) {
            var childNode = childrenNode[i];

            return SubsidiaryLedger.isStartOrEndAccountNode(tree, childNode, matchAccountId);
        }
    },
    showOrHideNode: function (tree, treeNode, show) {

        var childrenNode = treeNode.children;

        if (show) {
            $(treeNode.target).show();
        } else {
            $(treeNode.target).hide();
            $(treeNode.target).next().hide();
        }
    },
    showParentNode: function (tree, treeNode) {
        var parentNode = tree.tree("getParent", treeNode.target);

        if (!parentNode || parentNode.length == 0) {
            return;
        }

        $(parentNode.target).show();

        SubsidiaryLedger.showParentNode(tree, parentNode);
    },
    showChildrenNode: function (tree, treeNode) {
        var childrenNode = treeNode.children;

        if (!childrenNode || childrenNode.length == 0) {
            return;
        }

        for (var i = 0; i < childrenNode.length; i++) {
            var childNode = childrenNode[i];
            $(childNode.target).show();
            SubsidiaryLedger.showChildrenNode(tree, childNode);
        }
    },
    selectTreeNodeByIndex: function (selector , index) {
        var tree = $(selector).combotree("tree");

        var data = $(tree).tree("getData");

        if (data != null && data.length >= index) {
            var node = data[index];

            $(selector).combotree("setValue", {index:node.id});
        }

    }
}