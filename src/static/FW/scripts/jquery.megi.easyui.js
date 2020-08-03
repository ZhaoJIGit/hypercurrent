/*
    根据Megi项目事情，扩展Easyui的方法
    //1.目前我们系统用到的easyui需要校验的主要有 input / combobxo / datepicker,其中combobox可能是由select生成，也有可能是由input形式请求URL生成
    //2.需要将整个页面的空间都进行校验，一次性告诉用户所有存在不合法输入的地方
*/
; (function (Megi) {
    //给Megi赋值
    Megi = Megi || {};
    //textbox
    (function () {
        var mTextBox = (function () {
            var mTextBox = function (sel) {
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).textbox("getText");
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector);
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).val();
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    $(selector).val(value);
                };
                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).val() ? false : true;
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).width(width);
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    that.getInput(selector).trigger("focus.hint");
                    var result = $(selector).textbox("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).textbox("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).textbox("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).width(width);
                };
            };
            mTextBox.name = "textbox";
            mTextBox.createInstance = function (selector) {
                return new mTextBox(selector);
            };
            return mTextBox;
        })();
        window.mTextBox = $.mTextBox = mTextBox;
    })();


    //combotree
    (function () {
        var mCombotree = (function () {

            //可以为空
            var mDefault = "m-default";
            //日期格式
            var easyuiDateType = "easyui-date-type";
            var mCombotree = function (sel) {
                //如果sel有combo-f，则表示其不是原始的输入框，而是后期生成的
                sel = $(sel).hasClass("combo-text") ? $(sel).parent().prev(".combotree-f") : sel;
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("getText");
                };
                this.setText = function (text, selector) {
                    selector = selector || sel;
                    return $(selector).combotree("setText", text);
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().find(".combo-value").attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("textbox");
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("getValue");
                };
                this.getPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("panel");
                };
                this.showPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("showPanel");
                };
                this.hidePanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("hidePanel");
                };
                this.readonly = function (selector) {
                    selector = selector || sel;
                    //
                    var text = that.getText(selector);
                    //
                    if (text) {
                        //
                        var input = that.getInput(selector);
                        //
                        //
                        if (input.hasClass("tooltip-f")) {
                            //更新
                            input.tooltip("destroy");
                        }
                        //
                        input.removeAttr("etitle");
                        //
                        input.attr("etitle", text).tooltip({ content: text });
                        //
                        $(selector).combobox("readonly");
                    }
                }
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint属性
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };
                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).combotree("getValue") ? false : true;
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    //默认值可以为空
                    if ($(selector).hasClass(mDefault) && value || (!$(selector).hasClass(mDefault) && !$(selector).hasClass(easyuiDateType))) {
                        //赋值
                        $(selector).combotree("setValue", value);
                    }
                };
                this.getOptions = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("options");
                };
                this.getData = function (selector) {
                    selector = selector || sel;
                    return $(selector).combotree("getData");
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    //获取用户目前输入的值
                    var userInput = that.getText(selector);
                    //如果用户正常选择了，则getValue的值不为空
                    var selectedValue = that.getValue(selector);
                    //只针对用户输入了值，但是没有选择值得情况才去做验证
                    if (userInput && selectedValue == undefined) {
                        //获取用户可以选择的值
                        var datas = that.getData(selector);
                        //获取options
                        var options = that.getOptions(selector);
                        //值字段
                        var valueField = options.valueField;
                        //文本字段
                        var textField = options.textField;
                        //是否找到值
                        var findValue = "";
                        //遍历数据，找到列表中存在的项,用autocomplete的方法来做
                        for (var i = 0 ; userInput && (i < datas.length) ; i++) {
                            //当前数据
                            var data = datas[i];
                            //比较文本
                            if (data[textField] && data[textField].indexOf(userInput) > -1) {
                                //算是找到了吧
                                findValue = data[valueField];
                                //结束查找
                                break;
                            }
                        }
                        //把找到的值复制到combobox
                        that.setValue(findValue, selector);
                    }
                    //如果没有传value的值过来
                    var value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        mHint.blurFunc(that.getInput(selector), value);
                    }
                    var result = $(selector).combotree("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).combotree("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).combotree("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).combotree("resize", width);
                };
            };
            mCombotree.name = "combobox";
            mCombotree.className = "combotree-f";
            mCombotree.inputSelector = ".combo-text:not(.datebox-input)";
            mCombotree.createInstance = function (selector) {
                return new mCombotree(selector);
            };
            return mCombotree;
        })();
        window.mCombotree = $.mCombotree = mCombotree;
    })();
    //combobox
    (function () {
        var mCombobox = (function () {
            //可以为空
            var mDefault = "m-default";
            //日期格式
            var easyuiDateType = "easyui-date-type";

            var mCombobox = function (sel) {
                //如果sel有combo-f，则表示其不是原始的输入框，而是后期生成的
                sel = $(sel).hasClass("combo-text") ? $(sel).parent().prev(".combobox-f,.combogrid-f") : sel;
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("getText");
                };
                this.setText = function (text, selector) {
                    selector = selector || sel;
                    return $(selector).datebox("setText", text);
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().find(".combo-value").attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("textbox");
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("getValue");
                };
                this.getPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("panel");
                };
                this.showPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("showPanel");
                };
                this.hidePanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("hidePanel");
                };
                this.readonly = function (selector) {
                    selector = selector || sel;
                    //
                    var text = that.getText(selector);
                    //
                    if (text) {
                        //
                        var input = that.getInput(selector);
                        //
                        //
                        if (input.hasClass("tooltip-f")) {
                            //更新
                            input.tooltip("destroy");
                        }
                        //
                        input.removeAttr("etitle");
                        //
                        input.attr("etitle", text).tooltip({ content: text });
                        //
                        $(selector).combobox("readonly");
                    }
                }
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint属性
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };
                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).combobox("getValue") ? false : true;
                };
                this.setDateValue = function (value, selector) {
                    selector = selector || sel;
                    //把value转化成毫秒的格式
                    var minisecValue = mDate.parse(value).getTime();
                    //获取用户可以选择的值
                    var datas = that.getData(selector);
                    //获取options
                    var options = that.getOptions(selector);
                    //值字段
                    var valueField = options.valueField;
                    //遍历
                    for (var i = 0 ; i < datas.length ; i++) {
                        //当前数据
                        var data = datas[i];
                        //如果换算的毫秒数相同，则setValue
                        if (mDate.parse(data[valueField]).getTime() == minisecValue) {
                            //
                            $(selector).hasClass("combogrid-f") ? $(selector).combogrid("setValue", data[valueField]) : $(selector).combobox("setValue", data[valueField]);
                            //结束查找
                            break;
                        }
                    }
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    //默认值可以为空
                    if ($(selector).hasClass(mDefault) && value || (!$(selector).hasClass(mDefault) && !$(selector).hasClass(easyuiDateType))) {
                        //赋值
                        $(selector).hasClass("combogrid-f") ? $(selector).combogrid("setValue", value) : $(selector).combobox("setValue", value);
                    } else if ($(selector).hasClass(easyuiDateType)) {
                        //如果是日期类型的combobox,因为从后台传过来的JSON一般为/Data(XXXXXXX)/格式的，但是combobox的值可能不规范，
                        //因此要一个值一个值得匹配
                        //
                        that.setDateValue(value, selector);

                    }
                };
                this.getOptions = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("options");
                };
                this.getData = function (selector) {
                    selector = selector || sel;
                    return $(selector).combobox("getData");
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    //获取用户目前输入的值
                    var userInput = that.getText(selector);
                    //如果用户正常选择了，则getValue的值不为空
                    var selectedValue = that.getValue(selector);
                    //只针对用户输入了值，但是没有选择值得情况才去做验证
                    if (userInput && selectedValue == undefined) {
                        //获取用户可以选择的值
                        var datas = that.getData(selector);
                        //获取options
                        var options = that.getOptions(selector);
                        //值字段
                        var valueField = options.valueField;
                        //文本字段
                        var textField = options.textField;
                        //是否找到值
                        var findValue = "";
                        //遍历数据，找到列表中存在的项,用autocomplete的方法来做
                        for (var i = 0 ; userInput && (i < datas.length) ; i++) {
                            //当前数据
                            var data = datas[i];
                            //比较文本
                            if (data[textField] && data[textField].indexOf(userInput) > -1) {
                                //算是找到了吧
                                findValue = data[valueField];
                                //结束查找
                                break;
                            }
                        }
                        //把找到的值复制到combobox
                        that.setValue(findValue, selector);
                    }
                    //如果没有传value的值过来
                    var value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        mHint.blurFunc(that.getInput(selector), value);
                    }
                    var result = $(selector).combobox("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).combobox("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).combobox("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).combobox("resize", width);
                };
            };
            mCombobox.name = "combobox";
            mCombobox.className = "combobox-f";
            mCombobox.inputSelector = ".combo-text:not(.datebox-input)";
            mCombobox.createInstance = function (selector) {
                return new mCombobox(selector);
            };
            return mCombobox;
        })();
        window.mCombobox = $.mCombobox = mCombobox;
    })();
    //combo
    (function () {
        var mCombo = (function () {
            //
            var mCombo = function (sel) {
                //判断是combobox 还是combotree
                this.createInstance = function (selector) {
                    //
                    sel = selector || sel;
                    //
                    if ($(sel).parent().prev(".combotree-f").length > 0 || $(sel).hasClass("combotree-f")) {
                        //
                        return new mCombotree(sel);
                    }
                    if ($(sel).parent().prev(".combobox-f,.combo-f").length > 0 || $(sel).hasClass("combobox-f")) {
                        //
                        return new mCombobox(sel);
                    }
                }
            };
            return mCombo;
        })();
        mCombo.name = "combo";
        mCombo.className = "combo-f";
        mCombo.inputSelector = ".combo-text:not(.datebox-input)";
        window.mCombo = $.mCombo = mCombo;
    })();
    //numberbox
    (function () {
        var mNumberbox = (function () {
            var mNumberbox = function (sel) {
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).numberbox("getText");
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector);
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).numberbox("getValue");
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    $(selector).numberbox("setValue", value);
                };
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint属性
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };

                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).numberbox("getValue") ? false : true;
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    that.getInput(selector).trigger("focus.hint");
                    var result = $(selector).numberbox("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).numberbox("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).numberbox("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).width(width);
                };
            };
            mNumberbox.name = "numberbox";
            mNumberbox.className = "numberbox-f";
            mNumberbox.createInstance = function (selector) {
                return new mNumberbox(selector);
            };
            return mNumberbox;
        })();
        window.mNumberbox = $.mNumberbox = mNumberbox;
    })();
    //numberspinner
    (function () {
        var mNumberSpinner = (function () {
            var mNumberSpinner = function (sel) {
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).numberspinner("getText");
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector);
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).numberspinner("getValue");
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    $(selector).numberspinner("setValue", value);
                };
                this.getUpspinner = function (selector) {
                    selector = selector || sel;
                    return $(selector).siblings().find(".spinner-arrow-up");
                };
                this.getDownspinner = function (selector) {
                    selector = selector || sel;
                    return $(selector).siblings().find(".spinner-arrow-down");
                };
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint属性
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };

                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).numberspinner("getValue") ? false : true;
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    that.getInput(selector).trigger("focus.hint");
                    var result = $(selector).numberspinner("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).numberspinner("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).numberspinner("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).numberspinner("resize", width);
                };
            };
            mNumberSpinner.name = "numberspinner";
            mNumberSpinner.className = "numberspinner-f";
            mNumberSpinner.inputSelector = ".numberspinner-f";
            mNumberSpinner.createInstance = function (selector) {
                return new mNumberSpinner(selector);
            };
            return mNumberSpinner;
        })();
        window.mNumberSpinner = $.mNumberSpinner = mNumberSpinner;
    })();
    //datebox
    (function () {
        var mDatebox = (function () {
            var mDatebox = function (sel) {
                //如果sel有combo-f，则表示其不是原始的输入框，而是后期生成的
                sel = $(sel).hasClass("combo-text") ? $(sel).parent().prev(".datebox-f") : sel;
                var that = this;
                this.getText = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("getText");
                };
                this.setText = function (text, selector) {
                    selector = selector || sel;
                    return $(selector).datebox("setText", text);
                };
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).next().find(".combo-value").attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("textbox");
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("getValue");
                };
                this.getPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("panel");
                };
                this.showPanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("showPanel");
                };
                this.hidePanel = function (selector) {
                    selector = selector || sel;
                    return $(selector).datebox("hidePanel");
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    $(selector).datebox("setValue", mDate.parse(value));
                };
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint属性
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };
                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).datebox("getValue") ? false : true;
                };
                this.validate = function (selector) {
                    selector = selector || sel;
                    //获取用户目前输入的值
                    var userInput = that.getText(selector);
                    //如果用户正常选择了，则getValue的值不为空
                    var selectedValue = that.getValue(selector);
                    //如果用户输入为空，就不要去转化了
                    if (userInput) {
                        //进行格式转化
                        var dateInput = mDate.format(userInput);
                        //如果dateInput == undefined
                        dateInput = dateInput ? dateInput : "";
                        //赋值到combobox
                        dateInput != mDate.format(that.getValue()) && that.setValue(dateInput);
                    };
                    //如果没有传value的值过来
                    var value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        mHint.blurFunc(that.getInput(selector), value);
                    }
                    var result = $(selector).datebox("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).datebox("disable");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).datebox("enable");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).datebox("resize", width);
                };
            };
            mDatebox.name = "datebox";
            mDatebox.className = "datebox-f";
            mDatebox.inputSelector = ".datebox-input";
            mDatebox.createInstance = function (selector) {
                return new mDatebox(selector);
            };
            return mDatebox;
        })();
        window.mDatebox = $.mDatebox = mDatebox;
    })();
    //validatebox
    (function () {
        var mValidatebox = (function () {
            var mValidatebox = function (sel) {
                var that = this;
                this.getName = function (selector) {
                    selector = selector || sel;
                    return $(selector).attr("name");
                };
                this.getInput = function (selector) {
                    selector = selector || sel;
                    return $(selector);
                };
                this.getSourceInput = function (selector) {
                    return selector = selector || sel;;
                };
                this.getValue = function (selector) {
                    selector = selector || sel;
                    return $(selector).val();
                };
                this.setValue = function (value, selector) {
                    selector = selector || sel;
                    $(selector).val(value);
                };
                this.setHint = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("hint");
                    //如果value有值
                    if (value) {
                        //获取需要展示的hint
                        var $hint = that.getInput(selector);
                        //把这个也加上has-hint样式
                        $hint.addClass("has-hint");
                        //加上hint属性
                        $hint.attr("hint", $(selector).attr("hint"));
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                        $hint.off("blur.hint").on("blur.hint", function () {
                            mHint.blurFunc(this);
                        }).off("focus.hint").on("focus.hint", function () {
                            mHint.focusFunc(this, value);
                        }).blur();
                    }
                };
                this.setTabIndex = function (value, selector) {
                    selector = selector || sel;
                    //如果没有传value的值过来
                    value = value || $(selector).attr("tabIndex");
                    //如果value有值
                    if (value) {
                        //获取需要展示的tabIndex
                        var $tabIndex = that.getInput(selector);
                        //加上tabIndex属性
                        $tabIndex.attr("tabIndex", $(selector).attr("tabIndex"));
                    }
                };

                this.isValueChange = function (selector) {
                    selector = selector || sel;
                    var originalValue = $(selector).attr("originalValue");
                    if (originalValue == undefined) {
                        return false;
                    }
                    return originalValue == $(selector).val() ? false : true;
                }
                this.validate = function (selector) {
                    selector = selector || sel;
                    that.getInput(selector).trigger("focus.hint");
                    var result = $(selector).validatebox("isValid");
                    that.getInput(selector).trigger("blur.hint");
                    return result;
                };
                this.disable = function (selector) {
                    selector = selector || sel;
                    $(selector).attr("readonly", "readonly");
                };
                this.enable = function (selector) {
                    selector = selector || sel;
                    $(selector).attr("readonly", "");
                };
                this.resize = function (width, selector) {
                    //
                    selector = selector || sel;
                    //
                    $(selector).width(width);
                };
            };
            mValidatebox.name = "validatebox";
            mValidatebox.className = "easyui-validatebox";
            mValidatebox.createInstance = function (selector) {
                return new mValidatebox(selector);
            };
            return mValidatebox;
        })();
        window.mValidatebox = $.mValidatebox = mValidatebox;
    })();

    //easy目前的控件集
    var EasyUIControls = [
        mTextBox, mCombobox, mCombotree, mDatebox, mNumberbox, mValidatebox
    ];
    //常量定义
    var mEasyUI = (function () {

        //
        var mEasyUI = function (selector) {
            //
            var that = this;
            //

        };
        //返回
        return mEasyUI;
    })();
    //绑定到$对象上

    //是否是EasyUI组件 返回的是这个组件的信息
    $.fn.mIsEasyUIControl = function (selector) {
        //
        selector = selector ? selector : this;
        //遍历样式集
        for (var i = 0; i < EasyUIControls.length ; i++) {
            //比较
            if ($(selector).hasClass(EasyUIControls[i].className)) {
                return EasyUIControls[i].createInstance(selector);
            }
        }
        return false;
    };
    //获取easyui组件的文本
    $.fn.mGetEasyUIControlText = function (type) {
        return type.createInstance(this).getText();
    }
    //获取easyui组件的值
    $.fn.mGetEasyUIControlValue = function (type) {
        return type.createInstance(this).getValue();
    }
    //easy组件校验
    $.fn.mValidateEasyUI = function (selector) {
        //
        selector = selector ? selector : this;
        //返回结果
        var result = true;
        //获取所有的easyui组件
        var easyUIControls = $(selector).mGetEasyUIControls();
        //遍历每一种类型的组件校验
        for (var i = 0; i < easyUIControls.length ; i++) {
            //获取其中的一种
            var control = easyUIControls[i];
            //对其中某一种进行遍历
            for (var j = 0; j < control.controls.length ; j++) {
                //校验
                result = control.type.createInstance(control.controls[j]).validate() && result;
            }
        }
        return result;
    };
    //取消easyuiUI的Validate样式
    $.fn.mCancelValidateClass = function (selector) {
        //
        selector = selector ? selector : this;
        //
        $(".validatebox-invalid").not(".easyui-load-valid").removeClass("validatebox-invalid");
    };
    //获取easy多有的控件集
    $.fn.mGetEasyUIControls = function (selector) {
        //
        selector = selector ? selector : this;
        //控件集合
        var result = []
        //循环遍历获取每一种控件
        for (var i = 0 ; i < EasyUIControls.length ; i++) {
            //某种控件类型
            var easyUIControl = EasyUIControls[i];
            //获取控件
            var controls = ($(selector).hasClass(easyUIControl.className) && !$(selector).hasClass("no-validate")) ? [$(selector)] : $("." + easyUIControl.className + ":not(.no-validate)", selector);
            //如果找到则加入
            if (controls && controls.length > 0) {
                //加入集合
                result.push({ type: easyUIControl, controls: controls });
            }
        }
        //返回
        return result;
    };
    $.fn.mGetComboboxObject = function (valueField, selector) {
        //
        selector = selector ? selector : this;
        //获取所有的data
        var datas = $(selector).combobox("getData");
        //
        var value = $(selector).combobox("getValue");
        //
        for (var i = 0; i < datas.length; i++) {
            //
            if (datas[i][valueField] == value) {
                return datas[i];
            }
        }
        //
        return {};
    };
    //根据一行的数据以及keyName获取里面的值拼接成字符串
    $.fn.mGetKeyValueByKeyName = function (data, keyName) {
        //
        keyName = keyName.split(",");
        //
        var keyValue = [];
        //
        for (var i = 0; i < keyName.length ; i++) {
            //
            keyValue.push(_.encode(data[keyName[i]]));
        }
        //
        return keyValue.join(',');
    }
    //找到grid某一行的数据
    $.fn.mGetDataGridSingleRowData = function (table, selector, keyName, keyValue) {
        //获取唯一标识符的行名称
        var keyName = $(selector).attr(keyName || "keyname").split(',');
        //标识符值
        var keyValue = $(selector).attr(keyValue || "keyvalue").split(',');
        //先获取所有的行
        var rows = $(table).datagrid("getRows");
        //每一行进行筛选
        for (var i = 0; i < rows.length ; i++) {
            //
            var match = true;
            //必须保证每一个都正确
            for (var j = 0; j < keyName.length ; j++) {
                //
                match = match && (rows[i][keyName[j]] === _.decode(keyValue[j]));
            }
            //
            if (match === true) {
                //
                return rows[i];
            }
        }
        //
        return false;
    }
    //绑定datagrid格子里面的事件
    $.fn.mBindDatagridRowEvent = function (table, selector, callback, keyName, keyValue) {

        //找到对应的行之后
        var rows = $.fn.mGetDataGridSingleRowData(table, selector, keyName, keyValue);
        //如果有回调函数，就调用回调函数
        if ($.isFunction(callback) && row) {
            //
            $(selector).off("click.eu").on("click.eu", function () {
                //
                callback.apply(row);
            });
        }
    }
    //获取combobox的object对象

    //扩展到mEasyUI上
    mEasyUI.mIsEasyUIControl = $.fn.mIsEasyUIControl;
    //扩展到mEasyUI上
    mEasyUI.mGetEasyUIControlText = $.fn.mGetEasyUIControlText;
    //扩展到mEasyUI上
    mEasyUI.mGetEasyUIControlValue = $.fn.mGetEasyUIControlValue;
    //扩展到mEasyUI上
    mEasyUI.mValidateEasyUI = $.fn.mValidateEasyUI;
    //扩展到mEasyUI上
    mEasyUI.mGetEasyUIControls = $.fn.mGetEasyUIControls;
    //扩展
    mEasyUI.mCancelValidateClass = $.fn.mCancelValidateClass;
    //绑定datagrid里面的事件
    mEasyUI.mBindDatagridRowEvent = $.fn.mBindDatagridRowEvent;
    //
    mEasyUI.mGetDataGridSingleRowData = $.fn.mGetDataGridSingleRowData;
    //
    mEasyUI.mGetKeyValueByKeyName = $.fn.mGetKeyValueByKeyName;
    //把mEasyUI扩展到$ window Megi上
    $.mEasyUI = window.mEasyUI = Megi.mEasyUI = mEasyUI;
})(Megi);

