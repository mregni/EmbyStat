import { ShortMovie, SuspiciousMovie } from ".";
import { Card, Chart, TopCard } from "../common";
import { PersonStatistics } from "../person";

export interface MovieStatistics {
  cards: Card[];
  topCards: TopCard[],
  charts: Chart[];
  people: PersonStatistics;
  shorts: ShortMovie[];
  noImdb: SuspiciousMovie[];
  noPrimary: SuspiciousMovie[];
}
