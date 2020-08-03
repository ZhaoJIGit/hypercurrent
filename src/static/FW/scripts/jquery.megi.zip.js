(function () {

    var mZip = (function () {

        var mZip = function () {

            var that = this;

            //主入口函数
            this.zip = function (data, format) {

                if (data === undefined || data == null) return null;

                format = format || "gzip";

                var bufBody = that.getZipUintArray(data);

                var dataBuffer;

                switch (format) {
                    case 'gzip':
                        dataBuffer = window.pako.gzip(bufBody);
                        break;
                    case 'deflate':
                        dataBuffer = window.pako.deflate(bufBody);
                        break;
                    case 'deflate-raw':
                        dataBuffer = window.pako.deflateRaw(bufBody);
                        break;
                }

                return that.toBase64(dataBuffer);
            }

            //将Uint8Array转成字符串
            this.toBase64 = function (uint8Array) {

                var totalString = "";
                var step = 102400;

                for (var i = 0; i * step < uint8Array.length ; i++) {

                    var str = String.fromCharCode.apply(null, uint8Array.slice(step * i, (i + 1) * step));
                    totalString += str;
                }

                var result = btoa(totalString);

                return result;
            };

            //将要压缩的对象，转化成要缩后的数组
            this.getZipUintArray = function (data) {

                var text = data;

                if (typeof data !== "string")
                    text = JSON.stringify(data);

                text = encodeURIComponent(text);

                var bufferBody = new Uint8Array(text.length);

                for (var i = 0; i < text.length ; i++) {
                    bufferBody[i] = text.charCodeAt(i);
                }

                return bufferBody;
            }
        }

        return mZip;
    })();

    $.extend(mZip, {
        zip: function (data, format) {
            return new mZip().zip(data, format);
        }
    })

    $.mZip = window.mZip = mZip;
})()