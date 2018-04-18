import { Component, Input } from '@angular/core';

import { Card } from '../../models/card';

@Component({
  selector: 'app-card-number',
  templateUrl: './card-number.component.html',
  styleUrls: ['./card-number.component.scss']
})
export class CardNumberComponent {
  @Input() card: Card;

}
