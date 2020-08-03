var UCReportFooter = {
    options: null,
    init: function (opts) {
        $(".footer-content").html("");
        if (opts == undefined) {
            opts = {};
        }
        if (opts.Items == undefined || opts.Items == null) {
            opts.Items = new Array();
        }
        UCReportFooter.options = opts;
        if (opts.Items.length == 0) {
            return;
        }
        UCReportFooter.bindList();
    },
    bindList: function () {
        var html = '';
        for (var i = 0; i < UCReportFooter.options.Items.length; i++) {
            var item = UCReportFooter.options.Items[i];
            html += UCReportFooter.getItemHtml(item);
        }
        $(".footer-content").html(html);
        UCReportFooter.bindEvent();
    },
    bindEvent: function () {
        if (!UCReportFooter.options.hasChangeAuth) {
            return;
        }
        $(".footer-item>.item-view").unbind();
        $(".footer-item>.item-view").click(function () {
            var number = $(this).attr("number");
            var value = UCReportFooter.getItemValue(number);
            $(this).closest(".footer-item").find("textarea").val(value);
            UCReportFooter.editItem(number);
        }).mouseover(function () { $(this).addClass("hover"); }).mouseout(function () { $(this).removeClass("hover"); });
        $(".footer-item>.item-edit>.footer-action>.report-footer-save").click(function () {
            $(this).closest(".item-edit").hide();
            var number = $(this).attr("number");
            var value = $(this).closest(".item-edit").find("textarea").val();
            $(this).closest(".footer-item").find(".item-view").html(mText.encode(value)).show();
            UCReportFooter.updateItem(number, value)


            var rowIndex = $(this).attr("rowIndex");
            var cellIndex = Number($(this).attr("cellIndex"));
            //更新数据源
            UCReport.updateNote(rowIndex, cellIndex, number, value);

        });

        $(".footer-item>.item-edit>.footer-action>.report-footer-cancel").click(function () {
            $(this).closest(".item-edit").hide();
            $(this).closest(".footer-item").find(".item-view").show();
        });
        $(".footer-item>.item-close>.mg-icon-delete").click(function () {
            var number = $(this).attr("number");
            UCReportFooter.deleteItem(number);
            $(this).closest(".footer-item").remove();
        });
    },
    convertToHtml: function (value) {
        if (value == null || value == undefined) {
            return "";
        }
        return value.replace(/\n/g, "<br/>");
    },
    updateItem: function (number, value) {
        for (var i = 0; i < UCReportFooter.options.Items.length; i++) {
            if (UCReportFooter.options.Items[i].No == number) {
                UCReportFooter.options.Items[i].Value = value;
            }
        }
    },
    getItemValue: function (number) {
        for (var i = 0; i < UCReportFooter.options.Items.length; i++) {
            if (UCReportFooter.options.Items[i].No == number) {
                return UCReportFooter.options.Items[i].Value;
            }
        }
        return "";
    },
    deleteItem: function (number) {
        var arr = new Array();
        for (var i = 0; i < UCReportFooter.options.Items.length; i++) {
            if (UCReportFooter.options.Items[i].No != number) {
                arr.push(UCReportFooter.options.Items[i]);
            }
        }
        UCReportFooter.options.Items = arr;
        $(".note-no").each(function () {
            var noteNumber = $(this).attr("number");
            if (number == noteNumber) {
                var tdObj = $(this).closest("td");
                var rowIndex = $(tdObj).attr("rowIndex");
                var cellIndex = Number($(tdObj).attr("cellIndex"));
                //更新数据源
                UCReport.deleteNote(rowIndex, cellIndex, number);
                $(this).remove();
            }
        });
    },
    addItem: function (rowIndex, cellIndex) {
        var number = 1;
        if (UCReportFooter.options && UCReportFooter.options.Items.length > 0) {
            number = number + Number(UCReportFooter.options.Items[UCReportFooter.options.Items.length - 1].No);
        }
        number = number.toString();
        var obj = {};
        obj.No = number;
        obj.Value = "";
        obj.rowIndex = rowIndex;
        obj.cellIndex = cellIndex;

        UCReportFooter.options.Items.push(obj);
        UCReportFooter.bindList();
        UCReportFooter.editItem(number, true);
        return number;
    },
    editItem: function (number, isAdd) {
        $(".footer-item").each(function () {
            var itemNo = $(this).attr("number");
            if (number == itemNo) {
                $(this).find(".item-view").hide();
                $(this).find(".item-edit").show();
                $(this).find(".item-edit").find("textarea").focus();
                if (isAdd) {
                    $(this).find(".report-footer-cancel").hide();
                } else {
                    $(this).find(".report-footer-cancel").show();
                }
            } else {
                $(this).find(".item-edit").hide();
                $(this).find(".item-view").show();
            }
        });
    },
    getItemHtml: function (item) {
        var html = '';
        html += '<div class="footer-item" number="' + item.No + '" id="reportnote' + item.No + '">';
        html += '<div class="item-no">' + item.No + '</div>';
        html += '<div class="item-edit" style="display:none;">';
        html += '<div class="item-edit-box"><textarea>' + item.Value + '</textarea></div>';
        html += '<div class="footer-action"><a href="javascript:void(0)" rowIndex="' + item.rowIndex + '" cellIndex="' + item.cellIndex + '" class="easyui-linkbutton easyui-linkbutton-yellow m-button-small report-footer-save" number="' + item.No + '">' + HtmlLang.Write(LangModule.Common, "Save", "Save") + '</a><a href="javascript:void(0)" class="easyui-linkbutton l-btn m-button-small report-footer-cancel">' + HtmlLang.Write(LangModule.Common, "Cancel", "Cancel") + '</a></div>';
        html += '<br class="clear" />';
        html += '</div>';
        html += '<div class="item-view" number="' + item.No + '">';
        html += mText.encode(item.Value);
        html += '</div>';
        if (UCReportFooter.options.hasChangeAuth) {
            html += '<div class="item-close"><a href="javascript:void(0)" class="mg-icon-delete" number="' + item.No + '">&nbsp;</a></div>';
        }
        html += '<br class="clear"/>';
        html += '</div>';
        return html;
    }
}
