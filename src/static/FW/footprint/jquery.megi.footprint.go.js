top.window.FootPrintTable = top.window.FootPrintTable || {};


//使用优惠码
top.window.FootPrintTable["/SYS/Coupon/ApplyCoupon"] = {
    event: "Applied Coupon",
    properties: {
        description: "Valid coupon was applied",
        CouponType: "Trial Extension"
    },
    type: "track",
    track: function (param, data) {
        return data && data.Success === true;
    }
};

//点击开始试用组织
top.window.FootPrintTable["/BD/Setup/GLSuccess"] = {
    event: "Created New Org",
    properties: {
        description: 'User clicked \'Start Trial\' button at the end of the initial setup.'
    },
    type: "track",
    track: true
};

//新增银行账户
top.window.FootPrintTable["/BD/BDBank/BDBankAccountEdit"] = {
    event: "Tried to Add a {BankType} Account",
    properties: {
        description: 'User clicked "New" and selected "{BankType}" in {Place}.'
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            alias: 'Place',
            formatter: function (type) {
                if (type === "Mission_Control") return "Mission Control";
                if (type === "Banking") return "Banking";
                else return "other place";
            }
        },
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Mission_Control") return "Mission Control";
                if (type === "Banking") return "Banking";
                else return "other place";
            }
        }, {
            name: "type",
            alias: "BankType",
            formatter: function (type) {
                if (type === "1") return "Bank";
                if (type === "2") return "Credit";
                if (type === "3") return "Cash";
                if (type === "4") return "PayPal";
                if (type === "5") return "Alipay";
                return "Illegal Account";
            }
        }]
};
//保存银行
top.window.FootPrintTable["/BD/BDBank/SaveBDBankAccount"] = {
    event: "Added a {BankType} Account",
    properties: {
        description: "User entered or updated banking details in banking module"
    },
    type: "track",
    track: true,
    extProperties: [{
        name: "model",
        alias: "BankType",
        formatter: function (model) {
            model = model || {};
            var type = model.MBankAccountType;
            if (type === 1) return "Bank";
            if (type === 2) return "Credit Account";
            if (type === 3) return "Cash Account";
            if (type === 4) return "PayPal";
            if (type === 5) return "Alipay";
            return "Illegal Account";
        }
    }]
};

//获取银行feeds 
top.window.FootPrintTable["/BD/BDBank/ImportByBankFeeds"] = {
    event: "Added Bank Fees",
    properties: {
        description: 'User clicked on "Add Bank Feeds" in the drop down menu.'
    },
    type: "track",
    track: true
};

//创建联系人
top.window.FootPrintTable["/BD/Contacts/ContactsEdit"] = {
    event: "Tried to Add a New Contact",
    properties: {
        description: 'User clicked on \'New Contact\' in {Place}.'
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                else if (type === "Contacts") return "Contacts";
                else if (type === "Dropdown") return "dropdown list";
                else if (type === "AccountTransaction") return "Account transaction initialization";
                else return "Contacts";
            }
        },
        {
            name: "Origin",
            alias: 'Place',
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                else if (type === "Contacts") return "Contacts";
                else if (type === "Dropdown") return "dropdown list";
                else if (type === "AccountTransaction") return "Account transaction initialization";
                else return "Contacts";
            }
        }]
};

//保存联系人
top.window.FootPrintTable["/BD/Contacts/ContactsUpdate"] = {
    event: "Saved a Contact",
    properties: {
        description: 'Clicked on "Save" in the Contact section details.'
    },
    type: "track",
    track: true
};


//创建员工
top.window.FootPrintTable["/BD/Employees/EmployeesEdit"] = {
    event: "Created Employee",
    properties: {
        description: 'User clicked on \'New Employee\' in {Place}.'
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Employees") return "Employees";
                else if (type === "Dropdown") return "Dropdown List";
                else if (type === "AccountTransaction") return "Account transaction initialization";
                else return "Employees";
            }
        },
        {
            name: "Origin",
            alias: 'Place',
            formatter: function (type) {
                if (type === "Employees") return "Employees";
                else if (type === "Dropdown") return "Dropdown List";
                else if (type === "AccountTransaction") return "Account transaction initialization";
                else return "Employees";
            }
        }]
};

