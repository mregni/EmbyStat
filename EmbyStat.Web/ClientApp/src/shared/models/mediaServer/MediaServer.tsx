export interface MediaServer {
  address: string;
  port: number | null;
  protocol: 0 | 1;
  baseUrlNeeded: boolean;
  baseUrl: string;
  apiKey: string;
  type: 0 | 1;
  name: string | null;
  id: string | null;
}