"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ServerActionTypes;
(function (ServerActionTypes) {
    ServerActionTypes["LOAD_SERVERINFO"] = "[Server] Load ServerInfo";
    ServerActionTypes["LOAD_SERVERINFO_SUCCESS"] = "[Server] Load ServerInfo Success";
})(ServerActionTypes = exports.ServerActionTypes || (exports.ServerActionTypes = {}));
var LoadServerInfoAction = /** @class */ (function () {
    function LoadServerInfoAction(payload) {
        if (payload === void 0) { payload = null; }
        this.payload = payload;
        this.type = ServerActionTypes.LOAD_SERVERINFO;
    }
    return LoadServerInfoAction;
}());
exports.LoadServerInfoAction = LoadServerInfoAction;
var LoadServerInfoSuccessAction = /** @class */ (function () {
    function LoadServerInfoSuccessAction(payload) {
        this.payload = payload;
        this.type = ServerActionTypes.LOAD_SERVERINFO_SUCCESS;
    }
    return LoadServerInfoSuccessAction;
}());
exports.LoadServerInfoSuccessAction = LoadServerInfoSuccessAction;
//# sourceMappingURL=actions.server.js.map