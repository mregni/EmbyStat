import { Component, Input, OnInit } from '@angular/core';

import { TimespanCard } from '../../../models/common/timespan-card';

@Component({
  selector: 'app-time-card',
  templateUrl: './time-card.component.html',
  styleUrls: ['./time-card.component.scss']
})
export class TimeCardComponent implements OnInit {
  @Input() card: TimespanCard;

  constructor() { }

  ngOnInit() {
  }

}
