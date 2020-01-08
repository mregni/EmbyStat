import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { ApplicationState } from '../../../states/app.state';
import { EmbyServerActionTypes, EmbyServerInfoActions } from './emby-server.actions';

const INITIAL_STATE: ServerInfo = {
    id: '',
    systemUpdateLevel: 0,
    operatingSystemDisplayName: '',
    hasPendingRestart: false,
    isShuttingDown: false,
    supportsLibraryMonitor: false,
    webSocketPortNumber: '',
    canSelfRestart: false,
    canSelfUpdate: false,
    canLaunchWebBrowser: false,
    programDataPath: '',
    itemsByNamePath: '',
    cachePath: '',
    logPath: '',
    internalMetadataPath: '',
    transcodingTempPath: '',
    httpServerPortNumber: 0,
    supportsHttps: false,
    httpsPortNumber: '',
    hasUpdateAvailable: false,
    supportsAutoRunAtStartup: false,
    hardwareAccelerationRequiresPremiere: false,
    localAddress: '',
    wanAddress: '',
    serverName: '',
    version: '',
    operatingSystem: '',
    isLoaded: false
};

export function embyServerInfoReducer(state: ServerInfo = INITIAL_STATE, action: EmbyServerInfoActions) {
    switch (action.type) {
        case EmbyServerActionTypes.LOAD_EMBY_SERVER_INFO_SUCCESS:
            return {
                ...state,
                id: action.payload.id,
                systemUpdateLevel: action.payload.systemUpdateLevel,
                operatingSystemDisplayName: action.payload.operatingSystemDisplayName,
                hasPendingRestart: action.payload.hasPendingRestart,
                isShuttingDown: action.payload.isShuttingDown,
                supportsLibraryMonitor: action.payload.supportsLibraryMonitor,
                webSocketPortNumber: action.payload.webSocketPortNumber,
                canSelfRestart: action.payload.canSelfRestart,
                canSelfUpdate: action.payload.canSelfUpdate,
                canLaunchWebBrowser: action.payload.canLaunchWebBrowser,
                programDataPath: action.payload.programDataPath,
                itemsByNamePath: action.payload.itemsByNamePath,
                cachePath: action.payload.cachePath,
                logPath: action.payload.logPath,
                internalMetadataPath: action.payload.internalMetadataPath,
                transcodingTempPath: action.payload.transcodingTempPath,
                httpServerPortNumber: action.payload.httpServerPortNumber,
                supportsHttps: action.payload.supportsHttps,
                httpsPortNumber: action.payload.httpsPortNumber,
                hasUpdateAvailable: action.payload.hasUpdateAvailable,
                supportsAutoRunAtStartup: action.payload.supportsAutoRunAtStartup,
                hardwareAccelerationRequiresPremiere: action.payload.hardwareAccelerationRequiresPremiere,
                localAddress: action.payload.localAddress,
                wanAddress: action.payload.wanAddress,
                serverName: action.payload.serverName,
                version: action.payload.version,
                operatingSystem: action.payload.operatingSystem,
                isLoaded: true
            };
        default:
            return state;
    }
}

export namespace EmbyServerInfoQuery {
    export const getInfo = (state: ApplicationState) => state.embyServerInfo;
    export const getLoaded = (state: ApplicationState) => state.embyServerInfo.isLoaded;
}
