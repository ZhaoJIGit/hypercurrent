/*
	颜色处理帮助类
*/
(function () {
	var mColor = (function () {
		var mColor = function () {

		}
		return mColor;
	})();
	//扩展静态方法
	$.extend(mColor, {
		//#xxxxxx 转化为 数字
		Color2RGB: function (color) {
			var r = parseInt(color.substr(1, 2), 16);
			var g = parseInt(color.substr(3, 2), 16);
			var b = parseInt(color.substr(5, 2), 16); return new Array(r, g, b);
		},
		//数字转化为#xxxxxx
		RGB2Color: function (rgb) {
			//颜色标识
			var s = "#";
			//RGB
			for (var i = 0; i < 3; i++) {
				//十六进制
				var c = Math.round(rgb[i]).toString(16);
				//拼接
				s += (c.length == 1) ? ('0' + c ) : c;
			}
			return s.toUpperCase();
		},
		//渐变
		Gradient: function (from, to, step) {
			var result = [];
			var from = mColor.Color2RGB(from);
			var to = mColor.Color2RGB(to);
			for (var i = 0; i <= step; i++) {
				//渐变色
				var gradient = [];
				// RGB通道分别进行计算   
				for (var j = 0; j < 3; j++) {
					//
					gradient[j] = from[j] + (to[j] - from[j]) / step * i;
				}
				result.push(mColor.RGB2Color(gradient));
			}
			return result;
		}
	});
	//扩展到window
	window.mColor = $.mColor = mColor;
})()