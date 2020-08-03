
(function () {

    var mTitle = (function () {


        var mTitle = function (selector, title, type) {


            var fileName = "mtitle", dataName = "titleTarget", tipsClass = "m-title-tip", warningClass = "m-title-warning";

            var that = this;

            //默认是提示图标
            type = type ? 0 : type;
            title = title || mText.decode($(selector).attr(fileName)) || $(selector).data(fileName);
            title = title ? (title.indexOf("<") != 0 ? mText.encode(title) : title) : "";
            title = title.replace(/\\n/gi, '<br>');

            //添加的样式
            var className = type == 1 ? warningClass : tipsClass;

            //初始化函数
            this.init = function () {

                //清除已经保存的信息
                var target = $(selector).data(dataName);
                target && target.remove();
                $(selector).data(dataName, null);

                //显示一个图标
                var t = $("<span class='m-title-icon'>&nbsp;</span>").insertAfter(selector).addClass(className).css({
                    top: (+$(selector).parent().css("line-height").replace("px", "") - 15) / 2
                });

                $(selector).data(dataName, t);

                //绑定鼠标移动上面去的事件

                var c = $("<div class='m-title-content'>" + title + "</div>").appendTo("body");

                //使用tip的形式展示
                $(t).mTip({
                    trigger: "mouseover",
                    target: c,
                    width: 300
                });
            }

            this.destory = function () {

                var target = $(selector).data(dataName);

                target && target.remove();
            }
        }

        return mTitle;
    })();

    window.mTitle = mTitle;

    $.fn.mTitle = function (title) {
        //如果找不到就不需要初始化了
        if ($(this).length === 0)
            return;
        if (title === "destory") {
            return new mTitle(this).destory();
        }
        new mTitle(this, title).init();
    }
})()