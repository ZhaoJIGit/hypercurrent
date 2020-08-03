top.window.FootPrintTable = top.window.FootPrintTable || {};
top.window.FootPrintTable["/Account/UserRegister"] = {
    event: "Registered",
    properties: {
        description: "User's account was successfully created in the backend."
    },
    track: true,
    type: "track",
    extProperties: [{
        name: "MEmailAddress"
    }, {
        name: "MFirstName"
    }, {
        name: "MLastName"
    }, {
        name: "MPhoneNumber"
    }],
    identify: "MEmailAddress"
};