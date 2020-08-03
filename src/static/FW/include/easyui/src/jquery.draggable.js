(function ($) {
	function _1b(e) {
		var _1c = $.data(e.data.target, "draggable");
		var _1d = _1c.options;
		var _1e = _1c.proxy;
		var _1f = e.data;
		var _20 = _1f.startLeft + e.pageX - _1f.startX;
		var top = _1f.startTop + e.pageY - _1f.startY;
		if (_1e) {
			if (_1e.parent()[0] == document.body) {
				if (_1d.deltaX != null && _1d.deltaX != undefined) {
					_20 = e.pageX + _1d.deltaX;
				} else {
					_20 = e.pageX - e.data.offsetWidth;
				}
				if (_1d.deltaY != null && _1d.deltaY != undefined) {
					top = e.pageY + _1d.deltaY;
				} else {
					top = e.pageY - e.data.offsetHeight;
				}
			} else { if (_1d.deltaX != null && _1d.deltaX != undefined) {
					_20 += e.data.offsetWidth + _1d.deltaX;
				}
				if (_1d.deltaY != null && _1d.deltaY != undefined) {
					top += e.data.offsetHeight + _1d.deltaY;
				}
			}
		}
		if (e.data.parent != document.body) {
			_20 += $(e.data.parent).scrollLeft();
			top += $(e.data.parent).scrollTop();
		}
		if (_1d.axis == "h") {
			_1f.left = _20;
		} else { if (_1d.axis == "v") {
				_1f.top = top;
			} else {
				_1f.left = _20;
				_1f.top = top;
			}
		}
	};
	function _21(e) {
		var _22 = $.data(e.data.target, "draggable");
		var _23 = _22.options;
		var _24 = _22.proxy;
		if (!_24) {
			_24 = $(e.data.target);
		}
		_24.css({
			left: e.data.left,
			top: e.data.top
		});
		$("body").css("cursor", _23.cursor);
	};
	function _25(e) {
		$.fn.draggable.isDragging = true;
		var _26 = $.data(e.data.target, "draggable");
		var _27 = _26.options;
		var _28 = $(".droppable").filter(function () {
			return e.data.target != this;
		}).filter(function () {
			var _29 = $.data(this, "droppable").options.accept;
			if (_29) {
				return $(_29).filter(function () {
					return this == e.data.target;
				}).length > 0;
			} else {
				return true;
			}
		});
		_26.droppables = _28;
		var _2a = _26.proxy;
		if (!_2a) {
			if (_27.proxy) {
				if (_27.proxy == "clone") {
					_2a = $(e.data.target).clone().insertAfter(e.data.target);
				} else {
					_2a = _27.proxy.call(e.data.target, e.data.target);
				}
				_26.proxy = _2a;
			} else {
				_2a = $(e.data.target);
			}
		}
		_2a.css("position", "absolute");
		_1b(e);
		_21(e);
		_27.onStartDrag.call(e.data.target, e);
		return false;
	};
	function _2b(e) {
		var _2c = $.data(e.data.target, "draggable");
		_1b(e);
		if (_2c.options.onDrag.call(e.data.target, e) != false) {
			_21(e);
		}
		var _2d = e.data.target;
		_2c.droppables.each(function () {
			var _2e = $(this);
			if (_2e.droppable("options").disabled) {
				return;
			}
			var p2 = _2e.offset();
			if (e.pageX > p2.left && e.pageX < p2.left + _2e.outerWidth() && e.pageY > p2.top && e.pageY < p2.top + _2e.outerHeight()) {
				if (!this.entered) {
					$(this).trigger("_dragenter", [_2d]);
					this.entered = true;
				}
				$(this).trigger("_dragover", [_2d]);
			} else { if (this.entered) {
					$(this).trigger("_dragleave", [_2d]);
					this.entered = false;
				}
			}
		});
		return false;
	};
	function _2f(e) {
		$.fn.draggable.isDragging = false;
		_2b(e);
		var _30 = $.data(e.data.target, "draggable");
		var _31 = _30.proxy;
		var _32 = _30.options;
		if (_32.revert) {
			if (_33() == true) {
				$(e.data.target).css({
					position: e.data.startPosition,
					left: e.data.startLeft,
					top: e.data.startTop
				});
			} else { if (_31) {
					var _34, top;
					if (_31.parent()[0] == document.body) {
						_34 = e.data.startX - e.data.offsetWidth;
						top = e.data.startY - e.data.offsetHeight;
					} else {
						_34 = e.data.startLeft;
						top = e.data.startTop;
					}
					_31.animate({
						left: _34,
						top: top
					},
					function () {
						_35();
					});
				} else {
					$(e.data.target).animate({
						left: e.data.startLeft,
						top: e.data.startTop
					},
					function () {
						$(e.data.target).css("position", e.data.startPosition);
					});
				}
			}
		} else {
			$(e.data.target).css({
				position: "absolute",
				left: e.data.left,
				top: e.data.top
			});
			_33();
		}
		_32.onStopDrag.call(e.data.target, e);
		$(document).unbind(".draggable");
		setTimeout(function () {
			$("body").css("cursor", "");
		},
		100);
		function _35() {
			if (_31) {
				_31.remove();
			}
			_30.proxy = null;
		};
		function _33() {
			var _36 = false;
			_30.droppables.each(function () {
				var _37 = $(this);
				if (_37.droppable("options").disabled) {
					return;
				}
				var p2 = _37.offset();
				if (e.pageX > p2.left && e.pageX < p2.left + _37.outerWidth() && e.pageY > p2.top && e.pageY < p2.top + _37.outerHeight()) {
					if (_32.revert) {
						$(e.data.target).css({
							position: e.data.startPosition,
							left: e.data.startLeft,
							top: e.data.startTop
						});
					}
					$(this).trigger("_drop", [e.data.target]);
					_35();
					_36 = true;
					this.entered = false;
					return false;
				}
			});
			if (!_36 && !_32.revert) {
				_35();
			}
			return _36;
		};
		return false;
	};
	$.fn.draggable = function (_38, _39) {
		if (typeof _38 == "string") {
			return $.fn.draggable.methods[_38](this, _39);
		}
		return this.each(function () {
			var _3a;
			var _3b = $.data(this, "draggable");
			if (_3b) {
				_3b.handle.unbind(".draggable");
				_3a = $.extend(_3b.options, _38);
			} else {
				_3a = $.extend({},
				$.fn.draggable.defaults, $.fn.draggable.parseOptions(this), _38 || {});
			}
			var _3c = _3a.handle ? (typeof _3a.handle == "string" ? $(_3a.handle, this) : _3a.handle) : $(this);
			$.data(this, "draggable", {
				options: _3a,
				handle: _3c
			});
			if (_3a.disabled) {
				$(this).css("cursor", "");
				return;
			}
			_3c.unbind(".draggable").bind("mousemove.draggable", {
				target: this
			},
			function (e) {
				if ($.fn.draggable.isDragging) {
					return;
				}
				var _3d = $.data(e.data.target, "draggable").options;
				if (_3e(e)) {
					$(this).css("cursor", _3d.cursor);
				} else {
					$(this).css("cursor", "");
				}
			}).bind("mouseleave.draggable", {
				target: this
			},
			function (e) {
				$(this).css("cursor", "");
			}).bind("mousedown.draggable", {
				target: this
			},
			function (e) {
				if (_3e(e) == false) {
					return;
				}
				$(this).css("cursor", "");
				var _3f = $(e.data.target).position();
				var _40 = $(e.data.target).offset();
				var _41 = {
					startPosition: $(e.data.target).css("position"),
					startLeft: _3f.left,
					startTop: _3f.top,
					left: _3f.left,
					top: _3f.top,
					startX: e.pageX,
					startY: e.pageY,
					offsetWidth: (e.pageX - _40.left),
					offsetHeight: (e.pageY - _40.top),
					target: e.data.target,
					parent: $(e.data.target).parent()[0]
				};
				$.extend(e.data, _41);
				var _42 = $.data(e.data.target, "draggable").options;
				if (_42.onBeforeDrag.call(e.data.target, e) == false) {
					return;
				}
				$(document).bind("mousedown.draggable", e.data, _25);
				$(document).bind("mousemove.draggable", e.data, _2b);
				$(document).bind("mouseup.draggable", e.data, _2f);
			});
			function _3e(e) {
				var _43 = $.data(e.data.target, "draggable");
				var _44 = _43.handle;
				var _45 = $(_44).offset();
				var _46 = $(_44).outerWidth();
				var _47 = $(_44).outerHeight();
				var t = e.pageY - _45.top;
				var r = _45.left + _46 - e.pageX;
				var b = _45.top + _47 - e.pageY;
				var l = e.pageX - _45.left;
				return Math.min(t, r, b, l) > _43.options.edge;
			};
		});
	};
	$.fn.draggable.methods = {
		options: function (jq) {
			return $.data(jq[0], "draggable").options;
		},
		proxy: function (jq) {
			return $.data(jq[0], "draggable").proxy;
		},
		enable: function (jq) {
			return jq.each(function () {
				$(this).draggable({
					disabled: false
				});
			});
		},
		disable: function (jq) {
			return jq.each(function () {
				$(this).draggable({
					disabled: true
				});
			});
		}
	};
	$.fn.draggable.parseOptions = function (_48) {
		var t = $(_48);
		return $.extend({},
		$.parser.parseOptions(_48, ["cursor", "handle", "axis", {
			"revert": "boolean",
			"deltaX": "number",
			"deltaY": "number",
			"edge": "number"
		}]), {
			disabled: (t.attr("disabled") ? true : undefined)
		});
	};
	$.fn.draggable.defaults = {
		proxy: null,
		revert: false,
		cursor: "move",
		deltaX: null,
		deltaY: null,
		handle: null,
		disabled: false,
		edge: 0,
		axis: null,
		onBeforeDrag: function (e) {},
		onStartDrag: function (e) {},
		onDrag: function (e) {},
		onStopDrag: function (e) {}
	};
	$.fn.draggable.isDragging = false;
})(jQuery);