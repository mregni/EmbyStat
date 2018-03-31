"use strict";
var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
Object.defineProperty(exports, "__esModule", { value: true });
var serverInfo_1 = require("../models/serverInfo");
var actions_server_1 = require("./actions.server");
var INITIAL_STATE = {
    serverInfo: new serverInfo_1.ServerInfo(),
    isLoaded: false
};
function serverInfoReducer(state, action) {
    if (state === void 0) { state = INITIAL_STATE; }
    switch (action.type) {
        case actions_server_1.ServerActionTypes.LOAD_SERVERINFO_SUCCESS:
            return __assign({}, state, { serverInfo: action.payload, isLoaded: true });
        default:
            return state;
    }
}
exports.serverInfoReducer = serverInfoReducer;
var ServerQuery;
(function (ServerQuery) {
    ServerQuery.getServerInfo = function (state) { return state.serverInfo.serverInfo; };
    ServerQuery.getLoaded = function (state) { return state.serverInfo.isLoaded; };
})(ServerQuery = exports.ServerQuery || (exports.ServerQuery = {}));
//# sourceMappingURL=reducer.server.js.map