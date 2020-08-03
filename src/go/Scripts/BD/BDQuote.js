/* 基础资料禁用/删除逻辑返回信息的处理 */

var BDQuote = {
    SeeAllLinkItemByDelete: "GoItemList.deleteClick(true)",
    SeeAllLinkItemByArchive: "GoItemList.archiveItem(false,true)",

    SeeAllLinkTaxRateByDelete: "TaxRateSetting.deleteClick(true)",
    SeeAllLinkTaxRateByArchive: "TaxRateSetting.archiveItem(false,true)",

    SeeAllLinkExpenseItemByDelete: "GoExpenseItemList.deleteClick(true)",
    SeeAllLinkExpenseItemByArchive: "GoExpenseItemList.archiveItem(false,true)",

    SeeAllLinkContactByDelete: "GoContactsList.deleteClick(true)",
    SeeAllLinkContactByArchive: "GoContactsList.archiveClick(true)",

    SeeAllLinkEmployeesByDelete: "GoEmployeesList.deleteEmployee(true)",
    SeeAllLinkEmployeesByArchive: "GoEmployeesList.archiveEmployee(true)",

    SeeAllLinkAccountByDelete: "AccountList.deleteAccount(true)",
    SeeAllLinkAccountByArchive: "AccountList.archiveAccount(true)",

    SeeAllLinkPayItemByArchive: "SalaryItemList.disableItem(false,true);",

    GetQuoteMsg: function (data, isSeeAll) {
        var list = data.EntryList;
        var isDelete = data.IsDelete;
        //全部没有被引用
        var isAllSuccess = data.AllSuccess;
        //操作可以继续进行下去
        var isSuccess = data.Success;
        var objectName = data.ObjectName;
        var bizObject = data.BizObjectName;
        var listCount = list.length;
        var seeAllLang = HtmlLang.Write(LangModule.BD, "SeeAll", "查看全部");
        var retMsg = '<div class="popup-list-msg">';
        var title = "";
        var retLink = BDQuote.SeeAllLink(bizObject, isDelete);
        //标题
        if (isDelete) {
            if (isAllSuccess) {
                title = HtmlLang.Write(LangModule.BD, "AreYouSureDelete", "您确定要删除以下项吗：");
            }
            else {
                if (isSuccess) {
                    title = HtmlLang.Write(LangModule.BD, "DeleteDataIsQuoteContinueDelete", "以下项已被使用，您确定继续删除没有被引用的项吗：");
                } else {
                    title = HtmlLang.Write(LangModule.BD, "DeleteDataIsQuote", "以下项已被使用，无法删除：");
                }
                
            }
        } else {
            if (isAllSuccess) {
                title = HtmlLang.Write(LangModule.BD, "AreYouSureInactive", "您确定要禁用以下项吗：");
            } else {
                if (bizObject == "Account") {
                    title = HtmlLang.Write(LangModule.BD, "AccountInactiveIsQuote", "以下科目已被使用，无法禁用：");
                } else {
                    title = HtmlLang.Write(LangModule.BD, "InactiveDataIsQuote", "以下项已被使用，您确定要继续禁用吗：");
                }

            }
        }
        retMsg = retMsg + title + "</br>";
        //添加项的次数
        var j = 0;
        for (var i = 0; i < listCount; i++) {
            //
            var entry = list[i];
            var entryName = mText.encode(entry.Name);
            var entryCanDelte = entry.IsCanDelete;
            //全部没有被引用
            if (isAllSuccess) {
                if (!isSeeAll) {
                    if (i > 3) {

                        retMsg = retMsg + "<a href='javascript:void(0)' onclick='" + retLink + "'>" + seeAllLang + "</a>";
                        j++;
                        break;
                    } else {
                        retMsg = retMsg + " - " + entryName + "</br>";
                        j++;
                    }
                } else {
                    retMsg = retMsg + " - " + entryName + "</br>";
                    j++;
                }
            } else {
                if (bizObject != "Account") {
                    if (!entryCanDelte) {
                        if (!isSeeAll) {
                            if (j > 3) {
                                retMsg = retMsg + "<a href='javascript:void(0)' onclick='" + retLink + "'>" + seeAllLang + "</a>";
                                j++;
                                break;
                            } else {
                                retMsg = retMsg + " - " + entryName + "</br>";
                                j++;
                            }
                        } else {
                            retMsg = retMsg + " - " + entryName + "</br>";
                            j++;
                        }
                    }
                } else {
                    if (!entryCanDelte) {
                        retMsg = retMsg + " - " + entryName + "</br>";
                        j++;
                    }
                }

            }
        }
        retMsg = retMsg + "</div>";

        return retMsg;
    },
    //查看全部的跳转
    SeeAllLink: function (bizObject, isDelete) {
        var retLink = "";
        switch (bizObject) {
            case "Item":
                retLink = isDelete ? BDQuote.SeeAllLinkItemByDelete : BDQuote.SeeAllLinkItemByArchive;
                break;
            case "TaxRate":
                retLink = isDelete ? BDQuote.SeeAllLinkTaxRateByDelete : BDQuote.SeeAllLinkTaxRateByArchive;
                break;
            case "Account":
                retLink = isDelete ? BDQuote.SeeAllLinkAccountByDelete : BDQuote.SeeAllLinkAccountByArchive;
                break;
            case "Employees":
                retLink = isDelete ? BDQuote.SeeAllLinkEmployeesByDelete : BDQuote.SeeAllLinkEmployeesByArchive;
                break;
            case "Contact":
                retLink = isDelete ? BDQuote.SeeAllLinkContactByDelete : BDQuote.SeeAllLinkContactByArchive;
                break;
            case "ExpenseItem":
                retLink = isDelete ? BDQuote.SeeAllLinkExpenseItemByDelete : BDQuote.SeeAllLinkExpenseItemByArchive;
                break;
            case "PayRun":
                retLink = BDQuote.SeeAllLinkPayItemByArchive;
                break;
                
        }
        return retLink;
    }
}