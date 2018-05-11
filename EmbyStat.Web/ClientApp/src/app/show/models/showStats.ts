import { Card } from '../../shared/models/card';
import { TimespanCard } from '../../shared/models/timespanCard';

export class ShowStats {
  public showCount: Card;
  public episodeCount: Card;
  public missingEpisodeCount: Card;
  public totalPlayableTime: TimespanCard;
}
