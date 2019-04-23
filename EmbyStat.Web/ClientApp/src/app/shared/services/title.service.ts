import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TitleService {
  titleSubject = new BehaviorSubject<string>('');

  updateTitle(title: string){
    this.titleSubject.next(title);
  }
}
