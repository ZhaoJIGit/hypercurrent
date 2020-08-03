
var mObject = window.mObject || {};

mObject.setPropertyValue = function (obj, name, value) {
    obj = obj || {};
    obj[name] = value;
    return obj;
}

mObject.getPropertyValue = function (obj, name) {
    obj = obj || {};
    return obj[name];
}

mObject.hasOwnProperty = function (obj, name) {
    return obj.hasOwnProperty(name);
}