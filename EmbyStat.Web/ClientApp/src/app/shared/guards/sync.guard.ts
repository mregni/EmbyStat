import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { MatDialog } from '@angular/material';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { SyncIsRunningDialog } from '../dialogs/sync-is-running/sync-is-running.component';
import { JobService } from '../../jobs/service/job.service';
import { Job } from '../../jobs/models/job';

@Injectable()
export class SyncGuard implements CanActivate {
  constructor(
    private router: Router,
    private jobService: JobService,
    private dialog: MatDialog) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.jobService.getMediaSyncJob().pipe(
      map((job: Job) => {
      if (job.state === 1) {
        this.dialog.open(SyncIsRunningDialog,
          {
            width: '550px'
          });
        return false;
      }

      return true;
    }));
  }
}
