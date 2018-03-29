"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ConfigurationActionTypes;
(function (ConfigurationActionTypes) {
    ConfigurationActionTypes["LOAD_PLUGINS"] = "[Plugin] Load Plugins";
    ConfigurationActionTypes["LOAD_PLUGINS_SUCCESS"] = "[Plugin] Load Plugins Success";
})(ConfigurationActionTypes = exports.ConfigurationActionTypes || (exports.ConfigurationActionTypes = {}));
var LoadPluginAction = /** @class */ (function () {
    function LoadPluginAction(payload) {
        if (payload === void 0) { payload = null; }
        this.payload = payload;
        this.type = ConfigurationActionTypes.LOAD_PLUGINS;
    }
    return LoadPluginAction;
}());
exports.LoadPluginAction = LoadPluginAction;
var LoadPluginSuccessAction = /** @class */ (function () {
    function LoadPluginSuccessAction(payload) {
        this.payload = payload;
        this.type = ConfigurationActionTypes.LOAD_PLUGINS_SUCCESS;
    }
    return LoadPluginSuccessAction;
}());
exports.LoadPluginSuccessAction = LoadPluginSuccessAction;
//# sourceMappingURL=actions.plugin.js.map