///只实现了两层的父子关系

(function ($) {
	$.fn.megitreegrid = function (options, param) {
	    var selector = $(this);
		//执行方法
		if (typeof options == "string") {
			return $(this).treegrid(options, param);
		}
		$(selector).treegrid({
		    width:options.width,
		    fitColumns:options.fitColumns,
		    resizable: options.resizable,
            auto:options.auto,
		    idField: options.idField,
		    treeField:options.treeField,
		    columns: options.columns,
		    onClickRow: options.onClickRow,
		    onClickCell:options.onClickCell,
		    onAfterEdit:options.onAfterEdit,
		    onLoadSuccess: options.onLoadSuccess
		});

		$.ajax({
			type: options.type ? options.type : "POST",
			url: options.url,
			data: options.data,
			success: function (msg) {
				var data = initData(msg.Data, options);
				$(selector).treegrid("loadData", data);

				$(selector).treegrid("collapseAll");
			},
			error: function () {

			}
		});
	}
	function initData(data, options) {
		if (!data) {
			return;
		}
		var json = "[";
		//主键容器：用于循环了主键
		var keyContainer = new Array();
		for (var i = 0; i < data.length; i++) {
		    var row = data[i];
		    if (!isParentNode(row, data, options)) {
		        continue;
		    }
			if (valueIsExist(keyContainer, row[options.valueField])) {
				continue;
			}
			keyContainer.push(row[options.valueField]);
			json += "{";
			for (var key in row) {
				//遍历列，寻找id值
				if (key == options.valueField) {
				    json += '"id":' + '"' + row[options.valueField] + '",';
				    continue;
				}

				if (key == options.textField) {
				    //寻找text值
				    json += '"' + options.textField + '":' + '"' + mText.encode(row[options.textField]) + '",';
				    continue;
				}

				json += '"' + key + '":"' + row[key] + '",';
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
				    childRowJson += '{';

				    for (var key in childRow) {
				        //遍历列，寻找id值
				        if (key == options.valueField) {
				            childRowJson += '"id":' + '"' + childRow[options.valueField] + '",';
				            continue;
				        }

				        if (key == options.textField) {
				            //寻找text值
				            childRowJson += '"' + options.textField + '":' + '"' + mText.encode(childRow[options.textField]) + '",';
				            continue;
				        }

				        childRowJson += '"' + key + '":"' + childRow[key] + '",';
				    }
				    childRowJson += "},";

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
		if (json.length != 1) {
		    json = json.substring(0, json.length - 1);
		}

		json += "]";
		//将json字符串转换成json格式数据
		json = eval(json);

		return json;
	}
	//判断数组是否在容器中
	function valueIsExist(container, value) {
		for (var i = 0; i < container.length; i++) {
			if (container[i] == value) {
				return true;
			}
		}

		return false;
	}

    //查找该节点是否有父节点
	function isParentNode(node, nodes,options) {
	    for (var i = 0 ; i < nodes.length ; i++) {
	        var temp = nodes[i];
	        if (node[options.parentField]==0 || !node[options.parentField] || node[options.valueField] == temp[options.parentField]) {
	            return true;
	        }
	    }

	    return false;
	}
})(jQuery)