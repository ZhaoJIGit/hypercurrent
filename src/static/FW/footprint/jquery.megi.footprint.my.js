top.window.FootPrintTable = top.window.FootPrintTable || {};
top.window.FootPrintTable["/BD/Setup/OrgSetting"] = {
    event: "Tried to Create New Org",
    properties: {
        description: 'User clicked "Add an organisation" button.'
    },
    type: "track",//type: "page",
    track: function (param) {
        return param.id === "0";
    },
    extProperties: [{
        name: "version",
        formatter: function (value) {
            if (value === "0")
                return "Standard Ledger";
            else if (value === "1")
                return "Smart Ledger";
            return "Illegal Ledger";
        }
    }]
};

//修改用户邮箱
top.window.FootPrintTable["/Account/AcctUpdateEmail"] = {
    event: "Changed Email Address",
    properties: {
        description: 'User clicked in Account Settings and clicked "Change Email" .'
    },
    type: "track",
    track: function (param, data) {
        return data && data.Success;
    }
};

//修改用户信息
top.window.FootPrintTable["/Profile/UploadHeaderImage"] = {
    event: "Saved Profile",
    properties: {
        description: 'User clicked to edit and saved user profile information.'
    },
    type: "track",
    track: function (param, data) {
        return data && data.Success;
    }
};

//修改用户信息
top.window.FootPrintTable["ChangeLanguage"] = {
    event: "Switched language in-session",
    properties: {
        description: 'Clicked on "Yes" afer prompted to confirm after user clicked "Profile / English or Chinese"'
    },
    type: "track",
    track: true,
    extProperties: [{
        name: "lang",
        formatter: function (lang) {
            if (lang === "0x0009") return "english";
            else if (lang === "0x7804") return "简体";
            else return "繁体";
        }
    }]
};

/*
//支付跟踪
top.window.FootPrintTable["PayNow"] = {
    event: "Attempted Subscribe",
    properties: {
        description: "User clicked on 'Pay Now' button"
    },
    type: "track",
    track: true,
    extProperties: [{
        name: "version",
        alias: "Edition",
        formatter: function (value) {
            if (value === "0")
                return "Standard Ledger";
            else if (value === "1")
                return "Smart Ledger";
            return "Illegal Ledger";
        }
    }, {
        name: "status",
        alias: "Org Status",
        formatter: function (value) {
            if (value === "1") {
                return "Trial Expired";
            } else if (value === "2") {
                return "Free trial version";
            } else if (value === "3") {
                return "Paid version";
            } else if (value === "4") {
                return "Beta tester one year free use";
            } else if (value === "5") {
                return "Expired";
            } else {
                return "Free trial version";
            }
        }
    }]
};
*/