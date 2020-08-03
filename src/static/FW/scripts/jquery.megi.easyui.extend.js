$.extend($.fn.validatebox.defaults.rules, {
    gt: {
        validator: function (_444, _445) {
            var val = +_444;
            return val > +(_445[0])
        },
        message: HtmlLang.Write(LangModule.Common, "entervaluegreaterthan", "请输入一个大于{0}的值.")
    },
    lt: {
        validator: function (_444, _445) {
            var val = +_444;
            return val < +(_445[0])
        },
        message: HtmlLang.Write(LangModule.Common, "entervaluelessthan", "请输入一个小于{0}的值.")
    },
    ge: {
        validator: function (_444, _445) {
            var val = +_444;
            return val >= +(_445[0])
        },
        message: HtmlLang.Write(LangModule.Common, "entervaluegreaterequalthan", "请输入一个大于或等于{0}的值.")
    },
    le: {
        validator: function (_444, _445) {
            var val = +_444;
            return val <= +(_445[0])
        },
        message: HtmlLang.Write(LangModule.Common, "entervaluelessequalthan", "请输入一个小于或者等于{0}的值.")
    },
    idcard: {// 验证身份证
        validator: function (value) {
            return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'IDIncorrent', 'Id card number format is not correct.')
    },
    minLength: {
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: HtmlLang.Write(LangModule.Common, 'Needtoenteratleastcharacters', 'Need to enter at least {0} characters.')
    },
    length: {
        validator: function (value, param) {
            var len = $.trim(value).length;
            return len >= param[0] && len <= param[1];
        },
        message: HtmlLang.Write(LangModule.Common, 'InputContentLength', 'The input length must be between {0} and {1}')
    },
    phone: {// 验证电话号码
        validator: function (value) {
            var isPhone = /([0-9]{3,4}-)?[0-9]{7,8}/;
            var isMob = /^((\+?86)|(\(\+86\)))?(13[012356789][0-9]{8}|15[012356789][0-9]{8}|18[02356789][0-9]{8}|147[0-9]{8}|170[0-9]{8}|1349[0-9]{7})$/;
            //return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
            return isMob.test(value) || isPhone.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'PhoneNumberIncorrect', 'Phone Number Incorrect.eq:010-88888888')
    },
    mobile: {// 验证手机号码
        validator: function (value) {
            return /^(13|15|18)\d{9}$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'MobileNumberIncorrect', 'Mobile phone number format is not correct.')
    },
    intOrFloat: {// 验证整数或小数
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'IntOrFloat', 'Please enter the Numbers, and ensure the correct format.')
    },
    currency: {// 验证货币
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'CurrencyIncorrent', 'Currency format is not correct.')
    },
    qq: {// 验证QQ,从10000开始
        validator: function (value) {
            return /^[1-9]\d{4,9}$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'QQIncorrent', 'QQ Number format is not correct.')
    },
    integer: {// 验证整数 可正负数
        validator: function (value) {
            //return /^[+]?[1-9]+\d*$/i.test(value);

            return /^([+]?[0-9])|([-]?[0-9])+\d*$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'InputInt', 'Please enter an integer.')
    },
    age: {// 验证年龄
        validator: function (value) {
            return /^(?:[1-9][0-9]?|1[01][0-9]|120)$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'AgeIncorrent', 'Age must be an integer between 0 and 120.')
    },

    chinese: {// 验证中文
        validator: function (value) {
            return /^[\Α-\￥]+$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'InputChinese', 'Please input Chinese.')
    },
    english: {// 验证英语
        validator: function (value) {
            return /^[A-Za-z]+$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'InputEnglish', 'Please input English.')
    },
    unnormal: {// 验证是否包含空格和非法字符
        validator: function (value) {
            return /.+/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'unnormal', "The input value can't be empty and contains other illegal characters.")
    },
    numchseng: {//只能输入数字中文和英文
        validator: function (value) {
            return /^[\u4E00-\u9FA5\uF900-\uFA2D\da-zA-Z]+$/.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'unnormal', "The input value can't be empty and contains other illegal characters.")
    },
    numCheck: {//只能输入数字
        validator: function (value) {
            return /^[0-9]*$/.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'unnormal', "The input value can't be empty and contains other illegal characters.")
    },
    username: {// 验证用户名
        validator: function (value) {
            return /^[a-zA-Z][a-zA-Z0-9_]{5,15}$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'UserNameIncorrent', "user name illegal (letter, allowing 6-16 bytes, allow alphanumeric underscore).")
    },
    faxno: {// 验证传真
        validator: function (value) {
            //            return /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/i.test(value);
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'FaxnoIncorrent', "Fax number is not correct.")
    },
    zip: {// 验证邮政编码
        validator: function (value) {
            return /^[1-9]\d{5}$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'ZipIncorrent', "Zip number is not correct.")
    },
    ip: {// 验证IP地址
        validator: function (value) {
            return /d+.d+.d+.d+/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'IPIncorrent', "IP is not correct.")
    },
    name: {// 验证姓名，可以是中文或英文
        validator: function (value) {
            return /^[\Α-\￥]+$/i.test(value) | /^\w+[\w\s]+\w+$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'InputName', "Please input name.")
    },
    date: {// 验证姓名，可以是中文或英文
        validator: function (value) {
            //格式yyyy-MM-dd或yyyy-M-d
            return /^(?:(?!0000)[0-9]{4}([-]?)(?:(?:0?[1-9]|1[0-2])\1(?:0?[1-9]|1[0-9]|2[0-8])|(?:0?[13-9]|1[0-2])\1(?:29|30)|(?:0?[13578]|1[02])\1(?:31))|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)([-]?)0?2\2(?:29))$/i.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'DateFormatIncorrent', "Date Format is not corrent.")
    },
    msn: {
        validator: function (value) {
            return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: HtmlLang.Write(LangModule.Common, 'MSNIncorrent', "MSN is not corrent.eq:abc@hotnail(msn/live).com")
    },
    same: {
        validator: function (value, param) {
            if ($("#" + param[0]).val() != "" && value != "") {
                return $("#" + param[0]).val() == value;
            } else {
                return true;
            }
        },
        message: HtmlLang.Write(LangModule.Common, 'TowPasswordNotSame', "Tow  password not match")
    },
    maxDate: {
        validator: function (value, param) {
            var d1 = $.fn.datebox.defaults.parser(param[0]);
            var d2 = $.fn.datebox.defaults.parser(value);
            return d2 < d1;
        },
        message: HtmlLang.Write(LangModule.Common, 'datemustlessthan', 'The date must be less than {0}.')
    },
    minDate: {
        validator: function (value, param) {
            var d1 = $.fn.datebox.defaults.parser(param[0]);
            var d2 = $.fn.datebox.defaults.parser(value);
            return d2 >= d1;
        },
        message: HtmlLang.Write(LangModule.Common, 'datemustlargerthan', 'The date must be larger than or equals to {0}.')
    },
    day: {
        validator: function (value) {
            var temp = parseInt(value);
            return temp >= 1 && temp <= 31;
        },
        message: HtmlLang.Write(LangModule.Common, 'day', 'day must between 1 and 31')
    },
    isBlank: {
        validator: function (value, param) { return $.trim(value) != '' },
        message: HtmlLang.Write(LangModule.Common, 'unnormal', "The input value can't be empty and contains other illegal characters.")
    },
    notEqual: {
        validator: function (value, param) {
            return value != param[0];
        },
        message: HtmlLang.Write(LangModule.Common, 'InputNotEqualValue', "The input value can't equal with {0} !")
    }
});

