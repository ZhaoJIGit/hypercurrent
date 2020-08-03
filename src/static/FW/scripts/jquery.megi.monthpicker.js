//扩展Easyui datagrid editor
$.extend($.fn.datagrid.defaults.editors, {
    monthpicker: {
        init: function (container, options) {
            //
            var readonly = options.readonly === true;
            //
            var lang = options.lang ? options.lang : "en";
            //
            var minDate = options.minDate;
            //
            var maxDate = options.maxDate;
            //
            var height = options.height || $(container).innerHeight();
            //
            var width = options.width || $(container).innerWidth();
            //
            var required = options.required === true;
            //
            var inputClass = options.className || "";
            //
            var inputText = "<input type='text' class='Wdate "
                + inputClass
                + " validatebox' data-options='required:" + required + "'  style='align:center;"
                + (height ? ("height:" + height + "px;") : "") + (width ? ("width:" + width + "px;") : "")
                + "' onfocus=\"WdatePicker({ dateFmt: 'yyyy-MM',readOnly:"
                + readonly + ", lang: '"
                + lang + "', skin:'"
                + lang + "'"
                + (minDate ? (",minDate:'" + minDate + "'") : "")
                + (maxDate ? (",maxDate:'" + maxDate + "'") : "")
                + ((options.onpicked && $.isFunction(options.onpicked)) ? (",onpicked:function(){$(this).trigger('onpicked');}") : "")
                + "})\"/>";
            //
            var $input = $(inputText).appendTo(container);
            //
            if (options.onpicked && $.isFunction(options.onpicked)) {
                //
                $input.off("onpicked").on("onpicked", function () {
                    //
                    options.onpicked($(this).val());
                });
            }
            //
            $input.validatebox({ required: required });
            //返回
            return $input;

        },
        getValue: function (target) {
            //
            var value = $(target).val();
            //
            var date = value ? mDate.parse(value + "-01") : "";
            //
            if (date && date.getFullYear() == 1900) {
                //
                return "";
            }
            //
            return date;
        },
        setValue: function (target, value) {
            //
            var date = mDate.parse(value) || "";
            //
            if (date && date.getFullYear() == 1900) {
                //
                date = "";
            }
            else if (date) {
                //
                date = date.format("yyyy-MM")
            }
            //
            $(target).val(date);
        },
        resize: function (target, width) {
            $(target).width(width);
        }
    }
});