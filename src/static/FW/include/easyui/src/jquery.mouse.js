(function ($) {
	var _11 = null;
	var _12 = null;
	var _13 = false;
	function _14(e) {
		if (e.touches.length != 1) {
			return;
		}
		if (!_13) {
			_13 = true;
			dblClickTimer = setTimeout(function () {
				_13 = false;
			},
			500);
		} else {
			clearTimeout(dblClickTimer);
			_13 = false;
			_15(e, "dblclick");
		}
		_11 = setTimeout(function () {
			_15(e, "contextmenu", 3);
		},
		1000);
		_15(e, "mousedown");
		if ($.fn.draggable.isDragging || $.fn.resizable.isResizing) {
			e.preventDefault();
		}
	};
	function _16(e) {
		if (e.touches.length != 1) {
			return;
		}
		if (_11) {
			clearTimeout(_11);
		}
		_15(e, "mousemove");
		if ($.fn.draggable.isDragging || $.fn.resizable.isResizing) {
			e.preventDefault();
		}
	};
	function _17(e) {
		if (_11) {
			clearTimeout(_11);
		}
		_15(e, "mouseup");
		if ($.fn.draggable.isDragging || $.fn.resizable.isResizing) {
			e.preventDefault();
		}
	};
	function _15(e, _18, _19) {
		var _1a = new $.Event(_18);
		_1a.pageX = e.changedTouches[0].pageX;
		_1a.pageY = e.changedTouches[0].pageY;
		_1a.which = _19 || 1;
		$(e.target).trigger(_1a);
	};
	if (document.addEventListener) {
		document.addEventListener("touchstart", _14, true);
		document.addEventListener("touchmove", _16, true);
		document.addEventListener("touchend", _17, true);
	}
})(jQuery);