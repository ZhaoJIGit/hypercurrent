window.InterComAppID = "ir9cjxzd";
window.intercomSettings = {
    app_id: window.InterComAppID,
    widget: {
        "activator": "#megiwidget"
    }
};
(function () { var w = window; var ic = w.Intercom; if (typeof ic === "function") { ic('reattach_activator'); ic('update', intercomSettings); } else { var d = document; var i = function () { i.c(arguments) }; i.q = []; i.c = function (args) { i.q.push(args) }; w.Intercom = i; function l() { var s = d.createElement('script'); s.type = 'text/javascript'; s.async = true; s.src = 'https://widget.intercom.io/widget/ir9cjxzd'; var x = d.getElementsByTagName('script')[0]; x.parentNode.insertBefore(s, x); } if (w.attachEvent) { w.attachEvent('onload', l); } else { w.addEventListener('load', l, false); } } })();

var MegiTrack = {
    login: function (email, name,phone) {
        var myData = new Date();
        var times = myData.getTime();
        if (phone == undefined) {
            phone = "";
        }
        window.Intercom("boot", { app_id: window.InterComAppID, name: name, email: email, phone:phone, created_at: times });
    },
    logout: function () {
        window.Intercom("boot", { app_id: window.InterComAppID });
    },
    update: function () {
        window.Intercom('update');
    },
    TrackEvent: function (eventMessage, mixMessage) {
        window.Intercom("track event", eventMessage);
        if (mixMessage == undefined) {
            mixMessage = eventMessage;
        }
        mixpanel.track(mixMessage);
    }
};

MegiTrack.update();

