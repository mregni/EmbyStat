export class MediaServerLogin {
  apiKey: string;
  address: string;

  constructor(apiKey: string, address: string) {
    this.apiKey = apiKey;
    this.address = address;
  }
}
