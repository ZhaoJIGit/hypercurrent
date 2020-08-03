var FPFaPiaoSetting = {
    Type: 1, //进项：1,销项：0.
    ImportTypeConfig: [], //导入类型配置
    ConfigList: [], //配置的字段
    SpecialTypeModel: [],
    ProfessionalType: [],
    FPSettingType: 7,//进项：7,销项：8.
    //FPtype: 0,//增值税普通发票：0,增值税专业发票:1.
    Init: function () {
        FPFaPiaoSetting.BindAction();
        FPFaPiaoSetting.BindData();
        FPFaPiaoSetting.InitData();
    },
    InitData: function () {
        $("#isIncomesInfosAllCheck").attr("checked", "true");
        $("#isIncomesDetailsAllCheck").attr("checked", "true");

        $("#isSalesInfosAllCheck").attr("checked", "true");
        $("#isSalesDetailsAllCheck").attr("checked", "true");

    },
    BindAction: function () {
        //tab 切换事件
        $('#tt').tabs({
            onSelect: function (title, index) {
                if (index == 0) {
                    FPFaPiaoSetting.Type = 1;
                    FPFaPiaoSetting.FPSettingType = 7;
                } else if (index == 1) {
                    FPFaPiaoSetting.Type = 0;
                    FPFaPiaoSetting.FPSettingType = 8;
                }
                FPFaPiaoSetting.BindData();
            }
        });

        //进项保存发票设置界面
        $("#aSave").off("click").on("click", function () {
            FPFaPiaoSetting.SaveFaPiaoSetting();
        });

        //销项保存发票设置界面
        $("#aSalesSave").off("click").on("click", function () {
            FPFaPiaoSetting.SaveFaPiaoSetting();
        });

        //CheckBox事件
        $("#isIncomesInfosAllCheck").change(function () {
            var value = $("#isIncomesInfosAllCheck").is(':checked');
            if (value) {
                $("#IncomesFaPiaoInfoCheckGroup input[type='checkbox']").attr("checked", "true");
            } else {
                $("#IncomesFaPiaoInfoCheckGroup input[type='checkbox']").removeAttr("checked");
            }
        });

        $("#isIncomesDetailsAllCheck").change(function () {
            var value = $("#isIncomesDetailsAllCheck").is(':checked');
            if (value) {
                $("#IncomesDetailCheckGroup input[type='checkbox']").attr("checked", "true");
            } else {
                $("#IncomesDetailCheckGroup input[type='checkbox']").removeAttr("checked");
            }
        });

        $("#isSalesInfosAllCheck").change(function () {
            var value = $("#isSalesInfosAllCheck").is(':checked');
            if (value) {
                $("#SalesFaPiaoInfoCheckGroup input[type='checkbox']").attr("checked", "true");
            } else {
                $("#SalesFaPiaoInfoCheckGroup input[type='checkbox']").removeAttr("checked");
            }
        });

        $("#isSalesDetailsAllCheck").change(function () {
            var value = $("#isSalesDetailsAllCheck").is(':checked');
            if (value) {
                $("#SalesFaPiaoDetailCheckGroup input[type='checkbox']").attr("checked", "true");
            } else {
                $("#SalesFaPiaoDetailCheckGroup input[type='checkbox']").removeAttr("checked");
            }
        });

    },
    //保存发票配置
    SaveFaPiaoSetting: function () {
        var obj = {};
        var sModel = FPFaPiaoSetting.SpecialTypeModel;
        var pModel = FPFaPiaoSetting.ProfessionalType;
        var isInfoAll = false;
        var isDetailAll = false;
        var type = FPFaPiaoSetting.Type;
        //进项
        if (type == 1) {
            var selspecialType = $("#selSpecialType").combobox("getValue");
            sModel.MImportType = selspecialType;
            var professionalType = $("#selProfessionalType").combobox("getValue");
            pModel.MImportType = professionalType;

            isInfoAll = $("#isSalesInfosAllCheck").is(':checked');
            isDetailAll = $("#isSalesDetailsAllCheck").is(':checked');
        }
            //销项
        else if (type == 0) {
            var specialType = $("#specialAndProfessionalType").combobox("getValue");
            sModel.MImportType = specialType;
            isInfoAll = $("#isIncomesInfosAllCheck").is(':checked');
            isDetailAll = $("#isIncomesDetailsAllCheck").is(':checked');
        }

        obj.SpecialTypeModel = sModel;
        obj.ProfessionalType = pModel;
        obj.MType = type;
        obj.IsInfoAll = isInfoAll;
        obj.IsDetailAll = isDetailAll;

        mAjax.submit("/FP/FPSetting/SaveFaPiaoSetting", { model: obj },
        function (data) {
            if (data.Success) {
                mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "保存成功！"));
                $.mDialog.close();
            } else {
                mDialog.message(HtmlLang.Write(LangModule.FP, "SaveFailed", "保存失败！"));
            }
        });
    },

    BindData: function () {
        //获取导入方式
        mAjax.post("/FP/FPSetting/GetImportTypeConfig", {
            type: FPFaPiaoSetting.Type,
        }, function (data) {
            FPFaPiaoSetting.ImportTypeConfig = data;
            //进项
            if (FPFaPiaoSetting.Type == 1) {
                if (data) {
                    for (var i = 0; i < data.length; i++) {
                        var model = data[i];
                        //普票
                        if (model.MFPType == 0 && model.MType == 1) {
                            $("#selSpecialType").combobox("setValue", model.MImportType);
                            FPFaPiaoSetting.SpecialTypeModel = model;
                        }
                            //专票
                        else if (model.MFPType == 1 && model.MType == 1) {
                            $("#selProfessionalType").combobox("setValue", model.MImportType);
                            FPFaPiaoSetting.ProfessionalType = model;
                        }
                    }
                }
                //销项
            } else if (FPFaPiaoSetting.Type == 0) {
                for (var j = 0; j < data.length; j++) {
                    var pmodel = data[j];
                    if (pmodel.MFPType == 0 && pmodel.MType == 0) {
                        $("#specialAndProfessionalType").combobox("setValue", pmodel.MImportType);
                        FPFaPiaoSetting.SpecialTypeModel = pmodel;
                    }
                }
            }
        });

        //获取导入的字段
        mAjax.post("/FP/FPSetting/GetConfigList", {
            fpType: FPFaPiaoSetting.FPSettingType,
        }, function (data) {
            var checkHtml = FPFaPiaoSetting.GetCheckBoxHtml(data, false);
            var checkDetalHtml = FPFaPiaoSetting.GetCheckBoxHtml(data, true);
            if (FPFaPiaoSetting.Type == 1) {
                $("#IncomesFaPiaoInfoCheckGroup").children().remove();
                $("#IncomesDetailCheckGroup").children().remove();
                $("#IncomesFaPiaoInfoCheckGroup").append(checkHtml);
                $("#IncomesFaPiaoInfoCheckGroup input[type='checkbox']").attr("checked", "true");
                $("#IncomesDetailCheckGroup").append(checkDetalHtml);
                $("#IncomesDetailCheckGroup input[type='checkbox']").attr("checked", "true");
            } else if (FPFaPiaoSetting.Type == 0) {
                $("#SalesFaPiaoInfoCheckGroup").children().remove();
                $("#SalesFaPiaoDetailCheckGroup").children().remove();
                $("#SalesFaPiaoInfoCheckGroup").append(checkHtml);
                $("#SalesFaPiaoInfoCheckGroup input[type='checkbox']").attr("checked", "true");
                $("#SalesFaPiaoDetailCheckGroup").append(checkDetalHtml);
                $("#SalesFaPiaoDetailCheckGroup input[type='checkbox']").attr("checked", "true");
            }

        });
    },
    //拼接CheckBox
    GetCheckBoxHtml: function (data, isDetail) {
        var html = "";

        //先筛选发票头或明细数据,避免混在一起导致顺序不一致
        var filterData = null;
        if (isDetail) {
            filterData = data.where("x.MIsKey==0");
        } else {
            filterData = data.where("x.MIsKey==1");
        }
        if (filterData == null)
            return html;

        for (var i = 0; i < filterData.length; i++) {
            var model = filterData[i];
            if (i % 3 == 0) {
                html = html + "<tr><td><input type='checkbox'  disabled='disabled' Style='margin: 10px;'/><span style='margin-right: 10px;'>" + model.MName + "</span></td>" + "";
            } else if (i % 3 == 2) {
                html = html + "<td><input type='checkbox' disabled='disabled' Style='margin: 10px;' /><span style='margin-right: 10px;'>" + model.MName + "</span></td></tr>";
            } else {
                html = html + "<td><input type='checkbox' disabled='disabled' Style='margin: 10px;' /><span style='margin-right: 10px;'>" + model.MName + "</span></td>";
            }
        }
        if (html) {
            html = "<table>" + html.trim("</tr>") + "</tr></table>";
        }

        return html;
    },
    //设置参数
    OpenParamsSet: function (fpType, type) {
        var id = "";
        var typeValue = 0;//是否本地上传
        if (type == 0) {
            id = FPFaPiaoSetting.SpecialTypeModel.MItemID;
            if (fpType == 0) {
                typeValue = $("#specialAndProfessionalType").combobox("getValue");
            } else {
                typeValue = $("#selSpecialType").combobox("getValue");
            }
        } else if (type == 1) {
            id = FPFaPiaoSetting.ProfessionalType.MItemID;
            typeValue = $("#selProfessionalType").combobox("getValue");
        }
        if (typeValue == 1) {
            mDialog.message(HtmlLang.Write(LangModule.FP, "LocalUploadNotParams", "本地上传不用设置参数"));
            return;
        }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.FP, "SetAutoParams", "设置自动获取参数"),
            width: 500,
            height: 300,
            href: '/FP/FPSetting/FPSetAutoParams?id=' + id
        });
    }
}

$(document).ready(function () {
    FPFaPiaoSetting.Init();
});