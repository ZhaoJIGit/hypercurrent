var Register = {
    PasswordWarpper: new PassworWrapper(),
    lastEventTime: null,
    init: function () {
        //同意条款checkbox点击事件
        $("#ckAgreeTerm").off("click").on("click", function () {
            $("#GetStart").unbind("click");
            if ($(this).attr("checked") == "checked") {
                $("#GetStart").removeClass("m-btn-disabled");
                $("#GetStart").click(function () {
                    Register.startRegister();
                });
            }
            else {
                $("#GetStart").removeClass("m-btn-disabled").addClass("m-btn-disabled");
            }
        });
        
        //整个页面的的input（text 和button 都响应回车事件)
        $("input").on("keyup", function (event) {
            //如果是回车
            if (event.keyCode == 13) {
                //注册
                $("#GetStart").trigger("click");
                //返回失败
                return false;
            }
        });

        $("#MEmailAddress").keyup(function (e) {
            if (e.keyCode == 32) {
                $("#MEmailAddress").val($.trim($("#MEmailAddress").val()));
            }
        });
        $("#GetStart").unbind("click").click(function () {
            Register.startRegister();
        });

        //整个页面的的input（text 和button 都响应回车事件)
        $("body").on("keyup", function (event) {
            //如果是回车
            if (event.keyCode == 13) {
                //注册
                Register.startRegister();
                //返回失败
                return false;
            }
        });

        $("#pwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            Register.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (Register.lastEventTime - e.timeStamp == 0) {
                    var password = $("#pwd").val();

                    if (!Register.PasswordWarpper.isValidate(password)) {
                        $("#pwd_tips").css("display", "block");
                    } else {
                        $("#pwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });

        $("#rpwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            Register.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (Register.lastEventTime - e.timeStamp == 0) {
                    var password = $("#rpwd").val();

                    if (!Register.PasswordWarpper.isValidate(password)) {
                        $("#rpwd_tips").css("display", "block");
                    } else {
                        $("#rpwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });
    },
    startRegister: function () {
        //因为初始禁用了，现在开启，防止用户只点击注册按钮也可提交的问题
        $("#MEmailAddress").val($.trim($("#MEmailAddress").val()))
        $('input.easyui-validatebox').validatebox('enableValidation')
        if (!$("#divUserRegister").form('validate')) {
            return;
        }

        //邀请注册才需要进行秘密校验
        if ($("#hideOrgName").val()) {
            var newPassword = $("#pwd").val();
            var rNewPassword = $("#rpwd").val();

            if (newPassword != rNewPassword) {
                var message = HtmlLang.Write(LangModule.Login, "PasswordnotMatch", "Two password that your Input must be the same!");
                $.mDialog.alert(message);
                return;
            }

            if (!Register.PasswordWarpper.isValidate(newPassword)) {
                var message = HtmlLang.Write(LangModule.Common, "PasswordIsSimple", "密码必须包括大写字母，小写字母，数字和特殊字符！");
                $.mDialog.alert(message);
                return;
            }

        }

       

        var obj = {};
        obj.MFirstName = $("#MFirstName").val();
        obj.MLastName = $("#MLastName").val();
        obj.MEmailAddress = $("#MEmailAddress").val();
        obj.MMobilePhone = $("#MPhoneNumber").val();
        obj.MPassWord = $("#pwd").val();
        obj.MItemID = $("#MItemID").val();
        obj.MOrgID = $("#MOrgID").val();
        obj.DefaultEmail = $("#DefaultEmail").val();
        obj.SendLinkID = $("#SendLinkID").val();
        obj.PlanCode = $("#PlanCode").val();




        $("html").mask("");
        var toUrl = $(this).attr("href");
        $.mAjax.submit("/Account/UserRegister", obj, function (msg) {
            $("#MEmailAddress").val("");
            if (msg.Success) {
                 window.setTimeout(function () {
                    window.location = "/Account/Success/" + obj.MOrgID + "?email=" + obj.MEmailAddress;
                }, 100);
            } else {
                if (msg.Message) {
                    $.mDialog.alert(msg.Message);
                } else {
                    window.location = "/Account/RegisterFail";
                }
            }
            $("html").unmask();
        });
        return false;
    }

}

$(document).ready(function () {
    var scroll = true;
    //初始禁止验证错误
    //$('input.easyui-validatebox').validatebox('disableValidation')
    //       .focus(function () { $(this).validatebox('enableValidation'); })
    //       .blur(function () { $(this).validatebox('validate') });
    Register.init();
});