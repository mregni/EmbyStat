"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ConfigurationActionTypes;
(function (ConfigurationActionTypes) {
    ConfigurationActionTypes["LOAD_CONFIGURATION"] = "[Configuration] Load Configuration";
    ConfigurationActionTypes["LOAD_CONFIGURATION_SUCCESS"] = "[Configuration] Load Configuration Success";
})(ConfigurationActionTypes = exports.ConfigurationActionTypes || (exports.ConfigurationActionTypes = {}));
var LoadConfigurationAction = /** @class */ (function () {
    function LoadConfigurationAction(payload) {
        if (payload === void 0) { payload = null; }
        this.payload = payload;
        this.type = ConfigurationActionTypes.LOAD_CONFIGURATION;
    }
    return LoadConfigurationAction;
}());
exports.LoadConfigurationAction = LoadConfigurationAction;
var LoadConfigurationSuccessAction = /** @class */ (function () {
    function LoadConfigurationSuccessAction(payload) {
        this.payload = payload;
        this.type = ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS;
    }
    return LoadConfigurationSuccessAction;
}());
exports.LoadConfigurationSuccessAction = LoadConfigurationSuccessAction;
//# sourceMappingURL=configuration.actions.js.map