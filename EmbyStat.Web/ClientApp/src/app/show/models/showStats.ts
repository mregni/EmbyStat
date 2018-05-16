import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespanCard';
import { ShowPoster } from '../../shared/models/showPoster';

export class ShowStats {
  public showCount: Card;
  public episodeCount: Card;
  public missingEpisodeCount: Card;
  public totalPlayableTime: TimespanCard;
  public highestRatedShow: ShowPoster;
  public lowestRatedShow: ShowPoster;
  public showWithMostEpisodes: ShowPoster;
  public oldestPremieredShow: ShowPoster;
  public youngestPremieredShow: ShowPoster;
  public youngestAddedShow: ShowPoster;
}
