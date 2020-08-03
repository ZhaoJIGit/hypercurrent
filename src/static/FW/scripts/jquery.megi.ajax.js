/*
1.这是处理用户ajax请求的方法
2.如果用户请求的过程中发现用户已经没有权限了，则需要跳转到登陆页面
3.登陆完成后页面会关闭
4.这部分的代码要在jquery.megi.common.js前加载
*/
;
(function (w) {

    var mAjax = (function () {
        var mAjax = function () {
            //
            var that = this;

            //避免重名 felson 2016.4.17
            w.aJaxSubmitArray = [];

            var tokenName = 'hiddenToken';
            var pageIDName = "hiddenPageID"

            //检测是否重复提交
            this.checkResubmit = function (options) {
                //
                return _.indexOfArray(aJaxSubmitArray, options) >= 0;
            },
                //提交成功后删除提交信息 felson
                this.removeAjaxSbumit = function (options) {
                    //
                    aJaxSubmitArray = _.removeFromArray(aJaxSubmitArray, options);
                },
                //Post方法 offEvent表示是否取消所有当前页面的事件
                /*
                    url:请求的URL
                    param:参数 object对象
                    sucess:请求成功的回调函数
                    error:请求失败的回调函数
                    mask:请求的时候是否要遮罩页面
                    async:是都是异步请求，默认为 true
                    checkResubmit:是否不用检查重复提交，默认为false
                */
                this.Post = this.post = function (url, param, success, error, mask, async, checkResubmit, zipData) {

                    param = param || {};

                    //请求的类型
                    var requestOptions = {
                        url: url,
                        param: $.extend(true, {}, param)
                    };

                    //如果需要检查异步重复提交的问题
                    var input = $("input[name='" + tokenName + "']");
                    var pageid = input.attr("pageid");
                    var hiddenToken = input.val();

                    var postData = $.toJSON(param);

                    //数据超过1M，默认走压缩
                    var maxZipLength = 1024 * 1024 * 1;

                    zipData = zipData === true || (zipData !== false && postData.length >= maxZipLength);

                    //如果需要对数据进行压缩
                    if (zipData === true) {
                        postData = $.toJSON({
                            data: mZip.zip(postData),
                            zip: true
                        })
                    }

                    var options = {
                        type: "post",
                        url: url,
                        contentType: 'application/json; charset=utf-8',
                        data: postData,
                        async: async !== false,
                        beforeSend: function (XMLHttpRequest) {
                            !!pageid && XMLHttpRequest.setRequestHeader(pageIDName, pageid);
                            if (!!hiddenToken && checkResubmit) {
                                XMLHttpRequest.setRequestHeader(tokenName, hiddenToken);
                            }
                        },
                        success: function (response) {
                            //
                            that.removeAjaxSbumit(requestOptions);
                            //即使页面抛出异常，只要返回的里面有新的token，都要进行更新
                            that.setTokenValue(response);

                            //
                            if (mask) {
                                //取消遮罩
                                $("body").unmask();
                            }

                            //如果statusCode是 250（model不合法） 或者 251（登陆失败）
                            if (response && response.accessDenied === 1) {
                                //弹出登陆框
                                top.showLoginDialog(response.type, function () { });
                            } else if (response && response.RequstExcetion) {
                                that.removeAjaxSbumit(requestOptions);
                                $("body").unmask();
                                $("html").unmask();

                                var tipsOptions = {
                                    mWidth: 600,
                                    mHeight: 500,
                                    mShowbg: true,
                                    mContent: "iframe:" + response.ErrorUrl + "&showclose=1",
                                    mNoAuth: true,
                                    mShowTitle: false,
                                    mBoxID: "errorPage"
                                };

                                top.$.mDialog.show(tipsOptions);
                                $(".m-notfound-img", "body").css("height", "70%");
                                $(".m-error-parentdiv").css("display", "block");
                            }
                            else if (response && response.lengthOutOfRange === 1) {
                                //组装的html
                                var html = "";
                                //
                                var list = response.list;
                                //如果提交的时候有字段长度超出的情况发生
                                for (var i = 0; i < list.length; i++) {
                                    //组装html
                                    html += list[i] + "<br>";
                                }
                                //目前的处理逻辑，暂且不添加，可接受用户自定义的异常处理逻辑
                                $.isFunction(error) && error(response);
                                //弹出提醒吧
                                top.$.mDialog.error(html, function () { }, true);
                            }
                            else if (response && response.IsJsonResult) {
                                //如果校验失败的话，就直接返回了
                                if (new mError().checkResultValid(response)) {
                                    //
                                    var data = response.Data;

                                    window.mFilter.doFilter("track", [url, param, data]);

                                    //调用回掉函数
                                    $.isFunction(success) && success(data);
                                    //去掉校验
                                    w.noCancelValidate !== true && $("body").mCancelValidateClass();

                                }

                            } else {

                                var data = !!response.Data ? response.Data : response;

                                window.mFilter.doFilter("track", [url, param, data]);

                                //调用回掉函数
                                $.isFunction(success) && success(data);

                                //去掉校验
                                w.noCancelValidate !== true && $("body").mCancelValidateClass();
                            }
                        },
                        error: function (response) {
                            //
                            that.removeAjaxSbumit(requestOptions);
                            //
                            $("body").unmask();
                            $("html").unmask();

                            //top.$.mDialog.error(HtmlLang.Write(LangModule.Common, "RequestTimeout", "Request timeout, Please try again"));
                            //目前的处理逻辑，暂且不添加，可接受用户自定义的异常处理逻辑
                            $.isFunction(error) && error(response);

                        }
                    };
                    //检测是否重复提交了
                    if (checkResubmit && that.checkResubmit(requestOptions)) {
                        //
                        console.warn("美记研发组提醒：发现重复提交问题，具体url如下:" + url + ",具体的请求参数如下:" + JSON.stringify(param));
                        //
                        return false;
                    }
                    else {
                        //加入已经提交的序列
                        aJaxSubmitArray.push(requestOptions);
                    }
                    //
                    if (mask) {
                        $("body").mask("");
                    }
                    //提交
                    $.ajax(options);
                };

            /*
                设置token的值
            */
            this.setTokenValue = function (data) {

                if (!!data) {
                    //先更改token
                    if (data[tokenName] && data[tokenName].length > 0) {

                        $("input[name='" + tokenName + "']").val(data[tokenName])
                    }
                }
            }

            /*
                url:请求的URL
                param:参数 object对象
                sucess:请求成功的回调函数
                error:请求失败的回调函数
                nomask:请求的时候是否要遮罩页面 默认遮罩
                noasync:是都是异步请求，默认为异步
                nocheckResubmit:是否不用检查重复提交，默认为检查
                正常情况下，只需要 url param,sucess就行，默认 遮罩 异步 检查 重复提交
            */
            this.Submit = this.submit = function (url, param, success, error, nomask, noasync, nocheckResubmit, zipData) {
                //
                return that.post(url, param, success, error, nomask !== true, noasync !== true, nocheckResubmit !== true, zipData);
            }
            //application/x-www-form-urlencoded 形式发送数据
            this.FormGet = this.formPost = function (url, data, success, error) {
                //直接异步请求
                $.ajax({
                    type: "post",
                    url: url,
                    contentType: 'application/x-www-form-urlencoded;charset=utf-8',
                    data: data ? $.toJSON(data) : "",
                    //crossDomain: true,
                    success: function (response) {

                        d = response.Data;

                        //如果不成功，并且是重登陆，则打开重登陆的框
                        if (d && !d.Success && d.Relogin) {
                            //获取最顶层的windows
                            //弹出登陆框
                            $(w).mTopWindow.showLoginDialog();
                            //
                            if (error && error != undefined) {
                                error(d);
                            }
                        }
                        else if (d && success != undefined) {
                            //调用回掉函数
                            success(d);
                        }
                        //去掉校验
                        w.noCancelValidate !== true && $("body").mCancelValidateClass();
                    },
                    error: function (response) {
                        //目前的处理逻辑，暂且不添加，可接受用户自定义的异常处理逻辑
                        $.isFunction(error) && error(response);
                    }
                });
            };
            //Get方法
            this.Get = this.get = function (url, data, success, error) {
                $.ajax({
                    type: "get",
                    url: url,
                    contentType: 'application/json; charset=utf-8',
                    data: data ? $.toJSON(data) : "",
                    success: function (response) {

                        var d = response.Data;

                        //如果不成功，并且是重登陆，则打开重登陆的框
                        if (d && d.accessDenied === 1) {
                            //弹出登陆框
                            top.showLoginDialog(function () { mAjax.Get(url, data, success, error); });
                        }
                        else if ($.isFunction(success)) {
                            //调用回掉函数
                            success(d);
                        }
                        //去掉校验
                        w.noCancelValidate !== true && $("body").mCancelValidateClass();
                    },
                    error: function (response) {
                        //目前的处理逻辑，暂且不添加，可接受用户自定义的异常处理逻辑
                        $.isFunction(error) && error(response);
                    }
                });
            }
        }
        return mAjax;
    })();
    $.mAjax = w.mAjax = new mAjax();
})(window)
