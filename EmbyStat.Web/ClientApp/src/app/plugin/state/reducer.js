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
var actions_plugin_1 = require("./actions.plugin");
var INITIAL_STATE = [];
function configurationReducer(state, action) {
    if (state === void 0) { state = INITIAL_STATE; }
    switch (action.type) {
        case actions_plugin_1.PluginActionTypes.LOAD_PLUGINS_SUCCESS:
            return __assign({}, state);
        default:
            return state;
    }
}
exports.configurationReducer = configurationReducer;
var ConfigurationQuery;
(function (ConfigurationQuery) {
    ConfigurationQuery.getConfiguration = function (state) { return state.configuration; };
})(ConfigurationQuery = exports.ConfigurationQuery || (exports.ConfigurationQuery = {}));
//# sourceMappingURL=reducer.js.map