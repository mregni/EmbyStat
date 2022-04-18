export interface MediaServerUser {
  id: string;
  name: string;
  lastLoginDate: Date;
  lastActivityDate: Date;
  isAdministrator: boolean;
  isHidden: boolean;
  isDisabled: boolean;
  primaryImageTag: string;
  totalPlayCount: string;
}
