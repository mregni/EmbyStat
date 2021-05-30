import { JwtPayload } from 'jwt-decode';

export interface JwtPayloadCustom extends JwtPayload {
  roles: string[];
}