//datagrid的扩展
$.extend($.fn.datagrid.methods, {
    /**
     * 级联选择
     * @param {Object} target
     * @param {Object} param 
     *		param包括两个参数:
     *			id:勾选的节点ID
     *			deepCascade:是否深度级联
     * @return {TypeName} 
     */
    cascadeCheck: function (target, param) {
        var opts = $.data(target[0], "treegrid").options;
        if (opts.singleSelect)
            return;
        var idField = opts.idField;//这里的idField其实就是API里方法的id参数
        var status = false;//用来标记当前节点的状态，true:勾选，false:未勾选
        var selectNodes = $(target).treegrid('getSelections');//获取当前选中项
        for (var i = 0; i < selectNodes.length; i++) {
            if (selectNodes[i][idField] == param.id)
                status = true;
        }
        if (param.deepCascade || status === false)
            selectParent(target[0], param.id, idField, status);


        selectChildren(target[0], param.id, idField, param.deepCascade, status);

        //级联选择父节点
        //target
        //id 节点ID
        // status 节点状态，true:勾选，false:未勾选
        function selectParent(target, id, idField, status) {

            var parent = $(target).treegrid('getParent', id);
            if (parent) {
                var parentId = parent[idField];
                //还需要判断所有该父节点下所有的子节点是否选择，如果有没选择的，父节点应该也是不选中
                if (status) {
                    var childrenIsAllSelected = true;
                    var childrenNodes = $(target).treegrid("getChildren", parentId);
                    var selectNodes = $(target).treegrid('getSelections');//获取当前选中项
                    for (var i = 0; i < childrenNodes.length; i++) {
                        var isSelect = false;
                        for (var j = 0; j < selectNodes.length; j++) {
                            if (childrenNodes[i][idField] == selectNodes[j][idField]) {
                                isSelect = true;
                                break;
                            }
                        }
                        //如果有一个子科目没有选中，父级就不能选择
                        if (!isSelect) {
                            childrenIsAllSelected = false;
                            break;
                        }
                    }
                    if (childrenIsAllSelected) {
                        $(target).treegrid('select', parentId);
                    } else {
                        $(target).treegrid('unselect', parentId);
                    }

                    selectParent(target, parentId, idField, childrenIsAllSelected);
                }
                else {
                    $(target).treegrid('unselect', parentId);
                    selectParent(target, parentId, idField, false);
                }

            }
        }

        // 级联选择子节点
        // target
        // id 节点ID
        // deepCascade 是否深度级联
        // status 节点状态，true:勾选，false:未勾选
        // return {TypeName} 
        function selectChildren(target, id, idField, deepCascade, status) {

            //深度级联时先展开节点
            if (!status && deepCascade)
                $(target).treegrid('expand', id);
            //根据ID获取下层孩子节点
            var children = $(target).treegrid('getChildren', id);
            if (!children || children.length == 0) {
                return;
            }
            for (var i = 0; i < children.length; i++) {
                var childId = children[i][idField];
                if (status)
                    $(target).treegrid('select', childId);
                else
                    $(target).treegrid('unselect', childId);
            }
        }
    }
});


