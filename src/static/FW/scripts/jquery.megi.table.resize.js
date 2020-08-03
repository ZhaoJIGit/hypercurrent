/*
 一个普通的表格，提供表格头拖拉调整宽度的事件
 Felson 2016.3.3
*/

(function () {

    var mTableResize = (function () {

        var mTableResize = function (selector, options) {
            //
            var that = this;
            //
            var headers = [];
            //
            var curGbox;
            //
            var resizeMark = ".m-resize-mark";
            //
            var headerResizeable = ".m-header-resizable";

            //最小宽度
            var minWidth = 33;
            //
            var tablePadding = options.tablePadding || 0;
            //
            var isresizing = false;
            //
            var forceFit = options.forceFit ? true : false;
            //
            var resizeTable = options.resizeTable;
            //

            var thMark = "th";

            this.initColDragEvent = function () {
                var th = $("th:visible", selector);
                if (!th || th.length == 0) {
                    //第一行作为表头
                    th = $("tr:visible", selector).first().find("td:visible");
                    thMark = "td";
                }



                $(headerResizeable, th).remove();
                //初始化headers


                $(th).each(function (index) {
                    //每一个th加一个 
                    $(this).append("<span class='" + headerResizeable.trimStart('.') + "'>&nbsp;</span>");
                });
                //初始化基准线 
                curGbox = $(resizeMark);
                //在body里面加一个基准线，如果有的话就不需要加了
                if (curGbox.length == 0) {
                    //
                    $('<div class="' + resizeMark.trimStart('.') + '" style="display: none;">&nbsp;</div>').appendTo($("body"));
                }
                //绑定事件
                $(headerResizeable, selector).off("mousedown").on("mousedown", function (event) {
                    //
                    that.initDragParam();
                    //获取最近的span 如果不止一个的话就不做拉伸
                    var $span = $(event.target).closest(thMark + ">span" + headerResizeable);
                    //
                    if ($span.length !== 1) {
                        return;
                    }
                    //获取行
                    var $th = $span.parent();
                    //获取偏移
                    var offset = [[$th.position().left + $th.outerWidth()]];
                    //
                    offset[0] -= $(selector).scrollLeft();
                    //
                    offset.push($(selector).position().top);
                    //
                    offset.push($(selector).height());
                    //开始拖拽
                    that.colDragStart(that.getThIndex($th), event, offset);

                });
                $(selector).off("mousemove.resize").on("mousemove.resize", function (event) {

                    if (isresizing) {
                        //
                        that.initDragParam();
                        //实行拖拽
                        that.colDragMove(event);

                        return false;
                    }
                    return true;
                });
                $(document).off("mouseup.resize", resizeMark).on("mouseup.resize", resizeMark, function (event) {
                    //
                    if (isresizing) {
                        //结束拖拽
                        that.colDragEnd(event);
                        //
                        return false;
                    }
                });
            };

            //拖拽开始事件
            this.colDragStart = function (index, event, offset) {
                //
                isresizing = { idx: index, startX: event.clientX, sOL: event.clientX };
                //
                $(selector)[0].style.cursor = "col-resize";
                //
                curGbox.css({ display: "block", left: (event.clientX) + tablePadding + 'px', top: offset[1] + 'px', height: offset[2] + 'px' });
                //
                document.onselectstart = function () { return false; };
            };
            //拖拽结束事件
            this.colDragMove = function (event) {
                //如果正在拖拽
                if (isresizing) {
                    //
                    var direction = "ltr";
                    //
                    var diff = event.clientX - isresizing.startX,
                                        h = headers[isresizing.idx],
                                        newWidth = direction === "ltr" ? h.width + diff : h.width - diff, hn, nWn;
                    var tblwidth = $(selector).width();
                    //
                    if (newWidth > minWidth) {
                        //
                        curGbox.css({ left: (isresizing.sOL + diff) + tablePadding + 'px' });
                        //强制调整
                        if (forceFit === true) {
                            //
                            hn = headers[isresizing.idx];
                            //
                            nWn = direction === "ltr" ? hn.width - diff : hn.width + diff;
                            //
                            if (nWn > minWidth) {
                                //
                                h.newWidth = newWidth;
                                //
                                hn.newWidth = nWn;
                            }
                        } else {
                            h.newWidth = newWidth;
                        }
                    }
                }
            };
            //获取当前th的下标
            this.getThIndex = function ($th) {
                //
                var index = 0;
                //
                for (var i = 0; i < headers.length ; i++) {
                    //
                    if ($(headers[i].el).is(":visible")) {
                        //
                        if ($th.attr("rIndex") == headers[i].rIndex) {
                            //
                            return index;
                        }
                        //
                        index++;
                    }
                }
                return index;
            };
            //获取一个随机额random
            this.getRandomId = function () {
                var Str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                for (var i = 0, r = ""; i < 10; i++) {
                    r += Str.charAt(Math.floor(Math.random() * 62));
                };
                return r;
            };
            //拖拽事件结束
            this.colDragEnd = function () {
                //切换光标样式
                $(selector)[0].style.cursor = "default";
                //如果正在拖拽
                if (isresizing) {
                    //
                    var idx = isresizing.idx, nw = headers[idx].newWidth || headers[idx].width;
                    //
                    nw = parseInt(nw, 10);
                    //
                    isresizing = false;
                    //
                    curGbox.css("display", "none");
                    //
                    headers[idx].width = nw;
                    //
                    headers[idx].el.style.width = nw + "px";
                    //
                    if (forceFit === true) {
                        nw = headers[idx].newWidth || headers[idx].width;
                        headers[idx].width = nw;
                        headers[idx].el.style.width = nw + "px";
                    }
                }
                //
                curGbox = null;
                //
                document.onselectstart = function () { return true; };
                //
                userDraged = true;
                //
                $.isFunction(resizeTable) ? resizeTable() : that.resizeTable();
            };
            //默认调整内部的控件的宽度
            this.resizeTable = function () {
                //把表格里面每一个td里面的各种组件都重新调整一下宽度
                $("td:visible", selector).each(function () {
                    //
                    var inputsArray = [];
                    //获取里面所有的input 和 textarea
                    var inputs = $("input:visible:not([type='checkbox']),textarea:visible", $(this)).each(function (index) {
                        //
                        inputsArray.push(this);
                    });
                    //获取所有的easyui组件
                    var easyUIControls = $(this).mGetEasyUIControls();
                    //遍历每一种类型的组件校验
                    for (var i = 0; i < easyUIControls.length ; i++) {
                        //获取其中的一种
                        var control = easyUIControls[i];
                        //对其中某一种进行遍历
                        for (var j = 0; j < control.controls.length ; j++) {
                            //校验
                            var c = control.type.createInstance(control.controls[j]);
                            //如果有本什有resize函数就调用本身的resize函数
                            c.resize($(this).innerWidth());
                            //
                            inputsArray = inputsArray.removeItem(c.getInput()[0]);
                        }
                    }
                    //
                    var td = $(this);
                    //普通的input，也要调整宽度
                    for (var i = 0  ; i < inputsArray.length; i++) {
                        //
                        $(inputsArray[i]).width(td.innerWidth());
                    };
                });
            }
            //
            this.initDragParam = function () {
                //
                headers = [];
                var th = $("th:visible", selector);
                if (!th || th.length == 0) {
                    //第一行作为表头
                    th = $("tr:visible", selector).first().find("td:visible");
                }
                //初始化headers
                $(th).each(function () {
                    //
                    var rIndex = that.getRandomId();
                    //保存原来的宽度
                    headers.push({ "el": this, "width": $(this).width(), rIndex: rIndex });
                    //加上一个坐标号
                    $(this).attr("rIndex", rIndex);
                });
                //初始化基准线 
                curGbox = $(resizeMark);
            }
            //是否有header标签
            this.hasThTag = function (selector) {
                var th = $("th:visible", selector);
                if (!th || th.length == 0) {
                    return false;
                }
                return true;
            }
            //
            this.init = function () {
                //
                that.initColDragEvent();
            }
        }
        //
        return mTableResize;
    })();
    //
    $.fn.mTableResize = function (options) {
        //
        options = options || {};
        //
        return new mTableResize($(this), options).init();
    };
})();

$(function () {
    //所有带一定样式的都要初始化
    $(".m-resize-table").each(function () {
        $(this).mTableResize({
            forceFit: false
        });
    });
});