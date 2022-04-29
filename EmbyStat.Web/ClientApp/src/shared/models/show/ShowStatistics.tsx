import {Card, Chart, TopCard} from '../common';

export interface ShowStatistics {
  cards: Card[];
  topCards: TopCard[];
  barCharts: Chart[];
  pieCharts: Chart[];
}
