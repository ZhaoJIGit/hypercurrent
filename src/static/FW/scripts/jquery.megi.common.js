
//HtmlLang已经转移到 jquery.megi.top.js里面，因为其需要放在easyui前面


var clearMsgTimeoutId = null;
var Megi = {
    showMsg: function (msg) {
        top.$(".m-action-msg").remove();
        top.$("body").append("<div class='m-action-msg'></div>")
        top.$(".m-action-msg").html(msg);
        var winW = top.$(".m-wrapper").width();
        var msgW = top.$(".m-action-msg").width();
        var left = (winW - msgW) / 2;
        top.$(".m-action-msg").css({ "top": "5px", "left": left + "px" }).animate({ opacity: 'show' }, 'slow', function () { $(".m-action-msg").show(); });
        if (clearMsgTimeoutId != null) {
            clearTimeout(clearMsgTimeoutId);
        }
        clearMsgTimeoutId = setTimeout("Megi.hideMsg", 8000)
    },
    hideMsg: function () {
        top.$(".m-action-msg").remove();
    },
    removeTab: function (title) {
        top.MegiTab.removeTab(title);
    },
    addOrUpdate: function (title, url, isUqniue, show) {
        top.MegiTab.addOrUpdateTab(title, url, isUqniue, show);
        return false;
    },
    addTab: function (title, url, icon, isUnique) {
        top.MegiTab.add(title, url, icon, isUnique);
        return false;
    },
    setStable: function (iframe) {
        MegiTab.setStable(iframe);
    },
    show: function (msg, options, langModule, langKey) {
        Megi.createDialogMessage(msg, options, "Info", "info", false, langModule, langKey);
        Megi.closeDialogMessageEvent();
    },
    showSuccess: function (msg, options, langModule, langKey) {
        Megi.createDialogMessage(msg, options, "Success", "success", false, langModule, langKey);
        Megi.closeDialogMessageEvent();
    },
    alert: function (msg, options, langModule, langKey) {
        Megi.createDialogMessage(msg, options, "Error", "error", false, langModule, langKey);
        Megi.closeDialogMessageEvent();
    },
    warning: function (msg, options, langModule, langKey) {
        Megi.createDialogMessage(msg, options, "Warning", "warning", false, langModule, langKey);
        Megi.closeDialogMessageEvent();
    },
    confirm: function (msg, callback, options, langModule, langKey) {
        Megi.createDialogMessage(msg, options, "Confirm", "confirm", true, langModule, langKey);
        $("#divMessageContainer").find(".action>.m-btn-green").click(function () {
            $("#divMessageContainer").dialog("close");
            if (callback != null) {
                callback();
            }
        });
        $("#divMessageContainer").find(".action>.m-btn-gray").click(function () {
            $("#divMessageContainer").dialog("close");
        });
    },
    closeDialogMessageEvent: function () {
        $("#divMessageContainer").find(".action>.m-btn-green").click(function () {
            $("#divMessageContainer").dialog("close");
        });
    },
    request: function (name, value, url) {
        if (value == undefined) {

            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return "";
        }
        else {
            var pattern = "[\?|&]" + name + '=([^&]*)';
            var replaceText = name + '=' + value;
            if (url.match(pattern)) {
                var tmp = '/(' + name + '=)([^&]*)/gi';
                tmp = url.replace(eval(tmp), replaceText);
                return tmp;
            } else {
                if (url.match('[\?]')) {
                    return url + '&' + replaceText;
                } else {
                    return url + '?' + replaceText;
                }
            }
            alert(url + '\n' + name + '\n' + value);
            return url + '\n' + name + '\n' + value;
        }
    },
    getVersionUrl: function (url) {
        return Megi.request("ver", window.top.VersionNumber, url);
    },
    addTabTitle: function (url, title) {

        if (!url) return url;

        //如果没有传任何的过来，则需要根据iframe的来处理
        if (title == undefined) {

            var iframe = $(window.frameElement);
            if (iframe.length == 0) return url;

            var src = iframe.attr("src");
            if (!src) return url;

            var reg = new RegExp("(?<=_tabTitle_=)(\S*)");

            if (!reg.test(src)) return url;

            var t = reg.exec($(window.frameElement).attr("src"))[1];

            title = mDES.decrypt(t);
        }

        var tabTitle = "_tabTitle_=" + mDES.encrypt(title);

        return url.indexOf('?') < 0 ? (url + "?" + tabTitle) : (url + "&" + tabTitle);
    },
    createDialogMessage: function (msg, options, title, iconCls, showCancel, langModule, langKey) {
        if (langModule != undefined && langKey != undefined) {
            msg = HtmlLang.Write(langModule, langKey, msg);
        }
        $("#divMessageContainer").remove();
        if (options == undefined) {
            options = {};
        }
        if (options.width == undefined) {
            options.width = 350;
        }
        if (options.height == undefined) {
            options.height = 190;
        }
        if (options.title == undefined) {
            options.title = title;
        }
        var contentHeight = options.height - 150
        var html = "<div id='divMessageContainer' class='m-dialog'>";
        html += "<div class='content " + iconCls + "' style='height:" + contentHeight + "px'>" + msg + "</div>";
        html += "<div class='action'>";
        if (showCancel) {
            html += "<a href='javascript:void(0)' class='m-btn m-btn-gray'>Cancel</a>";
        }
        html += "<a href='javascript:void(0)' class='m-btn m-btn-green'>OK</a></div>";
        html += "</div>";
        $(html).appendTo('body');
        options.modal = true;
        $('#divMessageContainer').dialog(options);
    },
    displaySuccess: function (selector, htmlContent) {
        Megi.createDisplayMessage(selector, htmlContent, "green");
        window.scrollTo(0, 0);
    },
    createDisplayMessage: function (selector, htmlContent, color) {
        $('.m-msg-show').remove();
        var html = '<div class="m-msg-show m-msg-show-' + color + '">';
        html += '<div class="close"><a href="javascript:void(0)">&nbsp;</a></div>';
        html += '<div class="content">' + htmlContent + '</div>';
        html += '</div>';
        $(selector).append(html);
        $(selector).find(".m-msg-show>.close>a").click(function () {
            $('.m-msg-show').remove();
        });
    },
    encode: function (uri) {
        return uri;
    },
    confirmDelete: function (callback) {
        var result = confirm("Are you sure to delete?");
        if (result) {
            callback(result);
        }
    },
    popup: function (selector, options) {
        $(selector).popup(options);
    },
    trim: function (value) {
        return $.trim(value);
    },
    ComboBox: {
        setDefaultValue: function (selector) {
            var dataOptions = $(selector).attr("data-options");
            if (dataOptions == undefined || dataOptions.length == 0) {
                return;
            }
            dataOptions = "{" + dataOptions + "}";
            var obj = eval("(" + dataOptions + ")");
            if (obj.defaultValue == undefined) {
                return;
            }
            $(selector).combobox('setValue', obj.defaultValue);
        }
    },
    dateCompare: function (setDateStr, compareObj, compareType) {
        var setDate = new Date((setDateStr + "01").replace(/^(\d{4})(\d{2})(\d{2})$/, "$1/$2/$3"));
        var compareDate = new Date((compareObj.combobox('getValue') + "01").replace(/^(\d{4})(\d{2})(\d{2})$/, "$1/$2/$3"));
        switch (compareType) {
            case 1:
                if (setDate > compareDate) {
                    compareObj.combobox('setValue', setDateStr);
                }
                break;
            case 2:
                if (setDate < compareDate) {
                    compareObj.combobox('setValue', setDateStr);
                }
                break;
        }
    },
    autoChartSize: function () {
        var winW = $(window).get(0).innerWidth;
        var winH = $(window).get(0).innerHeight;
        $(".m-chart").each(function () {
            var winW = $(this).width();
            var chartContainerW = $(this).find("div").width();
            var chartContainerH = $(this).find("div").height();

            var newW = (winW / chartContainerW) * chartContainerW;
            var newH = (winW / chartContainerW) * chartContainerH;
            $(this).find("div").css({ "width": newW + "px", "height": newH + "px" });
            $(this).find("div").find("canvas").css({ "width": newW + "px", "height": newH + "px" });
        });
    },
    Chart: {
        getTip: function (title, lable, value) {
            return "<div class='m-chart-tip'><p class='tip-title'>" + title + "</p><div><p class='tip-lable'>" + lable + "</p><p class='tip-value'>" + value + "</p><div class='clear'></div></div></div>";
        }
    },
    getDataGridColumnList: function (columnConfigs, arrColumnIndex) {
        var cols = new Array();
        for (var i = 0; i < arrColumnIndex.length; i++) {
            cols.push(columnConfigs[arrColumnIndex[i]]);
        }
        var result = new Array();
        result.push(cols);
        return result;
    },
    grid: function (selector, options, params) {
        var optType = typeof (options);
        if (optType == "string") {
            if (options == "autoresize") {
                $(selector).datagrid('resize', { width: getGridContainerWidth() });
                return;
            } else if (options == "deleteSelected" || options == "optSelected" || options == "dbSelected") {
                var arr = "";
                $(selector).parent().find(".row-key-checkbox").each(function () {
                    if ($(this).attr("checked") == "checked") {
                        arr = arr + $(this).val() + ",";
                    }
                });
                if (arr.length == 0) {
                    //Megi.warning("Please select one or more items!", { title: "No items selected" });
                    //$.mDialog.alert("Please select one or more items!", { title: "No items selected" });
                    $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
                    return;
                }
                arr = arr.substring(0, arr.length - 1);
                if (params.url == undefined) {
                    params.callback(arr);
                    return;
                }
                var confirmMsg = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete?");
                if (arr && arr.split(',').length > 1) {
                    var tishi_1 = HtmlLang.Write(LangModule.Common, "SureToDeleteMore", "Are you sure to delete the {0} item ?")
                    confirmMsg = tishi_1.replace("{0}", arr.split(',').length);
                }

                if (params.msg != undefined && params.msg != null) {
                    confirmMsg = params.msg;
                }
                if (params.msg == '') {
                    var objParam = {};
                    if (params.param != undefined) {
                        objParam = $.extend(objParam, params.param);
                    }
                    objParam.KeyIDs = arr;
                    mAjax.submit(params.url, objParam, function (msg) {
                        if (params.callback != undefined) {
                            params.callback(msg);
                        }
                    });
                }
                else {
                    if (options == "dbSelected") {
                        var objParam = {};
                        if (params.param != undefined) {
                            objParam = $.extend(objParam, params.param);
                        }
                        objParam.KeyIDs = arr;
                        mAjax.submit(params.url, objParam, function (msg) {
                            if (params.callback != undefined) {
                                params.callback(msg);
                            }
                        });
                    }
                    else {
                        $.mDialog.confirm(confirmMsg, {
                            callback: function () {
                                var objParam = {};
                                if (params.param != undefined) {
                                    objParam = $.extend(objParam, params.param);
                                }
                                objParam.KeyIDs = arr;
                                mAjax.submit(params.url, objParam, function (msg) {
                                    if (params.callback != undefined) {
                                        params.callback(msg);
                                    }
                                });
                            }
                        });
                    }

                }
                return;
            }
            //var loadSuccess = options.onLoadSuccess;
            //options.onLoadSuccess = function (data) {
            //    if(loadSuccess!=undefined){
            //        loadSuccess(data);
            //    }
            //    $(selector).datagrid('resize', { width: getGridContainerWidth() });
            //}
            return $(selector).datagrid(options, params);
        }

        function getGridContainerWidth() {
            var containerWidth = $(selector).closest(".datagrid").parent().width();

            if (containerWidth < 200) {
                containerWidth = 200;
            }
            return containerWidth;
        }
        if (options.auto == undefined) {
            options.auto = true;
        }
        var isRowCheck = false;
        var lastRowIndex = 0;//上一次操作行的索引
        var currentRowIndex = 0;//当前操作行的索引
        var clickCellEvent = options.onClickCell;
        options.onClickCell = function (rowIndex, field, value) {
            lastRowIndex = currentRowIndex;
            currentRowIndex = rowIndex;
            var chk = $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").find(".row-key-checkbox").attr("checked");
            if (field == options.chkField) {
                if (chk == "checked") {
                    isRowCheck = true;
                    $(selector).datagrid('unselectRow', rowIndex);
                } else {
                    isRowCheck = false;
                    $(selector).datagrid('selectRow', rowIndex);
                }
            } else {
                if (chk == "checked") {
                    isRowCheck = true;
                    $(selector).datagrid('selectRow', rowIndex);
                } else {
                    isRowCheck = false;
                    $(selector).datagrid('unselectRow', rowIndex);
                }
            }
            if (clickCellEvent != undefined) {
                clickCellEvent(rowIndex, field, value);
            }
        }

        function selectOrUnselectRow(isChk, rowIndex) {
            if (isChk) {
                $(selector).datagrid('selectRow', rowIndex);
            } else {
                $(selector).datagrid('unselectRow', rowIndex);
            }
            $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").find(".row-key-checkbox").attr("checked", isChk);
        }

        var clickRowEvent = options.onClickRow;
        options.onClickRow = function (rowIndex, rowData) {
            selectOrUnselectRow(isRowCheck, rowIndex);
            if (clickRowEvent != undefined) {
                clickRowEvent(rowIndex, rowData);
            }
        }
        var clickDblRowEvent = options.onDblClickRow;
        options.onDblClickRow = function (rowIndex, rowData) {
            selectOrUnselectRow(isRowCheck, rowIndex);
            if (clickDblRowEvent != undefined) {
                clickDblRowEvent(rowIndex, rowData);
            }
        }

        var afterEditEvent = options.onAfterEdit;
        options.onAfterEdit = function (rowIndex, rowData, changes) {
            if (currentRowIndex == rowIndex) {
                selectOrUnselectRow(isRowCheck, rowIndex);
            } else {
                var chk = $(selector).parent().find("tr[datagrid-row-index=" + lastRowIndex + "]").attr("isSelected");
                var isChk = chk == "true" ? true : false;
                selectOrUnselectRow(isChk, lastRowIndex);
                var curChked = $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").attr("isSelected") == "true" ? true : false;
                $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").find(".row-key-checkbox").attr("checked", curChked);
            }

            if (afterEditEvent != undefined) {
                afterEditEvent(rowIndex, rowData, changes);
            }
        }

        options.onSelect = function (rowIndex, rowData) {
            $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").find(".row-key-checkbox").attr("checked", true);
            $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").attr("isSelected", true);
        }
        options.onUnselect = function (rowIndex, rowData) {
            $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").find(".row-key-checkbox").attr("checked", false);
            $(selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]").attr("isSelected", false);
        }
        options.onCheckAll = function () {
            var chkRowKey = options.isSelectAll ? ".row-key-checkbox" : ".row-key-checkbox:enabled";
            $(selector).parent().find(chkRowKey).attr("checked", true);
            $(selector).parent().find(".datagrid-row").attr("isSelected", true);
        }
        options.onUncheckAll = function () {
            var chkRowKey = options.isSelectAll ? ".row-key-checkbox" : ".row-key-checkbox:enabled";
            $(selector).parent().find(chkRowKey).attr("checked", false);
            $(selector).parent().find(".datagrid-row").attr("isSelected", false);
        }
        $(selector).datagrid(options, params);
        if (options.auto) {
            options.width = getGridContainerWidth();
            options.fitcolumns = true;
            $(window).resize(function () {
                $(selector).datagrid('resize', {
                    width: getGridContainerWidth()
                });
            });
        }
    },
    //datagrid操作列事件绑定 
    gridOperatorEventBind: function (selector, dom, func, paramFields) {
        var id = dom.attr("key");
        var row = $(selector).datagrid("getRowById", { idFieldName: "MItemID", rowId: id });
        var args = new Array();

        for (var i = 0 ; i < paramFields.length; i++) {
            var field = paramFields[i];
            var value = row[field];
            value = value == undefined ? "" : value;

            args.push(value);
        }
        if (func && $.isFunction(func)) {
            $(dom).bind("click", function () {
                func.apply(this, args);
            });
        }
    },
    gridEventBind: function (selector, dom, func, args, valueField) {
        var id = dom.attr("key");
        var row = $(selector).datagrid("getRowById", { idFieldName: "MItemID", rowId: id });

        if (valueField) {
            var value = row[valueField] ? row[valueField] : "";
            if ($(dom).is("input")) {
                $(dom).val(value);

            } else {
                $(dom).text(value);
            }
        }
        if (func && $.isFunction(func)) {
            $(dom).bind("click", function () {
                func.apply(this, args);
            });
        }
    },
    mergeGrid: function mergeGridColCells(grid, rowFildName) {
        var rows = grid.datagrid('getRows');
        //alert(rows.length);
        //alert(rows[1][rowFildName]);
        var startIndex = 0;
        var endIndex = 0;
        if (rows.length < 1) {
            return;
        }
        $.each(rows, function (i, row) {
            if (row[rowFildName] == rows[startIndex][rowFildName]) {
                endIndex = i;
            }
            else {
                grid.datagrid('mergeCells', {
                    index: startIndex,
                    field: rowFildName,
                    rowspan: endIndex - startIndex + 1
                });
                startIndex = i;
                endIndex = i;
            }

        });
        grid.datagrid('mergeCells', {
            index: startIndex,
            field: rowFildName,
            rowspan: endIndex - startIndex + 1
        });
    },
    changeLang: function (record, url) {
        if (url == undefined) {
            url = "/Framework/ChangeLang";
        }
        $("#loginBox").mask(HtmlLang.Write(LangModule.Common, "waitChanglang", "switching..."));
        $.ajax({
            type: "POST",
            url: url,
            data: { langId: record },
            success: function (msg) {
                if (msg == true) {
                    //
                    window.mWindow.reload();
                }
                $("#loginBox").unmask();
            }
        });

    },
    getUrlParam: function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (results == null || results[1] == undefined) {
            return null;
        }
        else {
            return results[1];
        }
    },
    isIE: function () {
        var userAgent = navigator.userAgent.toLowerCase();
        var ieRegex = /(msie\s|trident.*rv:)([\w.]+)/;
        return ieRegex.exec(userAgent) != null;
    },
    isIE9Previous: function () {
        return $.browser.msie != undefined && $.browser.msie && parseInt($.browser.version) <= 9;
    },
    isSafari: function () {
        return /^((?!chrome).)*safari/i.test(navigator.userAgent);
    },
    getCombineTitle: function (titles) {
        var joinChar = '';
        var curLang = $("#hidCurrentLangID", top.window.document).val();
        if (curLang == "0x0009") {
            joinChar += ' ';
        }
        return titles.join(joinChar);
    },
    attachPropChangeEvent: function (obj, callBack) {
        //解决IE9下监听控件值改变问题(#23103)
        if ($.browser.msie && !Megi.isIE9Previous()) {
            obj.onpropertychange = callBack;
        } else {
            obj.addEventListener("input", callBack, false);
        }
    },
    regClickToSelectAllEvt: function () {
        return false;
        if (!Megi.isEdge()) {
            //文本框单击全选
            $("input:text:not(\".click-no-select\"), input:password, textarea:not(\".click-no-select\")").off('click.selAll').on('click.selAll', function (e) {
                //单击时，如果鼠标没选中文本时，自动全选
                if (!Megi.isTextSelected(this)) {
                    $(this).selectAll(e);
                }
            });
        }

    },
    //判断鼠标是否有选中文本
    isTextSelected: function (input) {
        if (typeof input.selectionStart == "number") {
            return input.selectionEnd - input.selectionStart > 0;
        } else if (typeof document.selection != "undefined") {
            return document.selection.createRange().text.length > 0;
        }
    },
    //IE浏览器是否小于IE9
    isLowVersionIE9: function () {
        return $.browser.msie != undefined && $.browser.msie && parseInt($.browser.version) < 9;
    },
    isEdge: function () {
        return navigator.userAgent.indexOf("Edge") > -1;
    },
    openNewTab: function (url, data, isNewTab) {
        var form = document.createElement("form");
        form.action = url;
        form.method = "post";
        if (isNewTab) {
            form.target = "_blank";
        }
        if (data) {
            for (var key in data) {
                var input = document.createElement("input");
                input.type = 'hidden';
                input.name = key;
                input.value = typeof data[key] === "object" ? JSON.stringify(data[key]) : data[key];
                form.appendChild(input);
            }
        }
        form.style.display = 'none';
        document.body.appendChild(form);
        form.submit();
    },
    //通过post方式请求url -- by linfq
    postUrl: function (url, param, showMask) {
        var form = $("#frmSubmit");
        if (form.length == 0) {
            form = $("<form id='frmSubmit' method='post'></form>");
            form.appendTo($("body"));
        }
        form.attr('action', url);

        if (param) {
            var arrParam = param.split('&');
            for (var i = 0; i < arrParam.length; i++) {
                var arrKeyVal = arrParam[i].split('=');
                var input = form.find("input:hidden[name=" + arrKeyVal[0] + "]");
                if (input.length == 0) {
                    input = $("<input type='hidden' name='" + arrKeyVal[0] + "'/>");
                }
                input.attr('value', arrKeyVal[1]);
                form.append(input);
            }
        }
        //默认显示遮罩
        if (showMask || showMask == undefined) {
            $("body").mask("");
        }
        form.submit();
    },
    //打开弹框，参数以post方式传递 -- by linfq
    openDialog: function (url, title, param, w, h) {
        var postData = {};
        var arrParam = param.split('&');
        for (var i = 0; i < arrParam.length; i++) {
            var arrKeyVal = arrParam[i].split('=');
            postData[arrKeyVal[0]] = arrKeyVal[1];
        }
        
        var options = {
            mTitle: title,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mContent: "iframe:" + url,
            mPostData: postData
        };

        if(w){
            options.mWidth = w;
        }
        if(h){
            options.mHeight = h;
        }

        $.mDialog.show(options);
    },
    //解析参数
    getQueryString: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
}
