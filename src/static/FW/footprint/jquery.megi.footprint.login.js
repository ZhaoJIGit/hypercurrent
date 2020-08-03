top.window.FootPrintTable = top.window.FootPrintTable || {};
top.window.FootPrintTable["/Account/SignIn"] = {
    event: "Logged In",
    properties: {
        description: "When a user is logged in"
    },
    type: "track",
    track: true,
    extProperties: [{
        name: "Email"
    }],
    identify: "Email"
};
top.window.FootPrintTable["Logout"] = {
    event: "Logged out",
    properties: {
        description: "Clicked on \"Log Out\" and confirmed to log out"
    },
    type: "track",
    track: true,
    extProperties: [{
        name: "Email"
    }],
    identify: "Email"
};