(function (a) {

    var w = window.document;
    var ww = window;

    var frame = $("iframe", top.$(".m-tab-content:visible"))[0];

    if (window.parent !== top.window) {
        w = frame.contentWindow.document;
        ww = frame.contentWindow;
    }

    a.alerts = {
        verticalOffset: -75,
        horizontalOffset: 0,
        repositionOnResize: true,
        overlayOpacity: 0.50,
        overlayOpacity2nd: 0.40,
        overlayColor: "#000",
        draggable: true,
        okButton: LangKey.OK,
        //contactButton: HtmlLang.Write(LangModule.My, "ContactUs", "联系我们"),
        contactButton: LangKey.ContactUs,
        cancelButton: LangKey.Cancel,
        yesButton: LangKey.Yes,
        noButton: LangKey.No,
        cancelConfirm: LangKey.CancelConfirm,
        dialogClass: null,
        built: function (b, c, d, isL) {
            a.alerts._show(d, b, null, "built",
            function (e) {
                if (c) {
                    c(e)
                }
            },
            null, null, null, null, true, isL)
        },
        alert: function (b, c, d, f, isL) {
            a.alerts._show(d, b, null, "alert",
            function (e) {
                if (c) {
                    c(e)
                }
            },
            null, null, null, null, f, isL)
        },
        confirm: function (b, c, d, x, isL) {
            if (c == null) {
                c = "Are you sure"
            }
            a.alerts._show(c, b, null, "confirm",
            function (e) {
                if (d) {
                    d(e)
                }
            },
            null, null, null, null, x, isL)
        },
        prompt: function (b, c, d, e, isL) {
            if (d == null) {
                d = "Please enter something"
            }
            a.alerts._show(d, b, c, "prompt",
            function (f) {
                if (e) {
                    e(f)
                }
            },
            null, null, null, null, false, isL)
        },
        openBox: function (f, g, b, c, e, d, i, isL) {
            if (g == null) {
                g = "Information"
            }
            a.alerts._show(g, f, null, "openBox",
            function (h) {
                if (i) {
                    i(h)
                }
            },
            b, c, e, d, false, isL)
        },
        overAlert: function (c, b) {
            a.alerts._overShow(c, b)
        },
        _overShow: function (d, c) {
            if (c == null) {
                c = 3000
            }
            var b = c + 600;
            //不可在此使用，因为提示语里面可能有各种符号 d = encodeURI(d);
            a("body", w).append('<div id="over_container" style="display:none"><div id="over_message"></div></div>');
            a("#over_message", w).html(d.replace(/\\n/g, "<br />"));
            if (a.alerts.dialogClass) {
                a("#over_container", w).addClass(a.alerts.dialogClass)
            }
            var e = (a.browser.msie && parseInt(a.browser.version) <= 6) ? "absolute" : "fixed";
            a("#over_container", w).css({
                position: e,
                zIndex: 9999999,
                width: 350,
                padding: 0,
                margin: 0
            }).show("fast");
            a("#over_container", w).css({
                minWidth: a("#over_container", w).outerWidth(),
                maxWidth: a("#over_container", w).outerWidth()
            });
            a.alerts._overReposition();
            setTimeout(function () {
                a("#over_container", w).hide("fast")
            },
            c);
            setTimeout(function () {
                a("#over_container", w).remove()
            },
            b)
        },
        _overReposition: function () {
            var c = 4;
            c = ((a(ww).height() / 2) - (a("#over_container", w).outerHeight() / 2)) + a.alerts.verticalOffset;
            var b = ((a(ww).width() / 2) - (a("#over_container", w).outerWidth() / 2)) + a.alerts.horizontalOffset;
            if (c < 0) {
                c = 0
            }
            if (b < 0) {
                b = 0
            }
            if (a.browser.msie && parseInt(a.browser.version) <= 6) {
                c = c + a(ww).scrollTop()
            }
            if (a.browser.msie && parseInt(a.browser.version) <= 6) {
                b = b - 175
            }
            a("#over_container", w).css({
                top: c + "px",
                left: b + "px"
            });
            //a("#popup_overlay", w).height(a(document).height())
            a("#popup_overlay", w).height(a(w).height())
        },
        _show: function (j, b, k, g, m, l, c, f, n, x, isL) {
            a.alerts._hide();
            a.alerts._overlay("show");
            a("body", w).append('<div id="popup_container" style="display:none"><div id="popup_title">&nbsp;</div><div id="popup_content"><div id="popup_message"></div></div></div>');
            if (a.alerts.dialogClass) {
                a("#popup_container", w).addClass(a.alerts.dialogClass)
            }
            var i = (a.browser.msie && parseInt(a.browser.version) <= 6) ? "absolute" : "fixed";
            a("#popup_container", w).css({
                position: i,
                zIndex: 9999999,
                padding: 0,
                margin: 0
            }).show();
            a("#popup_content", w).addClass(g);
            if (g != "openBox") {
                a("#popup_message", w).html(b.replace(/\\n/g, "<br />"))
            }
            a("#popup_container", w).css({});
            a.alerts._reposition();
            a.alerts._maintainPosition(true);
            switch (g) {
                case "built":
                    a("#popup_message", w).after('<div id="popup_panel"><input type="button" class="easyui-linkbutton easyui-linkbutton-yellow" value="' + a.alerts.contactButton + '" id="popup_ok" /><input class="easyui-linkbutton m-btn-s" type="button" value="' + a.alerts.cancelButton + '" id="popup_cancel" /></div>');
                    a("#popup_ok", w).click(function (e) {
                        a.alerts._hide();
                        m(true)
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_cancel", w).click(function (e) {
                        a.alerts._hide();
                        if (m) {
                            m(false)
                        }
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_ok, #popup_cancel", w).keypress(function (h) {
                        if (h.keyCode == 13) {
                            a("#popup_ok", w).trigger("click")
                        }
                        if (h.keyCode == 27) {
                            a("#popup_cancel", w).trigger("click")
                        }
                    });
                    break;
                case "alert":
                    a("#popup_message", w).after('<div id="popup_panel"><input type="button" class="easyui-linkbutton easyui-linkbutton-yellow" value="' + a.alerts.okButton + '" id="popup_ok" /></div>');
                    a("#popup_ok", w).click(function (e) {
                        a.alerts._hide();
                        m(true)
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_ok", w).focus().keypress(function (h) {
                        if (h.keyCode == 13 || h.keyCode == 27) {
                            a("#popup_ok", w).trigger("click")
                        }
                    });
                    //针对提醒的类型
                    switch (j) {
                        case 0:
                            break;
                        case 1:
                            a("#popup_content", w).removeClass(g).addClass("warning");
                            break;
                        case 2:
                            a("#popup_content", w).removeClass(g).addClass("error");
                            break;
                        default:
                            break;
                    }
                    break;
                case "confirm":
                    a("#popup_message", w).after('<div id="popup_panel"><input class="easyui-linkbutton easyui-linkbutton-yellow" type="button" value="' + a.alerts.yesButton + '" id="popup_ok" /> ' + (x ? '<a id="popup_think" >  ' + a.alerts.cancelConfirm + ' </a>' : ' ') + '<input class="easyui-linkbutton m-btn-s" type="button" value="' + a.alerts.noButton + '" id="popup_cancel" /></div>');
                    a("#popup_ok", w).click(function (e) {
                        a.alerts._hide();
                        if (m) {
                            m(true)
                        }
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_think", w).click(function (e) {
                        a.alerts._hide();
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_cancel", w).click(function (e) {
                        a.alerts._hide();
                        if (m) {
                            m(false)
                        }
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_ok", w).focus();
                    a("#popup_ok, #popup_cancel", w).keypress(function (e) {
                        if (e.keyCode == 13) {
                            a("#popup_ok", w).trigger("click")
                        }
                        if (e.keyCode == 27) {
                            a("#popup_cancel", w).trigger("click")
                        }
                        e.stopPropagation();
                        return false;
                    });
                    break;
                case "prompt":
                    a("#popup_message", w).append('<br /><input type="text" size="30" id="popup_prompt" />').after('<div id="popup_panel"><input type="button" value="' + a.alerts.okButton + '" id="popup_ok" /> <input type="button" value="' + a.alerts.cancelButton + '" id="popup_cancel" /></div>');
                    a("#popup_prompt", w).width(a("#popup_message").width() - 10);
                    a("#popup_ok", w).click(function (e) {
                        var e = a("#popup_prompt", w).val();
                        a.alerts._hide();
                        if (m) {
                            m(e)
                        }
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_cancel", w).click(function (e) {
                        a.alerts._hide();
                        if (m) {
                            m(null)
                        }
                        e.stopPropagation();
                        return false;
                    });
                    a("#popup_prompt, #popup_ok, #popup_cancel", w).keypress(function (e) {
                        if (e.keyCode == 13) {
                            a("#popup_ok", w).trigger("click")
                        }
                        if (h.keyCode == 27) {
                            a("#popup_cancel", w).trigger("click")
                        }
                        e.stopPropagation();
                        return false;
                    });
                    if (k) {
                        a("#popup_prompt", w).val(k)
                    }
                    a("#popup_prompt", w).focus().select();
                    break;
                case "openBox":
                    a("#popup_message", w).append(a(b).html());
                    if (l) {
                        a("#popup_container", w).css({
                            width:
                        l + "px"
                        })
                    }
                    if (c) {
                        a("#popup_container", w).css({
                            height: c + "px"
                        });
                        a("#popup_message", w).css({
                            height: (c - 48) + "px"
                        })
                    }
                    a.alerts._reposition();
                    if (f) {
                        a(f).click(function (e) {
                            a.alerts._hide();
                            if (m) {
                                m(true)
                            }
                            e.stopPropagation();
                            return false;
                        })
                    }
                    if (n) {
                        a(n).click(function (e) {
                            a.alerts._hide();
                            return false;
                            if (m) {
                                m(false)
                            }
                            e.stopPropagation();
                            return false;
                        })
                    }
                    break
            }
            if (isL) {
                a("#popup_message", w).css("cssText", "text-align:left!important");
            }
            a("#popup_close", w).click(function (e) {
                a.alerts._hide();
                if (m) {
                    m()
                }
                e.stopPropagation();
                return false;
            });
            if (a.alerts.draggable) {
                try {
                    a("#popup_container", w).draggable({
                        handle: a("#popup_title", w)
                    });
                    a("#popup_title", w).css({
                        cursor: "move"
                    })
                } catch (d) { }
            }
        },
        _hide: function () {
            a("#popup_container", w).remove();
            a.alerts._overlay("hide");
            a.alerts._maintainPosition(false)
        },
        _overlay: function (b) {
            switch (b) {
                case "show":
                    a.alerts._overlay("hide");
                    a("BODY", w).append('<div id="popup_overlay"></div>');
                    a("#popup_overlay", w).css({
                        position:
                    "absolute",
                        zIndex: 9999998,
                        top: "0px",
                        left: "0px",
                        width: "100%",
                        //height: a(document, w).height(),
                        height: a(w).height(),
                        background: a.alerts.overlayColor,
                        opacity: ($("#XYTipsWindowBg", parent.document).is(":visible") ? a.alerts.overlayOpacity2nd : a.alerts.overlayOpacity)
                    });
                    break;
                case "hide":
                    a("#popup_overlay", w).remove();
                    break
            }
        },
        _reposition: function () {
            var c = ((a(ww).height() / 2) - (a("#popup_container", w).height() / 2)) + a.alerts.verticalOffset;
            var b = ((a(ww).width() / 2) - (a("#popup_container", w).width() / 2)) + a.alerts.horizontalOffset;
            if (c < 0) {
                c = 0
            }
            if (b < 0) {
                b = 0
            }
            if (a.browser.msie && parseInt(a.browser.version) <= 6) {
                c = c + a(ww).scrollTop()
            }
            a("#popup_container", w).css({
                top: c + "px",
                left: b + "px"
            });
            a("#popup_overlay", w).height(a(w).height())
        },
        _maintainPosition: function (b) {
            if (a.alerts.repositionOnResize) {
                switch (b) {
                    case true:
                        a(ww).bind("resize", a.alerts._reposition);
                        break;
                    case false:
                        a(ww).unbind("resize", a.alerts._reposition);
                        break
                }
            }
        }
    };
    hiBuilt = function (b, c, d, isL) {
        a.alerts.built(b, c, d, isL)
    };
    hiAlert = function (b, c, d, isL) {
        a.alerts.alert(b, c, d, undefined, isL)
    };
    hiConfirm = function (b, c, d, x, isL) {
        a.alerts.confirm(b, c, d, x, isL)
    };
    hiPrompt = function (b, c, d, e, isL) {
        a.alerts.prompt(b, c, d, e, isL)
    };
    hiBox = function (f, g, b, c, e, d, i, isL) {
        a.alerts.openBox(f, g, b, c, e, d, i, isL)
    };
    hiOverAlert = function (c, b) {
        a.alerts.overAlert(c, b)
    }
})(jQuery);
