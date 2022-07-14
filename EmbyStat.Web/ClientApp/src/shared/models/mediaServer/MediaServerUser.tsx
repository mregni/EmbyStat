import {Card} from '../common';

export interface MediaServerUser {
  id: string;
  name: string;
  isAdministrator: boolean;
}

export interface MediaServerUserDetails extends MediaServerUser {
  id: string;
  name: string;
  primaryImageTag: string;
  lastLoginDate: string | null;
  lastActivityDate: string | null;
  subtitleLanguagePreference: string;
  serverId: string;
  isAdministrator: boolean;
  isHidden: boolean;
  isHiddenRemotely: boolean;
  isHiddenFromUnusedDevices: boolean;
  isDisabled: boolean;
  enableLiveTvAccess: boolean;
  enableContentDeletion: boolean;
  enableContentDownloading: boolean;
  nableSubtitleDownloading: boolean;
  enableSubtitleManagement: boolean;
  enableSyncTranscoding: boolean;
  enableMediaConversion: boolean;
  invalidLoginAttemptCount: number;
  enablePublicSharing: boolean;
  remoteClientBitrateLimit: number;
  simultaneousStreamLimit: number;
  enableAllDevices: boolean;
  viewedMovieCount: Card;
  viewedEpisodeCount: Card;
}
