mContext = {

    getContext: function () {
        
        var inputs = $("input[type='hidden'][context='1']");

        var context = {};

        for (var i = 0; i < inputs.length; i++) {

            var value = inputs.eq(i).val();

            context[inputs.eq(i).attr("name")] = value;

        }

        return context;
    }
}