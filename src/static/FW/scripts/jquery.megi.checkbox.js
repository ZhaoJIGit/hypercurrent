(function ($) {
    $.fn.megicheckbox = function (options) {
        var selector = $(this);

        //在它的上面加上一个checkbox的div
        initUI(selector);
        //绑定事件
        bindKeyEvent(selector,options);
    };

    function initUI(selector) {
        var html = '<span class="tree-checkbox tree-checkbox0"></span>';
        $(selector).before(html);
    }

    function bindKeyEvent(selector, options) {
        //事件绑定在它的上一个同辈元素上
        $(selector).prev().click(function () {
            checkChange(selector);
            //检查它是否有子项，有子项同样选中

            var c = $(selector).attr("class");
            //没有值
            if (c.indexOf("megicheckbox-top")>=0) {
                var grid = options.grid;

                if ($(selector).attr("checked") == "checked") {
                    $(grid).treegrid("selectAll");

                    //选择所有行
                    $(".easyui-megicheckobx").each(function () {
                        $(this).attr("checked", "checked");
                        $(this).prev().removeClass("tree-checkbox0");
                        $(this).prev().removeClass("tree-checkbox2");
                        $(this).prev().addClass("tree-checkbox1");
                    });

                } else {
                    $(grid).treegrid("unselectAll");
                    $(".easyui-megicheckobx").each(function () {
                        $(this).removeAttr("checked");
                        $(this).prev().removeClass("tree-checkbox1");
                        $(this).prev().removeClass("tree-checkbox2");
                        $(this).prev().addClass("tree-checkbox0");
                    });
                }

            } else {
                
                checkChildren(selector, options);
                checkParents(selector, options);
            }
        });

        $(".megicheckbox-top").bind("click",function () {
            if ($(this).attr("checked") != "checked") {
                $(this).prev().removeClass("tree-checkbox1");
                $(this).prev().removeClass("tree-checkbox2");
                $(this).prev().addClass("tree-checkbox0");
            } else {
                $(this).prev().removeClass("tree-checkbox0");
                $(this).prev().removeClass("tree-checkbox2");
                $(this).prev().addClass("tree-checkbox1");
            }
        });
    }
    //checkbox样式改变 type:0全不选 1：部分选择 2全选
    function checkChange(selector,type) {
        if (type == 1) {
            $(selector).prev().removeClass("tree-checkbox1");
            $(selector).prev().removeClass("tree-checkbox0");
            $(selector).prev().addClass("tree-checkbox2");
            $(selector).removeAttr("checked");
        }else if (type==2 || $(selector).attr("checked") != "checked") {
            $(selector).prev().removeClass("tree-checkbox0");
            $(selector).prev().removeClass("tree-checkbox2");
            $(selector).prev().addClass("tree-checkbox1");
            $(selector).attr("checked", "checked");
        } else if (type == 0 || $(selector).attr("checked") == "checked") {
            $(selector).prev().removeClass("tree-checkbox1");
            $(selector).prev().removeClass("tree-checkbox2");
            $(selector).prev().addClass("tree-checkbox0");
            $(selector).removeAttr("checked");
        }
    }

    //选中它的子项
    function checkChildren(selector, options) {
        var id = "";
        if (selector.attr("type")) {
            var id = selector.val();
        } else {
            id = selector.id;
        }
        var type = selector.attr("checked") == "checked";
        type = type ? "2" : "0";
        var grid = options.grid;
        var children = grid.treegrid("getChildren", id);
        if (children && children.length > 0) {
            for (var i = 0; i < children.length; i++) {
                var child = $("input[value='" + children[i].id + "']");
                if (child) {
                    checkChange(child,type);
                    checkChildren(child, options);
                }
            }
        }
    }
    //检查父项
    function checkParents(selector, options) {
        var id = selector.val();
        var parent = options.grid.treegrid("getParent", id);
        //如果没有父项了返回
        if (!parent) {
            return;
        }
        var parentNode = $("input[value='" + parent.id + "']");
        var children = options.grid.treegrid("getChildren", parent.id);
        
        //原来是选中，因为是触发的点击事件，事件过后应该是不选中
        if (selector.attr("checked") != "checked") {
            //如果父节点是全选,应该要改成不全选
            if (parentNode.attr("checked") == "checked" && children.length == 1) {
                checkChange(parentNode, "0");
            } else {
                checkChange(parentNode, "1");
            }
        } else {
            var notCheckArray = getNotCheckedChild(parent, options);
            //如果没有选中的子项等于所有子项
            if (notCheckArray.length == children.length) {
                checkChange(parentNode,'0');
            } else if(notCheckArray.length ==0) {
                checkChange(parentNode, "2");
            } else if (notCheckArray.length > 1) {
                checkChange(parentNode,"1");
            }
        }

        checkParents(parentNode, options);

    }

    //获取子项中没有选中的项id
    function getNotCheckedChild(parent, options) {
        var array = new Array();
        var children = options.grid.treegrid("getChildren", parent.id);
        for (var i = 0; i < children.length; i++) {
            var child = $("input[value='" + children[i].id + "']");
            if (child.attr("checked") != "checked") {
                array.push(child.val());
            } 
        }
        return array;
    }

    //遍历所有的控件，进行初始化
    $(function () {
        $(".easyui-megicheckobx", "body").megicheckbox(options);
    });
})(jQuery)