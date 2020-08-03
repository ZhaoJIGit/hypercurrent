/*银行类型编辑页面*/
(function () {

    var BDBankTypeEdit = (function () {

        var BDBankTypeEdit = function () {
            //
            var that = this;
            //获取一个空的银行帐号类型的url
            var getEmptyBDBankTypeUrl = "/BD/BDBank/GetBDBankTypeEditModel";
            //保存银行类型的url
            var saveBDBankTypeUrl = "/BD/BDBank/SaveBDBankType";
            //银行类型的多语言字段
            var langFeild = "MName";
            //头部
            var langFieldTop = ".lang-field-top";
            //输入模版的id
            var inputModel = "#bankTypeFieldModel";
            //多语言名称
            var fieldName = ".lang-field-name";
            //多语言输入框
            var fieldValue = ".lang-field-value";
            //根据用户选择的多语言类型，添加多个input
            this.initLangInput = function () {
                //获取多语言
                var lang = mLang.getLang();
                //输入的模版
                var $model = $(inputModel);
            };
            //加载空的银行类型到表格
            this.getBDBankTypeModel = function () {
                //
                mAjax.post(getEmptyBDBankTypeUrl, "", that.initBDBankTypeModel);
            };
            //初始化model到页面
            this.initBDBankTypeModel = function (model) {
                //把model绑定到body上
                $("body").data("langModel", model);
            };
            //保存银行类型
            this.saveBDBankType = function () {

                $("body").mFormSubmit({
                    url: saveBDBankTypeUrl, param: { model: {} }, callback: function () {
                        //取消遮罩
                        $("body").unmask();
                        //提醒保存成功
                        var title = LangKey.SaveSuccessfully;
                        //
                        $.mDialog.message(title);
                        //关闭弹窗
                        $.mDialog.close();
                    }
                });
            };
            //初始化入口
            this.init = function () {
                //点击保存的事件
                $("#btnSave").off("click").on("click", that.saveBDBankType);
                //初始化input
                that.initLangInput();
                //初始化数据
                that.getBDBankTypeModel();
            };
        }
        return BDBankTypeEdit;
    })();
    window.BDBankTypeEdit = BDBankTypeEdit;
})()