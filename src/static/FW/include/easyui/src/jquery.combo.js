(function ($) {
	function _83e(_83f, _840) {
		var _841 = $.data(_83f, "combo");
		var opts = _841.options;
		var _842 = _841.combo;
		var _843 = _841.panel;
		if (_840) {
			opts.width = _840;
		}
		if (isNaN(opts.width)) {
			var c = $(_83f).clone();
			c.css("visibility", "hidden");
			c.appendTo("body");
			opts.width = c.outerWidth();
			c.remove();
		}
		_842.appendTo("body");
		var _844 = _842.find("input.combo-text");
		var _845 = _842.find(".combo-arrow");
		var _846 = opts.hasDownArrow ? _845._outerWidth() : 0;
		_842._outerWidth(opts.width)._outerHeight(opts.height);
		_844._outerWidth(_842.width() - _846);
		_844.css({
			height: _842.height() + "px",
			lineHeight: _842.height() + "px"
		});
		_845._outerHeight(_842.height());
		_843.panel("resize", {
			width: (opts.panelWidth ? opts.panelWidth : _842.outerWidth()),
			height: opts.panelHeight
		});
		_842.insertAfter(_83f);
	};
	function init(_847) {
		$(_847).addClass("combo-f").hide();
		var span = $("<span class=\"combo\">" + "<input type=\"text\" class=\"combo-text\" autocomplete=\"off\">" + "<span><span class=\"combo-arrow\"></span></span>" + "<input type=\"hidden\" class=\"combo-value\">" + "</span>").insertAfter(_847);
		var _848 = $("<div class=\"combo-panel\"></div>").appendTo("body");
		_848.panel({
			doSize: false,
			closed: true,
			cls: "combo-p",
			style: {
				position: "absolute",
				zIndex: 10
			},
			onOpen: function () {
				var p = $(this).panel("panel");
				if ($.fn.menu) {
					p.css("z-index", $.fn.menu.defaults.zIndex++);
				} else { if ($.fn.window) {
						p.css("z-index", $.fn.window.defaults.zIndex++);
					}
				}
				$(this).panel("resize");
			},
			onBeforeClose: function () {
				_854(this);
			},
			onClose: function () {
				var _849 = $.data(_847, "combo");
				if (_849) {
					_849.options.onHidePanel.call(_847);
				}
			}
		});
		var name = $(_847).attr("name");
		if (name) {
			span.find("input.combo-value").attr("name", name);
			$(_847).removeAttr("name").attr("comboName", name);
		}
		return {
			combo: span,
			panel: _848
		};
	};
	function _84a(_84b) {
		var _84c = $.data(_84b, "combo");
		var opts = _84c.options;
		var _84d = _84c.combo;
		if (opts.hasDownArrow) {
			_84d.find(".combo-arrow").show();
		} else {
			_84d.find(".combo-arrow").hide();
		}
		_84e(_84b, opts.disabled);
		_84f(_84b, opts.readonly);
	};
	function _850(_851) {
		var _852 = $.data(_851, "combo");
		var _853 = _852.combo.find("input.combo-text");
		_853.validatebox("destroy");
		_852.panel.panel("destroy");
		_852.combo.remove();
		$(_851).remove();
	};
	function _854(_855) {
		$(_855).find(".combo-f").each(function () {
			var p = $(this).combo("panel");
			if (p.is(":visible")) {
				p.panel("close");
			}
		});
	};
	function _856(_857) {
		var _858 = $.data(_857, "combo");
		var opts = _858.options;
		var _859 = _858.panel;
		var _85a = _858.combo;
		var _85b = _85a.find(".combo-text");
		var _85c = _85a.find(".combo-arrow");
		$(document).unbind(".combo").bind("mousedown.combo", function (e) {
			var p = $(e.target).closest("span.combo,div.combo-p");
			if (p.length) {
				_854(p);
				return;
			}
			$("body>div.combo-p>div.combo-panel:visible").panel("close");
		});
		_85b.unbind(".combo");
		_85c.unbind(".combo");
		if (!opts.disabled && !opts.readonly) {
			_85b.bind("click.combo", function (e) {
				if (!opts.editable) {
					_85d.call(this);
				} else {
					var p = $(this).closest("div.combo-panel");
					$("div.combo-panel:visible").not(_859).not(p).panel("close");
				}
			}).bind("keydown.combo paste.combo drop.combo", function (e) {
				switch (e.keyCode) {
				case 38:
					opts.keyHandler.up.call(_857, e);
					break;
				case 40:
					opts.keyHandler.down.call(_857, e);
					break;
				case 37:
					opts.keyHandler.left.call(_857, e);
					break;
				case 39:
					opts.keyHandler.right.call(_857, e);
					break;
				case 13:
					e.preventDefault();
					opts.keyHandler.enter.call(_857, e);
					return false;
				case 9:
				case 27:
					_85e(_857);
					break;
				default:
					if (opts.editable) {
						if (_858.timer) {
							clearTimeout(_858.timer);
						}
						_858.timer = setTimeout(function () {
							var q = _85b.val();
							if (_858.previousValue != q) {
								_858.previousValue = q;
								$(_857).combo("showPanel");
								opts.keyHandler.query.call(_857, _85b.val(), e);
								$(_857).combo("validate");
							}
						},
						opts.delay);
					}
				}
			});
			_85c.bind("click.combo", function () {
				_85d.call(this);
			}).bind("mouseenter.combo", function () {
				$(this).addClass("combo-arrow-hover");
			}).bind("mouseleave.combo", function () {
				$(this).removeClass("combo-arrow-hover");
			});
		}
		function _85d() {
			if (_859.is(":visible")) {
				_85e(_857);
			} else {
				var p = $(this).closest("div.combo-panel");
				$("div.combo-panel:visible").not(_859).not(p).panel("close");
				$(_857).combo("showPanel");
			}
			_85b.focus();
		};
	};
	function _85f(_860) {
		var _861 = $.data(_860, "combo");
		var opts = _861.options;
		var _862 = _861.combo;
		var _863 = _861.panel;
		_863.panel("move", {
			left: _864(),
			top: _865()
		});
		if (_863.panel("options").closed) {
			_863.panel("open");
			opts.onShowPanel.call(_860);
		} (function () {
			if (_863.is(":visible")) {
				_863.panel("move", {
					left: _864(),
					top: _865()
				});
				setTimeout(arguments.callee, 200);
			}
		})();
		function _864() {
			var left = _862.offset().left;
			if (opts.panelAlign == "right") {
				left += _862._outerWidth() - _863._outerWidth();
			}
			if (left + _863._outerWidth() > $(window)._outerWidth() + $(document).scrollLeft()) {
				left = $(window)._outerWidth() + $(document).scrollLeft() - _863._outerWidth();
			}
			if (left < 0) {
				left = 0;
			}
			return left;
		};
		function _865() {
			var top = _862.offset().top + _862._outerHeight();
			if (top + _863._outerHeight() > $(window)._outerHeight() + $(document).scrollTop()) {
				top = _862.offset().top - _863._outerHeight();
			}
			if (top < $(document).scrollTop()) {
				top = _862.offset().top + _862._outerHeight();
			}
			return top;
		};
	};
	function _85e(_866) {
		var _867 = $.data(_866, "combo").panel;
		_867.panel("close");
	};
	function _868(_869) {
		var opts = $.data(_869, "combo").options;
		var _86a = $(_869).combo("textbox");
		_86a.validatebox($.extend({},
		opts, {
			deltaX: (opts.hasDownArrow ? opts.deltaX : (opts.deltaX > 0 ? 1 : -1))
		}));
	};
	function _84e(_86b, _86c) {
		var _86d = $.data(_86b, "combo");
		var opts = _86d.options;
		var _86e = _86d.combo;
		if (_86c) {
			opts.disabled = true;
			$(_86b).attr("disabled", true);
			_86e.find(".combo-value").attr("disabled", true);
			_86e.find(".combo-text").attr("disabled", true);
		} else {
			opts.disabled = false;
			$(_86b).removeAttr("disabled");
			_86e.find(".combo-value").removeAttr("disabled");
			_86e.find(".combo-text").removeAttr("disabled");
		}
	};
	function _84f(_86f, mode) {
		var _870 = $.data(_86f, "combo");
		var opts = _870.options;
		opts.readonly = mode == undefined ? true : mode;
		var _871 = opts.readonly ? true : (!opts.editable);
		_870.combo.find(".combo-text").attr("readonly", _871).css("cursor", _871 ? "pointer" : "");
	};
	function _872(_873) {
		var _874 = $.data(_873, "combo");
		var opts = _874.options;
		var _875 = _874.combo;
		if (opts.multiple) {
			_875.find("input.combo-value").remove();
		} else {
			_875.find("input.combo-value").val("");
		}
		_875.find("input.combo-text").val("");
	};
	function _876(_877) {
		var _878 = $.data(_877, "combo").combo;
		return _878.find("input.combo-text").val();
	};
	function _879(_87a, text) {
		var _87b = $.data(_87a, "combo");
		var _87c = _87b.combo.find("input.combo-text");
		if (_87c.val() != text) {
			_87c.val(text);
			$(_87a).combo("validate");
			_87b.previousValue = text;
		}
	};
	function _87d(_87e) {
		var _87f = [];
		var _880 = $.data(_87e, "combo").combo;
		_880.find("input.combo-value").each(function () {
			_87f.push($(this).val());
		});
		return _87f;
	};
	function _881(_882, _883) {
		var opts = $.data(_882, "combo").options;
		var _884 = _87d(_882);
		var _885 = $.data(_882, "combo").combo;
		_885.find("input.combo-value").remove();
		var name = $(_882).attr("comboName");
		for (var i = 0; i < _883.length; i++) {
			var _886 = $("<input type=\"hidden\" class=\"combo-value\">").appendTo(_885);
			if (name) {
				_886.attr("name", name);
			}
			_886.val(_883[i]);
		}
		var tmp = [];
		for (var i = 0; i < _884.length; i++) {
			tmp[i] = _884[i];
		}
		var aa = [];
		for (var i = 0; i < _883.length; i++) {
			for (var j = 0; j < tmp.length; j++) {
				if (_883[i] == tmp[j]) {
					aa.push(_883[i]);
					tmp.splice(j, 1);
					break;
				}
			}
		}
		if (aa.length != _883.length || _883.length != _884.length) {
			if (opts.multiple) {
				opts.onChange.call(_882, _883, _884);
			} else {
				opts.onChange.call(_882, _883[0], _884[0]);
			}
		}
	};
	function _887(_888) {
		var _889 = _87d(_888);
		return _889[0];
	};
	function _88a(_88b, _88c) {
		_881(_88b, [_88c]);
	};
	function _88d(_88e) {
		var opts = $.data(_88e, "combo").options;
		var fn = opts.onChange;
		opts.onChange = function () {};
		if (opts.multiple) {
			if (opts.value) {
				if (typeof opts.value == "object") {
					_881(_88e, opts.value);
				} else {
					_88a(_88e, opts.value);
				}
			} else {
				_881(_88e, []);
			}
			opts.originalValue = _87d(_88e);
		} else {
			_88a(_88e, opts.value);
			opts.originalValue = opts.value;
		}
		opts.onChange = fn;
	};
	$.fn.combo = function (_88f, _890) {
		if (typeof _88f == "string") {
			var _891 = $.fn.combo.methods[_88f];
			if (_891) {
				return _891(this, _890);
			} else {
				return this.each(function () {
					var _892 = $(this).combo("textbox");
					_892.validatebox(_88f, _890);
				});
			}
		}
		_88f = _88f || {};
		return this.each(function () {
			var _893 = $.data(this, "combo");
			if (_893) {
				$.extend(_893.options, _88f);
			} else {
				var r = init(this);
				_893 = $.data(this, "combo", {
					options: $.extend({},
					$.fn.combo.defaults, $.fn.combo.parseOptions(this), _88f),
					combo: r.combo,
					panel: r.panel,
					previousValue: null
				});
				$(this).removeAttr("disabled");
			}
			_84a(this);
			_83e(this);
			_856(this);
			_868(this);
			_88d(this);
		});
	};
	$.fn.combo.methods = {
		options: function (jq) {
			return $.data(jq[0], "combo").options;
		},
		panel: function (jq) {
			return $.data(jq[0], "combo").panel;
		},
		textbox: function (jq) {
			return $.data(jq[0], "combo").combo.find("input.combo-text");
		},
		destroy: function (jq) {
			return jq.each(function () {
				_850(this);
			});
		},
		resize: function (jq, _894) {
			return jq.each(function () {
				_83e(this, _894);
			});
		},
		showPanel: function (jq) {
			return jq.each(function () {
				_85f(this);
			});
		},
		hidePanel: function (jq) {
			return jq.each(function () {
				_85e(this);
			});
		},
		disable: function (jq) {
			return jq.each(function () {
				_84e(this, true);
				_856(this);
			});
		},
		enable: function (jq) {
			return jq.each(function () {
				_84e(this, false);
				_856(this);
			});
		},
		readonly: function (jq, mode) {
			return jq.each(function () {
				_84f(this, mode);
				_856(this);
			});
		},
		isValid: function (jq) {
			var _895 = $.data(jq[0], "combo").combo.find("input.combo-text");
			return _895.validatebox("isValid");
		},
		clear: function (jq) {
			return jq.each(function () {
				_872(this);
			});
		},
		reset: function (jq) {
			return jq.each(function () {
				var opts = $.data(this, "combo").options;
				if (opts.multiple) {
					$(this).combo("setValues", opts.originalValue);
				} else {
					$(this).combo("setValue", opts.originalValue);
				}
			});
		},
		getText: function (jq) {
			return _876(jq[0]);
		},
		setText: function (jq, text) {
			return jq.each(function () {
				_879(this, text);
			});
		},
		getValues: function (jq) {
			return _87d(jq[0]);
		},
		setValues: function (jq, _896) {
			return jq.each(function () {
				_881(this, _896);
			});
		},
		getValue: function (jq) {
			return _887(jq[0]);
		},
		setValue: function (jq, _897) {
			return jq.each(function () {
				_88a(this, _897);
			});
		}
	};
	$.fn.combo.parseOptions = function (_898) {
		var t = $(_898);
		return $.extend({},
		$.fn.validatebox.parseOptions(_898), $.parser.parseOptions(_898, ["width", "height", "separator", "panelAlign", {
			panelWidth: "number",
			editable: "boolean",
			hasDownArrow: "boolean",
			delay: "number",
			selectOnNavigation: "boolean"
		}]), {
			panelHeight: (t.attr("panelHeight") == "auto" ? "auto" : parseInt(t.attr("panelHeight")) || undefined),
			multiple: (t.attr("multiple") ? true : undefined),
			disabled: (t.attr("disabled") ? true : undefined),
			readonly: (t.attr("readonly") ? true : undefined),
			value: (t.val() || undefined)
		});
	};
	$.fn.combo.defaults = $.extend({},
	$.fn.validatebox.defaults, {
		width: "auto",
		height: 22,
		panelWidth: null,
		panelHeight: 200,
		panelAlign: "left",
		multiple: false,
		selectOnNavigation: true,
		separator: ",",
		editable: true,
		disabled: false,
		readonly: false,
		hasDownArrow: true,
		value: "",
		delay: 200,
		deltaX: 19,
		keyHandler: {
			up: function (e) {},
			down: function (e) {},
			left: function (e) {},
			right: function (e) {},
			enter: function (e) {},
			query: function (q, e) {}
		},
		onShowPanel: function () {},
		onHidePanel: function () {},
		onChange: function (_899, _89a) {}
	});
})(jQuery);