export module MediaServerTypeSelector {
    export function getServerTypeString(type: number): string {
        return type === 0 ? 'Emby' : 'Jellyfin';
    }
}
