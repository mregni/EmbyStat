import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()

export class ShowChartsService {

  private opened = new BehaviorSubject<boolean>(false);
  open = this.opened.asObservable();

  constructor() { }

  changeOpened(open) {
    this.opened.next(open);
  }
}
