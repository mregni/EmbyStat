import { Card } from '../common/card';
import { TimespanCard } from '../common/timespan-card';
import { ShowPoster } from './show-poster';

export class GeneralShowStatistics {
  showCount: Card<number>;
  episodeCount: Card<number>;
  missingEpisodeCount: Card<number>;
  totalPlayableTime: TimespanCard;
  highestRatedShow: ShowPoster;
  lowestRatedShow: ShowPoster;
  showWithMostEpisodes: ShowPoster;
  oldestPremieredShow: ShowPoster;
  newestPremieredShow: ShowPoster;
  latestAddedShow: ShowPoster;
}
