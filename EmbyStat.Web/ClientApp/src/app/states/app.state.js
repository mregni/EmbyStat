"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ngrx_store_freeze_1 = require("ngrx-store-freeze");
var environment_1 = require("../../environments/environment");
var reducer_configuration_1 = require("../configuration/state/reducer.configuration");
var reducer_plugin_1 = require("../plugin/state/reducer.plugin");
var reducer_server_1 = require("../server/state/reducer.server");
exports.ROOT_REDUCER = {
    configuration: reducer_configuration_1.configurationReducer,
    plugins: reducer_plugin_1.pluginReducer,
    serverInfo: reducer_server_1.serverInfoReducer
};
exports.META_REDUCERS = !environment_1.environment.production ? [ngrx_store_freeze_1.storeFreeze] : [];
//# sourceMappingURL=app.state.js.map