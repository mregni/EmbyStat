import { Card } from '../common/card';
import { UserMediaView } from '../session/user-media-view';
import { MediaServerUserAccessSchedule } from './media-server-user-access-schedule';

export class MediaServerUser {
  id: string;
  name: string;
  serverId: string;
  lastLoginDate: Date;
  lastActivityDate: Date;
  subtitleMode: string;
  isAdministrator: boolean;
  isHidden: boolean;
  isDisabled: boolean;
  maxParentalRating: number;
  accessSchedules: MediaServerUserAccessSchedule[];
  invalidLoginAttemptCount: number;
  remoteClientBitRateLimit: number;
  deleted: boolean;
  primaryImageTag: string;
  viewedMovieCount: Card<number>;
  viewedEpisodeCount: Card<number>;
  lastWatchedMedia: UserMediaView[];
}
