

(function () {

    var FCHome = (function () {

        var FCHome = function () {

            //tab
            var tab = ".fc-main-tabs";
            //定义各个tab对应的index
            var voucher = 0, cashCoding = 1, fapiao = 2;
            //
            var item = HtmlLang.Write(LangModule.Common, "ModuleItemCount", "张");
            //
            var items = HtmlLang.Write(LangModule.Common, "ModuleItemsCount", "张");
            //
            var that = this;

            //
            var getModuleInfoUrl = "/FC/FCHome/GetModuleInfo";
            //初始化Tab
            this.initTab = function (index, subIndex) {
                //初始化先隐藏tab1和2的内容
                $(".fc-partial-1").hide();
                $(".fc-partial-2").hide();
                //初始化
                $(tab).tabsExtend({
                    //默认的显示标签
                    initTabIndex: index,
                    //选择标签函数
                    onSelect: function (index) {
                        //
                        that.showTab(index, subIndex);
                    }
                });
            };
            //显示tab
            this.showTab = function (index, subIndex) {
                //加上class
                $("ul.tab-links").find("li:eq(" + index + ")").addClass("current").siblings().removeClass("current");
                //当前需要显示的内容
                var currentPartial = $(".fc-partial-" + index);
                //是否已经初始化，强制每一个tab页切换都更新，否则可能出现数据不同步的情况
                var inited = currentPartial.attr("inited") == "1" && false;
                //根据index的不同，控制partial的显示
                currentPartial.show().siblings().hide();

                index = parseInt(index || 0);

                if ($(".fc-partial-0").length == 0) {
                    index++;
                    $(".fc-partial-1").show()
                }
                //初始化
                switch (index) {
                    //先控制显示
                    case voucher:
                        //
                        !inited && new FCVoucherModuleList().init();
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        break;
                    case cashCoding:
                        //如果没有初始化，则初始化
                        !inited && new FCCashCodingModuleList().init();
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        //
                        break;
                    case fapiao:
                        //如果没有初始化，则初始化
                        !inited && new FCFapiaoModuleList().init(subIndex);
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        //
                        break;
                    default:
                        break;
                }
            };
            //获取模板的数量
            this.updateModuleInfo = function () {
                //
                mAjax.post(getModuleInfoUrl, {}, function (data) {

                    that.initTabTitle(0, data[0] > 1 ? (data[0] + " " + items) : (data[0] + " " + item));
                    that.initTabTitle(1, data[1] > 1 ? (data[1] + " " + items) : (data[1] + " " + item));
                    that.initTabTitle(2, data[2] > 1 ? (data[2] + " " + items) : (data[2] + " " + item));
                });
            };
            //修改tab的名字
            this.initTabTitle = function (index, title) {
                //
                $(".fc-main-tabs li[index='" + index + "'] .title").text(title);
            };
            //初始化
            this.init = function (index, subIndex) {
                //初始化Tab
                that.initTab(index, subIndex);
                //
                that.showTab(index, subIndex);
                //更新模板数
                that.updateModuleInfo();
            };

            // #凭证模板 开始

            //删除凭证模板
            this.deleteVoucherModule = function (itemIDs, func) {
                //必须有勾选在做删除
                if (itemIDs.length > 0) {
                    //先提醒用户是否确定删除
                    mDialog.confirm(LangKey.AreYouSureToDelete, function () {
                        //删除
                        mAjax.submit(deleteVoucherUrl, {
                            ids: itemIDs
                        }, function (data) {
                            //提醒用户删除成功
                            mDialog.message(LangKey.DeleteSuccessfully);
                            //整个页面更新
                            $.isFunction(func) && func(data);
                        }, "", true);
                    });
                }
            };

            //导出凭证模板
            this.exportVoucher = function (params) {
                //
                mWindow.reload(exportVoucherModuleUrl + escape($.toJSON(params)));
                //显示正在导出
                mDialog.message(exportLang);
            };

            //导入凭证模板
            this.importVoucherModule = function () {
                ImportBase.showImportBox('/BD/Import/Import/VoucherModule', importLang, 900, 520);
            };

            //打印凭证模板
            this.printVoucherModule = function (itemID) {
                //
                var param = { MItemID: itemID };
                //弹出框
                Megi.dialog({
                    title: voucherLang,
                    top: window.pageYOffset || document.documentElement.scrollTop,
                    width: 1060,
                    height: 560,
                    href: printVoucherModuleUrl + escape($.toJSON(param))
                });
            };
            // #凭证模板 结束
        };

        //返回
        return FCHome;
    })();
    //
    window.FCHome = FCHome;
    //
    $.extend(FCHome, {
        Number2CN: function DX(n) {
            if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n))
                return "数据非法";
            var unit = "仟佰拾亿仟佰拾万仟佰拾元角分", str = "";
            n += "00";
            var p = n.indexOf('.');
            if (p >= 0)
                n = n.substring(0, p) + n.substr(p + 1, 2);
            unit = unit.substr(unit.length - n.length);
            for (var i = 0; i < n.length; i++)
                str += '零壹贰叁肆伍陆柒捌玖'.charAt(n.charAt(i)) + unit.charAt(i);
            return str.replace(/零(仟|佰|拾|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|元)/g, "$1").replace(/(亿)万|壹(拾)/g, "$1$2").replace(/^元零?|零分/g, "").replace(/元$/g, "元整");
        },
        toVoucherNumber: function (number) {
            //
            if (!number) {
                return "";
            }
            //
            number = parseInt(number.trimStart("0"));
            if (number < 10) {
                return "00" + number;
            }
            else if (number < 100) {
                return "0" + number;
            }
            return number;
        },
        //表格分页数量
        pageSize: 20
    });
})()