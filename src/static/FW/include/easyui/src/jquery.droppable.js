(function ($) {
	function _49(_4a) {
		$(_4a).addClass("droppable");
		$(_4a).bind("_dragenter", function (e, _4b) {
			$.data(_4a, "droppable").options.onDragEnter.apply(_4a, [e, _4b]);
		});
		$(_4a).bind("_dragleave", function (e, _4c) {
			$.data(_4a, "droppable").options.onDragLeave.apply(_4a, [e, _4c]);
		});
		$(_4a).bind("_dragover", function (e, _4d) {
			$.data(_4a, "droppable").options.onDragOver.apply(_4a, [e, _4d]);
		});
		$(_4a).bind("_drop", function (e, _4e) {
			$.data(_4a, "droppable").options.onDrop.apply(_4a, [e, _4e]);
		});
	};
	$.fn.droppable = function (_4f, _50) {
		if (typeof _4f == "string") {
			return $.fn.droppable.methods[_4f](this, _50);
		}
		_4f = _4f || {};
		return this.each(function () {
			var _51 = $.data(this, "droppable");
			if (_51) {
				$.extend(_51.options, _4f);
			} else {
				_49(this);
				$.data(this, "droppable", {
					options: $.extend({},
					$.fn.droppable.defaults, $.fn.droppable.parseOptions(this), _4f)
				});
			}
		});
	};
	$.fn.droppable.methods = {
		options: function (jq) {
			return $.data(jq[0], "droppable").options;
		},
		enable: function (jq) {
			return jq.each(function () {
				$(this).droppable({
					disabled: false
				});
			});
		},
		disable: function (jq) {
			return jq.each(function () {
				$(this).droppable({
					disabled: true
				});
			});
		}
	};
	$.fn.droppable.parseOptions = function (_52) {
		var t = $(_52);
		return $.extend({},
		$.parser.parseOptions(_52, ["accept"]), {
			disabled: (t.attr("disabled") ? true : undefined)
		});
	};
	$.fn.droppable.defaults = {
		accept: null,
		disabled: false,
		onDragEnter: function (e, _53) {},
		onDragOver: function (e, _54) {},
		onDragLeave: function (e, _55) {},
		onDrop: function (e, _56) {}
	};
})(jQuery);