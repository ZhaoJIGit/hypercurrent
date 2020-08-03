(function () {
    var GLExplanationEdit = (function () {

        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = 10;
        //总记录数
        var totalCount = 0;

        var GLExplanationEdit = function () {
            //分页div
            var _pager = ".re-pager-div";
            //保存按钮
            var _save = ".re-save-button";
            //取消按钮
            var _cancel = ".re-cancel-button";
            //文本框
            var _textarea = ".re-content-textarea";
            //
            var _new = ".re-new-button";
            //摘要内容
            var _content = ".content";
            //表格div
            var _tableDiv = ".re-list-div";
            //表格
            var _table = ".re-explanation-table";
            //编辑
            var _editSpan = ".re-edit-span";
            //删除
            var _deleteSpan = ".re-delete-span";
            //获取摘要的url
            var getExplanationUrl = "/GL/GLVoucher/GetVoucherExplanationPageList";
            //删除
            var deleteExplanationUrl = "/GL/GLVoucher/DeleteVoucherExplanation";
            //保存
            var updateExplanationUrl = "/GL/GLVoucher/UpdateVoucherExplanation";
            //
            var contentCannotBeNull = HtmlLang.Write(LangModule.GL, "ExplanationContentCannotBeNull", "摘要内容不可为空");
            //
            var that = this;

            //初始化翻页事件
            this.initPagerEvent = function () {
                //调用easyui组件
                $(_pager).pagination({
                    total: totalCount,
                    onChangePageSize: function (size) {
                        pageSize = size;
                        that.search();
                    },
                    onRefresh: function (number, size) {
                        pageIndex = number;
                        pageSize = size;
                        that.search();
                    },
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = size;
                        that.search();
                    }
                });
            };

            //开始进来的时候查找一下
            this.search = function () {
                //异步获取
                mAjax.post(getExplanationUrl,
                    {
                        filter: {
                            page: pageIndex,
                            rows: pageSize
                        }
                    }, function (data) {
                        // 
                        data = data || [];
                        //
                        totalCount = data.total;
                        //初始化分页控件
                        that.initPagerEvent();
                        //
                        that.bindData2Table(data.rows);
                    });
            };
            //将查询到的数据绑定到表格
            this.bindData2Table = function (data) {
                //获取表体
                var body = $("tbody", _table);
                //
                var rowsHtml = "";
                //
                for (var i = 0; i < data.length; i++) {
                    rowsHtml += "<tr itemid='" + data[i].MItemID + "'><td style='text-align:center;'>" + (i + 1) + "</td><td class='content'>" + mText.encode(data[i].MContent) + "</td><td class='operate'><span class='re-edit-span'></span><span class='re-delete-span'></span></td></tr>";
                }
                //
                body.empty().append(rowsHtml);
                //内部事件
                that.bindTableEvent(body);
            };
            //初始化表格内部的事件
            this.bindTableEvent = function (body) {
                //
                $("tr", body).each(function (index) {
                    //
                    var tr = $(this);
                    //编辑
                    $(_editSpan, tr).off("click").on("click", function () {
                        //
                        that.edit(tr);
                    });
                    //删除
                    $(_deleteSpan, tr).off("click").on("click", function () {
                        //
                        that.delete(tr.attr("itemid"));
                    });
                    //
                    $(".content", this).off('click').on("click", function () {
                        //
                        that.edit(tr);
                    });
                });
            };
            //保存一条摘要
            this.save = function () {
                //获取itemid
                var itemID = $(_textarea).attr("itemID");
                //
                var content = $(_textarea).val();
                //如果没有内容，则不做保存
                if (!content) {
                    //
                    $.mDialog.message(contentCannotBeNull);
                    //直接返回
                    return false;
                }
                //
                mAjax.submit(updateExplanationUrl, { itemID: itemID, content: content }, function (data) {
                    //如果保存成功
                    if (data.Success) {
                        //提示保存成功
                        mDialog.message(LangKey.SaveSuccessfully);
                        //刷新页面
                        that.search();
                    }
                    else {
                        //提示操作失败
                        var failedMsg = HtmlLang.Write(LangModule.Common, "SaveFailed", "Save Failed!");
                        //提醒失败
                        mDialog.error(failedMsg);
                    }
                }, "", true);
            };
            //取消编辑
            this.cancel = function () {
                //清除内容
                $.mDialog.close();
            };
            //新建一个
            this.new = function () {
                //清除内容
                $(_textarea).val("").attr("itemID", "");
                //
                $(".re-selected").removeClass("re-selected");
            }
            //删除一条摘要
            this.delete = function (itemID) {
                //先提醒用户是否确定删除
                mDialog.confirm(LangKey.AreYouSureToDelete, function () {
                    //删除
                    mAjax.submit(deleteExplanationUrl, {
                        itemID: itemID
                    }, function (data) {
                        //
                        if (data.Success) {
                            //提醒用户删除成功
                            mDialog.message(LangKey.DeleteSuccessfully);
                            //整个页面更新
                            that.search();
                        }
                        else {
                            //提醒用户删除失败
                            mDialog.error(HtmlLang.Write(LangModule.Common, "deletefialed", "Delete Failed!"));
                        }
                    }, "", true);
                });
            }
            //编辑一条摘要
            this.edit = function (tr) {
                //把内容放到textarea里面去
                $(_textarea).val($(_content, tr).text()).attr("itemID", tr.attr("itemID"));
                //
                $(".re-selected").removeClass("re-selected");
                //
                $(tr).addClass("re-selected");
            };
            //重置分页参数
            this.resetPager = function () {
                //初始化的时候，要把参数都置位原始值
                totalCount = 0;
                //
                pageIndex = 1;
                //
                pageSize = 10;
            };
            //初始化一些高度
            this.initDomSizeValue = function () {
                //
                $(_tableDiv).height($("body").height() - $(_tableDiv).offset().top - $("table", _pager).height() - 10);
            };
            //
            this.initEvent = function () {
                //保存
                $(_save).off("click").on("click", that.save);
                //取消
                $(_cancel).off("click").on("click", that.cancel);
                //编辑
                $(_new).off("click").on("click", that.new);
            };
            this.init = function () {
                //初始化分页控件
                that.initPagerEvent();
                //
                that.initEvent();
                //
                that.initDomSizeValue();
                //查找一下
                that.search();
            }
        };

        return GLExplanationEdit;
    })();
    //
    window.GLExplanationEdit = GLExplanationEdit;
})()