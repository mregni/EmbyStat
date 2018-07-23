export class EmbyLogin {
  public userName: string;
  public address: string;
  public password: string;

  constructor(username: string, password: string, address: string) {
    this.userName = username;
    this.password = password;
    this.address = address;
  }
}
