//(function () { var w = window.top; var ic = w.Intercom; if (typeof ic === "function") { ic('reattach_activator'); ic('update', w.intercomSettings); } else { var d = document; var i = function () { i.c(arguments) }; i.q = []; i.c = function (args) { i.q.push(args) }; w.Intercom = i; function l() { var s = d.createElement('script'); s.type = 'text/javascript'; s.async = true; s.src = 'https://widget.intercom.io/widget/' + w.InterComAppID; var x = d.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x); } if (w.attachEvent) { w.attachEvent('onload', l); } else { w.addEventListener('load', l, false); } } })();
//(function (e, b) {
//    if (!b.__SV) {
//        var a, f, i, g; window.mixpanel = b; b._i = []; b.init = function (a, e, d) {
//            function f(b, h) { var a = h.split("."); 2 == a.length && (b = b[a[0]], h = a[1]); b[h] = function () { b.push([h].concat(Array.prototype.slice.call(arguments, 0))) } } var c = b; "undefined" !== typeof d ? c = b[d] = [] : d = "mixpanel"; c.people = c.people || []; c.toString = function (b) { var a = "mixpanel"; "mixpanel" !== d && (a += "." + d); b || (a += " (stub)"); return a }; c.people.toString = function () { return c.toString(1) + ".people (stub)" }; i = "disable time_event track track_pageview track_links track_forms register register_once alias unregister identify name_tag set_config people.set people.set_once people.increment people.append people.union people.track_charge people.clear_charges people.delete_user".split(" ");
//            for (g = 0; g < i.length; g++) f(c, i[g]); b._i.push([a, e, d])
//        }; b.__SV = 1.2; a = e.createElement("script"); a.type = "text/javascript"; a.async = !0; a.src = "undefined" !== typeof MIXPANEL_CUSTOM_LIB_URL ? MIXPANEL_CUSTOM_LIB_URL : "file:" === e.location.protocol && "//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js".match(/^\/\//) ? "https://cdn.mxpnl.com/libs/mixpanel-2-latest.min.js" : "//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"; f = e.getElementsByTagName("script")[0]; f.parentNode.insertBefore(a, f)
//    }
//})(document, window.mixpanel || []);
//mixpanel.init("e2a19a79ea0ffdf26aebe95303e3e464");

//(function () {
//    var t, i, e, n = window, o = document, a = arguments, s = "script", r = ["config", "track", "identify", "visit", "push", "call", "trackForm", "trackClick"], c = function () { var t, i = this; for (i._e = [], t = 0; r.length > t; t++) (function (t) { i[t] = function () { return i._e.push([t].concat(Array.prototype.slice.call(arguments, 0))), i } })(r[t]) }; for (n._w = n._w || {}, t = 0; a.length > t; t++) n._w[a[t]] = n[a[t]] = n[a[t]] || new c; i = o.createElement(s), i.async = 1, i.src = "//static.woopra.com/js/w.js", e = o.getElementsByTagName(s)[0], e.parentNode.insertBefore(i, e)
//})("woopra");
//woopra.config({ domain: 'megichina.com' });
//woopra.track();
//if (window.mixpanelUserID != undefined) {
//    mixpanel.register(window.mixpanelSettings);
//    mixpanel.people.set(window.mixpanelSettings);
//    mixpanel.identify(window.mixpanelUserID);
//}



var MegiTrack = {
    TrackEvent: function (eventMessage, mixMessage) {
        return;
        if (window.top.isTrack == "0") {
            return;
        }
        window.top.Intercom("trackEvent", eventMessage);
        if (mixMessage == undefined) {
            mixMessage = eventMessage;
        }
        window.top.mixpanel.track(mixMessage);
    },
    Intercom: {
        track: function (event) {
            return;
            if (window.top.isTrack == "0") {
                return;
            }
            window.top.Intercom("trackEvent", event);
        }
    },
    MixPanel: {
        track: function (event, obj) {
            return;
            if (window.top.isTrack == "0") {
                return;
            }
            if (obj == null) {
                window.top.mixpanel.track(event);
            } else {
                window.top.mixpanel.track(event, obj);
            }
        },
        login: function (name, email, phone) {
            if (window.top.isTrack == "0") {
                return;
            }
            window.top.mixpanel.track("Signed in", {
                "$name": name,
                "$email": email,
                "$phone": phone
            });
        }
    }
};


//$(document).ready(function () {
//    var itercomTimeoutId = "";
//    itercomTimeoutId = setInterval(function () {
//        if ($("#intercom-container").length > 0) {
//            $("#intercom-container").find(".intercom-launcher").css({ "bottom": "60px" });
//            clearTimeout(itercomTimeoutId);
//        }
//    }, 100);
//});