(function () {

    var mFilter = (function () {

        var mFilter = function () {
        };

        return mFilter;
    })();

    window.mFilter = mFilter;

    //定义所有的过滤器
    window.mFilter.filters = [
        {
            type: "track",
            filter: function () {
                var domain = ".stdx.megiplus.com";
                var host = window.location.host.substring(window.location.host.indexOf('.'));
                return domain === host;
            },
            method: function (a, b, c, d, e) {
                if (top.window.mFootprint && top.window.mFootprint.track)
                    top.window.mFootprint.track(a, b, c, d, e);
                else
                    window.console && window.console.log("function not loaded.");
            }
        }
    ];
    //添加过滤器，返回当前对象，支持链式调用
    window.mFilter.add = function (filter) {
        mFilter.filters.push(filter);
        return this;
    };

    //初始化
    window.mFilter.initDomFilter = function () {

        $("[filter]").each(function () {

            $(this).off("click.filter").on("click.filter", function () {
                var filter = $(this).attr("filter");

                if (!filter) return;

                var splitValue = filter.split(';');

                if (splitValue.length < 2) return;

                var types = splitValue[0].split(',');

                var params = splitValue[1].split(',');

                mFilter.doFilter(types, params);
            });
        });
    };

    //过滤器执行
    window.mFilter.doFilter = function (types, params) {
        try {

            if (typeof types === "string")
                types = [types];

            if (!Array.isArray(params))
                params = [params];

            window.mFilter.filters = window.mFilter.filters || [];
            for (var i = 0; i < window.mFilter.filters.length; i++) {
                if (types.contains(window.mFilter.filters[i].type)) {

                    if (typeof mFilter.filters[i].filter === "function" && !mFilter.filters[i].filter()) continue;

                    if (typeof mFilter.filters[i].method !== "function") continue;

                    if (!params || params.length === 1)
                        mFilter.filters[i].method(params[0]);
                    else if (params.length === 2)
                        mFilter.filters[i].method(params[0], params[1]);
                    else if (params.length === 3)
                        mFilter.filters[i].method(params[0], params[1], params[2]);
                    else if (params.length === 4)
                        mFilter.filters[i].method(params[0], params[1], params[2], params[3]);
                    else if (params.length >= 5)
                        mFilter.filters[i].method(params[0], params[1], params[2], params[3], params[4]);
                }
            }
        } catch (e) {
            window.console && window.console.log("filter exception" + e);
        }

    };
})();

$(function () {
    mFilter.initDomFilter();
});