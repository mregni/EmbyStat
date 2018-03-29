"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ConfigurationActionTypes;
(function (ConfigurationActionTypes) {
    ConfigurationActionTypes["LOAD_CONFIGURATION"] = "[Configuration] Load Configuration";
    ConfigurationActionTypes["LOAD_CONFIGURATION_SUCCESS"] = "[Configuration] Load Configuration Success";
    ConfigurationActionTypes["UPDATE_CONFIGURATION"] = "[Configuration] Updated Configuration";
    ConfigurationActionTypes["UPDATE_CONFIGURATION_SUCCESS"] = "[Configuration] Updated Configuration Success";
    ConfigurationActionTypes["FIRE_SMALL_EMBY_SYNC"] = "[Configuration] Start Small Emby Sync";
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
var UpdateConfigurationAction = /** @class */ (function () {
    function UpdateConfigurationAction(payload) {
        this.payload = payload;
        this.type = ConfigurationActionTypes.UPDATE_CONFIGURATION;
    }
    return UpdateConfigurationAction;
}());
exports.UpdateConfigurationAction = UpdateConfigurationAction;
var UpdateConfigurationSuccessAction = /** @class */ (function () {
    function UpdateConfigurationSuccessAction(payload) {
        this.payload = payload;
        this.type = ConfigurationActionTypes.UPDATE_CONFIGURATION_SUCCESS;
    }
    return UpdateConfigurationSuccessAction;
}());
exports.UpdateConfigurationSuccessAction = UpdateConfigurationSuccessAction;
var FireSmallEmbySyncAction = /** @class */ (function () {
    function FireSmallEmbySyncAction(payload) {
        if (payload === void 0) { payload = null; }
        this.payload = payload;
        this.type = ConfigurationActionTypes.FIRE_SMALL_EMBY_SYNC;
    }
    return FireSmallEmbySyncAction;
}());
exports.FireSmallEmbySyncAction = FireSmallEmbySyncAction;
//# sourceMappingURL=actions.configuration.js.map