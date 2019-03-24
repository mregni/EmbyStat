import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { ChangeDetectorRef } from '@angular/core';

import { EmbyService } from '../../shared/services/emby.service';
import { PageService } from '../services/page.service';

import { EmbyUser } from '../../shared/models/emby/emby-user';
import { UserId } from '../../shared/models/user-id';

import { UserService } from '../services/user.service';

@Component({
  selector: 'user-container',
  templateUrl: './user-container.component.html',
  styleUrls: ['./user-container.component.scss']
})
export class UserContainerComponent implements OnInit, OnDestroy {
  private paramSub: Subscription;
  private userSub: Subscription;
  private pageSub: Subscription;

  userIds$: Observable<UserId[]>;
  selectedUserId: string;
  selectedPage = "details";

  constructor(private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly embyService: EmbyService,
    private readonly userService: UserService,
    private readonly pageService: PageService,
    private readonly cdRef: ChangeDetectorRef) {
    this.userIds$ = this.embyService.getUserIdList();

    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        this.embyService.getUserById(id).subscribe((user: EmbyUser) => {
          this.userService.userChanged(user);
          this.selectedUserId = user.id;
        });
      } else {
        this.router.navigate(['/users']);
      }
    });
  }

  onUserSelectionChanged(event: any) {
    this.embyService.getUserById(event.value).subscribe((user: EmbyUser) => {
      this.userService.userChanged(user);
      this.selectedUserId = user.id;
    });
  }

  onPageSelectionChanged(event: any) {
    this.router.navigate(['user/' + this.selectedUserId + '/' + event.value]);
  }

  ngOnInit() {
    this.pageSub = this.pageService.page.subscribe((page: string) => {
      this.selectedPage = page;
      this.cdRef.detectChanges();
    });
  }

  ngOnDestroy(): void {
    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }

    if (this.userSub !== undefined) {
      this.userSub.unsubscribe();
    }

    if (this.pageSub !== undefined) {
      this.pageSub.unsubscribe();
    }
  }
}
