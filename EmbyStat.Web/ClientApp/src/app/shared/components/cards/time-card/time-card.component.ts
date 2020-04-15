import { Component, Input } from '@angular/core';

import { TimespanCard } from '../../../models/common/timespan-card';

@Component({
  selector: 'es-time-card',
  templateUrl: './time-card.component.html',
  styleUrls: ['../number-card/number-card.component.scss']
})
export class TimeCardComponent {
  @Input() card: TimespanCard;
}
