import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class PageService {
  page = new BehaviorSubject<string>('details');

  pageChanged(page: string) {
    this.page.next(page);
  }
}
