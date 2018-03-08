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
var configuration_actions_1 = require("./configuration.actions");
var INITIAL_STATE = {
    wizardFinished: false,
    accessToken: "",
    embyUserName: "",
    embyServerAddress: "",
    username: "",
    userId: "",
    language: ""
};
function configurationReducer(state, action) {
    if (state === void 0) { state = INITIAL_STATE; }
    switch (action.type) {
        case configuration_actions_1.ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS:
            return __assign({}, state, { language: action.payload.language, wizardFinished: action.payload.wizardFinished });
        default:
            return state;
    }
}
exports.configurationReducer = configurationReducer;
var ConfigurationQuery;
(function (ConfigurationQuery) {
    ConfigurationQuery.getConfiguration = function (state) { return state.configuration; };
})(ConfigurationQuery = exports.ConfigurationQuery || (exports.ConfigurationQuery = {}));
//# sourceMappingURL=configuration.reducer.js.map