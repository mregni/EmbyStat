"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ngrx_store_freeze_1 = require("ngrx-store-freeze");
var environment_1 = require("../../environments/environment");
var configuration_reducer_1 = require("../configuration/state/configuration.reducer");
exports.ROOT_REDUCER = { configuration: configuration_reducer_1.configurationReducer };
exports.META_REDUCERS = !environment_1.environment.production ? [ngrx_store_freeze_1.storeFreeze] : [];
//# sourceMappingURL=app.state.js.map