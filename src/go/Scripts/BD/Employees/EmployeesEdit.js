var GoEmployeeEdit = {
    isEnableGL: $("#hideIsEnableGL").val() == "true",
    //是否已经加载数据
    IsLoadedPoyroll: false,
    //五险一金的参数
    PayrollParam: null,
    tabIndex: $("#hidTabIndex").val(),
    isUpdate: $("#EmployeeID").val().length > 0,
    init: function () {
        GoEmployeeEdit.initTab(GoEmployeeEdit.tabIndex);
        GoEmployeeEdit.saveClick();
        GoEmployeeEdit.pitChange();

        if (GoEmployeeEdit.isUpdate) {
            mAjax.post("/BD/Employees/GetEmployeesEditData/", { model: { MItemID: $("#EmployeeID").val() } }, function (data) {
                $("body").mFormSetForm(data);
                //设置员工工资数据
                
                if (GoEmployeeEdit.isEnableGL) {
                    $("#cbxCurrentMoney").combobox("setValue", data.MCurrentAccountCode);
                }
                //对国家赋值
                $("#selMPCountryID").combobox("setValue", data.MPCountryID);
                $("#selMRealCountryID").combobox("setValue", data.MRealCountryID);
                GoEmployeeEdit.initPayrollDetail(data.PayrollDetail);
            }, null, true);
        } else {
            GoEmployeeEdit.autoLoadPayrollParam();
        }

        $("#MSocialSecurityBase").off("blur.emp").on("blur.emp", function () {
            GoEmployeeEdit.autoLoadPayrollParam();
        });

        $("#MHosingProvidentFundBase").off("blur.emp").on("blur.emp", function () {
            GoEmployeeEdit.autoLoadPayrollParam();
        });

        $(".per", "#divPayrollDetail").each(function () {
            //找金额的input
            var moneyDom = $(this).nextAll(".money");
            $(this).off("keyup.emp").on("keyup.emp", function (event) {
                GoEmployeeEdit.payrollCalculate(this, moneyDom, 1);
            });
            
            //失焦重新计算，解决失焦金额跟比例不一致问题
            $(this).off("blur.emp").on("blur.emp", function (event) {
                GoEmployeeEdit.payrollCalculate(this, moneyDom, 1);
            });

            $(this).off("keydown.emp").on("keydown.emp", function (event) {
                if ($(this).val() > 100) {
                    $(this).val("100.00");
                }

                if ($(this).val() < 0) {
                    $(this).val('0.00');
                }
            });
        });

        $(".money", "#divPayrollDetail").each(function (newValue, oldValue) {
            //找百分比的input
            var perDom = $(this).prevAll(".per");

            if ($(this).val() > 100) {
                $(this).val("100.00");
            }

            if ($(this).val() < 0) {
                $(this).val('0.00');
            }

            $(this).off("keyup.emp").on("keyup.emp", function (event) {
                if ($(this).val())
                    GoEmployeeEdit.payrollCalculate(perDom, this, 2);
            });

            $(this).off("keydown.emp").on("keydown.emp", function (event) {
                if ($(this).val() < 0) {
                    $(this).val('0.00');
                }
            });
        });

        $("#tbxEmpEmail").off("keyup.emp").on("keyup.emp", function (e) {
            if (e.keyCode == 32) {
                $("#tbxEmpEmail").val($.trim($("#tbxEmpEmail").val()));
            }
        });

        $("#aCopyPostal").off("click").on("click", function () {
            //目前有6个字段
            var fieldCount = 6;
            //post field class prefix
            var postalClass = "postal";
            //physical field calss prefx
            var pyhsicalClass = "physical";
            //遍历赋值
            for (var i = 1; i <= fieldCount; i++) {
                //postal input
                var $postalInput = $("." + postalClass + i);
                //physical input
                var $physicalInput = $("." + pyhsicalClass + i);
                //多语言信息
                var langData = $postalInput.getLangEditorData();
                if (langData) {
                    //替换FieldName
                    var langDataTemp = jQuery.extend(true, {}, langData);
                    var fieldName = $physicalInput.attr("name");
                    langDataTemp.MFieldName = fieldName;

                    $physicalInput.initLangEditor(langDataTemp);
                }
                else {
                    //赋值
                    $physicalInput.val($postalInput.val());
                }
            }
            //对国家赋值
            var mpCountryId = $("#selMPCountryID").combobox("getValue");
            $("#selMRealCountryID").combobox("setValue", mpCountryId);
        });
    },
    initLog: function () {
        if (GoEmployeeEdit.isUpdate) {
            BusLog.bindGrid(employeeId);
        }
    },
    initTab: function (idx) {
        $("#divFinDetail").hide();
        $("#divPayrollDetail").hide();
        $(".tabs").find("li:eq(" + idx + ")").click();
        $('#tabEmployee').tabs({
            onSelect: function (title, index) {
                if (index == 0) {
                    $("#divEmpInfo").show();
                    $("#divFinDetail").hide();
                    $("#divPayrollDetail").hide();
                }
                else if (index == 1) {
                    $("#divEmpInfo").hide();
                    $("#divFinDetail").show();
                    $("#divPayrollDetail").hide();
                } else {
                    $("#divEmpInfo").hide();
                    $("#divFinDetail").hide();
                    $("#divPayrollDetail").show();

                    if (!GoEmployeeEdit.IsLoadedPoyroll) {
                        //获取参数失败
                        var failMsg = HtmlLang.Write(LangModule.BD, "GetPaySettingFail", "Failed to get set parameters! you need to set up company payroll parameters");
                        $.mDialog.warning(failMsg);
                    }
                }
            }
        });
    },
    //起征点金额改变时触发
    pitChange: function () {
        $.each($("#ulPITThreshold li input:text"), function (i, txt) {
            Megi.attachPropChangeEvent(txt, function () {
                //如果起征点为空，则设置为默认值
                if (!$(txt).val()) {
                    $(txt).val($(txt).attr("defValue"));
                }
            });
        });
    },
    saveClick: function () {
        $("#aSave").off("click.emp").on("click.emp", function () {

            //ID特殊校验
            if ($("#MIDType").combobox("getValue")) {
                $('#MIDNumber').validatebox({
                    required: true
                });
            } else {
                $('#MIDNumber').validatebox({
                    required: false
                });
            }
            if (!$("body").mFormValidate()) {
                //检查是否员工名称未填写，没有跳转到这个页签
                var fName = $("input[name='MFirstName']").val();
                var lName = $("input[name='MLastName']").val();
                if (!fName || !lName) {
                    $("#tabEmployee").tabs("select", 0);
                }
                return;
            } else if (!$('#MIDNumber').validatebox("isValid")) {
                $("#tabEmployee").tabs("select", 2);
                return;
            }
            var toUrl = $(this).attr("href");
            var obj = {};

            if (GoEmployeeEdit.isEnableGL) {

                obj.MCurrentAccountCode = $("#cbxCurrentMoney").combobox("getValue");
                obj.MExpenseAccountCode = ""; //$("#cbxExpense").combobox("getValue");
            }

            var msg = HtmlLang.Write(LangModule.BD, "ThresholdModifyConfirm", "修改的起征点会更新至草稿状态的工资表，如数据不符请手动至工资表进行修改！");
            //如果起征点有修改，则提示会更新草稿工资单的个税金额
            if (GoEmployeeEdit.isThresholdChanged()) {
                $.mDialog.alert(msg, function () {
                    GoEmployeeEdit.saveEmployee(obj);
                }, 0, false, true);
            }
            else {
                GoEmployeeEdit.saveEmployee(obj);
            }

            return false;
        });
    },
    //判断个税起征点是否有改动
    isThresholdChanged: function () {
        var isChanged = false;

        $.each($("#ulPITThreshold li input:text"), function (i, txt) {
            if ($(txt).val() != $(txt).attr("oriValue")) {
                isChanged = true;
            }
        });

        return isChanged;
    },
    //保存员工信息
    saveEmployee: function (obj) {
        obj.PayrollDetail = GoEmployeeEdit.getPayrollDetatilModel();
        $("body").mFormSubmit({
            url: "/BD/Employees/EmployeesUpdate", param: { model: obj }, callback: function (msg) {
                if (msg.Success) {
                    //保存员工的ID
                    var employeeID = msg.ObjectID;

                    //提交员工薪酬数据
                    var Empyesuccess = HtmlLang.Write(LangModule.BD, "EditEmployeSuccess", "Operation Successfully");
                    var MFirstName = $("input[name='MFirstName']").val();
                    var MLastName = $("input[name='MLastName']").val();
                    $.mMsg(Empyesuccess + MLastName + MFirstName);

                    if (parent.GoEmployeesList) {
                        parent.GoEmployeesList.reload();
                    }
                    $.mDialog.close(0, employeeID);

                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
    //设置员工薪酬数据
    initPayrollDetail: function (model) {
        //编辑
        if (model.MItemID) {
            $("#payrollDetailID").val(model.MItemID);

            if (mDate.parse(model.MJoinTime).getFullYear() <= 1901) {
                model.MJoinTime = "";
            }
            $('#MJoinTime').datebox("setValue", model.MJoinTime);

            $("#MBaseSalary").numberbox('setValue', model.MBaseSalary);
            $("#MIDType").combobox("setValue", model.MIDType);
            $("#MIDNumber").val(model.MIDNumber);
            $("#MSocialSecurityBase").numberbox("setValue", model.MSocialSecurityBase);
            $("#MSocialSecurityAccount").val(model.MSocialSecurityAccount);
            $("#MRetirementSecurityPercentage").numberbox('setValue', model.MRetirementSecurityPercentage);
            $("#MRetirementSecurityAmount").numberbox('setValue', model.MRetirementSecurityAmount);
            $("#MMedicalInsurancePercentage").numberbox('setValue', model.MMedicalInsurancePercentage);
            $("#MMedicalInsuranceAmount").numberbox('setValue', model.MMedicalInsuranceAmount);
            $("#MUmemploymentPercentage").numberbox('setValue', model.MUmemploymentPercentage);
            $("#MUmemploymentAmount").numberbox('setValue', model.MUmemploymentAmount);
            $("#MHosingProvidentFundBase").numberbox("setValue", model.MHosingProvidentFundBase);
            $("#MProvidentAccount").val(model.MProvidentAccount);
            $("#MProvidentPercentage").numberbox('setValue', model.MProvidentPercentage);
            $("#MProvidentAmount").numberbox('setValue', model.MProvidentAmount);
            $("#MProvidentAdditionalPercentage").numberbox('setValue', model.MProvidentAdditionalPercentage);
            $("#MProvidentAdditionalAmount").numberbox('setValue', model.MProvidentAdditionalAmount);
            GoEmployeeEdit.IsLoadedPoyroll = true;
        }
        //新增
        else {
            GoEmployeeEdit.autoLoadPayrollParam();
        }
    },
    //获取个税起征点列表数据
    getPITThresholdList: function (employeeId) {
        var ret = [];
        $.each($("#ulPITThreshold li.input"), function (i, tr) {
            var txt = $(this).find("input:text");
            var item = { MItemID: txt.attr("itemId"), MEmployeeID: employeeId, MEffectiveDate: txt.attr("period"), MAmount: txt.val(), MDefaultAmount: txt.attr("defValue") };
            if (!item.MAmount) {
                item.MAmount = item.MDefaultAmount;
            }
            ret.push(item);
        });

        return ret;
    },
    getPayrollDetatilModel: function (employeeID) {
        var model = {};
        model.MItemID = $("#payrollDetailID").val();
        model.MEmployeeID = $("#EmployeeID").val();
        model.MJoinTime = $('#MJoinTime').datebox('getValue');
        model.MBaseSalary = $("#MBaseSalary").numberbox("getValue");
        model.MIDType = $("#MIDType").combobox("getValue");
        model.MIDNumber = $("#MIDNumber").val();
        model.MSocialSecurityAccount = $("#MSocialSecurityAccount").val();
        model.MRetirementSecurityPercentage = $("#MRetirementSecurityPercentage").val();
        model.MRetirementSecurityAmount = $("#MRetirementSecurityAmount").val();
        model.MMedicalInsurancePercentage = $("#MMedicalInsurancePercentage").val();
        model.MMedicalInsuranceAmount = $("#MMedicalInsuranceAmount").val();
        model.MUmemploymentPercentage = $("#MUmemploymentPercentage").val();
        model.MUmemploymentAmount = $("#MUmemploymentAmount").val();
        model.MProvidentAccount = $("#MProvidentAccount").val();
        model.MProvidentPercentage = $("#MProvidentPercentage").val();
        model.MProvidentAmount = $("#MProvidentAmount").val();
        model.MProvidentAdditionalPercentage = $("#MProvidentAdditionalPercentage").val();
        model.MProvidentAdditionalAmount = $("#MProvidentAdditionalAmount").val();
        model.MHosingProvidentFundBase = $("#MHosingProvidentFundBase").numberbox("getValue");
        model.MSocialSecurityBase = $("#MSocialSecurityBase").numberbox("getValue");
        model.PITThresholdList = GoEmployeeEdit.getPITThresholdList(model.MEmployeeID);

        return model;
    },
    //自动载入组织的设置薪酬参数参数
    autoLoadPayrollParam: function () {
        //如果参数为空，则先获取参数
        if (!GoEmployeeEdit.PayrollParam) {
            var url = "/PA/PayrollBasic/GetPaySettingModel";
            mAjax.post(
                url,
                {},
                function (msg) {
                    if (msg) {
                        GoEmployeeEdit.PayrollParam = msg;
                        GoEmployeeEdit.payrollDetatilSet();
                        GoEmployeeEdit.IsLoadedPoyroll = true;
                    }
                    else {
                        GoEmployeeEdit.payrollDetatilSet();
                    }
                }, "");
        } else {
            GoEmployeeEdit.payrollDetatilSet();
        }
    },
    //根据参数计算结果
    payrollDetatilSet: function () {
        var param = GoEmployeeEdit.PayrollParam;
        var baseSalary = parseFloat($("#MBaseSalary").val());
        var socialSecurityBase = parseFloat($("#MSocialSecurityBase").numberbox("getValue"));  //社保基数
        var hosingProvidentFundBase = parseFloat($("#MHosingProvidentFundBase").numberbox("getValue")); //住房公积金基数

        var RSPDom = $("#MRetirementSecurityPercentage");
        var RSPOldValue = RSPDom.numberbox("getValue");
        if (!RSPOldValue || RSPOldValue <= 0) {
            $(RSPDom).numberbox('setValue', param.MEmpRetirementSecurityPer);
        }

        var MIPDom = $("#MMedicalInsurancePercentage");
        var MIPOldValue = $(MIPDom).numberbox('getValue');
        if (!MIPOldValue || MIPOldValue <= 0) {
            MIPDom.numberbox('setValue', param.MEmpMedicalInsurancePer);
        }

        var UPDom = $("#MUmemploymentPercentage");
        var UPOldValue = $(UPDom).numberbox('getValue');
        if (!UPOldValue || UPOldValue <= 0) {
            UPDom.numberbox('setValue', param.MEmpUmemploymentInsurancePer);
        }

        var PPDom = $("#MProvidentPercentage");
        var PPOldValue = $(PPDom).numberbox('getValue');
        if (!PPOldValue || PPOldValue <= 0) {
            PPDom.numberbox('setValue', param.MProvidentFundPer);
        }

        var PAPDom = $("#MProvidentAdditionalPercentage");
        var PAPOldValue = $(PAPDom).numberbox('getValue');
        if (!PAPOldValue || PAPOldValue <= 0) {
            PAPDom.numberbox('setValue', param.MAddProvidentFundPer);
        }

        //有基数的时候才进行计算
        if (socialSecurityBase >= 0) {
            $("#MRetirementSecurityAmount").numberbox('setValue', ($(RSPDom).numberbox("getValue") / 100 * socialSecurityBase).toFixed(2));
            $("#MMedicalInsuranceAmount").numberbox('setValue', ($(MIPDom).numberbox('getValue') / 100 * socialSecurityBase).toFixed(2));
            $("#MUmemploymentAmount").numberbox('setValue', ($(UPDom).numberbox('getValue') / 100 * socialSecurityBase).toFixed(2));
        }

        if (hosingProvidentFundBase >= 0) {
            $("#MProvidentAmount").numberbox('setValue', ($(PPDom).numberbox('getValue') / 100 * hosingProvidentFundBase).toFixed(2));
            $("#MProvidentAdditionalAmount").numberbox('setValue', ($(PAPDom).numberbox('getValue') / 100 * hosingProvidentFundBase).toFixed(2));
        }


    },
    //员工工资项计算
    payrollCalculate: function (selector, selectorMoney, type) {
        var baseSalary = parseFloat($("#MBaseSalary").val());
        //取基数
        var className = $(selector).attr("class");
        var base = 0;
        //表示是公积金相关的input
        if (className.indexOf("hpf") >= 0) {
            base = parseFloat($("#MHosingProvidentFundBase").numberbox("getValue"));
        } else {
            //社保基数
            base = parseFloat($("#MSocialSecurityBase").numberbox("getValue"));
        }

        //表示第一个selector做了更改
        if (type == 1) {
            var per = $(selector).val();
            var money = base * per / 100;

            $(selectorMoney).numberbox("setValue", money.toFixed(2));
        } else if (type == 2 && base > 0) {
            var money = $(selectorMoney).val();

            var per = money / base * 100;
            $(selector).numberbox("setValue", per);
        } else {
            $(selector).numberbox("setValue", 0.00);
        }
    }

}


$(document).ready(function () {
    GoEmployeeEdit.init();
});