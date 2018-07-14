import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()

export class WizardStateService {

  public finished = new BehaviorSubject<boolean>(true);

  constructor() { }

  changeState(value) {
    this.finished.next(value);
  }
}
