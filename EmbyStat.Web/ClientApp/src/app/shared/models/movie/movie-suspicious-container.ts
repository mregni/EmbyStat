import { Duplicate } from './duplicate';
import { ShortMovie } from './short-movie';
import { SuspiciousMovie } from './suspicious-movie';

export class MovieSuspiciousContainer {
  duplicates: Duplicate[];
  shorts: ShortMovie[];
  noImdb: SuspiciousMovie[];
  noPrimary: SuspiciousMovie[];
}
