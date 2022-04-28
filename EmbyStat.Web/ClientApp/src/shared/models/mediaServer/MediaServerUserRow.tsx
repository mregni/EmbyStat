export interface MediaServerUserRow {
  id: string;
  name: string;
  lastActivityDate: string | null;
  isAdministrator: boolean;
  isHidden: boolean;
  isDisabled: boolean;
  movieViewCount: number;
  episodeViewCount: number;
  totalViewCount: number;
}
