var BDBankCashCodingFastCode = {
    GetDataList: function () {
        var url = "/FC/FCCashCodingModule/GetListByCode";
        var result = [];
        //同步获取
        mAjax.post(url, { code: '' },
             function (data) {
                 result = data;
             }, "", "", false);
        if (result != null && result.length > 0) {
            for (var i = 0; i < result.length; i++) {
                if (result[i].MName) {
                    result[i].MText = result[i].MCode + ":" + result[i].MName;
                } else {
                    result[i].MText = result[i].MCode;
                }
            }
        }
        return result;
    }
}