/*combotree search 扩展*/
$.fn.combotree.defaults.editable = true;
$.extend($.fn.combotree.defaults.keyHandler, {
    up: function () {

    },
    down: function () {

    },
    enter: function () {

    },
    click: function () {

    },
    query: function (q) {
        //先清空掉所有选中行
        var opts = $(this).combotree('options');
        var t = $(this).combotree('tree');
        if (opts.multiple) {
            //多选
            var selectedChecked = t.tree("getChecked");

            if (selectedChecked && selectedChecked.length > 0) {
                for (var i = 0 ; i < selectedChecked.length; i++) {
                    var node = selectedChecked[i];

                    t.tree("uncheck", node.target);
                }
            }
        } else {
            $(this).combotree("setValue", "");
        }

        //var selectedNode = $(this).combotree("");

        var nodes = t.tree('getChildren');
        var showNodes = new Array();

        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];

            if (node.text.indexOf(q) >= 0) {
                showNodes.push(node);

                //所有子级
                getAllParent(t, node, showNodes);
                //所有父级
                getAllChildren(t, node, showNodes);
            }
        }

        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];

            var show = false;
            for (var j = 0; j < showNodes.length; j++) {
                if (node.id == showNodes[j].id) {
                    show = true;
                    break;
                }
            }

            if (show) {
                $(node.target).show();
            } else {
                $(node.target).hide();
            }
        }

        if (!opts.hasSetEvents) {
            opts.hasSetEvents = true;
            var onShowPanel = opts.onShowPanel;
            opts.onShowPanel = function () {
                var nodes = t.tree('getChildren');
                for (var i = 0; i < nodes.length; i++) {
                    $(nodes[i].target).show();
                }
                onShowPanel.call(this);
            }
            $(this).combo('options').onShowPanel = opts.onShowPanel;
        }
    }
});

function getAllChildren(treeControl, parentNode, showNodes) {
    var nodes = treeControl.tree("getChildren", parentNode.target);
    if (nodes && nodes.length > 0) {
        for (var i = 0 ; i < nodes.length; i++) {
            var node = nodes[i];
            var isExist = false;
            for (var j = 0 ; j < showNodes.length; j++) {
                showNode = showNodes[j];

                if (node.id == showNode.id) {
                    isExist = true;
                    break;
                }
            }
            if (!isExist) {
                showNodes.push(node);
            }

            getAllChildren(treeControl, node, showNodes);
        }
    } else {
        return;
    }
}

