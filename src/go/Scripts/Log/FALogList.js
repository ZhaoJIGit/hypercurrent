var FALog = {
    ItemId: $("#hidItemId").val(),
    //初始化页面操作
    init: function () {
        FALog.bindGrid();
    },
    //绑定列表
    bindGrid: function () {
        Megi.grid('#gridFALog', {
            resizable: true,
            auto: true,
            scrollY: true,
            pagination: false,
            height: "420",
            url: "/Log/Log/GetFALogList",
            queryParams: { MID: FALog.ItemId },
            columns: [[
                //操作日期
                {
                    title: LangKey.Date, field: 'MCreateDate', width: 120, align: 'center', sortable: false, formatter: function (value, row, index) {
                        return $.mDate.formatDateTime(row.MCreateDate);
                    }
                },
                //操作用户
                { title: LangKey.User, field: 'MUserName', width: 80, sortable: false },
                //更改类型
                {
                    title: HtmlLang.Write(LangModule.FA, 'ChangeType', '更改类型'), field: 'MType', width: 150, sortable: false, formatter: function (value) {
                        if (value==-1) {
                            return LangKey.New;
                        }
                        var retValue = new FAHome().getChangeTypeName(value);
                        return "<span title='" + mText.encode(retValue) + "' style='text-overflow: ellipsis;white-space: nowrap;overflow: hidden;width: 150px;'>" + mText.encode(retValue) + "</span>";
                    }
                },
                //操作明细
                {
                    title: LangKey.Details, field: 'MNote', width: 320, sortable: false,formatter:function(value) {
                        if (!value) {
                            return "";
                        }
                        return "<span title='" + mText.encode(value) + "' style='text-overflow: ellipsis;white-space: nowrap;overflow: hidden;width: 320px;'>" + mText.encode(value) + "</span>";
                    }
                }
            ]]
        });
    }
}

//页面加载
$(document).ready(function () {
    FALog.init();
});