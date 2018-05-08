"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var AppActionTypes;
(function (AppActionTypes) {
    AppActionTypes["ERROR"] = "[Error] Effect Error";
    AppActionTypes["NOOP"] = "[App] NOOP";
    AppActionTypes["RESET_ALL_LOAD_STATES"] = "[App] Reset all load states";
})(AppActionTypes = exports.AppActionTypes || (exports.AppActionTypes = {}));
var EffectError = /** @class */ (function () {
    function EffectError(payload) {
        if (payload === void 0) { payload = null; }
        this.type = AppActionTypes.ERROR;
    }
    return EffectError;
}());
exports.EffectError = EffectError;
var NoopAction = /** @class */ (function () {
    function NoopAction() {
        this.type = AppActionTypes.NOOP;
    }
    return NoopAction;
}());
exports.NoopAction = NoopAction;
//# sourceMappingURL=app.actions.js.map