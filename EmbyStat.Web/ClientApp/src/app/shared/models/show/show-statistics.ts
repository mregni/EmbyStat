import { PersonStatistics } from '../common/person-statistics';
import { GeneralShowStatistics } from './general-show-statistics';
import { ShowCharts } from './show-charts';

export class ShowStatistics {
    general: GeneralShowStatistics;
    charts: ShowCharts;
    people: PersonStatistics;
}
