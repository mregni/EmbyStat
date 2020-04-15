import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class SideBarService {
  menuVisibleSubject = new BehaviorSubject<boolean>(true);

  closeMenu(): void {
    this.menuVisibleSubject.next(false);
  }

  openMenu(): void {
    this.menuVisibleSubject.next(true);
  }
}