function getAllParent(treeControl, childNode, showNodes) {
    var node = treeControl.tree("getParent", childNode.target);
    if (node) {

        var isExist = false;
        for (var j = 0 ; j < showNodes.length; j++) {
            showNode = showNodes[j];

            if (node.id == showNode.id) {
                isExist = true;
                break;
            }
        }
        if (!isExist) {
            showNodes.push(node);
        }
        getAllParent(treeControl, node, showNodes);
    } else {
        return;
    }
}

//combobox扩展
$.extend($.fn.combobox.methods, {

});


$.extend($.fn.datagrid.methods, {
    //勾选全选框或者取消勾选的时候，列表里面的勾选也要勾选或者取消勾选
    //check，表示选择的状态
    doCheckAll: function (jq, checked) {

        var panel = $(jq).datagrid("getPanel");

        //获取所有的checkbox
        var allCheckBox = $("tr[datagrid-row-index]:visible input[type='checkbox']:visible:not(disabled)", panel);

        if (!allCheckBox || allCheckBox.length == 0) return;

        allCheckBox.attr("checked", checked);

    },
    //勾选单个勾选框的时候，校验是否需要最顶部的也勾选
    //check，表示选择的状态
    //filterFunc，过滤条件，控制那些不需要勾选或者取消勾选，接收的参数是row和check，返回的是true or false
    doCheck: function (jq, checked) {

        var panel = $(jq).datagrid("getPanel");

        //获取所有的checkbox checked=true，则取没有勾选的是否存在
        var checkboxes = $("tr[datagrid-row-index]:visible input[type='checkbox']:visible:not(disabled)" + (!checked ? ":checked" : ":not(:checked)"), panel);

        var allCheckBox = $("tr.datagrid-header-row:visible input[type='checkbox']:visible:not(disabled)", panel);

        if (!allCheckBox || allCheckBox.length == 0) return;

        allCheckBox.attr("checked", checkboxes.length == 0 ? checked : !checked);
    },
    resizeColumn: function (jq, param) {

        var opts = $(jq).datagrid("options");
        //先不处理有固定列的
        if (opts.frozenColumns.length > 0) return;


        //取出所有的列
        var columns = opts.columns[0];

        var panel = $(jq).datagrid("getPanel");
        var headerTables = panel.find(".datagrid-header:visible table");
        if (headerTables.length < 1) {
            return;
        }
        var headerTable = headerTables.eq(0);//

        var stylePanel = $("style[easyui]", panel);

        if (stylePanel.length == 0) {
            panel.closest("div.datagrid-view").append("<style type=\"text/css\" easyui=\"true\"></style>");
            stylePanel = $("style[easyui]", panel);
        }

        stylePanel.empty();

        for (var i = 0; i < headerTables.length && headerTables.length > 1 ; i++) {

            if (headerTables.eq(i).find("td[field]:visible").length > 0) {
                headerTable = headerTables.eq(i);
                break;
            }
        }

        var firstTd = headerTable.find("tr:eq(0) td:visible:eq(0)");
        var fieldName = firstTd.attr("field");
        var prefix = new RegExp("((datagrid-cell-c\\S+-)" + fieldName + ")").exec(firstTd.find(" > div").attr("class"))[2];

        var total = 0;

        var totalWidth = headerTable.closest("div.datagrid-header:visible").width() - (opts.scrollY ? 20 : 0);

        //先计算出总的宽度
        for (var i = 0; i < columns.length; i++) {

            if (headerTable.find("tr:eq(0) td[field='" + columns[i].field + "']").is(":hidden")) continue;

            total += columns[i].fixwidth;
        }

        var html = "";

        var x = [];

        for (var i = 0; i < columns.length; i++) {

            if (headerTable.find("tr:eq(0) td [field='" + columns[i].field + "']").is(":hidden")) continue;

            //有padding
            var w = +columns[i].fixwidth / total * totalWidth - 10;

            html += "." + prefix + columns[i].field + "{ width:" + w + "px }" + "\n";

            columns[i].width = w + 8;

            x.push(["." + prefix + columns[i].field, w + "px"]);
        }

        html += "";

        $("style[easyui]", panel).html(html);

        $.data(jq[0], "datagrid").ss.clean();
        $.data(jq[0], "datagrid").ss.add(x);
    },
    addEditor: function (jq, param) {
        if (param instanceof Array) {
            $.each(param, function (index, item) {
                var e = $(jq).datagrid('getColumnOption', item.field);
                e.editor = item.editor;
            });
        } else {
            var e = $(jq).datagrid('getColumnOption', param.field);
            e.editor = param.editor;
        }
    },
    removeEditor: function (jq, param) {
        if (param instanceof Array) {
            $.each(param, function (index, item) {
                var e = $(jq).datagrid('getColumnOption', item);
                e.editor = {};
            });
        } else {
            var e = $(jq).datagrid('getColumnOption', param);
            e.editor = {};
        }
    },
    getRowById: function (jq, param) {
        var data = $(jq).datagrid("getData");
        if (!data || data.rows.length == 0) {
            return {};
        }
        var rows = data.rows;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];

            if (row[param.idFieldName] == param.rowId) {
                return row;
            }
        }

        return {};
    },
    getRowByIndex: function (jq, rowIndex) {
        var data = $(jq).datagrid("getData");
        if (!data || data.rows.length == 0 || rowIndex > data.length) {
            return {};
        }
        return data.rows[rowIndex];
    },
    autoMergeCells: function (jq, fields) {
        return jq.each(function () {
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var rows = target.datagrid("getRows");
            var i = 0,
            j = 0,
            temp = {};

            for (i; i < rows.length; i++) {
                var row = rows[i];
                j = 0;
                for (j; j < fields.length; j++) {
                    var field = fields[j];
                    var tf = temp[field];
                    if (!tf) {
                        tf = temp[field] = {};
                        tf[row[field]] = [i];
                    } else {
                        var tfv = tf[row[field]];
                        if (tfv) {
                            tfv.push(i);
                        } else {
                            tfv = tf[row[field]] = [i];
                        }
                    }
                }
            }
            $.each(temp, function (field, colunm) {
                $.each(colunm, function () {
                    var group = this;

                    if (group.length > 1) {
                        var before,
                        after,
                        megerIndex = group[0];
                        for (var i = 0; i < group.length; i++) {
                            before = group[i];
                            after = group[i + 1];
                            if (after && (after - before) == 1) {
                                continue;
                            }
                            var rowspan = before - megerIndex + 1;
                            if (rowspan > 1) {
                                target.datagrid('mergeCells', {
                                    index: megerIndex,
                                    field: field,
                                    rowspan: rowspan
                                });
                            }
                            if (after && (after - before) != 1) {
                                megerIndex = after;
                            }
                        }
                    }
                });
            });
        });
    },
    autoMergeCellAndCells: function (jq, fields) {
        return jq.each(function () {
            var target = $(this);
            if (!fields) {
                fields = target.datagrid("getColumnFields");
            }
            var cfield = fields[0];
            if (!cfield) {
                return;
            }
            var rows = target.datagrid("getRows");
            var i = 0,
            j = 0,
            temp = {};
            for (i; i < rows.length; i++) {
                var row = rows[i];
                j = 0;
                var tf = temp[cfield];
                if (!tf) {
                    tf = temp[cfield] = {};
                    tf[row[cfield]] = [i];

                } else {
                    var tfv = tf[row[cfield]];
                    if (tfv) {
                        tfv.push(i);
                    } else {
                        tfv = tf[row[cfield]] = [i];

                    }
                }
            }

            $.each(temp, function (field, colunm) {
                $.each(colunm, function () {
                    var group = this;

                    if (group.length > 1) {
                        var before,
                        after,
                        megerIndex = group[0];
                        for (var i = 0; i < group.length; i++) {
                            before = group[i];
                            after = group[i + 1];
                            if (after && (after - before) == 1) {
                                continue;
                            }
                            var rowspan = before - megerIndex + 1;
                            if (rowspan > 1) {
                                for (var j = 0; j < fields.length; j++) {
                                    target.datagrid('mergeCells', {
                                        index: megerIndex,
                                        field: fields[j],
                                        rowspan: rowspan
                                    });
                                }
                            }
                            if (after && (after - before) != 1) {
                                megerIndex = after;
                            }
                        }
                    }
                });
            });
        });
    }
});

