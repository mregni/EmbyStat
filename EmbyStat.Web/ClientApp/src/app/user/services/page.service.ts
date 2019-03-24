import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class PageService {
  page = new BehaviorSubject<string>(undefined);

  pageChanged(page: string) {
    this.page.next(page);
  }
}