/*添加特殊的功能，把datebox后面的点击以及focus事件挂钩*/

/*针对于Menu点击的时候，需要点击div来响应点击内部div以及span的事件*/
$(function () {
    $(document).off("click.menu", ".menu-item").on("click.menu", ".menu-item", function (event) {
        //获取当前的触发对象
        var target = $.browser.msie ? event.srcElement : event.target;
        //只有在触发对象是本div的时候才触发内部事件，禁止内部的冒泡引起的无限循环
        if (target == this) {
            //事件触发
            $("a,span,div,li,input[type='button']", $(this)).click();
        }
    });

    //找出所有的datebox
    $(document).off("focus.minput", window.mCombo.inputSelector).on("focus.minput", window.mCombo.inputSelector, function () {
        //
        //新建一个对象
        var comboBox = new window.mCombo(this).createInstance();
        //
        var input = comboBox.getInput();//.select();
        //如果是只读的，就不需要选择了
        if (input.attr("readonly") == "readonly") {
            return;
        }
        //初始提示信息的focus事件
        //显示面板
        comboBox.showPanel();
        //触发一下hint事件
        $(this).trigger("focus.hint");
    }).off("blur.minput", window.mCombo.inputSelector).on("blur.minput", window.mCombo.inputSelector, function () {
        //标志为未选中
        $(this).attr("allsel", 0);
        //新建一个对象
        var comboBox = new window.mCombo(this).createInstance();
        //
        var input = comboBox.getInput()
        //如果用户没有输入值，则清空值
        if (input.val() == "") {
            //
            comboBox.setValue("");
            //
            comboBox.setText("");
        }
    });
    $("div:not(#divOrgList .item-list):not(.combo-panel),span,tbody").off("scroll.hi").on("scroll.hi", function (event) {
        //IE同意不做处理
        if (Megi.isIE()) {
            return false;
        }

        //如果是日期控件本身滚动则不做处理，解决日期控件（无内容时）第一次输入数字时触发。
        if ($(event.target || event.srcElement).is(".combo"))
            return;

        var combos = $(window.mCombo.inputSelector, this);
        //
        var fond = false;
        //
        for (var i = 0; i < combos.length ; i++) {
            //新建一个对象
            var comboBox = new window.mCombo(combos.eq(i)).createInstance();
            //
            comboBox.hidePanel();
            //
            fond = true;
        }
        //
        var dates = $(window.mDatebox.inputSelector, this);
        //
        for (var i = 0; i < dates.length ; i++) {
            //新建一个对象
            var dateBox = new window.mDatebox(dates.eq(i));
            if (!dateBox.getPanel().is(":visible")) {
                continue;
            }
            //
            dateBox.hidePanel();
            //
            dateBox.getInput().trigger("blur");
            //
            fond = true;
        }
        //
        fond && $(document).trigger("click");
    });

    //找出所有的datebox
    $(document).off("focus.minput", window.mDatebox.inputSelector).on("focus.minput", window.mDatebox.inputSelector, function () {
        //新建一个对象
        var datebox = new window.mDatebox(this);
        //触发一下hint事件
        $(this).trigger("focus.hint");
        //显示面板
        datebox.showPanel();
    }).off("click.minput", window.mDatebox.inputSelector).on("click.minput", window.mDatebox.inputSelector, function () {
        //如果文本长度不为空
        if ($(this).val().length > 0) {
            //光标位置
            var pos = getPosition(this);
            //选中日期对应的文本
            var start = getForeNumberPosition($(this).val(), pos);
            //结束位置
            var end = getAfterNumberPosition($(this).val(), pos);
            //记录当前选中的位置，1，2，3  0表示年，1表示月，2表示日
            $(this).attr("location", start == 0 ? 1 : (end == ($(this).val().length) ? 3 : 2));
            //开始位置，截至位置
            $(this).attr("start", start);
            //截至
            $(this).attr("end", end);
            //选中位置
            setSelect(this, start, end);
        }
    }).off("keyup.minput", window.mDatebox.inputSelector).on("keyup.minput", window.mDatebox.inputSelector, function (e) {
        //新建一个对象
        var datebox = new window.mDatebox(this);
        //获取光标位置
        var pos = getPosition(this);
        //
        switch (e.keyCode) {
            //left
            case 37:
                //左移动
                pos && setCursorPos(this, pos - 1);
                $(this).trigger("click.minput");
                break;
            case 8:
                //删除键
                var vals = $(this).val();
                if (vals) {
                    if (vals.length == 4 || vals.length == 7) {
                        $(this).val(vals.substring(0, vals.length - 1));
                    }
                }
                break;
                //up
            case 38:
                calcSelection(datebox, 1);
                break;
                //Tab right
            case 39:
                //左移动
                (pos < $(this).val().length + 1) && setCursorPos(this, pos + 1);
                $(this).trigger("click.minput");
                break;
                //down
            case 40:
                calcSelection(datebox, -1);
                break;

            default:
                break;
        }


        //如果输入的就是分隔符，则取消输入
        if ($(this).val().length > 5 && $(this).val().substr($(this).val().length - 2, 1) == mDate.Spliter && $(this).val().substr($(this).val().length - 1, 1) == mDate.Spliter) {

            $(this).val($(this).val().substr(0, $(this).val().length - 1));

            return setCursorPos(this, pos - 1);
        }

        //如果是数字键
        if ((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105)) {
            //如果光标的位置在4,6则自动跳转到下一个
            if ((pos == 4 || pos == 7
                || (pos == 6 && ((e.keyCode >= 50 && e.keyCode <= 57) || (e.keyCode >= 98 && e.keyCode <= 105))))) {
                //
                if (pos < ($(this).val().length)) {
                    //跳转
                    setCursorPos(this, pos + 1);
                    //选中
                    $(this).trigger("click.minput");
                }
                else {
                    //后面自动加一杠
                    $(this).val($(this).val().trimEnd(mDate.Spliter) + mDate.Spliter);
                    //光标设置到最后
                    setCursorPos(this, pos + 1);
                }
            }
        }

    }).off("blur.minput", window.mDatebox.inputSelector).on("blur.minput", window.mDatebox.inputSelector, function () {
        //新建一个对象
        var comboBox = new window.mDatebox(this);
        //
        var value = mDate.parse($(this).val());
        //
        if (comboBox && value && comboBox.getPanel().is(":hidden")) {
            //
            comboBox.setValue(value);
            //
            comboBox.setText(mDate.format(mDate.parse(value)));
        }
    });


    //找到所有的numberspinner
    $(document).off("keydown.spinner", window.mNumberSpinner.inputSelector).on("keydown.spinner", window.mNumberSpinner.inputSelector, function (e) {
        switch (e.keyCode) {
            case 38:
                new mNumberSpinner(this).getUpspinner().trigger("click");
                break;
            case 40:
                new mNumberSpinner(this).getDownspinner().trigger("click");
                break;
            default: break;
        }
    });

    //对所有的可见可用的input进行xss脚本过滤
    $(document).off("blur.xss", "input:visible:not(:disabled)").on("blur.xss", "input:visible:not(:disabled)", function (e) {
        //
        $(this).val($(this).val());
    });

    //计算选中的文本，加或者减
    function calcSelection(dateBox, m) {
        //
        var location = dateBox.getInput().attr("location");
        //开始位置，截至位置
        var start = dateBox.getInput().attr("start");
        //截至
        var end = dateBox.getInput().attr("end");
        //必须有设定了位置
        if (location) {
            //获取文本内容
            var dateValue = mDate.parse(dateBox.getInput().val());
            //是否是日期
            if (dateValue) {
                //
                switch (location) {
                    case "1":
                        dateValue = dateValue.addYears(m);
                        break;
                    case "2":
                        dateValue = dateValue.addMonths(m);
                        break;
                    case "3":
                        dateValue = dateValue.addDays(m);
                        break;
                    default:
                        break;
                }
                //然后再赋值
                dateBox.setValue(dateValue);
                //设置选中
                setSelect(dateBox.getInput()[0], start, end);
            }
        }
    };
    //选中一个光标前面所有的数字（非-/开始位置)
    function getForeNumberPosition(text, pos) {
        //
        for (var i = pos - 1; i > 0; i--) {
            //如果这个位置上不是1-9的数字
            if (!text.charAt(i).match(/[0-9]/)) {
                //返回上一个位置
                return i + 1;
            }
        }
        //如果遍历完了，就返回0
        return 0;
    };
    //选中一个光标前面所有的数字（非-/开始位置)
    function getAfterNumberPosition(text, pos) {
        //
        for (var i = pos; i < text.length; i++) {
            //如果这个位置上不是1-9的数字
            if (!text.charAt(i).match(/[0-9]/)) {
                //返回上一个位置
                return i;
            }
        }
        //如果遍历完了，就返回
        return text.length;
    };
    //设置光标位置
    function setCursorPos(ctrl, pos) {
        //IE
        if (navigator.userAgent.indexOf("MSIE") > -1) {
            var range = document.selection.createRange();
            var textRange = ctrl.createTextRange();
            textRange.moveStart('character', pos);
            textRange.collapse();
            textRange.select();
        } else {
            ctrl.setSelectionRange(pos, pos);
        }
    };
    //获取光标所在位置
    function getPosition(ctrl) {
        //默认是0
        var pos = 0;
        //IE的用法
        if (document.selection) {
            //先获取焦点
            //$(ctrl).focus();
            //创建一个选中
            var sel = document.selection.createRange();
            //往前
            sel.moveStart('character', -$(ctrl).val().length);
            //就是文本的长度
            pos = sel.text.length;
        }
        else if (ctrl.selectionStart || ctrl.selectionStart == '0') {
            //兼容FireFox
            pos = ctrl.selectionStart;
        }
        return (pos);
    };
    //设置选中位置
    function setSelect(ctrl, start, end) {
        //如果没有start 和 end，则选中全部
        start = start || 0;
        //
        end = end || $(ctrl).val().length;
        //如果支持选中文本
        if (ctrl.setSelectionRange) {
            //先获取焦点
            ctrl.focus();
            //选中文本
            ctrl.setSelectionRange(start, end);
        }
        else if (ctrl.createTextRange) {
            //创建一个选择对象
            var range = ctrl.createTextRange();
            //
            range.collapse(true);
            //开始
            range.moveEnd('character', end);
            //截至
            range.moveStart('character', start);
            //选中
            range.select();
        }
    };
})