$.extend($.fn.tabs.methods, {
    getTabById: function (jq, id) {
        var tabs = $.data(jq[0], 'tabs').tabs;
        for (var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];
            if (tab.panel('options').id == id) {
                return tab;
            }
        }
        return null;
    },
    selectById: function (jq, id) {
        return jq.each(function () {
            var state = $.data(this, 'tabs');
            var opts = state.options;
            var tabs = state.tabs;
            var selectHis = state.selectHis;
            if (tabs.length == 0) { return; }
            var panel = $(this).tabs('getTabById', id); // get the panel to be activated 
            if (!panel) { return }
            var selected = $(this).tabs('getSelected');
            if (selected) {
                if (panel[0] == selected[0]) { return }
                $(this).tabs('unselect', $(this).tabs('getTabIndex', selected));
                if (!selected.panel('options').closed) { return }
            }
            panel.panel('open');
            var title = panel.panel('options').title;        // the panel title 
            selectHis.push(title);        // push select history 
            var tab = panel.panel('options').tab;        // get the tab object 
            tab.addClass('tabs-selected');
            // scroll the tab to center position if required. 
            var wrap = $(this).find('>div.tabs-header>div.tabs-wrap');
            var left = tab.position().left;
            var right = left + tab.outerWidth();
            if (left < 0 || right > wrap.width()) {
                var deltaX = left - (wrap.width() - tab.width()) / 2;
                $(this).tabs('scrollBy', deltaX);
            } else {
                $(this).tabs('scrollBy', 0);
            }
            $(this).tabs('resize');
            opts.onSelect.call(this, title, $(this).tabs('getTabIndex', panel));
        });
    },
    existsById: function (jq, id) {
        return $(jq[0]).tabs('getTabById', id) != null;
    }
});

