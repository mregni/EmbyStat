export class EmbyUser {
  id: string;
  name: string;
  lastLoginDate: Date;
  lastActivityDate: Date;
  isAdministrator: boolean;
  isHidden: boolean;
  isDisabled: boolean;
  deleted: boolean;
  primaryImageTag: string;
}
