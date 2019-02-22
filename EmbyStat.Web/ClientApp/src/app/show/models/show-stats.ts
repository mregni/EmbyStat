import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespan-card';
import { ShowPoster } from '../../shared/models/show-poster';

export class ShowStats {
  showCount: Card<number>;
  episodeCount: Card<number>;
  missingEpisodeCount: Card<number>;
  totalPlayableTime: TimespanCard;
  highestRatedShow: ShowPoster;
  lowestRatedShow: ShowPoster;
  showWithMostEpisodes: ShowPoster;
  oldestPremieredShow: ShowPoster;
  youngestPremieredShow: ShowPoster;
  youngestAddedShow: ShowPoster;
}
