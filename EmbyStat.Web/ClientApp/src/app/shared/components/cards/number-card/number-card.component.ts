import { CountUpOptions } from 'countup.js';

import { Component, Input } from '@angular/core';

import { Card } from '../../../models/common/card';

@Component({
  selector: 'es-number-card',
  templateUrl: './number-card.component.html',
  styleUrls: ['./number-card.component.scss']
})
export class NumberCardComponent {
  @Input() card: Card<number>;
  options: CountUpOptions;

  constructor() {
    this.options = {};
    this.options.startVal = 0;
    this.options.useEasing = true;
    this.options.separator = '';
    this.options.decimal = ',';
    this.options.duration = 4;
  }
}
