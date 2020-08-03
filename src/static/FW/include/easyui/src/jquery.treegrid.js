(function ($) {
	function _771(_772) {
		var _773 = $.data(_772, "treegrid");
		var opts = _773.options;
		$(_772).datagrid($.extend({},
		opts, {
			url: null,
			data: null,
			loader: function () {
				return false;
			},
			onBeforeLoad: function () {
				return false;
			},
			onLoadSuccess: function () {},
			onResizeColumn: function (_774, _775) {
				_78b(_772);
				opts.onResizeColumn.call(_772, _774, _775);
			},
			onSortColumn: function (sort, _776) {
				opts.sortName = sort;
				opts.sortOrder = _776;
				if (opts.remoteSort) {
					_78a(_772);
				} else {
					var data = $(_772).treegrid("getData");
					_7a0(_772, 0, data);
				}
				opts.onSortColumn.call(_772, sort, _776);
			},
			onBeforeEdit: function (_777, row) {
				if (opts.onBeforeEdit.call(_772, row) == false) {
					return false;
				}
			},
			onAfterEdit: function (_778, row, _779) {
				opts.onAfterEdit.call(_772, row, _779);
			},
			onCancelEdit: function (_77a, row) {
				opts.onCancelEdit.call(_772, row);
			},
			onSelect: function (_77b) {
				opts.onSelect.call(_772, find(_772, _77b));
			},
			onUnselect: function (_77c) {
				opts.onUnselect.call(_772, find(_772, _77c));
			},
			onCheck: function (_77d) {
				opts.onCheck.call(_772, find(_772, _77d));
			},
			onUncheck: function (_77e) {
				opts.onUncheck.call(_772, find(_772, _77e));
			},
			onClickRow: function (_77f) {
				opts.onClickRow.call(_772, find(_772, _77f));
			},
			onDblClickRow: function (_780) {
				opts.onDblClickRow.call(_772, find(_772, _780));
			},
			onClickCell: function (_781, _782) {
				opts.onClickCell.call(_772, _782, find(_772, _781));
			},
			onDblClickCell: function (_783, _784) {
				opts.onDblClickCell.call(_772, _784, find(_772, _783));
			},
			onRowContextMenu: function (e, _785) {
				opts.onContextMenu.call(_772, e, find(_772, _785));
			}
		}));
		if (!opts.columns) {
			var _786 = $.data(_772, "datagrid").options;
			opts.columns = _786.columns;
			opts.frozenColumns = _786.frozenColumns;
		}
		_773.dc = $.data(_772, "datagrid").dc;
		if (opts.pagination) {
			var _787 = $(_772).datagrid("getPager");
			_787.pagination({
				pageNumber: opts.pageNumber,
				pageSize: opts.pageSize,
				pageList: opts.pageList,
				onSelectPage: function (_788, _789) {
					opts.pageNumber = _788;
					opts.pageSize = _789;
					_78a(_772);
				}
			});
			opts.pageSize = _787.pagination("options").pageSize;
		}
	};
	function _78b(_78c, _78d) {
		var opts = $.data(_78c, "datagrid").options;
		var dc = $.data(_78c, "datagrid").dc;
		if (!dc.body1.is(":empty") && (!opts.nowrap || opts.autoRowHeight)) {
			if (_78d != undefined) {
				var _78e = _78f(_78c, _78d);
				for (var i = 0; i < _78e.length; i++) {
					_790(_78e[i][opts.idField]);
				}
			}
		}
		$(_78c).datagrid("fixRowHeight", _78d);
		function _790(_791) {
			var tr1 = opts.finder.getTr(_78c, _791, "body", 1);
			var tr2 = opts.finder.getTr(_78c, _791, "body", 2);
			tr1.css("height", "");
			tr2.css("height", "");
			var _792 = Math.max(tr1.height(), tr2.height());
			tr1.css("height", _792);
			tr2.css("height", _792);
		};
	};
	function _793(_794) {
		var dc = $.data(_794, "datagrid").dc;
		var opts = $.data(_794, "treegrid").options;
		if (!opts.rownumbers) {
			return;
		}
		dc.body1.find("div.datagrid-cell-rownumber").each(function (i) {
			$(this).html(i + 1);
		});
	};
	function _795(_796) {
		var dc = $.data(_796, "datagrid").dc;
		var body = dc.body1.add(dc.body2);
		var _797 = ($.data(body[0], "events") || $._data(body[0], "events")).click[0].handler;
		dc.body1.add(dc.body2).bind("mouseover", function (e) {
			var tt = $(e.target);
			var tr = tt.closest("tr.datagrid-row");
			if (!tr.length) {
				return;
			}
			if (tt.hasClass("tree-hit")) {
				tt.hasClass("tree-expanded") ? tt.addClass("tree-expanded-hover") : tt.addClass("tree-collapsed-hover");
			}
			e.stopPropagation();
		}).bind("mouseout", function (e) {
			var tt = $(e.target);
			var tr = tt.closest("tr.datagrid-row");
			if (!tr.length) {
				return;
			}
			if (tt.hasClass("tree-hit")) {
				tt.hasClass("tree-expanded") ? tt.removeClass("tree-expanded-hover") : tt.removeClass("tree-collapsed-hover");
			}
			e.stopPropagation();
		}).unbind("click").bind("click", function (e) {
			var tt = $(e.target);
			var tr = tt.closest("tr.datagrid-row");
			if (!tr.length) {
				return;
			}
			if (tt.hasClass("tree-hit")) {
				_798(_796, tr.attr("node-id"));
			} else {
				_797(e);
			}
			e.stopPropagation();
		});
	};
	function _799(_79a, _79b) {
		var opts = $.data(_79a, "treegrid").options;
		var tr1 = opts.finder.getTr(_79a, _79b, "body", 1);
		var tr2 = opts.finder.getTr(_79a, _79b, "body", 2);
		var _79c = $(_79a).datagrid("getColumnFields", true).length + (opts.rownumbers ? 1 : 0);
		var _79d = $(_79a).datagrid("getColumnFields", false).length;
		_79e(tr1, _79c);
		_79e(tr2, _79d);
		function _79e(tr, _79f) {
			$("<tr class=\"treegrid-tr-tree\">" + "<td style=\"border:0px\" colspan=\"" + _79f + "\">" + "<div></div>" + "</td>" + "</tr>").insertAfter(tr);
		};
	};
	function _7a0(_7a1, _7a2, data, _7a3) {
		var _7a4 = $.data(_7a1, "treegrid");
		var opts = _7a4.options;
		var dc = _7a4.dc;
		data = opts.loadFilter.call(_7a1, data, _7a2);
		var node = find(_7a1, _7a2);
		if (node) {
			var _7a5 = opts.finder.getTr(_7a1, _7a2, "body", 1);
			var _7a6 = opts.finder.getTr(_7a1, _7a2, "body", 2);
			var cc1 = _7a5.next("tr.treegrid-tr-tree").children("td").children("div");
			var cc2 = _7a6.next("tr.treegrid-tr-tree").children("td").children("div");
			if (!_7a3) {
				node.children = [];
			}
		} else {
			var cc1 = dc.body1;
			var cc2 = dc.body2;
			if (!_7a3) {
				_7a4.data = [];
			}
		}
		if (!_7a3) {
			cc1.empty();
			cc2.empty();
		}
		if (opts.view.onBeforeRender) {
			opts.view.onBeforeRender.call(opts.view, _7a1, _7a2, data);
		}
		opts.view.render.call(opts.view, _7a1, cc1, true);
		opts.view.render.call(opts.view, _7a1, cc2, false);
		if (opts.showFooter) {
			opts.view.renderFooter.call(opts.view, _7a1, dc.footer1, true);
			opts.view.renderFooter.call(opts.view, _7a1, dc.footer2, false);
		}
		if (opts.view.onAfterRender) {
			opts.view.onAfterRender.call(opts.view, _7a1);
		}
		opts.onLoadSuccess.call(_7a1, node, data);
		if (!_7a2 && opts.pagination) {
			var _7a7 = $.data(_7a1, "treegrid").total;
			var _7a8 = $(_7a1).datagrid("getPager");
			if (_7a8.pagination("options").total != _7a7) {
				_7a8.pagination({
					total: _7a7
				});
			}
		}
		_78b(_7a1);
		_793(_7a1);
		$(_7a1).treegrid("setSelectionState");
		$(_7a1).treegrid("autoSizeColumn");
	};
	function _78a(_7a9, _7aa, _7ab, _7ac, _7ad) {
		var opts = $.data(_7a9, "treegrid").options;
		var body = $(_7a9).datagrid("getPanel").find("div.datagrid-body");
		if (_7ab) {
			opts.queryParams = _7ab;
		}
		var _7ae = $.extend({},
		opts.queryParams);
		if (opts.pagination) {
			$.extend(_7ae, {
				page: opts.pageNumber,
				rows: opts.pageSize
			});
		}
		if (opts.sortName) {
			$.extend(_7ae, {
				sort: opts.sortName,
				order: opts.sortOrder
			});
		}
		var row = find(_7a9, _7aa);
		if (opts.onBeforeLoad.call(_7a9, row, _7ae) == false) {
			return;
		}
		var _7af = body.find("tr[node-id=\"" + _7aa + "\"] span.tree-folder");
		_7af.addClass("tree-loading");
		$(_7a9).treegrid("loading");
		var _7b0 = opts.loader.call(_7a9, _7ae, function (data) {
			_7af.removeClass("tree-loading");
			$(_7a9).treegrid("loaded");
			_7a0(_7a9, _7aa, data, _7ac);
			if (_7ad) {
				_7ad();
			}
		},
		function () {
			_7af.removeClass("tree-loading");
			$(_7a9).treegrid("loaded");
			opts.onLoadError.apply(_7a9, arguments);
			if (_7ad) {
				_7ad();
			}
		});
		if (_7b0 == false) {
			_7af.removeClass("tree-loading");
			$(_7a9).treegrid("loaded");
		}
	};
	function _7b1(_7b2) {
		var rows = _7b3(_7b2);
		if (rows.length) {
			return rows[0];
		} else {
			return null;
		}
	};
	function _7b3(_7b4) {
		return $.data(_7b4, "treegrid").data;
	};
	function _7b5(_7b6, _7b7) {
		var row = find(_7b6, _7b7);
		if (row._parentId) {
			return find(_7b6, row._parentId);
		} else {
			return null;
		}
	};
	function _78f(_7b8, _7b9) {
		var opts = $.data(_7b8, "treegrid").options;
		var body = $(_7b8).datagrid("getPanel").find("div.datagrid-view2 div.datagrid-body");
		var _7ba = [];
		if (_7b9) {
			_7bb(_7b9);
		} else {
			var _7bc = _7b3(_7b8);
			for (var i = 0; i < _7bc.length; i++) {
				_7ba.push(_7bc[i]);
				_7bb(_7bc[i][opts.idField]);
			}
		}
		function _7bb(_7bd) {
			var _7be = find(_7b8, _7bd);
			if (_7be && _7be.children) {
				for (var i = 0, len = _7be.children.length; i < len; i++) {
					var _7bf = _7be.children[i];
					_7ba.push(_7bf);
					_7bb(_7bf[opts.idField]);
				}
			}
		};
		return _7ba;
	};
	function _7c0(_7c1, _7c2) {
		if (!_7c2) {
			return 0;
		}
		var opts = $.data(_7c1, "treegrid").options;
		var view = $(_7c1).datagrid("getPanel").children("div.datagrid-view");
		var node = view.find("div.datagrid-body tr[node-id=\"" + _7c2 + "\"]").children("td[field=\"" + opts.treeField + "\"]");
		return node.find("span.tree-indent,span.tree-hit").length;
	};
	function find(_7c3, _7c4) {
		var opts = $.data(_7c3, "treegrid").options;
		var data = $.data(_7c3, "treegrid").data;
		var cc = [data];
		while (cc.length) {
			var c = cc.shift();
			for (var i = 0; i < c.length; i++) {
				var node = c[i];
				if (node[opts.idField] == _7c4) {
					return node;
				} else { if (node["children"]) {
						cc.push(node["children"]);
					}
				}
			}
		}
		return null;
	};
	function _7c5(_7c6, _7c7) {
		var opts = $.data(_7c6, "treegrid").options;
		var row = find(_7c6, _7c7);
		var tr = opts.finder.getTr(_7c6, _7c7);
		var hit = tr.find("span.tree-hit");
		if (hit.length == 0) {
			return;
		}
		if (hit.hasClass("tree-collapsed")) {
			return;
		}
		if (opts.onBeforeCollapse.call(_7c6, row) == false) {
			return;
		}
		hit.removeClass("tree-expanded tree-expanded-hover").addClass("tree-collapsed");
		hit.next().removeClass("tree-folder-open");
		row.state = "closed";
		tr = tr.next("tr.treegrid-tr-tree");
		var cc = tr.children("td").children("div");
		if (opts.animate) {
			cc.slideUp("normal", function () {
				$(_7c6).treegrid("autoSizeColumn");
				_78b(_7c6, _7c7);
				opts.onCollapse.call(_7c6, row);
			});
		} else {
			cc.hide();
			$(_7c6).treegrid("autoSizeColumn");
			_78b(_7c6, _7c7);
			opts.onCollapse.call(_7c6, row);
		}
	};
	function _7c8(_7c9, _7ca) {
		var opts = $.data(_7c9, "treegrid").options;
		var tr = opts.finder.getTr(_7c9, _7ca);
		var hit = tr.find("span.tree-hit");
		var row = find(_7c9, _7ca);
		if (hit.length == 0) {
			return;
		}
		if (hit.hasClass("tree-expanded")) {
			return;
		}
		if (opts.onBeforeExpand.call(_7c9, row) == false) {
			return;
		}
		hit.removeClass("tree-collapsed tree-collapsed-hover").addClass("tree-expanded");
		hit.next().addClass("tree-folder-open");
		var _7cb = tr.next("tr.treegrid-tr-tree");
		if (_7cb.length) {
			var cc = _7cb.children("td").children("div");
			_7cc(cc);
		} else {
			_799(_7c9, row[opts.idField]);
			var _7cb = tr.next("tr.treegrid-tr-tree");
			var cc = _7cb.children("td").children("div");
			cc.hide();
			var _7cd = $.extend({},
			opts.queryParams || {});
			_7cd.id = row[opts.idField];
			_78a(_7c9, row[opts.idField], _7cd, true, function () {
				if (cc.is(":empty")) {
					_7cb.remove();
				} else {
					_7cc(cc);
				}
			});
		}
		function _7cc(cc) {
			row.state = "open";
			if (opts.animate) {
				cc.slideDown("normal", function () {
					$(_7c9).treegrid("autoSizeColumn");
					_78b(_7c9, _7ca);
					opts.onExpand.call(_7c9, row);
				});
			} else {
				cc.show();
				$(_7c9).treegrid("autoSizeColumn");
				_78b(_7c9, _7ca);
				opts.onExpand.call(_7c9, row);
			}
		};
	};
	function _798(_7ce, _7cf) {
		var opts = $.data(_7ce, "treegrid").options;
		var tr = opts.finder.getTr(_7ce, _7cf);
		var hit = tr.find("span.tree-hit");
		if (hit.hasClass("tree-expanded")) {
			_7c5(_7ce, _7cf);
		} else {
			_7c8(_7ce, _7cf);
		}
	};
	function _7d0(_7d1, _7d2) {
		var opts = $.data(_7d1, "treegrid").options;
		var _7d3 = _78f(_7d1, _7d2);
		if (_7d2) {
			_7d3.unshift(find(_7d1, _7d2));
		}
		for (var i = 0; i < _7d3.length; i++) {
			_7c5(_7d1, _7d3[i][opts.idField]);
		}
	};
	function _7d4(_7d5, _7d6) {
		var opts = $.data(_7d5, "treegrid").options;
		var _7d7 = _78f(_7d5, _7d6);
		if (_7d6) {
			_7d7.unshift(find(_7d5, _7d6));
		}
		for (var i = 0; i < _7d7.length; i++) {
			_7c8(_7d5, _7d7[i][opts.idField]);
		}
	};
	function _7d8(_7d9, _7da) {
		var opts = $.data(_7d9, "treegrid").options;
		var ids = [];
		var p = _7b5(_7d9, _7da);
		while (p) {
			var id = p[opts.idField];
			ids.unshift(id);
			p = _7b5(_7d9, id);
		}
		for (var i = 0; i < ids.length; i++) {
			_7c8(_7d9, ids[i]);
		}
	};
	function _7db(_7dc, _7dd) {
		var opts = $.data(_7dc, "treegrid").options;
		if (_7dd.parent) {
			var tr = opts.finder.getTr(_7dc, _7dd.parent);
			if (tr.next("tr.treegrid-tr-tree").length == 0) {
				_799(_7dc, _7dd.parent);
			}
			var cell = tr.children("td[field=\"" + opts.treeField + "\"]").children("div.datagrid-cell");
			var _7de = cell.children("span.tree-icon");
			if (_7de.hasClass("tree-file")) {
				_7de.removeClass("tree-file").addClass("tree-folder tree-folder-open");
				var hit = $("<span class=\"tree-hit tree-expanded\"></span>").insertBefore(_7de);
				if (hit.prev().length) {
					hit.prev().remove();
				}
			}
		}
		_7a0(_7dc, _7dd.parent, _7dd.data, true);
	};
	function _7df(_7e0, _7e1) {
		var ref = _7e1.before || _7e1.after;
		var opts = $.data(_7e0, "treegrid").options;
		var _7e2 = _7b5(_7e0, ref);
		_7db(_7e0, {
			parent: (_7e2 ? _7e2[opts.idField] : null),
			data: [_7e1.data]
		});
		_7e3(true);
		_7e3(false);
		_793(_7e0);
		function _7e3(_7e4) {
			var _7e5 = _7e4 ? 1 : 2;
			var tr = opts.finder.getTr(_7e0, _7e1.data[opts.idField], "body", _7e5);
			var _7e6 = tr.closest("table.datagrid-btable");
			tr = tr.parent().children();
			var dest = opts.finder.getTr(_7e0, ref, "body", _7e5);
			if (_7e1.before) {
				tr.insertBefore(dest);
			} else {
				var sub = dest.next("tr.treegrid-tr-tree");
				tr.insertAfter(sub.length ? sub : dest);
			}
			_7e6.remove();
		};
	};
	function _7e7(_7e8, _7e9) {
		var _7ea = $.data(_7e8, "treegrid");
		$(_7e8).datagrid("deleteRow", _7e9);
		_793(_7e8);
		_7ea.total -= 1;
		$(_7e8).datagrid("getPager").pagination("refresh", {
			total: _7ea.total
		});
	};
	$.fn.treegrid = function (_7eb, _7ec) {
		if (typeof _7eb == "string") {
			var _7ed = $.fn.treegrid.methods[_7eb];
			if (_7ed) {
				return _7ed(this, _7ec);
			} else {
				return this.datagrid(_7eb, _7ec);
			}
		}
		_7eb = _7eb || {};
		return this.each(function () {
			var _7ee = $.data(this, "treegrid");
			if (_7ee) {
				$.extend(_7ee.options, _7eb);
			} else {
				_7ee = $.data(this, "treegrid", {
					options: $.extend({},
					$.fn.treegrid.defaults, $.fn.treegrid.parseOptions(this), _7eb),
					data: []
				});
			}
			_771(this);
			if (_7ee.options.data) {
				$(this).treegrid("loadData", _7ee.options.data);
			}
			_78a(this);
			_795(this);
		});
	};
	$.fn.treegrid.methods = {
		options: function (jq) {
			return $.data(jq[0], "treegrid").options;
		},
		resize: function (jq, _7ef) {
			return jq.each(function () {
				$(this).datagrid("resize", _7ef);
			});
		},
		fixRowHeight: function (jq, _7f0) {
			return jq.each(function () {
				_78b(this, _7f0);
			});
		},
		loadData: function (jq, data) {
			return jq.each(function () {
				_7a0(this, data.parent, data);
			});
		},
		load: function (jq, _7f1) {
			return jq.each(function () {
				$(this).treegrid("options").pageNumber = 1;
				$(this).treegrid("getPager").pagination({
					pageNumber: 1
				});
				$(this).treegrid("reload", _7f1);
			});
		},
		reload: function (jq, id) {
			return jq.each(function () {
				var opts = $(this).treegrid("options");
				var _7f2 = {};
				if (typeof id == "object") {
					_7f2 = id;
				} else {
					_7f2 = $.extend({},
					opts.queryParams);
					_7f2.id = id;
				}
				if (_7f2.id) {
					var node = $(this).treegrid("find", _7f2.id);
					if (node.children) {
						node.children.splice(0, node.children.length);
					}
					opts.queryParams = _7f2;
					var tr = opts.finder.getTr(this, _7f2.id);
					tr.next("tr.treegrid-tr-tree").remove();
					tr.find("span.tree-hit").removeClass("tree-expanded tree-expanded-hover").addClass("tree-collapsed");
					_7c8(this, _7f2.id);
				} else {
					_78a(this, null, _7f2);
				}
			});
		},
		reloadFooter: function (jq, _7f3) {
			return jq.each(function () {
				var opts = $.data(this, "treegrid").options;
				var dc = $.data(this, "datagrid").dc;
				if (_7f3) {
					$.data(this, "treegrid").footer = _7f3;
				}
				if (opts.showFooter) {
					opts.view.renderFooter.call(opts.view, this, dc.footer1, true);
					opts.view.renderFooter.call(opts.view, this, dc.footer2, false);
					if (opts.view.onAfterRender) {
						opts.view.onAfterRender.call(opts.view, this);
					}
					$(this).treegrid("fixRowHeight");
				}
			});
		},
		getData: function (jq) {
			return $.data(jq[0], "treegrid").data;
		},
		getFooterRows: function (jq) {
			return $.data(jq[0], "treegrid").footer;
		},
		getRoot: function (jq) {
			return _7b1(jq[0]);
		},
		getRoots: function (jq) {
			return _7b3(jq[0]);
		},
		getParent: function (jq, id) {
			return _7b5(jq[0], id);
		},
		getChildren: function (jq, id) {
			return _78f(jq[0], id);
		},
		getLevel: function (jq, id) {
			return _7c0(jq[0], id);
		},
		find: function (jq, id) {
			return find(jq[0], id);
		},
		isLeaf: function (jq, id) {
			var opts = $.data(jq[0], "treegrid").options;
			var tr = opts.finder.getTr(jq[0], id);
			var hit = tr.find("span.tree-hit");
			return hit.length == 0;
		},
		select: function (jq, id) {
			return jq.each(function () {
				$(this).datagrid("selectRow", id);
			});
		},
		unselect: function (jq, id) {
			return jq.each(function () {
				$(this).datagrid("unselectRow", id);
			});
		},
		collapse: function (jq, id) {
			return jq.each(function () {
				_7c5(this, id);
			});
		},
		expand: function (jq, id) {
			return jq.each(function () {
				_7c8(this, id);
			});
		},
		toggle: function (jq, id) {
			return jq.each(function () {
				_798(this, id);
			});
		},
		collapseAll: function (jq, id) {
			return jq.each(function () {
				_7d0(this, id);
			});
		},
		expandAll: function (jq, id) {
			return jq.each(function () {
				_7d4(this, id);
			});
		},
		expandTo: function (jq, id) {
			return jq.each(function () {
				_7d8(this, id);
			});
		},
		append: function (jq, _7f4) {
			return jq.each(function () {
				_7db(this, _7f4);
			});
		},
		insert: function (jq, _7f5) {
			return jq.each(function () {
				_7df(this, _7f5);
			});
		},
		remove: function (jq, id) {
			return jq.each(function () {
				_7e7(this, id);
			});
		},
		pop: function (jq, id) {
			var row = jq.treegrid("find", id);
			jq.treegrid("remove", id);
			return row;
		},
		refresh: function (jq, id) {
			return jq.each(function () {
				var opts = $.data(this, "treegrid").options;
				opts.view.refreshRow.call(opts.view, this, id);
			});
		},
		update: function (jq, _7f6) {
			return jq.each(function () {
				var opts = $.data(this, "treegrid").options;
				opts.view.updateRow.call(opts.view, this, _7f6.id, _7f6.row);
			});
		},
		beginEdit: function (jq, id) {
			return jq.each(function () {
				$(this).datagrid("beginEdit", id);
				$(this).treegrid("fixRowHeight", id);
			});
		},
		endEdit: function (jq, id) {
			return jq.each(function () {
				$(this).datagrid("endEdit", id);
			});
		},
		cancelEdit: function (jq, id) {
			return jq.each(function () {
				$(this).datagrid("cancelEdit", id);
			});
		}
	};
	$.fn.treegrid.parseOptions = function (_7f7) {
		return $.extend({},
		$.fn.datagrid.parseOptions(_7f7), $.parser.parseOptions(_7f7, ["treeField", {
			animate: "boolean"
		}]));
	};
	var _7f8 = $.extend({},
	$.fn.datagrid.defaults.view, {
		render: function (_7f9, _7fa, _7fb) {
			var opts = $.data(_7f9, "treegrid").options;
			var _7fc = $(_7f9).datagrid("getColumnFields", _7fb);
			var _7fd = $.data(_7f9, "datagrid").rowIdPrefix;
			if (_7fb) {
				if (! (opts.rownumbers || (opts.frozenColumns && opts.frozenColumns.length))) {
					return;
				}
			}
			var _7fe = 0;
			var view = this;
			var _7ff = _800(_7fb, this.treeLevel, this.treeNodes);
			$(_7fa).append(_7ff.join(""));
			function _800(_801, _802, _803) {
				var _804 = ["<table class=\"datagrid-btable\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody>"];
				for (var i = 0; i < _803.length; i++) {
					var row = _803[i];
					if (row.state != "open" && row.state != "closed") {
						row.state = "open";
					}
					var css = opts.rowStyler ? opts.rowStyler.call(_7f9, row) : "";
					var _805 = "";
					var _806 = "";
					if (typeof css == "string") {
						_806 = css;
					} else { if (css) {
							_805 = css["class"] || "";
							_806 = css["style"] || "";
						}
					}
					var cls = "class=\"datagrid-row " + (_7fe++%2 && opts.striped ? "datagrid-row-alt " : " ") + _805 + "\"";
					var _807 = _806 ? "style=\"" + _806 + "\"" : "";
					var _808 = _7fd + "-" + (_801 ? 1 : 2) + "-" + row[opts.idField];
					_804.push("<tr id=\"" + _808 + "\" node-id=\"" + row[opts.idField] + "\" " + cls + " " + _807 + ">");
					_804 = _804.concat(view.renderRow.call(view, _7f9, _7fc, _801, _802, row));
					_804.push("</tr>");
					if (row.children && row.children.length) {
						var tt = _800(_801, _802 + 1, row.children);
						var v = row.state == "closed" ? "none" : "block";
						_804.push("<tr class=\"treegrid-tr-tree\"><td style=\"border:0px\" colspan=" + (_7fc.length + (opts.rownumbers ? 1 : 0)) + "><div style=\"display:" + v + "\">");
						_804 = _804.concat(tt);
						_804.push("</div></td></tr>");
					}
				}
				_804.push("</tbody></table>");
				return _804;
			};
		},
		renderFooter: function (_809, _80a, _80b) {
			var opts = $.data(_809, "treegrid").options;
			var rows = $.data(_809, "treegrid").footer || [];
			var _80c = $(_809).datagrid("getColumnFields", _80b);
			var _80d = ["<table class=\"datagrid-ftable\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tbody>"];
			for (var i = 0; i < rows.length; i++) {
				var row = rows[i];
				row[opts.idField] = row[opts.idField] || ("foot-row-id" + i);
				_80d.push("<tr class=\"datagrid-row\" node-id=\"" + row[opts.idField] + "\">");
				_80d.push(this.renderRow.call(this, _809, _80c, _80b, 0, row));
				_80d.push("</tr>");
			}
			_80d.push("</tbody></table>");
			$(_80a).html(_80d.join(""));
		},
		renderRow: function (_80e, _80f, _810, _811, row) {
			var opts = $.data(_80e, "treegrid").options;
			var cc = [];
			if (_810 && opts.rownumbers) {
				cc.push("<td class=\"datagrid-td-rownumber\"><div class=\"datagrid-cell-rownumber\">0</div></td>");
			}
			for (var i = 0; i < _80f.length; i++) {
				var _812 = _80f[i];
				var col = $(_80e).datagrid("getColumnOption", _812);
				if (col) {
					var css = col.styler ? (col.styler(row[_812], row) || "") : "";
					var _813 = "";
					var _814 = "";
					if (typeof css == "string") {
						_814 = css;
					} else { if (cc) {
							_813 = css["class"] || "";
							_814 = css["style"] || "";
						}
					}
					var cls = _813 ? "class=\"" + _813 + "\"" : "";
					var _815 = col.hidden ? "style=\"display:none;" + _814 + "\"" : (_814 ? "style=\"" + _814 + "\"" : "");
					cc.push("<td field=\"" + _812 + "\" " + cls + " " + _815 + ">");
					var _815 = "";
					if (!col.checkbox) {
						if (col.align) {
							_815 += "text-align:" + col.align + ";";
						}
						if (!opts.nowrap) {
							_815 += "white-space:normal;height:auto;";
						} else { if (opts.autoRowHeight) {
								_815 += "height:auto;";
							}
						}
					}
					cc.push("<div style=\"" + _815 + "\" ");
					if (col.checkbox) {
						cc.push("class=\"datagrid-cell-check ");
					} else {
						cc.push("class=\"datagrid-cell " + col.cellClass);
					}
					cc.push("\">");
					if (col.checkbox) {
						if (row.checked) {
							cc.push("<input type=\"checkbox\" checked=\"checked\"");
						} else {
							cc.push("<input type=\"checkbox\"");
						}
						cc.push(" name=\"" + _812 + "\" value=\"" + (row[_812] != undefined ? row[_812] : "") + "\">");
					} else {
						var val = null;
						if (col.formatter) {
							val = col.formatter(row[_812], row);
						} else {
							val = row[_812];
						}
						if (_812 == opts.treeField) {
							for (var j = 0; j < _811; j++) {
								cc.push("<span class=\"tree-indent\"></span>");
							}
							if (row.state == "closed") {
								cc.push("<span class=\"tree-hit tree-collapsed\"></span>");
								cc.push("<span class=\"tree-icon tree-folder " + (row.iconCls ? row.iconCls : "") + "\"></span>");
							} else { if (row.children && row.children.length) {
									cc.push("<span class=\"tree-hit tree-expanded\"></span>");
									cc.push("<span class=\"tree-icon tree-folder tree-folder-open " + (row.iconCls ? row.iconCls : "") + "\"></span>");
								} else {
									cc.push("<span class=\"tree-indent\"></span>");
									cc.push("<span class=\"tree-icon tree-file " + (row.iconCls ? row.iconCls : "") + "\"></span>");
								}
							}
							cc.push("<span class=\"tree-title\">" + val + "</span>");
						} else {
							cc.push(val);
						}
					}
					cc.push("</div>");
					cc.push("</td>");
				}
			}
			return cc.join("");
		},
		refreshRow: function (_816, id) {
			this.updateRow.call(this, _816, id, {});
		},
		updateRow: function (_817, id, row) {
			var opts = $.data(_817, "treegrid").options;
			var _818 = $(_817).treegrid("find", id);
			$.extend(_818, row);
			var _819 = $(_817).treegrid("getLevel", id) - 1;
			var _81a = opts.rowStyler ? opts.rowStyler.call(_817, _818) : "";
			function _81b(_81c) {
				var _81d = $(_817).treegrid("getColumnFields", _81c);
				var tr = opts.finder.getTr(_817, id, "body", (_81c ? 1 : 2));
				var _81e = tr.find("div.datagrid-cell-rownumber").html();
				var _81f = tr.find("div.datagrid-cell-check input[type=checkbox]").is(":checked");
				tr.html(this.renderRow(_817, _81d, _81c, _819, _818));
				tr.attr("style", _81a || "");
				tr.find("div.datagrid-cell-rownumber").html(_81e);
				if (_81f) {
					tr.find("div.datagrid-cell-check input[type=checkbox]")._propAttr("checked", true);
				}
			};
			_81b.call(this, true);
			_81b.call(this, false);
			$(_817).treegrid("fixRowHeight", id);
		},
		deleteRow: function (_820, id) {
			var opts = $.data(_820, "treegrid").options;
			var tr = opts.finder.getTr(_820, id);
			tr.next("tr.treegrid-tr-tree").remove();
			tr.remove();
			var _821 = del(id);
			if (_821) {
				if (_821.children.length == 0) {
					tr = opts.finder.getTr(_820, _821[opts.idField]);
					tr.next("tr.treegrid-tr-tree").remove();
					var cell = tr.children("td[field=\"" + opts.treeField + "\"]").children("div.datagrid-cell");
					cell.find(".tree-icon").removeClass("tree-folder").addClass("tree-file");
					cell.find(".tree-hit").remove();
					$("<span class=\"tree-indent\"></span>").prependTo(cell);
				}
			}
			function del(id) {
				var cc;
				var _822 = $(_820).treegrid("getParent", id);
				if (_822) {
					cc = _822.children;
				} else {
					cc = $(_820).treegrid("getData");
				}
				for (var i = 0; i < cc.length; i++) {
					if (cc[i][opts.idField] == id) {
						cc.splice(i, 1);
						break;
					}
				}
				return _822;
			};
		},
		onBeforeRender: function (_823, _824, data) {
			if ($.isArray(_824)) {
				data = {
					total: _824.length,
					rows: _824
				};
				_824 = null;
			}
			if (!data) {
				return false;
			}
			var _825 = $.data(_823, "treegrid");
			var opts = _825.options;
			if (data.length == undefined) {
				if (data.footer) {
					_825.footer = data.footer;
				}
				if (data.total) {
					_825.total = data.total;
				}
				data = this.transfer(_823, _824, data.rows);
			} else {
				function _826(_827, _828) {
					for (var i = 0; i < _827.length; i++) {
						var row = _827[i];
						row._parentId = _828;
						if (row.children && row.children.length) {
							_826(row.children, row[opts.idField]);
						}
					}
				};
				_826(data, _824);
			}
			var node = find(_823, _824);
			if (node) {
				if (node.children) {
					node.children = node.children.concat(data);
				} else {
					node.children = data;
				}
			} else {
				_825.data = _825.data.concat(data);
			}
			this.sort(_823, data);
			this.treeNodes = data;
			this.treeLevel = $(_823).treegrid("getLevel", _824);
		},
		sort: function (_829, data) {
			var opts = $.data(_829, "treegrid").options;
			if (!opts.remoteSort && opts.sortName) {
				var _82a = opts.sortName.split(",");
				var _82b = opts.sortOrder.split(",");
				_82c(data);
			}
			function _82c(rows) {
				rows.sort(function (r1, r2) {
					var r = 0;
					for (var i = 0; i < _82a.length; i++) {
						var sn = _82a[i];
						var so = _82b[i];
						var col = $(_829).treegrid("getColumnOption", sn);
						var _82d = col.sorter ||
						function (a, b) {
							return a == b ? 0 : (a > b ? 1 : -1);
						};
						r = _82d(r1[sn], r2[sn]) * (so == "asc" ? 1 : -1);
						if (r != 0) {
							return r;
						}
					}
					return r;
				});
				for (var i = 0; i < rows.length; i++) {
					var _82e = rows[i].children;
					if (_82e && _82e.length) {
						_82c(_82e);
					}
				}
			};
		},
		transfer: function (_82f, _830, data) {
			var opts = $.data(_82f, "treegrid").options;
			var rows = [];
			for (var i = 0; i < data.length; i++) {
				rows.push(data[i]);
			}
			var _831 = [];
			for (var i = 0; i < rows.length; i++) {
				var row = rows[i];
				if (!_830) {
					if (!row._parentId) {
						_831.push(row);
						rows.splice(i, 1);
						i--;
					}
				} else { if (row._parentId == _830) {
						_831.push(row);
						rows.splice(i, 1);
						i--;
					}
				}
			}
			var toDo = [];
			for (var i = 0; i < _831.length; i++) {
				toDo.push(_831[i]);
			}
			while (toDo.length) {
				var node = toDo.shift();
				for (var i = 0; i < rows.length; i++) {
					var row = rows[i];
					if (row._parentId == node[opts.idField]) {
						if (node.children) {
							node.children.push(row);
						} else {
							node.children = [row];
						}
						toDo.push(row);
						rows.splice(i, 1);
						i--;
					}
				}
			}
			return _831;
		}
	});
	$.fn.treegrid.defaults = $.extend({},
	$.fn.datagrid.defaults, {
		treeField: null,
		animate: false,
		singleSelect: true,
		view: _7f8,
		loader: function (_832, _833, _834) {
			var opts = $(this).treegrid("options");
			if (!opts.url) {
				return false;
			}
			$.ajax({
				type: opts.method,
				url: opts.url,
				data: _832,
				dataType: "json",
				success: function (data) {
					_833(data);
				},
				error: function () {
					_834.apply(this, arguments);
				}
			});
		},
		loadFilter: function (data, _835) {
			return data;
		},
		finder: {
			getTr: function (_836, id, type, _837) {
				type = type || "body";
				_837 = _837 || 0;
				var dc = $.data(_836, "datagrid").dc;
				if (_837 == 0) {
					var opts = $.data(_836, "treegrid").options;
					var tr1 = opts.finder.getTr(_836, id, type, 1);
					var tr2 = opts.finder.getTr(_836, id, type, 2);
					return tr1.add(tr2);
				} else { if (type == "body") {
						var tr = $("#" + $.data(_836, "datagrid").rowIdPrefix + "-" + _837 + "-" + id);
						if (!tr.length) {
							tr = (_837 == 1 ? dc.body1 : dc.body2).find("tr[node-id=\"" + id + "\"]");
						}
						return tr;
					} else { if (type == "footer") {
							return (_837 == 1 ? dc.footer1 : dc.footer2).find("tr[node-id=\"" + id + "\"]");
						} else { if (type == "selected") {
								return (_837 == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-selected");
							} else { if (type == "highlight") {
									return (_837 == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-over");
								} else { if (type == "checked") {
										return (_837 == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-checked");
									} else { if (type == "last") {
											return (_837 == 1 ? dc.body1 : dc.body2).find("tr:last[node-id]");
										} else { if (type == "allbody") {
												return (_837 == 1 ? dc.body1 : dc.body2).find("tr[node-id]");
											} else { if (type == "allfooter") {
													return (_837 == 1 ? dc.footer1 : dc.footer2).find("tr[node-id]");
												}
											}
										}
									}
								}
							}
						}
					}
				}
			},
			getRow: function (_838, p) {
				var id = (typeof p == "object") ? p.attr("node-id") : p;
				return $(_838).treegrid("find", id);
			},
			getRows: function (_839) {
				return $(_839).treegrid("getChildren");
			}
		},
		onBeforeLoad: function (row, _83a) {},
		onLoadSuccess: function (row, data) {},
		onLoadError: function () {},
		onBeforeCollapse: function (row) {},
		onCollapse: function (row) {},
		onBeforeExpand: function (row) {},
		onExpand: function (row) {},
		onClickRow: function (row) {},
		onDblClickRow: function (row) {},
		onClickCell: function (_83b, row) {},
		onDblClickCell: function (_83c, row) {},
		onContextMenu: function (e, row) {},
		onBeforeEdit: function (row) {},
		onAfterEdit: function (row, _83d) {},
		onCancelEdit: function (row) {}
	});
})(jQuery);