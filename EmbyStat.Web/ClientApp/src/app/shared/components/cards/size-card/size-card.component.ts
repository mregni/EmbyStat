import { CountUpOptions } from 'countup.js';
import { Card } from 'src/app/shared/models/common/card';

import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'es-size-card',
  templateUrl: './size-card.component.html',
  styleUrls: ['../number-card/number-card.component.scss']
})
export class SizeCardComponent implements OnInit {
  @Input() card: Card<number>;
  options: CountUpOptions;

  constructor() {
    this.options = {};
    this.options.startVal = 0;
    this.options.useEasing = true;
    this.options.separator = '';
    this.options.decimal = ',';
    this.options.duration = 4;
    this.options.decimalPlaces = 2;
   }

  ngOnInit() {
  }

  getFormattedValue(value: number): number {
    if (value < 1000) {
      return value;
    } else if (value < 1000000) {
      return value / 1024;
    } else if (value < 1000000000) {
      return value / 1024 / 1024;
    }
    return value / 1024 / 1024 / 1024;
  }

  getProperUnit(value: number): string {
    if (value < 1000) {
      return 'MB';
    } else if (value < 1000000) {
      return 'GB';
    } else if (value < 1000000000 ) {
      return 'TB';
    }
    return 'PB';
  }
}
