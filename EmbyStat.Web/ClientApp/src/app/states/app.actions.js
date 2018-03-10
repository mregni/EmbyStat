"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var AppActionTypes;
(function (AppActionTypes) {
    AppActionTypes["ERROR"] = "[Error] Effect Error";
})(AppActionTypes = exports.AppActionTypes || (exports.AppActionTypes = {}));
var EffectError = /** @class */ (function () {
    function EffectError(payload) {
        if (payload === void 0) { payload = null; }
        this.type = AppActionTypes.ERROR;
    }
    return EffectError;
}());
exports.EffectError = EffectError;
//# sourceMappingURL=app.actions.js.map