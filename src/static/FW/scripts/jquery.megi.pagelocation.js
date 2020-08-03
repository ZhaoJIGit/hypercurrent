//页面刷新后的位置还原
var PageLocation = (function () {
    function PageLocation() {
        //滚动元素
        this.scrollElement = ".m-location-restore";
        this.scrollClearElement = ".m-location-clear";
        this.scrollPageClearElement = ".m-page-location-clear";

        //滚动事件绑定元素，例如table 只能绑定在tbody上
        this.eventBindElement = "";

        //用来记录是否已经还原
        this.isRestore = false;
        //滚动元素是不是在重新加载数据，这个时候，不对滚动位置做记录
        this.isReloadData = false;
        this.pageName = window.location.pathname;
        this.key = window.location.pathname + "_scollposition";

        //分页控件
        this.pagerElement = ".pagination";
        this.pagerPre = ".pagination-prev";
        this.pagerNext = ".pagination-next";
        this.pagerFirst = ".pagination-first";
        this.pagerLast = ".pagination-last";

        this.pageIndex = 1;

        this.isFirstLoad = true;
    };

    //初始化
    PageLocation.prototype.init = function () {
        var _this = this;

        this.initUI();

        this.setEventElement();

        this.initEvent();

        this.restoreScrollPosition();
    };

    //给每个滚动设置一个索引
    PageLocation.prototype.initUI = function () {
        var _this = this;
        $(_this.scrollElement).each(function (index) {
            $(this).attr("scroll-index", index);
        });

        //如果是第一次加载，先清空所有位置
        if (!window.name || window.name != _this.pageName) {
            _this.clearPosition();
            window.name = _this.pageName;
        }
    }

    //获取索引
    PageLocation.prototype.getIndex = function (selector) {

        if (selector[0].tagName.toLowerCase() == "tbody") {
            selector = selector.parent();
        }

        return selector.attr("scroll-index");
    }

    //事件绑定
    PageLocation.prototype.initEvent = function () {
        var _this = this;
        //页面刷新事件 记录当前滚动条位置

        $(_this.eventBindElement).each(function () {
            $(this).off("scroll.positionrestore").on("scroll.positionrestore", function (e) {
                if (!_this.isReloadData) {
                    var scrollIndex = _this.getIndex($(this));
                    _this.recordScrollPosition(scrollIndex, $(this));
                }

                //跳过一次后解除锁定
                _this.isReloadData = false;

            });
        })

        $(_this.eventBindElement).each(function () {
            $(this).off("onload.positionrestore").on("onload.positionrestore", function () {
                //查找是第几个元素需要还原滚动位置
                var index = _this.getIndex($(this));

                var topSize = localStorage.getItem(_this.key + "_" + index);

                $(this).scrollTop(+topSize);

                _this.isRestore = true;

            });
        });

        $(document).off("click.clear", _this.scrollClearElement).on("click.clear", _this.scrollClearElement, function () {

            var index = +($(_this.scrollElement + ":visible").attr("scroll-index"));

            _this.clearPosition(index);
        });

        $(document).off("click.clear", _this.scrollPageClearElement).on("click.clear", _this.scrollPageClearElement, function () {

            $(_this.scrollElement).each(function(){

                var index = +($(this).attr("scroll-index"));
                _this.clearPosition(index);
            });

            
        });

        //这个事件捕获的是元素刷新事件，用来判断是否需要重新进行定位
        $(_this.scrollElement).each(function () {
            $(this).off("DOMNodeRemoved.positionrestore").on("DOMNodeRemoved.positionrestore", function (e) {

                _this.isRestore = false;
                window.name = _this.pageName;
                _this.isReloadData = true;

                var srollElementIndex = _this.getIndex($(this));

                var pager = $(_this.pagerElement);

                if (pager && pager.length > 0) {
                    var pagerOptions = pager.pagination("options");

                    if (!pagerOptions) {
                        return;
                    }

                    var currentPageNumber = pagerOptions.pageNumber;

                    if (currentPageNumber != _this.pageIndex) {
                        _this.clearPosition(srollElementIndex);

                        _this.pageIndex = currentPageNumber;
                    }
                }

            });
        })
    };

    PageLocation.prototype.recordScrollPosition = function (index, ele) {
        var _this = this;
        var top = $(ele).scrollTop();

        localStorage.setItem(_this.key + "_" + index, top);

    };

    PageLocation.prototype.restoreScrollPosition = function () {
        var _this = this;
        var setInterVal = setInterval(function () {
            //判断是否可以进行还原
            if (_this.isRetorePosition()) {

                $(_this.eventBindElement).each(function () {
                    $(this).trigger("onload");
                });
            }
        }, 100);

    };

    PageLocation.prototype.isRetorePosition = function () {
        var _this = this;

        if (window.name == "") {
            return false;
        }

        if (_this.isRestore) {
            return false;
        }

        return true;
    };

    PageLocation.prototype.setEventElement = function () {
        var _this = this;

        //没有合适的元素
        var selector = $(_this.scrollElement);

        if (selector.length == 0) {
            return;
        }

        if (selector[0].tagName.toLowerCase() == "table") {
            _this.eventBindElement = _this.scrollElement + " tbody";
        } else {
            _this.eventBindElement = _this.scrollElement;
        }
    }

    PageLocation.prototype.clearPosition = function (index) {
        var _this = this;

        for (var i = 0 ; i < localStorage.length; i++) {
            var key = localStorage.key(i);

            //如果传入了滚动元素索引，就清空索引当前滚动元素位置，否则清空当前页面所有的位置
            var matchKey = index ? _this.key + "_" + index : _this.key;

            if (key && key.indexOf(matchKey) == 0) {
                localStorage.setItem(key, 0);
            }
        }
    };

    return PageLocation;
}());

$(document).ready(function () {
    var pageLocation = new PageLocation();
    pageLocation.init();
});