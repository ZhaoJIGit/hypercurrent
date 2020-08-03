/*
    在这js里面提供一些通用的获取数据的接口，默认情况下都是同步的，如果是异步的话，就需要传入回调函数
*/
; (function () {

    var mData = (function () {

        var mData = function () {

        };

        return mData;
    })();

    /*
        通过获取跟踪项 返回结果是 List<<Name,Value>>格式的
    */
    mData.getNameValueTrackDataList = function (param, callback) {
        //
        var url = "/BD/Tracking/GetTrackBasicInfo";
        //
        var trackData = [];
        //同步获取
        mAjax.post(url, param,
             function (data) {
                 trackData=data;
             }, "", "", false);
        return trackData;
    }

    /*
        获取当前组织的汇率
    */
    mData.getTaxRateList = function (param, callback) {
        //
        var url = "/BD/TaxRate/GetTaxRateList";
        //
        var taxRateData = [];
        //同步获取
        mAjax.post(url, param,
             function (data) {
                 //
                 if (data) {
                     //
                     taxRateData = data;
                     //需要把里面的汇率进行一个计算
                     for (var i = 0 ; i < data.length ; i++) {
                         //
                         data[i].MName = data[i].MName + "(" + (+data[i].MTaxRate).toFixed(2) + "%)";
                         //
                         data[i].MTaxRate = data[i].MTaxRate / 100.00;
                         //
                         data[i].MEffectiveTaxRate = data[i].MEffectiveTaxRate / 100.00;
                     }
                     //
                     $.isFunction(callback) && callback(taxRateData);
                 }
             }, "", "", false);
        //
        return taxRateData;
    }

    //
    window.mData = $.mData = mData;
})()