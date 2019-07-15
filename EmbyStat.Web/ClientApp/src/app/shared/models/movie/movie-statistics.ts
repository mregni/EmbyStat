import { PersonStatistics } from '../common/person-statistics';
import { GeneralMovieStatistics } from './general-movie-statistics';
import { MovieCharts } from './movie-charts';
import { MovieSuspiciousContainer } from './movie-suspicious-container';

export class MovieStatistics {
    general: GeneralMovieStatistics;
    charts: MovieCharts;
    people: PersonStatistics;
    suspicious: MovieSuspiciousContainer;
}
