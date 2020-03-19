export module MediaServerTypeSelector {
    export function getServerTypeString(type: number): string {
        return type === 0 ? 'Emby' : 'Jellyfin';
    }

    export function getOtherServerTypeString(type: number): string {
      return type === 1 ? 'Emby' : 'Jellyfin';
    }
}
