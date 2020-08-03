var DepreciationReportBase = {
    reportType: null,
    accountList: null,
    currencyList: null,
    isClick: false,
    //选择的具体核算维度值
    checkTypeValueList: null,
    init: function () {
        DepreciationReportBase.initAction();
        DepreciationReportBase.initUI();
    },
    initAction: function () {
        $("#btnMoreFilter").click(function () {
            if (!DepreciationReportBase.isMouseover) {
                DepreciationReportBase.showMoreFilter();
            }
            DepreciationReportBase.isClick = true;

        }).mouseover(function (e) {
            setTimeout(function () {
                if (!DepreciationReportBase.isClick) {
                    DepreciationReportBase.showMoreFilter();
                }
            }, 700);
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
            DepreciationReportBase.isClick = false;
        });

        $("#btnUpdate").click(function () {
            var baseFilter = DepreciationReportBase.getFilter();
            $("#aReportUpdate").trigger("click");
            $(".gl-rpt-search").hide();
        });

        $("#btnReset").click(function () {
            DepreciationReportBase.resetSearchFilter();
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
                        DepreciationReportBase.setCheckTypeValueObject(checkTypeParams);
                    }

                },
                mPostData: { "value": encodeURIComponent(JSON.stringify(DepreciationReportBase.checkTypeValueList)) }
            });
        });

        $("#ckbIncludeChecktype").click(function () {
            if ($(this).is(":checked")) {
                $("#btnCheckTypeDetail").show();
            } else {
                $("#btnCheckTypeDetail").hide();
            }
        });
    },
    showMoreFilter: function () {
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
            DepreciationReportBase.checkTypeValueList = null;
        } else {
            DepreciationReportBase.checkTypeValueList = new Array();
            checkTypeParams = JSON.parse(checkTypeParams);
            for (var i = 0 ; i < checkTypeParams.length; i++) {
                var nameValueObject = {};
                var checkTypeParam = checkTypeParams[i];
                nameValueObject.MName = checkTypeParam.key;
                nameValueObject.MValue = checkTypeParam.value;
                nameValueObject.MValue1 = mText.encode(checkTypeParam.name);
                DepreciationReportBase.checkTypeValueList.push(nameValueObject);
            }


        }
    },
    initUI: function () {

        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");

        if ($("#ckbIncludeChecktype").is(":checked")) {
            $("#btnCheckTypeDetail").show();
        } else {
            $("#btnCheckTypeDetail").hide();
        };
    },
    initFilter: function () {

        if ($("#ckbIncludeChecktype").is(":checked")) {
            $("#btnCheckTypeDetail").show();
        } else {
            $("#btnCheckTypeDetail").hide();
        };
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
        filter.CheckTypeValueList = filter.IncludeCheckType ? DepreciationReportBase.checkTypeValueList : null;
        return filter;
    },
   
    resetSearchFilter: function () {
        $("#ckbIncludeChecktype").removeAttr("checked");
    }
}