//检测底层是否匹配
function treeMatchTest(data, x, topMatch, options) {
    //
    var matchOne = false;
    for (var i = 0; i < data.length ; i++) {
        //本身是否匹配
        var match = false;
        var forbidden = false;
        //是否有子集匹配
        var subMatch = false;
        var node = data[i];
        match = topMatch || (!x || ($.isFunction(x) ? x(node) : (node.text.toLowerCase().indexOf(x.toLowerCase()) >= 0)));

        //如果不匹配并且没有子集的话，则直接隐藏
        if (!match) {
            //如果不匹配，在检测底层的是否匹配
            if (node.children && node.children.length > 0) {
                subMatch = treeMatchTest(node.children, x, false, options);
            }
        }


        if (options && options.showDisabled) {
            forbidden = false;
        } else {
            //禁用的不在匹配
            if (options && options.hidItemKey && options.hideItemValue) {
                forbidden = node[options.hidItemKey] == options.hideItemValue;
            } else {
                forbidden = "MIsActive" in node ? node.MIsActive == false || node.MIsActive == "0" : false;
            }
        }


        //如果符合的话，就把node显示，如果不符合就隐藏起来
        (match || subMatch) && !forbidden ? $('#' + node.domId).show() : $('#' + node.domId).hide();
        //如果本身不匹配，那么上面的函数已经去匹配过一次子集了，就不要再来一次了
        if (match) {
            //如果有子集的话，子集也要过滤,如果父级符合条件则，子集全部符合条件
            if (node.children && node.children.length > 0) {
                treeMatchTest(node.children, x, true);
            }
        }
        //
        matchOne = matchOne || match || subMatch;
    }
    return matchOne;
}

/*tree搜索方法扩展*/
$.extend($.fn.tree.methods, {
    doFilter: function (jq, param) {
        return jq.each(function () {
            //
            param = param || "";
            //
            var data = $.data(this, 'tree').data;
            var opts = $.data(this, 'tree').options;
            //
            treeMatchTest(data, param, false, opts);
        });
    },
    checkAll: function (jq) {
        var roots = $(jq).tree('getRoots');
        for (var i = 0; i < roots.length; i++) {
            var node = $(jq).tree('find', roots[i].id);//查找节点  
            $(jq).tree('check', node.target);//将得到的节点选中  
        }
    },
    unCheckAll: function (jq) {
        var roots = $(jq).tree('getRoots');
        for (var i = 0; i < roots.length; i++) {
            var node = $(jq).tree('find', roots[i].id);
            $(jq).tree('uncheck', node.target);
        }
    }
});
$.fn.combotree.defaults.editable = true;
$.extend($.fn.combotree.defaults.keyHandler, {
    query: function (q) {
        var data = $.data($.data(this, "combotree").tree[0], "tree").data;
        var options = $.data($.data(this, "combotree").tree[0], "tree").options;
        treeMatchTest(data, q, false, options);
    }
});

