import { PersonStatistics } from '../common/person-statistics';
import { GeneralShowStatistics } from './general-show-statistics';
import { ShowCharts } from './show-charts';
import { ShowCollectionRow } from './show-collection-row';

export class ShowStatistics {
    general: GeneralShowStatistics;
    charts: ShowCharts;
    people: PersonStatistics;
}
