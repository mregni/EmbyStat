"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var PluginActionTypes;
(function (PluginActionTypes) {
    PluginActionTypes["LOAD_PLUGINS"] = "[Plugin] Load Plugins";
    PluginActionTypes["LOAD_PLUGINS_SUCCESS"] = "[Plugin] Load Plugins Success";
})(PluginActionTypes = exports.PluginActionTypes || (exports.PluginActionTypes = {}));
var LoadPluginAction = /** @class */ (function () {
    function LoadPluginAction(payload) {
        if (payload === void 0) { payload = null; }
        this.payload = payload;
        this.type = PluginActionTypes.LOAD_PLUGINS;
    }
    return LoadPluginAction;
}());
exports.LoadPluginAction = LoadPluginAction;
var LoadPluginSuccessAction = /** @class */ (function () {
    function LoadPluginSuccessAction(payload) {
        this.payload = payload;
        this.type = PluginActionTypes.LOAD_PLUGINS_SUCCESS;
    }
    return LoadPluginSuccessAction;
}());
exports.LoadPluginSuccessAction = LoadPluginSuccessAction;
//# sourceMappingURL=actions.plugin.js.map