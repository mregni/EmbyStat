export interface MediaServerUdpBroadcast {
  address: string;
  id: string;
  name: string;
  port: number;
  protocol: 0 | 1;
  type: 0 | 1;
  baseUrl: string;
}
