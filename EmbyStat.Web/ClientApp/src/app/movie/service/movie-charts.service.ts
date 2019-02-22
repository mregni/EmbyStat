import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()

export class MovieChartsService {

  private opened = new BehaviorSubject<boolean>(false);
  open = this.opened.asObservable();

  constructor() { }

  changeOpened(open) {
    this.opened.next(open);
  }
}
