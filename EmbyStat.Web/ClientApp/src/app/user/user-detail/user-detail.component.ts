import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';

import { EmbyService } from '../../shared/services/emby.service';
import { EmbyUser } from '../../shared/models/emby/emby-user';

@Component({
  selector: 'user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy {
  private paramSub: Subscription;

  user$: Observable<EmbyUser>;

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly embyService: EmbyService) {
    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        console.log(id);
        this.user$ = this.embyService.getUserById(id);
      } else {
        this.router.navigate(['/users']);
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }
  }
}
