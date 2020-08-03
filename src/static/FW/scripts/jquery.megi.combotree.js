(function ($) {
    $.fn.megicombotree = function (options , param) {
      
        //执行方法
        if (typeof options == "string") {
            return $(this).combotree(options, param);
        }

        $.ajax({
            type: options.type?options.type:"POST",
            url: options.url,
            data: options.data,
            success: function (msg) {
                var data = initData(msg,options);
              
                $(options.selector).combotree("loadData", data);
            },
            error: function () {
               
            }
        });
    }
    function initData(data , options) {
        if (!data) {
            return;
        }
        var json = "[";
        //主键容器：用于循环了主键
        var keyContainer = new Array();
        for (var i = 0; i < data.length; i++) {
            
            var row = data[i];
            if(valueIsExist(keyContainer, row[options.valueField])){
                continue;
            }
            keyContainer.push(row[options.valueField]);
            json += "{";
           
            
            for (var key in row) {
                //遍历列，寻找id值
                if (key == options.valueField) {
                    json += '"id":' + '"' + row[options.valueField] + '",';
                }

                //寻找text值
                if (key == options.textField) {
                    json += '"text":' + '"' + row[options.textField] + '",';
                }
            }
            //寻找子项
            var childJson = '"children":[';
            var childRowJson = "";
            for (var j = 0; j < data.length; j++) {
                var childRow = data[j];
                if (valueIsExist(keyContainer, childRow[options.valueField])) {
                    continue;
                }
                
                if (childRow[options.parentField] == row[options.valueField]) {
                    childRowJson += '{"id":"' + childRow[options.valueField] + '","text":"' + childRow[options.textField] + '"},';
                    keyContainer.push(childRow[options.valueField]);
                }
            }
            if (childRowJson) {
                childRowJson = childRowJson.substring(0, childRowJson.length - 1);
                childJson += childRowJson;
            }
            childJson += ']';

            json += childJson;

            json += "},";
        }
        json = json.substring(0, json.length - 1);
        json += "]";
        //将json字符串转换成json格式数据
        json = eval(json);

        return json;
    }
    //判断数组是否在容器中
    function valueIsExist(container, value) {
        for (var i = 0; i < container.length;i++) {
            if (container[i] == value) {
                return true;
            }
        }

        return false;
    }
})($)