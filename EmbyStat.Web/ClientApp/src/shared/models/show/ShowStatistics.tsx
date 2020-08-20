import { Card, Chart, TopCard } from '../common';
import { PersonStatistics } from '../person';

export interface ShowStatistics {
  cards: Card[];
  topCards: TopCard[];
  barCharts: Chart[];
  pieCharts: Chart[];
  people: PersonStatistics;
}