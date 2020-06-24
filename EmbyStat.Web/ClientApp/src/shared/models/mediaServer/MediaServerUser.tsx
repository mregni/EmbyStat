export interface Card<T> {
  title: string;
  value: T;
}

export interface UserMediaView {
  id: string;
  banner: string;
  primary: string;
  name: string;
  parentId: string;
  productionYear: number;
  sortName: string;
  watchedPercentage: number;
  watchedTime: number;
  startedWatching: Date;
  endedWatching: Date;
  deviceId: string;
  deviceLogo: string;
}

export interface MediaServerUserAccessSchedule {
  id: string;
  dayOfWeek: string;
  startHour: number;
  endHour: number;
}

export interface MediaServerUser {
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
