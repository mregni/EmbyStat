import {Card, Chart, TopCard} from '../common';

export interface SuspiciousMovie {
  mediaId: string;
  title: string;
  number: number;
}

export interface ShortMovie {
  number: number;
  mediaId: string;
  title: string;
  duration: number;
}

export interface MovieStatistics {
  cards: Card[];
  topCards: TopCard[];
  charts: Chart[];
  shorts: ShortMovie[];
  noImdb: SuspiciousMovie[];
  noPrimary: SuspiciousMovie[];
}
