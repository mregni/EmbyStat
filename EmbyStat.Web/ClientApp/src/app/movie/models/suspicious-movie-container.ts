import { Duplicate } from './duplicate';
import { ShortMovie } from './short-movie';
import { SuspiciousMovie } from './suspicious-movie';

export class SuspiciousMovieContainer {
  duplicates: Duplicate[];
  shorts: ShortMovie[];
  noImdb: SuspiciousMovie[];
  noPrimary: SuspiciousMovie[];
}
