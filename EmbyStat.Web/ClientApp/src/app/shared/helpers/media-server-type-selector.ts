export class MediaServerTypeSelector {
  public static getServerTypeString(type: number): string {
    return type === 0 ? 'Emby' : 'Jellyfin';
  }

  public static getOtherServerTypeString(type: number): string {
    return type === 1 ? 'Emby' : 'Jellyfin';
  }

  public static getServerApiPage(type: number): string {
    return type === 0 ? 'apikeys' : 'apikeys.html';
  }
}
