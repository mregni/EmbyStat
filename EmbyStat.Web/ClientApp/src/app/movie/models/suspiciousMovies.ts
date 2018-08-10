import { Duplicate } from './duplicate';
import { ShortMovie } from './shortMovie';
import { SuspiciousMovie } from './suspiciousMovie';

export class SuspiciousMovies {
  duplicates: Duplicate[];
  shorts: ShortMovie[];
  noImdb: SuspiciousMovie[];
  noPrimary: SuspiciousMovie[];
}