//保存员工
top.window.FootPrintTable["/BD/Employees/EmployeesUpdate"] = {
    event: "Saved Employee",
    properties: {
        description: 'User edited & saved employee details.'
    },
    type: "track",
    track: true
};


//创建跟踪项大项
top.window.FootPrintTable["CreateTrackCategory"] = {
    event: "Created Tracking Category",
    properties: {
        description: 'User clicked to create new tracking category.',
        Origin: 'Org Settings'
    },
    type: "track",
    track: true,
    extProperties: [
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Dropdown") return "Dropdown List";
                else return "Org Setting";
            }
        }]
};

//保存跟踪项大项
top.window.FootPrintTable["/BD/Tracking/SaveTrackingInfo"] = {
    event: "Saved Tracking Category",
    properties: {
        description: 'User clicked to edit & save change to existing tracking category.'
    },
    type: "track",
    track: true
};

//创建商品
top.window.FootPrintTable["/BD/Item/ItemEdit"] = {
    event: "Created Inventory",
    properties: {
        description: 'User clicked on \'New Contact\' in {Place}.'
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            alias: 'Place',
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                if (type === "Dropdown") return "Dropdown List";
                else return "Org Setting";
            }
        },
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                if (type === "Dropdown") return "Dropdown List";
                else return "Org Setting";
            }
        }]
};

//保存商品
top.window.FootPrintTable["/BD/Item/ItemInfoUpd"] = {
    event: "Saved Inventory",
    properties: {
        description: 'User edited & saved an inventory item.'
    },
    type: "track",
    track: true
};


//新建销售单
top.window.FootPrintTable["/IV/Invoice/InvoiceEdit"] = {
    event: "Tried to Create a New Invoice",
    properties: {
        description: "User clicked to create new invoice."
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                if (type === "Misson_Control") return "Misson Control";
                else return "Invoice Main Page";
            }
        }]
};

//保存销售单 / 采购单
top.window.FootPrintTable["/IV/IVInvoiceBase/UpdateInvoice"] = {
    event: "Saved an {type}",
    properties: {
        description: "Use edited & saved {qty_type}."
    },
    type: "track",
    track: function (param, data) {
        return (param.type === "Bill" || param.type === "Sale") && data && data.Success;
    },
    extProperties: [
        {
            name: "type",
            formatter: function (type) {
                return type === "Sale" ? "Invoice" : "Bill";
            }
        },
        {
            name: "type",
            alias: "qty_type",
            formatter: function (type) {
                return type === "Sale" ? "an invoice" : "a bill";
            }
        }]
};

//更新信息也是一样的
top.window.FootPrintTable["/IV/IVInvoiceBase/UpdateInvoiceExpectedInfo"] = top.window.FootPrintTable["/IV/IVInvoiceBase/UpdateInvoice"];



//新建采购单
top.window.FootPrintTable["/IV/Bill/BillEdit"] = {
    event: "Created Bill",
    properties: {
        description: "User clicked to create new bill"
    },
    type: "track",
    track: function (param) {
        if (!!param && !!param.id) return false;
        return true;
    },
    extProperties: [
        {
            name: "Origin",
            formatter: function (type) {
                if (type === "Quick_Menu") return "Quick Menu";
                if (type === "Misson_Control") return "Misson Control";
                else return "Invoice Main Page";
            }
        }]
};

