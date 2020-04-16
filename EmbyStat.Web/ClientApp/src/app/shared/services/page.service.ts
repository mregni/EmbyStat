import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

@Injectable()
export class PageService {
  page = new BehaviorSubject<string>('detail');

  pageChanged(page: string): void {
    this.page.next(page);
  }
}
