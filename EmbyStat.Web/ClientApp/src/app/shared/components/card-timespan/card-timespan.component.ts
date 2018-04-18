import { Component, Input } from '@angular/core';

import { TimespanCard } from '../../models/timespanCard';

@Component({
  selector: 'app-card-timespan',
  templateUrl: './card-timespan.component.html',
  styleUrls: ['./card-timespan.component.scss']
})
export class CardTimespanComponent {
  @Input() card: TimespanCard;

}
