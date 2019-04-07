import { EmbyUserAccessSchedule } from './emby-user-access-schedule';
import { Card } from '../card';
import { UserMediaView } from '../session/user-media-view';
import { BarGraph } from '../graph/bar-graph';

export class EmbyUser {
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
  accessSchedules: EmbyUserAccessSchedule[];
  invalidLoginAttemptCount: number;
  remoteClientBitRateLimit: number;
  deleted: boolean;
  primaryImageTag: string;
  viewedMovieCount: Card<number>;
  viewedEpisodeCount: Card<number>;
  lastWatchedMedia: UserMediaView[];
  hoursPerDayGraph: BarGraph<number>;
}