//导入(联系人、员工)
top.window.FootPrintTable["/BD/Import/Import"] = {
    event: " Imported Data",
    properties: {
        description: "User clicked on 'Import' in {Place}."
    },
    type: "track",
    track: function (param) {
        var typeList = ["Contact", "Employees"];
        return typeList.contains(param.id);
    },
    extProperties: [
        {
            name: "id",
            alias: "DataClass",
            formatter: function (type) {
                if (type === "Contact") return "Contact";
                if (type === "Employees") return "Employee";
                else return "";
            }
        },
        {
            name: "id",
            alias: "Origin",
            formatter: function (type) {
                if (type === "Contact") return "Contacts";
                if (type === "Employees") return "Employees";
                else return "";
            }
        },
        {
            name: "id",
            alias: "Place",
            formatter: function (type) {
                if (type === "Contact") return "the contact main page";
                if (type === "Employees") return "the employee main page";
            }
        }
    ]
};


//导入银行对账单
top.window.FootPrintTable["/BD/BDBank/Import"] = {
    event: "Imported Data",
    properties: {
        description: "User clicked on 'Import' in the import popup window (bank module)",
        DataClass: 'Bank',
        Origin: 'Banking',
        Place:'the import popup window (bank module)'
    },
    type: "track",
    track: true
};


//完成 导入 
top.window.FootPrintTable["/BD/Import/CompleteImport"] = {
    event: "Completed Import",
    properties: {
        description: 'User clicked on "Complete Import" in {Place}',
    },
    type: "track",
    track: function (param) {
        var typeList = ["Contact", "Employees"];
        return typeList.contains(param.id);
    },
    extProperties: [
        {
            name: "id",
            alias: "DataClass",
            formatter: function (type) {
                if (type === "Contact") return "Contact";
                if (type === "Employees") return "Employee";
                else return "";
            }
        },
        {
            name: "id",
            alias: "Origin",
            formatter: function (type) {
                if (type === "Contact") return "Contacts";
                if (type === "Employees") return "Employees";
                else return "";
            }
        },
        {
            name: "id",
            alias: "Place",
            formatter: function (type) {
                if (type === "Contact") return "the contact main page";
                if (type === "Employees") return "employee tab";
            }
        }
    ]
};

//完成导入银行对账单
top.window.FootPrintTable["/BD/BDBank/SaveImportSolution"] = {
    event: "Completed Import",
    properties: {
        description: 'User clicked on "Complete Import" in the import popup window (bank module)',
        DataClass: 'Bank',
        Origin: 'Banking'
    },
    type: "track",
    track: true
};

//点击帮助中心
top.window.FootPrintTable["uploadFile"] = {
    event: "Uploaded File",
    properties: {
        description: "User uploaded files"
    },
    type: "track",
    track: true
};


//点击帮助中心
top.window.FootPrintTable["helpCenter"] = {
    event: "Clicked for Help",
    properties: {
        description: "User clicked on the help icon in the top right-hand corner"
    },
    type: "track",
    track: true
};


//下载excel工具
top.window.FootPrintTable["/api/File/GetExcelPlusApp"] = {
    event: "Downloaded XLDT",
    properties: {
        description: "User clicked Excel Data Tools"
    },
    type: "track",
    track: true
};

//顶部的查询按钮
top.window.FootPrintTable["topSearch"] = {
    event: "Searched",
    properties: {
        description: "User clicked on Search button inside the search window."
    },
    type: "track",
    track: true
};

//邀请用户 
top.window.FootPrintTable["/BD/User/UserInviteSendMeg"] = {
    event: "Invited User",
    properties: {
        description: "User clicked 'Send invitation' as they created a new use."
    },
    type: "track",
    track: true
};

//保存用户
top.window.FootPrintTable["/BD/User/UserPermissionUpd"] = {
    event: "Saved User",
    properties: {
        description: "User clicked 'Save' in the user details page."
    },
    type: "track",
    track: function (param, data) {
        return param && !!param.model.MItemID;
    }
};

//邀请用户 
top.window.FootPrintTable["InviteUser"] = {
    event: "Tried to invite a new user",
    properties: {
        description: "User clicked 'Invite a User' as they created a new use."
    },
    type: "track",
    track: true
};

