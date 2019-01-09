export class EmbyLogin {
  userName: string;
  address: string;
  password: string;

  constructor(username: string, password: string, address: string) {
    this.userName = username;
    this.password = password;
    this.address = address;
  }
}
